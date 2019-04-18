module Authentication

open Elmish

open Fable.Core.JsInterop
open Fable.Import.Browser
open Fable.PowerPack.Fetch


// not secrets, just don't need to be public
let private clientId = "509363f2-0873-4f6f-8016-38fdde52634b"
let private tenantId = "64b0287b-af0d-4307-ae82-febfb154a6e6"

// should be initialized once on UI startup
let context =
    let config = createEmpty<adal.Adal.Config>
    config.clientId <- clientId
    config.tenant <- Some tenantId
    config.postLogoutRedirectUri <- Some "https://google.com" //Some window.location.origin
    config.cacheLocation <- Some "localstorage"

    adal.Adal.createAuthContext(config)

let bearerHeader() =
    let mutable token = None
    let callback _ (t : string) : obj option =
        token <- if System.String.IsNullOrWhiteSpace t
                 then None
                 else Some t
        None

    context.acquireToken(clientId, callback)

    match token with
    | None -> requestHeaders []
    | Some t -> requestHeaders [ Authorization ("Bearer " + t) ]

let runWithAdal (authenticationContext : adal.Adal.AuthenticationContext) (program : Elmish.Program<_,_,_,_>) =
    authenticationContext.handleWindowCallback()

    if obj.ReferenceEquals(Fable.Import.Browser.window, Fable.Import.Browser.window.parent) &&
       obj.ReferenceEquals(Fable.Import.Browser.window, Fable.Import.Browser.window.top) &&
       (not (authenticationContext.isCallback(Fable.Import.Browser.window.location.hash))) then
        if isNull (authenticationContext.getCachedToken(authenticationContext.config.clientId)) || isNull (authenticationContext.getCachedUser()) then
            authenticationContext.login()
        else
            Program.run program
