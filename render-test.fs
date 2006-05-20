\ render test
\needs xrender include xrender.fs
\needs xconst | import xconst
x11 also xrender also xconst also minos also forth
screen xrc dpy @ Value dpy
s" minos/minos3.png" read-png Value h Value w drop Value mpict
dpy dup dup DefaultScreen DefaultVisual XRenderFindVisualFormat Constant rgb24
Create pict_attrib sizeof XRenderPictureAttributes allot
1 pict_attrib XRenderPictureAttributes dither !
dpy term dpy dpy xwin @ rgb24 $800 pict_attrib XRenderCreatePicture Value mdest
dpy 3 mpict 0 mdest 0 0 0 0 0 0 w h XRenderComposite