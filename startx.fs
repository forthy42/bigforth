\ Include files needed for xbigforth                   01jan09py

[defined] .blk [IF]
    ' .blk is .status
[THEN]
include minos.fs
include editor.fs
include browser.fs
include login.fs
include ptty.fs
include status.fb
[IFDEF] x11
also x11  [IFDEF] has-xft previous
include xft-font.fs  also [THEN] previous
[THEN]

