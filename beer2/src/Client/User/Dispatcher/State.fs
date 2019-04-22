module User.Dispatcher.State

open Elmish

open Types

open Fable.Import
open Router

let init (userPage : Router.User) =
    let model = { Model.Empty with CurrentPage = userPage }
    let currentPageModel, currentPageCmd =
        match userPage with
        | User.Index ->
            let (subModel, subCmd) = User.Index.State.init ()
            { model with IndexModel = Some subModel }, Cmd.map IndexMsg subCmd
        | User.Show userId ->
            let (subModel, subCmd) = User.Show.State.init userId
            { model with ShowModel = Some subModel }, Cmd.map ShowMsg subCmd
        | User.Create ->
            let (subModel, subCmd) = User.Create.State.init ()
            { model with CreateModel = Some subModel }, Cmd.map CreateMsg subCmd
        | User.AboutMe ->
            let (subModel, subCmd) = User.About.State.init Shared.Entities.UserAboutInfo.SelfId
            { model with AboutModel = Some subModel }, Cmd.map AboutMsg subCmd

    currentPageModel, currentPageCmd

let update msg model =
    match msg, model with
    | IndexMsg msg, { IndexModel = Some extractedModel } ->
        let (subModel, subCmd) = User.Index.State.update msg extractedModel
        { model with IndexModel = Some subModel }, Cmd.map IndexMsg subCmd

    | ShowMsg msg, { ShowModel = Some extractedModel } ->
        let (subModel, subCmd) = User.Show.State.update msg extractedModel
        { model with ShowModel = Some subModel }, Cmd.map ShowMsg subCmd

    | CreateMsg msg, { CreateModel = Some extractedModel } ->
        let (subModel, subCmd) = User.Create.State.update msg extractedModel
        { model with CreateModel = Some subModel }, Cmd.map CreateMsg subCmd

    | AboutMsg msg, { AboutModel = Some extractedModel } ->
        let (subModel, subCmd) = User.About.State.update msg extractedModel
        { model with AboutModel = Some subModel }, Cmd.map AboutMsg subCmd
    // do not wildcard the entire model so pattern matching will help you detect when you add a new Model type
    // so, do not do this:
    // | AMsg _, _ ->
    //
    // XyzMsg can only apply when XyzModel is initialized to handle Msgs
    | IndexMsg _, { IndexModel = None }
    | ShowMsg _, { ShowModel = None }
    | CreateMsg _, { CreateModel = None }
    | AboutMsg _, { AboutModel = None } ->
        Browser.console.log "[User.Dispatcher.State] discarded message"
        model, Cmd.none
