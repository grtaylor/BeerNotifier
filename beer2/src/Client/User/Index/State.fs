module User.Index.State

open Types

open Elmish

let init () =
    Model.Empty, Cmd.ofMsg GetUsers

let update msg (model : Model) =
    match msg with
    | GetUsers ->
        model, Cmd.ofPromise Rest.getUsers () GetUsersResult (GetUsersResult.Error >> GetUsersResult)
    | GetUsersResult result ->
        match result with
        | GetUsersResult.Success users ->
            { model with Users = Some users }, Cmd.none
        | GetUsersResult.Error error ->
            Logger.debugfn "[User.Index.State] Error when fetching users:\n %A" error
            model, Cmd.none