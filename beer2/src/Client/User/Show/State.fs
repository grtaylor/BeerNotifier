module User.Show.State

open Types

open Elmish

let init userId =
    Model.Empty userId, Cmd.ofMsg (GetDetails userId)

let update msg (model : Model) =
    match msg with
    | GetDetails userId ->
        model, Cmd.ofPromise Rest.getDetails userId (GetDetailsResult.Success >> GetDetailsResult) (GetDetailsResult.Error >> GetDetailsResult)

    | GetDetailsResult result ->
        match result with
        | GetDetailsResult.Success user ->
            { model with User = Some user }, Cmd.none
        | GetDetailsResult.Error error ->
            Logger.debugfn "[User.Show.State] Error when fetching details: \n %A" error
            { model with User = None }, Cmd.none
