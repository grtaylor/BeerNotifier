namespace Shared

open System

/// The Server Entities we wish to share with the Client
module Entities =

    type User =
        { UserName: string }

    type UserAboutInfo =
        { UserName: string
          LastPurchaseDate: System.DateTime }
        /// Id that the server will assume means the AD logged in user
        static member SelfId = 0