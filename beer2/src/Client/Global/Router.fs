module Router

open Fable.Import
open Fable.React.Props
open Elmish.Navigation
open Elmish.UrlParser

[<RequireQualifiedAccess>]
type User =
| Index
| Show of int // make int UserId
| Create // rename as Join beer notifier?
// | Login // elmish has runWithAdal at start in Client.fs
| AboutMe

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
            | User.Index -> "#users/index"
            | User.Show userId -> sprintf "#users/%i" userId
            | User.Create -> "#users/create"
            | User.AboutMe -> "#users/about"
    | Page.Home -> "#/"

let private pageParser : Parser<Page -> Page, Page> =
    oneOf [
        map (User.Index |> AuthPage.User |> Page.AuthPage) (s "users" </> s "index")
        map (User.Show >> AuthPage.User >> Page.AuthPage) (s "users" </> i32)
        map (User.Create |> AuthPage.User |> Page.AuthPage) (s "users" </> s "create")
        map (User.AboutMe |> AuthPage.User |> Page.AuthPage) (s "users" </> s "about")
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
    Browser.Dom.window.location.href <- toHash route

let urlParser location = parseHash pageParser location