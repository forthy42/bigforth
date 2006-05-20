\ Xrender extension

Module xrender

also dos

legacy off

library libXrender libXrender.so.1

libXrender XRenderFindFormat ptr int ptr int (ptr) XRenderFindFormat ( dpy mask templ count -- pict )
libXrender XRenderFindVisualFormat ptr ptr (ptr) XRenderFindVisualFormat ( dpy visual -- pict )
libXrender XRenderCreatePicture ptr int ptr int ptr (int) XRenderCreatePicture ( dpy drawable format valuemask attributes -- picture )
libXrender XRenderComposite ptr int ptr ptr ptr int int int int int int int int (void) XRenderComposite ( dpy op src mask dst srcx srcy maskx masky dstx dsty w h -- )
libXrender XRenderSetPictureClipRegion ptr int int (void) XRenderSetPictureClipRegion ( dpy pict region -- )
libXrender XRenderSetPictureClipRectangles ptr ptr int int ptr int (void) XRenderSetPictureClipRectangles

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

Module;
