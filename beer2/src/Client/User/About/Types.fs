module User.About.Types

type Model =
    { Name: string }

    static member Empty =
        { Name = "Unknown User!" }

type Msg =
| GetAboutInfo