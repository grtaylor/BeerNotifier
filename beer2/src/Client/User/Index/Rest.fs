module User.Index.Rest

open Types

open Fable.PowerPack

let getUsers _ =
    promise {
        let data =
            [ for dummyId in 1..10 do
                yield
                    { Id = dummyId
                      Name = sprintf "Dummy Name %i" dummyId } ]

        do! Promise.sleep 500

        return GetUsersResult.Success data
    }