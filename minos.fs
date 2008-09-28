\             *** X-Windows Widgets for bigFORTH ***   25aug96py

\ Copyright (C) 1996,1997,1998,2000,2003,2004,2005,2006,2007,2008 Bernd Paysan

\ This file is part of MINOS

\ MINOS is free software; you can redistribute it and/or
\ modify it under the terms of the GNU Lesser General Public License
\ as published by the Free Software Foundation, either version 3
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU Lesser General Public License for more details.

\ You should have received a copy of the GNU Lesser General Public License
\ along with this program; if not, see http://www.gnu.org/licenses/.


\ generic loadscreen                                   21sep07py

[defined] VFXFORTH [IF]
    include vfx-minos/oof.fs
    include vfx-minos/lambda.fs
    include vfx-minos/helper.fs
    include sincos.fs
    include vfx-minos/points.fs
\    include vfx-minos/qsort.fs
    include vfx-minos/string.fs
    include vfx-minos/xchar.fs
    include i18n.fs
    include vfx-minos/struct.fs
[ELSE]
\needs {        include locals.fs
\needs object   include oof.fb
\needs :[       include lambda.fs
include sincos.fs
\needs >xyxy    include points.fs
\needs sort     include qsort.fs
\needs $!       include string.fs
\needs xc@+     include utf-8.fs
\needs l"       include i18n.fs
[THEN]
[defined] >class" 0= [IF]
\ useful utilities                                     09jan00py

    [defined] VFXFORTH [IF]
	: pin 2+ cells sp@ + ! ;
	$7FFFFFFF | Constant mi
	: 0max dup 0< 0= and ;
	: 0min dup 0< and ;
	: 8*  ( n -- 8*n ) 3 lshift ;
	: 3*  ( n -- 3*n ) dup 2* + ;
    [ELSE]
	Code pin ( x n -- )  DX pop  DX SP AX *4 I) mov  AX pop
	    Next end-code macro :dx :ax T&P
	$7FFFFFFF | Constant mi
	: 0max dup 0>= and ;
	: 0min dup 0< and ;
	Code 8*  ( n -- 8*n ) 3 # AX sal  Next end-code macro
	Code 3*  ( n -- 3*n ) AX AX *2 I) AX lea  Next end-code macro
    [THEN]	
\ class utility                                        01jan00py
| : cell-@  dup IF cell- @ THEN ;
: parent@ ( object -- parent )  >o object parento @ cell-@ o> ;
: child@  ( object -- child  )  >o object childo  @ cell-@ o> ;
: next@   ( child  -- next   )  >o object nexto   @ cell-@ o> ;
: object" ( object -- addr n )  body> >name count $1F and ;
: >class" ( object -- addr n )
    dup @ swap parent@ child@
    BEGIN  2dup @ <>  WHILE  next@ dup 0=  UNTIL
    2drop s" ---"  EXIT  THEN
    nip object" ;
: lctype  bounds ?DO  I c@ tolower emit  LOOP ;

Patch .class
:noname ." class: " base push hex ^ . o@ . cr ;  IS .class
\ :noname ( object -- )  >class" lctype cr ; IS .class
[THEN]


[defined] unix [IF]
\ Loadscreen for X11                                   21sep07py

    [defined] VFXFORTH [IF]
	include vfx-minos/x11.fs
	include vfx-minos/xrender.fs
	include vfx-minos/xpm.fs
	include x.fs
    [ELSE]
	\needs x11      include x11.fs
	\needs xrender  include xrender.fs
	\needs xpm      include xpm.fs
	\needs opengl   include opengl.fs
	\needs xconst   | import xconst
	\needs glconst  | import glconst
    [THEN]
Onlyforth
Module MINOS
Memory also x11 also xrender also xconst also Forth also MINOS


[THEN]
[defined] win32 [IF]

\ Loadscreen for win32                                 21sep07py

\needs struct{  include struct.fs
\needs win32api include win32.fs
\needs opengl   include opengl.fs
\needs glconst  include glconst.fs
include win32ex.fs

Onlyforth
Module MINOS
Memory also win32api also Forth also MINOS

[THEN]

include minos-base.fs
include displays.fs
include minos-fonts.fs
include vdisplays.fs
include actors.fs
include widgets.fs
include minos-boxes.fs
include minos-buttons.fs
include minos-viewport.fs
include minos-windows.fs
include minos-complex.fs
include resources.fs
\ win-init                                             07jan07py

: clear-resources  ( -- )
  clear-icons  clear-fonts [defined] term [IF] 0 bind term [THEN] ;

[defined] x11 [IF]  also DOS
: win-init ( -- ) !time clear-resources
  xresource new  xresource with
     s" DISPLAY" env$ 0= IF drop s" :0.0" THEN open connect
     colors ^ endwith
  displays new bind screen
  screen xrc dpy @ 0" STRING" 0 XInternAtom to XA_STRING8
  screen xrc dpy @
  maxascii $80 = IF  0" UTF8_STRING"  ELSE  0" STRING"  THEN  0
  XInternAtom to XA_STRING
  screen timeoffset  screen xrc timeoffset !
  screen xrc calibrate  XTime screen lastcal !
  normal-font
  'nilscreen screen bind parent
  get-pixmap-format  pixmap-format @ 1 = IF  mono  THEN ;
previous                                        [THEN]

\ win-init                                             07jan07py

[defined] win32 [IF]
ficon: minos-win icons/minos1+.icn"
: win-init ( -- ) clear-resources
  xresource new xresource with
      IDI_APPLICATION 0 LoadIcon register
\      minos-win icon-pixmap with
\          image @ shape @ &24 1 h @ w @
\      endwith inst @ CreateIcon register
      get-sys-colors
      colors  ^ endwith
  displays new bind screen
  normal-font
  'nilscreen screen bind parent ;
[THEN]

\ main: cold: bye:                                     10apr04py

[defined] VFXFORTH 0= [IF]
    main: ['] WINi/o IS standardi/o ;

[defined] x11 [IF]
    cold: win-init ;
[THEN]
[defined] win32 [IF]
    cold: set-exceptions win-init ;
[THEN]
: event-loop  BEGIN  stop
    screen childs self 'nilscreen =  UNTIL ;
[ELSE]
    ' win-init atcold
    : minos-idle screen handle-events ;
    : event-loop ( -- ) BEGIN  minos-idle
	screen childs self 'nilscreen =  UNTIL ;
[THEN]

\ init sequence                                        10apr04py
[defined] x11 [defined] VFXFORTH 0= and [IF]
: "geometry ( addr u -- ) scratch 0place
  0 sp@ >r 0 0 0 scratch r> dup cell- dup cell- dup cell-
  XParseGeometry >r
  r@ [ WidthValue HeightValue or ] Literal tuck and =
  IF  map-size 2!  ELSE  2drop  THEN
  r> [ XValue YValue or ] Literal tuck and =
  IF  map-pos  2!  ELSE  2drop  THEN  2 ;
: -geometry ( -- )  bl word count "geometry drop ;

also -options definitions
' "geometry Alias -geometry
previous definitions

export minos -geometry ;
[THEN]

[defined] unix [IF]
include xstyle.fs
toss toss toss toss toss
Module;
[THEN]
[defined] win32 [IF]
include xstyle.fs
include w95style.fs
toss toss toss
Module;
[THEN]
