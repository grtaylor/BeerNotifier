module User.About.State

open Types

open Elmish

let init () =
    Model.Empty, Cmd.ofMsg GetAboutInfo

let update msg model =
    model, Cmd.none