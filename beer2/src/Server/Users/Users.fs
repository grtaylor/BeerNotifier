module Users.Controller

open Saturn

let controller = controller {
    // requires an "x-controller-version" header to match the version specified in this controller { ... } computation expression
    // version "1"

    // view list of users
    index (fun ctx -> Users.Database.getAll () |> Controller.json ctx)
    add (fun ctx -> "Add handler version 1" |> Controller.text ctx)
    show (fun ctx userId -> (sprintf "Show handler version 1 - %i" userId) |> Controller.text ctx)
    edit (fun ctx userId -> (sprintf "Edit handler version 1 - %i" userId) |> Controller.text ctx)
}