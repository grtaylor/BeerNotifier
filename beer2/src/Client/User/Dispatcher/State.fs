module User.Dispatcher.State

open Elmish

open Types

open Fable.Import

let init (userPage : Router.UserPage) =
    let model = { Model.Empty with CurrentPage = userPage }
    match userPage with
    | Router.UserPage.Index ->
        let (subModel, subCmd) = User.Index.State.init ()
        { model with IndexModel = Some subModel }, Cmd.map IndexMsg subCmd
    | Router.UserPage.Show userId ->
        let (subModel, subCmd) = User.Show.State.init userId
        { model with ShowModel = Some subModel }, Cmd.map ShowMsg subCmd
    | Router.UserPage.Create ->
        let (subModel, subCmd) = User.Create.State.init ()
        { model with CreateModel = Some subModel }, Cmd.map CreateMsg subCmd


let update user msg model =
    match msg, model with
    | IndexMsg msg, { IndexModel = Some extractedModel } ->
        let (subModel, subCmd) = User.Index.State.update msg extractedModel
        { model with IndexModel = Some subModel }, Cmd.map IndexMsg subCmd

    | ShowMsg msg, { ShowModel = Some extractedModel } ->
        let (subModel, subCmd) = User.Show.State.update user msg extractedModel
        { model with ShowModel = Some subModel }, Cmd.map ShowMsg subCmd

    | CreateMsg msg, { CreateModel = Some extractedModel } ->
        let (subModel, subCmd) = User.Create.State.update user msg extractedModel
        { model with CreateModel = Some subModel }, Cmd.map CreateMsg subCmd
    // do not wildcard the entire model so pattern matching will help you detect when you add a new Model type
    | IndexMsg _, { IndexModel = None }
    | ShowMsg _, { ShowModel = None }
    | CreateMsg _, { CreateModel = None } ->
        Browser.console.log "[User.Dispatcher.State] discarded message"
        model, Cmd.none
