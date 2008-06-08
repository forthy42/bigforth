include xpm.fs
xconst also xpm also windows

create attribs sizeof XpmAttributes allot

dpy0 dpy @ dpy0 screen @ DefaultColormap attribs XpmAttributes colormap !
&40000 attribs XpmAttributes closeness !
XpmSize XpmReturnPixels or XpmColormap or XpmCloseness or
attribs XpmAttributes valuemask !
Variable shape
Variable pixmap
attribs shape pixmap 0" /usr/X11R6/include/X11/pixmaps/xterm-linux.xpm"
screen xwin @ dpy0 dpy @ XpmReadFileToPixmap .

: set-fun  ( n -- )  win0 drawable' nip rot XSetFunction drop ;

: draw-icon ( x y -- ) { x y |
  0 win0 drawable nip XSetForeground drop
  1 set-fun 1 y x 100 100 0 0 win0 drawable shape @ swap XCopyPlane drop
  6 set-fun   0 0 100 100 x y pixmap @ win0 drawimage
  3 set-fun } ;
