\ Enlightenment style                                  14feb01py

\ This style uses Imlib                                14feb01py

\needs imlib include imlib.fs
\needs xconst | import xconst

also DOS also Memory also imlib also xconst also X11 also MINOS also

: get-imdata ( dpy -- imdata )
    dup ImlibInit >r
    dup DefaultScreen DefaultDepth
    dup 24 >= IF drop 4 ELSE
    15 < IF  \ 0" colors.rgb" r@ ImlibLoadColors drop
             3 ELSE 5 THEN THEN
    r@ ImlibSetRenderType drop r> ;

xresource implements
        : open ( string -- )
          XOpenDisplay dup dpy ! get-imdata imdata ! ;
        \ There should be a close method, but there
        \ is no such thing in Imlib
class;

screen xrc dpy @ get-imdata screen xrc imdata !

\ read icons with imlib                                14feb98py

\ make a awful pink the default background
| Create bgcol  struct ImColor allot
$FE bgcol ImColor r !
$00 bgcol ImColor g !
$FF bgcol ImColor b !

: (read-imicon ( addr u -- image )
  s" .icn" suffix? IF  read-icn  pause  EXIT  THEN
  dup 1+ NewPtr dup >r place r@ c>0"
  r@ >path.file
  screen xrc imdata @ ImlibLoadImage
  bgcol over Image shape_color sizeof ImColor move
  r> DisposPtr ;
: >pswh ( image --- pixmap1 pixmap2 w h )  >r
  r@ Image pixmap @
  r@ Image shape_mask @
  r@ Image width @
  r> Image height @ ;
: read-imicon  (read-imicon >pswh ;

\ ' read-imicon IS read-icon
