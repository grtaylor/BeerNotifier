module Router

open Fable.Import
open Fable.Helpers.React.Props
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

[<RequireQualifiedAccess>]
type User =
| Index
| Show of int // make int UserId
| Create // rename as Join beer notifier?
// | Login // elmish has runWithAdal at start in Client.fs

[<RequireQualifiedAccess>]
type AuthPage =
| User of User

[<RequireQualifiedAccess>]
type Page =
// user must be authenticated to reach these `AuthPage`s
| AuthPage of AuthPage
// a page that can be accessed when the user is not logged in
| Home

let private toHash page =
    match page with
    | Page.AuthPage authPage ->
        match authPage with
        | AuthPage.User userPage ->
            match userPage with
            | User.Index -> "#user/index"
            | User.Show userId -> sprintf "#user/%i" userId
            | User.Create -> "#user/create"
            // | User.Login -> "#user/login"
    | Page.Home -> "#/"

let pageParser : Parser<Page -> Page, Page> =
    oneOf [
        map (User.Index |> AuthPage.User |> Page.AuthPage) (s "user" </> s "index")
        map (User.Show >> AuthPage.User >> Page.AuthPage) (s "user" </> i32)
        map (User.Create |> AuthPage.User |> Page.AuthPage) (s "user" </> s "create")
        // go to Home if not matched
        map (Page.Home) top
    ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    Browser.window.location.href <- toHash route