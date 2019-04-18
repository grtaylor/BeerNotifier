module User.Dispatcher.View

open Router

open Types

let root model dispatch =
    match model with
    | { CurrentPage = User.Index
        IndexModel = Some extractedModel } ->
            User.Index.View.root extractedModel (IndexMsg >> dispatch)

    | { CurrentPage = User.Show _
        ShowModel = Some extractedModel } ->
            User.Show.View.root extractedModel (ShowMsg >> dispatch)

    | { CurrentPage = User.Create
        CreateModel = Some extractedModel } ->
            User.Create.View.root extractedModel (CreateMsg >> dispatch)
    // don't do wildcard on the model record so the compiler
    // helps when you add a new kind of Router.NewPage
    | { IndexModel = _ }
    | { ShowModel = _ }
    | { CreateModel = _ } ->
        Render.pageNotFound