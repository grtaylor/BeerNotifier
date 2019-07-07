module User.About.Rest

open Types

open Fetch

let getAboutInfo userId authUser =
    promise {
        let! data =
            let authenticatedJsonHeaders =
                [ Authentication.makeAuthHeader authUser
                  HttpRequestHeaders.ContentType "application/json" ]

            let requestProperties =
                [ (requestHeaders authenticatedJsonHeaders) ]

            fetch (sprintf "/api/users/%i/about" userId)
                  requestProperties
            |> Promise.bind (fun result -> result.json<Shared.Entities.UserAboutInfo>())

        return GetAboutInfoResult.Success data
    }