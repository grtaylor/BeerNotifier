module Client.Main

open Elmish
open Elmish.React
open Elmish.Navigation
open Elmish.UrlParser

open Client.State
open Client.View



#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update root
|> Program.toNavigable (Router.urlParser) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
