module User.Create.Rest

open Fable.PowerPack

open System

let createUser (userName : string) =
    promise {
        let nextId = int (DateTime.UtcNow.ToString("yyyyMMDDHHmmss"))
        let user : Entities.User =
            { Id = nextId
              UserName = sprintf "UserName-%i" nextId }

        // fable.import.lowdb later, maybe?

        do! Promise.sleep 500

        return user
    }