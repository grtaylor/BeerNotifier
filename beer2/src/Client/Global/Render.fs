module Render

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let pageNotFound =
    div [] [ str "[Dispatcher.View] page not found" ]