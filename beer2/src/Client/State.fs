module Client.State

open Elmish

open Fable.Import
// open Fable.PowerPack.Fetch

open Thoth.Json

open Shared

open Router

open Types

let urlUpdate (result : Option<Router.Page>) model =
    match result with
    | None ->
        Browser.Dom.console.error("Error parsing url: " + Browser.Dom.window.location.href)
        model, Router.modifyUrl model.CurrentPage
    | Some page ->
        Browser.Dom.console.log(sprintf "Page is %A" page)
        let model = { model with CurrentPage = page }
        match page with
        | Page.AuthPage authPage ->
            match authPage with
            | AuthPage.User userPage ->
                let (subModel, subCmd) = User.Dispatcher.State.init userPage
                { model with UserDispatcher = Some subModel }, Cmd.map UserDispatcherMsg subCmd
        | Page.Home ->
            let (subModel, subCmd) = User.Dispatcher.State.init (Router.User.Show 42) // add ShowCurrentUser?
            { model with UserDispatcher = Some subModel }, Cmd.map UserDispatcherMsg subCmd

let private printAndDiscardMessage capturedMsg (model: Model) =
    Browser.Dom.console.log "[Client.State] discarded message"
    printfn "%A" capturedMsg
    model, Cmd.none

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (model : Model) : Model * Cmd<Msg> =
    match msg, model with
    | AuthDispatcherMsg msg, { Session = Some extractedModel } ->
        let (subModel, subCmd) = Authentication.update msg extractedModel
        { model with Session = Some subModel }, Cmd.map AuthDispatcherMsg subCmd

    // continue with the rest of the application when Authenticated
    | (UserDispatcherMsg msg, { UserDispatcher = Some extractedModel
                                Session = Some extractedSession }) ->
        let (subModel, subCmd) = User.Dispatcher.State.update extractedSession msg extractedModel
        { model with UserDispatcher = Some subModel }, Cmd.map UserDispatcherMsg subCmd

    /// handle not set state, like missing XyzDispatcher or Session
    | UserDispatcherMsg capturedMsg, ({ UserDispatcher = None } as model) ->
        printAndDiscardMessage capturedMsg model
    | AuthDispatcherMsg capturedMsg, ({ Session = None } as model) ->
        printAndDiscardMessage capturedMsg model

    /// other patterns that should be discarded
    | capturedMsg, ({ CurrentPage = _
                      Session = None
                      UserDispatcher = Some _ } as model) ->
        printAndDiscardMessage capturedMsg model

// defines the initial state and initial command (= side-effect) of the application
let init result =
    // Choose to require authentication before rendering parsing a url
    Authentication.init ()
    |> fun (authModel, authMsg) ->
        { Model.Empty with Session = Some authModel },
            Cmd.map AuthDispatcherMsg authMsg
        // Browser.Dom.console.log (sprintf "[Client.State.init] %A" result)
        // urlUpdate result Model.Empty
