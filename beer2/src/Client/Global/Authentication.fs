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

let private substringSafe (str : string) length =
    if str.Length >= length
    then str.Substring(0, length)
    else str.Substring(0)

let private substringTruncatedSafe str length =
    (substringSafe str length) + sprintf "... [truncated to length %i]" length

let private getCachedToken () =
    let t = context.getCachedToken(clientId)

    if System.String.IsNullOrWhiteSpace t
    then Logger.debug "cached token: no"; None
    else Logger.debugfn "cached token: yes -> %s" (substringTruncatedSafe t 5); Some t

let private acquireToken () =
    let mutable token = None
    let callback error (t : string) : obj option =
        if not (System.String.IsNullOrWhiteSpace t)
        then Logger.debugfn "ADAL.js error: %s" error

        token <- if System.String.IsNullOrWhiteSpace t
                 then None
                 else Some t
        None

    match getCachedToken () with
    | Some token -> Some token
    | None ->
        context.handleWindowCallback()
        context.acquireToken(clientId, callback)
        token

let tokenError () =
    context.getLoginError ()

let bearerHeader() =

    match acquireToken () with
    | None ->
    // this path seems to fire even when
    // 1. a cached token expires,
    // 2. adal.js gets a new token,
    // 3. the token is acceptable.
    // It is misleading when the console shows messages indicating "failure to get a token" while token api calls still work.
        Logger.errorfn "Could not acquire token because:\n %s" (tokenError())
        requestHeaders []
    | Some token ->
        requestHeaders [ Authorization (sprintf "Bearer %s" token) ]

let runWithAdal (authenticationContext : adal.Adal.AuthenticationContext) (program : Elmish.Program<_,_,_,_>) =
    authenticationContext.handleWindowCallback()

    if obj.ReferenceEquals(Fable.Import.Browser.window, Fable.Import.Browser.window.parent) &&
       obj.ReferenceEquals(Fable.Import.Browser.window, Fable.Import.Browser.window.top) &&
       (not (authenticationContext.isCallback(Fable.Import.Browser.window.location.hash))) then
        if isNull (authenticationContext.getCachedToken(authenticationContext.config.clientId)) || isNull (authenticationContext.getCachedUser()) then
            authenticationContext.login()
        else
            Program.run program
