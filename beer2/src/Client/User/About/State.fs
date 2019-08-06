module User.About.State

open Types

open Elmish

let init userId =
    Model.Empty, Cmd.ofMsg (GetAboutInfo userId)

let update (authModel: Authentication.Model) msg (model : Model) =
    match msg with
    | GetAboutInfo userId ->
        model, Cmd.OfPromise.either
                    // do I need to wrap getAboutInfo when I have all args?
                    (fun _ -> Rest.getAboutInfo userId authModel)
                    ()
                    GetAboutInfoResult
                    (GetAboutInfoResult.Error >> GetAboutInfoResult)
    | GetAboutInfoResult result ->
        match result with
        | GetAboutInfoResult.Success aboutInfo ->
            { model with Name = aboutInfo.UserName }, Cmd.none
        | GetAboutInfoResult.Error error ->
            Logger.debugfn "[User.About.State] Error when fetching aboutinfo:\n %A" error
            model, Cmd.none