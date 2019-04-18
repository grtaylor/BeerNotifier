module User.Index.Rest

open Types

open Fable.PowerPack
open Fable.PowerPack.Fetch

let getUsers _ =
    promise {
        let! data =
        // todo - pass http://server:port/api/ from config to `getUsers`
            fetch "/api/users" [ Mode RequestMode.Sameorigin
                                 Authentication.bearerHeader() ]
            |> Promise.bind (fun result -> result.json<seq<Shared.Entities.User>>())

        return GetUsersResult.Success data
    }