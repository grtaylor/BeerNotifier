module User.Index.View

open Types

open Fable.React
open Fable.React.Props

open Fulma

let private userView (user : Shared.Entities.User) =
    Card.card [] [
        Card.Header.title [ Card.Header.Title.IsCentered ]
            [ str user.UserName ]
        Card.content []
            [ Content.content []
                [ str "How many times this person has been chosen?" ] ]
    ]

let private usersList users =
    ul [] [ for user in users do
                yield userView user ]

let private content model =
    match model.Users with
    | Some users ->
        [ usersList users ]
    | None ->
        [ str "Nothing to show right now." ]

let root model _ =
    Container.container [] (content model)
