module User.Index.Types

open Shared

type Model =
    { Users : Entities.User list option }

    static member Empty =
        { Users = None }

type GetUsersResult =
| Success of Entities.User list
| Error of exn

type Msg =
| GetUsers
| GetUsersResult of GetUsersResult