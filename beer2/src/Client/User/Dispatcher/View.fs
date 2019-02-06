module User.Dispatcher.View

open Types

let root user model dispatch =
    match model with
    | { CurrentPage = Router.UserPage.Index
        IndexModel = Some extractedModel } ->
            User.Index.View.root extractedModel (IndexMsg >> dispatch)

    | { CurrentPage = Router.UserPage.Show _
        ShowModel = Some extractedModel } ->
            User.Show.View.root extractedModel (ShowMsg >> dispatch)

    | { CurrentPage = Router.UserPage.Create
        CreateModel = Some extractedModel } ->
            User.Create.View.root user extractedModel (CreateMsg >> dispatch)
    // don't do wildcard on the model record so the compiler
    // helps when you add a new kind of Router.NewPage
    | { IndexModel = _ }
    | { ShowModel = _ }
    | { CreateModel = _ } ->
        Render.pageNotFound