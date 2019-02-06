module User.Show.View

open Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let private userView user model dispatch =
    p [] [ str user.Name ]

let private pageContent userInfo model dispatch =
    Section.section []
        [ Columns.columns [ Columns.IsCentered ]
            [ Column.column [ Column.Width(Screen.All, Column.IsTwoThirds) ]
                [ userView userInfo model dispatch ] ] ]

let root model dispatch =
    div [] [ str "TODO load the user" ]