
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

