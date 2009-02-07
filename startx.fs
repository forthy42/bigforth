\ Include files needed for xbigforth                   01jan09py

[defined] .blk [IF]
    ' .blk is .status
[THEN]
include minos.fs
include editor.fs
[defined] VFXForth 0= [IF]
include browser.fs
include login.fs
include ptty.fs
include status.fb
[defined] x11 [IF]
also x11  [defined] has-xft [IF] previous
include xft-font.fs  also [THEN] previous
[THEN]
[THEN]