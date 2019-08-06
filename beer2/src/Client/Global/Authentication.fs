module Authentication

open Elmish
open Elmish.React

open Fable.Core.JsInterop // provides !! operator

open Fable.React
open Fable.React.Props

open Fulma

open Fable.FontAwesome

open Shared


// not secrets, just don't need to be public
let [<Literal>] private ClientId = "509363f2-0873-4f6f-8016-38fdde52634b"
let [<Literal>] private TenantId = "64b0287b-af0d-4307-ae82-febfb154a6e6"

type User = {
    Name: string
    // Token: string
}

type Model =
| Loading
| NotAuthenticated
| Authenticated of User

type Msg =
| SignIn
| SignInResult of Result<User, exn>
| SignOut
| SignOutResult of Result<unit, exn>

let userAgentApplication =
    let authority = sprintf "https://login.microsoftonline.com/%s" TenantId

    let options =
        let cacheOptions = createEmpty<Msal.CacheOptions>
        cacheOptions.cacheLocation <- Some Msal.CacheLocation.LocalStorage

        let authOptions = createEmpty<Msal.AuthOptions>
        authOptions.authority <- Some authority
        authOptions.clientId <- ClientId
        // I don't know why type Fable.Core.U2 is necessary over just option.
        // authOptions.redirectUri <- Some (Fable.Core.Case1 WebRedirectUri)

        let o = createEmpty<Msal.Configuration>
        o.cache <- Some cacheOptions
        o.auth <- authOptions
        o

    Msal.UserAgentApplication.Create(options)

let getToken () = promise {
    let authParams = createEmpty<Msal.AuthenticationParameters>
    authParams.scopes <- Some !![| ClientId |]
    try
        let! authResponse = userAgentApplication.acquireTokenSilent authParams
        return authResponse.accessToken
    with error ->
        try
            Logger.errorfn "Error in getToken: %A" error
            // if error :? Msal.InteractionRequiredAuthError then
            let! authResponse = userAgentApplication.acquireTokenPopup authParams
            return authResponse.accessToken
            // else
            //     return reraise()
        with error ->
            return failwith "Please sign in using your Microsoft account."
}

let makeAuthHeader model =

    let getAuthHeader() = promise {
        let! token = getToken()
        return Fetch.Types.HttpRequestHeaders.Authorization ("Bearer " + token)
    }

    match model with
    | NotAuthenticated
    | Loading -> None
    | Authenticated _ -> Some (getAuthHeader())

let private signInPromise _ = promise {
    let authParams = createEmpty<Msal.AuthenticationParameters>
    authParams.scopes <- Some (ResizeArray [| "User.Read" |])
    let! authResponse = userAgentApplication.loginPopup authParams
    let! token = getToken()
    return { Name = userAgentApplication.getAccount().name } //; Token = token }
}

let private signOutPromise _ = promise {
    userAgentApplication.logout()
}

let update msg model =
    match msg with
    | SignIn -> model, Cmd.OfPromise.either signInPromise () (Ok >> SignInResult) (Error >> SignInResult)
    | SignInResult (Ok user) -> Authenticated user, Cmd.none
    | SignInResult (Error _) -> NotAuthenticated, Cmd.none
    | SignOut -> model, Cmd.OfPromise.either signOutPromise () (Ok >> SignOutResult) (Error >> SignOutResult)
    | SignOutResult  (Ok ()) -> NotAuthenticated, Cmd.none
    | SignOutResult  (Error _) -> NotAuthenticated, Cmd.none

let init () = Loading, Cmd.ofMsg SignIn

let root (model: Model option) dispatch =
    match model with
    | Some Loading ->
        Button.button [ Button.IsLoading true ] []

    | Some NotAuthenticated
    | None ->
        Button.button
            [ Button.OnClick (fun _e -> dispatch SignIn) ]
            [
                Icon.icon [] [ Fa.i [ Fa.Brand.Windows ] [] ]
                span [] [ str "Sign in" ]
            ]

    | Some (Authenticated user) ->
        Button.button
            [ Button.OnClick (fun _e -> dispatch SignOut) ]
            [
                Icon.icon [] [ Fa.i [ Fa.Brand.Windows ] [] ]
                span [] [ str (sprintf "%s | Sign out" user.Name) ]
            ]

