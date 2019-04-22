module Users.About.Controller

open System.Security.Claims

open Shared.Entities

open Saturn

type HttpContext = Microsoft.AspNetCore.Http.HttpContext

let getAboutInfo userId (ctx : HttpContext) =

    // this could be better... maybe use router instead?
    // `subController` forces the http route to /:id/:controller_name
    // https://saturnframework.org/docs/api/controller/#subcontroller

    if userId = Shared.Entities.UserAboutInfo.SelfId then
        // yes ClaimTypes.Name is actually the email
        let email = ctx.User.FindFirst ClaimTypes.Name
        { UserAboutInfo.UserName = email.Value
          LastPurchaseDate = System.DateTime.UtcNow }
    else
        { UserAboutInfo.UserName = sprintf "TODO look up user with id %i" userId
          LastPurchaseDate = System.DateTime.UtcNow }
    |> Controller.json ctx

let controller userId = controller {
    // index (fun ctx -> getAboutInfo userId ctx) // same as next line
    index (getAboutInfo userId)
}