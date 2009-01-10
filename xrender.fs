\ Xrender extension

Module xrender

also dos

legacy off

[IFDEF] osx
    s" /usr/X11/lib/libXrender.dylib" file-status nip 0= [IF]
	library libXrender /usr/X11/lib/libXrender.dylib
    [ELSE]
	library libXrender /usr/X11R6/lib/libXrender.dylib
    [THEN]
[ELSE]
    library libXrender libXrender.so.1
[THEN]

libXrender XRenderFindFormat ptr int ptr int (ptr) XRenderFindFormat ( dpy mask templ count -- pict )
libXrender XRenderFindVisualFormat ptr ptr (ptr) XRenderFindVisualFormat ( dpy visual -- pict )
libXrender XRenderFindStandardFormat ptr int (ptr) XRenderFindStandardFormat ( dpy format -- pict )
libXrender XRenderCreatePicture ptr int ptr int ptr (int) XRenderCreatePicture ( dpy drawable format valuemask attributes -- picture )
libXrender XRenderComposite ptr int ptr ptr ptr int int int int int int int int (void) XRenderComposite ( dpy op src mask dst srcx srcy maskx masky dstx dsty w h -- )
libXrender XRenderSetPictureClipRegion ptr int int (void) XRenderSetPictureClipRegion ( dpy pict region -- )
libXrender XRenderSetPictureClipRectangles ptr ptr int int ptr int (void) XRenderSetPictureClipRectangles
libXrender XRenderFreePicture ptr ptr (void) XRenderFreePicture

previous

struct{
cell repeat
cell alpha_map
cell alpha_x_origin
cell alpha_y_origin
cell clip_x_origin
cell clip_y_origin
cell clip_mask
cell graphics_exposures
cell subwindow_mode
cell poly_edge
cell poly_mode
cell dither
cell component_alpha
} XRenderPictureAttributes

0 Constant PictStandardARGB32
1 Constant PictStandardRGB24
2 Constant PictStandardA8
3 Constant PictStandardA4
4 Constant PictStandardA1
\ 5 Constant PictStandardNUM

Module;
