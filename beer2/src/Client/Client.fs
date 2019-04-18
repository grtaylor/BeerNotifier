module Client.Main

open Elmish
open Elmish.React
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

open Client.State
open Client.View



#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update root
|> Program.toNavigable (parseHash Router.pageParser) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Authentication.runWithAdal Authentication.context
