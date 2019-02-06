module Client.State

open Elmish

open Fable.Import
open Fable.PowerPack.Fetch

open Thoth.Json

open Shared

open Types

let urlUpdate (result : Option<Router.Page>) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        model, Router.modifyUrl model.CurrentPage
    | Some page ->
        Browser.console.log(sprintf "Page is %A" page)
        let model = { model with CurrentPage = page }
        match page with
        | Router.User userPage ->
            let (subModel, subCmd) = User.Dispatcher.State.init userPage
            { model with UserDispatcher = Some subModel }, Cmd.map UserDispatcherMsg subCmd
        | Router.Home ->
            // model, Cmd.none
            let (subModel, subCmd) = User.Dispatcher.State.init Router.UserPage.Index
            { model with UserDispatcher = Some subModel }, Cmd.map UserDispatcherMsg subCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg, model with
    | (UserDispatcherMsg msg, { UserDispatcher = Some extractedModel }) ->
        let (subModel, subCmd) = User.Dispatcher.State.update model.Session msg extractedModel
        { model with UserDispatcher = Some subModel }, Cmd.map UserDispatcherMsg subCmd
    | (UserDispatcherMsg capturedMsg, _) ->
        Browser.console.log "[Client.State] discarded message"
        printfn "%A" capturedMsg
        model, Cmd.none

// defines the initial state and initial command (= side-effect) of the application
let init result =
    Browser.console.log (sprintf "[Client.State.init] %A" result)
    urlUpdate result Model.Empty