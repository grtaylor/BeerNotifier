module User.Index.Types

open Shared

type Model =
    { Users : Entities.User seq option }

    static member Empty =
        { Users = None }

type GetUsersResult =
| Success of Entities.User seq
| Error of exn

type Msg =
| GetUsers
| GetUsersResult of GetUsersResult