module User.About.State

open Types

open Elmish

let init userId =
    Model.Empty, Cmd.ofMsg (GetAboutInfo userId)

let update msg (model : Model) =
    match msg with
    | GetAboutInfo userId ->
        model, Cmd.ofPromise
                    Rest.getAboutInfo userId
                    GetAboutInfoResult
                    (GetAboutInfoResult.Error >> GetAboutInfoResult)
    | GetAboutInfoResult result ->
        match result with
        | GetAboutInfoResult.Success aboutInfo ->
            { model with Name = aboutInfo.UserName }, Cmd.none
        | GetAboutInfoResult.Error error ->
            Logger.debugfn "[User.About.State] Error when fetching aboutinfo:\n %A" error
            model, Cmd.none