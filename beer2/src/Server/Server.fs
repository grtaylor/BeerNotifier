module Server

open System
open System.IO
open System.Security.Claims
open System.Threading.Tasks

open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.OpenIdConnect
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection

open FSharp.Control.Tasks.V2

open Giraffe
open Saturn

open Shared
open Saturn.Extensions.OpenIdAuth


let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us


let handleGetSecured =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let email = ctx.User.FindFirst ClaimTypes.NameIdentifier
        text (sprintf "User %s is authorized to access this resource." email.Value) next ctx

let securedRouter = router {
    pipe_through (Auth.requireAuthentication (ChallengeType.Custom OpenIdConnectDefaults.AuthenticationScheme))
    get "/secured" handleGetSecured
}

let handleGetPublic =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        text ("public access") next ctx

let topRouter = router {
    get "/" handleGetPublic
    forward "/secured" securedRouter
}

let configureSerialization (services:IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let openIdConfig =

    // todo - load from appsettings
    let clientId = "509363f2-0873-4f6f-8016-38fdde52634b"
    let tenantId = "64b0287b-af0d-4307-ae82-febfb154a6e6"
    let authority = sprintf "https://login.microsoftonline.com/%s" tenantId

    let fn = fun (o: OpenIdConnect.OpenIdConnectOptions) ->
        o.ClientId <- clientId
        o.Authority <- authority
        o.UseTokenLifetime <- true
        // maybe I don't need to set this because /signin-oidc is default per https://github.com/openiddict/openiddict-core/issues/35#issuecomment-162450400
        o.CallbackPath <- Microsoft.AspNetCore.Http.PathString("/signin-oidc")
        o.ResponseType <- "id_token"

    Action<OpenIdConnect.OpenIdConnectOptions>(fn)

// application { ... } connects to ASP.NET configuration interfaces under the hood - IWebHostBuilder, IServiceCollection, IApplicationBuilder
let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router topRouter
    memory_cache
    use_static publicPath
    service_config configureSerialization
    // we extended Saturn with SaturnExtensions.fs (name of the file does not matter)
    use_open_id_auth_with_config openIdConfig
    use_gzip
}

run app
