\ Xft fonts

Module xft

also dos

legacy off

library libXft libXft.so.2

libXft XftFontMatch int int int int (int) XftFontMatch ( dpy screen pattern result -- pattern )
libXft XftFontOpenPattern int int (int) XftFontOpenPattern ( dpy pattern -- font )
libXft XftFontOpenXlfd int int int (int) XftFontOpenXlfd ( dpy screen xlfd -- font )
libXft XftXlfdParse int int int (int) XftXlfdParse ( xlfd igf compf -- pattern )
libXft XftTextExtents8 int int int int int (void) XftTextExtents8 ( dpy font string len extents -- )
libXft XftTextExtentsUtf8 int int int int int (void) XftTextExtentsUtf8 ( dpy font string len extents -- )
libXft XftDrawCreate int int int int (int) XftDrawCreate ( dpy drawable visual colormap -- draw )
libXft XftDrawCreateAlpha int int int (int) XftDrawCreateAlpha ( dpy drawable depth -- draw )
libXft XftDrawChange int int (void) XftDrawChange ( draw drawable -- )
libXft XftDrawDestroy int (void) XftDrawDestroy ( draw -- )
libXft XftDrawString8 int int int int int int int (void) XftDrawString8 ( d color font x y addr u -- )
libXft XftDrawString16 int int int int int int int (void) XftDrawString16 ( d color font x y addr u -- )
libXft XftDrawString32 int int int int int int int (void) XftDrawString32 ( d color font x y addr u -- )
libXft XftDrawStringUtf8 int int int int int int int (void) XftDrawStringUtf8 ( d color font x y addr u -- )
libXft XftDrawRect int int int int int int (void) XftDrawRect ( d color x y w h -- )
libXft XftColorAllocValue int int int int int (int) XftColorAllocValue ( d v cmap color result -- )
libXft XftColorFree int int int int (void) XftColorFree ( d v cmap color -- )
libXft XftDrawSetClip int int (int) XftDrawSetClip ( d r -- bool )

: XftTextExtents  maxascii $80 =
    IF  XftTextExtentsUtf8  ELSE  XftTextExtents8  THEN ;
: XftDrawString  maxascii $80 =
    IF  XftDrawStringUtf8  ELSE  XftDrawString8  THEN ;

previous

0 Constant XftTypeVoid
1 Constant XftTypeInteger
2 Constant XftTypeDouble
3 Constant XftTypeString
4 Constant XftTypeBool
5 Constant XftTypeMatrix

struct{
    short red
    short green
    short blue
    short alpha
} XRenderColor

 struct{
    cell pixel
    struct XRenderColor color
} XftColor   

struct{
    double xx
    double xy
    double yx
    double yy
} XftMatrix

\ Create Xft0Matrix  $3FF00000. 2, 0. 2, 0. 2, $3FF00000. 2,
\ Create Xft90Matrix  0. 2, $3FF00000. 2, $3FF00000. 2, 0. 2,

struct{
    cell type
    {
        ptr s
|       cell i
|       cell b
|       double d
|       ptr m
    }
} XftValue

struct{
    ptr next
    struct XftValue value
} XftValueList

struct{
    ptr object
    ptr values
} XftPatternElt

struct{
    cell num
    cell size
    ptr elts
} XftPattern

struct{
    cell ascent
    cell descent
    cell height
    cell max_advance_width
    ptr charset
    ptr pattern
} XftFont

struct{
    short width
    short height
    short x
    short y
    short xOff
    short yOff
} XGlyphInfo

also minos also

Variable xft-draw'

displays implements
    : dispose  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        :: dispose ;
class;

backing implements
    : create-pixmap  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        xft-draw off  :: create-pixmap ;
    : dispose  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        :: dispose ;
class;

doublebuffer implements
    : create-pixmap  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        xft-draw off  :: create-pixmap ;
    : dispose  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        :: dispose ;
class;

pixmap implements
    : create-pixmap  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        xft-draw off  :: create-pixmap ;
    : dispose  xft-draw @ IF  xft-draw @ XftDrawDestroy  THEN
        :: dispose ;
class;

previous xft definitions minos

X-font class Xft-font
    method add-font
    cell var extra-id
    cell var extra-name
    cell var extra-ascent
    2 cells var extra-code
