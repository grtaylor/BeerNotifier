module User.Create.View

open Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let root model dispatch =
    Container.container []
        [ Section.section []
            [ Heading.h3 []
                [ str "TODO - Sign up for beer notifier" ]
              str "TODO - show sign up info form" ] ]
              //User.Index.View.root model dispatch ] ]