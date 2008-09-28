\ Color system                                         06jan05py

: CellArray Create  DOES>  swap cells + ;
CellArray colors
       $0001 , $0203 ,  \ focus and defocus for buttons
       $0405 , $0607 ,  \ focus and defocus for labels
       $0809 , $0A0B ,  \ border colors
       $0C0D , $0E0F ,  \ color for texts&cursor
       $0D0D , $0D0D ,  \ revers text&cursor colors

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
#90 Value contrast
-1 Value twoborders
: set-col ( r g b -- ) to blue to green to red ;
: grayish ( -- )   $C0 $C0 $C0 set-col #92 to contrast ;
: redish  ( -- )   $CE $43 $10 set-col #90 to contrast ;
: bluish  ( -- )   $37 $CE $FC set-col #90 to contrast ;
: bisquish ( -- )  $E6 $D8 $A3 set-col #90 to contrast ;

grayish

\ Color system - RGB table                             31mar99py

: +contrast ( n -- )  #100 swap 0 ?DO  contrast #100 */  LOOP ;
: -contrast ( n -- )  #100 swap 0 ?DO  #100 contrast */  LOOP ;
: cx ( n -- ) dup 0< IF negate -contrast ELSE +contrast THEN
  $FF min ;
Create graytable   $-10 cx c,  -3 cx c,  -1 cx c,
                      0 cx c,   7 cx c,   6 cx c,   0 c,
: re-gray  6 7 0 -1 -3 $-10
  6 0 DO  cx graytable I + c!  LOOP ;
: rgb ( r g b -- rgb )  8 << or 8 << or ;
: gray  ( n -- rgb )  graytable + c@ >r
  r@ red #100 */ $FF min  r@ green #100 */ $FF min
  r> blue #100 */ $FF min  rgb ;

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
       here $F0 cells dup allot erase
: !rgbs ( -- )  dp push  Colortable $10 cells + dp !
  rgbs DO i >rgb , LOOP ; \ standard colors

\ Color system - RGB table  X11 version                09aug99py
[defined] x11 [IF]
Create color  sizeof XColor allot
       DoRed DoGreen DoBlue or or color XColor flags c!
: get-rgbs ( cmap dpy array end start -- )
  ?DO  Colortable I cells + c@+ c@+ c@
       cfix -rot  cfix swap  cfix
       color XColor red  w!+ w!+ w!
       2dup swap color XAllocColor drop
       color @ over ! cell+  LOOP  drop 2drop ;
: get-color ( cmap dpy array size -- )  0 get-rgbs ;
: get-stcolor ( cmap dpy array -- )  !rgbs  rgbs get-rgbs ;
Create syscolors 6 c, 2 c, 6 c, 3 c, 6 c, 2 c, 6 c, 3 c,
                 1 c, 5 c, 3 c, 6 c, 6 c, 0 c, 4 c, 3 c,
