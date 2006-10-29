\ read raw pixmap formats (P4, P6)                     30jul97py

\ memory also
\ dos also
\ x11 also
\ \needs xconst import xconst
\ xconst also

also dos also [IFDEF] x11  xpm also  [THEN]
\ minos also forth

: pixels   dup 2* + ;   macro
: bits     1- 3 >> 1+ ; macro
: wbits    1- 5 >> 1+ 4* ; macro
: sbits    1- 4 >> 1+ 2* ; macro
1 pad ! pad c@ 1 =
[IF]    : t@  c@+ c@+ c@ swap 8 << or swap $10 << or 8 << ;
[ELSE]  : t@  @ 8 >> $FFFFFF and ; macro
[THEN]

[IFDEF] x11    
Create pixmap-format here sizeof XPixmapFormatValues
       dup allot erase
Create bitmap-format here sizeof XPixmapFormatValues
       dup allot erase

: get-pixmap-format ( -- )
    pixmap-format sizeof XPixmapFormatValues erase
    bitmap-format sizeof XPixmapFormatValues erase
    0 sp@ screen xrc dpy @ XListPixmapFormats
    tuck swap sizeof XPixmapFormatValues * bounds
    ?DO
        I XPixmapFormatValues bits_per_pixel @
        dup 1 =
        IF  I bitmap-format sizeof XPixmapFormatValues move  THEN
        screen xrc depth @ I XPixmapFormatValues depth @ =
        IF    &24 min pixmap-format XPixmapFormatValues bits_per_pixel @ >
              IF  I pixmap-format sizeof XPixmapFormatValues move  THEN
        ELSE  drop  THEN
        sizeof XPixmapFormatValues +LOOP
    XFree drop ;
[THEN]

[IFDEF] win32 
Variable pixel-bits

: get-pixmap-format ( -- )
  BITSPIXEL screen xrc dc @ GetDeviceCaps pixel-bits ! ;

: ImageByteOrder ( dpy -- n ) drop 1 ;
[THEN]

: <>.24 ( addr u -- )
    bounds ?DO  I c@ I 2+ c@ I c! I 2+ c!  3 +LOOP ;

