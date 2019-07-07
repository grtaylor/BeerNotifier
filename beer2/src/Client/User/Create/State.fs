module User.Create.State

open Types

open Elmish

open System

let init _ : Model * Cmd<Msg> =
    Model.Empty, Cmd.none

let requiredField (value : string) =
    if String.IsNullOrWhiteSpace value then
        "This field is required"
    else
        ""

let validateModel model =
    let newModel =
        { model with UserNameError = requiredField model.UserName }

    let hasError =
        // UserName is required
        String.IsNullOrWhiteSpace model.UserName

    newModel, hasError

let update msg (model : Model) =
    match msg with
    | Submit ->
        match validateModel model with
        | newModel, true ->
            newModel, Cmd.none
        | newModel, false ->
            { newModel with IsWaitingOnServer = true },
                Cmd.OfPromise.either
                    Rest.createUser
                    (model.UserName)
                    (CreateUserResult.Success >> CreateUserResult)
                    (CreateUserResult.Error >> CreateUserResult)
    | CreateUserResult result ->
        match result with
        | CreateUserResult.Success createdUser ->
            { model with IsWaitingOnServer = false },
            Router.newUrl (Router.User.Show createdUser.Id |> Router.AuthPage.User |> Router.Page.AuthPage)
        | CreateUserResult.Error error ->
            Logger.debugfn "[User.Show.State] Error when creating user:\n %A" error
            { model with IsWaitingOnServer = false }, Cmd.none
