module Router

open Fable.Import
open Fable.Helpers.React.Props
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

type UserPage =
| Index
| Show of int // make UserId
| Create // rename as Join beer notifier?

type Page =
| User of UserPage
| Home

let private toHash page =
    match page with
    | User userPage ->
        match userPage with
        | Index -> "#user/index"
        | Show userId -> sprintf "#user/%i" userId
        | Create -> "#user/create"
    | Home -> "#/"

let pageParser : Parser<Page -> Page, Page> =
    oneOf [
        map (UserPage.Index |> User) (s "user" </> s "index")
        map (UserPage.Show >> User) (s "user" </> i32)
        map (UserPage.Create |> User) (s "user" </> s "create")
        // go to Home if not matched
        map (Home) top
    ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    Browser.window.location.href <- toHash route