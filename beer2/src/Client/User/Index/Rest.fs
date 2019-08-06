module User.Index.Rest

open Types

open Fetch

let getUsers authModel =
    promise {
        let! authHeader =
            match Authentication.makeAuthHeader authModel with
            | Some header -> header
            | None -> Promise.reject "Unable to create authHeader because [makeAuthHeader] returned None"

        let! data =
            let authenticatedJsonHeaders =
                [ authHeader
                  HttpRequestHeaders.ContentType "application/json" ]

            let requestProperties =
                [ requestHeaders authenticatedJsonHeaders ]

            // todo - pass http://server:port/api/ from config to `getUsers`
            fetch "/api/users" requestProperties
            |> Promise.bind (fun result -> result.json<list<Shared.Entities.User>>())

        return GetUsersResult.Success data
    }