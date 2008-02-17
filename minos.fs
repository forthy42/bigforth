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
\ Color system                                         06jan05py

Create colors
       $0001 , $0203 ,  \ focus and defocus for buttons
       $0405 , $0607 ,  \ focus and defocus for labels
       $0809 , $0A0B ,  \ border colors
       $0C0D , $0E0F ,  \ color for texts&cursor
       $0D0D , $0D0D ,  \ revers text&cursor colors
DOES>  swap cells + ;

Variable redraw-all             redraw-all off
\ Variable kbstate
$1000000 Value maxpixmap \ 16 MB pixmap buffers
Variable dummy

\ text scratch                                         28jul07py

0 Value scratch
$4000 Constant scratch#
: char$ ( char -- addr u )  scratch xc!+ scratch tuck - ;

\ Color system - RGB table                             25mar99py

$FF Value red   $FF Value green $FF Value blue
&90 Value contrast
-1 Value twoborders
: set-col ( r g b -- ) to blue to green to red ;
: grayish ( -- )   $C0 $C0 $C0 set-col &92 to contrast ;
: redish  ( -- )   $CE $43 $10 set-col &90 to contrast ;
: bluish  ( -- )   $37 $CE $FC set-col &90 to contrast ;
: bisquish ( -- )  $E6 $D8 $A3 set-col &90 to contrast ;

grayish

\ Color system - RGB table                             31mar99py

: +contrast ( n -- )  &100 swap 0 ?DO  contrast &100 */  LOOP ;
: -contrast ( n -- )  &100 swap 0 ?DO  &100 contrast */  LOOP ;
: cx ( n -- ) dup 0< IF negate -contrast ELSE +contrast THEN
  $FF min ;
Create graytable   -$10 cx c,  -3 cx c,  -1 cx c,
                      0 cx c,   7 cx c,   6 cx c,   0 c,
: re-gray  6 7 0 -1 -3 -$10
  6 0 DO  cx graytable I + c!  LOOP ;
: rgb ( r g b -- rgb )  8 << or 8 << or ;
: gray  ( n -- rgb )  graytable + c@ >r
  r@ red &100 */ $FF min  r@ green &100 */ $FF min
  r> blue &100 */ $FF min  rgb ;

\ Color>RGB RGB>color                                  01jan05py

1 Value color-round
5 Value reds    5 Value greens  5 Value blues
: rgbs  ( end start -- ) $10 reds greens blues * * bounds ;
: >rgb ( n -- rgb )  $10 - reds /mod greens /mod
  rot $1FE reds   1- */ color-round + 2/ $FF min
  rot $1FE greens 1- */ color-round + 2/ $FF min
  rot $1FE blues  1- */ color-round + 2/ $FF min rgb ;
: rgb> ( r g b -- n )
  blues  1- $FF */ blues  1- min greens *  swap
  greens 1- $FF */ greens 1- min + reds *  swap
  reds   1- $FF */ reds   1- min +
  $10 + ;
