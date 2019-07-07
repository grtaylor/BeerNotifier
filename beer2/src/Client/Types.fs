module Client.Types

open Shared

// Mark things as optional that initially will not be available from the client.
// The initial value will be requested from server.
type Model =
    { CurrentPage : Router.Page
      Session : Authentication.Model option
      UserDispatcher : User.Dispatcher.Types.Model option }

    static member Empty =
        { CurrentPage = Router.Page.Home
          Session = None
          UserDispatcher = None }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| AuthDispatcherMsg of Authentication.Msg
| UserDispatcherMsg of User.Dispatcher.Types.Msg

