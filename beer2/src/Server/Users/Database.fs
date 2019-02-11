module Users.Database

open Types

let getAll () : Entities.User seq =
    seq {
        yield { UserName = "UserName-1" }
        yield { UserName = "UserName-2" }
        yield { UserName = "UserName-3" }
    }