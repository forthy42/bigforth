\ Xrender extension

Vocabulary xrender

also xrender definitions

library: libXrender.so.1

extern: char * XRenderFindFormat( char *, int, char *, int );  ( dpy mask templ count -- pict )
extern: char * XRenderFindVisualFormat( char *, char * );  ( dpy visual -- pict )
extern: char * XRenderFindStandardFormat( char *, int );  ( dpy format -- pict )
extern: int XRenderCreatePicture( char *, int, char *, int, char * );  ( dpy drawable format valuemask attributes -- picture )
extern: void XRenderComposite( char *, int, char *, char *, char *, int, int, int, int, int, int, int, int ); ( dpy op src mask dst srcx srcy maskx masky dstx dsty w h -- )
extern: void XRenderSetPictureClipRegion( char *, int, int );  ( dpy pict region -- )
extern: void XRenderSetPictureClipRectangles( char *, char *, int, int, char *, int ); 
extern: void XRenderFreePicture( char *, char * ); 

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

previous definitions
