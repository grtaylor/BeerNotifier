module User.Index.Types

type UserInfo =
    { Id : int
      Name : string }

type Model =
    { Users : UserInfo list option }

    static member Empty =
        { Users = None }

type GetUsersResult =
| Success of UserInfo list
| Error of exn

type Msg =
| GetUsers
| GetUsersResult of GetUsersResult