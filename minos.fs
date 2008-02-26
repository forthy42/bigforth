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
\ helper
[IFDEF] x11
: screen-sync  ( -- )   screen sync ;
: screen-ic!  ( ic -- ) screen xrc ic ! ;       [THEN]

\ file widget                                          10apr04py
DOS also
lbutton class file-widget
public: cell var size           cell var time
        cell var attr           cell var wsize
        cell var wtime          cell var wdate
how:    \ 6 colors defocuscol !
        : dispose 0 bind callback  super dispose ;
        : assign ( size time attr addr len -- )  base push
          super assign attr ! time ! size ! ;
        : !resized super !resized  decimal
          size @ 0 <# #S #> 0 textsize drop wsize !
          S" 00may99"       0 textsize drop wdate !
          S" 00:00:00"      0 textsize drop wtime ! ;
[IFDEF] x11   : dir@ attr @ $C >> ;             [THEN]
[IFDEF] win32 : dir@ attr @ $10 and 0<> 4 and ; [THEN]

\ file widget                                          10apr04py
        : draw ( -- )  base push decimal  push? 1 and >r
          xywh color @ dpy box
          r@ IF  shadow swap xS xywh drawshadow  THEN
          text $@
          xywh nip texth @ - 2/ +  xS 1+ 0 p+
          r@ r@ p+  x @ xS + r@ + y @ xS + r@ +
          dir@ r> 4 << or ficons icon-pixmap with draw-at
          w @ endwith xS + xM color @ 8 >>
          { iw m cc |  dpy mask
          2swap 2over iw 0 p+ cc .text
          w @ wdate @ - 6 - 0 p+
          time @ >date 2over  cc .text
          m wtime @ + 0 p-   time @ >time 2over cc .text
          m wsize @ + 0 p- size @ 0 <# #S #>
          2swap cc .text } ;

\ file widget                                          10apr04py
        : hglue ( -- glue )  super hglue xM 3 *
          wdate @ wtime @ wsize @ + + +
          dir@ ficons >o icon-pixmap w @ o> + 8 + 0 p+ ;
        : vglue ( -- glue )  super vglue swap
          dir@ ficons >o icon-pixmap h @ o> xS 2* + 1+
          max swap ;
        : clicked  ( click -- )
          dup 0= IF  2drop 2drop  EXIT  THEN
          dup 2/ 1 > >r >released ( cc )
          0= IF  rdrop  EXIT THEN
          0 text $@ callback store
          r> IF  #cr 0 dpy dpy keyed  THEN ;
        : keyed ( key sh -- )  drop bl =
          IF  xywh 2drop  1 2 clicked  THEN ;
class;

\ file listbox                                         10apr04py
[IFDEF] x11     : dir? @attr $C >> 4 = ;        [THEN]
[IFDEF] win32   : dir? @attr $10 and 0<> ;      [THEN]
component class file-listbox
public: actor ptr file          actor ptr path
        cell var file<=         early name<=
        early date<=            early length<=
how:    : read-files ( addr attr -- w1 .. wn n )
          fsfirst 0 >r
          BEGIN  pause  0=  WHILE  dir? 0=
                 IF  \ cr ." file " dtaname >file type
                     file self
                     @length @time @attr
                     dtaname >len file-widget new
                     r> 1+ >r  THEN  fsnext
          REPEAT  r> ;

\ file listbox                                         10apr04py

        : read-dir  ( addr attr -- w1 .. wn n )
          over >len '/ -scan + dup push '* swap w!
          fsfirst  0 >r
          BEGIN  pause  0=  WHILE  dir?
                 dtaname >len s" ." compare 0<>
                 dtaname >len s" .." compare 0<> and and
                 IF  \ cr ." dir " dtaname >file type
                     path self
                     @length @time @attr
                     dtaname >len file-widget new
                     r> 1+ >r  THEN  fsnext  REPEAT  r> ;

        : close   dpy close ;

\ file listbox sort methods                            10apr04py

        : name<= ( w1 w2 -- flag )   >r
          file-widget with text $@ endwith  r>
          file-widget with text $@ endwith  compare 0>= ;

        : date<= ( w1 w2 -- flag )   2dup
          file-widget with time @ endwith  swap
          file-widget with time @ endwith
          2dup = IF  2drop name<=  ELSE  u>= nip nip  THEN ;

        : length<= ( w1 w2 -- flag ) 2dup
          file-widget with size @ endwith swap
          file-widget with size @ endwith
          2dup = IF  2drop name<=  ELSE  u>= nip nip  THEN ;

