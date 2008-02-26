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
/* Font resources                                      12nov06py

[IFDEF] win32
| : /1  1 /string ;
| : /-  '- scan ;
| : -*- /1 2dup /- 2swap over2 - ;
: >font ( addr u -- addr' u' family type width height )
  /1 /- -*- 2swap
  -*- s" bold" str= IF  FW_BOLD  ELSE  FW_NORMAL  THEN  -rot
  -*- s" i" str= IF   ITALIC_FONTTYPE  ELSE  0  THEN  -rot
  /1 /- /1 /- -*- s>number drop >r 2drop swap 0 -rot r> ;
[THEN]
*/
\ Font resources                                       12nov06py

AVariable fonts
: font? ( addr u -- font/0 )  fonts >r
  BEGIN  r> @ dup  WHILE  >r
         2dup r@ cell+ cell+ count compare 0= UNTIL
  2drop r>  EXIT  THEN  nip nip ;

: ?font ( -- addr/0 )  >in @ '" parse  font?  swap >in ! ;
: font@ ( addr -- font ) cell+ dup cell+ count
  2 pick @  IF  2drop @  EXIT  THEN
  new-font tuck swap ! ;

: >font ( font o -- )  gadget with font! endwith ;

\ Font resources                                       10apr04py

: (font" ( o -- o )
  r> dup cell+ cell+ count + >r font@ over >font ; restrict
: (font@ ( o -- o )
  r> dup cell+ >r @ font@ over >font ;             restrict

: font" ( "font"<"> -- )  ?font ?dup
  0= IF   postpone (font" fonts @ here fonts ! A, 0 , ,"
  ELSE    postpone (font@ A, '" parse 2drop  THEN ;
                                            immediate restrict

: .font cr base push hex dup cell+ @ 8 .r
  space 2 cells + count type ;
: .fonts fonts LIST> .font ;

\ Font resources                                       10apr04py
[IFDEF] x11
: font16@ ( addr -- icon ) cell+ dup cell+ count
  2 pick @  IF  2drop @  EXIT  THEN
  X-font16 new tuck swap ! ;

: (font16" ( o -- o )
  r> dup cell+ cell+ count + >r font16@ over >font ; restrict
: (font16@ ( o -- o )
  r> dup cell+ >r @ font16@ over >font ;             restrict

: font16" ( "font"<"> -- )  ?font ?dup
  0= IF   postpone (font16" fonts @ here fonts ! A, 0 , ,"
  ELSE    postpone (font16@ A, '" parse 2drop  THEN ;
                                            immediate restrict
[THEN]

\ File icons                                           10apr04py

AVariable icons
: icon? ( addr u -- icon/0 )  icons >r
  BEGIN  r> @ dup  WHILE  >r
         2dup r@ cell+ cell+ count compare 0= UNTIL
  2drop r>  EXIT  THEN  nip nip ;

: ?icon ( -- addr/0 )  >in @ '" parse  icon?  swap >in ! ;
: icon@ ( addr -- icon ) cell+ dup cell+ count
  2 pick @  IF  2drop @  EXIT  THEN
  icon-pixmap new tuck swap ! ;

: (icon" ( -- )  r> dup cell+ cell+ count + >r icon@ ; restrict
: (icon@ ( -- )  r> dup cell+ >r @ icon@ ;             restrict

\ File icons                                           10apr04py

: icon" ( "file"<"> -- )  ?icon ?dup
  0= IF   postpone (icon" icons @ here icons ! A, 0 , ,"
  ELSE    postpone (icon@ A, '" parse 2drop  THEN ;
                                            immediate restrict

: 2icon" ( "file"<">"file<">" -- )
  postpone icon" postpone icon" ;            immediate restrict

: .icon cr base push hex dup cell+ @ 8 .r
  space 2 cells + count type ;
: .icons icons LIST> .icon ;
: reload-icons ( -- ) icons LIST>
  dup cell+ @ IF  cell+ dup cell+ count rot @
       icon-pixmap with assign endwith  ELSE  drop  THEN ;

\ File icons                                           26jul04py

| : ficon-does   DOES>   icon@ ;
| : ficon@-does  DOES> @ icon@ ;

: ficon: ( "name" "file" -- )  Create ?icon ?dup
  0= IF  icons @ here icons ! A, 0 , ," ficon-does
  ELSE   A, ficon@-does '" parse 2drop  THEN ;

ficon: dir-icon icons/dir"
ficon: diro-icon icons/diropen"
ficon: file-icon icons/file"
ficon: sym-icon icons/sym"
ficon: dot-dir icons/dot-dir"
ficon: dotdot-dir icons/dotdot-dir"
ficon: term-w icons/script"

\ File icons                                           10apr04py

Create ficons
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' dir-icon  A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' sym-icon  A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' diro-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' sym-icon  A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
DOES> ( i -- icon )  swap $1F and cells + perform ;
: set-pixmaps ( pm_0 .. pm_i i -- )
  1- FOR  Pixmaps I cells + !  NEXT
  redraw-all @ redraw-all on  screen draw redraw-all ! ;

\ pixmap style                                         29oct06py

ficon: dark-pm pattern/dark"
ficon: normal-pm pattern/normal"
ficon: focus-pm pattern/focus"
ficon: light-pm pattern/light"
ficon: back-pm pattern/back"
ficon: backtext-pm pattern/backtext"
: marble ( -- )
  0           focus-pm  0           normal-pm
  0           focus-pm  0           normal-pm
  light-pm    dark-pm   light-pm    dark-pm
  0           back-pm   backtext-pm normal-pm $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-w-pm pattern/dark-w"
ficon: normal-w-pm pattern/normal-w"
ficon: focus-w-pm pattern/focus-w"
ficon: light-w-pm pattern/light-w"
ficon: back-w-pm pattern/back-w"
ficon: backtext-w-pm pattern/backtext-w"
: water ( -- )
  0           focus-w-pm  0             normal-w-pm
  0           focus-w-pm  0             normal-w-pm
  light-w-pm  dark-w-pm   light-w-pm    dark-w-pm
  0           back-w-pm   backtext-w-pm normal-w-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-w1-pm pattern/dark-w1"
ficon: normal-w1-pm pattern/normal-w1"
ficon: focus-w1-pm pattern/focus-w1"
ficon: light-w1-pm pattern/light-w1"
ficon: back-w1-pm pattern/back-w1"
ficon: backtext-w1-pm pattern/backtext-w1"
: water1 ( -- )
  0           focus-w1-pm  0              normal-w1-pm
  0           focus-w1-pm  0              normal-w1-pm
  light-w1-pm dark-w1-pm   light-w1-pm    dark-w1-pm
  0           back-w1-pm   backtext-w1-pm normal-w1-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-h-pm pattern/dark-h"
ficon: normal-h-pm pattern/normal-h"
ficon: focus-h-pm pattern/focus-h"
ficon: light-h-pm pattern/light-h"
ficon: back-h-pm pattern/back-h"
ficon: backtext-h-pm pattern/backtext-h"
: wood ( -- )
  0           focus-h-pm  0             normal-h-pm
  0           focus-h-pm  0             normal-h-pm
  light-h-pm  dark-h-pm   light-h-pm    dark-h-pm
  0           back-h-pm   backtext-h-pm normal-h-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-p-pm pattern/dark-p"
ficon: normal-p-pm pattern/normal-p"
ficon: focus-p-pm pattern/focus-p"
ficon: light-p-pm pattern/light-p"
ficon: back-p-pm pattern/back-p"
ficon: backtext-p-pm pattern/backtext-p"
: paper ( -- )
  0           focus-p-pm  0             normal-p-pm
  0           focus-p-pm  0             normal-p-pm
  light-p-pm  dark-p-pm   light-p-pm    dark-p-pm
  0           back-p-pm   backtext-p-pm normal-p-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-p1-pm pattern/dark-p1"
ficon: normal-p1-pm pattern/normal-p1"
ficon: focus-p1-pm pattern/focus-p1"
ficon: light-p1-pm pattern/light-p1"
ficon: back-p1-pm pattern/back-p1"
ficon: backtext-p1-pm pattern/backtext-p1"
: paper1 ( -- )
  0            focus-p1-pm  0              normal-p1-pm
  0            focus-p1-pm  0              normal-p1-pm
  light-p1-pm  dark-p1-pm   light-p1-pm    dark-p1-pm
  0            back-p1-pm   backtext-p1-pm normal-p1-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-c-pm pattern/dark-c"
ficon: normal-c-pm pattern/normal-c"
ficon: focus-c-pm pattern/focus-c"
ficon: light-c-pm pattern/light-c"
ficon: back-c-pm pattern/back-c"
ficon: backtext-c-pm pattern/backtext-c"
: cracle ( -- )
  0           focus-c-pm  0             normal-c-pm
  0           focus-c-pm  0             normal-c-pm
  light-c-pm  dark-c-pm   light-c-pm    dark-c-pm
  0           back-c-pm   backtext-c-pm normal-c-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-m-pm pattern/dark-m"
ficon: normal-m-pm pattern/normal-m"
ficon: focus-m-pm pattern/focus-m"
ficon: light-m-pm pattern/light-m"
ficon: back-m-pm pattern/back-m"
ficon: backtext-m-pm pattern/backtext-m"
: mono ( -- )
  0           focus-m-pm  0             normal-m-pm
  0           focus-m-pm  0             normal-m-pm
  light-m-pm  dark-m-pm   light-m-pm    dark-m-pm
  0           back-m-pm   backtext-m-pm normal-m-pm
  $10 set-pixmaps ;

\ pixmap style                                         10apr04py

ficon: dark-d-pm pattern/dark-d"
ficon: normal-d-pm pattern/normal-d"
ficon: focus-d-pm pattern/focus-d"
ficon: light-d-pm pattern/light-d"
ficon: back-d-pm pattern/back-d"
ficon: backtext-d-pm pattern/backtext-d"
: mud ( -- )
  0           focus-d-pm  0             normal-d-pm
  0           focus-d-pm  0             normal-d-pm
  light-d-pm  dark-d-pm   light-d-pm    dark-d-pm
  0           back-d-pm   backtext-d-pm normal-d-pm
  $10 set-pixmaps ;

\ pixmap style                                         10mar07py
: no-pixmap ( -- ) $10 0 DO  0  LOOP $10 set-pixmaps ;
: re-color ( -- )  get-sys-colors
  screen xrc free-colors  screen xrc colors ;
: gray-colors  ( -- )  grayish  re-color no-pixmap ;
: red-colors   ( -- )  redish   re-color no-pixmap ;
: blue-colors  ( -- )  bluish   re-color no-pixmap ;
: bisque-colors ( -- ) bisquish re-color no-pixmap ;
: color-cube ( r g b -- )
  dup 2over * * $F0 > abort" too many colors"
  to blues to greens to reds re-color  reload-icons
  redraw-all dup push on  screen resized ;
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

previous

\ alert                                                10apr04py

User alert#
button class alertbutton
        cell var exit#          cell var 'alert#
how:    : !alert#  exit# @ 'alert# @ !  dpy close ;
        : init  ( addr len n -- )
          ( swap) exit# !   alert# 'alert# !
          ( >r) 0 ['] !alert# simple new
          -rot ( r>) super init ;
class;

\ alert icons, alert strings                           10apr04py

ficon: error-alert icons/ERROR"
ficon: fatal-alert icons/FATAL"
ficon: info-alert icons/INFO"
ficon: none-alert icons/NONE"
ficon: question-alert icons/QUESTION"
ficon: warning-alert icons/WARNING"

Create alert-icons  T]
        error-alert     question-alert  warning-alert
        fatal-alert     info-alert      none-alert [

Create alert-titles
        ," Error!"      ," Question?"   ," Warning!"
        ," Fatal!"      ," Info"        ," None"

\ alert boxhandler                                     10aug05py
: alert-text ( $1 .. $n n -- o )  dup >r
0 ?DO  I' 2* I - 1- dup >r roll r> roll text-label new LOOP
  r> vbox new ;
: alert-buttons ( $1 .. $n n -- o default )  dup >r
  0 ?DO  I' 2* I - 1- dup >r roll r> roll I alertbutton new
  LOOP  r> dup pick >r hatbox new hskip
  0 [ 1 *fill ] Literal  2dup *hglue new -rot *hglue new
  rot swap 3 habox new r> ;
: make-alert ( $1 .. $n n $1 .. $m m n -- o )
  >r  alert-buttons  r> -rot  >r >r  >r  alert-text
  0 [ 1 *fill ] Literal *vglue new
  r> cells alert-icons + perform icon new
  0 [ 1 *fill ] Literal *vglue new  3 vbox new
  2skip rot 3 hbox new
  -1 [ 1 *fill ] Literal *vglue new r> 3 r> modal new panel ;

\ mousemap                                             10apr04py

minos

: mousemap ( -- )
  screen mouse drop
  ^ window with  xywh resize
      xywh  2swap 2drop p2/ p- 0max swap 0max swap
      repos sync show  endwith ;

\ Alert                                                10apr04py

: title$  ( n -- addr len )   alert-titles swap
  0 ?DO  count +  LOOP  count ;

: alert ( $1 .. $n n $1 .. $m m f -- n2 )
  screen self window new window with  dup >r make-alert
  r> title$ assign
[IFDEF] x11
  0" Alert" dup sp@ xwin @ xrc dpy @ XSetClassHint drop 2drop
[THEN]
  mousemap  stop ( dispose ) endwith  alert# @ ;

\ boxhandler                                           10apr04py

Variable ?showpath  ?showpath on

| : scr# ( -- addr len ) scr @ abs extend
    <# #S s"  klB" bounds  DO  I c@ hold  LOOP  #> ;

: boxhandler  ( addr -- )  tflush
  dup count here count  ?showpath @
  IF    scratch "back 3  ELSE  2  THEN    loaderr @
  IF    >r  scr#  r> 1+ s" Cancel" s" Editor" 2
  ELSE  s"  OK " 1  THEN  0 alert >r (error
  r> 1 = IF " V" find 0= IF drop ELSE execute THEN THEN ;

\ normal font scheme                                   21jun05py
[IFDEF] x11
: (normal-font ( -- )  screen xrc with
  S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
  0 font!  \ normal font
S" -misc-fixed-medium-r-semicondensed-*-*-120-*-*-c-*-iso8859-1"
  1 font!  \ terminal font
  S" -adobe-helvetica-medium-r-normal-*-*-80-*-*-p-*-iso8859-1"
  2 font!  \ icon font
  S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
  3 font!  \ text font
  0" -adobe-helvetica-*-r-*-*-*-120-*-*-*-*-*-*,-misc-fixed-*-r-*-*-*-130-*-*-*-*-*-*" fontset!
  4 cells fontarray @ @ + off
  endwith screen !resized ;

\ large font scheme                                    16jan05py

: (large-font ( -- )  screen xrc with
  S" -adobe-helvetica-bold-r-normal-*-*-180-*-*-p-*-iso8859-1"
  0 font!   \ normal font
  S" -misc-fixed-medium-r-normal-*-*-200-*-*-c-*-iso8859-1"
  1 font!   \ terminal font
  S" -adobe-helvetica-medium-r-normal-*-*-140-*-*-p-*-iso8859-1"
  2 font!   \ icon font
  S" -adobe-helvetica-bold-r-normal-*-*-180-*-*-p-*-iso8859-1"
  3 font!   \ text font
  4 cells fontarray @ @ + off
  endwith screen !resized ;

\ chinese font scheme                                  10apr04py

: chinese-font ( -- )  screen xrc with
  S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
  0 font!   \ normal font
  S" -sony-fixed-medium-r-normal--16-*-*-*-c-*-iso8859-1"
  1 font!   \ terminal font
  S" -adobe-helvetica-medium-r-normal-*-*-80-*-*-p-*-iso8859-1"
  2 font!   \ icon font
  S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
  3 font!   \ text font
  S" -isas-song ti-medium-r-normal--16-*-*-*-c-*-gb2312.1980-0"
  4 font16! \ chinese font
  endwith screen !resized ;

\ japanese font scheme                                 10apr04py

: japanese-font ( -- )  screen xrc with
  S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
  0 font!   \ normal font
  S" -sony-fixed-medium-r-normal--16-*-*-*-c-*-iso8859-1"
  1 font!   \ terminal font
  S" -adobe-helvetica-medium-r-normal-*-*-80-*-*-p-*-iso8859-1"
  2 font!   \ icon font
  S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
  3 font!   \ text font
  S" -jis-fixed-medium-r-normal--16-*-*-*-c-*-jisx0208.1983-0"
  4 font16! \ chinese font
  endwith screen !resized ;

[THEN]

\ clear-resources                                      10apr04py

: clear-icons  icons LIST>  cell+ off ;
: clear-fonts  fonts LIST>  cell+ off ;

: clear-resources  ( -- )
  clear-icons  clear-fonts  0 bind term ;

\ fonts                                                29jul07py

[IFDEF] win32
: (normal-font ( -- )  screen xrc with
S" -*-Arial-medium-r-normal--15-*-*-*-p-*-iso8859-1"
     0 font!
S" -*-Courier new-medium-r-normal--15-*-*-*-c-*-iso8859-1"
     1 font!
S" -*-Arial-medium-r-normal--10-*-*-*-p-*-iso8859-1"
     2 font!
S" -*-Arial-medium-r-normal--15-*-*-*-p-*-iso8859-1"
     3 font!
  endwith screen !resized ;

\ fonts                                                29jul07py

: (large-font ( -- )  screen xrc with
S" -*-Arial-bold-r-normal--20-*-*-*-p-*-iso8859-1"
     0 font!
S" -*-Courier new-bold-r-normal--20-*-*-*-c-*-iso8859-1"
     1 font!
S" -*-Arial-medium-r-normal--12-*-*-*-p-*-iso8859-1"
     2 font!
S" -*-Arial-bold-r-normal--20-*-*-*-p-*-iso8859-1"
     3 font!
  endwith screen !resized ;                     [THEN]
patch normal-font  ' (normal-font IS normal-font
patch large-font   ' (large-font  IS large-font

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
