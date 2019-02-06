[<AutoOpen>]

module Database

open System

module Entities =

    type User =
        { Id : int
          UserName : string }

        static member EmptyTest =
            { Id = -25
              UserName = "EmptyTestUserName" }