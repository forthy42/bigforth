\ Test OpenGL library

include opengl.fs
import float
| import glconst
| import xconst
float also
xconst also
x11 also
minos also
opengl also
glconst also
forth

Create attrib GLX_rgba ,
GLX_Red_size , 1 ,
GLX_Green_size , 1 ,
GLX_Blue_size , 1 , 0 ,

0 Value visinfo
0 Value pm
0 Value ctx
0 Value glxpm

: init-it
  attrib screen xrc dpy @ dup DefaultScreen swap glXChooseVisual TO visinfo

  visinfo XVisualInfo depth @ $100 $100 screen xwin @ screen xrc dpy @
  XCreatePixmap TO pm

  1 0 visinfo screen xrc dpy @ glXCreateContext TO ctx

  pm visinfo screen xrc dpy @ glXCreateGLXPixmap TO glxpm

  ctx pm screen xrc dpy @ glXMakeCurrent drop ;

: render-it
  GL_FLAT glShadeModel drop
  !1 f>fs !.5 f>fs dup dup glClearColor drop
  GL_COLOR_BUFFER_BIT glClear drop
  $100 $100 0 0 glViewport drop
  !1 f>fd !-1 f>fd !1 f>fd !-1 f>fd !1 f>fd !-1 f>fd glOrtho drop
  !1 f>fs dup !0 f>fs glColor3f drop
  !.75 f>fs dup !-.75 f>fs dup glRectf drop ;
: draw-it
  0 0 $100 $100 0 0 pm term dpy image ;
: do-it  init-it render-it draw-it ;
