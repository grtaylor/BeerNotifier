module Client.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fable.FontAwesome

open Fulma

open Router

open Types


let safeComponents =
    let components =
        span [ ]
           [
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io/elmish/" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://mangelmaxime.github.io/Fulma" ] [ str "Fulma" ]
           ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let private iconClock =
    Icon.icon []
        [ Fa.i [ Fa.Solid.Clock] [] ]

let private iconBeer =
    Icon.icon []
        [ Fa.i [ Fa.Solid.Beer ] [] ]

let private currentPageName model =
    match model with
    | { CurrentPage = page } ->
        let name =
            match page with
            | Router.Page.Home ->
                p [] [str "Home"]
            | Router.Page.AuthPage authPage ->
                p [] [str (sprintf "Page: %A | Logged in as User - %A" authPage model.Session)]
        Navbar.Item.div [] [ name ]

let navbarPropOnClickGoTo routerPage =
    Navbar.Item.Props [ OnClick (fun _ -> routerPage |> Router.modifyLocation ) ]

let private navbarStart dispatch =
    Navbar.Start.div []
        [ Navbar.Item.a [ navbarPropOnClickGoTo Router.Page.Home ]
            [ str "Home" ]
          Navbar.Item.a [ navbarPropOnClickGoTo (Router.User.Index |> Router.AuthPage.User |> Router.Page.AuthPage) ]
            [ str "Participants" ]
          Navbar.Item.a [ navbarPropOnClickGoTo (Router.User.AboutMe |> Router.AuthPage.User |> Router.Page.AuthPage) ]
            [ str "About Me" ]
        ]

let private navbarView model dispatch =
    div [ ClassName "navbar-bg" ]
        [ Container.container []
            [ Navbar.navbar [ Navbar.CustomClass "is-primary" ]
                [ Navbar.Brand.div []
                    [ Navbar.Item.a [ Navbar.Item.Props [ Href "#" ] ]
                        [ iconBeer
                          iconClock
                          Heading.p [ Heading.Is4 ] [ str "Beer Notifier" ] ]
                      navbarStart dispatch
                    ] ]
            ] ]

let private homeView model dispatch =
    Container.container [ Container.IsFluid ]
        [ Content.content []
            [ str "Should probably greet the logged in user"
              button "Sign me up for beer next week!" (fun _ ->
                Fable.Import.Browser.window.alert("OK - you are signed up for beer next week! (once this button is wired up)") )
            ]
        ]

let private renderPage model dispatch =
    match model with
    | { CurrentPage = Page.Home } ->
        homeView model dispatch
    | { CurrentPage = Page.AuthPage (AuthPage.User _)
        UserDispatcher = Some extractedModel } ->
        let d' = (UserDispatcherMsg >> dispatch)
        User.Dispatcher.View.root extractedModel d'
    | _ ->
        p [] [ str "Page not found" ]

let root (model : Model) (dispatch : Msg -> unit) =
    div []
        [ navbarView model dispatch
          Container.container []
              [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ renderPage model dispatch ] ]

          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ]
        ]