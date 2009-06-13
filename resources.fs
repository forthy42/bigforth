\ Font resources                                       12nov06py

AVariable fonts
: font? ( addr u -- font/0 )  fonts >r
  BEGIN  r> @ dup  WHILE  >r
         2dup r@ cell+ cell+ count compare 0= UNTIL
  2drop r>  EXIT  THEN  nip nip ;

: ?font ( -- addr/0 )  >in @ '"' parse  font?  swap >in ! ;
: font@ ( addr -- font ) cell+ dup cell+ count
  2 pick @  IF  2drop @  EXIT  THEN
  new-font tuck swap ! ;

: >font ( font o -- )  gadget with font! endwith ;

: font-assign ( o addr -- o )  font@ over >font ;

\ Font resources                                       10apr04py

: (font" ( o -- o )
  r> dup cell+ cell+ count + aligned >r font-assign ; restrict
[defined] doNotSin [IF] doNotSin [THEN]

: font" ( "font"<"> -- )  ?font ?dup
  0= IF   postpone (font" fonts @ here fonts ! A, 0 , ," align
  ELSE    postpone Aliteral postpone font-assign '"' parse 2drop  THEN ;
                                           immediate restrict

: .font cr base push hex dup cell+ @ 8 .r
  space 2 cells + count type ;
: .fonts fonts LIST> .font ;

\ Font resources                                       10apr04py
[defined] x11 [IF]
: font16@ ( addr -- icon ) cell+ dup cell+ count
  2 pick @  IF  2drop @  EXIT  THEN
  X-font16 new tuck swap ! ;

: font16-assign ( o addr -- o )  font16@ over >font ;

: (font16" ( o -- o )
  r> dup cell+ cell+ count + aligned >r font16@ over >font ; restrict
[defined] doNotSin [IF] doNotSin [THEN]

: font16" ( "font"<"> -- )  ?font ?dup
  0= IF   postpone (font16" fonts @ here fonts ! A, 0 , ," align
  ELSE    postpone ALiteral postpone font16-assign '"' parse 2drop  THEN ;
                                            immediate restrict
[THEN]

\ File icons                                           10apr04py

AVariable icons
: icon? ( addr u -- icon/0 )  icons >r
  BEGIN  r> @ dup  WHILE  >r
         2dup r@ cell+ cell+ count compare 0= UNTIL
  2drop r>  EXIT  THEN  nip nip ;

: ?icon ( -- addr/0 )  >in @ '"' parse  icon?  swap >in ! ;
: icon@ ( addr -- icon ) cell+ dup cell+ count
  2 pick @  IF  2drop @  EXIT  THEN
  icon-pixmap new tuck swap ! ;

: (icon" ( -- )  r> dup cell+ cell+ count + aligned >r icon@ ; restrict
[defined] doNotSin [IF] doNotSin [THEN]
: (icon@ ( -- )  r> dup cell+ >r @ icon@ ;             restrict
[defined] doNotSin [IF] doNotSin [THEN]

\ File icons                                           10apr04py

: icon" ( "file"<"> -- )  ?icon ?dup
  0= IF   postpone (icon" icons @ here icons ! A, 0 , ," align
  ELSE    postpone (icon@ A, '"' parse 2drop  THEN ;
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
  ELSE   A, ficon@-does '"' parse 2drop  THEN ;

ficon: dot-dir icons/dot-dir"
ficon: dotdot-dir icons/dotdot-dir"
ficon: diro-icon icons/diropen"
ficon: term-w icons/script"
ficon: dir-icon icons/dir"
ficon: file-icon icons/file"
ficon: sym-icon icons/sym"

\ File icons                                           10apr04py

: icon-table ( -- )  Create
  DOES> ( i -- icon )  swap $1F and cells + perform ;

icon-table ficons
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' dir-icon  A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' sym-icon  A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' diro-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' sym-icon  A,  ' file-icon A,
' file-icon A,  ' file-icon A,  ' file-icon A,  ' file-icon A,

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
\ normal font scheme                                   21jun05py
[defined] x11 [IF]
: (normal-font ( -- )  screen xrc with
    [defined] has-utf8 [IF]
	maxascii $80 = IF
	    S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso10646-1"
	    0 font!  \ normal font
	    S" -misc-fixed-medium-r-semicondensed-*-*-120-*-*-c-*-iso10646-1"
	    1 font!  \ terminal font
	    S" -adobe-helvetica-medium-r-normal-*-*-80-*-*-p-*-iso10646-1"
	    2 font!  \ icon font
	    S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso10646-1"
	    3 font!  \ text font
	    0" -adobe-helvetica-*-r-*-*-*-120-*-*-*-*-*-*,-misc-fixed-*-r-*-*-*-130-*-*-*-*-*-*" fontset!
	ELSE
	    S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
	    0 font!  \ normal font
	    S" -misc-fixed-medium-r-semicondensed-*-*-120-*-*-c-*-iso8859-1"
	    1 font!  \ terminal font
	    S" -adobe-helvetica-medium-r-normal-*-*-80-*-*-p-*-iso8859-1"
	    2 font!  \ icon font
	    S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
	    3 font!  \ text font
	    0" -adobe-helvetica-*-r-*-*-*-120-*-*-*-*-*-*,-misc-fixed-*-r-*-*-*-130-*-*-*-*-*-*" fontset!
	THEN
    [ELSE]
	S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
	0 font!  \ normal font
	S" -misc-fixed-medium-r-semicondensed-*-*-120-*-*-c-*-iso8859-1"
	1 font!  \ terminal font
	S" -adobe-helvetica-medium-r-normal-*-*-80-*-*-p-*-iso8859-1"
	2 font!  \ icon font
	S" -adobe-helvetica-bold-r-normal-*-*-120-*-*-p-*-iso8859-1"
	3 font!  \ text font
	0" -adobe-helvetica-*-r-*-*-*-120-*-*-*-*-*-*,-misc-fixed-*-r-*-*-*-130-*-*-*-*-*-*" fontset!
    [THEN]
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

\ fonts                                                29jul07py

[defined] win32 [IF]
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

\ alert                                                10apr04py

[defined] VFXFORTH [IF] cell [THEN]
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

Create alert-icons
       ' error-alert A,   ' question-alert A, ' warning-alert A,
       ' fatal-alert A,   ' info-alert A,     ' none-alert A,

Create alert-titles
        ," Error!"      ," Question?"   ," Warning!"
        ," Fatal!"      ," Info"        ," None"

\ alert boxhandler                                     10aug05py
: alert-text ( $1 .. $n n -- o )  dup { n }
0 ?DO  n 2* I - 1- dup >r roll r> roll text-label new LOOP
  n vbox new ;
: alert-buttons ( $1 .. $n n -- o default )  dup { n }
  0 ?DO  n 2* I - 1- dup >r roll r> roll I alertbutton new
  LOOP  n dup pick >r hatbox new hskip
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
[defined] x11 [IF]
  0" Alert" dup sp@ xrc dpy @ xwin @ rot XSetClassHint 2drop
[THEN]
  mousemap  stop ( dispose ) endwith  alert# @ ;

\ boxhandler                                           10apr04py

[defined] VFXFORTH 0= [IF]
Variable ?showpath  ?showpath on

| : scr# ( -- addr len ) scr @ abs extend
    <# #S s"  klB" bounds  DO  I c@ hold  LOOP  #> ;

: boxhandler  ( addr -- )  tflush
  dup count here count  ?showpath @
  IF    scratch "back 3  ELSE  2  THEN    loaderr @
  IF    >r  scr#  r> 1+ s" Cancel" s" Editor" 2
  ELSE  s"  OK " 1  THEN  0 alert >r (error
  r> 1 = IF " V" find 0= IF drop ELSE execute THEN THEN ;
[THEN]

