module User.Dispatcher.Types

type Model =
    { CurrentPage : Router.User
      IndexModel : User.Index.Types.Model option
      ShowModel : User.Show.Types.Model option
      CreateModel : User.Create.Types.Model option }

    static member Empty =
        { CurrentPage = Router.User.Index
          IndexModel = None
          ShowModel = None
          CreateModel = None }

type Msg =
| IndexMsg of User.Index.Types.Msg
| ShowMsg of User.Show.Types.Msg
| CreateMsg of User.Create.Types.Msg