how:
    : assign ( addr u -- )
        name-string $! 0 name-string $@ + c!
        screen xrc dpy @  screen xrc screen @
        name-string $@ drop XftFontOpenXlfd dup id !
        dup 0= abort" Font not found!"
        XftFont ascent @ ascent ! ;
    : add-font ( addr u clow chigh -- )
        extra-code 2! extra-name $! 0 extra-name $@ + c!
        screen xrc dpy @  screen xrc screen @
        extra-name $@ drop XftFontOpenXlfd dup extra-id !
        dup 0= abort" Font not found!"
        XftFont ascent @ extra-ascent ! ;
    | Create xft-color  sizeof XftColor allot
    $FFFF xft-color XftColor color XRenderColor alpha w!
    : set-color ( color dpy -- )
        displays with
            dup xrc color xft-color XftColor pixel !
            cells Colortable + @
            dup cfix >r 8 >> dup cfix >r 8 >> cfix r> r>
            [ xft-color XftColor color XRenderColor red ] ALiteral w!+ w!+ w!
        endwith ;
    : scan-within ( addr u -- addr' ) over + >r
        dup  BEGIN  nip dup r@ u<  WHILE
            dup xc@+ extra-code 2@ within  UNTIL
            drop  THEN  rdrop ;
    : swap-id ( -- )
        id @ extra-id @ id ! extra-id !
        ascent @ extra-ascent @ ascent ! extra-ascent !
        extra-code 2@ swap extra-code 2! ;
    | Create text_r sizeof XGlyphInfo allot
    : size ( addr u -- w h ) 0 { addr u w }
        0  BEGIN
            1 +  addr u  scan-within >r
            screen xrc dpy @ id @ addr r@ over - text_r XftTextExtents
            text_r XGlyphInfo xOff w@ w + to w
            addr u r> addr - /string to u to addr  swap-id
        u 0<= UNTIL
        1 and IF  swap-id  THEN
        w id @ dup XftFont ascent @ swap XftFont descent @ + ;
    : draw ( addr u x y dpy -- ) { addr u x y dpy }
        color @ $FF and dpy set-color
        dpy displays with
            xft-draw @ 0= IF
                & pixmap @ class? IF
                    drawable drop xrc depth @
                    XftDrawCreateAlpha
                ELSE
                    drawable drop xrc vis @
                    xrc cmap @ XftDrawCreate
                THEN  xft-draw !
            THEN
            xft-draw @ dup clip-r @ XftDrawSetClip drop
        endwith xft-draw' ! 0
        BEGIN
            1+  addr u  scan-within >r
            xft-draw' @ xft-color id @ x y ascent @ +
            addr r@ over - XftDrawString
            screen xrc dpy @ id @ addr r@ over - text_r XftTextExtents
            text_r XGlyphInfo xOff w@ x + to x
            addr u r> addr - /string to u to addr  swap-id
        u 0<= UNTIL
        1 and IF  swap-id  THEN ;
class;

: xft-new-font  Xft-font new ;

: clear-font ( n -- )
    screen xrc font[] off ;
: add-font ( addr u encl ench n -- )
    screen xrc font[] @ Xft-font with add-font endwith ;

: (normal-font ( -- )
    screen xrc with
        s" -*-helvetica-bold-r-normal--12-*-*-*-p-*-iso10646-1" 0 font!
        s" -*-courier new-medium-r-normal--12-*-*-*-c-*-iso10646-1" 1 font!
        s" -*-helvetica-medium-r-normal--8-*-*-*-p-*-iso10646-1" 2 font!
        s" -*-helvetica-medium-r-normal--12-*-*-*-c-*-iso10646-1" 3 font!
    endwith
    maxascii $80 = IF
	s" -*-ar pl kaitim gb-bold-r-normal--15-*-*-*-c-*-iso10646-1"   $2E80 $A000 0 add-font
	s" -*-ar pl kaitim gb-medium-r-normal--14-*-*-*-c-*-iso10646-1" $2E80 $A000 1 add-font
	s" -*-ar pl kaitim gb-medium-r-normal--10-*-*-*-c-*-iso10646-1" $2E80 $A000 2 add-font
	s" -*-ar pl kaitim gb-medium-r-normal--15-*-*-*-c-*-iso10646-1" $2E80 $A000 3 add-font
    THEN
    0" -adobe-helvetica-*-r-*-*-*-120-*-*-*-*-*-*,-misc-fixed-*-r-*-*-*-130-*-*-*-*-*-*" screen xrc fontset!
    screen !resized ;

: (large-font ( -- )
    screen xrc with
       s" -*-helvetica-bold-r-normal--16-*-*-*-p-*-iso10646-1" 0 font!
       s" -*-courier new-medium-r-normal--16-*-*-*-c-*-iso10646-1" 1 font!
       s" -*-helvetica-medium-r-normal--10-*-*-*-p-*-iso10646-1" 2 font!
       s" -*-helvetica-medium-r-normal--16-*-*-*-p-*-iso10646-1" 3 font!
    endwith
    maxascii $80 = IF
	s" -*-ar pl kaitim gb-bold-r-normal--22-*-*-*-c-*-iso10646-1"   $2E80 $A000 0 add-font
	s" -*-ar pl kaitim gb-medium-r-normal--20-*-*-*-c-*-iso10646-1" $2E80 $A000 1 add-font
	s" -*-ar pl kaitim gb-medium-r-normal--12-*-*-*-c-*-iso10646-1" $2E80 $A000 2 add-font
	s" -*-ar pl kaitim gb-medium-r-normal--22-*-*-*-c-*-iso10646-1" $2E80 $A000 3 add-font
    THEN
    0" -adobe-helvetica-*-r-*-*-*-120-*-*-*-*-*-*,-misc-fixed-*-r-*-*-*-130-*-*-*-*-*-*" screen xrc fontset!
    screen !resized ;

also xft

: xft-fonts ( -- )
    ['] xft-new-font IS new-font
    ['] (large-font IS large-font
    ['] (normal-font IS normal-font ;

main: xft-fonts ;

previous previous

export xft (normal-font (large-font clear-font xft-fonts ;

module;

xft-fonts

\ 4 0 [DO] [I] clear-font [LOOP]
\ normal-font
