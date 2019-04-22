module User.About.View

open Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let root (model : Model) _ =
    Container.container []
        [ str (sprintf "Logged in as: %s" model.Name ) ]