\ file listbox                                         10apr04py

        : newdir ( addr len attr -- object )
          scratch 0place
          file<= @ F IS lex
          scratch $1C0 read-dir   >r  sp@ r@ sort
          scratch $0C0 read-files >r  sp@ r@ sort
          r> r> + dup 0=
          IF  s" -Empty Directory-" text-label new swap 1+  THEN
          0 1 *filll 2dup   rule new swap 1+ vresize new
          ['] <= F IS lex ;
        : init ( addr u file-act path-act <= -- )
          file<= !  bind path  bind file
          newdir 1 super init ;
        : dispose path dispose file dispose super dispose ;
class;

\ file selector box                                    22sep07py
window class file-selector
public: icon-but ptr reloader   button ptr oker
        infotextfield ptr path  infotextfield ptr file
        viewport ptr file-list  cell var ok?
        vabox ptr sort-menu     info-menu ptr sort-title
        modal ptr close-it      actor ptr do-ok
        early by-name           early by-date
        early by-length         method reload
how:    AVariable file<=
        : cancel ( -- )  ok? off hide :: close ;
        : ok     ( -- )  ok? on  hide
          0 path get  file get  do-ok store :: close ;
        : close  cancel ;
        : !file  ( addr len -- )  file assign ;

\ file selector box                                    10apr04py

        : !path  ( addr len -- )
          2dup  s" ."  compare
          IF    path get  >r scratch r@ move scratch r@ '/ -scan
                2over s" .." compare 0=
                IF    2swap 2drop 2dup + >r 1- '/ -scan
                      over + r> over - r@ swap dup >r F delete
                      r> r> swap -
                ELSE  2 pick 1+ r> + >r r@ swap /string
                      s" /" 2over insert insert scratch r> THEN
[IFDEF] x11     over c@ '/ = IF  path assign  ELSE  2drop  THEN
[ELSE]          path assign   [THEN]
          ELSE  2drop  THEN
          sort-title get reload ;

\ file selector box                                    10apr04py

        : newdir ( addr len -- object ) \  dta fsetdta
          ^ S[ !file ]S ^ S[ !path ]S file<= @
          file-listbox new ;
        : reload ( addr len -- )
          sort-title assign  path get  newdir
          file-list with  assign  resized  endwith ;
        : by-name
          file-listbox ' name<=   file<= ! s" name"   reload ;
        : by-date
          file-listbox ' date<=   file<= ! s" date"   reload ;
        : by-length
          file-listbox ' length<= file<= ! s" length" reload ;

\ file selector box                                    10apr04py
        : >real-path ( addr n1 -- addr' n2 )
[IFDEF] win32
          over 1+ c@ ': <>
[ELSE]    over c@ '/ <>   [THEN]
          IF  2dup  pad dup 0 dgetpath drop >len
[IFDEF] win32 2dup bounds ?DO  I c@ '\ = IF  '/ I c!  THEN LOOP
[THEN]        dup IF  2dup + '/ swap c! 1+  THEN
              dup >r + swap move r> + nip pad swap
          THEN ;
        : sort-menu:    ( -- o )
          ^ ['] by-name   simple new s" name"   menu-entry new
          ^ ['] by-date   simple new s" date"   menu-entry new
          ^ ['] by-length simple new s" length" menu-entry new
          3 vabox new  widget :: xS borderbox ;

\ file selector window                                 10apr04py
        : panel-line ( info l file l path l -- widget )
          >real-path
          ^ ST[ reloader self close-it default! ]ST
          -rot s" Path:" tableinfotextfield new bind path
          2swap ^ ST[ oker self close-it default! ]ST
          4 -roll  tableinfotextfield new bind file
          path self  file self   sort-title self  2fill
          ^ S[ s" ."  !path ]S dot-dir    icon-but new
                                   dup bind reloader
          ^ S[ s" .." !path ]S dotdot-dir icon-but new
          2 hatbox new 2 hskips 2skip
                ^ ['] ok     simple new s" OK"   button new
          dup >r                   dup bind oker
          2skip ^ ['] cancel simple new s" Cancel" button new
          3 hatbox new

\ file selector window                                 10apr04py

          5 habox new  3 r> modal new  panel  dup bind close-it
          1 habox new  vfixbox  path get
          1 1 viewport new
          D[ newdir ]D  dup bind file-list
          asliderview new  2 vabox new ;

\ file selector window                                 10apr04py

        : assign ( info len file len path len -- )
          sort-menu self
          s" Sort by" info-menu new bind sort-title
          panel-line  s" File Selector"  super assign ;
        : init ( action dpy -- )
          super init  bind do-ok  file-listbox ' name<= file<= !
          sort-menu: bind sort-menu  diro-icon set-icon ;
        : keyed  over #cr =
          IF  close-it keyed  ELSE  super keyed  THEN ;
class;

\ fsel-input                                           10apr04py

minos

: path+file ( path len file len -- file len )
  >r >r tuck scratch 2+ swap move  scratch 2+ swap  r> r> 2swap
  '/ -scan + swap 2dup + 0 swap c! move  scratch 2+ >len ;

: fsel-action ( info len file1 len1 path1 len1 simple -- )
  screen self file-selector new
  file-selector with  assign  0 $10 geometry  show  endwith ;

: fsel-dialog ( info len file1 len1 path1 len1 simple -- )
  screen self file-selector new  get-win  swap
  file-selector with  set-parent  assign  0 $10 geometry
  show endwith ;

\ fsel-input                                           10apr04py

: ?suffix ( path len suffix len -- path len' )
\  2swap tuck scratch 2+ move scratch 2+ swap 2swap
  dup >r 2over dup r> - 0max /string 2over compare
  IF    >r >r tuck scratch 2+ swap move  scratch 2+ swap  r> r>
        2swap + swap 2dup + 0 swap c! move  scratch 2+ >len
  ELSE  2drop  THEN ;

previous minos

\ win-init                                             07jan07py
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
