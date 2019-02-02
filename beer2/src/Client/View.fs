module Client.View

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fable.FontAwesome

open Fulma

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

let show = function
| { Counter = Some counter } -> string counter.Value
| { Counter = None   } -> "Loading..."

let iconClock =
    Icon.icon []
        [ Fa.i [ Fa.Solid.Clock] [] ]

let iconBeer =
    Icon.icon []
        [ Fa.i [ Fa.Solid.Beer ] [] ]

let private navbarView =
    div [ ClassName "navbar-bg" ]
        [ Container.container []
            [ Navbar.navbar [ Navbar.CustomClass "is-primary" ]
                [ Navbar.Brand.div []
                    [ Navbar.Item.a [ Navbar.Item.Props [ Href "#" ] ]
                        [ iconBeer
                          iconClock
                          Heading.p [ Heading.Is4 ] [ str "Beer Notifier" ] ]
                    ] ]
            ] ]

let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ navbarView
          Container.container []
              [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ Heading.h3 [] [ str ("Press buttons to manipulate counter: " + show model) ] ]
                Columns.columns []
                    [ Column.column [] [ button "-" (fun _ -> dispatch Decrement) ]
                      Column.column [] [ button "+" (fun _ -> dispatch Increment) ] ] ]

          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ]
        ]