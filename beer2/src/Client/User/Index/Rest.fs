module User.Index.Rest

open Types

open Fetch

let getUsers authUser =
    promise {
        let! data =
            let authenticatedJsonHeaders =
                [ Authentication.makeAuthHeader authUser
                  HttpRequestHeaders.ContentType "application/json" ]

            let requestProperties =
                [ requestHeaders authenticatedJsonHeaders ]

            // todo - pass http://server:port/api/ from config to `getUsers`
            fetch "/api/users" requestProperties
            |> Promise.bind (fun result -> result.json<list<Shared.Entities.User>>())

        return GetUsersResult.Success data
    }