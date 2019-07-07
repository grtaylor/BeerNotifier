module User.Show.Rest

open Types

open Fable.Core
open Fable.Core.JsInterop

open System

let getDetails (userId : int) =
    promise {
        let data =
            { Id = userId
              Name = sprintf "User-%i" userId }
        do! Promise.sleep 500
        return data
    }