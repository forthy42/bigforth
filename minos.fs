\             *** X-Windows Widgets for bigFORTH ***   25aug96py

\ generic loadscreen                                   21sep07py

\needs {        include locals.fs
\needs object   include oof.fb
\needs :[       include lambda.fs
include sincos.fs
\needs >xyxy    include points.fs
\needs sort     include qsort.fs
\needs $!       include string.fs
\needs xc@+     include utf-8.fs
\needs l"       include i18n.fs
[IFUNDEF] >class"
\ useful utilities                                     09jan00py

Code pin ( x n -- )  DX pop  DX SP AX *4 I) mov  AX pop
     Next end-code macro :dx :ax T&P
$7FFFFFFF | Constant mi
: 0max dup 0>= and ;
: 0min dup 0< and ;
Code 8*  ( n -- 8*n ) 3 # AX sal  Next end-code macro
Code 3*  ( n -- 3*n ) AX AX *2 I) AX lea  Next end-code macro

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


[IFDEF] unix
\ Loadscreen for X11                                   21sep07py

\needs x11      include x11.fs
\needs xrender  include xrender.fs
\needs xpm      include xpm.fs
\needs opengl   include opengl.fs
\needs xconst   | import xconst
\needs glconst  | import glconst

Onlyforth
Module MINOS
Memory also x11 also xrender also xconst also Forth also MINOS


[THEN]
[IFDEF] win32

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
  clear-icons  clear-fonts  0 bind term ;

[IFDEF] x11  also DOS
: win-init ( -- ) !time clear-resources
  xresource new  xresource with
     s" DISPLAY" env$ 0= IF drop s" :0.0" THEN open connect
     colors ^ endwith
  displays new bind screen
  0 0" STRING" screen xrc dpy @ XInternAtom to XA_STRING8
  0 maxascii $80 = IF  0" UTF8_STRING"  ELSE  0" STRING"  THEN
  screen xrc dpy @ XInternAtom to XA_STRING
  screen timeoffset  screen xrc timeoffset !
  screen xrc calibrate  XTime screen lastcal !
  normal-font
  'nilscreen screen bind parent
  get-pixmap-format  pixmap-format @ 1 = IF  mono  THEN ;
previous                                        [THEN]

\ win-init                                             07jan07py

[IFDEF] win32
ficon: minos-win icons/minos1+.icn"
: win-init ( -- )  clear-resources
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

main: ['] WINi/o IS standardi/o ;

[IFDEF] x11
cold: win-init ;
[THEN]
[IFDEF] win32
cold: set-exceptions win-init ;
[THEN]

\ init sequence                                        10apr04py
[IFDEF] x11
: "geometry ( addr u -- ) scratch 0place
  0 0 0 0 sp@ dup cell+ dup cell+ dup cell+ scratch
  XParseGeometry >r
  r@ [ WidthValue HeightValue or ] Literal tuck and =
  IF  map-size 2!  ELSE  2drop  THEN
  r> [ XValue YValue or ] Literal tuck and =
  IF  map-pos  2!  ELSE  2drop  THEN ;
: -geometry ( -- )  bl word count "geometry ;

also -options definitions
' "geometry Alias -geometry
previous definitions
export minos -geometry ;
[THEN]

[IFDEF] unix
include xstyle.fs
toss toss toss toss toss
Module;
[THEN]
[IFDEF] win32
include xstyle.fs
include w95style.fs
toss toss toss
Module;
[THEN]
