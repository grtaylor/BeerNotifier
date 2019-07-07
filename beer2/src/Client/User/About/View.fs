module User.About.View

open Types

open Fable.React
open Fable.React.Props

open Fulma

let root (model : Model) _ =
    Container.container []
        [ str (sprintf "Logged in as: %s" model.Name ) ]