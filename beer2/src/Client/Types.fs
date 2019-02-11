module Client.Types

open Shared

// Mark things as optional that initially will not be available from the client.
// The initial value will be requested from server.
type Model =
    { CurrentPage : Router.Page
      // populate this with LDAP
      Session : Shared.Entities.User
      UserDispatcher : User.Dispatcher.Types.Model option }

    static member Empty =
        { CurrentPage = Router.Home
          Session = { Shared.Entities.User.UserName = "EmptyTest" }
          UserDispatcher = None }
        // { CurrentPage =
        //     Router.UserPage.Index
        //     |> Router.User
        //   Session =
        //     { Id = 1
        //       UserName = "testUserName" }
        //   UserDispatcher = None }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| UserDispatcherMsg of User.Dispatcher.Types.Msg

