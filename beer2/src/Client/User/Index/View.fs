module User.Index.View

open Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let private userView (user : UserInfo) =
    p [] [ str user.Name ]

let private usersList users =
    Columns.columns [ Columns.IsCentered ]
        [ Column.column [ Column.Width(Screen.All, Column.IsTwoThirds) ]
            (users |> List.map userView) ]

let root model _ =
    match model.Users with
    | Some users ->
        Container.container []
            [ usersList users ]
    | None ->
        Container.container []
            [ str "Nothing to show right now." ]