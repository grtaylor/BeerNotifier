module User.Create.View

open Types

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Fulma

let root (user : Entities.User) model dispatch =
    Container.container []
        [ Section.section []
            [ Heading.h3 []
                [ str ("You are " + user.UserName) ] ] ]