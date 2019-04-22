module User.About.Types

open Shared

type Model =
    { Name: string }

    static member Empty =
        { Name = "Unknown User!" }

type GetAboutInfoResult =
| Success of Entities.UserAboutInfo
| Error of exn

type Msg =
| GetAboutInfo of int
| GetAboutInfoResult of GetAboutInfoResult