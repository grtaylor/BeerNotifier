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
open Saturn.Extensions.OpenIdAuth

open Shared


let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

// todo - show current user's beer history data to them (last "deed of excellence", how many times they brought beer)
// and a functioning "volunteer to be next" button (which may be more for the UI to handle than the API)
let handleGetSecured =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        // yes ClaimTypes.Name is actually the email
        let email = ctx.User.FindFirst ClaimTypes.Name
        text (sprintf "User %s is authorized to access this resource." email.Value) next ctx

let apiRouter = router {
    not_found_handler (text "Api 404")

    pipe_through (Auth.requireAuthentication (ChallengeType.Custom OpenIdConnectDefaults.AuthenticationScheme))

    // any routes in this router are relative to http:localhost/port/api/
    // this get means http://localhost:port/api
    get "" handleGetSecured
    get "/" handleGetSecured

    forward "/users" Users.Controller.controller
}

let handleGetPublic =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        text ("public access") next ctx

let topRouter = router {
    get "/" handleGetPublic

    forward "/api" apiRouter
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
        // <website root>/signin-oidc is default per https://github.com/openiddict/openiddict-core/issues/35#issuecomment-162450400
        // make sure this path is set in portal.azure.com Azure AD Redirect URIs
        o.ResponseType <- "id_token"

    Action<OpenIdConnect.OpenIdConnectOptions>(fn)

// application { ... } connects to ASP.NET configuration interfaces under the hood - IWebHostBuilder, IServiceCollection, IApplicationBuilder
let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router topRouter
    memory_cache
    use_static publicPath
    // TODO - https://zaid-ajaj.github.io/Fable.Remoting/src/dependency-injection.html
    service_config configureSerialization
    // we extended Saturn with SaturnExtensions.fs (name of the file does not matter)
    use_open_id_auth_with_config openIdConfig
    use_gzip
    use_cors "localhost:8080"
        (fun builder -> builder.WithOrigins("http://localhost:8080")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowAnyOrigin() |> ignore)
}

run app
