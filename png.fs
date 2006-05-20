\ portable network graphics

also DOS

s" libpng12.so.0" getlib 0<> Value newpng
true Value png3

newpng [IF]
    library libpng libpng12.so.0
[ELSE]
    s" libpng.so.3" getlib 0<> to png3
    png3 [IF]
	library libpng libpng.so.3 0 ,
    [ELSE]
	library libpng libpng.so.2 0 ,
    [THEN]
[THEN]

: init-png-lib ( -- )
    & libpng cell+ @ ?EXIT
    true to png3 s" libpng12.so.0" getlib 0<> to newpng
    newpng IF  s" libpng12.so.0"
    ELSE  s" libpng.so.3" getlib 0<> to png3
	png3  IF  s" libpng.so.3"  ELSE  s" libpng.so.2"  THEN  THEN
    & libpng 4 cells + place ;

legacy off

libpng png_create_read_struct int ptr int int (ptr) png_create_read_struct
libpng png_create_info_struct ptr (ptr) png_create_info_struct
libpng png_destroy_read_struct ptr ptr ptr (void) png_destroy_read_struct
libpng png_read_png ptr ptr int ptr (void) png_read_png
libpng png_init_io ptr ptr (void) png_init_io
libpng png_get_rows ptr ptr (ptr) png_get_rows ( read info -- rows )
libpng png_read_end ptr ptr (void) png_read_end ( png info -- )
libpng png_free_data ptr ptr int int (void) png_free_data ( png info mask seq -- )
    
libc fdopen int ptr (ptr) fdopen
libc _dup int (int) dup
libc setjmp ptr (int) setjmp

Variable user_error_ptr

: info-struct ( readstruc -- infostruc readstruc ) >r
    r@ png_create_info_struct
    dup 0= IF  rp@ 0 0 png_destroy_read_struct
        true abort" no info struct"
    THEN r> ;

: init-png ( -- infostruc readstruc )
    init-png-lib
    png3 IF  0" 1.2.0"  ELSE  0" 1.0.5"  THEN
    user_error_ptr ['] noop dup png_create_read_struct
    dup 0= abort" PNG: no read structure"
    info-struct dup setjmp IF
        dup png_destroy_read_struct
        true abort" PNG: setjmp received"
    THEN ;

struct{
cell width
cell height
cell valid
cell rowbytes
cell palette
short num_palette
short num_trans
byte bit_depth
byte color_type
byte compression_type
byte filter_type
byte interlace_type
byte channels
byte pixel_depth
byte spare_byte
8 signature
} png_info_struct

Variable color_type

: png2array ( readstruc infostruc -- addr w h color_type )
    dup png_info_struct width @
    over png_info_struct height @
    over2 png_info_struct color_type c@ >r
    over2 png_info_struct rowbytes @
    { png info w h rowbytes |
    png info png_get_rows
    w h * cells allocate throw tuck
    h 0 ?DO  over @ over rowbytes move  cell rowbytes d+  LOOP
    2drop w h
\    png info png_read_end
    png info $7FFF -1 png_free_data } r> ;

DOS

$0095 Value pngflags

: read-png-image ( addr u -- addr w h color_type )
    r/o open-file throw >r
    r@ filehandle @ _dup 0" r" fdopen dup
    init-png >r r@ rot png_init_io
    r@ over pngflags 0 png_read_png
    swap fclose drop r>
    r> close-file throw
    swap png2array ;

\needs xconst | import xconst
\needs xrender include xrender.fs
also memory also xconst also x11 also xrender also minos

Create ARGB32
0 ,    \ id - dummy
1 ,    \ direct
$20 ,  \ depth
$10 w, \ red
$FF w,
8 w,   \ green
$FF w,
$0 w,  \ blue
$FF w,
$18 w, \ alpha
$FF w,
0 ,    \ colormap - dummy

: read-png ( addr u -- pixmap mask w h )
    read-png-image 4 and IF
        screen xrc dpy @ { img w h dpy |
	$20 h w screen xwin @ dpy XCreatePixmap
	ARGB32 @ 0= IF
	    dpy $7FE ARGB32 0 XRenderFindFormat
	    ARGB32 $20 move  THEN
	ARGB32 2dup dpy -rot 0 0 XRenderCreatePicture { pixmap rgba32 mpict |
        w 4* $20 h w img 0 ZPixmap $20 dpy dup DefaultScreen DefaultVisual dpy
        XCreateImage  0 0 pixmap dpy XCreateGC { ximg gc |
        h w 0 0 0 0 ximg gc pixmap dpy XPutImage drop
        gc dpy XFreeGC drop
        ximg XImage data off  ximg XDestroyImage   img DisposPtr
        mpict -1 w h } } }
    ELSE
        pixmap-format XPixmapFormatValues bits_per_pixel @ 3 >>
        { data w h bits |
        data w h * 3* <>.24
        w bits * pixmap-format XPixmapFormatValues depth @
        1 = IF  8  ELSE  pixmap-format XPixmapFormatValues scanline_pad @  THEN
        h w
        data w h * 3* w h bits
        screen xrc dpy @  create-pixmap 0 -rot }
    THEN ;
    
previous previous previous previous previous previous

