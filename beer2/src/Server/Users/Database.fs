module Users.Database

let getAll () : Shared.Entities.User seq =
    seq {
        yield { UserName = "UserName-1" }
        yield { UserName = "UserName-2" }
        yield { UserName = "UserName-3" }
    }