: (flip-byte ( n -- n' )
    0 8 0 DO 2* over 1 and or swap 2/ swap LOOP nip ;

Create flip-byte  $100 0 [DO] [I] (flip-byte c, [LOOP]

: <>.8 ( addr u -- )
    bounds ?DO  I c@ flip-byte + c@ I c!  LOOP ;

Create trigger
        $0000 , $8000 , $2000 , $A000 ,
        $C000 , $4000 , $E000 , $6000 ,
        $3000 , $B000 , $1000 , $9000 ,
        $F000 , $7000 , $D000 , $5000 ,

: trans.1  { data size line dpy |
    0 data 0  data size bounds ?DO
        2 pick
        I line bounds ?DO
            I c@+ c@+ c@
            $20 * swap $A0 * + swap $40 * +
            swap dup >r cells trigger + @
            > IF  2dup +bit  ELSE  2dup -bit  THEN
            1+ r@ 1+ 3 and r> $C and or
            3 +LOOP
        drop rot 4+ $F and -rot
        line +LOOP
    dpy ImageByteOrder 0= IF data size &24 / <>.8  THEN
    2drop drop } ;

[IFDEF] old_trans.8
Code cs+!  ( n addr -- )
    DX pop  DL AX ) add
    b IF  DL DL test 0>=  IF  .b $FF # AX ) mov  THEN
    ELSE  DL DL test 0<   IF  .b $00 # AX ) mov  THEN
    THEN  AX pop
    Next end-code macro :dx :ax T&P

: trans.8  { data size line dpy |
    data  data size bounds ?DO
        I c@+ c@+ c@ rgb>
        dup cells Colortable + c@+ c@+ c@
        I 2+ c@ swap -  dup 2/ dup  I 5 + cs+! - I line + 2+ cs+!
        I 1+ c@ swap -  dup 2/ dup  I 4 + cs+! - I line + 1+ cs+!
        I    c@ swap -  dup 2/ dup  I 3 + cs+! - I line +    cs+!
        screen xrc color  swap c!+
    3 +LOOP  drop } ;

[ELSE]

Create hilbert
    $00 c, $01 c, $11 c, $10 c, $20 c, $30 c, $31 c, $21 c, $22 c, $32 c, $33 c, $23 c, $13 c, $12 c, $02 c, $03 c,
    $04 c, $14 c, $15 c, $05 c, $06 c, $07 c, $17 c, $16 c, $26 c, $27 c, $37 c, $36 c, $35 c, $25 c, $24 c, $34 c,
    $44 c, $54 c, $55 c, $45 c, $46 c, $47 c, $57 c, $56 c, $66 c, $67 c, $77 c, $76 c, $75 c, $65 c, $64 c, $74 c,
    $73 c, $72 c, $62 c, $63 c, $53 c, $43 c, $42 c, $52 c, $51 c, $41 c, $40 c, $50 c, $60 c, $61 c, $71 c, $70 c,
    $80 c, $90 c, $91 c, $81 c, $82 c, $83 c, $93 c, $92 c, $A2 c, $A3 c, $B3 c, $B2 c, $B1 c, $A1 c, $A0 c, $B0 c,
    $C0 c, $C1 c, $D1 c, $D0 c, $E0 c, $F0 c, $F1 c, $E1 c, $E2 c, $F2 c, $F3 c, $E3 c, $D3 c, $D2 c, $C2 c, $C3 c,
    $C4 c, $C5 c, $D5 c, $D4 c, $E4 c, $F4 c, $F5 c, $E5 c, $E6 c, $F6 c, $F7 c, $E7 c, $D7 c, $D6 c, $C6 c, $C7 c,
    $B7 c, $A7 c, $A6 c, $B6 c, $B5 c, $B4 c, $A4 c, $A5 c, $95 c, $94 c, $84 c, $85 c, $86 c, $96 c, $97 c, $87 c,
    $88 c, $98 c, $99 c, $89 c, $8A c, $8B c, $9B c, $9A c, $AA c, $AB c, $BB c, $BA c, $B9 c, $A9 c, $A8 c, $B8 c,
    $C8 c, $C9 c, $D9 c, $D8 c, $E8 c, $F8 c, $F9 c, $E9 c, $EA c, $FA c, $FB c, $EB c, $DB c, $DA c, $CA c, $CB c,
    $CC c, $CD c, $DD c, $DC c, $EC c, $FC c, $FD c, $ED c, $EE c, $FE c, $FF c, $EF c, $DF c, $DE c, $CE c, $CF c,
    $BF c, $AF c, $AE c, $BE c, $BD c, $BC c, $AC c, $AD c, $9D c, $9C c, $8C c, $8D c, $8E c, $9E c, $9F c, $8F c,
    $7F c, $7E c, $6E c, $6F c, $5F c, $4F c, $4E c, $5E c, $5D c, $4D c, $4C c, $5C c, $6C c, $6D c, $7D c, $7C c,
    $7B c, $6B c, $6A c, $7A c, $79 c, $78 c, $68 c, $69 c, $59 c, $58 c, $48 c, $49 c, $4A c, $5A c, $5B c, $4B c,
    $3B c, $2B c, $2A c, $3A c, $39 c, $38 c, $28 c, $29 c, $19 c, $18 c, $08 c, $09 c, $0A c, $1A c, $1B c, $0B c,
    $0C c, $0D c, $1D c, $1C c, $2C c, $3C c, $3D c, $2D c, $2E c, $3E c, $3F c, $2F c, $1F c, $1E c, $0E c, $0F c,

Create framebuf  $300 allot
Create ditherbuf $100 allot
Create colcorrect 3 allot

: hilbert-dither ( -- )
    colcorrect c@+ c@+ c@  hilbert $100 bounds DO
        framebuf I c@ 3* + 3 bounds DO  rot I c@ +  LOOP
        dup 2over rot rgb> dup screen xrc color  ditherbuf I c@ + c!
        cells Colortable + 3 bounds DO  rot I c@ -  LOOP
    LOOP  swap rot colcorrect c!+ c!+ c! ;

: trans.8  { data size line dpy |  size 3 / line 3 / { size/3 line/3 |
    size/3 NewHandle dup @ dup size/3 + { dend |
    colcorrect 3 erase
    data size bounds ?DO
        line 0 ?DO
            framebuf dup $300 erase
            $10 0 DO
                K J + I line * + dup K' <
                IF
                    over $30 J' J - min move
                ELSE  drop  LEAVE  THEN
                $30 +
            LOOP  drop
            hilbert-dither dup
            ditherbuf $100 bounds DO
                dup dend <
                IF
                    I over $10 J' J - 3 / min move
                ELSE  LEAVE  THEN
                line/3 +
            $10 +LOOP  drop
            $10 +
            $30 +LOOP
        line/3 $F * +
        line $10 * +LOOP  drop
    dup @ data size/3 move
    DisposHandle } } } ;

[THEN]

: trans.16   { data size line dpy |
    data  data size bounds ?DO
        I c@+ c@+ c@
             3 >>
        swap 3 << $07E0 and or
        swap 8 << $F800 and or
        swap w!+  3 +LOOP  drop } ;

: trans.24 ( data size line dpy -- ) nip
    ImageByteOrder 0= IF  <>.24  ELSE  2drop  THEN ;

: trans.32 ( data size line dpy -- ) nip { data size dpy |
    size 3 / 1-  dpy ImageByteOrder 0=
    IF
        FOR  0 data I pixels + c@+ c@+ c@ data I 4* + c!+ c!+ c!+ c!  NEXT
    ELSE
        FOR  data I pixels + t@ data I 4* + !  NEXT
    THEN } ;

Create trans T] trans.1 trans.8 trans.16 trans.24 trans.32 [

[IFDEF] x11

: create-pixmap { data size w h bits dpy | ( --> pixmap w h )
    data size w pixels dpy trans bits cells + perform
    data 0 ZPixmap pixmap-format XPixmapFormatValues depth @
    dpy dup DefaultScreen DefaultVisual
    dpy XCreateImage
    pixmap-format XPixmapFormatValues depth @
    h w screen xwin @ dpy XCreatePixmap
    { img pix |
      h w 0 0 0 0 img screen drawable nip pix swap XPutImage drop
      img XImage data off  img XDestroyImage  data DisposPtr
      pix w h } } ;

[IFDEF] has-png  include png.fs [THEN]

: readP6 ( fd w h bits -- pixmap w h )
    { fd w h bits |
      w bits * pixmap-format XPixmapFormatValues depth @
      1 = IF  8  ELSE  pixmap-format XPixmapFormatValues scanline_pad @  THEN
      h w  w pixels h * w pixels h $10 + -$10 and *
      w bits * h $10 + -$10 and * max cell+ dup NewPtr
      tuck swap erase
      screen xrc dpy @
      { size data dpy |
        data size fd read-file throw drop
        data size w h bits dpy create-pixmap } } ;

Create values sizeof XGCValues allot

: readP4.1 ( fd w h -- pixmap )
    { fd w h |
      w bits bitmap-format XPixmapFormatValues scanline_pad @ h w
      w bits h * dup NewPtr  screen xrc dpy @
      { size data dpy |
        data size fd read-file throw drop
        dpy BitmapBitOrder 0= IF  data size <>.8  THEN
        data 0 XYPixmap bitmap-format XPixmapFormatValues depth @
        dpy dup DefaultScreen DefaultVisual
        dpy XCreateImage
        bitmap-format XPixmapFormatValues depth @
        h w screen xwin @ dpy XCreatePixmap
        { img pix |
          h w 0 0 0 0 img
          values 0 pix dpy XCreateGC dup >r
          pix dpy XPutImage drop
          r> dpy XFreeGC drop
          img XImage data off  img XDestroyImage   data DisposPtr
          pix w h } } } ;
[THEN]

[IFDEF] win32
Create bminfohead  sizeof BITMAPINFOHEADER allot
Create bminfo      sizeof BITMAPINFOHEADER allot  0 , $FFFFFF ,
sizeof BITMAPINFOHEADER dup bminfohead ! bminfo !
1      bminfo BITMAPINFOHEADER biPlanes w!
BI_RGB bminfo BITMAPINFOHEADER biCompression w!
0      bminfo BITMAPINFOHEADER biSizeImage w!
&2952  bminfo BITMAPINFOHEADER biXPelsPerMeter w!
&2952  bminfo BITMAPINFOHEADER biYPelsPerMeter w!
0      bminfo BITMAPINFOHEADER biClrImportant w!
1      bminfohead BITMAPINFOHEADER biPlanes w!
BI_RGB bminfohead BITMAPINFOHEADER biCompression w!
0      bminfohead BITMAPINFOHEADER biSizeImage w!
&2952  bminfohead BITMAPINFOHEADER biXPelsPerMeter w!
&2952  bminfohead BITMAPINFOHEADER biYPelsPerMeter w!
0      bminfohead BITMAPINFOHEADER biClrImportant w!

: expand-bits ( u1 u2 h addr -- )
    { addr |
    1- FOR
        over I * addr + over I * addr + 3 pick move
        dup I * addr + over 3 pick /string erase
    NEXT 2drop } ;
    
: paligned ( -- )  3 + -4 and ;

: !bmheader ( w h bits bminfo -- ) >r   
          r@ BITMAPINFOHEADER biBitCount w!
  negate  r@ BITMAPINFOHEADER biHeight !
          r> BITMAPINFOHEADER biWidth ! ;

: readP6 ( fd w h bits -- pixmap w h )
    { fd w h bits |
      w pixels h *
      w pixels paligned h 1+ *
      w bits * paligned h 1+ * max cell+ NewPtr
      { size data | data size fd read-file throw drop  data size <>.24
        w pixels dup paligned <> IF
            w pixels dup paligned h data expand-bits
        THEN
\       data size w pixels dpy trans bits cells + perform
        w h &24 bminfo !bmheader
        w h pixel-bits @ bminfohead !bmheader

        DIB_RGB_COLORS bminfo data CBM_INIT bminfohead
        screen xrc dc @ CreateDIBitmap >r
        data DisposPtr r> w h } }  ;

: readP4.1 ( fd w h -- pixmap )
    { fd w h |
      w bits h * w wbits h * NewPtr
      { size data | data size fd read-file throw drop
        w sbits w wbits <> IF
            w sbits w wbits h data expand-bits
        THEN
\       data size w pixels dpy trans bits cells + perform
        w h 1 bminfo !bmheader
        w h 1 bminfohead !bmheader
        
        DIB_RGB_COLORS bminfo data CBM_INIT bminfohead
        screen xrc dc @ CreateDIBitmap >r
        data DisposPtr r> w h } }  ;
[THEN]

: read-format ( fd -- w h ) >r
    scratch $100
    BEGIN  drop dup $100 r@ read-line  throw drop  over c@ '#
           <>  UNTIL
    0. 2swap >number 1 /string 0. 2swap >number 2drop drop nip
    rdrop ;
: read-P6 ( fd -- pixmap w h ) >r
    r@ read-format
    scratch $100 r@ read-line throw 2drop
    r> -rot
[IFDEF] x11
    pixmap-format XPixmapFormatValues bits_per_pixel @  [THEN]
[IFDEF] win32
    pixel-bits @  [THEN]
    3 >> readP6 ;
    
: read-P4 ( fd -- pixmap w h ) >r
    r@ read-format
    r> -rot readP4.1 ;
    
: read-picture ( addr u -- pixmap w h )
    r/o open-file throw >r
    scratch dup $100 r@ read-line throw drop
    2dup S" P6" compare 0=
    IF  2drop r@ read-P6 r> close-file throw  EXIT  THEN
    2dup S" P4" compare 0=
    IF  2drop r@ read-P4 r> close-file throw  EXIT  THEN
    2drop r> close-file true abort" Unsupported format" throw ;

[IFDEF] x11
: fix-color { shape pixmap w h |
    4 screen drawable nip XSetFunction drop
    1 pixmap-format XPixmapFormatValues depth @ << 1-
    screen drawable nip XSetBackground drop
    1 0 0 h w 0 0 screen drawable nip shape pixmap rot
          XCopyPlane drop
    3 screen drawable nip XSetFunction drop
    } ;
[THEN]

[IFDEF] win32
| : >gc ( win -- gc )
     screen xrc dc @ CreateCompatibleDC tuck SelectObject drop ;
$00BB0226 Value :fixand
: fix-color { shape pixmap w h |
    :fixand 0 0 pixmap >gc dup >r
      h w 0 0 shape >gc dup >r BitBlt drop r> DeleteDC drop
      r> DeleteDC drop
    } ;
[THEN]

: read-icn ( fd -- pixmap1 pixmap2 w h ) >r
    scratch dup $100 r@ read-line throw drop
    2dup S" P6" compare 0= IF  2drop r@ read-P6 2drop
        scratch dup $100 2dup erase r@ read-line throw drop  THEN
    2dup S" P4" compare 0= IF  2drop r@ read-P4 r> close-file throw
        2over 2over fix-color EXIT  THEN
    2drop r> close-file true abort" Unsupported format" throw ;

: read-ppm ( fd -- pixmap1 0 w h ) >r
    scratch dup $100 r@ read-line throw drop
    S" P6" compare 0=
    IF  r@ read-P6 0 -rot
        r> close-file throw  EXIT  THEN
    r> close-file true abort" Unsupported format" throw ;

\ read Xpm icons                                       30jul97py

[IFDEF] x11

Create Xpmattribs sizeof XpmAttributes allot
        XpmSize XpmReturnPixels + \ XpmColormap +
        XpmRGBCloseness +
        Xpmattribs XpmAttributes valuemask !

: set-attrib ( -- )
    screen xrc dpy @ screen xrc screen @ DefaultColormap
    Xpmattribs XpmAttributes colormap !
    $3000 Xpmattribs XpmAttributes red_closeness !
    $3000 Xpmattribs XpmAttributes green_closeness !
    $8000 Xpmattribs XpmAttributes blue_closeness !
    1 Xpmattribs XpmAttributes alloc_close_colors ! ;

: read-xpm ( fd -- pixmap1 pixmap2 w h )
    dup >r filename set-attrib
    >r 0 sp@ >r 0 sp@ >r Xpmattribs r> r> r>
    screen xwin @ screen xrc dpy @ XpmReadFileToPixmap drop
    Xpmattribs XpmAttributes width @
    Xpmattribs XpmAttributes height @
    r> close-file throw ;
[THEN]

\ read icons                                           30jul97py

Variable icon-base

: suffix? ( addr1 u1 addr2 u2 -- addr1 u1 false / fd true )
    2over 2over 2swap dup 3 pick - 0max /string compare 0=
    IF  2drop r/o open-file throw  true  EXIT  THEN
    2over icon-base $! icon-base $+!
    icon-base $@ r/o open-file 0=
    IF  nip nip  true  ELSE  drop false  THEN ;

: (read-icon ( addr u -- pixmap1 pixmap2 w h )
[IFDEF] read-png
    s" .png" suffix? IF  read-png  EXIT  THEN
[THEN]
    s" .icn" suffix? IF  read-icn  EXIT  THEN
[IFDEF] x11
    s" .xpm" suffix? IF  read-xpm  EXIT  THEN
[THEN]
    s" .ppm" suffix? IF  read-ppm  EXIT  THEN
    true abort" Unknown icon file format" ;

Patch read-icon
' (read-icon IS read-icon

0 [IF]
get-pixmap-format

s" icons/printer.icn" read-icon
constant h constant w constant shape constant pixmap
s" icons/printer.xpm" read-icon
constant xh constant xw constant xshape constant xpixmap

: pm 0 0  w h  0 0 shape pixmap term dpy mask ;
: xpm 0 0  xw xh  0 0 xshape xpixmap term dpy mask ;
[THEN]

[IFDEF] x11   toss  [THEN]  toss toss