Defer cfix
:noname ( n -- n' ) $FF and dup 8 << + ; IS cfix

\ Color system - RGB table                             04aug99py

Create Pixmaps  $10 0 [DO] 0 , [LOOP]

Create Colortable
       6 gray , 2 gray , \ Text, background focus button
       6 gray , 3 gray , \ Text, background defocus button
       6 gray , 2 gray , \ Text, background focus menu
       6 gray , 3 gray , \ Text, background defocus menu
       1 gray , 5 gray , \ Highlight/shadow border button
       3 gray , 6 gray , \ Highlight/shadow border label
       6 gray , 0 gray , \ Text color/Cursor in focus
       4 gray , 3 gray , \ Text color/Cursor defocus
       $F0 cells allot
: !rgbs ( -- )  dp push  Colortable $10 cells + dp !
  rgbs DO i >rgb , LOOP ; \ standard colors

\ Color system - RGB table  X11 version                09aug99py
[IFDEF] x11
Create color  sizeof XColor allot
       DoRed DoGreen DoBlue or or color XColor flags c!
: get-rgbs ( cmap dpy array end start -- )
  ?DO  Colortable I cells + c@+ c@+ c@
       cfix -rot  cfix swap  cfix
       color XColor red  w!+ w!+ w!
       color 2over XAllocColor drop
       color @ over ! cell+  LOOP  drop 2drop ;
: get-color ( cmap dpy array size -- )  0 get-rgbs ;
: get-stcolor ( cmap dpy array -- )  !rgbs  rgbs get-rgbs ;
Create syscolors 6 c, 2 c, 6 c, 3 c, 6 c, 2 c, 6 c, 3 c,
                 1 c, 5 c, 3 c, 6 c, 6 c, 0 c, 4 c, 3 c,
: (get-sys-colors ( -- )   re-gray
  $10 0 DO  I syscolors + c@ gray Colortable I cells + ! LOOP ;

\ Color system - TrueColor                             05jun06py
$FFFF Value alpha
: >subc ( 16bcol mask -- color )
  swap dup $10 << or  alpha dup $10 << or um* nip 1+
  over um* nip 1+ and ;
: make-rgbs ( vis array end start -- )
  ?DO  Colortable I cells + c@+ c@+ c@
       cfix -rot  cfix swap  cfix
       4 pick Visual red_mask   @ >subc -rot
       4 pick Visual green_mask @ >subc -rot
       4 pick Visual blue_mask  @ >subc or or
       2 pick Visual red_mask @+ @+ @ or or tuck and
       alpha rot invert >subc or
       over ! cell+  LOOP  2drop ;
: make-color ( vis array size -- )  0 make-rgbs ;
: make-stcolor ( vis array -- )  !rgbs  rgbs make-rgbs ;

\ X timer correction                                   23apr06py

&060 Value sameclick
&150 Value twoclicks
&6 Value samepos
&31 Value XA_STRING
&31 Value XA_STRING8
also dos
: XTime ( -- time )  timeval timezone gettimeofday
  timeval 2@ &1000 * swap &1000 / + ;
previous
: get-td ( win dpy -- n )  >r >r
  S" round delay trip" swap 0 8 &31 &16 r> r> 2dup >r >r
  XChangeProperty drop
  scratch PropertyChangeMask r> r> XWindowEvent drop
  XTime scratch XPropertyEvent time @ - ;

\ X timer correction                                   07jan07py

: get-tds ( win dpy count -- min max )
  scratch 2over XGetWindowAttributes drop
  PropertyChangeMask
  scratch XSetWindowAttributes event_mask dup @ >r !
  scratch 2over CWEventMask -rot XChangeWindowAttributes drop
  $7FFFFFFF $80000000 rot 0
  ?DO  2over get-td tuck max -rot min swap  LOOP 2swap
  r> scratch XSetWindowAttributes event_mask !
  scratch CWEventMask 2swap XChangeWindowAttributes drop ;

NotUseful ( WhenMapped ) Value backing-mode

[THEN]

\ Color system - RGB table  win32 version              24oct99py
[IFDEF] win32

Create sys-colors
       COLOR_WINDOWTEXT dup , ,
                       COLOR_WINDOW , COLOR_3DFACE ,
       COLOR_WINDOWTEXT dup , ,       COLOR_3DFACE dup , ,
       COLOR_HIGHLIGHTTEXT dup , ,    COLOR_HIGHLIGHT dup , ,
       COLOR_MENUTEXT dup , ,         COLOR_3DFACE dup , ,
       COLOR_3DHILIGHT dup , ,        COLOR_3DSHADOW dup , ,
       COLOR_3DFACE dup , ,           COLOR_WINDOWTEXT dup , ,
       COLOR_WINDOWTEXT dup , ,       COLOR_WINDOW dup , ,
       COLOR_GRAYTEXT dup , ,         COLOR_3DFACE dup , ,

\ Color system - RGB table  win32 version              23oct99py

: (get-sys-colors ( -- )
  $10 0 DO  sys-colors I 2* cells + 2@
            GetSysColor swap GetSysColor
            3 0 DO  over $FF and over $FF and + 2/ -rot
                    8 >> swap 8 >>  LOOP  2drop
            8 << or 8 << or
            Colortable I cells + !  LOOP ;

\ Color system - RGB table  win32 version              19aug97py

: get-brushs ( array size start -- )
  DO  Colortable Ith CreateSolidBrush
      over ! cell+  LOOP  drop ;
: get-pens ( array size start -- )
  DO  Colortable Ith 0 PS_SOLID CreatePen
      over ! cell+  LOOP  drop ;
: get-rgbs ( array size start -- )
  DO  Colortable Ith
      over ! cell+  LOOP  drop ;

: get-brush ( array size -- )   0 get-brushs ;
: get-pen ( array size -- )     0 get-pens ;
: get-rgb ( array size -- )     0 get-rgbs ;

\ Color system - RGB table  win32 version              19jan00py

: get-stbrush ( array -- )      rgbs get-brushs ;
: get-stpen ( array -- )        rgbs get-pens ;
: get-strgb ( array -- )        !rgbs  rgbs get-rgbs ;

&060 Value sameclick
&150 Value twoclicks
&6 Value samepos
Variable emulate-3button
$001100A6 Value :srcand         $00440328 Value :srcor

: ?err  ( flag -- )
  0= IF  base push hex  GetLastError ?dup
         IF  cr 9 u.r  THEN  THEN ;
[THEN]

\ keyboard handling                                    22aug99py
[IFDEF] x11
: shift-keys? ( key -- flag )  dup $FF7E $FF80 within
  over $FFE1 $FFEF within or  swap $FE00 $FE10 within or ;
Create xswa sizeof XSetWindowAttributes allot
CWBackingStore CWEventMask or CWBitGravity or CWWinGravity or
CWBackPixel or CWBorderPixel or CWColormap or Value xswavals
CWBackPixel CWBorderPixel or
CWBackPixmap or CWBorderPixmap or             Value xswavalvis
CWBackPixel CWBorderPixel or CWColormap or CWEventMask or
                                              Constant glxvals
XC_top_left_arrow Value mouse_cursor            [THEN]
[IFDEF] win32   : shift-keys? ( key -- flag )  drop false ;
IDC_ARROW         Value mouse_cursor            [THEN]
2Variable txy   0 0 txy 2!      Variable spot
Patch get-sys-colors    ' (get-sys-colors IS get-sys-colors

\ polygon draw utilities                               22jun02py

Variable (poly'
Variable (poly#
[IFDEF] win32   Variable (poly''  [THEN]
2Variable (lastp

: <poly ( x y -- x y )  (poly# off (poly' @ 0=
  IF  $1000 NewPtr (poly' !
      [IFDEF] win32  $2008 NewPtr (poly'' !  (poly' @ off [THEN]
  THEN   2dup (lastp 2! ;
: poly' ( -- addr )  (poly' @ (poly# @ cells + ;
: poly, ( dx dy -- )  2dup (lastp 2@ p+ (lastp 2!
  1 (poly# +!   swap poly' w!+ w! ;
: poly# ( x y -- )  (lastp 2@ p- poly, ;
: poly> ( -- addr n )  (poly' @ (poly# @ 1+ ;

\ bezier path                                          22jun02py

1 loadfrom splines.fb

Variable (bezier#

: <bezier ( -- )  (poly# @ (bezier# ! ;
: bezier> ( n -- )  (lastp 2@ poly# >r
  (poly# @ (bezier# @ - >r  (lastp 2@
  r@ 0 ?DO  2dup -1 (poly# +! poly' wx@+ wx@ p-  LOOP
  2drop r> r> >bezier >r 2dup (lastp 2!
  r> 0 ?DO  poly#  LOOP ;

\ gadget                                               06feb00py

0 Value ^^
: ^>^^ ( -- )  r> & ^^ push  ^ to ^^ >r ;

debugging class gadget
public: cell var x              cell var y
        cell var w              cell var h
        static /step            static shadowcol
        static focuscol         static defocuscol
        ptr widgets             ptr parent
        method hglue            method vglue
        method hglue@           method vglue@
        method >released        method dpy!
        method font!

\ gadget                                               27jul97py

        method close            method xywh
        method resize           method repos
        method assign           method get
        method clicked          method keyed
        method inside?          method handle-key?
        method focus            method defocus
        method show             method hide
        method delete           method show-you
        method append           method draw
        method next-active      method prev-active
        method first-active     method text!
        method xinc             method yinc
        method resized          method !resized
        method moved            method leave

\ widget class                                         27jun02py
how:    &40 /step V!            4 colors shadowcol !
        0 colors focuscol !     1 colors defocuscol !
        : repos  ( x y -- )  y ! x ! ;
        : range  ( n min glue -- n' )  over >r + min r> max ;
        : append ( o before -- )  widgets self over =
          IF    swap bind widgets  widgets bind widgets
                parent self widgets bind parent
          ELSE  widgets goto append  THEN ;
        : delete ( addr addr' -- )  over self =
          IF    widgets self swap ! drop
          ELSE  drop link widgets  widgets goto delete  THEN ;
        : resize ( x y w h -- )
          vglue@ range swap hglue@ range swap
          h ! w ! repos ;
        : show-you ( -- ) ;

\ widget class                                         05jan07py
        : focus ;
        : defocus ;
        : close  ;
        : xywh ( -- x y w h ) x @ y @ w @ h @ ;
        : inside? ( x y -- f )
          y @ - h @ 0max u< swap x @ - w @ 0max u< and ;
        : draw ( -- ) ;
        : clicked ( x y b n -- )  2drop 2drop ;
        : keyed   ( key state -- )  2drop ;
        : assign ;
        : get ;
        : handle-key? ( -- flag )  false ;
        : next-active  false ;
        : prev-active  false ;
        : first-active ;

\ widget class                                         23oct99py
        : hglue  ( -- min glue )  w @ 0 ;
        : vglue  ( -- min glue )  h @ 0 ;
        : hglue@ ( -- min glue )  hglue ;
        : vglue@ ( -- min glue )  vglue ;
        : xinc   ( -- o inc ) 0 1 ;
        : yinc   ( -- o inc ) 0 1 ;
        : !resized ;
        : leave  ( -- ) ;
        : moved  ( x y -- )  2drop ;
        : text! ( addr n -- )  2drop ;
        : font! ( font -- ) drop ;
        : show ;
        : hide ;
        : resized  parent resized ;
class;

\ Event data structure                                 09mar99py
[IFDEF] x11
Create Handlers  MappingNotify [FOR] ' noop A, [NEXT]
        KeyPressMask  KeyReleaseMask or \ KeymapStateMask or
        ButtonPressMask or ButtonReleaseMask or
        EnterWindowMask or LeaveWindowMask or
        PointerMotionMask or ExposureMask or
       ( VisibilityChangeMask or ) StructureNotifyMask or
       ( ResizeRedirectMask or ) PropertyChangeMask or
        FocusChangeMask or \ ColormapChangeMask or
Constant event-mask
Create event here sizeof XEvent dup allot erase
Variable event-time
[THEN]
0 AValue 'nilscreen             0 AValue 'nil
$20 Constant maxclicks

\ Error handling                                       09jan00py
[IFDEF] x11
Create err-event here sizeof XEvent dup allot erase
Variable err-dpy
Code X-error  R:  4 SP D) AX mov  AX err-dpy A#) mov
     SI push  DI push  $10 SP D) SI mov  err-event A# DI mov
     sizeof XErrorEvent # CX mov  .b rep movs
     DI pop   SI pop  ret  end-code
: Xerr$ ( n -- )  $80 strerrbuf rot err-dpy @ XGetErrorText drop
  strerrbuf dup 0>c" "error ! ;
' Xerr$ error$s 3 cells + !

\ Error handling                                       07jan07py

Forward screen-sync
Forward screen-ic!

: .Xerror" ( -- )
  $80 strerrbuf err-event XErrorEvent error_code c@ err-dpy @
  XGetErrorText drop strerrbuf >len
  ." X Error: " type cr  err-dpy off ;
: .Xerror ( -- )  screen-sync  err-dpy @ 0= ?EXIT  .Xerror" ;
: .Xerror~~ screen-sync  err-dpy @ 0= IF  2drop 2drop EXIT  THEN
  (~~) .Xerror" ;
: x~~  ~~, postpone .Xerror~~ ; immediate
[THEN]

\ Event data structure                                 29jul07py
[IFDEF] win32
Create event sizeof MSG allot
Create Handler 0 A,

: Handler, ( start end -- ) 1+ here negate 3 and allot
  here Handler @ A, Handler ! 2dup , , swap
  ?DO  ( ['] DefWindowProc ) 0 A,  LOOP ;
: Handler@ ( msg -- addr )  Handler @
  BEGIN  dup  WHILE  2dup cell+ 2@ within
         IF  cell+ dup cell+ @ rot swap - 2+ cells +  EXIT  THEN
         @  REPEAT  2drop 0 ;

\ Event data structure                                 29jul07py
WM_NULL              WM_NOTIFYFORMAT     Handler,
WM_KEYDOWN           WM_SYSDEADCHAR      Handler,
WM_MOUSEMOVE         WM_MOUSEWHEEL       Handler,
WM_PARENTNOTIFY      WM_DEVICECHANGE     Handler,
WM_WINDOWPOSCHANGING WM_WINDOWPOSCHANGED Handler,
WM_NCMOUSEMOVE       WM_NCMOUSEMOVE      Handler,
WM_TIMER             WM_TIMER            Handler,
\ WM_IME_SETCONTEXT    WM_IME_KEYDOWN      Handler,

\ : Ime-Handler ( lparam wparam msg wnd -- ret )
\   ~~ 2over 2over ImmIsUIMessage 0=
\   IF  ~~ DefWindowProc  ELSE  2drop 2drop 0  THEN ;
\ WM_IME_KEYDOWN 1+ WM_IME_SETCONTEXT [DO]
\   ' Ime-Handler [I] Handler@ !  [LOOP]

\ Callback                                             29jul07py
Forward win>o
Variable 'event-task

: do-callback ( lparam wparam msg wnd -- result )
  dup win>o op!  ^ IF  over Handler@ ?dup  ELSE  false  THEN
  IF  @ ?dup IF  catch IF  ?error clearstack 0  THEN  EXIT  THEN
      DefWindowProc EXIT  THEN  DefWindowProc ;

Code WinProc ( R: lparam wparam msg wnd ret -- result ) R:
     SI push  UP push  OP push  sys-sp A#) push
     SP sys-sp A#) mov   -$2000 SP D) SI lea
     3 [FOR] -$1000 SP D) SP lea  SP ) DX mov  [NEXT]
     $2000 SI D) SP lea  ;c: 'up @ up!  rp@ $3F00 - sys-sp !
     rp@ $14 + @+ @+ @+ @ swap 2swap swap
     ( lparam wparam msg wnd )  s0 @ >r sp@ $10 + s0 !

\ other Win32 parts                                    28jul07py

     do-callback
     depth 1 <> IF  depth >r clearstack sys-sp @ $18 + @ r> ~~
                    2drop 0  THEN  r> s0 ! ( u o si ret res )
     >c: R:  sys-sp A#) pop  OP pop  UP pop  SI pop
     $10 # ret  end-code
: >lohi ( lparam -- lo hi ) dup $FFFF and wextend swap $10 >> ;
: hilo> ( lo hi -- lparam ) $10 << swap $FFFF and or ;

Create Xform0  $3F800000 , 0 , 0 , $3F800000 , 0 , 0 , 0 , 0 ,

\ Unicode support                                      28jul07py

: utf16> ( addr u -- addr' u' )  0 0 2swap
  swap scratch# scratch 2swap 0
  maxascii $80 = IF  CP_UTF8  ELSE  CP_ISO_8859_1  THEN
  WideCharToMultiByte scratch swap ;
: >utf16 ( addr u -- addr' u' )
  swap scratch# 2/ scratch 2swap 0
  maxascii $80 = IF  CP_UTF8  ELSE  CP_ISO_8859_1  THEN
  MultiByteToWideChar scratch swap 2dup 2* + 0 swap w! ;
: >wlen ( addr -- addr u )
  dup BEGIN w@+ swap 0= UNTIL over - 2- ;
Create minosw  'M w, 'I w, 'N w, 'O w, 'S w, 0 w,
Create popupw  'P w, 'o w, 'p w, 'u w, 'p w, 0 w,
[THEN]

\ abstract datatype: font object                       26may02py

debugging class font
public: method assign ( specific -- )
        method draw   ( addr u x y dpy -- )
        method size   ( addr u -- w h )
        early select  ( addr u color -- )
        early display ( x y dpy -- )
        cell var addr           cell var u
        cell var color
how:
        : select   color ! u ! addr ! ;
class;

Defer new-font
[IFDEF] x11   Defer new-font16  [THEN]

\ XResource                                            03jul07py

[IFDEF] x11
Create vis# $8 c, $10 c, $20 c, $18 c,
debugging class xresource
public: cell var fontarray      cell var colarray
        cell var clone?         cell var cursors
        cell var dpy            cell var screen
        cell var gc             cell var cmap
        cell var xN             cell var xM
        cell var hM             cell var #rgbs
        cell var timeoffset     cell var imdata
        cell var vis            cell var depth
        cell var cur-color      cell var xim
        cell var im             cell var ic
        cell var fontset

\ XResource                                            01jan05py

        method open             method close
        method connect          method calibrate
        method get-gc           method cursor
        method color            method colors
        method font!            method font16!
        method fid              method font@
        early !font             method fontset!
        method free-colors      method clone
        method set-color        method set-tile
        method set-function     method get-visual
        early best-visual       early best-cmap
        early best-im           method get-ic
        early font[]

\ XResource                                            28jul07py

how:    : init ( -- )
          $400 NewHandle colarray !
           $40 NewHandle fontarray !
          XC_num_glyphs cells NewHandle cursors !
          cursors @ @ XC_num_glyphs cells erase
          scratch 0= IF  scratch# & scratch Handle!  THEN ;
        : dispose ( -- )  clone? @ 0=
          IF  colarray  @ DisposHandle
              fontarray @ DisposHandle
              cursors   @ DisposHandle  THEN
          super dispose ;
        : close ( -- )  dpy @ XCloseDisplay drop ;
        : color ( i --  n )  $FF and cells colarray @ @ + @ ;
        : font@ ( i --  n )  cells fontarray @ @ + @ ;

\ XResource                                            17sep07py

        : open ( string -- )  [ also DOS ]
          6 0" " setlocale 0= abort" Cannot set locale"
          XOpenDisplay dup dpy ! 0= abort" Can't connect"
          XSupportsLocale IF
             s" XMODIFIERS" env$ drop ?dup IF
             XSetLocaleModifiers  0= IF
                ." Warning: Cannot set locale modifiers to '"
                s" XMODIFIERS" env$ type  ." '" cr THEN  THEN
          THEN [ toss ] ;

\ XResource                                            03jul07py
        Create visinfo sizeof XVisualInfo allot
        : best-visual ( -- vis depth screen )
          0 0 sp@ pad 0 dpy @ XGetVisualInfo
          swap sizeof XVisualInfo * bounds
          4 0 DO  TrueColor I vis# + c@ 8 = + 1+ StaticGray DO
                  2dup ?DO  I XVisualInfo depth @ K vis# + c@ =
                            I XVisualInfo class @ J = and
                            IF  rot drop I -rot THEN
          sizeof XVisualInfo +LOOP   LOOP  LOOP   2drop
          dup 0= IF drop  dpy @ screen @ DefaultVisual
                          dpy @ screen @ DefaultDepth screen @
                  ELSE    dup XVisualInfo visual @ swap
                          dup XVisualInfo depth  @ swap
                              XVisualInfo screen @  THEN ;

\ XResource                                            11aug99py

        : best-cmap ( -- cmap )
          AllocNone vis @
          dpy @ screen @ RootWindow dpy @ XCreateColormap ;
        : tmp-win ( -- win )
          dpy @ XFlush  0 dpy @ XSync 2drop
          cmap @ xswa XSetWindowAttributes colormap !
          0      xswa XSetWindowAttributes background_pixel !
          1      xswa XSetWindowAttributes border_pixel !

          xswa CWBackPixel CWBorderPixel or CWColormap or
          vis @ InputOutput depth @ 2 &100 dup 0 0
          dpy @ screen @ RootWindow dpy @ XCreateWindow

          dpy @ XFlush  0 dpy @ XSync 2drop ;

\ XResource                                            22oct07py

        : best-im ( -- im )  0 0 0 dpy @ XOpenIM dup xim !
          IF  0 0
              sp@ 0 swap XNQueryInputStyle xim @ XGetIMValues_1
              drop dup >r w@+ 2+ @ swap cells bounds ?DO
                  I @ dup XIMPreedit and 0<>
                  swap XIMStatusNothing and
                  0<> and IF  drop I @ LEAVE  THEN
              cell +LOOP dup 0= IF ." No style found" cr  THEN
              r> XFree drop ELSE 0 THEN  ;

\ XResource                                            27jan07py

        : get-ic ( win -- ) xim @ 0= IF  drop  EXIT  THEN
          ic @ IF
             0 swap XNFocusWindow ic @ XSetICValues_1 drop
             EXIT  THEN  >r
          0 ( r@ XNClientWindow ) r> XNFocusWindow
          0 spot XNSpotLocation
            fontset @ XNFontSet 0 XVaCreateNestedList_2 >r
          r@ XNPreeditAttributes
          im @ XNInputStyle xim @ XCreateIC_3 dup ic !
          screen-ic!
          r> XFree drop
          ic @ ?dup IF  XSetICFocus drop  THEN ;

\ XResource                                            22mar03py

        Create xgc  sizeof XGCValues allot
        : connect ( -- )
          ['] X-error XSetErrorHandler drop
          dpy @ DefaultScreen          screen !
          dpy @ screen @ DefaultColormap cmap !
          dpy @ screen @ DefaultGC         gc !
          best-visual over dpy @ screen @ DefaultDepth <>
          IF    screen ! depth ! vis !  best-cmap cmap !
                xgc 0 tmp-win dup >r dpy @ XCreateGC gc !
                r> dpy @ XDestroyWindow drop
          ELSE  drop depth ! dpy @ screen @ DefaultVisual vis !
                xswavals xswavalvis xor to xswavals drop
          THEN  best-im im !
          0 &38 dpy @ XKeycodeToKeysym drop ;

\ XResource                                            01jan05py

        : !font ( -- )  fontarray @ @ @ font with
          s" n" size  s" M" size  endwith  hM ! xM ! drop xN ! ;
        : font[] ( n -- fid )  cells fontarray @ @ + ;
        : font! ( params n -- )
          >r  r@ font[] >r
          r@ @ 0= IF    new-font r> !
                  ELSE  r> @ font with assign endwith  THEN
          r> 0= IF  !font  THEN ;
        : font16! ( params n -- )
          >r  r@ font[] >r
          r@ @ 0= IF    new-font16 r> !
                  ELSE  r> @ font with assign endwith  THEN
          r> 0= IF  !font  THEN ;
        : fid  dpy @ 2 cells + @ ;

\ XResource                                            19dec04py

        : fontset! ( addr -- )  >r
          0 sp@ >r 0 sp@ >r 0 sp@ r> r> r> dpy @
          XCreateFontSet fontset ! 2drop XFreeStringList drop ;
\          0 ?DO  ." Missing charset: " dup Ith >len type cr
\          LOOP  drop ;

\ XResource                                            27jul04py
        : calibrate ( -- )
          dpy @ DefaultRootWindow
          dpy @ 4 get-tds drop negate timeoffset @ ! ;
        : colors ( -- )  vis @ Visual class @ TrueColor >=
          IF   vis @ colarray @ @ $10         make-color
               vis @ colarray @ @ $10 cells + make-stcolor
          ELSE cmap @ dpy @ colarray @ @ $10         get-color
               cmap @ dpy @ colarray @ @ $10 cells + get-stcolor
          THEN rgbs drop #rgbs ! ;
        : free-colors  vis @ Visual class @ TrueColor >= ?EXIT
          0 #rgbs @ colarray @ @ cmap @ dpy @ XFreeColors drop ;
        : cursor ( n -- shape )
          dup cells cursors @ @ + @ dup IF  nip EXIT  THEN
          drop dup dpy @ XCreateFontCursor tuck swap cells
          cursors @ @ + ! ;

\ XResource                                            27jun02py
        : set-color ( index -- )
\          dup cur-color @ = IF  drop  EXIT  THEN
          dup cur-color !
                    [ xgc XGCValues foreground ] ALiteral !
          FillSolid [ xgc XGCValues fill_style ] ALiteral !
          xgc  [ GCForeground GCFillStyle or ] Literal
          gc @ dpy @ XChangeGC drop ;
        : set-function ( n -- )
          gc @ dpy @ XSetFunction drop ;
        : get-gc ( win -- )
          1 depth @ &24 min << 1-
                    [ xgc XGCValues background ] ALiteral !
          xgc GCBackground rot dpy @ XCreateGC gc ! !font ;
        : get-visual ( -- visual depth )
          vis @ depth @ ;

\ XResource                                            22sep07py

        : clone ( -- o )  dpy @ screen @
          fontarray @ gc @ cmap @ timeoffset @ xn @
          xM @ colarray @ cursors @ imdata @
          vis @ depth @  xim @ im @ fontset @ new >o
          colarray @  DisposHandle
          fontarray @ DisposHandle
          cursors @   DisposHandle
          fontset ! im ! xim !  depth ! vis !
          imdata ! cursors !  colarray !  xM !
          xn ! timeoffset ! cmap ! gc ! fontarray !
          screen ! dpy !  clone? on  ^ o> ;
class;
[THEN]

\ win32 Resource                                       20apr99py
[IFDEF] win32
debugging class xresource
public: cell var fontarray      cell var colarray
        cell var penarray       cell var cursors
        cell var rgbarray
        cell var dc             cell var inst
        cell var xN             cell var xM
        cell var hM             cell var clone?
        method register         method init-dc
        method get-gc           method clone
        method color            method colors
        method font!            method fid
        method font@            early !font
        method cursor           method free-colors
        method rgb              method pen

\ win32 Resource                                       28jul07py
how:    : init ( -- )   0 GetModuleHandle inst !
          $400 NewHandle colarray !
          $400 NewHandle penarray !
          $400 NewHandle rgbarray !
           $40 NewHandle fontarray !
          [ IDC_HELP IDC_ARROW - 1+ cells ] Literal
          dup NewHandle cursors ! cursors @ @ swap erase
          GetDesktopWindow get-gc
          scratch 0= IF  scratch# & scratch Handle!  THEN ;
        : dispose ( -- )  clone? @ 0=
          IF  colarray  @ DisposHandle \ dc @ ReleaseDC drop
              penarray  @ DisposHandle
              rgbarray  @ DisposHandle
              fontarray @ DisposHandle
              cursors   @ DisposHandle  THEN  super dispose ;

\ win32 Resource                                       06nov99py

        : color ( i --  n )  $FF and cells colarray @ @ + @ ;
        : pen   ( i --  n )  $FF and cells penarray @ @ + @ ;
        : rgb   ( i --  n )  $FF and cells rgbarray @ @ + @ ;
        : colors ( -- )
          rgbarray @ @ dup $10 get-rgb   $40 + get-strgb
          colarray @ @ dup $10 get-brush $40 + get-stbrush
          penarray @ @ dup $10 get-pen   $40 + get-stpen ;
        : cursor ( n -- shape )
          dup IDC_ARROW -
          cells cursors @ @ + @ dup IF  nip EXIT  THEN
          drop dup 0 LoadCursor tuck swap IDC_ARROW - cells
          cursors @ @ + ! ;
        : fid  0 ;

\ win32 Resource                                       29aug99py

        : font@  ( i --  n )  cells fontarray @ @ + @ ;
        : !font ( -- )  fontarray @ @ @ font with
          s" n" size  s" M" size  endwith  hM ! xM ! drop xN ! ;
        : font! ( addr u n -- )
          >r  r@ cells fontarray @ @ + >r
          r@ @ 0= IF    new-font r> !
                  ELSE  r> @ font with assign endwith  THEN
          r> 0= IF  !font  THEN ;

\ win32 Resource                                       20may99py

        : init-dc ( -- )
          TRANSPARENT dc @ SetBkMode       drop
          ALTERNATE   dc @ SetPolyFillMode drop
          0 font@     dc @ SelectObject    drop ;
        : clone ( -- o )
          inst @ fontarray @ dc @ xN @  xM @
          colarray @ penarray @ rgbarray @ cursors @  new >o
          colarray @  DisposHandle   penarray @  DisposHandle
          fontarray @ DisposHandle   rgbarray @  DisposHandle
          cursors @   DisposHandle
          cursors !  rgbarray !  penarray !  colarray !
          xM ! xN ! dc ! fontarray ! inst !  clone? on
          init-dc ^ o> ;
        : get-gc ( win -- )  GetDC dc !  init-dc ;

\ win32 Resource                                       17aug97py

        : free-colors ( -- )  colarray @ @ $90 cells bounds
          DO  I @ DeleteObject drop I off  cell +LOOP
          penarray @ @ $90 cells bounds
          DO  I @ DeleteObject drop I off  cell +LOOP ;

\ win32 Resource                                       28jul07py
        Create bf-class here sizeof WNDCLASS dup allot erase
                        CS_OWNDC bf-class WNDCLASS style !
        : register ( icon -- ) [ also DOS ]
          ['] WinProc bf-class WNDCLASS lpfnWndProc !
\          0 bf-class WNDCLASS cbClsExtra !
\          0 bf-class WNDCLASS cbWndExtra !
          inst @ bf-class WNDCLASS hInstance !
          bf-class WNDCLASS hIcon !
          IDC_ARROW 0 LoadCursor bf-class WNDCLASS hCursor !
\          0 bf-class WNDCLASS hbrBackground !
\          0 bf-class WNDCLASS lpszMenuName !
          minosW bf-class WNDCLASS lpszClassName !
          bf-class RegisterClassW drop [ toss ] ;
class;                                          [THEN]

\ selection                                            13mar99py

Variable selection

: nextpow2 ( n -- x~2 )  1  BEGIN  2* 2dup <  UNTIL  nip ;

: (@select  ( -- addr n )  selection @ dup 0=
  IF  drop S" "  ELSE  @+ swap  THEN ;

: +select ( addr n -- )  selection @ 0=
  IF    dup cell+ nextpow2 selection Handle!
        dup selection @ !  selection @ cell+
  ELSE  selection 2dup @ @ + cell+ 1+ nextpow2 SetHandleSize
        #lf selection @ dup @ + cell+ tuck c!
        over 1+ selection @ +! 1+
  THEN  swap move ;

\ selection                                            16jan05py

[IFDEF] x11
Variable own-selection

: post-selection ( addr n win dpy -- ) swap >r >r
  swap 2dup r@ XStoreBytes drop
  PropModeReplace 8 XA_STRING 9 r@ DefaultRootWindow
  r@ XChangeProperty drop
  event-time @ r> r> 1 rot XSetSelectionOwner drop
  own-selection on ;

\ selection                                            23apr06py
Variable got-selection          Variable str-selection
Forward screen-event
: wait-for-select ( -- flag )  got-selection off
  &5000 after  BEGIN  screen-event
           timeout? got-selection @ or UNTIL  drop ;
: fetch-property ( prop win dpy -- )
  over2 0= IF  str-selection @ IF  2drop drop got-selection on
                                   str-selection off  EXIT  THEN
            str-selection on >r nip CurrentTime swap
            1 XA_STRING8 1 r> XConvertSelection drop  EXIT  THEN
  >r >r >r 0 sp@ >r 0 sp@ >r 0 sp@ >r 0 sp@ >r 0 sp@
  r> r> r> r> AnyPropertyType 1 $10000 0 r> r> r>
  XGetWindowProperty 0=
  IF  nip dup >r swap +select 2drop r> XFree drop
  ELSE  drop 2drop 2drop  THEN  got-selection on ;

\ selection                                            23apr06py

: @select ( win dpy -- addr n )
  own-selection @
  IF    2drop
  ELSE  >r  selection HandleOff
        1 r@ XGetSelectionOwner 0=
        IF    0 sp@ r> XFetchBytes tuck swap +select XFree 2drop
        ELSE  event-time @ swap
              1 XA_STRING 1 r> XConvertSelection drop
              wait-for-select
        THEN
  THEN  (@select ;
[THEN]

\ selection                                            28jul07py
[IFDEF] win32
: post-selection ( addr u win dpy -- ) 2drop
  0 OpenClipboard drop  EmptyClipboard drop
  dup IF  >utf16
          dup 2* 2+ [ GMEM_MOVEABLE GMEM_DDESHARE or ] Literal
          GlobalAlloc >r
          r@ GlobalLock swap 2dup 2* + 0 swap w! 2* move
          r@ GlobalUnlock drop r>
          CF_UNICODETEXT SetClipboardData drop
  ELSE  2drop  THEN  CloseClipboard drop ;
: @select ( win dpy -- addr n )  2drop
  selection @ IF  selection HandleOff  THEN
  0 OpenClipboard drop CF_UNICODETEXT GetClipboardData >r
  r@ GlobalLock >wlen 2/ utf16> +select
  r> GlobalUnlock drop  CloseClipboard drop (@select ;  [THEN]

\ selection                                            06feb00py

: !select ( win dpy -- )
  (@select  2swap post-selection ;

: -select ( -- )  selection HandleOff ;

&30 Value minwait

0 Value event-task

\ Display                                              05aug99py
gadget class displays
public: ptr dpy                 ptr childs
        xresource ptr xrc       ptr nextwin
        cell var xwin           cell var cur-cursor
        cell var rw             cell var rh
        cell var mx             cell var my
        cell var mb             cell var exposed
        cell var !moved         cell var clicks
        cell var lastclick      cell var lasttime
        cell var pending        cell var draw?
        cell var tx             cell var ty
        cell var clipregion     cell var counter
        cell var clip-is        cell var clip-should
        cell var clip-r         gadget ptr pointed
        font ptr cur-font       cell var events

\ Display                                              11nov06py
[IFDEF] xrender
        cell var xpict          method ?xpict   [THEN]
[IFDEF] x11   sizeof XRectangle var xrect
        cell var lastcal        cell var timeoffset    [THEN]
[IFDEF] win32   sizeof rect       var xrect
        cell var style          cell var owner  [THEN]
        cell var xft-draw

\ Display                                              11nov06py
        method line             method text
        method image            method box
        method mask             method fill
        method stroke           method drawer
        method drawable         \ method ximage
        method mouse            method screenpos
        method trans            method trans'
        method transback        method sync
        method set-cursor       method set-rect
        method set-color        method set-font
        method set-hints        method set-linewidth
        method moreclicks       method transclick

\ Display                                              04aug05py
        method clip-rect        method txy!
        method get-event        method handle-event
        method schedule-event   method size-event
        method click            method click?
        method moved?           method moved!
        method get-win          method get-dpy
        method click~           method flush-queue
        method show-me          method scroll
        method clipx            method clipy
        method geometry         method geometry?
        method >exposed         method child-moved
[IFDEF] win32                   method win>o     [THEN]
        early xS                method mxy!
        method schedule         method invoke
        method cleanup          method do-idle

\ Display                                              06feb00py
how:    : dispose  clicks HandleOff
          xrc dispose  super dispose ;
        : xS ( -- n )  xrc xN @ 2+ 2/ 2/ ;
        : dpy! ( dpy -- )  bind dpy !resized ;
        | Variable schedules-root
        : cleanup ( o -- )  >r events
          BEGIN  dup @ WHILE
                 dup @ cell+ cell+ @ r@ =
                 IF  dup @ dup >r @ over !
                     r> schedules-root DelFix
                 ELSE  @  THEN
          REPEAT  drop rdrop ;
[IFDEF] x11
        : ?calib ( -- )  lastcal @ 0= XTime lastcal @ - &60000 >
          or  IF  xrc calibrate XTime lastcal !  THEN ;  [THEN]

\ Display tasking                                      29aug99py
        : schedule ( xt object time -- )
          schedules-root 4 cells $40 NewFix dup >r cell+ !
          r@ 2 cells + 2!   events
          BEGIN  dup @  WHILE
                 dup @ cell+ @ r@ cell+ @ - 0<  WHILE
                 @  REPEAT  THEN
          dup @ r@ ! r> swap ! ;
        : invoke ( -- ms )  events @
          IF    events @ cell+ @ timer@ - dup 0<
                IF    events @ dup @ events ! dup 2 cells + 2@
                      rot schedules-root DelFix >o send o>
                ELSE  >us &1000 m/mod nip  THEN
          ELSE  minwait  THEN  -1 max minwait min ;
        : do-idle ( n -- ) dup 0>
          IF  xrc fid swap idle  ELSE  drop sync  THEN ;

\ Display tasking                                      09jul00py

        : event-task  $20000 $10000 NewTask activate
          >tib off $100 newtib  up@ TO event-task
          Onlyforth dynamic   " event-task" r0 @ cell+ !
[IFDEF] win32    up@ 'event-task !              [THEN]
          BEGIN  depth >r ['] handle-event catch
                 ?dup IF  .  "error @ ?dup
                          IF  count type  THEN  "error off
                          cr ['] .except catch drop
                          cr ['] .back   catch drop  THEN
                 depth r> <> IF  ~~  THEN  clearstack
[IFDEF] x11      err-dpy @ IF  .Xerror  THEN  ?calib   [THEN]
                 ['] invoke catch drop do-idle
          AGAIN ;
        : set-hints ;

\ Display                                              07jan07py

        : init ( resource -- )  bind xrc  self bind dpy
          'nilscreen bind childs  'nilscreen bind nextwin
[IFDEF] x11
          xrc dpy @ dup xrc screen @ RootWindow xwin !
          xrc screen @ 2dup DisplayWidth  w ! DisplayHeight h !
[THEN]
[IFDEF] win32
          GetDesktopWindow xwin !
          0 0 0 0 sp@ xwin @ GetWindowRect drop 2drop w ! h !
[THEN]
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase
          -1 rw ! -1 rh !  -1 cur-cursor !
          event-task ;

\ Display                                              27jun02py

        : append  ( addr -- )  childs self swap  bind childs
          childs bind nextwin  self childs bind parent ;
        : delete  ( addr -- )  link childs  childs delete ;
        : show-me ( x y -- )  2drop ;
        : scroll ( x y -- )  2drop ;
        : clipx ( -- x w ) 0 w @ ;
        : clipy ( -- y h ) 0 h @ ;
        : ALLCHILDS ( .. -- ..' )  childs self
          BEGIN  dup 'nilscreen <>  WHILE
                 r@ swap >o execute nextwin self o>
          REPEAT  drop rdrop ;
        : !resized 0 set-font  xrc !font  ALLCHILDS  !resized ;
        : resized  ALLCHILDS  resized ;
        : draw     ALLCHILDS  draw ;

\ Display                                              05jan07py

        : get-win [IFDEF] win32  xrc dc @
                  [ELSE]  xwin @  [THEN] ;
        : ?clip ( -- )  clip-is @ clip-should @ <>
          IF  0 clip-rect   self >o
              BEGIN   clip-is @  clip-is off  clip-should off
                      dup op!  0= UNTIL  o>
              ( clip-should @ clip-is ! )  THEN ;
        : txy! ( x y -- )  txy 2@ p+ ty ! tx ! ;
        : geometry ( w h -- )
          x @ y @ swap resize ;
        : geometry? ( -- w h )  w @ h @ ;
        : transback ;    : trans ;   : trans' ;
        : set-rect ( o -- )  bind pointed ;

\ Display                                              18jul99py

        : clip-box ( x y w h -- x' y' w' h' ) >xyxy
          clipy over + >r max r> min >r
          clipx over + >r max r> min >r
          clipy over + >r max r> min >r
          clipx over + >r max r> min r> r> r> >xywh ;

        : drawer ( x y o xt -- )  ?clip
          ^ rot >o swap execute o> ;

\ Xrender extras                                       11nov06py

[IFDEF] xrender
        Create pict_attrib sizeof XRenderPictureAttributes allot
        1 pict_attrib XRenderPictureAttributes dither !
        : ?xpict ( -- )  xpict @ ?EXIT  xrc dpy @ xwin @
          over dup dup DefaultScreen DefaultVisual
          XRenderFindVisualFormat
          $800 pict_attrib XRenderCreatePicture xpict ! ;
        Create dummy_rect 0 w, 0 w, $7FFF w, $7FFF w,
        : ?pclip ( -- )  xrc dpy @ xpict @ clip-r @ dup
          IF    XRenderSetPictureClipRegion
          ELSE  drop 0 0 dummy_rect 1
                XRenderSetPictureClipRectangles
          THEN ;
[THEN]

\ Display                                              11nov06py

[IFDEF] x11
        : drawable ( -- gc win dpy ) xrc gc @ xwin @ xrc dpy @ ;
        : set-font ( n -- )  xrc font@ bind cur-font ;
        : set-color ( color -- )  ?clip $FF and dup $10 <
          over cells Pixmaps + @ and ?dup
          IF    nip  tx @ ty @ rot xrc set-tile
          ELSE  xrc color xrc set-color  THEN ;
        : set-cursor ( n -- )
          cur-cursor @ over = IF  drop  EXIT  THEN
          dup cur-cursor !
          xrc cursor drawable rot drop XDefineCursor drop ;
        : set-linewidth ( n -- ) >r
          1 0 0 r> drawable nip XSetLineAttributes drop ;

\ Display                                              21aug99py

        : line ( x y x y color -- )  set-color
          swap 2swap swap drawable XDrawLine drop ;
        : text ( addr u x y color -- )  set-color
          self cur-font draw ;
        : box ( x y w h color -- )  set-color \ clip-box
          swap 2swap swap drawable XFillRectangle drop ;
        : fill ( x y array n color -- )  set-color
          2swap swap 2over drop w!+ w!
          CoordModePrevious Complex 2swap swap
          drawable XFillPolygon drop ;
        : stroke ( x y array n color -- )  set-color
          2swap swap 2over drop w!+ w!
          CoordModePrevious -rot swap drawable XDrawLines drop ;

\ Display                                              11nov06py

        : clip-mask ( y x w -- )  drawable nip XSetClipMask drop
          drawable nip XSetClipOrigin drop ;
        : mask { xs ys w h x y win1 win2 |  ?clip
          win1 0= IF
              y x h w ys xs drawable win2 swap XCopyArea drop
[IFDEF] xrender  ELSE  win1 -1 = IF
              ?xpict ?pclip
              xrc dpy @ 3 win2 0 xpict @ xs ys xs ys x y w h
              XRenderComposite  [THEN]

\ Display                                              21mar04py

          ELSE  clip-is @ 0= IF  y x win1 clip-mask
              y x h w ys xs drawable win2 swap XCopyArea drop
              0 0 0 clip-mask
          ELSE  1 xrc set-function  0 xrc set-color
              1 y x h w ys xs drawable win1 swap XCopyPlane drop
              6 xrc set-function
              y x h w ys xs drawable win2 swap XCopyArea drop
              3 xrc set-function
          THEN THEN [IFDEF] xrender THEN [THEN] } ;
        : image { xs ys w h x y win |  ?clip
          y x h w ys xs drawable win swap XCopyArea drop } ;
[THEN]

\ Display                                              27jun02py

[IFDEF] win32
        : drawable ( -- dc ) xrc dc @ ;
        : set-font ( n -- )  xrc font@ bind cur-font ;
        : set-color ( color -- )  ?clip
          dup xrc color drawable SelectObject drop
          dup xrc pen   drawable SelectObject drop
              xrc rgb   drawable SetTextColor drop ;
        : set-cursor ( n -- )
          cur-cursor @ over = IF  drop  EXIT  THEN
          dup cur-cursor !
          xrc cursor dup SetCursor drop
          GCL_HCURSOR xwin @ SetClassLong drop ;
        : set-linewidth drop ;

\ Display                                              27jun02py

        : line ( x y x y color -- )      set-color
          2swap swap 0 -rot drawable MoveToEx ?err
          swap drawable LineTo ?err ;
        : text ( addr u x y color -- )   set-color
          self cur-font draw ;
        : box ( x y w h color -- )
          >r 2dup 1 1 d= IF
               2drop r> xrc rgb -rot swap drawable SetPixel drop
               EXIT  THEN  r>  set-color
          2over p+ swap 2swap swap drawable Rectangle ?err ;

\ Display                                              27jun02py

        : make-path ( x y addr n color -- n polygon )
          set-color
          2over swap (poly'' @ 2!
          (poly'' @ 2 cells + -rot  dup >r  cells bounds
          ?DO  I wx@+ wx@ \ swap dup 0< + dup 0> - swap
               rot >r p+ 2dup swap r> !+ !+
               cell +LOOP  drop 2drop
          r> 1+ (poly'' @ ;
        : fill ( x y array n color -- )  make-path
          drawable Polygon  ?err ;
        : stroke ( x y array n color -- )  make-path
          drawable Polyline ?err ;

\ Display                                              10aug02py

\        : ?gc ( win -- gc )  dup GetDC dup 0=
\          IF  drop  ELSE  nip  THEN ;
        : mask { xs ys w h x y win1 win2 | ?clip
          :srcand ys xs win1 \ ?gc
          h w y x drawable BitBlt ?err
          :srcor  ys xs win2 \ ?gc
          h w y x drawable BitBlt ?err  } ;
        : image { xs ys w h x y win |      ?clip
          $00CC0020 ys xs win \ ?gc
          h w y x  drawable BitBlt ?err } ;
[THEN]

\ Display       Clipping rectangle x11                 12may02py
[IFDEF] x11
        : rect>reg ( rect -- r )  XCreateRegion
          dup >r tuck swap XUnionRectWithRegion drop r> ;
        : clip-r? ( -- )  clipregion @ clip-r @ <>
          clip-r @ and  IF  clip-r @ XDestroyRegion drop  THEN ;

        : clip-rect ( rect -- )  clip-r? clipregion @
          IF    dup IF    rect>reg >r r@ clipregion @ r@
                          XIntersectRegion drop r>
                    ELSE  drop clipregion @  THEN
          ELSE  dup IF    rect>reg
                    ELSE  drop None drawable nip XSetClipMask
                          drop clip-r off EXIT  THEN
          THEN  dup clip-r ! drawable nip XSetRegion drop ;
[THEN]

\ Display       Clipping rectangle win32               20oct99py
[IFDEF] win32
        : clip-rect ( rect -- )  clipregion @
          IF    dup IF    w@+ w@+ w@+ w@ 2over p+
                          swap 2swap swap
                          CreateRectRgn >r
                          RGN_AND r@ clipregion @ r@
                          CombineRgn drop
                          r@ xrc dc @ SelectClipRgn drop
                          rdrop \ no need to destroy it?
                    ELSE  drop clipregion @ xrc dc @
                          SelectClipRgn drop  THEN
          ELSE  dup IF    w@+ w@+ w@+ w@ 2over p+
                          swap 2swap swap CreateRectRgn
                    THEN  xrc dc @ SelectClipRgn drop
          THEN ;                                [THEN]

\ Display                                              25jan03py

[IFDEF] x11
        : add-region ( x y w h -- )
\          base push cr hex xwin @ . decimal
\          ." : add region " 4 0 DO I pick . LOOP
          clipregion @ dup 0= IF  drop XCreateRegion  THEN  >r
          swap 2swap swap  xrect  w!+ w!+ w!+ w!
          r@ dup xrect  XUnionRectWithRegion drop
          clipregion @ clip-r @ = IF  r@ clip-r !  THEN
          r> clipregion ! ;

\ Display                                              12may02py

        : size-event ( -- )
          rw @ -1 <> rh @ -1 <> or
          IF  clipregion @ ?dup IF XDestroyRegion drop THEN
              clip-r? clipregion off  clip-r off  0 clip-rect
              w @ rw @ <>  h @ rh @ <> or
              IF  xywh 2drop rw @ rh @ resize  THEN  draw
              -1 rw !  -1 rh !  THEN
          clipregion @
          IF  clipregion @ dup >r
              xrc gc @ xrc dpy @ XSetRegion drop   draw
              clip-r? clipregion off  clip-r off
              r> XDestroyRegion drop  0 clip-rect  THEN
          nextwin goto size-event ;
[THEN]

\ Display                                              28mar99py

[IFDEF] win32
        : size-event ( -- )
          rw @ -1 <> rh @ -1 <> or
          IF  w @ rw @ dup w ! <>  h @ rh @ dup h !  <> or
              IF  xywh resize draw  THEN
              -1 rw !  -1 rh !  THEN ;
[THEN]

\ Display                                              07jan07py
[IFDEF] x11
        : get-event ( mask -- ) dup >r
          IF  BEGIN  event r@ xrc dpy @ XCheckMaskEvent  WHILE
                     0 event XFilterEvent
                     0= IF  childs handle-event  THEN  REPEAT
          ELSE  BEGIN  xrc dpy @  XPending  WHILE
                       event xrc dpy @ XNextEvent drop
                       0 event XFilterEvent
                       0= IF  childs handle-event  THEN
                REPEAT  childs size-event  \ drop
          THEN  rdrop ;
        : sync ( -- )  0 xrc dpy @ XSync drop ;
        : mouse ( -- x y b )  0 sp@ >r 0 sp@ >r 0 sp@
          r> r> dummy dup dup dup xwin @ xrc dpy @
          XQueryPointer drop 8 >> ;             [THEN]

\ Display                                              18oct98py

[IFDEF] win32
        : check-events ( event -- event )
          ALLCHILDS  dup get-event ;
        : get-event ( event -- )
          check-events drop ;
        : sync ( -- ) ;
        : mouse ( -- x y b )  QS_MOUSEMOVE  get-event
          mx @ my @ mb @ ;

        : win>o ( win -- win o )
          0 ALLCHILDS  over xwin @ = IF  drop ^  THEN ;
[THEN]

\ Display                                              07jan07py

        : screenpos ( -- x y )  x @ y @ ;
        : schedule-event ( -- )  flush-queue
          clicks @ @  IF   click clicked  THEN
          moved?  IF  child-moved  THEN
          nextwin goto schedule-event ;
        : handle-event ( -- )
          0 get-event
          childs schedule-event
          ( invoke ) ;
        : moved!  !moved on ;
        : moved?  ( -- flag )  !moved @ !moved off ;
        : get-dpy ( -- addr )  ^ ;
        : mxy! ( mx my -- ) my ! mx ! ;

\ Display                                              04aug05py
[IFDEF] x11
        :noname  event XMotionEvent time @ event-time !
          event XMotionEvent x @+ @ mxy!
          event XMotionEvent state @ 8 >> mb !  moved! ;
        MotionNotify cells Handlers + !
        | 2Variable comp_stat
        | Variable look_key
        | Create look_chars $100 allot

\ Display                                              04jan07py

        :noname ( -- ) \ cr 'd emit 'o emit
          event XKeyEvent time @ event-time !
          comp_stat look_key $FF look_chars event
[IFDEF] has-utf8  xrc ic @ dup
          IF    Xutf8LookupString
          ELSE  drop XLookupString  THEN
[ELSE]    XLookupString  [THEN]
          ?dup IF  look_chars swap bounds ?DO
                   I xc@+ swap >r event XKeyEvent state @ keyed
               r> I - +LOOP  EXIT  THEN
          look_key @ event XKeyEvent state @ keyed ;
        KeyPress cells Handlers + !

\ Display                                              11sep05py

        :noname \ cr ." mapping notify"
          event XRefreshKeyboardMapping drop ;
        MappingNotify cells Handlers + !
        : click~ ( -- event )  clicks @ @+ swap 8* + ;
        : transclick ( x y -- x' y' ) ;
        : sendclick ( count event -- )  pending on  click~ >r >r
          r@ XButtonEvent state @
          r@ XButtonEvent button @ $80 swap << xor
          r@ XButtonEvent x @
          r> XButtonEvent y @ transclick swap
          r> w!+ w!+ w!+ w!  ;
        : !xyclick ( event -- )  click~ >r
          dup XButtonEvent x @ swap
              XButtonEvent y @ transclick swap r> w!+ w! ;

\ Display                                              05aug99py

        : in-time? ( time flag -- flag )
          lasttime @ rot - swap lastclick @ =
          IF  sameclick  ELSE  twoclicks  THEN  < ;
        : samepos? ( event -- flag )  pending @
          IF    XButtonEvent x @+ @  click~ w@+ w@ p-
                dup * swap dup * + samepos <
          ELSE  drop false  THEN ;
        : moreclicks ( -- )
          clicks @ @ maxclicks 1- < negate clicks @ +! ;
        : flush-queue ( -- )  XTime xrc timeoffset @ @ +
          lasttime @ - twoclicks > pending @ and
          IF  click~ 6+ w@ 1 and
              IF  moreclicks  THEN  pending off  THEN  ;
        : +clicks ( -- ) click~ 6+ dup w@ 2+ -2 and swap w! ;

\ Display                                              09mar99py
        : .button cr base push hex event XButtonEvent window
          @+ @+ @ swap rot xwin @ 9 .r 9 .r 9 .r 9 .r
          space event XButtonEvent x @+ swap . @ . ;
        :noname ( -- )  event XButtonEvent time @ event-time !
          event XButtonEvent time @ dup true in-time?
          swap lasttime !  IF   event samepos?
               IF  lastclick @
                   IF    1 event XButtonEvent button @ 1- <<
                         click~ 5 + dup c@ rot or swap c!
                   ELSE  click~ 6 + w@ -2 and 1+ event sendclick
                         lastclick on
                   THEN  EXIT  THEN   event !xyclick pending off
          THEN  pending @  IF  moreclicks  THEN
          1 event sendclick lastclick on ;
          ButtonPress cells Handlers + !

\ Display                                              09mar99py
        :noname ( -- )  event XButtonEvent time @ event-time !
          event XButtonEvent time @ dup 0 in-time?
          swap lasttime !
          IF  event samepos?  IF  lastclick @
              IF    +clicks  lastclick off
                    moreclicks 2 event sendclick
              ELSE  click~ 6+ w@ 1 and
                    IF  1 event XButtonEvent button @ 1- <<
                        click~ 5 + dup c@ rot invert and swap c!
                    THEN  THEN  EXIT  THEN  click~ 6+ w@  1 and
              ELSE true THEN  \ output push display .button
          IF  event !xyclick +clicks moreclicks  THEN
          pending @ 0= IF  pending push  THEN
          2 event sendclick  lastclick off ;
          ButtonRelease cells Handlers + !

\ Display                                              28jun98py

        : click?  ( -- n )  clicks @ @ 0=
          IF  0 get-event  THEN  clicks @ @ ;
        : click   ( -- x y b n )
          BEGIN  pause click?  UNTIL
          -1 clicks @ +!  clicks @ cell+ wx@+ wx@+ c@+ c@+ w@
          rot kbshift !
          clicks @ $C + dup 8 - clicks @ @ 8* move ;

\ Display                                              04aug05py

        :noname
          event XExposeEvent x @+ @+ @+ @  add-region
\         event XExposeEvent count @ 0= IF ."  draw"  draw  THEN
          exposed on ;
        dup Expose         cells Handlers + !
            GraphicsExpose cells Handlers + !
\        :noname  pointed self
\          IF  mx @ my @ pointed moved  THEN ;
\        EnterNotify    cells Handlers + !
        :noname   pointed self
          IF  pointed leave  0 bind pointed  moved? drop  THEN ;
        LeaveNotify    cells Handlers + !

\ Display                                              23apr06py

        Create xev  here  sizeof XEvent  dup allot erase

        :noname \ cr  ." Selection Notify "
          event XSelectionRequestEvent time @ event-time !
          event XSelectionEvent property @
          event XSelectionEvent requestor @
          xrc dpy @ fetch-property ;
        SelectionNotify cells Handlers + !

        :noname \ cr  ." Selection Clear "
          own-selection off ;
        SelectionClear  cells Handlers + !

\ Display                                              16jan05py

: rest-request ( n addr mode format type -- )
  event XSelectionRequestEvent property @ dup >r
  event XSelectionRequestEvent requestor @
  xrc dpy @ XChangeProperty drop sync
  r> xev XSelectionEvent property ! ;
: string-request ( -- )
  (@select swap PropModeReplace 8 XA_STRING rest-request ;
: string8-request ( -- )
  (@select swap PropModeReplace 8 XA_STRING8 rest-request ;
: compound-request ( -- )  string-request ;

\ Display                                              23apr06py

| Create 'string XA_STRING , XA_STRING8 ,
: target-request ( -- )
  XA_STRING8 XA_STRING 'string 2!
  2 'string PropModeReplace &32 4 rest-request ;

\ Display                                              16jan05py
        :noname \ cr  ." Selection Request "
          event XSelectionRequestEvent time @ event-time !
          event xev 4 cells move
          event XSelectionRequestEvent requestor
          xev XSelectionEvent requestor 6 cells move
          xev XSelectionEvent property off
          SelectionNotify xev XSelectionEvent type !
          event XSelectionRequestEvent target @ xrc dpy @
          XGetAtomName >len  BEGIN \ output push display
          2dup s" STRING"  str= IF  string8-request LEAVE  THEN
      2dup s" UTF8_STRING" str= IF  string-request  LEAVE  THEN
          2dup s" TARGETS" str= IF  target-request  LEAVE  THEN
          2dup s" COMPOUND_TEXT" str=
                                IF  compound-request LEAVE THEN
          DONE  ( 2dup type cr ) 2drop

\ Display                                              07jan05py

          xev 0 0 event XSelectionRequestEvent requestor @
          xrc dpy @ XSendEvent drop ;
        SelectionRequest cells Handlers + !

\ Display                                              07jan07py

        :noname  exposed on ;  NoExpose cells Handlers + !
        :noname ( -- ) \ resize request
           event XConfigureEvent x @ x !
           event XConfigureEvent y @ y !
           event XConfigureEvent width  @ rw !
           event XConfigureEvent height @ rh ! ;
        ConfigureNotify cells Handlers + !
        ' focus    FocusIn  cells Handlers + !
        ' defocus  FocusOut cells Handlers + !
        : >exposed ( -- )  sync  exposed off
          BEGIN  ( ExposureMask ) 0 get-event
                 pause  exposed @  UNTIL ;

\ Display                                              02aug98py
        :noname ( -- )
          event sizeof XClientMessageEvent dump ;
        ClientMessage cells Handlers + !

[THEN]

\ Display                                              19jan00py
[IFDEF] win32        Create paint  $40 allot
        :noname ( lparam wparam msg win -- ret )
          xrc dc @ >r  paint xwin @ BeginPaint xrc dc !
          Xform0 xrc dc @ SetWorldTransform drop
          draw  paint xwin @ EndPaint drop  r> xrc dc !
          2drop 2drop 0 exposed on ;         WM_PAINT Handler@ !
        :noname  3 pick >lohi y ! x ! DefWindowProc ;
                                             WM_MOVE  Handler@ !
        :noname  2drop 2drop close 0 ;       WM_CLOSE Handler@ !
        :noname  3 pick WINDOWPOS flags @ SWP_NOSIZE and
          IF  DefWindowProc  EXIT  THEN  2drop drop
          WINDOWPOS cx 2@ 0. 0. sp@ 0 style @ rot
          AdjustWindowRect drop p- p- rw ! rh !
          size-event 0 ;          WM_WINDOWPOSCHANGED Handler@ !

\ Display                                              28jul07py
        :noname 2drop drop { rect |
          vglue >r hglue >r 0 0 sp@ >r 0 style @ r>
          AdjustWindowRect drop p- rect 2@ p+
          dup r> + 2 pick r> + >r >r
          rect 2 cells + 2@ rot max >r max r>
          r> min swap r> min swap
          yinc >r xinc >r 0 0 sp@ >r 0 style @ r>
          AdjustWindowRect drop p- rect 2@ p+
          rot over - r@ 2/ + r@ / r> * + -rot swap
              over - r@ 2/ + r@ / r> * + swap
          rect 2 cells + 2! 0 } ;           WM_SIZING Handler@ !
 \        :noname ( lparam wparam msg win -- ret )
 \        DefWindowProc ;          WM_INPUTLANGCHANGE Handler@ !
 \       :noname ( lparam wparam msg win -- ret )
 \        DefWindowProc ;   WM_INPUTLANGCHANGEREQUEST Handler@ !

\ Display                                              19jan00py

        : >kshift ( n -- n' )  VK_SHIFT - 1 swap <<
          dup 1 <> IF  2*  THEN ;
        : shift@ ( -- kbshift )  0
          VK_MENU 1+ VK_SHIFT ?DO
              I GetKeyState wextend 0< IF  I >kshift or  THEN
          LOOP ;
public: displays ptr grab-key
private:
        : ?keyed  grab-key self
          IF  grab-key keyed  ELSE  keyed  THEN ;
        : ?grab  grab-key self
          IF  r> grab-key self >o execute o>  THEN ;

\ Display                                              29jul07py

        | Create xkeys  $FF55 , $FF56 , $FF57 , $FF50 ,
                        $FF51 , $FF52 , $FF53 , $FF54 ,
                        0 ,     0 ,     0 ,     0 ,
                        $0000 , $007F ,
        :noname 2drop nip dup $21 $2F within
          IF    $21 - cells xkeys + @ ?dup
                IF  shift@ ?keyed  THEN
          ELSE  drop  THEN  0 ;            WM_KEYDOWN Handler@ !
        :noname  2drop nip shift@       ?keyed 0 ;
                                              WM_CHAR Handler@ !
 \       :noname  2drop nip shift@       ?keyed 0 ;
 \                                        WM_IME_CHAR Handler@ !
        :noname  2drop nip shift@ ( 8 or )  ?keyed 0 ;
                                           WM_SYSCHAR Handler@ !

\ Display                                              12aug00py

        : click~ ( -- event )  clicks @ @+ swap 8* + ;
        : >mshift ( fwkeys -- mstate ) 0
          over $01 and 0<> $100 and or
          over $02 and 0<> $400 and or
          over $10 and 0<> $200 and or
          dup $500 = emulate-3button @ and IF  $700 xor  THEN
          nip shift@ or ;
        : !xyclick ( event -- )  click~ >r
          MSG lparam @ >lohi swap r> w!+ w! ;
        : sendclick ( count event -- )
          pending on  click~ >r
          dup MSG wparam @ >mshift swap
              MSG lparam @ >lohi swap r> w!+ w!+ w!+ w! ;

\ Display                                              19jan00py
        : in-time? ( time flag -- flag )
          lasttime @ rot swap - swap lastclick @ =
          IF  sameclick  ELSE  twoclicks  THEN  < ;
        : samepos? ( event -- flag )  pending @
          IF    MSG lparam @ >lohi ( swap ) click~ w@+ w@ p-
                dup * swap dup * + samepos <
          ELSE  drop false  THEN ;
        : moreclicks ( -- )
          clicks @ @ maxclicks 1- < negate clicks @ +! ;
        : flush-queue ( -- )  GetTickCount
          lasttime @ - twoclicks > pending @ and
          IF  click~ 6+ w@ 1 and
              IF  moreclicks  THEN  pending off
              ( ReleaseCapture drop )  THEN  ;
        : +clicks ( -- ) click~ 6+ dup w@ 2+ -2 and swap w! ;

\ Display                                              19jan00py
        :noname ( lparam wparam msg win -- 0 ) ?grab \ add press
          SetCapture 2drop 2drop
          event MSG time @ dup true in-time?
          swap lasttime !
          IF   event samepos?
               IF  lastclick @
                   IF   event MSG wparam @ >mshift click~ 4 + w!
                   ELSE  click~ 6 + w@ -2 and 1+ event sendclick
                         lastclick on
                   THEN  0 EXIT  THEN event !xyclick pending off
          THEN  pending @  IF  moreclicks  THEN
          1 event sendclick lastclick on 0 ;
                                   dup WM_LBUTTONDOWN Handler@ !
                                   dup WM_RBUTTONDOWN Handler@ !
                                       WM_MBUTTONDOWN Handler@ !

\ Display                                              19jan00py
        :noname  2drop $13 and 0= IF ReleaseCapture drop THEN
          ?grab  drop event MSG time @ dup 0 in-time?
          swap lasttime !
          IF  event samepos?  IF  lastclick @
              IF    +clicks  lastclick off
                    moreclicks 2 event sendclick
              ELSE  click~ 6+ w@ 1 and
                    IF  event MSG wparam @ >mshift click~ 4 + w!
                    THEN  THEN 0 EXIT  THEN  click~ 6+ w@  1 and
              ELSE true THEN  \ output push display .button
          IF  event !xyclick +clicks moreclicks  THEN
          pending @ 0= IF  pending push  THEN
          2 event sendclick  lastclick off 0 ;
                                   dup WM_LBUTTONUP   Handler@ !
        dup WM_RBUTTONUP   Handler@ !  WM_MBUTTONUP   Handler@ !

\ mouse wheel                                          01jun07py
        : >wshift ( fwkeys -- count mstate ) dup >r >mshift
          r@ 0< $1000 and or  r@ 0> $800 and or
          r> 16 >> &60 / abs swap ;
        : sendwheel ( event -- )  pending on
          dup MSG wparam @ >wshift drop 0 ?DO
             dup   MSG wparam @ >wshift nip I 2+ swap
             over2 MSG lparam @ >lohi y @ - swap x @ -
             click~ w!+ w!+ w!+ w!
             moreclicks
          2 +LOOP drop ;
        :noname ( lparam wparam msg win -- ) moved!
          pending @ IF  moreclicks pending off  THEN
          event MSG time @  lasttime !
          2drop 2drop event sendwheel 0 ;
                                        WM_MOUSEWHEEL Handler@ !

\ Display                                              12aug00py
        : click?  ( -- n )  clicks @ @ 0=
          IF  0 get-event  THEN  clicks @ @ ;
        : click   ( -- x y b n )
          BEGIN  pause click?  UNTIL
          -1 clicks @ +!  clicks @ cell+ wx@+ wx@+ c@+ c@+ w@
          rot kbshift ! \ kb-shift off
          clicks @ $C + dup 8 - clicks @ @ 8* move
          ( 2over 2over cr . . . . ) ;

        :noname  2drop drop >r
          vglue + hglue +
          0. sp@ 0 style @ rot AdjustWindowRect drop p-
          r> $8 + 2! 0 ;
                                     WM_GETMINMAXINFO Handler@ !

\ Display                                              29jul07py
        :noname ( lparam wparam msg win -- ) ?grab moved!
          2drop >mshift $FF and mb ! >lohi mxy! 0 ;
                                         WM_MOUSEMOVE Handler@ !
        :noname  pointed self
          IF  pointed leave 0 bind pointed  THEN
          DefWindowProc ;              WM_NCMOUSEMOVE Handler@ !

        :noname focus   2drop 2drop 0 ;  WM_SETFOCUS  Handler@ !
        :noname defocus 2drop 2drop 0 ;  WM_KILLFOCUS Handler@ !
        :noname ( lparam wparam msg win -- )
          2drop 2drop get-sys-colors xrc free-colors
          xrc colors 0 ;            WM_SYSCOLORCHANGE Handler@ !
        : >exposed ;
[THEN]
class;

\ Display                                              21oct99py

displays ptr screen

[IFDEF] x11
: screen-event  ( -- )  0 screen get-event ;
[THEN]

[IFDEF] win32
: win>o ( win -- o )  screen win>o nip ;
[THEN]

\ font implementation                                  21aug99py

font implements
        : display  >r color @
          r@ displays with  set-color  endwith
          addr @ u @ 2swap r> draw ;
class;

\ X fonts                                              07dec04py

[IFDEF] x11
font class X-font
public: cell var name-string
        cell var id
        cell var ascent
how:    : assign ( addr u -- )  name-string $!
          0 name-string $@ + c!  name-string $@ drop
          screen xrc dpy @  XLoadQueryFont
          dup 0= abort" Font not found"
\          dup 0<= IF  drop 0 screen xrc font@
\                      with id @ endwith  THEN
          dup id ! XFontStruct ascent @ ascent ! ;
        : init ( addr u -- )  assign ;

\ X fonts                                              21aug99py

        | Create text_r sizeof XCharStruct allot
        | Variable font_d
        | Variable font_a
        | Variable dir_r
        : size ( addr u -- w h )  >r >r
          text_r font_d font_a dir_r
          r> r> swap id @ XTextExtents drop
          font_d @ font_a @ +
          text_r XCharStruct rbearing wx@
          text_r XCharStruct lbearing wx@ -
          text_r XCharStruct width w@ max swap ;

\ X fonts                                              12nov06py

        | Create text_i here sizeof XTextItem dup allot erase
        : draw ( addr u x y dpy -- )
          >r id @ XFontStruct fid @ text_i XTextItem font !
          2swap swap text_i XTextItem chars 2!
          ascent @ +
          r> displays with
             1 text_i 2swap swap drawable  XDrawText drop
          endwith ;
class;

: new-x-font ( addr u -- font ) x-font new ;
' new-x-font IS new-font

\ X fonts 16 bit                                       24oct99py

X-font class X-font16
how:    : size ( addr u -- w h )  >r >r
          text_r font_d font_a dir_r
          r> r> 2/ swap id @ XTextExtents16 drop
          font_d @ font_a @ +
          text_r XCharStruct rbearing wx@
          text_r XCharStruct lbearing wx@ -
          text_r XCharStruct width w@ max swap ;

\ X fonts 16 bit                                       26may02py

        : draw ( addr u x y dpy -- )
          >r id @ XFontStruct fid @ text_i XTextItem font !
          2swap 2/ swap text_i XTextItem chars 2!
          ascent @ +
          r> displays with
             1 text_i 2swap swap drawable  XDrawText16 drop
          endwith ;
class;

: new-x-font16 ( -- font ) x-font16 new ;
' new-x-font16 IS new-font16

[THEN]

\ win-font                                             28jul07py

[IFDEF] win32
font class win-font
public: cell var name-string
        cell var id
how:    : ?? ( flags n -- flag )  >> 1 and ; hmacro
        : assign ( addr u family flags width height -- )
          { family flags w h |
          name-string $! 0 name-string $@ + c!
          name-string $@ dup IF  drop  ELSE  nip  THEN
          family ANTIALIASED_QUALITY
          CLIP_DEFAULT_PRECIS OUT_TT_PRECIS DEFAULT_CHARSET
          flags 2 ??  flags 1 ??  flags 0 ??
          0 0 0 w h CreateFont }  id ! ;
        : init ( params -- ) assign ;

\ win-font                                             29jul07py
        : size ( addr u -- x y )  >utf16 >r >r
          id @ screen drawable SelectObject drop
          0. sp@ r> r> swap screen drawable GetTextExtentPoint32
          drop swap ;
        : draw ( addr u x y dpy -- )
          id @ swap displays with
              drawable SelectObject drop swap
              2swap >utf16 swap 2swap drawable TextOutW ?err
          endwith ;
class;
: new-win-font ( params -- font ) win-font new ;

\ win-font with X string convention                    12nov06py
slowvoc on
Vocabulary X-family             also X-family definitions
FF_DONTCARE    Constant *
FF_DECORATIVE  Constant decorative
FF_MODERN      Constant modern
FF_ROMAN       Constant roman
FF_SCRIPT      Constant script
FF_SWISS       Constant swiss   previous definitions
Vocabulary X-pitch              also X-pitch definitions
DEFAULT_PITCH  Constant *
FIXED_PITCH    Constant m
FIXED_PITCH    Constant c
VARIABLE_PITCH Constant p       previous definitions
Vocabulary X-charset            also X-charset definitions
DEFAULT_CHARSET     Constant *

\ win-font with X string convention                    23sep99py
ANSI_CHARSET        Constant iso8859-1
SYMBOL_CHARSET      Constant microsoft-symbol
SHIFTJIS_CHARSET    Constant jisx0208.1983-0
HANGEUL_CHARSET     Constant hangeul-0
GB2312_CHARSET      Constant gb2312.1980-0
CHINESEBIG5_CHARSET Constant big5-0
GREEK_CHARSET       Constant iso8859-7
TURKISH_CHARSET     Constant iso8859-9
HEBREW_CHARSET      Constant iso8859-8
ARABIC_CHARSET      Constant iso8859-6
BALTIC_CHARSET      Constant iso8859-4
RUSSIAN_CHARSET     Constant iso8859-5
THAI_CHARSET        Constant thai-0
EASTEUROPE_CHARSET  Constant iso8859-2
OEM_CHARSET         Constant oem-0  previous definitions

\ win-font with X string convention                    12nov06py
Vocabulary X-slant              also X-slant definitions
0 Constant r      1 Constant i    2 Constant u    3 Constant ui
4 Constant s      5 Constant si   6 Constant su   7 Constant sui
 0 Constant *                   previous definitions
Vocabulary X-weight             also X-weight definitions
FW_DONTCARE   Constant *
FW_THIN       Constant thin
FW_EXTRALIGHT Constant extralight
FW_LIGHT      Constant light
FW_NORMAL     Constant normal
FW_MEDIUM     Constant medium
FW_SEMIBOLD   Constant semibold
FW_BOLD       Constant bold
FW_EXTRABOLD  Constant extrabold
FW_HEAVY      Constant heavy    previous definitions slowvoc off

\ win-font with X string convention                    28jul07py

win-font class X-font
public: cell var win-name
how:    : -extract   '- skip 2dup '- scan 2swap 2 pick - ;
        : ?exec ( addr u wid -- ) dup >r search-wordlist
          IF  execute rdrop   ELSE  s" *" r> recurse  THEN ;
        : make-font ( family addr u wd s w h pitch chset -- id )
          { family addr u wd s w h pitch chset |
          addr u win-name $! 0 win-name $@ + c!
          u IF  win-name $@ drop  ELSE  0  THEN
          family pitch or ANTIALIASED_QUALITY
          CLIP_DEFAULT_PRECIS OUT_TT_PRECIS chset
          s 2 ??  s 1 ??  s 0 ??
          wd 0 0 0 w CreateFont } ;

\ win-font with X string convention                    28jul07py
        : assign ( addr u -- ) base push decimal  name-string $!
          name-string $@
          -extract & X-family ?exec -rot        \ foundry
          -extract 2swap                        \ family
          -extract & X-weight ?exec -rot        \ weight
          -extract & X-slant ?exec -rot         \ slant
          -extract 2drop                        \ adjstyl
          -extract 0. 2swap >number 2drop drop -rot \ width
          -extract 0. 2swap >number 2drop drop -rot \ pixelsize
          -extract 2drop  -extract 2drop  -extract 2drop
          -extract & X-pitch ?exec -rot         \ spacing
          -extract 2drop                        \ avgwidth
                   & X-charset ?exec  make-font  id ! ;  class;
: new-X-font ( params -- font )  X-font new ;
' new-X-font IS new-font                        [THEN]

\ backing                                              15aug99py

displays class backing
public: gadget ptr child        method create-pixmap
        cell var noback         cell var closing
        2 cells var hglues      2 cells var vglues
[IFDEF] win32                   cell var oldbm      [THEN]

\ backing                                              28aug99py

how:    : init ( -- ) ;
        : schedule ( xt o time -- )  dpy schedule ;
        : invoke ( -- time )  dpy invoke ;
        : cleanup ( o -- )  dpy cleanup ;
        : sync ( -- )   dpy sync ;
        : get-event ( mask -- )  dpy get-event ;
        : !txy ( -- ) tx @ ty @ trans' dpy txy! ;
        : !t00 ( -- ) 0 0 dpy txy! ;
        gadget :: delete

\ backing                                              28mar99py

        : get-glues ( -- )
          child hglue hglues 2!
          child vglue vglues 2! ;
        : dpy! ( dpy -- )
          bind dpy  xrc self IF  xrc dispose  THEN
          dpy xrc clone bind xrc  create-pixmap
[IFDEF] x11     get-win xrc get-gc  [THEN]
          0 clip-rect  draw? on  child self 0= ?EXIT
          self child dpy!  !resized
          child xywh  resize ;

\ backing store                                        11nov06py
[IFDEF] x11
        : create-pixmap ( -- )   xwin @
          IF  xwin @ xrc dpy @ XFreePixmap drop xwin off  THEN
          xpict @ IF  screen xrc dpy @ xpict @
                      XRenderFreePicture xpict off  THEN
          noback @ ?EXIT  xrc depth @
          h @ 1 max w @ 1 max dpy get-win xrc dpy @
          XCreatePixmap xwin ! ;
        : dispose ( -- )  child self  IF  child dispose  THEN
          xwin @  IF  xwin @ xrc dpy @ XFreePixmap drop  THEN
          super dispose ;
        : ?xpict ( -- )  xpict @ ?EXIT  xrc dpy @ xwin @
          over PictStandardRGB24 XRenderFindStandardFormat
          $800 pict_attrib XRenderCreatePicture xpict ! ;
[THEN]

\ backing store                                        11nov06py

[IFDEF] win32
        : create-pixmap ( -- )
          xwin @  IF  xwin @ DeleteObject drop xwin off  THEN
          noback @ ?EXIT
          h @ 1 max w @ 1 max
          screen xrc dc @ CreateCompatibleBitmap xwin !
          screen xrc dc @ CreateCompatibleDC xrc dc !
          xwin @ xrc dc @ SelectObject oldbm !  xrc init-dc ;
        : dispose ( -- )  child dispose
          xwin @  IF  xwin @ DeleteObject ?err
                      xrc dc @ DeleteDC ?err  THEN
          super dispose ;
[THEN]

\ backing store                                        06jul02py

        : child-dispose  child self IF child dispose THEN ;
        : set-child ( widget -- )  0 bind pointed
          ['] child-dispose catch IF .except THEN
          bind child  self child bind parent
          dpy self 0= IF  rdrop  THEN ;
        : rest-child ( -- )  get-glues  child xywh  resize ;

\ backing store                                        17dec00py
        : assign ( widget -- )  set-child
          self child dpy!  rest-child ;
        : resized
          get-glues child xywh resize parent resized ;
        : trans ( x y -- x' y' )  x @ y @ p- ;
        : trans' ( x y -- x' y' )  x @ y @ p+ ;
        : screenpos  dpy screenpos x @ y @ p+ ;
        : next-active   child next-active ;
        : prev-active   child prev-active ;
        : first-active  child first-active ;
        : clicked ( click -- ) 2swap trans 2swap child clicked ;
        : click? ( -- flag )  dpy click?  ;
        : click ( -- x y b n )  dpy click 2swap trans 2swap ;

\ backing store                                        11nov06py
        : resize ( x y w h -- )   w @ h @ >r >r
          2swap 2over super resize 0 0 2swap child resize
          r> r> w @ h @ d= 0=  xwin @ 0= noback @ 0= and or
          IF create-pixmap draw? dup push off child draw THEN ;
        : draw ( -- ) xwin @ noback @ 0= and redraw-all @ 0= and
          IF    0 0 w @ h @ x @ y @
                [IFDEF] win32  xrc dc @ dpy image
                [ELSE]  xpict @  IF  -1 xpict @ dpy mask
                        ELSE  xwin @ dpy image  THEN  [THEN]
          ELSE  child draw  THEN ;
        : moved? ( -- flag )  dpy moved?  ;
        : moved! ( -- )  dpy moved!  ;
        : mouse ( -- x y b ) dpy mouse >r trans r>  ;
        : keyed ( key -- )  child keyed  ;
        : handle-key?  child handle-key? ;

\ backing store                                        20oct99py
        : line ( x y x y color -- )  draw? @
          IF  !txy >r 2over trans' 2over trans' r@
              dpy line r> !t00  THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super line ;
        : text ( addr u x y color -- )  draw? @
          IF  !txy >r  2over 2over trans' r@ dpy text
              r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super text ;
        : box ( x y w h color -- )  draw? @ IF
          !txy >r 2over trans' 2over r@ dpy box r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super box ;

\ backing store                                        01mar98py
        : image ( x y w h x y win -- )  draw? @
          IF  [ 5 ] [FOR] 6 pick [NEXT] trans' 6 pick
              dpy image  THEN
          xwin @ 0= IF  drop 2drop 2drop 2drop  EXIT  THEN
          super image ;
\        : ximage ( x y w h x y win -- )  draw? @
\          IF  [ 5 ] [FOR] 6 pick [NEXT] trans' 6 pick
\              dpy ximage  THEN
\          xwin @ 0= IF  drop 2drop 2drop 2drop  EXIT  THEN
\          super ximage ;
        : mask ( x y w h x y win1 win2 -- )  draw? @
          IF  [ 5 ] [FOR] 7 pick [NEXT] trans' 7 pick 7 pick
              dpy mask  THEN
          xwin @ 0= IF  2drop 2drop 2drop 2drop  EXIT  THEN
          super mask ;

\ backing store                                        11nov06py

        : fill ( x y addr n color -- )  draw? @ IF
          !txy >r 2over trans' 2over r@ dpy fill r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super fill ;
        : stroke ( x y addr n color -- )  draw? @ IF
          !txy >r 2over trans' 2over r@ dpy stroke r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super stroke ;

        : drawer ( x y o xt -- )  draw? @
          IF   !txy 2over trans' 2over dpy drawer !t00  THEN
          xwin @ 0= IF  2drop 2drop  EXIT  THEN  super drawer ;
        : set-linewidth  dup dpy set-linewidth
          super set-linewidth ;

\ backing store                                        20oct99py
        : moved ( x y -- )  ^ dpy set-rect trans pointed self
          IF  2dup pointed xywh >r >r
              p- r> r> rot swap u< -rot u< and
              IF  backing self pointed class?
                  IF  pointed moved  ELSE  2drop THEN  EXIT THEN
              pointed leave  0 bind pointed  THEN
          child moved ;
        : leave  pointed self
          IF  pointed leave 0 bind pointed  THEN ;
        : set-cursor ( n -- )  dpy set-cursor ;
        : set-font ( font -- ) dup dpy set-font super set-font ;
        : show  child show ;
        : hide  child hide ;
        : focus    child focus     ;
        : defocus  child defocus   ;

\ backing store                                        15aug99py
        gadget :: append
        : xinc   child xinc ;
        : yinc   child yinc ;
        : txy! ( x y -- )  ty ! tx ! ;
        : get-dpy  dpy get-dpy ;
        : show-you  child show-you ;
        : hglue  ( -- g )  hglues 2@ ;
        : vglue  ( -- g )  vglues 2@ ;
        : get-win xwin @ ?dup 0= IF  dpy get-win  THEN ;
        : !resized ( -- ) xrc !font
          0 set-font  child !resized  get-glues ;
        : transback  xwin @ 0= IF trans' dpy transback  THEN ;
        : close  closing push closing @ closing on
          IF  dpy close  ELSE  child close  THEN ;
        : set-hints  dpy set-hints ;            class;

\ doublebuffer                                         28mar99py
backing class doublebuffer
how:    displays :: line ( x y x y color -- )
        displays :: text ( addr u x y color -- )
        displays :: box ( x y w h color -- )
        displays :: image ( x y w h x y win -- )
\        displays :: ximage ( x y w h x y win -- )
        displays :: mask ( x y w h x y win1 win2 -- )
        displays :: fill ( x y addr n color -- )
        displays :: stroke ( x y addr n color -- )
        displays :: drawer ( x y o xt -- )
        : sync  draw dpy sync ;
        : keyed  super keyed draw ;
        : clicked  super clicked draw ;
        : resize  super resize
          draw? @ IF  child draw draw  THEN ;   class;

\ pixmap                                               28oct06py

doublebuffer class pixmap
public: method map@
how:    : init ( depth w h dpy -- )
          screen self dpy! xrc clone bind xrc ;
        : draw ( -- ) ;
[IFDEF] x11
        : create-pixmap ( depth w h -- ) over2 xrc depth !
          2dup h ! w ! xwin @
          IF  xwin @ xrc dpy @ XFreePixmap drop xwin off  THEN
          swap dpy get-win xrc dpy @
          XCreatePixmap xwin ! ;
        : get ( -- addr w h )
          ZPixmap -1 h @ w @ 0 0 xwin @ xrc dpy @ XGetImage ;
[THEN]

\ pixmap                                               28oct06py

[IFDEF] win32
        : create-pixmap ( depth w h -- )  h ! w ! drop
          super create-pixmap ;
\ !!!FIXME!!! This doesn't work!
        : get ( -- addr w h )  pad w @ h @ ;
[THEN]

class;

\ beamer                                               24jan98py
backing class beamer            cell var resize!
public: ptr nextb               ptr firstb
        cell var enable         method clone
        early all-on            early all-off
        early resize-all        early all-wh
        early delete-me         early set-first
how:    : drops  cells sp@ + cell+ sp! ;
        : line ( x y x y color -- ) >r
          enable @ IF  2over 2over r@ super line  THEN
          nextb self IF  r> nextb goto line
          ELSE  rdrop 2drop 2drop  THEN ;
        : text ( addr u x y color -- ) >r
          enable @ IF  2over 2over r@ super text  THEN
          nextb self IF  r> nextb goto text
          ELSE  rdrop 2drop 2drop  THEN ;

\ beamer                                               20oct99py

        : box ( x y w h color -- )  >r
          enable @  IF  2over 2over r@ super box  THEN
          nextb self IF  r> nextb goto box
          ELSE  rdrop 2drop 2drop  THEN ;
        : image ( x y w h x y win -- )
          enable @ IF [ 6 ] [FOR] 6 pick [NEXT] super image THEN
          nextb self IF nextb goto image  ELSE  7 drops  THEN ;
\        : ximage ( x y w h x y win -- )
\        enable @ IF [ 6 ] [FOR] 6 pick [NEXT] super ximage THEN
\         nextb self IF nextb goto ximage  ELSE  7 drops  THEN ;
        : mask ( x y w h x y win1 win2 -- )
          enable @ IF [ 7 ] [FOR] 7 pick [NEXT] super mask  THEN
          nextb self IF nextb goto mask  ELSE   8 drops  THEN ;

\ beamer                                               11nov06py
        : fill ( x y addr n color -- ) >r
          enable @  IF  2over 2over r@ super fill  THEN
          nextb self IF  r> nextb goto fill
          ELSE  rdrop 2drop 2drop  THEN ;
        : stroke ( x y addr n color -- ) >r
          enable @  IF  2over 2over r@ super stroke  THEN
          nextb self IF  r> nextb goto stroke
          ELSE  rdrop 2drop 2drop  THEN ;
        : drawer ( x y o xt -- )
          enable @  IF 2over 2over super drawer THEN
          nextb self IF nextb goto drawer
          ELSE  2drop 2drop  THEN ;
        : set-linewidth ( n -- )
          enable @  IF dup super set-linewidth THEN nextb self
          IF nextb goto set-linewidth ELSE drop THEN ;

\ beamer                                               28mar99py

        : init ( first next -- )  noback on
          super init  bind nextb  bind firstb
          firstb self 0= IF  ^ bind firstb  THEN
          enable on ;
        : clone ( -- beamer )
          firstb self nextb self ( rot ) new bind nextb
          child self IF  child self nextb assign  THEN
          nextb self ;
        : all-on  enable on
          nextb self IF  nextb goto all-on THEN ;
        : all-off  enable off
          nextb self IF  nextb goto all-off THEN ;
        : draw  firstb all-off  enable on  super draw
          firstb all-on ;

\ beamer                                               28mar99py

        : first?  ^ firstb self = ;
        : hglue  first? IF  super hglue
          ELSE  firstb w @ 0  THEN ;
        : vglue  first? IF  super vglue
          ELSE  firstb h @ 0  THEN ;
        : resize-all ( -- )
          xywh 2drop  firstb xywh 2swap 2drop  super resize
          parent resized
          nextb self  IF  nextb goto resize-all  THEN ;
        : resize ( x y w h -- )
          first? IF  super resize resize! on
          ELSE  gadget :: resize THEN ;
        : draw  super draw  resize! @ 0= ?EXIT  resize! off
          nextb self IF  nextb resize-all  THEN ;

\ beamer                                               28mar99py

        : delete-me ( beam -- )  dup nextb self =
          IF    nextb nextb self bind nextb
          ELSE  nextb self  IF  nextb delete-me  THEN
          THEN ;
        : set-first ( beam -- )  dup bind firstb
          nextb self IF  nextb goto set-first  THEN ;
        : dispose  first? nextb self and
          IF  nextb self nextb set-first drop  THEN
          self firstb delete-me drop
          first? 0= nextb self or IF  0 bind child  THEN
          super dispose ;

\ beamer                                               17dec00py

        : dpy! ( dpy -- )  bind dpy
          xrc self IF  xrc dispose  THEN
          dpy xrc clone bind xrc  create-pixmap
[IFDEF] x11     get-win xrc get-gc  [THEN]
          0 clip-rect  draw? on
          first?  IF  self child dpy!  THEN
          !resized  child xywh  resize ;
        : assign ( widget -- )  set-child
          first?  IF  self child dpy!  THEN
          rest-child ;
        : close  dpy close ;
class;
: :beamer  0 0 ;

\ actor                                                23nov97py
debugging class actor
public: object ptr called       gadget ptr caller
        method set              ( -- )
        method reset            ( -- )
        method toggle           ( -- )
        method fetch            ( -- x1 .. xn )
        method store            ( x1 .. xn -- )
        method click            ( x y b n -- ... )
        method key              ( key sh -- )
        method enter            ( -- )
        method leave            ( -- )
        method assign           ( x1 .. xn -- )
        method set-called       ( o -- )
\ all methods send appropriate messages to called/caller
\ Use: <actor> (set|reset|toggle|fetch|store|click|enter|leave)

\ actor                                                23nov97py

how:    : init ( o -- ) bind called ;
        : set    fetch 0= IF  true  store  THEN ;
        : reset  fetch    IF  false store  THEN ;
        : toggle fetch 0= store ;
        : click  dup 0= IF  2drop 2drop  EXIT  THEN
          caller >released  IF  toggle  THEN ;
        : key ( key sh -- )  drop  dup bl = swap #cr = or
          IF  caller xywh 2drop  1 2 click  THEN ;
        : enter ( cr ." enter" ) ;
        : leave ( cr ." leave" ) ;
        : assign ;
        : set-called  bind called ;
class;

\ actor                                                23aug97py

actor class toggle
public: cell var do-set         cell var do-reset
        cell var set?
how:    : init ( o state xtset xtreset -- )
          do-reset !  do-set !  assign  super init ;
        : assign ( flag -- )  set? ! ;
        : fetch ( -- flag )  set? @ ;
        : store ( flag -- )  set? !
          set? @ IF  do-set  ELSE  do-reset  THEN  @
          called send ;
        : click  dup 0= IF  2drop 2drop  EXIT  THEN
          toggle  caller >released  drop ;
class;

\ actor                                                05mar07py

actor class toggle-var
public: cell var addr           cell var xt
how:    : init ( o var xt -- ) xt ! assign super init ;
         : fetch ( -- n )  addr @ @ ;
        : store ( n -- )  addr @ ! xt @ called send ;
        : assign ( addr -- )  addr ! ;
class;
toggle-var class toggle-num
public: cell var num
how:    : assign ( o num var -- )  super assign num ! ;
        : !if ( n num addr -- )  rot IF  !  ELSE  nip on  THEN ;
        : fetch ( -- flag ) num @ addr @ @ = ;
        : store ( n -- )  num @ addr @ !if xt @ called send ;
class;

\ toggle bit                                           05mar07py

toggle-var class toggle-bit
public: cell var bit
how:    : fetch ( -- n )  addr @ bit @ bit@ ;
        : store ( n -- )  >r addr @ bit @
          r> IF  +bit  ELSE  -bit  THEN
          xt @ called send ;
        : assign ( addr bit -- ) bit ! addr ! ;
class;

\ actor                                                25sep99py

actor class toggle-state
public: cell var do-store       cell var do-fetch
how:    : init ( o xtstore xtfetch -- )
          do-fetch ! do-store ! super init ;
        : fetch ( -- x1 .. xn ) do-fetch @ called send ;
        : store ( x1 .. xn -- ) do-store @ called send ;
class;

actor class simple
public: cell var do-it
how:    : init ( o xt -- ) do-it ! super init ;
        : fetch 0 ;
        : store do-it @ called send drop ;
class;

\ actor                                                25sep99py

: noop-act  0 ['] noop simple new ;

simple class click
how:    : click  store ;
        : fetch ;
        : store  do-it @ called send ;
class;

simple class data-act
public: cell var data
how:    : init ( o data xt -- ) swap data ! super init ;
        : store data @ super store ;
class;

\ actor                                                31aug97py

toggle-state class scale-act
public: cell var max
how:    : init ( o do-store do-fetch max -- )
          assign super init ;
        : assign ( max -- )  max ! ;
        : fetch  max @ do-fetch @ called send ;
class;

scale-act class slider-act
public: cell var step
how:    \ init ( o do-store do-fetch max step -- )
        : assign  step ! super assign ;
        : fetch  max @ step @ do-fetch @ called send ;
class;

\ actor                                                12apr98py

actor class scale-var
public: cell var max            cell var pos
how:    : init ( o pos max -- ) assign super init ;
        : assign  ( pos max -- )  max ! pos ! ;
        : fetch  max @ pos @ ;
        : store  pos !       ;
class;
scale-var class slider-var
public: cell var step
how:    : assign ( o pos max step -- ) step ! super assign ;
        : fetch  max @ step @ pos @ ;
class;

\ actor                                                24sep99py

scale-var class scale-do
public: cell var action
how:    : init ( o n max xt -- ) action ! super init ;
        : store  super store pos @ action @ called send ;
class;


slider-var class slider-do
public: cell var action
how:    : init ( o n max step xt -- ) action ! super init ;
        : store  super store pos @ action @ called send ;
class;

\ actor simplification                                 05mar07py

' :[ alias S[                                immediate restrict
' :[ alias DT[                               immediate restrict
' :[ alias T[                                immediate restrict
' :[ alias TS[                               immediate restrict
' :[ alias CK[                               immediate restrict
: ]S  postpone ]: simple postpone new ;      immediate restrict
: ]DT postpone ]: data-act postpone new ;    immediate restrict
: ]T  postpone ]: toggle postpone new ;      immediate restrict
: ]CK postpone ]: click  postpone new ;      immediate restrict
: ][  postpone ]: postpone :[ ;              immediate restrict
: ]TS  postpone ]: toggle-state postpone new ;
                                             immediate restrict
: ]N ;                                       immediate
: ]TERM ;                                    immediate

\ other simplifications                                05mar07py
: C[ ;                                       immediate restrict
' :[ alias SC[                               immediate restrict
' :[ alias SL[                               immediate restrict
: ]SC  postpone ]: scale-do postpone new ;   immediate restrict
: ]SL  postpone ]: slider-do postpone new ;  immediate restrict
: TV[  ;                                     immediate restrict
: TB[  ;                                     immediate restrict
: TN[  ;                                     immediate restrict
' :[ alias ]T[                               immediate restrict
: ]TV  postpone ]: toggle-var postpone new ; immediate restrict
: ]TB  postpone ]: toggle-bit postpone new ; immediate restrict
: ]TN  postpone ]: toggle-num postpone new ; immediate restrict
' noop Alias CP[ immediate      ' noop Alias ]CP immediate
: DF[ postpone dup postpone >o ;             immediate restrict
: ]DF postpone o> ;                          immediate restrict

\ widget                                               13may99py

gadget class widget             early >callback
public: displays ptr dpy        actor ptr callback
        early dopress           early whilepress
        early shadow            early xS
        early drawshadow        early textsize
        early xN                early xM
        early hM
        method +push            method -push

\ widget                                               27jun02py

how:    : >callback ( cb -- )
          callback self IF  callback dispose  THEN
          bind callback  self callback bind caller
          callback called self ?EXIT
          self callback set-called ;
        : dispose callback self  IF  callback dispose  THEN
          super dispose ;
        : dpy!  bind dpy !resized ;
        : textsize ( addr u n -- w h )
          dpy xrc font@ font with  size  endwith ;
        : close  dpy close ;

\ widget                                               28aug99py

        : DOPRESS  ( dx dy -- dx dy x y )
          BEGIN  BEGIN  dpy click? 0=  WHILE  dpy moved?
                        IF    2dup dpy mouse drop r@ execute
                              dpy sync
                        THEN  dpy invoke dpy do-idle
                 REPEAT dpy click nip 1 and  WHILE
          2drop  REPEAT  dpy moved! ;

\ widget                                               01mar98py

        : >timeout ( time -- )  dpy sync
          BEGIN  timeout? 0=  WHILE  dpy invoke 0=  UNTIL  THEN
          till ;
        : WHILEPRESS ( x y b n -- ) \ 2over moved
          nip nip nip 1 and
          IF    BEGIN 0 after
                    BEGIN  r@ swap >r execute
                           r> /step @ ms>time +
                           dup >timeout  dpy click?  UNTIL  drop
                    dpy click nip nip nip 1 and 0=  UNTIL
                rdrop dpy moved!
          ELSE  &50 after >timeout  THEN ;

\ draw shadow                                          25mar99py

        : draw-edge ( x y n col -- )  >r 1-
          dup 1 <  IF  2drop drop rdrop  EXIT  THEN
          dup 1 =  IF  dup r> dpy box    EXIT  THEN  >r
          <poly r@ 0 poly, 0 r@ - r> poly, poly> r> dpy fill ;
        : (drawshadow ( lightcol shadowcol n x y w h -- )
          { n x y w h | n 0< IF  swap  THEN  n abs  { lc sc n |
             x         y         w n - n      lc dpy box
             x         y n +     n     h n -  lc dpy box
             x         y h + n - w     n      sc dpy box
             x w + n - y         n     h n -  sc dpy box
             x w + n - y         n            lc draw-edge
             x         y h + n - n            lc draw-edge } } ;

\ draw shadow                                          27jan07py
         : 2+?  dup $10 < IF  2+  THEN ;
         : drawshadow ( lc sc n x y w h -- )
           4 pick abs twoborders u<= IF  (drawshadow
           ELSE { lc sc n x y w h |
                n IF  lc    sc n         x y w h (drawshadow
                      lc 2+? sc 2+? n 0< 2* 1+
                                         x y w h (drawshadow
                THEN }
           THEN ;
[IFDEF] x11 : focus  dpy self 0= ?EXIT
          dpy xrc self 0= ?EXIT dpy xrc ic @ 0= ?EXIT
          xywh nip + dpy trans' swap  spot w!+ w!
          0 spot XNSpotLocation 0 XVaCreateNestedList_1 >r
          0 r@ XNPreeditAttributes dpy xrc ic @
            XSetICValues_1 drop r> XFree drop ;  [THEN]

\ widget                                               19oct99py
        : shadow ( -- lc sc )
          shadowcol @ @ dup 8 >> swap $FF and ;
        : show-you ( -- ) xywh p2/ p+ dpy show-me ;
        : >released ( click -- )  WHILEPRESS ;
        : clicked  >released ;
        : +push ;
        : -push ;
        : xN ( -- n )  dpy xrc xN @ ;
        : xM ( -- n )  dpy xrc xM @ ;
        : hM ( -- n )  dpy xrc hM @ ;
        : xS ( -- n )  dpy xS ;
        : moved ( x y -- )  2drop
          mouse_cursor dpy set-cursor ^ dpy set-rect ;
        : leave ( -- ) ;
class;

\ repeated press and moved press                       23aug97py

simple class rep
how:    : >press  widget WHILEPRESS  widget callback toggle ;
        : click  caller with widget +push >press
                             widget -push endwith ;
class;

simple class drag
how:    : click  toggle ;
class;

' :[ alias R[  immediate restrict
' :[ alias M[  immediate restrict
: ]R  postpone ]: rep    postpone new ;      immediate restrict
: ]M  postpone ]: drag   postpone new ;      immediate restrict

\ Icon                                                 21mar04py
include pixmap.fs
widget class icon-pixmap
public: cell var shape          cell var image
        method draw-at
how:    : init ( file len -- ) super init  assign ;
        : dispose-image ( -- )
[IFDEF] x11
          shape @ ?dup IF screen xrc dpy @ XFreePixmap drop THEN
          image @ ?dup IF screen xrc dpy @ XFreePixmap drop THEN
 [THEN]   shape off image off ;
        : assign ( file len -- )  dispose-image
          read-icon  h ! w !
[IFDEF] win32  2 0 DO  swap  screen xrc dc @
          CreateCompatibleDC tuck SelectObject drop  LOOP
[THEN]    shape ! image ! ;

\ Icon                                                 27jun02py
        : dispose ( -- )  dispose-image super dispose ;
        : draw-at ( x y -- x y w h x y w1 w2 ) >r >r
          0 0 xywh 2swap 2drop r> r> shape @ image @ ;
        : draw  xywh 2drop draw-at dpy mask ;
class;
[IFDEF] x11
xresource implements
        : set-tile ( x y pixmap -- )  \ -1 cur-color !
          icon-pixmap with image @ endwith
                    [ xgc XGCValues tile         ] ALiteral !
          swap      [ xgc XGCValues ts_x_origin  ] ALiteral 2!
          FillTiled [ xgc XGCValues fill_style   ] ALiteral !
          xgc [ GCTile GCTileStipXOrigin or
                GCTileStipYOrigin or GCFillStyle or ] Literal
          gc @ dpy @ XChangeGC drop ;   class;  [THEN]

\ Icon                                                 26oct07py
widget class icon
public: icon-pixmap ptr picture
how:    : init ( ficon -- )  super init assign ;
        : !resized  picture xywh  h ! w ! 2drop ;
        : assign ( ficon -- )  bind picture !resized ;
        : draw   xywh defocuscol @ @ dpy box
          xywh 2drop picture draw-at dpy mask ;
class;
[IFDEF] x11
icon-pixmap class memory-pixmap
how:    : assign ( data w h -- )  dispose-image
          2dup * pixels -rot
          create-pixmap h ! w ! image ! ;
class;
[THEN]

\ glue                                                 10aug05py

widget class glue
public: cell var w+             cell var h+
        cell var wmin           cell var hmin
how:    : init ( w w+ h h+ -- )  super init
          h+ ! dup h ! hmin ! w+ ! dup w ! wmin ! ;
        : hglue  wmin @ w+ @ ;
        : vglue  hmin @ h+ @ ;
        : draw ( -- )  xywh defocuscol @ @ dpy box ;
        widget :: handle-key? ( -- flag )
class;

\ nil                                                  27jun02py

widget class (nil
how:    : delete ( addr addr' -- )  2drop ;
        : hglue  0 0 ;
        : vglue  0 0 ;
        : init ;
        : dispose ;
        : show-you ( -- ) ;
class;

(nil : nil
nil self nil bind widgets
nil self to 'nil

\ (nilscreen                                           26jul98py

displays class (nilscreen
how:    (nil :: delete
        (nil :: hglue
        (nil :: vglue
        (nil :: init
        (nil :: dispose
        : size-event ( -- ) ;
        : get-event ( mask -- ) drop ;
        : schedule ( xt o time -- )  dpy schedule ;
        : invoke ( -- flag )  dpy invoke ;
        : cleanup ( o -- )  dpy cleanup ;

\ (nilscreen                                           03aug98py

        : schedule-event ( -- ) ;
        : handle-event ( -- )
\          base push hex cr ." Event "
\          event XAnyEvent type ?
\          event XAnyEvent window ?
\          event sizeof XEvent dump
        ;
class;
(nilscreen : nilscreen
nilscreen self nilscreen bind nextwin
nilscreen self to 'nilscreen

\ fills                                                28mar99py

: *fil    ( n -- glue )   $C << ;
: *fill   ( n -- glue )  $14 << ;
: *filll  ( n -- glue )  $1C << ;
: *hpix   ( n -- n )            ; immediate
: *hfil   ( n -- glue )   $C << ;
: *hfill  ( n -- glue )  $14 << ;
: *hfilll ( n -- glue )  $1C << ;
: *vpix   ( n -- n )            ; immediate
: *vfil   ( n -- glue )   $C << ;
: *vfill  ( n -- glue )  $14 << ;
: *vfilll ( n -- glue )  $1C << ;
: +fil   ( -- ) 0 1 *fil   2dup glue new ;
: +fill  ( -- ) 0 1 *fill  2dup glue new ;
: +filll ( -- ) 0 1 *filll 2dup glue new ;

\ hglue vglue                                          10aug05py
: chcol ( addr col-addr -- )
  @ @ over @ $FFFF0000 and or swap ! ;
: tocol ( addr col-addr -- )
  @ @ over @ $FF000000 and or swap ! ;

glue class *hglue
how:    : init ( w w+ -- )  1 1 *hfil       super init ;
class;

glue class *vglue
how:    : init ( h h+ -- )  1 1 *hfil 2swap super init ;
class;

\ hrule vrule                                          05jan07py
glue class rule                 cell var color
how:    : init ( w w+ h h+ -- ) \ dup dup drop drop
          super init defocuscol @ @ assign ;
        : assign ( color -- )  color ! ;
        : draw ( -- )  xywh color @ dpy box
          shadow swap color @ $18 >> xywh drawshadow  ;
        : defocus  color defocuscol tocol draw ;
        : focus    color focuscol   tocol draw ;
class;
rule class hrule
how:    : init ( w w+ -- )  2 0 ( rot) super init ;
class;
rule class vrule
how: : init ( h h+ -- ) ( >r) 2 0 2swap ( r>)super init ;
class;

\ lines                                                28mar99py
: linepar  $1000000 0 colors @ or 0 1 *fill ;
: vline linepar vrule new widget with  assign ^  endwith ;
: hline linepar hrule new widget with  assign ^  endwith ;
rule class Mskip
how:    : init  ( -- )
          0 0 0 1 *fill super init ;
        : hglue ( -- min glue )  xM w+ @ ;
class;
rule class Mfill
how:    : init ( -- )
          0 1 *fill 0 1 *fill super init ;
        : hglue ( -- min glue )  xM w+ @ ;
class;
: 2skip   Mskip new ;
: 2fill   Mfill new ;

\ canvas                                               16jun02py

rule class canvas
public: defer drawer            defer pixel
        cell var angle          cell var coord
        cell var color          cell var shown
        cell var dx             cell var dy
        2 cells var sw          2 cells var sh
        cell var xp             cell var yp
        1 var textpx            1 var textpy
        font ptr fnt            widget ptr outer
        2 cells var startp

\ canvas                                               08aug99py

        method fd               method rt
        early bk                early lt
        method path             method to
        method stroke           method fill
        method clear            method steps
        method up               method down
        method home!            method linewidth
        method drawcolor        method fillcolor
        method backcolor
        method text             method font
        method textpos          method icon
        method dto              method dhome!

\ canvas                                               22jun02py
how:    : init ( xt ac w w+ h h+ -- )  super init bind outer
         >callback IS drawer down &360 coord ! $0D030C color ! ;
       : pixel, xp 2@ p+ 2dup xp 2! wextend swap wextend pixel ;
        : dx+ ( d -- n )  dx @ extend d+ swap dup dx ! 0< - ;
        : dy+ ( d -- n )  dy @ extend d+ swap dup dy ! 0< - ;
        : draw  clear  ^ drawer ;
        : fd ( n -- ) >r angle @ sincos
          r@ negate m* sh 2@ d* $10 d>> dy+
          swap   r> m* sw 2@ d* $10 d>> dx+ pixel, ;
        : dto ( x y -- )  dnegate sh 2@ d* dy+
                             -rot sw 2@ d* dx+ pixel, ;
        : to ( x y -- )  swap extend rot extend dto ;
        : rt ( n -- ) 2pi 2* coord @ */ 1+ 2/ angle +! ;
        : bk ( n -- ) negate fd ;
        : lt ( n -- ) negate rt ;

\ canvas                                               25apr07py
        : clear ( -- )  xywh color 2+ c@ dpy box ;
        : stroke ( -- ) startp 2@ poly> color c@ dpy stroke ;
        : fill ( -- ) startp 2@ poly> color 1+ c@ dpy fill ;
        : path ( -- )
          xp 2@ wextend swap wextend <poly startp 2! ;
        : dhome! ( dx dy -- ) dx off  dy off  angle off
          sh 2@ d* dy+ -rot sw 2@ d* dx+ y @ x @ p+ xp 2! ;
        : home! ( p -- )  0 tuck dhome! ;
        : show  shown on ;
        : hide  shown off ;
        : steps ( w h -- )
          $00000000 h @ 1- rot ud/mod sh 2! drop
          $00000000 w @ 1- rot ud/mod sw 2! drop ;
        : up    ['] 2drop IS pixel ;
        : down  ['] poly# IS pixel ;

\ canvas                                               11nov06py
        : linewidth ( n -- ) dpy set-linewidth ;
        : drawcolor ( n -- ) color c! ;
        : fillcolor ( n -- ) color 1+ c! ;
        : backcolor ( n -- ) color 2+ c! ;
        : >textxy ( w h -- x y )  textpy c@ * 2/ negate
          swap textpx c@ * 2/ negate  xp 2@ p+ swap ;
        : font ( fnt -- )  bind fnt ;
        : dpy! ( dpy -- )  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font  THEN ;

\ canvas                                               21mar04py
        : icon ( icon-pixmap -- )  >r
          r@ icon-pixmap with xywh endwith 2swap 2drop >textxy
          r> icon-pixmap with draw-at endwith dpy mask ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;
        : text ( addr u -- )
          2dup fnt size >textxy color c@ .text ;
        : textpos ( x y -- )  textpy c! textpx c! ;
        : clicked ( x y b n -- ) callback click ;
        : keyed ( key sh -- )  callback key ;
        : moved ( x y -- )  super moved  callback enter ;
        : leave ( -- )  callback leave ;
        : >released  WHILEPRESS ;
class;

\ combined widgets attributes                          08apr00py

$01 Constant :hfix
$02 Constant :vfix
$04 Constant :flip
$08 Constant :resized
$10 Constant :notshadow
$20 Constant :norshadow
$40 Constant :nobshadow
$80 Constant :nolshadow

\ combined widgets                                     27may00py

widget class combined
public: gadget ptr childs       cell var n
        2 cells var hglues      2 cells var vglues
        1 var hskip             1 var vskip
        1 var borderw           1 var attribs
        gadget ptr active       cell var tab-steps
        method compose          method (clicked
        early hskip@            early vskip@
        method add              method remove
        method flip
        method +flip            method -flip
        method tab!             method tab@
        method tabs
        early >box              early ALLCHILDS

\ combined widgets                                     21mar00py
how:    : >box  'nil bind childs  'nil bind active
          0 ?DO  childs self >r  bind childs
                 r> childs bind widgets
                 self childs bind parent
          LOOP  first-active ;
        : init ( widget_1 .. widget_n n -- )
          focuscol @ color !
          super init  dup n ! >box ;
        : ALLCHILDS ( .. -- ..' )  childs self
          BEGIN  dup 'nil <>  WHILE
                 r@ swap >o execute widgets self o>
          REPEAT  drop rdrop ;
        : (dpy!  ALLCHILDS  dup dpy! ;
        : dpy! ( dpy -- )  dup bind dpy (dpy! drop
          0. hglues 2!  0. vglues 2! ;

\ combined widgets                                     28mar99py

        : remove  ( o -- )
          link childs childs delete -1 n +! resized ;
        : add ( o before -- )
          over dpy self swap >o dpy! o>
          dup childs self =
          IF    swap bind childs  childs bind widgets
                self childs bind parent
          ELSE  childs append  THEN  1 n +! ( resized ) ;

        : ?nodraw ( -- flag )  attribs c@ :flip and ;

\ combined widgets                                     29aug99py

        : (font!  ALLCHILDS  dup font! ;
        : font! ( font -- )  (font! drop
          dpy self IF  !resized  THEN ;

        : ?2b ( c n -- )  twoborders < IF  2 +  THEN ;

        : draw ( -- )   borderw c@  0= ?EXIT
          shadow borderw cx@ xS * 2/ xywh    attribs c@
          $F0 and 0= IF  widget :: drawshadow  EXIT  THEN

\ combined widgets                                     25mar99py

          { n x y w h | n 0< IF  swap  THEN  { lc sc | n abs 0
          ?DO  attribs c@ :notshadow and 0=
               IF  x I + y I + w I 2* - 1- 1
                                          lc I ?2b dpy box  THEN
               attribs c@ :nolshadow and 0=
               IF  x I + y I + 1+ 1 h I 2* - 2-
                                          lc I ?2b dpy box  THEN
               attribs c@ :norshadow and 0=
               IF  x w + I - 1- y I + 1 h i 2* -
                                          sc I ?2b dpy box  THEN
               attribs c@ :nobshadow and 0=
               IF  x I + y h + I - 1- w I - 1
                                          sc I ?2b dpy box  THEN
          LOOP } } ;

\ combined widgets                                     21mar00py

        2Variable xy
        Variable +skip          Variable /skip
        Variable sclip          Variable eclip
        : x+ ( dx -- )  xy cell+ +! ;
        : y+ ( dy -- )  xy +! ;
        : maxglue ( g1 g2 -- g )  rot min -rot max swap ;
        : gsize   ( g1 g2 -- g n )  swap
          2over 2over >r 2* rot ?dup  IF  */  ELSE  min  THEN
          1+ 2/ >r drop negate r@ negate p+ r> r> + ;
        : >range  vglues 2@ range swap hglues 2@ range swap ;
        : resize  ( x y w h -- ) hglue@ 2drop vglue@ 2drop
          >range  h ! w ! repos  compose  2drop 2drop ;
        : close  active close ;

\ combined widgets: event handling                     09mar07py
        : hskip@  ( -- n )  hskip c@ dpy xrc xn @ * ;
        : vskip@  ( -- n )  vskip c@ dpy xrc xn @ * ;
        : -border ( x y w h -- x' y' w' h' )
          borderw cx@ abs xS * 2/ xywh- ;
        : dispose-childs  ALLCHILDS  dispose ;
        : tab-step-off  tab-steps HandleOff ;
        : dispose dispose-childs tab-step-off super dispose ;
        : focus    ?nodraw ?EXIT  ALLCHILDS  focus   ;
        : defocus  ?nodraw ?EXIT  ALLCHILDS  defocus ;
        : show     ?nodraw ?EXIT  ALLCHILDS  show ;
        : hide     ALLCHILDS  hide ;
        : keyed    ( key sh -- )  active keyed ;
        : handle-key?  active handle-key? ;
        : !resized  0. hglues 2!  0. vglues 2!  tab-step-off
          ALLCHILDS !resized ;

\ combined widgets: event handling                     19oct99py

        : (moved ( x y -- x y flag )
          2dup inside? dup 0= ?EXIT  0 and
          ALLCHILDS  drop  2dup inside?
          IF  rdrop 2dup moved -1 o> rdrop  ELSE  0  THEN ;
        : moved ( x y -- )  (moved
          0= IF  mouse_cursor dpy set-cursor  THEN  2drop ;

        : (clicked  ( x y b n -- x y b n / -1 )  2over inside?
          0= IF  2drop 2drop -1  EXIT  THEN
          ALLCHILDS  2over inside?
          IF  rdrop  clicked -1 o> rdrop  THEN ;
        : clicked   ( x y b n -- )  (clicked dup 0<
          IF  drop  ELSE  >released  THEN ;

\ combined widgets: active point                       21mar00py

        : next-active ( -- flag )
          active self 'nil =
          IF    childs self
          ELSE  active next-active  dup ?EXIT  drop
                active widgets self
          THEN  >o
          BEGIN  ^ 'nil <>  WHILE
                 handle-key?  0=  WHILE
                 widgets self op!  REPEAT  THEN  ^ o>
          dup  bind active  'nil <> dup 0= ?EXIT
          active first-active ;

        : first-active  'nil bind active  next-active drop ;

\ combined widgets: active point                       14sep97py
        : prev-active ( -- flag )
          active prev-active  dup  ?EXIT  drop
          active self dup >r  'nil bind active
          BEGIN  drop  active self  next-active  WHILE
                 active self  r@ =  UNTIL  THEN  rdrop
          dup bind active
          BEGIN  active next-active  0=  UNTIL
          active prev-active drop  'nil <> ;
        : inside? ( x y -- f )  borderw cx@ abs xS * 2/ >r
          r@ - y @ - h @ r@ 2* - 0max u< swap
          r@ - x @ - w @ r> 2* - 0max u< and ;
        : >inc ( glue o inc -- glue' )
          dup 1 =   IF  2drop  EXIT  THEN
          2 pick 0= IF  2drop  EXIT  THEN
          { o inc | o - dup negate inc mod + 0max o + } ;

\ combined widgets                                     08apr00py
        : ?vfix  attribs c@ :vfix and 0= and ;
        : ?hfix  attribs c@ :hfix and 0= and ;
        : hglue@  hglues 2@ 2dup d0= IF 2drop hglue THEN ?hfix ;
        : vglue@  vglues 2@ 2dup d0= IF 2drop vglue THEN ?vfix ;
        : resized ( -- )  attribs c@ :resized and 0=
          IF    hglues 2@ hglue d=  vglues 2@ vglue d= and
                IF  xywh 2dup 2>r resize xywh 2swap 2drop
                    >range 2r> d=  IF  draw  EXIT  THEN  THEN
          ELSE  hglue 2drop vglue 2drop  THEN
          parent resized ;
        : flip  attribs c@ :flip and IF +flip ELSE -flip THEN ;
        : -flip  attribs c@ :flip or         attribs c!
          hide parent !resized parent resized ;
        : +flip  attribs c@ :flip invert and attribs c!
          show parent !resized parent resized draw ;

\ combined widgets                                     21mar00py

        0 Value bc
        : tabs ( -- n )  tab-steps @ 0= IF  0  EXIT  THEN
          tab-steps $@len 2/ cell/ ;
        : tab-size! ( n -- )
          tab-steps @ 0= IF  S" " tab-steps $!  THEN
          1+ 2* cells tab-steps $@len
          dup >r max tab-steps $!len
          tab-steps $@ r> /string erase ;
        : tab@ ( n -- glue )
          dup 0< IF  drop 0 0  EXIT  THEN
          dup tab-size! 2* cells tab-steps $@ drop + 2@ ;
        : tab! ( glue n -- ) dup >r tab@ rot max -rot max swap
          r> 2* cells tab-steps $@ drop + 2! ;
class;

\ public visible ALLCHILDS                             02jul00py

:  ALLCHILDS ( .. -- ..' ) r> swap >o >r combined childs self
   BEGIN  dup 'nil <>  WHILE
          r@ swap >o execute combined widgets self o>
   REPEAT  drop rdrop o> ;

\ hbox                                                 19dec99py

combined class hbox
how:    : >hglue ( -- min glue ) 0 0  ALLCHILDS hglue@ p+ ;
        : >vglue ( -- min glue ) 0 mi n @ 0<> and
          ALLCHILDS vglue@ maxglue ;
        : >vmax  ( g -- gmin )  ALLCHILDS  vglue@ + min ;
        : hskips+ ( n -- n' )
          n @ vskip@ IF  1+  ELSE  1-  THEN  0max  hskip@ * +
          borderw cx@ abs xS * + ;
        : hglue  attribs c@ :flip and
          IF    0 0
          ELSE  >hglue  swap  hskips+  swap
          THEN  2dup hglues 2! ?hfix ;
        : hglue@  attribs c@ :flip and
          IF  0 0  ELSE  super hglue@  THEN ;

\ hbox                                                 19dec99py

        : vskips+ ( n -- n' )
          vskip@ borderw cx@ abs xS * 2/ + 2* + ;
        : vglue ( -- glue )
          >vglue over + >vmax  swap
          1- yinc 2dup >r >r swap y @ - tuck
          >r >r - r> tuck / 1+ * r> + tuck -
          swap vskips+
          tuck + swap  r> r>  >inc tuck -
          dup 0< IF  + 0  THEN
          2dup vglues 2! ?vfix ;

\ hbox                                                 21oct99py
        : draw ( -- ) ?nodraw  ?EXIT  super draw
          sclip push  eclip push  +skip push  /skip push
          & bc push  dpy clipx  over + eclip ! sclip !
          hskip@ +skip !  vskip@ /skip !  defocuscol @ @ to bc
          xywh -border 2over 2over drop /skip @  bc dpy box
          /skip @ - rot + swap /skip @           bc dpy box
          /skip @
          IF xywh -border nip +skip @ swap bc dpy box THEN
          ALLCHILDS xywh drop +skip @ + nip over +
                    eclip @ umin sclip @ umax swap
                    eclip @ umin sclip @ umax u<= ?EXIT
depth 1+ >r ['] draw catch depth r> <> or IF .class THEN
                    /skip @ 0= IF widgets self 'nil = ?EXIT THEN
                    +skip @ 0= ?EXIT  x @ w @ + y @ +skip @ h @
                    bc dpy box ;

\ hbox                                                 19dec99py

        : compose  ( -- g1 g2 )  xy dup push cell+ push
          +skip push  hskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + vskip@ 0= IF  hskip@ -  THEN
          y @ r@ + vskip@ + xy 2!
          hglues 2@ swap negate w @ +
          vglues 2@ swap h @ max vskip@ 2* - r> 2* -
          ALLCHILDS 2swap +skip @ 0 gsize x+  hglue@ gsize >r
                    2swap r@ over xy 2@ 2swap resize
                    r> x+ ;

\ hbox                                                 21oct99py

        : xinc  0 1  attribs c@ :flip and ?EXIT
          swap hskips+ swap
          ALLCHILDS  xinc { mi o i |  i 1 <>
          IF    o +  mi i max
          ELSE  hglue@ drop + mi  THEN } ;
        : yinc  0 1  attribs c@ :flip and ?EXIT
          swap vskips+ swap
          ALLCHILDS  yinc { mi o i |  i 1 <>
          IF  o +  mi i max  ELSE  mi  THEN } ;
class;

\ vbox                                                 19dec99py
combined class vbox
how:    : >hglue ( -- min glue ) 0 mi n @ 0<> and
          ALLCHILDS hglue@ maxglue ;
        : >vglue ( -- min glue ) 0 0  ALLCHILDS vglue@ p+ ;
        : >hmax  ( g -- gmin )  ALLCHILDS  hglue@ + min ;
        : vskips+ ( n -- n' )
          n @ hskip@ IF  1+  ELSE  1-  THEN  0max  vskip@ * +
          borderw cx@ abs xS * + ;
        : vglue  attribs c@ :flip and
          IF    0 0
          ELSE  >vglue  swap  vskips+ swap
          THEN  2dup vglues 2! ?vfix ;
        : vglue@  attribs c@ :flip and
          IF  0 0  ELSE  super vglue@  THEN ;

\ vbox                                                 19dec99py

       : >hglue' ( -- min glue ) 0 mi n @ 0<> and
          ALLCHILDS hglue maxglue ;
        : hskips+ ( n -- n' )
          hskip@ borderw cx@ abs xS * 2/ + 2* + ;
        : hglue ( -- glue )  tab-step-off  0.
          BEGIN   2drop tabs 1- tab@ >r >r  >hglue
                  r> r> tabs 1- tab@ d= UNTIL
          over + >hmax  swap
          1- xinc 2dup >r >r swap x @ - tuck
          >r >r - r> tuck / 1+ * r> + tuck -
          swap hskips+
          tuck + swap  r> r>  >inc tuck -
          dup 0< IF  + 0  THEN
          2dup hglues 2! ?hfix ;

\ vbox                                                 21oct99py
        : draw ( -- ) ?nodraw  ?EXIT  super draw
          sclip push  eclip push  +skip push  /skip push
          & bc push  dpy clipy  over + eclip ! sclip !
          vskip@ +skip !  hskip@ /skip !  defocuscol @ @ to bc
          xywh -border 2over 2over nip /skip @ swap bc dpy box
          >r /skip @ - rot + swap /skip @ r>        bc dpy box
          /skip @
          IF xywh -border drop +skip @ bc dpy box  THEN
          ALLCHILDS xywh xS + +skip @ + nip rot drop over +
                    eclip @ umin sclip @ umax swap
                    eclip @ umin sclip @ umax u<= ?EXIT
depth 1+ >r ['] draw catch depth r> <> or IF .class THEN
                    /skip @ 0= IF widgets self 'nil = ?EXIT THEN
                    +skip @ 0= ?EXIT  x @ y @ h @ + w @ +skip @
                    bc dpy box ;

\ vbox                                                 19dec99py

        : compose  ( -- g1 g2 )  xy dup push cell+ push
          +skip push  vskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + hskip@ +
          y @ r@ + hskip@ 0= IF  vskip@ -  THEN  xy 2!
          hglues 2@ swap w @ max hskip@ 2* - r> 2* -
          vglues 2@ swap negate h @ +
          ALLCHILDS  +skip @ 0 gsize y+ vglue@ gsize >r
                     2swap dup r@ xy 2@ 2swap resize 2swap
                     r> y+ ;

\ vbox                                                 14sep97py

        : yinc  0 1  attribs c@ :flip and ?EXIT
          swap vskips+ swap
          ALLCHILDS  yinc { mi o i |  i 1 <>
          IF    o +  mi i max
          ELSE  vglue@ drop +  mi  THEN } ;
        : xinc  0 1  attribs c@ :flip and ?EXIT
          swap hskips+ swap
          ALLCHILDS  xinc { mi o i |  i 1 <>
          IF  o +  mi i max  ELSE  mi  THEN } ;
class;

\ box management                                       08apr00py
: borderbox ( o n -- o )  swap
  combined with  borderw c! ^ endwith ;
: noborderbox ( o flags -- o )  swap
  combined with  attribs dup c@ rot   or swap c! ^ endwith ;
: hfixbox ( o -- o )
  combined with  attribs dup c@ :hfix or swap c! ^ endwith ;
: vfixbox ( o -- o )
  combined with  attribs dup c@ :vfix or swap c! ^ endwith ;
: fixbox ( o -- o )
  combined with  attribs dup c@ [ :hfix :vfix or ] Literal
                                      or swap c! ^ endwith ;
: flipbox ( o -- o )
  combined with  attribs dup c@ :flip xor swap c! ^ endwith ;
: rzbox ( o -- o )  combined with
      attribs dup c@ :resized xor swap c! ^ endwith ;

\ box management                                       07dec97py

: hskips ( o n -- o )  swap
  combined with  hskip c!             ^ endwith ;
: vskips ( o n -- o )  swap
  combined with  vskip c!             ^ endwith ;

: hskip ( o -- o )  1 hskips ;
: vskip ( o -- o )  1 vskips ;
: panel ( o -- o )  hskip vskip ;

\ boxes with focus                                     21mar00py
' noop Alias component immediate
vbox class vabox
how:    : focus   ( -- )   attribs c@ :flip and
          0= IF  active focus    THEN ;
        : defocus ( -- )  attribs c@ :flip and
          0= IF  active defocus  THEN ;
        : show-you ( -- ) active show-you ;
        : keyed ( key sh -- ) over dup $0009 = swap $FE20 = or
          IF  defocus  nip 1 and
              IF    prev-active 0=  IF  prev-active drop  THEN
              ELSE  next-active 0=  IF  next-active drop  THEN
              THEN  focus show-you  EXIT
          THEN  super keyed ;
        : handle-key?  attribs c@ :flip and IF false  EXIT THEN
          0  ALLCHILDS  handle-key? or ;

\ boxes with focus                                     11nov06py

        : (clicked  ( x y b n -- x y b n / -1 )
          ALLCHILDS  2over inside?
          IF  rdrop over $18 and 0= handle-key? and
              IF  ^ r@ >o  dup active self <>
                  IF    defocus bind active  focus
                  ELSE  drop  THEN  o>  THEN
              clicked -1 o> rdrop  THEN ;
class;

hbox class habox
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
class;

\ tabulator box                                        19dec99py

hbox class htbox
how:    Create minmax 0 , 0 ,
        : >hglues ( -- min max )  0 n @ 0<>
          ALLCHILDS   & glue @ class? 0=
             IF  >r hglue@ over + >r umax r> r> umin  THEN ;
        : >hglue+ ( -- min glue )
          >hglues over - minmax 2!
          0 0  ALLCHILDS  & glue @ class?
            IF  hglue@  ELSE  minmax 2@  THEN  p+ ;
        : hglue  attribs c@ :flip and  IF  0 0
          ELSE  >hglue+  swap  n @ vskip@ IF  1+  ELSE  1-  THEN
                0max  hskip@ * +  borderw cx@ abs xS * +  swap
          THEN  2dup hglues 2! ?hfix ;

\ tabulator box                                        28mar99py
        : compose  ( -- g1 g2 )
          minmax dup push cell+ push
          >hglues over - minmax 2!
          xy     dup push cell+ push
          +skip push  hskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + vskip@ 0= IF  hskip@ -  THEN
          y @ r@ + vskip@ + xy 2!
          hglues 2@ swap negate w @ +
          vglues 2@ swap h @ max vskip@ 2* - r> 2* -
          ALLCHILDS  2swap +skip @ 0 gsize x+
                     & glue @ class?
                     IF  hglue@  ELSE  minmax 2@  THEN
                     gsize >r
                     2swap r@ over xy 2@ 2swap resize
                     r> x+ ;                    class;

\ tabulator box                                        19dec99py

vbox class vtbox
how:    Create minmax 0 , 0 ,
        : >vglues ( -- min max )  0 n @ 0<>
          ALLCHILDS   & glue @ class? 0=
             IF  >r vglue over + >r umax r> r> umin  THEN ;
        : >vglue+ ( -- min glue )
          >vglues over - minmax 2!
          0 0  ALLCHILDS  & glue @ class?
            IF  vglue  ELSE  minmax 2@  THEN  p+ ;
        : vglue  attribs c@ :flip and  IF  0 0
          ELSE  >vglue+  swap  n @ hskip@ IF  1+  ELSE  1-  THEN
                0max  vskip@ * +  borderw cx@ abs xS * + swap
          THEN  2dup vglues 2! ?vfix ;

\ tabulator box                                        28mar99py
        : compose  ( -- g1 g2 )
          minmax dup push cell+ push
          >vglues over - minmax 2!
          xy     dup push cell+ push
          +skip push  vskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + hskip@ +
          y @ r@ + hskip@ 0= IF  vskip@ -  THEN  xy 2!
          hglues 2@ swap w @ max hskip@ 2* - r> 2* -
          vglues 2@ swap negate h @ +
          ALLCHILDS  +skip @ 0 gsize y+
                     & glue @ class?
                     IF  vglue@  ELSE  minmax 2@  THEN
                     gsize >r
                     2swap dup r@ xy 2@ 2swap resize 2swap
                     r> y+ ;                    class;

\ htbox variants                                       21may97py

htbox class hatbox
how:    habox :: (clicked       habox :: show-you
        habox :: focus          habox :: defocus
        habox :: keyed          habox :: handle-key?
class;

vtbox class vatbox
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
class;

\ table boxes                                          19dec99py

hbox class htab
how:    : >htglue ( -- n )
          0  ALLCHILDS  { n }
          hglue@ parent parent with
                 n 1- tab@ p+ n tab! endwith
          n 1+ ;
        : hglue  attribs c@ :flip and  IF  0 0
          ELSE  >htglue drop parent with tabs 1- tab@ endwith
                swap hskips+ swap
          THEN  2dup hglues 2! ?hfix ;
        : hglue@  hglue ;
        : resized  parent resized ;

\ table boxes                                          19dec99py
        Variable >I
        : compose  ( -- g1 g2 )
          xy dup push cell+ push  >I push  >I off
          +skip push  hskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + vskip@ 0= IF  hskip@ -  THEN
          y @ r@ + vskip@ + xy 2!
          hglues 2@ swap negate w @ +
          vglues 2@ swap h @ max vskip@ 2* - r> 2* -
          ALLCHILDS  2swap +skip @ 0 gsize x+
                     parent parent with >I @ dup
                        >r tab@ r> 1- tab@ p- endwith
                     gsize >r
                     2swap r@ over xy 2@ 2swap resize
                     r> x+  1 >I +! ;
class;

\ table boxes                                          26apr01py

vbox class vtab
how:    : >vtglue ( -- n )
          0  ALLCHILDS  { n }
          vglue@ parent parent with
                 n 1- tab@ p+ n tab! endwith
          n 1+ ;
        : vglue  attribs c@ :flip and  IF  0 0
          ELSE  >vtglue drop parent with tabs 1- tab@ endwith
                swap vskips+ swap
          THEN  2dup vglues 2! ?vfix ;
        : vglue@  vglue ;
        : resized  parent resized ;

\ table boxes                                          19dec99py
        Variable >I
        : compose  ( -- g1 g2 )
          xy dup push cell+ push  >I push  >I off
          +skip push  vskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + hskip@ +
          y @ r@ + hskip@ 0= IF  vskip@ -  THEN  xy 2!
          hglues 2@ swap w @ max hskip@ 2* - r> 2* -
          vglues 2@ swap negate h @ +
          ALLCHILDS  +skip @ 0 gsize y+
                     parent parent with >I @ dup
                       >r tab@ r> 1- tab@ p- endwith
                     gsize >r
                     2swap dup r@ xy 2@ 2swap resize 2swap
                     r> y+  1 >I +! ;
class;

\ table boxes                                          19dec99py

htab class hatab
how:    habox :: (clicked       habox :: show-you
        habox :: focus          habox :: defocus
        habox :: keyed          habox :: handle-key?
class;

vtab class vatab
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
class;

\ parbox                                               10aug05py

: indent-glue  widget xM 0      *hglue new ;
: null-glue    0 0              *hglue new ;
: fill-glue    0 1 *fill        *hglue new ;
: space-glue   widget xN dup    *hglue new ;

Create block-par
 T] indent-glue  null-glue  null-glue  fill-glue  space-glue [
\   first-left   right      left       last-right space
Create center-par
 T] fill-glue    fill-glue  fill-glue  fill-glue  space-glue [
Create left-par
 T] indent-glue  fill-glue  null-glue  fill-glue  space-glue [
Create right-par
 T] fill-glue    null-glue  fill-glue  null-glue  space-glue [

\ parbox                                               19mar00py

vbox class parbox
public:
    cell var glues
    cell var n'
    gadget [] items
    method hboxing
    method unhbox
    static sub-box
how:
    & hbox @ sub-box !
    : init ( widget_1 .. widget_n n format -- )
      glues ! dup n' ! [],  over bind[] items
      ?DO  I !  -cell +LOOP
      0 hboxing super init ;

\ parbox                                               19mar00py

    : hglue  ( -- glue )  super hglue 2drop
      0 n' @ 0 ?DO  I items hglue drop max  LOOP  1 *fill ;
    : vglue  ( -- glue )  super vglue drop 0 ;
    : hglue@  hglue ;
    : vglue@  super vglue@ drop 0 ;

/*
    : unbox ( parbox i -- parbox i )
      ALLCHILDS  dup 0  ?DO  I over2 with items self endwith
                             link childs delete  LOOP ;
    : dispose-childs  ALLCHILDS  dispose ;
    : unhbox  ( -- parbox i' )
      self n' @  unbox  dispose-childs ;
*/
\ parbox                                               27may00py

    : check-hbox ( w -- w )
      ALLCHILDS  hglue + over <
                 IF  fill-glue 'nil add
                     1 n +!  hglue 2drop  THEN ;

    : unglue ( parbox i -- parbox i' )
      ALLCHILDS  over with dup items self endwith ^ <>
                 IF  dispose  ELSE  1+  THEN ;
    : unhbox  ( -- parbox i' )
      self 0 ALLCHILDS
      unglue  'nil bind childs  dispose ;

\ parbox                                               27jun02py

    : resize  ( x y w h -- )  h @ >r
      h ! w @ over w ! <> dpy self and
      IF  unhbox 2drop w @ hboxing >box
          dpy self dpy! w @ check-hbox drop
          vglue drop h !
      THEN  w @ h @ super resize
      r> 0= IF  parent resized  THEN ;
    : dispose
      unhbox 2drop  dispose[] items  'nil bind childs
      super dispose ;

\ formating child boxes                                19mar00py

    : glue' ( n -- glue )  cells glues @ + perform ;
    : glueW ( widget -- n )  widget with hglue endwith drop ;
    : disposeW ( widget -- ) widget with dispose endwith ;
    : hbox-new  sub-box @ new, ;

\ formating child boxes                                19mar00py
    : hboxing ( width -- box1 .. boxn n )
      dpy self 0= IF  drop
         n' @ 0 ?DO  I items self  LOOP  n' @ hbox-new 1  EXIT
      THEN
      dup 0 0  0 glue' { w w0 p q sp |
      n' @ 0 ?DO
          I items hglue drop sp glueW + w0 <
          IF    sp I items self  p 2+ to p
                w0 I items hglue drop sp glueW + - to w0
          ELSE  p IF  1 glue' p 1+ hbox-new  q 1+ to q  THEN
                sp disposeW  2 glue' I items self  2 to p
                over glueW over glueW + w swap - to w0
          THEN  4 glue' to sp
      LOOP  sp disposeW  3 glue' p 1+ hbox-new q 1+ } ;
class;

\ formating child boxes                                19mar00py

parbox class parabox
how:
    & habox @ sub-box !
    vabox :: (clicked
    vabox :: show-you
    vabox :: focus
    vabox :: defocus
    vabox :: keyed
    vabox :: handle-key?
class;

\ boxchar                                              21aug99py

widget class boxchar
public: font ptr fnt            cell var color
        cell var textwh         cell var texth
        method !textwh          early push?
        early textcenter        early textleft
        early ]C                early shadedbox
how:    : init ( cb char -- )  super init assign >callback ;
        : !textwh ( addr u -- )
          fnt self 0= IF  2drop  EXIT  THEN
          fnt size  swap textwh 2! ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;

\ boxchar                                              21aug99py

        : textcenter ( string len ox oxy -- ) { x xy |
          xywh textwh @+ @ p- p2/ p+ x xy + xy p+
          color @ 8 >> .text } ;
        : textleft ( string len ox oxy -- ) { x xy |
          xywh nip texth @ - 2/ +
          x xy + xy p+ color @ 8 >> .text } ;

\ boxchar                                              21mar00py

        : Xshadow ( -- n )  color @ $18 >> dup 0< - xS * 2/ ;
        : ~shadow ( -- ) color @ $FF000000 xor color ! ;
        : push? ( -- flag )  color @ 0< ;
        : +push ( -- ) push? 0=
          IF   ~shadow  1 1 dpy txy!  draw  0 0 dpy txy!  THEN ;
        : -push ( -- ) push?  IF   ~shadow  draw  THEN ;

        : (released ( x y b n -- )
          DOPRESS  inside?  IF  +push  ELSE  -push  THEN 2drop ;
        : >released ( x y b n -- flag )
          /step @ after >r +push nip 1 and
          IF    rdrop (released
          ELSE  2drop dpy sync r> till  THEN  push? -push ;
        : handle-key?  true ;

\ boxchar                                              20oct99py

        : shadedbox ( -- )
          xywh Xshadow abs xywh- color @ dpy box
          shadow Xshadow xywh drawshadow ;
        : clicked ( x y b n -- ) callback click ;
        : keyed ( key sh -- )  callback key ;
        : defocus  color defocuscol chcol  draw ;
        : focus    super focus color focuscol   chcol  draw ;
        : hglue  textwh @       xS 2* + 1+ 1 *fil ;
        : vglue  texth @ xS 2* + 1+ 1 *fil ;

\ boxchar                                              26sep99py

        : moved ( x y -- )  super moved  callback enter ;
        : leave ( -- )  callback leave ;
        : assign ( char -- )  $10 << defocuscol @ @ or
          [ 2 $18 << ] Literal or color !
          dpy self IF  !resized  THEN ;
        : !resized  color 2+ 1 !textwh ;
        : ]C ( o -- ) callback set-called ;
        : dpy! ( dpy -- )  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font!  THEN ;
        : font! ( font -- )  bind fnt
          dpy self IF  !resized  THEN ;
class;

\ triangle button                                      28mar99py

boxchar class tributton
public: early ltri              early rtri
        early utri              early dtri
how:    \ init  ( callback n -- )
        : assign ( n -- )  $FF and super assign ;

\ triangle button                                      23aug97py
        | Create pd1  0 c, 1 c, 2 c, 2 c, 2 c, 1 c, 0 c, 0 c,
        : pd ( x y h n -- )
          dup 7 and pd1 + c@ swap 2- 7 and pd1 + c@
          rot tuck * 2/ -rot * 2/ swap p+ ;
        : triangle abs { x y h c1 c2 c3 pos n |
          x y h pos    pd <poly x y h pos 3+ pd poly#
          x y h pos 6+ pd poly# poly> color @ dpy fill
          n 0 ?DO x y h pos    pd  x y h pos 3+ pd  c1 dpy line
                  x y h pos 3+ pd  x y h pos 6+ pd  c2 dpy line
                  x y h pos 6+ pd  x y h pos    pd  c3 dpy line
                  x 1+ to x  y 1+ to y  h 2- to h  LOOP } ;
        : ltri ( x y h lc sc -- ) tuck      4 Xshadow triangle ;
        : rtri ( x y h lc sc -- ) over      0 Xshadow triangle ;
        : utri ( x y h lc sc -- ) dup       6 Xshadow triangle ;
        : dtri ( x y h lc sc -- ) swap dup  2 Xshadow triangle ;

\ triangle button                                      07nov99py

public: Table: tritable  ltri utri rtri dtri [
[IFDEF] x11
        Create triarrows XC_sb_left_arrow  ,
                         XC_sb_up_arrow    ,
                         XC_sb_right_arrow ,
                         XC_sb_down_arrow  ,
[THEN]
[IFDEF] win32
        Create triarrows 3 [FOR] mouse_cursor , [NEXT]
[THEN]
        : moved ( x y -- ) super moved
          color @ $E >> $C and triarrows + @ dpy set-cursor ;
class;

\ triangle button                                      11apr99py

tributton class slidetri
how:    : hglue  xM 0 ;
        : vglue  xM 0 ;
class;

' noop alias TRI:                               immediate

0 Constant :left
1 Constant :up
2 Constant :right
3 Constant :down

\ togglechar                                           11nov06py
boxchar class togglechar
public: cell var oncolor
        method set              method reset
how:    &20 /step V!
        : assign  ( char- char+ -- )
          $10 << defocuscol @ @ or [ 2 $18 << ] Literal or
          oncolor ! super assign ;
        : reverse ( -- )  color @ oncolor @ color ! oncolor !
          draw ;
        : >released  reverse super >released ;
        : defocus  oncolor defocuscol chcol super defocus ;
        : focus    oncolor   focuscol chcol super focus   ;
        : reset  callback reset ;
        : set  callback set ;

\ togglechar                                           13nov06py

        : click-it ( -- )  bl 0 parent keyed
          parent self combined with active show-you draw
          endwith ;
        : keyed ( key sh -- )
          over $FF52 & hbox @ parent class? + =
          IF  2drop parent prev-active  0=
              IF   parent prev-active drop
              THEN  click-it
              EXIT  THEN
          over $FF54 & hbox @ parent class? + =
          IF  2drop parent next-active  0=
              IF  parent first-active  THEN
              click-it  EXIT  THEN
          super keyed ;

\ button                                               20may06py

        : clicked ( x y b n --)  over $08 and
          IF  2drop 2drop parent prev-active  0=
              IF   parent prev-active drop
              THEN  click-it
              EXIT  THEN
          over $10 and
          IF  2drop 2drop parent next-active  0=
              IF  parent first-active  THEN
              click-it  EXIT  THEN
          callback click draw ;
        : get  callback fetch ;
class;

\ button                                               27jun02py
boxchar class button
public: cell var text
how:    : init ( callback addr len -- )
          defocuscol @ @ [ 2 $18 << ] Literal or color !
          super init ;
        : dispose ( -- )  text $off super dispose ;
        : hglue  textwh @ xS 2* + 1+ 1 *fil ;
        : vglue  texth  @ xS 2* + 1+ 1 *fil ;
        : text! ( addr n -- )  text $!
          dpy self IF  !resized  THEN ;
        : get ( -- addr len ) text $@ ;
        : assign ( addr len -- )  text!
          dpy self IF  parent resized  THEN ;
        : !resized  text $@ !textwh ;
class;

\ button variants                                      13may00py

button class lbutton
class;

lbutton class text-label
how:    : init ( addr len -- ) ( >r)
          noop-act -rot ( r>) super init ;
        widget :: handle-key?
class;

text-label class text-word
class;

\ button variants                                      05mar00py

text-label class menu-label
how:    7 colors focuscol !     7 colors defocuscol !
        widget :: focus
        widget :: defocus
class;

\ toggle buttons                                       11apr99py

togglechar class tbutton
public: cell var text           early halfshade
how:    : init ( callback addr len -- )
          defocuscol @ @ oncolor !
          defocuscol @ @   color !
          boxchar :: init ;
        : halfshade ( -- )  color @ $18 >>
          IF    xywh xS 2/ xywh- color @ dpy box
                shadow swap xS 2/ xywh drawshadow
          ELSE  xywh color @ dpy box  THEN ;

\ toggle buttons                                       27jun02py

        : text! ( addr n -- )  text $!
          dpy self IF  !resized  THEN ;
        : assign ( addr len -- )  text!
          w @ 0= ?EXIT  w @   textwh @+ @
          xS 2* + h @ max h ! dup xS 2* + 1+ w @ max w !
          < IF  parent resized  ELSE  draw  THEN ;
        : !resized  text $@ !textwh ;
        : dispose text $off super dispose ;
class;

\ radio button, flipbutton, togglebutton               10aug05py

tbutton class rbutton                           class;
tbutton class flipbutton                        class;
tbutton class topindex                          class;

*hglue class topglue
how:    : init ( -- )  0 1 *filll super init ;
class;

combined with
F : flipper  combined ' +flip
    :[ attribs c@ :flip or attribs c! hide ]:
    toggle new ;
endwith

\ Topindex, topglue                                    11apr99py
tbutton class togglebutton
public: cell var text1
        cell var textwh1        cell var texth1
how:    : ?size ( -- )  dpy self IF  !resized  ELSE  EXIT  THEN
          w @ 0= ?EXIT
          w @   textwh @ textwh1 @ max  texth @ texth1 @ max
          xS 2* + h @ max h ! dup xS 2* + 1+ w @ max w !
          < IF  parent resized  ELSE  draw  THEN ;
        : !resized  ( -- )  super !resized
          text1 $@ 0 textsize swap textwh1 2! ;
        : text! ( addr n -- )  text1 $!  ?size ;
        : assign ( addr n -- )  text $!  ?size ;
        : init ( toggle addr1 n1 addr2 n2 -- )
          text! super init [ 2 $18 << ] Literal
          dup color +!  oncolor +! ;            class;

\ Icon+text button                                     11apr99py

button class icon-button
public: icon-pixmap ptr icon
how:    : init ( callback icon addr len -- )  text! super init ;
        : assign ( icon -- )  bind icon
          w @ IF  parent resized  THEN ;
class;

icon-button class icon-but
how:    \ init ( callback icon -- )
        : text!  s" " super text! ;
class;

\ toggleicon, flipicon                                 28dec97py

togglechar class toggleicon
public: icon-pixmap ptr icon-
        icon-pixmap ptr icon+
how:    : assign  ( icon- icon+ -- )
          bind icon+ bind icon- bl bl super assign ;
class;

togglechar class flipicon
public: icon-pixmap ptr icon
how:    : assign  ( icon -- )
          bind icon  bl bl super assign ;
class;

\ togglebutton with text                               11apr99py

tbutton class ticonbutton
public: icon-pixmap ptr icon-   icon-pixmap ptr icon+
how:    : assign ( icon- icon+ -- )
          bind icon+ bind icon- ;
        : init ( callback icon- icon+ addr len -- )
          defocuscol @ @ oncolor !
          defocuscol @ @   color !
          text!  boxchar :: init ;
class;

\ icon with small text                                 21mar04py
icon-button class big-icon
how:    : inside? ( x y -- ) 2dup super inside?
          0= IF  2drop false  EXIT  THEN
          2dup x @ w @ icon w @ - 2/ + y @ p-
          over icon w @ u< over icon h @ u< and
          IF
[IFDEF] x11   icon shape @ -1 = IF
                    >r >r ZPixmap -1 1 1 r> r> swap
                    icon image @ 1- dpy xrc dpy @ XGetImage >r
                    r@ IF  0 0 r@ XGetPixel r> XDestroyImage
                           0< >r  THEN
              ELSE  >r >r ZPixmap 1 1 1 r> r> swap
                    icon shape @ dpy xrc dpy @ XGetImage >r
                    r@ IF  0 0 r@ XGetPixel r> XDestroyImage
                           0<> >r  THEN  THEN            [THEN]

\ icon with small text                                 21mar04py

[IFDEF] win32 swap icon shape @ GetDC GetPixel 0<> >r    [THEN]
          ELSE  2drop false >r  THEN
          xywh textwh @+ @ p- drop 2/ icon h @ p+ p-
          textwh @+ @ rot swap u< >r u< r> and r> or ;
class;

\ simple text field                                    19dec99py
button class (textfield
public: cell var curpos         cell var selw
        cell var curx           cell var curw
        cell var old-h          cell var ds
        method ins              method del
        method c                method cur!
        early 'text+            early 'text-
how:    0 colors focuscol !     7 colors defocuscol !
        : init  ( dostroke addr len -- )
          1 selw ! super init ;
        : show-you ( -- )  curx 2@ swap 2/ + x @ +
          y @ h @ 1+ 2/ + dpy show-me ;
        : hglue  textwh @ xS +
          1- ds @ >> 1+ ds @ << dup old-h ! 1 *fil ;
        : vglue  texth  @                   1 *fil ;

\ simple text input field                              13jan05py

        : textwh@ ( addr u -- w h )
          fnt self 0= IF  2drop 0 0  ELSE  fnt size  THEN ;
        : !curxw ( -- )
          text $@ 1+ 2dup curpos @ selw @ 0max + min
          textwh@ drop curx !
          curpos @ selw @ 0min + /string
          selw @ abs min over dup xchar+ swap - max
          textwh@ drop dup curw !
          negate curx +! ;

\ simple text input field                              20feb00py
        : text! ( -- )  dpy self 0= ?EXIT  !resized
          0 text $@ + c!
          hglue drop dup w @ <= swap r> = and
          IF  draw  ELSE
              parent self 0= ?EXIT  parent resized  THEN
          callback toggle ;
        : assign ( addr n -- )  tuck text $! bl text $@ + c!
          curpos ! text! ;
        : get ( -- addr len )  text $@ ;
        : >pos ( x -- n )
          text $@ 0
          ?DO  2dup I 1+ textwh@ drop <=
               IF  2drop I unloop EXIT  THEN
          LOOP  2drop text $@len ;
        : !resized ( -- )  text $@ 1+ !textwh !curxw ;

\ simple text input field                              06jan05py

        : moved ( x y -- )  2drop
[IFDEF] x11   XC_xterm   [THEN] [IFDEF] win32 IDC_IBEAM  [THEN]
          dpy set-cursor  ^ dpy set-rect ;
        : (dpy  [IFDEF] x11    dpy get-win  dpy xrc dpy @
          [ELSE] 0 0 [THEN] ;

        : 'text ( -- addr )  text $@ curpos @ /string
          0= IF  bl over c!  THEN ;
        : 'text+ ( -- len )  'text dup xchar+ swap - ;
        : 'text- ( -- len )  'text dup
          curpos @ IF  xchar-  THEN swap - ;

\ simple text input field                              04nov06py

        : ins ( addr u -- )  text curpos @ $ins text! ;
        : del ( n -- )  text curpos @ rot $del text! ;
        : c ( n -- flag )
          dup 0>= IF    dup IF  'text+ max  THEN
                        curpos @ text $@len >=
                  ELSE  'text- min
                        curpos @ 0<>  THEN  swap
          curpos @ + 0max text $@len min curpos !
          'text+ selw !  !curxw draw show-you ;
        : cur! ( n -- )  curpos @ - c drop ;

\ simple text input field                              16jan05py

        : ins-sel ( -- )
          (dpy @select dup >r ins r> c drop ;
        : >select ( n -- )  selw @ >r
          dup selw ! text $@ rot dup >r
          0min curpos @ + /string r> abs min
          -select +select
          [IFDEF] x11    dpy get-win  dpy xrc dpy @
          [ELSE] 0 0 [THEN]  !select !curxw
          selw @ r> <> IF  draw  THEN ;
        : sel-word ( -- )
          text $@ 2dup curpos @ /string bl scan nip -
          2dup bl -scan nip /string >r
          text $@ drop - curpos ! r> >select ;
        : sel-all ( -- ) curpos off text $@len >select ;

\ simple text input field                              14apr01py

        Variable click
        : clicked ( x y b n -- )
          click @ 0= IF  click on  callback click  click off
                         EXIT  THEN  1 selw !
          swap >r 2 pick x @ - >pos cur!  r>
              1 and 0=  IF  2drop drop ins-sel  EXIT  THEN
          dup 1 and 0=  IF  nip nip
             dup 4 = IF  drop sel-word  EXIT  THEN
                 4 > IF       sel-all         THEN  EXIT THEN
          drop
          DOPRESS  drop dup y @ h @ 2/ + dpy scroll
                   x @ - >pos curpos @ - >select 2drop ;
class;

\ text input actor                                     06jan05py

actor class edit-action
public: static key-methods      & caller (textfield asptr edit
        early bind-key          early find-key
        cell var stroke
how:    0 key-methods !
        : find-key ( key -- addr )  >r key-methods
          BEGIN  @ dup  WHILE  dup 2 cells + @ r@ =
                 IF  rdrop  EXIT  THEN  REPEAT  rdrop ;
        : key ( key sh -- ) drop  dup 0= IF  drop  EXIT  THEN
          dup shift-keys? IF  drop  EXIT  THEN  dup find-key dup
          IF    cell+ @ caller send drop
          ELSE  drop char$ edit with ins 1 c drop endwith
          THEN ;
        : click ( x y b n -- ) caller clicked ;

\ text input actor                                     15apr01py

        : bind-key ( key method -- )
          here key-methods @ A, key-methods ! A, , ;
        : init ( o xt -- ) stroke ! super init ;
        : store ( addr u -- )  edit assign ;
        : toggle ( -- )  stroke @ called send ;
        : fetch ( -- addr u )  edit get ;
class;

' :[ alias ST[                               immediate restrict
: ]ST postpone ]: edit-action postpone new ; immediate restrict

\ text input key binding                               15apr01py

: K[ ( key -- )  (textfield postpone with :noname ;
: ]K ( key sys ) postpone ; (textfield postpone endwith
  & edit-action >o edit-action bind-key o> ;          immediate
: K-alias ( key1 key2 -- ) swap edit-action find-key
  ?dup IF  cell+ @
           & edit-action >o edit-action bind-key o> THEN ;

\ text input key binding                               07jan07py

$FF08  K[ selw @ 'text+ <> IF  selw @ dup 0< IF  dup c drop THEN
          abs del  ELSE -1 c IF  selw @ del 0 c drop THEN  THEN
                                      ]K  $FF08 ctrl H K-alias
$FF51  K[ -1 c drop                   ]K
$FF53  K[  1 c drop                   ]K
$FFFF  K[ selw @ 'text+ <> IF  selw @ dup 0< IF  dup c drop THEN
          abs del  ELSE 0 c 0= IF  selw @ del 0 c drop THEN THEN
                                      ]K  $FFFF ctrl ? K-alias
$FF1B  K[ s" " assign                 ]K  $FF1B ctrl [ K-alias
ctrl L K[ parent resized              ]K
ctrl K K[ text $@ curpos @ min assign ]K
$FF50  K[ 0 cur!                      ]K  $FF50 ctrl A K-alias
$FF57  K[ text $@len cur!             ]K  $FF57 ctrl E K-alias
ctrl W K[ selw @ dup 0min c drop abs del ]K

\ key action with text pointer                         14apr01py

edit-action class edit-var
public: cell var text
how:    : update  edit get text @ $! ;
        : store   super store  update ;
        : init ( o addr -- )  text ! ['] noop  super init ;
        : key ( kb sh -- )  super key  update ;
        : click ( x y b n -- )  super click  update ;
class;

' noop alias VT[                             immediate restrict
: ]VT edit-var postpone new ;                immediate restrict

\ number input field                                   27apr98py
edit-action class number-action
public: cell var nbase
how:    : ># ( d -- addr u ) base push nbase @ base ! tuck dabs
          <# #S  nbase @ $10 = IF  '$ hold  THEN
                 nbase @ %10 = IF  '% hold  THEN  rot sign  #> ;
        : key ( key sh -- ) drop base push nbase @ base !
          dup shift-keys? IF  drop  EXIT  THEN  dup find-key dup
          IF    cell+ @ caller send drop
          ELSE  drop dup digit? nip 0= ?EXIT
                sp@ 1 edit with ins drop 1 c drop endwith
          THEN  stroke @ called send ;
        : store ( d -- )  ># edit assign ;
        : fetch ( -- d ) edit get base push decimal s>number ;
        : init  &10 nbase ! super init ;
class;

\ number input field                                   28aug99py

: #[ ( key -- )  (textfield postpone with :noname ;
: ]# ( key sys ) postpone ; (textfield postpone endwith
  & number-action >o number-action bind-key o> ;      immediate
'$ #[ callback self number-action with
      fetch $10 nbase ! store endwith ]#
'% #[ callback self number-action with
      fetch %10 nbase ! store endwith ]#
'& #[ callback self number-action with
      fetch &10 nbase ! store endwith ]#
'- #[ callback self number-action with
      fetch dnegate store endwith ]#
' :[ alias SN[                               immediate restrict
: ]SN postpone ]: number-action postpone new ;
                                             immediate restrict

\ number edit variables                                15apr01py

number-action class edit-int
public: cell var int
how:    : update  fetch drop int @ ! ;
        : store   super store  update ;
        : init ( o addr -- )  int ! ['] noop  super init ;
        : key ( kb sh -- )  super key  update ;
        : click ( x y b n -- )  super click  update ;
class;

' noop alias IV[                             immediate restrict
: ]IV edit-int postpone new ;                immediate restrict

\ text field derivates                                 19dec99py

habox class textfield
public: (textfield ptr edit
how:    : init ( act xxx -- )
          rot s" " (textfield new
          bind edit  assign  5 edit ds !  edit self
          1 super init -2 borderw c! ;
        : assign ( xxx -- ) edit callback store ;
        : get ( -- xxx ) edit callback fetch ;
        : clicked ( x y b n -- ) dup 0= IF 2drop 2drop EXIT THEN
          super clicked ;
class;

: text@  (textfield callback fetch ;

\ text field derivates                                 19dec99py

textfield class infotextfield
public: text-label ptr info
how:    : init ( act xxx addr2 u2 -- )
          text-label new bind info
          rot s" " (textfield new
          bind edit  assign  5 edit ds !
          info self 1 habox new hfixbox
          edit self 1 habox new -2 borderbox
          2 super super init ;
        : text! ( addr u -- ) info assign ;
class;

\ text field derivates                                 19dec99py
hatab class tableinfotextfield
public: (textfield ptr edit
        text-label ptr info
how:    : init ( act xxx addr2 u2 -- )
          text-label new bind info
          rot s" " (textfield new
          bind edit  assign  5 edit ds !
          0 1 *fil 2dup glue new
          info self 1 habox new hfixbox 2 habox new hfixbox
          edit self 1 habox new -2 borderbox
          2 super super init ;
        infotextfield :: text! ( addr u -- )
        textfield :: assign ( xxx -- )
        textfield :: get ( -- xxx )
        textfield :: clicked ( x y b n -- )     class;

\ vrbox (radio box)                                    27may00py
vbox class vrbox
public: early reset-childs      early activate?
how:    : reset-childs  ALLCHILDS
          & togglechar @ class?  IF  togglechar reset draw THEN
          & combined   @ class?  IF  recurse  THEN ;
        : (clicked ( x y b n -- )  reset-childs super (clicked ;
        : activate? ( key -- flag )
           dup bl = swap #cr = or ;
        : keyed   ( key sh -- )  over  activate?
          IF  reset-childs  THEN  super keyed ; class;
vabox  class varbox
how:    : (clicked ( x y b n -- )
          vrbox :: reset-childs super (clicked ;
        : keyed   ( key sh -- )   over vrbox :: activate?
          IF  vrbox :: reset-childs  THEN  super keyed ;  class;

\ hboxes                                               04sep97py
hbox   class hrbox
how:    vrbox  :: (clicked      vrbox  :: keyed         class;
habox  class harbox
how:    varbox :: (clicked      varbox :: keyed         class;

htbox   class hrtbox
how:    vrbox  :: (clicked      vrbox  :: keyed         class;
hatbox  class hartbox
how:    varbox :: (clicked      varbox :: keyed         class;

vtbox   class vrtbox
how:    vrbox  :: (clicked      vrbox  :: keyed         class;
vatbox  class vartbox
how:    varbox :: (clicked      varbox :: keyed         class;

\ dialog management                                    28aug99py

vabox class modal               \ cell var app
public: gadget ptr default      method default!
how:    : init ( widget1 .. widgetn n default -- )
          ( up@ app ! ) ( swap ) ?dup IF  bind default  THEN
          super init ;
        : close ( -- )  dpy close ;
        : keyed ( key sh -- )  over #cr = default self and
          IF  default keyed  ELSE  super keyed  THEN ;
        : default!  default self over bind default
          <> IF  draw  THEN ;
class;

\ text with parbox                                     27may00py

parbox class text-parbox
how:    Variable text-string
        : init ( addr u format -- )  >r
          text-string $! 0 text-string bl
          :[ -trailing bl skip text-word new swap 1+ ]: $iter
          r> super init  text-string $off ;
        : assign ( addr u -- ) text-string $!  dispose-childs
          unhbox 2drop dispose[] items 'nil bind childs
          0 text-string bl
          :[ -trailing bl skip text-word new swap 1+ ]: $iter
          dup n' !  text-string $off  [],  over bind[] items
          ?DO  I !  -cell +LOOP  0 hboxing dup n ! >box
          dpy self dpy! ;
class;

\ new slider                                           20oct99py
widget class arule
public: cell var color
        cell var gethglue       cell var getvglue
how:    : init ( actor hxt vxt -- )
          super init getvglue ! gethglue ! >callback
          defocuscol @ @ assign ;
        : assign ( color -- )  color ! ;
        : Xshadow ( -- n )  color @ $18 >> dup 0< - ;
        : hglue ( -- glue ) gethglue @ callback called send ;
        : vglue ( -- glue ) getvglue @ callback called send ;
        : clicked  callback click ;
        : keyed    callback key ;
        : defocus Xshadow 0= ?EXIT color defocuscol tocol draw ;
        : focus   Xshadow 0= ?EXIT color focuscol   tocol draw ;
        boxchar :: >released                    class;

\ new slider                                           11dec04py
hbox class hslider public:
        early part0             early part1
        early part2             early part3
        method lstep            method rstep
        method lpage            method rpage
        method slide            method do-slide
        method reslide          method subbox
how:    : get  ( -- steps step pos )
          callback self 0=  IF 1 1 0 EXIT THEN callback fetch ;
        : ?fil ( n1 n2 -- )  1 *fil < IF  *fil  THEN ;
        : part0 ( -- glue )  xM    1 *fil            ;
        : part1 ( -- glue )  0     get nip swap  ?fil ;
        : part2 ( -- glue )  xS 3* get drop swap ?fil ;
        : part3 ( -- glue )  0     get + over swap - swap ?fil ;
        : draw ( -- )  xywh resize super draw ;

\ new slider                                           08mar07py
        : lstep ( -- ) get nip nip 1- 0max             reslide ;
        : rstep ( -- ) get >r - r> 1+ min              reslide ;
        : lpage ( -- ) get swap 2 max - 1+ nip 0max    reslide ;
        : rpage ( -- ) get over 2 max + 1- >r - r> min reslide ;
        : init ( callback -- )  >callback subbox super init ;
        : reslide ( n -- )  get nip nip case? ?EXIT
          assign draw ;
        : do-slide  drop  ( pos x0 x )
          over - >r over r> ( pos dx )
          get 2drop 2* w @ hglue drop -
          ?dup IF  */ 1+ 2/  ELSE  2drop 0  THEN  +
          0max get drop - min reslide 2drop ;
        : slide ( x y b n -- ) drop
          nip 1 and 0= IF  2drop 0  EXIT  THEN
          drop get nip nip swap 0 -rot DOPRESS do-slide ;

\ new slider                                           08mar07py
        : keyed ( key sh -- )  drop
          $FF51 case?  IF  lstep  EXIT  THEN
          $FF53 case?  IF  rstep  EXIT  THEN
          $FF50 case?  IF  0 reslide           EXIT  THEN
          $FF57 case?  IF  get drop - reslide  EXIT  THEN
          $FF55 case?  IF  lpage  EXIT  THEN
          $FF56 case?  IF  rpage  EXIT  THEN  drop ;
        : assign   ( pos -- )  callback store ;
        : hglue@ hglue ;
        : moved ( x y -- )  widget :: moved  callback enter ;
        : leave ( -- )  callback leave ;
        : clicked ( x y b n -- )  leave
          over $10 and  IF  2drop 2drop rpage  EXIT  THEN
          over $08 and  IF  2drop 2drop lpage  EXIT  THEN
          super clicked ;                       class;

\ new slider                                           11dec04py
vbox class vslider public:
        early part0             early part1
        early part2             early part3
        method lstep            method rstep
        method lpage            method rpage
        method slide            method do-slide
        method reslide          method subbox
how:    : get  ( -- steps step pos )
          callback self 0=  IF 1 1 0 EXIT THEN callback fetch ;
        : ?fil ( n1 n2 -- )  1 *fil < IF  *fil  THEN ;
        : part0 ( -- glue )  xM    1 *fil            ;
        : part1 ( -- glue )  0     get nip swap  ?fil ;
        : part2 ( -- glue )  xS 3* get drop swap ?fil ;
        : part3 ( -- glue )  0     get + over swap - swap ?fil ;
        : draw ( -- )  xywh resize super draw ;

\ new slider                                           08mar07py

        : lstep ( -- ) get nip nip 1- 0max             reslide ;
        : rstep ( -- ) get >r - r> 1+ min              reslide ;
        : lpage ( -- ) get swap 2 max - 1+ nip 0max    reslide ;
        : rpage ( -- ) get over 2 max + 1- >r - r> min reslide ;
        hslider :: reslide ( n -- )
        : init ( callback -- )  >callback subbox super init ;
        : do-slide  nip  ( pos y0 y )
          over - >r over r> ( pos dy )
          get 2drop 2* h @ vglue drop -
          ?dup IF  */ 1+ 2/  ELSE  2drop 0  THEN  +
          0max get drop - min reslide 2drop ;
        : slide ( x y b n -- ) drop
          nip 1 and 0= IF  2drop 0  EXIT  THEN
          nip get nip nip swap 0 -rot DOPRESS do-slide ;

\ new slider                                           08mar07py

        : keyed ( key sh -- )  drop
          $FF52 case?  IF  lstep  EXIT  THEN
          $FF54 case?  IF  rstep  EXIT  THEN
          $FF50 case?  IF  0 reslide           EXIT  THEN
          $FF57 case?  IF  get drop - reslide  EXIT  THEN
          $FF55 case?  IF  lpage  EXIT  THEN
          $FF56 case?  IF  rpage  EXIT  THEN  drop ;
        : assign   ( pos -- ) callback store ;
        : vglue@ vglue ;
        : moved ( x y -- )  widget :: moved  callback enter ;
        : leave ( -- )  callback leave ;
        hslider :: clicked
class;

\ hslider0 vslider0                                    21mar00py

hslider class hslider0
how:    : handle-key?  get drop <> ;
        : draw    handle-key?  IF  super draw  THEN ;
        : vglue   handle-key?
          IF  super vglue   ELSE  0 0  THEN ;
        : vglue@  handle-key?
          IF  super vglue@  ELSE  0 0  THEN ;
        : hglue   super hglue  drop 1 *fill ;
        : hglue@  super hglue@ drop 1 *fill ;
        : focus   handle-key?  IF  super focus    THEN ;
        : defocus handle-key?  IF  super defocus  THEN ;
class;

\ hslider0 vslider0                                    21mar00py

vslider class vslider0
how:    : handle-key?  get drop <> ;
        : draw    handle-key?  IF  super draw  THEN ;
        : hglue   handle-key?
          IF  super hglue   ELSE  0 0  THEN ;
        : hglue@  handle-key?
          IF  super hglue@  ELSE  0 0  THEN ;
        : vglue   super vglue  drop 1 *fill ;
        : vglue@  super vglue@ drop 1 *fill ;
        : focus   handle-key?  IF  super focus    THEN ;
        : defocus handle-key?  IF  super defocus  THEN ;
class;

\ scaler helper words                                  11dec04py

: max10 ( n max -- n' )  >r  &1000000000
  BEGIN  tuck mod dup r@ u>  WHILE
         swap &10 /  REPEAT  nip rdrop ;
: digit+ ( digit max n -- max n' )
  &10 * rot '0 - over 0< IF  -  ELSE  +  THEN ;

\ new scaler                                           03dec06py

hslider class hscaler public:   cell var offset
        cell var textwh         cell var texth
        cell var text*/         cell var text/
        font ptr fnt            cell var color
        early part0a            early part0b
        early part1             early part3
        early part4             early part5
        early slide1
public: method #>text           early scalekey

\ new scaler                                           03dec06py

how:    : #>text ( n -- addr u )  base push decimal
          text/ @ m* tuck dabs  <#
          text*/ @ 1 ?DO  # I 9 * +LOOP
          text*/ @ 1 > IF  '. hold  THEN  #S rot sign #> ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;
        : get  ( -- steps step pos )  super get 0 swap ;
        : o+  ( n -- n' )  offset @  + ;
        : o-  ( n -- n' )  offset @  - ;
        : o'+ ( n -- n' )  offset @  text*/ 2@ */  + ;
        : o'- ( n -- n' )  offset @  text*/ 2@ */  - ;
        : init  1 1 text*/ 2!  super init ;

\ new scaler                                           03dec06py
        : keyed ( k s -- k s )  over '0 '9 1+ within
          IF  drop get >r - text*/ 2@ */ r> text*/ 2@ */ digit+
              dup 0< IF  nip negate 0 o'- max10 negate
                   ELSE  swap o'+ max10  THEN
              text*/ 2@ swap */ swap text*/ 2@ swap */ swap
              reslide  EXIT  THEN                    over #bs =
          IF  2drop get nip nip s>d &10 sm/rem nip o- 0max o+
              reslide  EXIT  THEN                     over '% =
          IF  2drop get >r - r> 0max &100 min
              &100 */ o+ reslide  EXIT  THEN          over '- =
          IF  2drop get >r - 1- r> negate o- 0max min o+ reslide
              EXIT  THEN  >r
          $FF50 case? IF  0 o+ reslide          rdrop  EXIT THEN
          $FF57 case? IF  get drop - o+ reslide rdrop  EXIT THEN
          r> super keyed ;

\ new scaler                                           06mar07py
        : !resized  fnt self 0= ?EXIT
          get drop - #>text fnt size swap textwh 2!
          super !resized ;
        boxchar :: handle-key?
        : show   get nip o- 0max min o+ assign  super show ;
        : part0a ( -- glue ) 0 1 *fil ;
        : part0b ( -- glue ) xM 0 ;
        : part1 ( -- glue )  0 get o- nip swap  ?fil ;
        : part3 ( -- glue )  0 get o- + over swap - swap ?fil ;
        : part4 ( -- glue )  textwh @ 2/ xS + 0 ;
        : part5 ( -- glue )  texth @ 0 ;

\ new scaler                                           08mar07py

        : lstep get o- nip nip 1- 0max             o+ reslide ;
        : rstep get o- >r - r> 1+ min              o+ reslide ;
        : lpage get o- nip nip text*/ 2@ swap /
          - 0max     o+ reslide ;
        : rpage get o- >r - r> text*/ 2@ swap /
          + min      o+ reslide ;
        : clicked ( x y b n -- )
          over $10 and  IF  2drop 2drop rpage  EXIT  THEN
          over $08 and  IF  2drop 2drop lpage  EXIT  THEN
          super clicked ;

\ new scaler                                           20dec04py
        : do-slide  ( pos x0 x y  -- )  drop  ( pos x0 x )
          over - >r over r> ( pos dx )
          get 2drop 2* w @ hglue drop -
          ?dup IF  */ 1+ 2/  ELSE  2drop 0  THEN  +
          o- 0max get drop - min o+ reslide 2drop ;
        : slide1 ( x y b n -- ) drop >r drop  0 -rot
          0 o+ x @ part4 drop 2* + 2swap
          r> 1 and 0= IF  do-slide  EXIT  THEN
          2drop  DOPRESS  do-slide ;
        : font! dup bind fnt super font! ;
        : focus    focuscol @ @ color !   super focus   draw ;
        : defocus  defocuscol @ @ color ! super defocus draw ;
        : dpy!  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font!  THEN ;
class;

\ new scaler                                           03dec06py

vslider class vscaler public:   cell var offset
        cell var textwh         cell var texth
        cell var text*/         cell var text/
        font ptr fnt            cell var color
        early part0a            early part0b
        early part1             early part3
        early part4             early part5
        early slide1
public: method #>text           early scalekey

\ new scaler                                           08mar07py
how:    : #>text ( n -- addr u )  base push decimal
          text/ @ m* tuck dabs  <#
          text*/ @ 1 ?DO  # I 9 * +LOOP
          text*/ @ 1 > IF  '. hold  THEN  #S rot sign #> ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;
        : get  ( -- steps step pos )  super get 0 swap ;
        : o+ ( n -- n' ) offset @ + ;
        : o- ( n -- n' ) offset @ - ;
        : o'+ ( n -- n' ) offset @  text*/ 2@ */  + ;
        : o'- ( n -- n' ) offset @  text*/ 2@ */  - ;
        : clicked ( x y b n -- )  leave
          over $10 and  IF  2drop 2drop lpage  EXIT  THEN
          over $08 and  IF  2drop 2drop rpage  EXIT  THEN
          super clicked ;

\ new scaler                                           08mar07py
        : init  1 1 text*/ 2!  super init ;
        : keyed ( k s -- k s )  over '0 '9 1+ within
          IF  drop get >r - text*/ 2@ */ r> text*/ 2@ */ digit+
              dup 0< IF  nip negate 0 o'- max10 negate
                   ELSE  swap o'+ max10  THEN
              text*/ 2@ swap */ swap text*/ 2@ swap */ swap
              reslide  EXIT  THEN                    over #bs =
          IF  2drop get nip nip s>d &10 sm/rem nip o- 0max o+
              reslide  EXIT  THEN                     over '% =
          IF  2drop get >r - r> 0max &100 min
              &100 */ o+ reslide  EXIT  THEN          over '- =
          IF  2drop get >r - 1- r> negate o- 0max min o+ reslide
              EXIT  THEN  drop

\ new scaler                                           20may06py
          $FF52 case?  IF  rstep  EXIT  THEN
          $FF54 case?  IF  lstep  EXIT  THEN
          $FF50 case?  IF  get drop - o+ reslide  EXIT  THEN
          $FF57 case?  IF  0 o+ reslide           EXIT  THEN
          $FF55 case?  IF  rpage  EXIT  THEN
          $FF56 case?  IF  lpage  EXIT  THEN  0  super keyed ;
        : !resized fnt self 0= ?EXIT  get drop -
          o+ #>text fnt size swap  0 o+ #>text fnt size drop
          max xS + textwh 2! super !resized ;
        : part4 ( -- glue )  texth @ 2/ xS + 0 ;
        : part5 ( -- glue )  textwh @ 0 ;
        : part0a ( -- glue ) 0 1 *fil ;
        : part0b ( -- glue ) xM 0 ;
        : part1 ( -- glue )  0 get o- nip swap  ?fil ;
        : part3 ( -- glue )  0 get o- + over swap - swap ?fil ;

\ new scaler                                           11dec04py
        : do-slide  ( pos y0 x y -- )  nip  ( pos y0 y )
          over - >r over r> ( pos dy )
          get 2drop 2* h @ vglue drop -
          ?dup IF  */ 1+ 2/  ELSE  2drop 0  THEN  +
          o- 0max get 2drop tuck min - o+ reslide 2drop ;
        : slide1 ( x y b n -- ) drop >r drop  0 -rot
          0 o+ y @ part4 drop + xS + 2swap
          r> 1 and 0= IF  do-slide  EXIT  THEN
          2drop  DOPRESS  do-slide ;
        : font! dup bind fnt super font! ;
        : focus    focuscol @ @ color !   super focus   draw ;
        : defocus  defocuscol @ @ color ! super defocus draw ;
        : dpy!  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font!  THEN ;

\ new scaler                                           11dec04py

        hscaler :: lstep
        hscaler :: rstep
        hscaler :: lpage
        hscaler :: rpage
        boxchar :: handle-key?
        : slide ( x y b n -- ) drop
          nip 1 and 0= IF  2drop 0  EXIT  THEN
          nip  get nip o- - o+ swap 0 -rot DOPRESS do-slide ;
        : show  ( -- )  get nip o- 0max min o+ assign
          super show ;
class;

: SC# ( o offset -- o )  over hscaler with  offset !  endwith ;
: SC*/ ( o * / -- o )  over2 hscaler with text*/ 2! endwith ;

\ viewport                                             02jan05py
$20 Value /mslide

backing class viewport
public: 0 var org
private: cell var orgy          cell var orgx
        2 cells var steps
public: cell var sw             cell var sh
        cell var hstep          cell var vstep
        cell var minw           cell var minh
        2 cells var cliprec
        method 'hslide          method 'vslide
        method slided
        hslider ptr hspos       vslider ptr vspos
        early hslide            early vslide
        method xpos!            method ypos!

\ viewport                                             28mar99py
how:    : init  ( sx sy -- )  noback on  super init
          2dup steps 2! vstep ! hstep ! ;
        : trans ( x y -- x' y' )
          orgx @ hstep @ * x @ -
          orgy @ vstep @ * y @ - p+ ;
        : trans' ( x y -- x' y' )
          orgx @ hstep @ * x @ -
          orgy @ vstep @ * y @ - p- ;
        : <max  ( x y -- xmy y )  tuck max swap ;
        : hslide  w @ hstep @ /
          sw @ hstep @ / <max  orgx @ ;
        : vslide  h @ vstep @ /
          sh @ vstep @ / <max  orgy @ ;
        : screenpos ( -- x y )  dpy screenpos trans' ;
        gadget :: delete

\ viewport                                             11aug99py
[IFDEF] x11
        : create-pixmap ( -- )
          xwin @ IF  0 0 0 sp@ >r
                     r@ dummy r@ cell+ r> 2 cells + dummy dummy
                     dummy xwin @ xrc dpy @ XGetGeometry drop
                     * * 3 >> maxpixmap + TO maxpixmap
                     xwin @ xrc dpy @ XFreePixmap drop  THEN
          xwin off  noback @ ?EXIT
          xrc depth @
          dup h @ w @ * * 3 >> dup
          maxpixmap > IF  2drop EXIT  THEN
          maxpixmap swap - TO maxpixmap
          h @ 1 max w @ 1 max
          screen xwin @ xrc dpy @
          XCreatePixmap xwin ! ;                [THEN]

\ viewport                                             21oct99py
        : ?incbase ( b i -- )  dup 1 = IF  2drop 0 1  THEN ;
        : !steps
          child xinc ?incbase steps cell+ @ * hstep ! minw !
          child yinc ?incbase steps       @ * vstep ! minh ! ;
        : resized ( -- ) get-glues
          hglues cell+ @ sw @ max w !
          vglues cell+ @ sh @ max h !   !steps
          parent resized ( dpy resized ) ;
        : resize ( x y w h -- )  sh ! sw ! y ! x !
          hglues cell+ @ sw @ max w !
          vglues cell+ @ sh @ max h !
          & glue @ child class? 0=
          IF  0 0 w @ h @ child resize create-pixmap  xwin @
              IF    draw? push draw? off  child draw  THEN
          THEN  hslide -rot - min  vslide -rot - min org 2! ;

\ viewport                                             28mar99py

        : hglue  ( -- g )  hstep @ hglues 2@ + over -
          over minw @ - 0min + swap minw @ max swap ;
        : vglue  ( -- g )  vstep @ vglues 2@ + over -
          over minh @ - 0min + swap minh @ max swap ;

        : dpy!  bind dpy  xrc self IF  xrc dispose  THEN
          dpy xrc clone bind xrc  create-pixmap
[IFDEF] x11     get-win xrc get-gc  [THEN]
          0 clip-rect  draw? on
          self child dpy!
          child hglue 2dup hglues 2! drop
          child vglue 2dup vglues 2! drop  !steps
          2dup h ! w !  0 0 2swap child resize ;

\ viewport                                             09nov97py

        : clipx ( -- x w )   xwin @ 0=
          IF    orgx @ hstep @ * sw @
                cliprec 2@ d0= 0=
                IF  0 and cliprec    w@+ 2+ w@ p+ THEN
          ELSE  0 w @  THEN ;
        : clipy ( -- y h )  xwin @ 0=
          IF    orgy @ vstep @ * sh @
                cliprec 2@ d0= 0=
                IF  0 and cliprec 2+ w@+ 2+ w@ p+ THEN
          ELSE  0 h @  THEN ;
        : clipxywh ( -- x y w h )  cliprec 2@ d0=
          IF    xywh
          ELSE  cliprec w@+ w@+ w@+ w@
                2swap x @ y @ p+ 2swap  THEN ;

\ viewport                                             27mar99py

        : clipxy ( x y d -- x' y' )  clipxywh
          { d x y w h | x y p- swap
                      w d + min d max swap
                      h d + min d max  x y p+ } ;
        : clip  ( x y w h -- x y w h )
          >xyxy 2swap 0 clipxy 2swap -1 clipxy >xywh ;

        : !resized  super !resized  !steps ;

        : inclip? ( x y -- flag )  trans'
          -$8000 $8000 within swap
          -$8000 $8000 within and ;

\ viewport                                             17dec00py

        : draw ( -- )   clip-should @
          IF  xrect w@+ w@+ >r trans' r> w@+ w@ clip
              2swap 2drop 0= swap 0= or ?EXIT  THEN
          xwin @  IF    orgx @ hstep @ *    orgy @ vstep @ *
                        xywh 2swap
                        xwin @ dpy image
                  ELSE  child draw  THEN ;
        : assign ( widget -- )  set-child
          self child dpy!
          child hglue 2dup hglues 2! drop
          child vglue 2dup vglues 2! drop  !steps
          2dup h ! w !  0 0 2swap child resize  parent resized ;

\ viewport                                             24jan98py

        : <clip ( -- )   !txy
          clip-should @ dup 0=  IF  self nip  THEN
          dup dpy clip-should !  dpy clip-is @ =
          IF    xwin @ 0= ?EXIT  THEN   clip-should @
          IF    xrect w@+ w@+ >r trans' r> w@+ w@ clip
          ELSE  clipxywh  THEN  swap 2swap
          dpy with  clip-should @ dpy clip-is @ =
                    IF  xwin @ 0=  IF transback THEN  THEN
          endwith
          swap dpy xrect w!+ w!+ w!+ w!
          dpy xrect dpy clip-rect  clip-should @ clip-is !
          dpy clip-should @ dpy clip-is ! ;

\ viewport                                             25jul98py

        : clip> ( -- )
          dpy clip-should off  0 0 dpy txy!  xwin @ 0= ?EXIT
          0 clip-rect  clip-is off  clip-should off
          dpy clip-is off ;

        : xinc    0 hstep @ ;
        : yinc    0 vstep @ ;
        : >exposed xwin @ 0= IF  dpy >exposed  THEN ;
        : xywh ( -- x y w h )  x @ y @ sw @ sh @ ;

        : clipback ( x y w h -- x' y' w' h' )
          dpy with  xwin @ 0=
          IF  2swap trans' 2swap clip recurse
              2swap trans  2swap  THEN  endwith ;

\ viewport                                             02jan05py

        : xpos! ( p -- )  orgx @ case? 0=
          IF  orgx @ over  orgx ! -  hstep @ *
              dup abs sw @ <
              IF  clipxywh 2over 2swap clipback
                  { o x0 y0 x y w h |
                  x o 0max + y  dpy transback
                  w o abs - h x o 0min - y
                  dpy get-win  dpy image \ dpy >exposed
                  h o abs 0  y y0 - 0max
                  o 0> IF  w + o -  THEN  x x0 - 0max +
                  cliprec w!+ w!+ w!+ w! }
              ELSE  drop  THEN  draw  0. cliprec 2!  THEN ;

\ viewport                                             02jan05py

        : ypos! ( p -- )  orgy @ case? 0=
          IF  orgy @ over  orgy ! -  vstep @ *
              dup abs sh @ <
              IF  clipxywh 2over 2swap clipback
                  { o x0 y0 x y w h |
                  x y o 0max +  dpy transback
                  w h o abs - x y o 0min -
                  dpy get-win  dpy image \ dpy >exposed
                  o abs w 0    o 0> IF  h + o -  THEN
                  y y0 - 0max + x x0 - 0max
                  cliprec w!+ w!+ w!+ w! }
              ELSE  drop  THEN  draw  0. cliprec 2!  THEN ;
        : 'hslide self ['] xpos! ['] hslide toggle-state new ;
        : 'vslide self ['] ypos! ['] vslide toggle-state new ;

\ viewport                                             20oct99py
        : line ( x y x y color -- )  >r  2dup inclip?
          IF  xwin @  IF  2over 2over r@ super super line  THEN
              draw? @ IF  2over trans' 2over trans' r@
                          <clip dpy line clip>  THEN  THEN
          rdrop 2drop 2drop ;
        : text ( addr u x y color -- )  >r 2dup inclip?
          IF  xwin @  IF  2over 2over r@ super super text  THEN
              draw? @ IF  2over 2over trans' r@
                          <clip dpy text clip>  THEN  THEN
          rdrop 2drop 2drop ;
        : box ( x y w h color -- ) >r
          xwin @  IF  2over 2over r@ super super box  THEN
          draw? @ IF  2over trans' 2over clip
                      r@ <clip  dpy box  clip>  THEN
          rdrop 2drop 2drop ;

\ viewport                                             28jun98py
        : image ( x y w h x y win -- ) >r 2dup inclip?
          IF  xwin @  IF  [ 5 ] [FOR] 5 pick [NEXT] r@
                          super super image  THEN
              draw? @ IF  r@ xwin @ =
                          IF    draw
                        ELSE [ 5 ] [FOR] 5 pick [NEXT] trans' r@
                               <clip dpy image clip>  THEN THEN
          THEN  rdrop 2drop 2drop 2drop ;
        : mask ( x y w h x y win1 win2 -- ) 2over inclip?
          IF  xwin @ IF  [ 7 ] [FOR] 7 pick [NEXT]
                         super super mask  THEN
              draw? @
              IF  [ 5 ] [FOR] 7 pick [NEXT] trans' 7 pick 7 pick
                  <clip dpy mask clip>  THEN
          THEN  2drop 2drop 2drop 2drop ;

/* viewport                                            28jun98py
        : ximage ( x y w h x y win -- ) >r  2dup inclip?
          IF  xwin @  IF  [ 5 ] [FOR] 5 pick [NEXT] r@
                          super super ximage  THEN
              draw? @ IF  r@ xwin @ =
                          IF    draw
                       ELSE  [ 5 ] [FOR] 5 pick [NEXT] trans' r@
                               <clip dpy ximage clip>  THEN THEN
          THEN  rdrop 2drop 2drop 2drop ;
*/
\ viewport                                             28jun98py
        : fill ( x y addr n color -- )  >r  2over inclip?
          IF  xwin @  IF  2over 2over r@ super super fill  THEN
              draw? @ IF  2over trans' 2over r@
                          <clip dpy fill clip>  THEN
          THEN  rdrop 2drop 2drop ;
        : stroke ( x y addr n color -- )  >r  2over inclip?
          IF xwin @  IF  2over 2over r@ super super stroke  THEN
             draw? @ IF  2over trans' 2over r@
                         <clip dpy stroke clip>  THEN
          THEN  rdrop 2drop 2drop ;
        : drawer ( x y o xt -- )
          xwin @  IF  2over 2over super super drawer  THEN
          draw? @ IF  2over trans' 2over
                      <clip dpy drawer clip>  THEN
          2drop 2drop ;

\ viewport                                             09nov97py
        : inside?  ( x y -- )
          y @ - sh @ u< swap x @ - sw @ u< and ;
        : xlegal ( x -- x' )
          w @ sw @ - hstep @ / min 0max ;
        : ylegal ( y -- y' )
          h @ sh @ - vstep @ / min 0max ;
        : slided ( -- )
          hspos self  IF  hspos draw  THEN
          vspos self  IF  vspos draw  THEN ;
        : show-me  ( x y -- ) sw @ sh @
          { x y w h |  y orgy @ vstep @ * - h u>= dup
            IF  y h 2/ - vstep @ / ylegal ypos!  THEN
            x orgx @ hstep @ * - w u>= dup
            IF  x w 2/ - hstep @ / xlegal xpos!  THEN
            or IF  slided  THEN  x y trans' dpy show-me } ;

\ viewport                                             02jan05py

        : scroll  ( x y -- )  sw @ 4- sh @ 4- { x y w h |
          y orgy @ vstep @ * - dup
          0<    IF  drop y  ELSE
          h >=  IF  y h - orgy @ 1+ vstep @ * max BUT  THEN
                    vstep @ / ylegal ypos! true  ELSE false THEN
          x orgx @ hstep @ * - dup
          0<    IF  drop x  ELSE
          w >=  IF  x w - orgx @ 1+ hstep @ * max BUT  THEN
                    hstep @ / xlegal xpos! true  ELSE false THEN
          or IF  slided moved!  THEN
          x y trans' dpy scroll } ;
        : focus    child focus   ;
        : defocus  child defocus ;

\ viewport                                             22jan05py

        : ud ( -- n )  /mslide vstep @ / 1+ ;
        : clicked ( x y b n -- )
          over $18 and over 1 and 0= and
          sh @ h @ < and IF  \ scroll
             over $10 and  IF  orgy @ ud + ylegal ypos!  THEN
             over $08 and  IF  orgy @ ud - ylegal ypos!  THEN
             over $18 and  IF  slided  THEN
             2drop 2drop
          ELSE  super clicked  THEN ;

\ viewport                                             22jan05py

        : /vpage ( -- n )  sh @ vstep @ / 1- 1 max ;
        : /hpage ( -- n )  sw @ hstep @ / 1- 1 max ;
        : keyed ( key sh -- )  dup 1 and IF
          over $FF55 =  IF  orgy @ /vpage - ylegal ypos!
                            slided  2drop  EXIT  THEN
          over $FF56 =  IF  orgy @ /vpage + ylegal ypos!
                            slided  2drop  EXIT  THEN
          THEN  super keyed ;
class;

\ viewport variants                                    12nov06py

viewport class hviewport
        2 cells var yincs
how:    : vglue  ( -- g )  vglues 2@ ;
        : resized  child vglue nip 0=
          IF 0 1 ELSE child yinc THEN yincs 2! super resized ;
        : yinc   yincs 2@ ;
        : clicked ( x y b n -- )
          over $18 and over 1 and 0= and
          sw @ w @ < and IF  \ scroll
             over $10 and  IF  orgx @ ud + xlegal xpos!  THEN
             over $08 and  IF  orgx @ ud - xlegal xpos!  THEN
             over $18 and  IF  slided  THEN
             2drop 2drop
          ELSE  super super clicked  THEN ;

\ viewport variants                                    22jan05py

        : keyed ( key sh -- )  dup 1 and  IF
          over $FF55 =  IF  orgx @ /hpage - xlegal xpos!
                            slided  2drop  EXIT  THEN
          over $FF56 =  IF  orgx @ /hpage + xlegal xpos!
                            slided  2drop  EXIT  THEN
          THEN  super super keyed ;
class;
viewport class vviewport
        2 cells var xincs
how:    : hglue  ( -- g )  hglues 2@ ;
        : resized  child hglue nip 0=
          IF 0 1 ELSE child xinc THEN xincs 2! super resized ;
        : xinc   xincs 2@ ;
class;

\ Sskip                                                11nov06py
vviewport class clipper
how:    : vglue  ( -- g )  vglues 2@ ;
        : resized  super super resized ;
        : init  1 1 super init ;
        backing :: xinc
        backing :: yinc
class;

Mskip class Sskip
public: widget ptr vsize        widget ptr hsize
how:    : init ( ho vo -- )  super init
          bind vsize bind hsize ;
        : hglue  hsize hglue drop 0 ;
        : vglue  vsize vglue drop 0 ;
class;

\ Container object                                     03jan98py

vbox class sliderview           cell var glues
public: gadget ptr inner        & inner viewport asptr viewp
        static border-at
how:    0 border-at v!

\ Container object                                     08apr00py

        : init ( viewport -- ) dup bind inner
          1 hbox new rzbox
          -2  border-at @ IF  borderw c!  ELSE  borderbox  THEN
          & viewport @ inner class?
          IF  & hviewport @ viewp class?  0=
              IF  viewp 'vslide vslider0 new
              dup viewp bind vspos  2 hbox new rzbox THEN
              & vviewport @ viewp class?  0=
              IF  viewp 'hslide hslider0 new
                  dup viewp bind hspos
                  & hviewport @ viewp class?
                  0= IF  viewp vspos self  viewp hspos self
                  Sskip new  2 hbox new rzbox THEN
         2 ELSE 1 THEN ELSE 1 THEN super init ;

\ Container object                                     14sep97py

        : glue-off  0. hglues 2!  0. vglues 2!
          & viewport @ inner class?
          IF  & hviewport @ viewp class?  0=
              IF  childs hglue 2drop  childs vglue 2drop  THEN
              & vviewport @ viewp class?  0=
              IF  childs widgets hglue 2drop
                  childs widgets vglue 2drop  THEN
          THEN ;
        : glues? ( -- n )
          & vviewport @ viewp class? 0=
          IF  viewp hspos vglue drop 0= 1 and  ELSE  0  THEN
          & hviewport @ viewp class? 0=
          IF  viewp vspos hglue drop 0= 2 and  ELSE  0  THEN
          or ( dup . ) dup glues ! ;

\ Container object                                     28mar99py

        : ?portwin
          viewp hslide -rot - min  viewp vslide -rot - min
          viewp org 2!
          viewp xwin @
          IF    viewp noback @ IF  viewp draw  THEN
          ELSE  viewp noback @ 0= IF
                viewp create-pixmap
                viewp draw? dup push off
                viewp child draw  THEN  THEN ;
        : sresize ( x y w h -- x y w h )
          viewp org 2@ >r >r
          2over 2over  super resize glue-off
          r> r> viewp org 2! ;

\ Container object                                     03oct98py

        : >parent
          dpy counter @ 4 <
          IF
              1 dpy counter +!  parent resized -1 dpy counter +!
          THEN ;

\ Container object                                     28mar99py

\        : >parent  parent resized ;
        : resize ( x y w h -- )
          & viewport @ inner class?
          IF  viewp noback dup @ >r on viewp draw? off
              glues @ >r  viewp org 2@ >r >r
              2over 2over xS xywh- viewp resize glue-off
              r> r> viewp org 2!  glues? dup r@ or
              IF  >r sresize glues? r> <> IF  sresize  THEN
                  glues? r> <>  r> viewp noback ! viewp draw? on
                  IF    >parent parent draw
                  THEN  2drop 2drop  ?portwin EXIT
              THEN  drop rdrop  r> viewp noback ! viewp draw? on
          THEN  super resize ;
class;

\ Container object                                     03jan98py

sliderview class asliderview
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
        0 border-at v!

\ Container object                                     08apr00py
        : init ( viewport -- ) dup bind inner
          1 habox new rzbox
          -2  border-at @ IF  borderw c!  ELSE  borderbox  THEN
          & viewport @ inner class?
          IF  & hviewport @ viewp class?  0=
              IF  viewp 'vslide      vslider0 new
              dup viewp bind vspos 2 habox new rzbox THEN
              & vviewport @ viewp class?  0=
              IF  viewp 'hslide      hslider0 new
                  dup viewp bind hspos
                  & hviewport @ viewp class?
                  0= IF  viewp vspos self  viewp hspos self
                 Sskip new  2 habox new rzbox THEN
                  2  ELSE  1  THEN
          ELSE  1  THEN super super init ; class;

\ display simplicifacionts                             06feb00py

: D[   postpone >r ;                         immediate restrict
| : (]D   r> r> swap >r displays with  assign self  endwith ;
                                                       restrict
: ]D  -cell loffset +!  postpone (]D ;       immediate restrict

: DS[  postpone >r ;                         immediate restrict
| : (]DS  r> r> swap >r viewport with  assign self  endwith
    asliderview new ;                                  restrict
: ]DS  -cell loffset +!  postpone (]DS ;     immediate restrict

\ Container object                                     03aug98py

vabox class vresize
how:    : yinc  0 childs vglue drop ;
class;

habox class hresize
how:    : xinc  0 childs hglue drop ;
class;

\ Container object                                     21oct99py
: /glue ( glue addr -- glue' )
  >r  over swap + r@ @ min max dup r> ! 0 ;
Variable do-size
vabox class vasbox
public: cell var hsize          cell var vsize
        method oglue
how:    : vglue  ( -- glue )  super vglue  vsize /glue ;
        : vglue@ ( -- glue )  super vglue@ vsize /glue ;
        : oglue  ( -- glue )  super vglue ;
        : yinc   ( -- inc )   0 1 ;
        : resized  do-size @ IF  dpy counter @ 2 > ?EXIT  THEN
          super resized vglue 2drop ;
        : init  super init ;
class;

\ Container object                                     21oct99py



habox class hasbox
public: cell var hsize          cell var vsize
        method oglue
how:    : hglue  ( -- glue )  super hglue  hsize /glue ;
        : hglue@ ( -- glue )  super hglue@ hsize /glue ;
        : oglue  ( -- glue )  super hglue ;
        : xinc   ( -- inc )   0 1 ;
        : resized  do-size @ IF  dpy counter @ 2 > ?EXIT  THEN
          super resized hglue 2drop ;
        : init  super init ;
class;

\ vsizer                                               28mar99py
boxchar class vrtsizer
public: method drawxorline      & parent vasbox asptr vsized
        early relation?         method <size>
how:    : init ( -- )
          noop-act 0 ( rot) super init ;
        Variable vsize'
        : relation? ( -- flag )  widgets self 'nil = ;
        : hglue ( -- glue )  xN 1 *filll ;
        : vglue ( -- glue )  xN 0 ;
        : draw ( -- )  shadedbox ;
        : moved ( x y -- )  2drop
[IFDEF] x11
          relation?  IF  XC_bottom_side  ELSE  XC_top_side  THEN
 [THEN] [IFDEF] win32  IDC_SIZENS  [THEN]
          dpy set-cursor ^ dpy set-rect ;

\ vsizer                                               27mar99py

        : maxsize ( -- m )
          vsized parent with h @ vglue drop - endwith ;
        : <size> ( h -- h' )
          vsized oglue over >r + min 0max maxsize
          vsized vsize @ +  min r> max 0max ;
        : size! ( h -- ) <size> dup vsized vsize
          dup @ -rot ! = ?EXIT
          do-size on  vsized resized  do-size off ;
        : drawxorline  vsized vsize @ vsize' @ <>
          IF  vsize' @ size!  THEN ;
        : >released ( x y -- ) vsized vsize @
          relation? IF  -  ELSE  +  THEN
          DOPRESS  drawxorline  nip relation? IF  swap  THEN -
                   <size> vsize' ! drop drawxorline ;

\ vsizer                                               27mar99py
        : keyed ( key sh -- )   drop
          relation? IF  $FF57  ELSE  $FF50  THEN  case?
          IF  maxsize vsized vsize @ +
                                size!  EXIT  THEN
          relation? IF  $FF50  ELSE  $FF57  THEN  case?
          IF  0                 size!  EXIT  THEN
          relation? IF  $FF54  ELSE  $FF52  THEN  case?
          IF  vsized vsize @ vsized yinc nip + size!  EXIT  THEN
          relation? IF  $FF52  ELSE  $FF54  THEN  case?
          IF  vsized vsize @ vsized yinc nip - size!  EXIT  THEN
          drop ;
        : clicked ( x y b n -- )  2over moved  nip 1 and 0=
          IF  2drop  EXIT  THEN  vsized vsize @ vsize' !
          drawxorline  >released ( cc )
          drawxorline   vsize' @ size! ;        class;

\ hsizer                                               28mar99py
boxchar class hrtsizer
public: method drawxorline      & parent hasbox asptr hsized
        early relation?         method <size>
how:    : init ( -- )
          noop-act 0 ( rot) super init ;
        Variable hsize'
        : relation? ( -- flag )  widgets self 'nil = ;
        vrtsizer :: draw
        : hglue ( -- glue )  xN 0 ;
        : vglue ( -- glue )  xN 1 *filll ;
        : moved ( x y -- )  2drop
[IFDEF] x11
          relation?  IF  XC_right_side  ELSE  XC_left_side  THEN
 [THEN] [IFDEF] win32  IDC_SIZEWE  [THEN]
          dpy set-cursor  ^ dpy set-rect ;

\ hsizer                                               27mar99py

        : maxsize ( -- m )
          hsized parent with w @ hglue drop - endwith ;
        : <size> ( w -- w' )
          hsized oglue over >r + min 0max  maxsize
          hsized hsize @ + min r> max 0max ;
        : size! ( w -- ) <size> dup hsized hsize
          dup @ -rot ! = ?EXIT
          do-size on  hsized resized  do-size off ;
        : drawxorline  hsized hsize @ hsize' @ <>
          IF  hsize' @ size!  THEN ;
        : >released ( x y -- ) swap  hsized hsize @
          relation? IF  -  ELSE  +  THEN
          DOPRESS  drawxorline  drop relation? IF  swap  THEN -
                   <size> hsize' ! drop drawxorline ;

\ hsizer                                               27mar99py
        : keyed ( key sh -- )   drop
          relation? IF  $FF57  ELSE  $FF50  THEN  case?
          IF  maxsize hsized hsize @ +
                                size!  EXIT  THEN
          relation? IF  $FF50  ELSE  $FF57  THEN  case?
          IF  0                 size!  EXIT  THEN
          relation? IF  $FF53  ELSE  $FF51  THEN  case?
          IF  hsized hsize @ hsized yinc nip + size!  EXIT  THEN
          relation? IF  $FF51  ELSE  $FF53  THEN  case?
          IF  hsized hsize @ hsized yinc nip - size!  EXIT  THEN
          drop ;
        : clicked ( x y b n -- )  nip 1 and 0=
          IF  2drop  EXIT  THEN  hsized hsize @ hsize' !
          drawxorline  >released ( cc )
          drawxorline   hsize' @ size! ;        class;

\ vsizer                                               14nov98py

vrtsizer class vsizer
how:    : drawxorline
[IFDEF] x11    9 dpy drawable nip XSetFunction drop [THEN]
          x @ y @ vsized vsize @ vsize' @
          relation? IF  -  ELSE  swap -  THEN  -
          h @ 2/ + w @ 1 color @ 8 >> dpy box
[IFDEF] x11    3 dpy drawable nip XSetFunction drop [THEN] ;
class;

vrtsizer class vxrtsizer
how:    : <size> ( h -- )  vsized oglue + min 0max ;
class;

\ hsizer                                               14nov98py

hrtsizer class hsizer
how:    : drawxorline
[IFDEF] x11    9 dpy drawable nip XSetFunction drop [THEN]
          x @ hsized hsize @ hsize' @
          relation? IF  -  ELSE  swap -  THEN  -
          w @ 2/ + y @ 1 h @ color @ 8 >> dpy box
[IFDEF] x11    3 dpy drawable nip XSetFunction drop [THEN] ;
class;

hrtsizer class hxrtsizer
how:    : <size> ( w -- )  hsized oglue + min 0max ;
class;

\ window                                               15aug99py

displays class window
public: gadget ptr child        cell var title
        method make-window      method decoration
        gadget ptr innerwin     & innerwin viewport asptr viewp
        cell var shown          cell var closing
        cell var app
        method title!           method title+!
        method stop             method set-icon
        method set-parent
how:    : xinc  child xinc ;
        : yinc  child yinc ;
        : schedule ( xt o time -- )  dpy schedule ;
        : invoke ( -- flag )  dpy invoke ;
        : cleanup ( o -- )  dpy cleanup ;

\ window                                               10may99py

        Variable border-size

[IFDEF] x11
        Variable wm_delete_window
        : set-protocol ( -- )
          0 0" WM_DELETE_WINDOW" xrc dpy @ XInternAtom
          wm_delete_window !
          1 wm_delete_window 1 &32 4
             0 0" WM_PROTOCOLS" xrc dpy @ XInternAtom
             xwin @ xrc dpy @ XChangeProperty drop ;
        :noname  event XClientMessageEvent data @
          wm_delete_window @ =  IF  close  THEN ;
        ClientMessage cells Handlers + !

\ window transient subclassing                         13nov99py

        : set-parent ( win -- )
          xwin @ xrc dpy @ XSetTransientForHint drop ;

\ window                                               16aug98py
        Create WMhints sizeof XWMHints allot
        Create hints   sizeof XSizeHints allot
        : set-hint ( -- )  1 WMhints XWMHints input !
          NormalState WMhints XWMhints initial_state !
          [ InputHint StateHint or ] Literal
          WMhints XWMHints flags !
          WMhints xwin @ xrc dpy @ XSetWMHints drop ;
        : set-icon ( o -- )
          icon-pixmap with 0 0 draw-at endwith
          >r >r 2drop 2drop 2drop r> r>
          WMhints XWMHints icon_pixmap !
          WMhints XWMHints icon_mask   !
          [ IconPixmapHint IconMaskHint or ] Literal
          WMhints XWMHints flags !
          WMhints xwin @ xrc dpy @ XSetWMHints drop ;

\ window                                               19dec04py

        : set-xswa ( -- )  0 xrc color 3 xrc color
                    xswa XSetWindowAttributes background_pixel !
                    xswa XSetWindowAttributes border_pixel !
          backing-mode xswa XSetWindowAttributes backing_store !
        NorthWestGravity xswa XSetWindowAttributes bit_gravity !
        NorthWestGravity xswa XSetWindowAttributes win_gravity !
          None     xswa XSetWindowAttributes background_pixmap !
          None      xswa XSetWindowAttributes border_pixmap !
          xrc cmap @   xswa XSetWindowAttributes colormap !
          event-mask   xswa XSetWindowAttributes event_mask ! ;

\ window                                               28oct06py
        : set-hints  shown @ 0= ?EXIT  x @ y @ d0= 0= 5 and
          $178  or hints XSizeHints flags !
          yinc  xinc rot swap
                hints XSizeHints width_inc 2!
                hints XSizeHints base_width 2!
          hglue 2dup + w @ min 2 pick max
                hints XSizeHints width !
          over  hints XSizeHints min_width !
          +     hints XSizeHints max_width !
          vglue 2dup + h @ min 2 pick max
                hints XSizeHints height !
          over  hints XSizeHints min_height !
          +     hints XSizeHints max_height !
          y @ x @ hints XSizeHints x 2!
          hints xwin @ xrc dpy @ XSetWMNormalHints drop ;

\ window                                               23jan07py

        : make-window ( n -- )  >r  set-xswa
          xswa  xswavals r> or  xrc get-visual 0 swap
          border-size @ border-size off
          h @ 1 max w @ 1 max 0 0 dpy xwin @ xrc dpy @
          XCreateWindow xwin !   set-protocol set-hint
          xwin @ xrc get-ic ;
[THEN]

\ window                                               28jul07py
[IFDEF] win32
        : make-window ( n -- )   >r  x @ y @ d0=
          IF  $80000000 dup x ! y !  THEN
          0 xrc inst @ 0  r@ $80000000 and
          0= IF    0        y @ x @ h @ w @
                   WS_OVERLAPPEDWINDOW
             ELSE  owner @  y @ x @ h @ w @
                   WS_POPUP border-size @ border-size off
                   IF  WS_BORDER or  THEN
             THEN  dup style !
          popupW minosW
          r> $7FFFFFFF and  CreateWindowExW xwin !
          0 0 0 0 sp@ xwin @ GetWindowRect drop x ! y ! 2drop
          app-win @ 0= IF  xwin @ app-win !  THEN ;
        : set-icon  ( o -- ) drop ;

\ window                                               13nov99py

        : set-parent ( win -- ) dup  owner !
          xwin @ SetParent drop ;
[THEN]
        : init ( dpy -- )   bind dpy  self dpy append
          dpy xrc clone bind xrc
          0 make-window  xwin @ xrc get-gc  0 set-font
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase
          title off ;
        : ?app  app @   IF  app @ wake pause  app off  THEN ;

\ window                                               22sep07py

        : dispose ( -- )
          child self  drop child dispose  self cleanup
          title $off
          xwin @  IF
[IFDEF] x11           xrc ic @ dup IF  XDestroyIC  THEN  drop
                      xwin @ xrc dpy @ XDestroyWindow drop
[THEN]
[IFDEF] win32         xwin @ DestroyWindow drop
                      dpy handle-event
[THEN]    THEN
          self dpy delete ?app super dispose ;

\ window                                               09aug04py

[IFDEF] x11
        : show   ( -- )  child show
          h @ w @ d0= IF  xywh resize THEN
          shown @  shown on  set-hints  \ dpy sync
          IF  x @ y @ d0=
              IF    h @ w @ xwin @ xrc dpy @ XResizeWindow drop
              ELSE  h @ w @ y @ x @ xwin @ xrc dpy @
                    XMoveResizeWindow drop  THEN  dpy sync  THEN
          xwin @ xrc dpy @ XMapRaised drop ;
[THEN]

\ window                                               13nov99py

[IFDEF] win32
        : show   ( -- )  child show
          h @ w @ d0= IF  xywh resize THEN
          shown on   SWP_NOZORDER SWP_SHOWWINDOW or
          owner @ IF  SWP_NOACTIVATE or  THEN
          h @ w @ 0 0 sp@ >r 0 style @ r>
          AdjustWindowRect drop p-
          y @ x @
          owner @ IF  HWND_TOPMOST  ELSE  HWND_TOP  THEN
          xwin @ SetWindowPos drop ;
[THEN]

\ window                                               01nov06py

        : hide ( -- )  shown off  child hide \ ?app
[IFDEF] x11
          xwin @ xrc dpy @ XUnmapWindow drop  [THEN]
[IFDEF] win32
          SW_HIDE xwin @ ShowWindow drop  [THEN] ;
        : stop  up@ app !  F stop ;
        : delete ( addr addr' -- )  over self =
          IF    nextwin self swap ! drop
          ELSE  drop link nextwin  nextwin goto delete  THEN ;
        : append ( o before -- )  nextwin self over =
          IF    swap bind nextwin  nextwin bind nextwin
                parent self nextwin bind parent
          ELSE  nextwin goto append  THEN ;

\ window                                               22sep07py
        : decoration ( o -- o' )
          & viewport @ innerwin class?
          IF  sliderview new  THEN ;
        : focus  [IFDEF] x11
          xrc ic @ dup IF  >r
                           0 xwin @ XNFocusWindow
                           xwin @ XNClientWindow r@
                           XSetICValues_2 drop
                           r> XSetICFocus
          THEN  drop  [THEN]
          child focus   ;
        : defocus
          child defocus ;

\ window                                               25jan03py

[IFDEF] x11
        : get-event ( mask -- )  dpy get-event  flush-queue ;
        : handle-event ( -- )
          event XAnyEvent window @ xwin @ =
          event XAnyEvent type @
          dup FocusIn = swap FocusOut = or
          IF    event XEnterWindowEvent subwindow @ xwin @ = or
          THEN
          IF \ cr ." sending event " event @ . ." to win "
             \ base @ event XAnyEvent window @ hex . base !
             event @ cells Handlers + perform
             ( ."  done " ) EXIT  THEN
          nextwin goto handle-event ;
[THEN]

\ window                                               29jul07py
[IFDEF] win32
        : .event base push hex cell+ @+ swap 4 .r
          @+ swap 5 .r @+ swap 9 .r @+ swap 9 .r @+ @ swap
          5 .r 5 .r cr ;
        : get-event ( mask -- )  drop
          BEGIN  PM_REMOVE 0 0 xwin @ event PeekMessageW
                 WHILE  handle-event  REPEAT
          size-event ;
        : handle-event ( -- )  \ event .event
          event TranslateMessage drop  maxascii $80 =
          IF    event DispatchMessageW drop
          ELSE  event DispatchMessage drop  THEN
          pause ;
[THEN]

\ window                                               25jan03py

        : !resized  xrc !font
          0 set-font  child !resized resized ;
        : geometry ( w h -- ) { gw gh |
          1 counter ! rw off rh off
          x @ y @ xinc gw * + yinc gh * + resize
          0 counter ! rw on  rh on
          x @ y @ xinc gw * + yinc gh * + resize }
[IFDEF] win32  output push display ." "  [THEN] ;
        : geometry? ( -- w h )
          w @ xinc >r - r> /
          h @ yinc >r - r> / ;
        : draw ( -- ) \ base push hex xwin @ . ." : w-draw "
          clip-should off  clip-is off
          0 clip-rect  child draw ;

\ window                                               05oct07py
[IFDEF] x11
        Create 'textprop 0 , 0 , 8 , 1 ,
        : !title ( -- )  0 title $@ + c!
          0" MINOS" title $@ drop sp@
          xwin @ xrc dpy @ XSetClassHint drop 2drop
          XA_STRING title @ cell+ 'textprop 2!
          title @ @ 'textprop 3 cells + !
          0 0" _NET_WM_NAME" xrc dpy @ XInternAtom
          'textprop xwin @ xrc dpy @ XSetTextProperty drop
          0 0" _NET_WM_ICON_NAME" xrc dpy @ XInternAtom
          'textprop xwin @ xrc dpy @ XSetTextProperty drop
          title @ cell+ xwin @ xrc dpy @ XStoreName drop
          title @ cell+ xwin @ xrc dpy @ XSetIconName drop ;
        : title!  ( addr u -- ) title $!  !title ;
        : title+! ( addr u -- ) title $+! !title ;  [THEN]

\ window                                               29jul07py

[IFDEF] win32
        : !title ( -- )  title $@ >utf16 drop
          xwin @ SetWindowTextW drop ;
        : title!  ( addr u -- ) title $!  !title ;
        : title+! ( addr u -- ) title $+! !title ;
        : screenpos ( -- x y )
          0 0 0 0 sp@ xwin @ GetWindowRect drop 2swap 2drop
          h @ w @ 0 0
          sp@ >r 0 style @ r> AdjustWindowRect drop 2swap 2drop
          p- swap ;
        : mxy! ( mx my -- ) 2dup super mxy!
          screenpos p+ screen mxy! ;
[THEN]

\ window                                               17dec00py
        : assign ( widget addr n -- )
          child self IF  child dispose  THEN  title!
          dup bind innerwin  decoration  bind child
          self child dpy!  self child bind parent ;
        : adjust-inc ( n off inc -- n' )
          >r tuck - r@ 2/ + r@ / r> * + ;
        : min-max ( n glue -- n' ) over + >r umax r> umin ;
        : child-size? ( -- x y )  child xywh 2swap 2drop  2dup
          yinc adjust-inc vglue min-max h !
          xinc adjust-inc hglue min-max w ! ;
        : child-resize ( -- )
          BEGIN  0 0 w @ h @ 2dup 2>r child resize
                 2r> child-size? 2over w @ h @ d= >r
                 d= r> and  UNTIL ;

\ window                                               19oct99py
[IFDEF] x11
        : re-size ( -- )
          rw @ rh @ w @ h @ d= 0= IF
              h @ w @ xwin @ xrc dpy @ XResizeWindow drop
          THEN ;
[THEN]
[IFDEF] win32
        : re-size ( -- )
          rw @ rh @ w @ h @ d= 0= IF
              1 h @ w @ 0 0
              sp@ >r 0 style @ r> AdjustWindowRect drop
              2dup >r >r p- screenpos swap r> r> p+
              xwin @ MoveWindow drop
          THEN ;
[THEN]

\ window                                               07jan07py

        : (resized ( -- )
          child-resize  child-moved
\          rw @ rh @  child-size?  d= 0=  IF  draw  THEN
          set-hints dpy sync  re-size ;
        : close  ( -- )  closing push closing @ closing on
          IF    hide ['] dispose self &10 after schedule
          ELSE  innerwin close  THEN ;

\ window                                               15jul01py

        : repos ( x y -- )   2dup y ! x !
[IFDEF] x11   set-hints
          swap xwin @ xrc dpy @ XMoveWindow drop sync ; [THEN]
[IFDEF] win32
          >r >r 0 h @ w @ r> r> swap
          xwin @ MoveWindow drop ;  [THEN]
        : resized  ( -- )  (resized counter @ ?EXIT  draw ;
        : child-moved ( -- )  pointed self
          IF  mx @ my @ pointed xywh >r >r
              p- r> r> rot swap u< -rot u< and
              IF  & backing @ pointed class?
                  IF mx @ my @ pointed moved THEN  EXIT  THEN
              pointed leave  0 bind pointed  THEN
          child self  IF  mx @ my @  child moved  THEN ;

\ window                                               28mar99py
        : resize ( x y w h -- )
          h ! w ! 2drop (resized ;
        : mouse ( -- x y b ) mx @ my @ mb @ ;
        : clicked  ( x y b n -- )  child clicked ;
        : hglue ( -- glue ) child hglue ;
        : vglue ( -- glue ) child vglue ;
        : keyed ( key -- )  dup 8 and
          IF  over $FF51 =  2 pick $FF53 = or
              & vviewport @ innerwin class? not and
              IF  viewp hspos keyed  EXIT  THEN
              over $FF52 =  2 pick $FF54 = or
              & hviewport @ innerwin class? not and
              IF  viewp vspos keyed  EXIT  THEN
          THEN  innerwin keyed ;
class;

\ menu-entry                                           05jan07py

actor uptr menu-call

'& Value menu-sep
button class menu-entry
how:    \ init ( act addr len -- )
        2 colors focuscol !     3 colors defocuscol !
        : clicked ( x y b n -- ) dup 0= IF 2drop 2drop EXIT THEN
          >released drop
          dpy hide callback self F bind menu-call ;
        : keyed ( key sh -- )  drop  dup bl = swap #cr = or
          IF  x @ y @  1 2 clicked  THEN ;
        : focus  super focus color   focuscol chcol +push draw ;
        : defocus color defocuscol chcol -push draw ;

\ menu-entry                                           12dec99py

        : hglue  text $@ menu-sep scan nip
          IF    0 text menu-sep :[ fnt size drop 1 *fil
                               2 pick parent with
                                   dup >r 1- combined tab@ p+
                                   r> combined tab!
                               endwith  1+ ]: $iter
                1- parent with combined tab@ endwith
                xM xS + 1+ 0 p+
          ELSE  textwh @ xM + xS + 1+ 1 *fil  THEN ;
class;

\ event handler for sub-window                         30aug05py
window class window-stub
how:    : init ( widget win -- )  xwin !  title off
          dup bind innerwin bind child
          child with dpy self endwith bind dpy
          self dpy with dpy append endwith
          dpy xrc clone bind xrc
          xwin @ xrc get-gc  0 set-font
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase ;
        : resize-win ( -- )  h @ w @ y @ x @ or or or 0= ?EXIT
[IFDEF] win32  SWP_NOZORDER SWP_SHOWWINDOW or  [THEN]
          h @ w @ y @ x @
[IFDEF] win32  owner @ IF  HWND_TOPMOST  ELSE  0  THEN
          xwin @ SetWindowPos  [THEN]
[IFDEF] x11    xwin @ xrc dpy @ XMoveResizeWindow  [THEN] drop ;

\ event handler for sub-window                         20nov07py
        : show ( -- )  resize-win
[IFDEF] win32  SWP_SHOWWINDOW xwin @ ShowWindow drop [THEN]
[IFDEF] x11    xwin @ xrc dpy @ XMapWindow drop  [THEN] ;
        : dispose-it ( -- )  self cleanup
          self dpy get-dpy with dpy delete endwith
          title $off
          xrc dispose gadget :: dispose ;
        : dispose ( -- )
[IFDEF] win32  xwin @ IF  xwin @ DestroyWindow drop  THEN
          ['] dispose-it  self &20 after schedule ;  [THEN]
[IFDEF] x11  dispose-it ;  [THEN]
        : resize  h ! w ! y ! x !  resize-win ;

\ event handler for sub-window                         30aug05py

        : moved!  dpy moved! ;
        : moved?  dpy moved? ;
        : click~  dpy click~ ;
        : moreclicks dpy moreclicks ;
        : mxy!    transclick dpy mxy! ;
        : keyed   dpy keyed ;
        : transclick ( x y -- x' y' ) x @ y @ p+ ;
class;

\ window without border                                12dec99py
[IFDEF] win32   Variable owner-win  [THEN]
window class frame
public: cell var map?           method set-dpys
        method grab             method ungrab
        method handle [IFDEF] win32  displays ptr ?grab  [THEN]
how:    : make-window  ( attrib -- )
[IFDEF] x11  mouse_cursor xrc cursor
          xswa XSetWindowAttributes cursor !
          1 xswa XSetWindowAttributes override_redirect !
          1 xswa XSetWindowAttributes save_under !
          CWSaveUnder or CWOverrideRedirect or CWCursor or
[THEN]
[IFDEF] win32  owner-win @ owner ! owner-win off  $80000000 or
          WS_EX_TOPMOST or WS_EX_TOOLWINDOW or  [THEN]
          super make-window ;

\ frame                                                08aug04py

        : handle ( -- flag )
          -1 -1 0 0 child clicked  true
          BEGIN  click? 0=
                 IF  moved?
                     IF   mouse drop child inside?
                          mouse 0 child clicked tuck <>
                          IF dup IF   child focus
                                 ELSE child defocus  THEN THEN
                     THEN  dpy xrc fid &30 idle
                 ELSE  click 2over child inside? dup >r
                       IF    child clicked
                       ELSE  hide 2drop 2drop
                       THEN  drop r>
                 THEN  map? @ 0=  UNTIL ;

\ frame                                                09mar07py

[IFDEF] x11
        Variable grab-win       grab-win on
        : Xgrab ( win -- )  grab-win @ map? ! grab-win !
          CurrentTime None dup GrabModeAsync GrabModeAsync
          [ ButtonPressMask ButtonReleaseMask PointerMotionMask
            or or ] Literal
          0 grab-win @ xrc dpy @ XGrabPointer drop
          CurrentTime RevertToParent
          grab-win @ xrc dpy @ XSetInputFocus drop ;
        : grab  xwin @ Xgrab ;
        : ungrab ( -- )  map? @ dup grab-win !
          dup -1 <>  IF  Xgrab map? off  EXIT  THEN drop
          CurrentTime xrc dpy @ XUngrabPointer drop map? off ;
[THEN]

\ frame                                                27jun02py
[IFDEF] win32
        : Wgrab ( win -- ) dup re-time  grab-key self bind ?grab
          SetCapture dup 0= or map? !  ^ F bind grab-key ;
        : grab ( -- )  xwin @ Wgrab ;
        : ungrab ( -- )  map? @
          IF    ?grab self  F bind grab-key  0 bind ?grab
                map? @ -1 <>  IF  map? @ grab  ?grab self
                   F bind grab-key  0 bind ?grab  THEN  map? off
          ELSE  ReleaseCapture drop  app-win @ re-time  THEN ;
 [THEN] : dispose ( -- )
          title $off
[IFDEF] x11  xwin @ IF xwin @ xrc dpy @ XDestroyWindow drop THEN
 [THEN] [IFDEF] win32
          xwin @  IF  xwin @ DestroyWindow drop  THEN
 [THEN]   self dpy delete  displays :: dispose ;

\ window without border                                29aug98py

        : show ( x y -- )
          y ! x !  shown on  super show ;
        : set-dpys ( widget -- )  recursive
          BEGIN  dup 0<> over 'nil <> and  WHILE  ^ swap >o
                 widget bind dpy   widget widgets self
                 & combined @ class?
                 IF    combined childs self o> set-dpys
                 ELSE  o>  THEN
          REPEAT  drop ;
class;

\ tool tips                                            16jul00py

frame class frame-tip
public: displays ptr owner-dpy
how:    : make-window ( n -- )  1 border-size !
          super make-window ;
        : init ( owner dpy -- )  super init  bind owner-dpy ;
        : keyed  owner-dpy keyed ;
class;

\ tool tips                                            27jun02py

minos

&1000 Value tooltip-delay

actor class tooltip
public: widget ptr tip          actor ptr feed
        frame-tip ptr tip-frame early show-tip
how:    : init ( actor tip -- )  bind tip  bind feed
          feed called self  set-called ;
        : dispose leave  tip dispose super dispose ;
        : leave  ^ screen cleanup  tip-frame self 0= ?EXIT
          tip-frame hide  tip-frame dispose 0 bind tip-frame ;

\ tool tips                                            07nov99py
        : show-tip ( -- )
[IFDEF] win32  caller with widget dpy get-dpy endwith
               displays with xwin @ endwith owner-win ! [THEN]
          caller with widget dpy pointed self ^ =
              IF   0 widget dpy set-rect  THEN  endwith
          caller xywh  & hbox @ caller parent class?
          IF  nip 0 swap  ELSE  drop 0  THEN  p+
          caller self widget with xN endwith dup p+
          caller self widget with dpy screenpos endwith p+
[IFDEF] x11  caller with widget dpy get-win endwith  [THEN]
          tip self caller self widget with dpy self endwith
          screen self frame-tip new dup bind tip-frame
          frame-tip with s" tooltip" assign
              [IFDEF] x11  set-parent  [THEN]  show focus
          endwith ;

\ tool tips                                            21sep07py

        : enter  [IFDEF] x11  leave  [THEN]
          [IFDEF] win32  tip-frame self ?EXIT  [THEN]
          ['] show-tip ^ tooltip-delay after screen schedule ;
        : toggle leave feed toggle ;
        : fetch  leave feed fetch ;
        : store  leave feed store ;
        : set-called  dup super set-called feed set-called ;
class;

: TT[  ;                                         immediate
: ]TT  tooltip postpone new ;                    immediate
: TT-string  text-label new tooltip new ;
: TT"  postpone X" postpone TT-string ;          immediate

\ menu-frame                                           09mar07py

frame class menu-frame
public: early popup
how:    : assign ( widget -- ) child self IF child dispose THEN
          dup bind child   bind innerwin
          self child dpy!  self child bind parent
          resized ;
        : screenpos ( -- x y )  x @ y @ ;
        : hide  ( -- )  super hide  ungrab ;
        : show ( x y -- ) super show  grab ;
        : keyed ( key sh -- )
          over #esc =  IF  2drop drop 0 hide EXIT  THEN
          super keyed ;

\ menu-frame                                           05mar07py

        : submenu-vpos { x y w h w' h' | ( --> x y )
          x y h + dup h' + screen h @ >  IF  h' - h - 0max  THEN
          swap screen w @ w' - min 0max swap } ;
        : submenu-hpos { x y w h w' h' | ( --> x y )
          x w + y screen h @ h' - min 0max
          swap dup w' + screen w @ >  IF  w' - w - 0max  THEN
          swap } ;

\ menu-frame                                           09mar07py
        : popup ( [xwin] child -- flag )  >r
[IFDEF] win32+  dpy get-dpy displays with xwin @ endwith
                owner-win !   [THEN]
          r@ widget with dpy self endwith
          dpy screenpos  xywh  >r >r p+ r> r>
          & hbox @ parent class?
          r> screen self new  with  assign  defocus
             >r  ( !resized ) 0 0 0 0 resize
             child with w @ h @ endwith
             r>  IF  submenu-vpos  ELSE  submenu-hpos  THEN
             >r rot [IFDEF] x11 set-parent [ELSE] drop [THEN] r>
             show  focus   handle  swap child dpy!
             dispose  endwith ;
class;

\ menu title                                           05mar07py
menu-entry class menu-title
        method menu-action
public: widget ptr callw
how:    0 colors focuscol !     1 colors defocuscol !
        : init  ( widget addr len -- )
          noop-act -rot super init bind callw ;
        : dispose callw dispose super dispose ;
        : menu-action  menu-call called self
          0= IF  dpy self menu-call set-called  THEN
          menu-call toggle ;
        : >released ( x y b n -- ) 2drop 2drop
          1 color 2+ c!  draw
          dpy get-win callw self menu-frame popup
          0=   IF    callback self F bind menu-call
               ELSE  dpy focus  THEN    0 color 2+ c!  draw ;

\ menu title                                           21apr01py

        : clicked  ( x y b n -- )
          dup 0= IF  2drop 2drop  EXIT  THEN
          >released  callw hide  menu-action ;
        : !resized  super !resized ( callw !resized ) ;
class;

\ sub-menu                                             27dec99py

menu-title class sub-menu
how:    \ : init ( widget addr u -- )  super init ;
        : menu-action ( -- )
          menu-call self callback self <> IF  dpy hide  THEN ;
class;

' noop alias M:                                 immediate

\ info-menu                                            27dec99py
hbox class info-menu
public: textfield ptr text      tributton ptr tri
        text-label ptr info     gadget ptr callw
how:    : assign ( addr u -- ) text assign ;
        : get ( -- addr u ) text get ;
        : text!  ( addr u -- ) info assign ;
        : menu-action  menu-call self 0= ?EXIT
          menu-call called self
          0= IF  dpy self menu-call set-called  THEN
          menu-call toggle ;
        : keyed ( key sh -- )
          over bl =  IF  tri keyed  ELSE  text keyed  THEN ;
        gadget :: prev-active
        gadget :: next-active
        gadget :: first-active

\ info-menu                                            02dec00py
        : init  ( widget addr len -- )
          text-label new bind info  bind callw
          0 ST[ ]ST callw self combined with childs get endwith
                    textfield new dup bind text
          0 text edit ds !
            ^ M[ clicked ]M :down tributton new bind tri
            info self 1 habox new hfixbox  text self
            ^ S[ ]S :[ text childs vglue ]: :[ xS 0 ]: arule new
               tri self
            ^ S[ ]S :[ text childs vglue ]: :[ xS 0 ]: arule new
            3 vbox new hfixbox 2 hbox new
            ^ S[ ]S :[ callw hglue ]: :[ 0 0 ]: arule new
          2 vbox new  +fill 3 super init drop ;
        : dpy!  dup callw dpy!  super dpy! ;
        : !resized  super !resized  callw !resized ;

\ info-menu                                            05mar07py

        : >released ( x y b n -- ) 2drop 2drop
          :up tri assign tri draw  0 F bind menu-call
          dpy get-win
          callw self text with menu-frame popup endwith
          0=   IF callback self F bind menu-call THEN
          :down tri assign tri draw ;
        : clicked  ( x y b n -- ) \ first-active
          dup 0= IF  2drop 2drop  EXIT  THEN
          >released  menu-action ;
        : dispose  callw dispose  super dispose ;
        boxchar :: handle-key?
class;

\ window with menu                                     27dec99py

window class menu-window
how:    : decoration ( menu widget -- )
          super decoration 2 vbox new ;
class;

\ component                                            04mar00py

s" COMPONENT" pad place  bl pad count + c!
' vabox pad context @ (find drop (name> !
: get-win ( -- win )  & displays @ object class?
  IF  displays get-win  ELSE  widget dpy get-win  THEN ;
: new-component ( o od addr u -- o )
  >r >r  1 swap modal new  r> r>
  screen self window new  window with  assign ^  endwith ;
: open-component    ( o od addr u -- )
  new-component  window with  show  endwith ;
: open-dialog       ( o od addr u -- )
  new-component  get-win
  swap window with  set-parent show  endwith ;
: open-application  ( o od addr u -- )
  new-component  window with  show up@ app !  endwith ;

\ OpenGL canvas                                        22jun02py

opengl also glconst also

[IFDEF] win32
        | Create pfd  sizeof PIXELFORMATDESCRIPTOR w, 1 w,
          0 ( PFD_DRAW_TO_WINDOW or ) PFD_DRAW_TO_BITMAP or
          PFD_SUPPORT_OPENGL or \ PFD_SUPPORT_GDI or
          ( PFD_DOUBLEBUFFER or ) ,
          PFD_TYPE_RGBA c,
          &24 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c,
          0 c, 0 c, 0 c, 0 c, &32 c, 0 c, 0 c,
          PFD_MAIN_PLANE c, 0 c, 0 , 0 , 0 ,
        | Create bih sizeof BITMAPINFOHEADER ,
          0 , 0 , 1 w, &24 w, BI_RGB , 0 , 0 , 0 , 0 , 0 ,
[THEN]

\ OpenGL canvas                                        15aug99py

0 Value canvas-mode

glue class glcanvas
public: defer drawer            method render
        cell var visinfo        cell var pixmap
        cell var ctx            cell var glxpm
        cell var glxwin         cell var rendered
        window-stub ptr stub    cell var shown
[IFDEF] win32
        cell var oldbm          cell var dibptr
[THEN] [IFDEF] x11
        cell var cmap
[THEN]
        widget ptr outer

\ OpenGL canvas                                        08jul00py
how:
[IFDEF] x11
        | Create attrib GLX_DOUBLEBUFFER ,
                        GLX_RGBA ,
                        GLX_RED_SIZE   ,   1 ,
                        GLX_GREEN_SIZE ,   1 ,
                        GLX_BLUE_SIZE  ,   1 ,
                        GLX_DEPTH_SIZE ,  $10 ,  0 ,
        : init  ( exec actor w w+ h h+ -- )
          super init  >callback  IS drawer  bind outer ;
        : dpy!  super dpy!
          dpy xrc with dpy @ screen @ endwith
          attrib canvas-mode 1 and cells +
          glXChooseVisual visinfo !
          dpy xrc dpy @ visinfo @ 0 1 glXCreateContext ctx ! ;

\ OpenGL canvas                                        09dec07py
        : destroy-pixmap ( -- ) dpy xrc dpy @
          glxwin @ ?dup  IF  over XDestroyWindow drop
                             glxwin off  THEN
          glxpm  @ ?dup  IF  over swap glXDestroyGLXPixmap
                             glxpm  off  THEN
          pixmap @ ?dup  IF  over XFreePixmap drop
                             pixmap off  THEN
          cmap   @ ?dup  IF  over XFreeColormap drop
                             cmap   off  THEN  drop ;
        : set-context ( -- )
          dpy xrc dpy @ glxpm @ glxwin @ or
          ctx @ glXMakeCurrent drop ;
        : dpyscreen ( -- dpy screen )
          dpy xrc dpy @ visinfo @ XVisualInfo screen @ ;

\ OpenGL canvas                                        09jan00py
        : new-window   xswa sizeof XSetWindowAttributes erase
          AllocNone visinfo @ XVisualInfo visual @
          dup dpy xrc vis @ <> canvas-mode 4 and or
          IF    dpy drawable rot drop XCreateColormap dup cmap !
          ELSE  2drop dpy xrc cmap @  THEN
              xswa XSetWindowAttributes colormap !
          dpyscreen BlackPixel dup
              xswa XSetWindowAttributes border_pixel !
              xswa XSetWindowAttributes background_pixel !
          event-mask  xswa XSetWindowAttributes event_mask !
          xswa glxvals   visinfo @ XVisualInfo visual @
          InputOutput    visinfo @ XVisualInfo depth  @ 0
          h @ 1 max w @ 1 max y @ x @
          dpy get-win dpy xrc dpy @  XCreateWindow
          self over window-stub new bind stub ;

\ OpenGL canvas                                        09dec07py

        : new-pixmap ( -- )  glxwin @ ?EXIT  glxpm @ ?EXIT
          dpy xwin @ dpy get-win = canvas-mode 2 and 0= and  IF
              new-window glxwin ! rendered off  EXIT THEN
          visinfo @ XVisualInfo depth @
          h @ 4 max w @ 4 max 3 + -4 and
          dpy get-win dpy xrc dpy @ XCreatePixmap dup pixmap !
          dpy xrc dpy @ visinfo @ rot glxCreateGLXPixmap
          glxpm ! rendered off ;
        : show ( -- )  shown @ shown on ?EXIT
          new-pixmap stub self 0= ?EXIT
          xywh stub resize stub show ;
        : hide ( -- )  shown @ shown off 0= ?EXIT
          stub self 0= ?EXIT  stub hide ;
[THEN]

\ OpenGL canvas                                        23sep99py
[IFDEF] win32
        : set-context ctx @ pixmap @ wglMakeCurrent ?err ;
        : add-dib-section  h @ 1 max w @ 1 max  bih cell+ 2!
          0 0 0 DIB_RGB_COLORS bih
          pixmap @ CreateDIBSection dup ?err glxpm !
          glxpm @ pixmap @ SelectObject dup ?err oldbm !
          pfd dup pixmap @ ChoosePixelFormat dup ?err
          pixmap @ SetPixelFormat ?err ;
        : new-pixmap ( -- )  0 0 wglMakeCurrent drop
          screen xrc dc @ CreateCompatibleDC dup ?err pixmap !
          add-dib-section
          pixmap @ wglCreateContext dup ?err ctx !
          rendered off ;
        : init  ( exec actor w w+ h h+ -- )
          super init  >callback  IS drawer  bind outer ;

\ OpenGL canvas                                        01nov06py

        : destroy-pixmap ( -- )
          ctx    @ ?dup  IF  0 0 wglMakeCurrent drop
                             wglDeleteContext drop ctx off THEN
          pixmap @ ?dup  IF  DeleteObject drop pixmap off THEN
          glxpm  @ ?dup  IF  DeleteObject drop glxpm  off THEN ;

[THEN]

\ OpenGL canvas                                        09dec07py

        : resize ( x y w h -- )
          super resize rendered off
[IFDEF] win32
          oldbm @ pixmap @ SelectObject ?err
          glxpm  @ ?dup  IF  DeleteObject drop glxpm  off THEN
          add-dib-section
[ELSE]    glxpm  @   IF  destroy-pixmap  THEN  new-pixmap
          stub self  IF  xywh stub resize  stub show  THEN
[THEN]  ;
        : dispose destroy-pixmap  [IFDEF] x11
          ctx @ ?dup  IF
              dpy xrc dpy @ swap glXDestroyContext  THEN
[THEN]    stub self IF  stub dispose  THEN  glFlush
          super dispose ;

\ OpenGL canvas                                        08dec07py

        : render ( -- ) \ ." render "
          pixmap @ glxwin @ or 0= IF  new-pixmap  THEN
          set-context ^ drawer  glFlush
[IFDEF] x11
          glxpm @
          IF  dpy xrc dpy @ glxpm @ glXSwapBuffers  THEN
[THEN]    rendered on ;

\ OpenGL canvas                                        22oct06py

        : draw ( -- )
          rendered @ 0=  IF  render  THEN
[IFDEF] x11   pixmap @
          IF    0 0 xywh 2swap pixmap @ dpy image
          ELSE  glxwin @
            IF  dpy xrc dpy @ glxwin @ glXSwapBuffers
                rendered off  THEN
          THEN
[THEN]
[IFDEF] x11_ximage   0 0 xywh 2swap 0 sp@ >r 0 sp@ r>
          pixmap @ dpy xrc dpy @ XMesaFindBuffer
          XMesaGetBackBuffer drop nip dpy ximage  [THEN]
[IFDEF] win32   0 0 xywh 2swap pixmap @ dpy image  [THEN]
        ;

\ OpenGL canvas                                        04aug05py

        boxchar :: clicked ( x y b n -- )
        boxchar :: keyed ( key sh -- )
        : moved ( x y -- ) 2drop  stub self
          IF    mouse_cursor stub set-cursor  ^ stub set-rect
          ELSE  mouse_cursor dpy  set-cursor  ^ dpy  set-rect
          THEN  callback enter ;
        boxchar :: leave ( -- )
class;

\ canvas                                               11jul99py

previous previous

: CV[  postpone :[ canvas postpone with ;        immediate
: ]CV  canvas postpone endwith  postpone ]: ;    immediate
: GL[  postpone :[ glcanvas postpone with ;        immediate
: ]GL  glcanvas postpone endwith  postpone ]: ;    immediate

\ helper words for Theseus                             21sep07py

: T"   postpone S" ;                             immediate

: ^^bind  postpone dup  postpone bind ;      immediate restrict
/*
| Create ^^bin-buf $100 allot
: ^^bin ( -- )
  ^^bin-buf off ^^bin-buf $sum !
  s" dup ^^ with bind " $add
  bl word count $add s"  endwith" $add
  ^^bin-buf count evaluate ; immediate
*/
\ IO-Window                                            26oct99py

: scan8 ( addr u -- addr u' )  2dup bounds
  ?DO  I c@ $80 and IF  drop I over - LEAVE  THEN  LOOP ;
: scan16 ( addr u -- addr' u' )  bounds  scratch 0 2swap
  ?DO  I c@ $80 and 0= ?LEAVE
       2dup + I c@ $7F and 8 << I 1+ c@ or
       swap w! 2+  2 +LOOP ;

\ IO-Window                                            12mar00py

0 Value do-scroll

boxchar class terminal
public: cell var cols           cell var rows
        cell var color          cell var cursor#
        cell var pos            cell var selw
        cell var keys           cell var start
        cell var scrolls        cell var typebuf
        cell var maxrows        cell var minrows
        cell var addr           cell var u
        1 var resize!           1 var flush!
        2 var text-color
        cell var sizew          font ptr fnt16
        & dpy viewport asptr vdpy

\ IO-Window                                            24oct99py

        method type             method page
        method emit             method flush
        method decode           method clrline
        method cr               method c
        method atxy?            method drawcur
        method at?              method at
        method curoff           method curon
        method key?             method key
        method 'start           method 'line
        method scrollup         method scrollback
        method paste-selection
        early showtext          early curpos
        early .text

\ IO-Window                                            06feb00py

how:    6 colors focuscol !     1 colors defocuscol !
        : assign ( w h -- )  1 max rows ! 1 max cols !
          rows @ maxrows !  rows @ minrows !
          typebuf  HandleOff
          start    HandleOff
          cols @ cell+ typebuf Handle!  typebuf @ off
          rows @ 1+ cols @ * start 2dup Handle! @ swap bl fill
          1 selw !  dpy self IF  resized  THEN ;

\ IO-Window                                            05jan07py
        : 'start ( -- addr ) start @
          scrolls @ cols @ * + ;
        : 'line  ( n -- addr u )
          scrolls @ cols @ * dup >r + rows @ cols @ * mod r> -
          'start + cols @ -trailing ;
        : !resized  s" M" !textwh
          4 dpy xrc font@ bind fnt16 ;
        : !tile  0 scrolls @ texth @ * negate dpy txy! ;
        : focus    focuscol   @ @ dup 8 >> swap $FF and 8 << or
                                  color !  drawcur super focus ;
        : defocus  defocuscol @ @ color !  drawcur ;
        : dpy! ( dpy -- )  widget :: dpy!
          fnt   self 0= IF  1 dpy xrc font@ font!       THEN
          fnt16 self 0= IF  4 dpy xrc font@ bind fnt16  THEN ;

\ mixed font output                                    24oct99py

        : .texts ( addr u x y dpy -- )
          fnt16 self 0= IF  fnt draw  EXIT  THEN  { x y dpy |
          BEGIN  dup  WHILE  2dup scan8 dup
                 IF    tuck x y dpy fnt draw
                       dup textwh @ * x + to x /string
                 ELSE  2drop  THEN
                 2dup scan16 dup
                 IF    tuck x y dpy fnt16 draw
                       dup textwh @ * x + to x /string
                 ELSE  2drop  THEN  REPEAT  2drop } ;

\ mixed font output                                    16jan05py

        : font-color! ( c dpy -- )
          over fnt color !  displays with  set-color  endwith ;
        : display-texts ( x y dpy -- )
          >r text-color @ r@ font-color!
          addr @ u @ 2swap r> .texts ;
        : .text ( addr u x y c -- )
          text-color ! 2swap u ! addr !
          ^ ['] display-texts dpy drawer ;

\ mixed font output                                    05may07py

        : expand16 ( -- )  maxascii $80 = IF
             selw @ pos @ 'line drop
             dup 1+ xchar- tuck - negate pos +!
             dup xchar+ swap - max 1 max selw !  EXIT
          THEN  fnt16 self 0= ?EXIT
          pos @ 1- 0max 'line drop c@ $80 and
          IF  -1 pos +!  1 selw +!  THEN
          pos @ selw @ 1- + 0max 'line drop c@ $80 and
          IF  1 selw +!  THEN ;
        : csize ( s i -- size )
          dup >r - 0max r> 'line rot 2dup swap - 0max >r
          min x-width r> +
          textwh @ * ;

\ IO-Window                                            20oct06py
        : drawcur  dpy self 0= ?EXIT  !tile  expand16
          cursor# @  IF  6 colors @  ELSE  color @  THEN
          pos @ typebuf @ @ +
          dup selw @ + 2dup min -rot max { color s e |
          x @ y @ cols @ rows @ * 0
          ?DO  s I - cols @ u<  e I - cols @ u< or
               I s e within or
               IF over s I csize dup >r + over r>
                  w @ e I csize min swap - 1 max
                  texth @ color dpy box
                  I 'line e I - 0max min s I - 0max
                  /string  2over swap s I csize +
                  swap color 8 >> .text
               THEN  texth @ + cols @ +LOOP
          2drop } ;

\ IO-Window                                            16jun02py

        : draw-io ( x y dpyo -- )
          dup displays with clipy endwith
          over + { dpyo sclip eclip |
          cols @ rows @ * 0
          ?DO  dup sclip eclip within
               IF  2dup w @ texth @
                   6 colors @ dpyo displays with box endwith
                   I 'line 2over 6 colors @ 8 >>
                   dpyo font-color!
                   dpyo .texts
               THEN  texth @ + cols @ +LOOP  2drop } ;
        : draw ( -- )  !tile
          x @ y @ ^ ['] draw-io dpy drawer
          drawcur  0 0 dpy txy! ;

\ IO-Window                                            12mar00py

        : resize-it2 ( -- )
          0 resize! c!  sizew off  parent resized  show-you ;
        : resize-it ( -- )
          vdpy sw @ cols @ textwh @ * min sizew !
          parent resized  dpy set-hints
          ['] resize-it2 ^ /step @ after dpy schedule ;
        : screen-resize
          start rows @ $20 + -$20 and
          cols @ * SetHandleSize
          resize! c@ ?EXIT  1 resize! c!
          ['] resize-it ^ /step @ after dpy schedule ;

        : xinc ( -- o inc ) sizew @ textwh @ ;
        : yinc ( -- o inc ) 0       texth @ ;

\ IO-Window                                            12mar00py

        : redraw-it ( -- )  0 resize! c!  draw ;
        : screen-redraw
          resize! c@ ?EXIT  1 resize! c!
          ['] redraw-it ^ /step @ after dpy schedule ;

\ IO-Window                                            12mar00py
        : scrollup ( -- )  rows @ maxrows @ <
          IF  1 rows +!   screen-resize
              cols @ rows @ 1- * 'line drop cols @ bl fill
              EXIT  THEN
          scrolls @ 1+ rows @ mod scrolls !
          cols @ dup negate pos +!
          cols @ rows @ 1- * 'line drop swap bl fill  do-scroll
          IF    x @ y @ texth @ dup >r +  dpy transback
                w @ h @ r> - x @ y @
                dpy get-win dpy image  !tile
                x @ y @ texth @ rows @ 1- * +
                w @ texth @ 6 colors @ dpy box
                dpy >exposed  0 0 dpy txy!
          ELSE  screen-redraw  THEN ;
        : scrollback ( n -- ) rows @ max maxrows ! ;

\ IO-Window                                            16jan05py
        : showtext ( addr u1 u2 -- )
          resize! c@ IF  drop 2drop  EXIT  THEN
          !tile drop cols @ >r  x @ y @
          at? drop 0 swap texth @ * p+
          2dup textwh 2@ r> * swap 6 colors @ dpy box
     pos @ at? nip - 'line 2swap 6 colors @ 8 >> .text 2drop ;
        : linetype ( addr u -- )
          tuck pos @ 'line drop swap 2dup -trailing >r drop move
          >r pos @ r@ + cols @ rows @ * >=
          IF  scrollup  THEN
          pos @ 'line drop r> r> over >r showtext  r> pos +! ;
        : vglue  rows @ texth @ *  0 ;
        : hglue  cols @ textwh @ *  0 ;
        : ?flush ( -- ) flush! c@ ?EXIT  1 flush! c!
          ['] flush ^ /step @ after dpy schedule ;

\ IO-Window                                            06jan05py
        : win-type  ( addr len -- )  cols @ >r
          BEGIN  dup pos @ r@ mod r@ - + dup 0>=  WHILE
                 tuck - >r over r@ + swap rot r> linetype REPEAT
          drop linetype rdrop  ;
        : type  ( addr len -- )  typebuf @ @ over + cols @ >=
          IF   flush curoff win-type curon
          ELSE ?flush tuck typebuf @ @+ + swap move typebuf @ +!
          THEN ;
        : emit  ( char -- )  char$ type ;
        : flush ( -- )  0 flush! c!  typebuf @ @
          IF  typebuf @ @+ swap
              curoff typebuf @ off  win-type  curon  THEN ;
        : moved ( x y -- )  2drop  ^ dpy set-rect
[IFDEF] x11            XC_xterm   [THEN]
[IFDEF] win32          IDC_IBEAM  [THEN]  dpy set-cursor  ;

\ IO-Window                                            12mar00py
        : page  ( -- )  flush curoff  pos off  typebuf @ off
          scrolls off  minrows @ rows !  screen-resize
          'start cols @ rows @ * bl fill  curon draw ;
        : at ( r c -- )  flush 0max cols @ 1- min
          swap 0max rows @ 1- min
          cols @ * + curoff pos ! curon ;
        : at? ( -- r c )
          pos @ typebuf @ @ + cols @ /mod swap ;
        : show-you ( -- ) dpy self 0= ?EXIT
          at? textwh 2@ rot * -rot * x @ y @ p+ dpy show-me ;
        : ?sel-scroll  ( c r -- c r )
          over textwh @ * over texth @ *
          x @ y @ p+ dpy scroll ;
        : curpos ( -- x y )
          at? textwh @ * swap 1+ texth @ * ;

\ IO-Window                                            24oct99py

        : at-sel ( r c -- )
          0max cols @ 1- min
          swap 0max rows @ 1- min  ?sel-scroll
          cols @ * + pos @ - cursor# @ pos @ selw @
          { s1 c# p s |
          s s1 xor 0<
          IF  1 cursor# ! drawcur      p       s1      0
          ELSE  s1 0max s 0max <  IF p s1 +  s s1 -  1  ELSE
                s1 0max s 0max >  IF p s +   s1 s -  0  ELSE
                s1 0min s 0min <  IF p s1 +  s s1 -  0  ELSE
                s1 0min s 0min >  IF p s +   s1 s -  1  ELSE
                p 0 1  THEN THEN THEN THEN
          THEN  cursor# ! selw ! pos ! drawcur
          c# cursor# ! p pos ! s1 selw ! } ;

\ IO-Window                                            30dec99py
        : clrline   flush  curoff pos @ dup cols @ mod - pos !
          pos @ 'line drop cols @
          2dup -trailing >r drop 2dup bl fill
          r> showtext curon ;
        : curon  ( -- )  -1 cursor# +!  cursor# @ 0> ?EXIT
          1 selw ! drawcur show-you  cursor# off ;
        : curoff ( -- )  cursor# @  1 cursor# +!  0> ?EXIT
          drawcur 0 selw !  1 cursor# ! ;
        : c  ( n -- )  flush  curoff  pos @ + 0max  pos !
          BEGIN pos @ cols @ rows @ * >= WHILE  scrollup  REPEAT
          curon ;
        : cr  ( -- ) flush cols @ pos @ over mod - c
          resize! c@ ?EXIT show-you ;
        : curup    cols @ negate c ;
        : curdown  cols @        c ;

\ IO-Window                                            09mar99py
        : selecting ( -- )  flush  textwh 2@ swap
          DOPRESS  x @ y @ p- 2swap swap >r / swap r> / at-sel ;
        : (dpy  [IFDEF] x11    dpy get-win  dpy xrc dpy @
          [ELSE] 0 0 [THEN] ;
        : mark-selection ( x y -- )  defocus  at? >r >r
          swap at pos @ >r selecting
          -select selw @ pos @ + r>
          2dup max -rot min  0 -rot
          ?DO  drop cols @ I over mod -
               I 'line ( drop over -trailing ) I' I - min  tuck
               +select  over I' I - min  <> swap  +LOOP
          IF  s" " +select  THEN  (dpy !select
          curoff  r> r> at focus  curon ;
        : paste-selection ( addr u -- )
          bounds ?DO  I xc@+ 0 keyed pause  I - +LOOP ;

\ IO-Window                                            21aug99py

        : readline  >r >r at? drop cols @ * 'line
          r@ swap 4 pick min dup 3 pin move
          r> over r> min ;
        : >atxy  ( msap xy -- msap )  at? >r >r $100 /mod swap
          2dup at r> rot <>
          IF  >r readline r> rdrop over >r  THEN  r>
          - + dup 0min dup negate c - 2 pick over -
          0min dup c + ;

\ IO-Window                                            07jun03py
        : keyed ( key state -- )
          over shift-keys?  IF  2drop  EXIT  THEN
          BEGIN  keys @ @ $1F >=  WHILE  pause  REPEAT $10 << or
          keys @ dup @ 1+ $1F min dup keys @ ! cells + ! ;
        boxchar :: handle-key?
        : key?  ( -- flag )
          keys @ @ 0= IF  pause  THEN  keys @ @ 0> ;
        : getkey ( -- key )  keys @ @
          IF    keys @ cell+ @  keys @ 8+ dup cell- $78 move
                -1 keys @ +! $10000 /mod kbshift !
          ELSE  0  THEN ;
        : key   ( -- key )  flush  1 cursor# ! curon
          BEGIN  key?  0= WHILE
                 dpy xrc fid &50 idle  REPEAT
          getkey curoff ;

\ IO-Window                                            06jan05py

        : decode ( m s addr pos char -- m s addr pos flag )
          kbshift @ $100 and  IF  >atxy  0 EXIT  THEN
[IFDEF] (Ftast  dup $FFBE $FFCA within
          IF  $FFBE - cells (Ftast + -rot >r >r -rot >r >r
              perform r> r> r> r> prompt cr save-cursor
              over 3 pick type row over at 0 EXIT THEN
 [THEN]   $FF51 case?  IF  ctrl B  THEN
          $FF52 case?  IF  ctrl P  THEN
          $FF53 case?  IF  ctrl F  THEN
          $FF54 case?  IF  ctrl N  THEN
          dup $007F = IF  drop ctrl D  THEN
          dup $FF00 and $FF00 =  IF  drop 0 EXIT  THEN
[IFDEF] utf-8  xdecode [ELSE] PCdecode [THEN] ;

\ IO-Window                                            01jan05py

        : init ( w h -- )  $80 keys Handle!  keys @ off
        ^ CK[ 2swap y @ -
              texth @ / swap x @ - textwh @ / swap 2swap
              1 and  IF  drop mark-selection  EXIT  THEN
              1 and 0=  IF  2drop (dpy @select paste-selection
                            EXIT  THEN
              8 << or kbshift @ $100 or keyed ]CK >callback
          assign  defocuscol @ @ color ! ;
        : close  S" ^Jbye"  bounds ?DO  i c@ 0 keyed  LOOP ;
        : dispose start HandleOff  keys HandleOff
          typebuf HandleOff  ^ dpy cleanup  super dispose ;
class;

\ Window IO words                                      10apr04py
terminal uptr term      Forward openw
| : term?  term self 0= IF  openw  THEN ;
: WINtype  ( addr l -- )  term? term type pause ;
: WINemit  ( char -- )    term? term emit ;
: WINflush ( -- )         term? term flush ;
: WINcr    ( -- )         term? term cr pause ;
: WINpage  ( -- )         term? term page ;
: WINat    ( rol col -- ) term? term at  ;
: WINat?   ( -- row col ) term? term at? ;
: WINform  ( -- rs cs )   term? term rows @ term cols @ ;
: WINcuron    ( -- )      term? term curon ;
: WINcuroff   ( -- )      term? term curoff ;
: WINcurleft  ( -- )      term? -1 term c ;
: WINcurrite  ( -- )      term?  1 term c ;
: WINclrline  ( -- )      term? term clrline ;

\ Window IO words                                      05jan05py

Output: WINdisplay
        WINemit true WINcr WINtype PCdel WINpage
        WINat WINat? WINform  noop noop WINflush
        WINcuron WINcuroff WINcurleft WINcurrite WINclrline [

: WINkey? ( -- flag )  term? term key? ;
: WINkey  ( -- key )   term? term key ;
: WINdecode            term? term decode ;
[IFDEF] xaccept
        Input:  WINkeyboard
        WINkey WINkey? WINdecode xaccept false [  [THEN]
[ELSE]  Input:  WINkeyboard
        WINkey WINkey? WINdecode PCaccept false [  [THEN]
: WINi/o  WINdisplay  WINkeyboard ;

\ openw                                                10apr04py

2Variable map-size              PCform swap map-size 2!
2Variable map-pos
&1000 Value MaxScroll
hbox uptr term-menu             rule uptr term-last
Defer terminal-menu             ' noop IS terminal-menu

forward term-w

minos

\ openw                                                21jun05py

: openw ( -- )  screen self menu-window new
  menu-window with
      term-w set-icon
      0 1 *fill 0 1 *fil rule new dup F bind term-last
   1 hbox new vfixbox dup F bind term-menu 1 vbox new
      map-size 2@ 1 1 viewport new
          D[ terminal new dup F bind term ]D
      s" bigFORTH Dialog" assign
      terminal-menu
      map-size 2@ geometry  show endwith
  MaxScroll term scrollback
  event-task task's term dup @
  0= IF  term self swap !  ELSE  drop  THEN
  ['] WINi/o IS standardi/o  WINi/o ;

\ terminal menu operations                             10apr04py

: add-menu ( menu -- )  term-last self term-menu add ;
: add-help ( menu -- )  'nil           term-menu add ;
: hide-menu ( -- )  term-menu parent self
  vbox with -flip endwith ;
: show-menu ( -- )  term-menu parent self
  vbox with +flip endwith ;

: send-string ( addr u -- )
  bounds ?DO  i c@ 0 displays keyed  LOOP ;

\ terminal menu operations                             10apr04py

actor class key-actor
public: cell var string
how:    : init ( o addr u -- )  string $!  super init ;
        : fetch ( -- n ) 0 ;
        : store ( n -- ) string $@
          ['] send-string called send drop ;
class;

: key"  state @
  IF    postpone ^  postpone S" key-actor postpone new
  ELSE  ^ '" parse key-actor new  THEN ;        immediate

\ : term-dpy  term dpy dpy self ;

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