: (get-sys-colors ( -- )   re-gray
  $10 0 DO  I syscolors + c@ gray Colortable I cells + ! LOOP ;

\ Color system - TrueColor                             05jun06py
$FFFF Value alpha
$FF0000 Value red##
$00FF00 Value green##
$0000FF Value blue##
: >subc ( 16bcol mask -- color )
  swap dup $10 << or  alpha dup $10 << or um* nip 1+
  over um* nip 1+ and ;
: make-rgbs ( vis array end start -- )
    3 pick
    dup Visual red_mask @   to red##
    dup Visual green_mask @ to green##
        Visual blue_mask @  to blue##
  ?DO  Colortable I cells + c@+ c@+ c@
       cfix -rot  cfix swap  cfix
       red##   >subc -rot
       green## >subc -rot
       blue##  >subc or or
       2 pick Visual red_mask @+ @+ @ or or tuck and
       alpha rot invert >subc or
       over ! cell+  LOOP  2drop ;
: make-color ( vis array size -- )  0 make-rgbs ;
: make-stcolor ( vis array -- )  !rgbs  rgbs make-rgbs ;

\ X timer correction                                   23apr06py

#060 Value sameclick
#150 Value twoclicks
#6 Value samepos
#31 Value XA_STRING
#31 Value XA_STRING8
[defined] VFXFORTH [IF]
extern: int gettimeofday ( void * , void * );
Create timeval 0 , 0 ,
Create timezone 0 , 0 ,
: XTime ( -- time )  timeval timezone gettimeofday
  timeval 2@ #1000 * swap #1000 / + ;
[ELSE]
also dos
: XTime ( -- time )  timeval timezone gettimeofday
  timeval 2@ #1000 * swap #1000 / + ;
previous
[THEN]
: get-td ( win dpy -- n ) { win dpy }
  dpy win #16 #31 8 0 S" round delay trip"
  XChangeProperty drop
  dpy win PropertyChangeMask scratch XWindowEvent
  XTime scratch XPropertyEvent time @ - ;

\ X timer correction                                   07jan07py

: get-tds ( win dpy count -- min max )
  over 3 pick scratch XGetWindowAttributes drop
  PropertyChangeMask
  scratch XSetWindowAttributes event_mask dup @ >r !
  over 3 pick CWEventMask scratch XChangeWindowAttributes drop
  $7FFFFFFF $80000000 rot 0
  ?DO  2over get-td tuck max -rot min swap  LOOP 2swap
  r> scratch XSetWindowAttributes event_mask !
  swap CWEventMask scratch XChangeWindowAttributes drop ;

NotUseful ( WhenMapped ) Value backing-mode

[THEN]

\ Color system - RGB table  win32 version              24oct99py
[defined] win32 [IF]

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

#060 Value sameclick
#150 Value twoclicks
#6 Value samepos
Variable emulate-3button
$001100A6 Value :srcand         $00440328 Value :srcor

: ?err  ( flag -- )
  0= IF  base push hex  GetLastError ?dup
         IF  cr 9 u.r  THEN  THEN ;
[THEN]

\ keyboard handling                                    22aug99py
[defined] x11 [IF]
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
[defined] win32 [IF]   : shift-keys? ( key -- flag )  drop false ;
IDC_ARROW         Value mouse_cursor            [THEN]
2Variable txy   0 0 txy 2!      Variable spot
Patch get-sys-colors    ' (get-sys-colors IS get-sys-colors

\ polygon draw utilities                               22jun02py

Variable (poly'
Variable (poly#
[defined] win32 [IF]   Variable (poly''  [THEN]
2Variable (lastp

: <poly ( x y -- x y )  (poly# off (poly' @ 0=
  IF  $1000 NewPtr (poly' !
      [defined] win32 [IF]  $2008 NewPtr (poly'' !  (poly' @ off [THEN]
  THEN   2dup (lastp 2! ;
: poly' ( -- addr )  (poly' @ (poly# @ cells + ;
: poly, ( dx dy -- )  2dup (lastp 2@ p+ (lastp 2!
  1 (poly# +!   swap poly' w!+ w! ;
: poly# ( x y -- )  (lastp 2@ p- poly, ;
: poly> ( -- addr n )  (poly' @ (poly# @ 1+ ;

\ bezier path                                          22jun02py

[defined] VFXFORTH [IF]
    include vfx-minos/splines.fs
[ELSE]
1 loadfrom splines.fb
[THEN]

Variable (bezier#

: <bezier ( -- )  (poly# @ (bezier# ! ;
: bezier> ( n -- )  (lastp 2@ poly# >r
  (poly# @ (bezier# @ - >r  (lastp 2@
  r@ 0 ?DO  2dup -1 (poly# +! poly' wx@+ wx@ p-  LOOP
  2drop r> r> >bezier >r 2dup (lastp 2!
  r> 0 ?DO  poly#  LOOP ;

\ flags                                                26jun08py

$0 Constant #hidden
$1 Constant #exposed
$2 Constant #moved
$3 Constant #pending
$4 Constant #draw

\ gadget                                               06feb00py

0 Value ^^
: ^>^^ ( -- )  r> & ^^ push  ^ to ^^ >r ;

debugging class gadget
public: cell var x              cell var y
        cell var w              cell var h
        cell var flags
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
how:    #40 /step V!            4 colors shadowcol !
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
        : show flags #hidden -bit ;
        : hide flags #hidden +bit ;
        : resized  parent resized ;
class;

\ Event data structure                                 09mar99py
[defined] x11 [IF]
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
[defined] x11 [defined] VFXFORTH 0= and [IF]
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

: .Xerror" ( -- )  output push display
  err-dpy @ err-event XErrorEvent error_code c@ strerrbuf $80
  XGetErrorText drop strerrbuf >len
  ." X Error: " type cr  err-dpy off ;
: .Xerror ( -- )  screen-sync  err-dpy @ 0= ?EXIT  .Xerror" ;
: .Xerror~~ screen-sync  err-dpy @ 0= IF  2drop 2drop EXIT  THEN
  (~~) .Xerror" ;
: x~~  ~~, postpone .Xerror~~ ; immediate
[ELSE]
    Defer screen-sync
    Defer screen-ic!
[THEN]

\ Event data structure                                 29jul07py
[defined] win32 [IF]
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
[defined] x11 [IF]   Defer new-font16  [THEN]

\ XResource                                            03jul07py

[defined] x11 [IF]
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
          $100 cells NewHandle colarray !
           $10 cells NewHandle fontarray !
          XC_num_glyphs cells NewHandle cursors !
          cursors @ @ XC_num_glyphs cells erase
          scratch 0= IF  scratch# & scratch Handle!  THEN ;
        : dispose ( -- )  clone? @ 0=
          IF  colarray  @ DisposHandle
              fontarray @ DisposHandle
              cursors   @ DisposHandle  THEN
          super dispose ;
        : close ( -- )  dpy @ XCloseDisplay drop ;
	: color ( i --  n )
	    $FF and cells colarray @ @ + @ ;
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
        : best-visual ( -- vis depth screen ) 0 { vdepth }
          0 0 sp@ >r dpy @ 0 pad r> XGetVisualInfo
          swap sizeof XVisualInfo * bounds
	  4 0 DO  TrueColor I vis# + c@ dup to vdepth
		  8 = + 1+ StaticGray DO
                  2dup ?DO  I XVisualInfo depth @ vdepth =
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
	  dpy @ dup screen @ RootWindow vis @ AllocNone
	  XCreateColormap ;
        : tmp-win ( -- win )
          dpy @ XFlush  dpy @ 0 XSync
          cmap @ xswa XSetWindowAttributes colormap !
          0      xswa XSetWindowAttributes background_pixel !
          1      xswa XSetWindowAttributes border_pixel !
	  dpy @ dup screen @ RootWindow  0 0 #100 dup
	  2 depth @ InputOutput vis @
	  CWBackPixel CWBorderPixel or CWColormap or
	  xswa XCreateWindow

          dpy @ XFlush  dpy @ 0 XSync ;

\ XResource                                            22oct07py

        : best-im ( -- im )  dpy @ 0 0 0 XOpenIM dup xim !
          IF  0 0 sp@ >r xim @ XNQueryInputStyle r> 0
              XGetIMValues_1
              drop dup >r w@+ 2+ @ swap cells bounds ?DO
                  I @ dup XIMPreedit and 0<>
                  swap XIMStatusNothing and
                  0<> and IF  drop I @ LEAVE  THEN
              cell +LOOP dup 0= IF ." No style found" cr  THEN
              r> XFree drop ELSE 0 THEN  ;

\ XResource                                            27jan07py

        : get-ic ( win -- ) xim @ 0= IF  drop  EXIT  THEN
          ic @ IF
             >r ic @ XNFocusWindow r> 0 XSetICValues_1 drop
             EXIT  THEN
	  0 XNFontSet fontset @ XNSpotLocation spot 0
	  XVaCreateNestedList_2 { win list }
	  xim @ XNInputStyle im @ XNPreeditAttributes list
	  XNFocusWindow win ( XNClientWindow r@ ) 0
	  XCreateIC_3 dup ic !
	  screen-ic!
	  list XFree drop
	  ic @ ?dup IF  XSetICFocus drop  THEN ;

\ XResource                                            22mar03py

        Create xgc  sizeof XGCValues allot
        : connect ( -- )
	  [defined] X-error [IF]
	      ['] X-error XSetErrorHandler drop
	  [THEN]
          dpy @ DefaultScreen          screen !
          dpy @ screen @ DefaultColormap cmap !
          dpy @ screen @ DefaultGC         gc !
          best-visual over dpy @ screen @ DefaultDepth <>
          IF    screen ! depth ! vis !  best-cmap cmap !
	        dpy @ tmp-win dup >r 0 xgc XCreateGC gc !
                dpy @ r> XDestroyWindow drop
          ELSE  drop depth ! dpy @ screen @ DefaultVisual vis !
                xswavals xswavalvis xor to xswavals drop
          THEN  best-im im !
          dpy @ #38 0 XKeycodeToKeysym drop ;

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
          0 sp@ >r 0 sp@ >r 0 sp@ >r dpy @ r> r> swap r> r> swap 2swap
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
	  dpy @ cmap @ colarray @ @ #rgbs @ 0 XFreeColors drop ;
        : cursor ( n -- shape )
          dup cells cursors @ @ + @ dup IF  nip EXIT  THEN
          drop dpy @ over XCreateFontCursor tuck swap cells
          cursors @ @ + ! ;

\ XResource                                            27jun02py
        : set-color ( index -- )
\          dup cur-color @ = IF  drop  EXIT  THEN
          dup cur-color !
                    [ xgc XGCValues foreground ] ALiteral !
          FillSolid [ xgc XGCValues fill_style ] ALiteral !
	  dpy @ gc @ [ GCForeground GCFillStyle or ] Literal xgc
	  XChangeGC drop ;
        : set-function ( n -- )
          dpy @ gc @ rot XSetFunction drop ;
        : get-gc ( win -- )
          1 depth @ #24 min << 1-
                    [ xgc XGCValues background ] ALiteral !
	  dpy @ swap GCBackground xgc XCreateGC gc ! !font ;
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
[defined] win32 [IF]
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
          $100 cells NewHandle colarray !
          $100 cells NewHandle penarray !
          $100 cells NewHandle rgbarray !
           $10 cells NewHandle fontarray !
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
          rgbarray @ @ dup $10 get-rgb   $10 cells + get-strgb
          colarray @ @ dup $10 get-brush $10 cells + get-stbrush
          penarray @ @ dup $10 get-pen   $10 cells + get-stpen ;
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

: nextpow2 ( n -- x^2 )  1  BEGIN  2* 2dup <  UNTIL  nip ;

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

[defined] x11 [IF]
Variable own-selection

: post-selection ( addr n win dpy -- ) { win dpy }
  2dup dpy -rot XStoreBytes drop
  >r >r
  dpy dpy DefaultRootWindow 9 #31 ( XA_STRING ) 8 PropModeReplace
  r> r> XChangeProperty drop
  dpy 1 win event-time @ XSetSelectionOwner drop
  own-selection on ;

\ selection                                            23apr06py
Variable got-selection          Variable str-selection
[defined] Forward [IF]
    Forward screen-event
[ELSE]
    Defer screen-event
[THEN]
: wait-for-select ( -- flag )  got-selection off
  #5000 after  BEGIN  screen-event
           timeout? got-selection @ or UNTIL  drop ;
: fetch-property ( prop win dpy -- ) { prop win dpy }
  prop 0=  IF  str-selection @ IF  got-selection on
                                   str-selection off  EXIT  THEN
	       str-selection on dpy
	       1 XA_STRING8 1 win CurrentTime
	       XConvertSelection drop  EXIT  THEN
  0 sp@ >r 0 sp@ >r 0 sp@ >r 0 sp@ >r 0 sp@ >r
  dpy win prop 0 $10000 1 AnyPropertyType
  r> r> r> r> r>
  XGetWindowProperty 0=
  IF  2drop nip over >r +select r> XFree drop
  ELSE  drop 2drop 2drop  THEN  got-selection on ;

\ selection                                            23apr06py

: @select ( win dpy -- addr n )
  own-selection @
  IF    2drop
  ELSE  >r  selection HandleOff
        r@ 1 XGetSelectionOwner 0=
        IF    0 sp@ r> swap XFetchBytes tuck swap +select XFree 2drop
	ELSE  r> swap >r
	      1 XA_STRING 1 r> event-time @
              XConvertSelection drop
              wait-for-select
        THEN
  THEN  (@select ;
[THEN]

\ selection                                            28jul07py
[defined] win32 [IF]
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

#30 Value minwait

0 Value event-task

