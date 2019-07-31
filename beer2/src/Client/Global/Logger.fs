module Logger

open Fable.Core

let inline error msg = Browser.Dom.console.error msg

let inline errorfn fn msg = Browser.Dom.console.error (sprintf fn msg)

let inline log msg = Browser.Dom.console.log msg

let inline debug (info : obj) = Browser.Dom.console.log("[Debug]", info)

let inline debugfn fn info = Browser.Dom.console.log("[Debug] " + sprintf fn info)