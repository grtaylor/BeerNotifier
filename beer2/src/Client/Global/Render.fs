module Render

open Fable.React
open Fable.React.Props

open Fulma

let pageNotFound =
    div [] [ str "[Dispatcher.View] page not found" ]