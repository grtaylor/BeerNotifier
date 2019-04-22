module User.About.Rest

open Types

open Fable.PowerPack
open Fable.PowerPack.Fetch

let getAboutInfo userId =
    promise {
        let! data =
            fetch (sprintf "/api/users/%i/about" userId) [ Authentication.bearerHeader() ]
            |> Promise.bind (fun result -> result.json<Shared.Entities.UserAboutInfo>())

        return GetAboutInfoResult.Success data
    }