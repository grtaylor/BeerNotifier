module User.Show.Types

type UserInfo =
    { Id : int
      Name : string }

type Model =
    { UserId : int
      User : UserInfo option }

    static member Empty userId =
        { UserId = userId
          User = None }

type GetDetailsResult =
| Success of UserInfo
| Error of exn

type Msg =
| GetDetails of int
| GetDetailsResult of GetDetailsResult