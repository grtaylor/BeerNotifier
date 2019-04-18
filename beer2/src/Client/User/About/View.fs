module User.About.View

open Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let root model _ =
    Container.container []
        [ str "All about YOU" ]