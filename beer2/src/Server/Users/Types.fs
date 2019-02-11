module Users.Types

/// Models types at the data boundary:
/// - our Database Types
/// - other API/Service types outside of this application
module Entities =
    type User =
        { UserName: string }

/// Domain specific types
module Domain =
    type User =
        { DisplayName: string }