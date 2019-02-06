module User.Create.Types

type Model =
    // get some sort of active directory token
    { UserName : string
      UserNameError : string
      IsWaitingOnServer : bool }

    static member Empty =
        { UserName = ""
          UserNameError = ""
          IsWaitingOnServer = false }

type CreateUserResult =
| Success of Entities.User
| Error of exn

type Msg =
| Submit
| CreateUserResult of CreateUserResult