module User.Create.View

open Types

open Fable.React
open Fable.React.Props

open Fulma

let root model dispatch =
    Container.container []
        [ Section.section []
            [ Heading.h3 []
                [ str "TODO - Sign up for beer notifier" ]
              str "TODO - show sign up info form" ] ]
              //User.Index.View.root model dispatch ] ]