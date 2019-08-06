module User.About.Rest

open Types

open Fetch

let getAboutInfo userId authModel =
    promise {
        let! authHeader =
            match Authentication.makeAuthHeader authModel with
            | Some header -> header
            | None -> Promise.reject "Unable to create authHeader"

        let! data =
            let authenticatedJsonHeaders =
                [ authHeader
                  HttpRequestHeaders.ContentType "application/json" ]

            let requestProperties =
                [ (requestHeaders authenticatedJsonHeaders) ]

            fetch (sprintf "/api/users/%i/about" userId)
                  requestProperties
            |> Promise.bind (fun result -> result.json<Shared.Entities.UserAboutInfo>())

        return GetAboutInfoResult.Success data
    }