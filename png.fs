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

: read-png-image ( fd -- addr w h color_type ) >r
    r@ filehandle @ 0" r" fdopen dup
    init-png >r r@ rot png_init_io
    r@ over pngflags 0 png_read_png
    nip r>
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

: read-png ( fd -- pixmap mask w h )
    read-png-image 4 and IF
        screen xrc dpy @ { img w h dpy |
        dpy screen xwin @ w h $20 XCreatePixmap
        ARGB32 @ 0= IF
            dpy PictStandardARGB32 XRenderFindStandardFormat
            ARGB32 $20 move  THEN
        ARGB32 2dup dpy -rot 0 0 XRenderCreatePicture { pixmap rgba32 mpict |
	dpy dup dup DefaultScreen DefaultVisual $20 ZPixmap 0 img w h $20 w 4*
        XCreateImage  dpy pixmap 0 0 XCreateGC { ximg gc |
        dpy pixmap gc ximg 0 0 0 0 w h XPutImage
        dpy gc XFreeGC
        ximg XImage data off  ximg XDestroyImage   img DisposPtr
        mpict -1 w h } } }
    ELSE
        pixmap-format XPixmapFormatValues bits_per_pixel @ 3 >>
        { data w h bits |
        data w h * 3* <>.24
        data w h * 3* w h
        create-pixmap 0 -rot }
    THEN ;
    
previous previous previous previous previous previous

