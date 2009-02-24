\ Mesa Open GL library interface                       04oct97py

Module OpenGL

DOS also OpenGL

[IFDEF] x11
    also x11
    
\    library libGL libMesaGL.so.3  $20 allot   depends libXext depends libm
[IFDEF] osx
    s" /usr/X11/lib/libGL.dylib" file-status nip 0= [IF]
	library libGL /usr/X11/lib/libGL.dylib $20 allot    depends libX11
	library libGLU /usr/X11/lib/libGLU.dylib
    [ELSE]
	library libGL /usr/X11R6/lib/libGL.dylib $20 allot    depends libX11
	library libGLU /usr/X11R6/lib/libGLU.dylib
    [THEN]
[ELSE]
    library libGL libGL.so.1 $20 allot    depends libX11
    library libGLU libGLU.so.1
[THEN]
    | ' libGL alias libGLext
    previous
    true Value ?texture immediate

    : !gl-lib ( "name" -- )
      bl word count [ ' libGL >body $10 + ] ALiteral place ;
[THEN]
[IFDEF] win32
    library libGL OpenGL32
    library libGLext OpenGL32
    
Defer dummy-canvas  ' noop IS dummy-canvas
: ?texture ( -- flag )  & libGL @syms
  dummy-canvas   
  s" glBindTexture" ['] libGL >body cell+ @ procaddr 0<> ;
immediate
[THEN]

\ functions                                            04oct97py

legacy off

libGL glClearIndex sf (void/fp) glClearIndex ( f -- )
libGL glClearColor sf sf sf sf (void/fp) glClearColor ( r g b a -- )
libGL glClear int (void/fp) glClear ( mask -- )
libGL glIndexMask int (void/fp) glIndexMask ( mask -- )
libGL glColorMask int int int int (void/fp) glColorMask ( r g b a -- )
libGL glAlphaFunc int sf (void/fp) glAlphaFunc ( f r -- )
libGL glBlendFunc int int (void/fp) glBlendFunc ( d s -- )
libGL glLogicOp int (void/fp) glLogicOp ( opcode -- )
libGL glCullFace int (void/fp) glCullFace ( mode -- )
libGL glFrontFace int (void/fp) glFrontFace ( mode -- )
libGL glPointSize sf (void/fp) glPointSize ( sf -- )
libGL glLineWidth sf (void/fp) glLineWidth ( wf -- )
libGL glLineStipple int int (void/fp) glLineStipple ( factor pattern -- )
libGL glPolygonMode int int (void/fp) glPolygonMode ( face mode -- )
libGLext glPolygonOffset sf sf (void/fp) glPolygonOffset ( funits ffactor -- )
libGL glPolygonStipple int (void/fp) glPolygonStipple ( *mask -- )
libGL glGetPolygonStipple int (void/fp) glGetPolygonStipple ( *mask -- )
libGL glEdgeFlag int (void/fp) glEdgeFlag ( flag -- )
libGL glEdgeFlagv int (void/fp) glEdgeFlagv ( *flag -- )
libGL glScissor int int int int (void/fp) glScissor ( x y w h -- )
libGL glClipPlane int int (void/fp) glClipPlane ( plane *equation -- )
libGL glGetClipPlane int int (void/fp) glGetClipPlane ( plane *eqation -- )
libGL glDrawBuffer int (void/fp) glDrawBuffer ( mode -- )
libGL glReadBuffer int (void/fp) glReadBuffer ( mode -- )
libGL glEnable int (void/fp) glEnable ( cap -- )
libGL glDisable int (void/fp) glDisable ( cap -- )
libGL glIsEnabled int (int/fp) glIsEnabled ( cap -- flag )
libGLext glEnableClientState int (void/fp) glEnableClientState ( cap -- )
libGLext glDisableClientState int (void/fp) glDisableClientState ( cap -- )
libGL glGetBooleanv int int (void/fp) glGetBooleanv ( pname params -- )
libGL glGetDoublev int int (void/fp) glGetDoublev ( pname params -- )
libGL glGetFloatv int int (void/fp) glGetFloatv ( pname params -- )
libGL glGetIntegerv int int (void/fp) glGetIntegerv ( pname params -- )
libGL glPushAttrib int (void/fp) glPushAttrib ( mask -- )
libGL glPopAttrib (void/fp) glPopAttrib ( -- )
libGLext glPushClientAttrib int (void/fp) glPushClientAttrib ( mask -- )
libGLext glPopClientAttrib (void/fp) glPopClientAttrib ( -- )
libGL glRenderMode int (void/fp) glRenderMode ( mode -- n )
libGL glGetError (void/fp) glGetError ( -- n )
libGL glGetString int (void/fp) glGetString ( name -- string )
libGL glFinish (void/fp) glFinish ( -- )
libGL glFlush (void/fp) glFlush ( -- )
libGL glHint int int (void/fp) glHint ( target mode -- )

\ Depth Buffer                                         04oct97py

libGL glClearDepth df (void/fp) glClearDepth ( ddepth -- )
libGL glDepthFunc int (void/fp) glDepthFunc ( func -- )
libGL glDepthMask int (void/fp) glDepthMask ( flag -- )
libGL glDepthRange df df (void/fp) glDepthRange ( dnear dfar -- )

\ Accumulation Buffer                                  04oct97py

libGL glClearAccum sf sf sf sf (void/fp) glClearAccum ( fr fg fb fa -- )
libGL glAccum sf int (void/fp) glAccum ( op fvalue -- )

\ Transformation                                       04oct97py

libGL glMatrixMode int (void/fp) glMatrixMode ( mode -- )
libGL glOrtho df df df df df df (void/fp) glOrtho
                   ( dleft dright dbottom dtop dnear dfar -- )
libGL glFrustum df df df df df df (void/fp) glFrustum
                   ( dleft dright dbottom dtop dnear dfar -- )
libGL glViewport int int int int (void/fp) glViewport ( x y w h -- )
libGL glPushMatrix (void/fp) glPushMatrix ( -- )
libGL glPopMatrix (void/fp) glPopMatrix ( -- )
libGL glLoadIdentity (void/fp) glLoadIdentity ( -- )
libGL glLoadMatrixd int (void/fp) glLoadMatrixd ( *m -- )
libGL glLoadMatrixf int (void/fp) glLoadMatrixf ( *m -- )
libGL glMultMatrixd int (void/fp) glMultMatrixd ( *m -- )
libGL glMultMatrixf int (void/fp) glMultMatrixf ( *m -- )
libGL glRotated df df df df (void/fp) glRotated ( da dx dy dz -- )
libGL glRotatef sf sf sf sf (void/fp) glRotatef ( fa fx fy fz -- )
libGL glScaled df df df (void/fp) glScaled ( dx dy dz -- )
libGL glScalef sf sf sf (void/fp) glScalef ( fx fy fz -- )
libGL glTranslated df df df (void/fp) glTranslated ( dx dy dz -- )
libGL glTranslatef sf sf sf (void/fp) glTranslatef ( fx fy fz -- )

\ Display Lists                                        04oct97py

libGL glIsList int (int/fp) glIsList ( list -- flag )
libGL glDeleteLists int int (void/fp) glDeleteLists ( range list -- )
libGL glGenLists int (int/fp) glGenLists ( range -- u )
libGL glNewList int int (void/fp) glNewList ( list mode -- )
libGL glEndList (void/fp) glEndList ( -- )
libGL glCallList int (void/fp) glCallList ( list -- )
libGL glCallLists int int int (void/fp) glCallLists ( n type *lists -- )
libGL glListBase int (void/fp) glListBase ( base -- )

\ Drawing Functions                                    04oct97py

libGL glBegin int (void/fp) glBegin ( mode -- )
libGL glEnd (void/fp) glEnd ( -- )

libGL glVertex2d df df (void/fp) glVertex2d ( dx dy -- )
libGL glVertex2f sf sf (void/fp) glVertex2f ( fx fy -- )
libGL glVertex2i int int (void/fp) glVertex2i ( x y -- )
libGL glVertex2s int int (void/fp) glVertex2s ( x y -- )

libGL glVertex3d df df df (void/fp) glVertex3d ( dx dy dz -- )
libGL glVertex3f sf sf sf (void/fp) glVertex3f ( fx fy fz -- )
libGL glVertex3i int int int (void/fp) glVertex3i ( x y z -- )
libGL glVertex3s int int int (void/fp) glVertex3s ( x y z -- )

libGL glVertex4d df df df df (void/fp) glVertex4d ( fx fy fz fw -- )
libGL glVertex4f sf sf sf sf (void/fp) glVertex4f ( fx fy fz fw -- )
libGL glVertex4i int int int int (void/fp) glVertex4i ( x y z w -- )
libGL glVertex4s int int int int (void/fp) glVertex4s ( x y z w -- )

libGL glVertex2dv int (void/fp) glVertex2dv ( *v -- )
libGL glVertex2fv int (void/fp) glVertex2fv ( *v -- )
libGL glVertex2iv int (void/fp) glVertex2iv ( *v -- )
libGL glVertex2sv int (void/fp) glVertex2sv ( *v -- )

libGL glVertex3dv int (void/fp) glVertex3dv ( *v -- )
libGL glVertex3fv int (void/fp) glVertex3fv ( *v -- )
libGL glVertex3iv int (void/fp) glVertex3iv ( *v -- )
libGL glVertex3sv int (void/fp) glVertex3sv ( *v -- )

libGL glVertex4dv int (void/fp) glVertex4dv ( *v -- )
libGL glVertex4fv int (void/fp) glVertex4fv ( *v -- )
libGL glVertex4iv int (void/fp) glVertex4iv ( *v -- )
libGL glVertex4sv int (void/fp) glVertex4sv ( *v -- )

libGL glNormal3b int int int (void/fp) glNormal3b ( nx ny nz -- )
libGL glNormal3d df df df (void/fp) glNormal3d ( dnx dny dnz -- )
libGL glNormal3f sf sf sf (void/fp) glNormal3f ( nx ny nz -- )
libGL glNormal3i int int int (void/fp) glNormal3i ( nx ny nz -- )
libGL glNormal3s int int int (void/fp) glNormal3s ( nx ny nz -- )

libGL glNormal3bv int (void/fp) glNormal3bv ( *v -- )
libGL glNormal3dv int (void/fp) glNormal3dv ( *v -- )
libGL glNormal3fv int (void/fp) glNormal3fv ( *v -- )
libGL glNormal3iv int (void/fp) glNormal3iv ( *v -- )
libGL glNormal3sv int (void/fp) glNormal3sv ( *v -- )

libGL glIndexd df (void/fp) glIndexd ( dc -- )
libGL glIndexf sf (void/fp) glIndexf ( fc -- )
libGL glIndexi int (void/fp) glIndexi ( c -- )
libGL glIndexs int (void/fp) glIndexs ( c -- )
libGLext glIndexub int (void/fp) glIndexub ( c -- )

libGL glIndexdv int (void/fp) glIndexdv ( *c -- )
libGL glIndexfv int (void/fp) glIndexfv ( *c -- )
libGL glIndexiv int (void/fp) glIndexiv ( *c -- )
libGL glIndexsv int (void/fp) glIndexsv ( *c -- )
libGLext glIndexubv int (void/fp) glIndexubv ( *c -- )

libGL glColor3b int int int (void/fp) glColor3b ( r g b -- )
libGL glColor3d df df df (void/fp) glColor3d ( r g b -- )
libGL glColor3f sf sf sf (void/fp) glColor3f ( r g b -- )
libGL glColor3i int int int (void/fp) glColor3i ( r g b -- )
libGL glColor3s int int int (void/fp) glColor3s ( r g b -- )
libGL glColor3ub int int int (void/fp) glColor3ub ( r g b -- )
libGL glColor3ui int int int (void/fp) glColor3ui ( r g b -- )
libGL glColor3us int int int (void/fp) glColor3us ( r g b -- )

libGL glColor4b int int int int (void/fp) glColor4b ( r g b a -- )
libGL glColor4d df df df df (void/fp) glColor4d ( r g b a -- )
libGL glColor4f sf sf sf sf (void/fp) glColor4f ( r g b a -- )
libGL glColor4i int int int int (void/fp) glColor4i ( r g b a -- )
libGL glColor4s int int int int (void/fp) glColor4s ( r g b a -- )
libGL glColor4ub int int int int (void/fp) glColor4ub ( r g b a -- )
libGL glColor4ui int int int int (void/fp) glColor4ui ( r g b a -- )
libGL glColor4us int int int int (void/fp) glColor4us ( r g b a -- )

libGL glColor3bv int (void/fp) glColor3bv ( *v -- )
libGL glColor3dv int (void/fp) glColor3dv ( *v -- )
libGL glColor3fv int (void/fp) glColor3fv ( *v -- )
libGL glColor3iv int (void/fp) glColor3iv ( *v -- )
libGL glColor3sv int (void/fp) glColor3sv ( *v -- )
libGL glColor3ubv int (void/fp) glColor3ubv ( *v -- )
libGL glColor3uiv int (void/fp) glColor3uiv ( *v -- )
libGL glColor3usv int (void/fp) glColor3usv ( *v -- )

libGL glColor4bv int (void/fp) glColor4bv ( *v -- )
libGL glColor4dv int (void/fp) glColor4dv ( *v -- )
libGL glColor4fv int (void/fp) glColor4fv ( *v -- )
libGL glColor4iv int (void/fp) glColor4iv ( *v -- )
libGL glColor4sv int (void/fp) glColor4sv ( *v -- )
libGL glColor4ubv int (void/fp) glColor4ubv ( *v -- )
libGL glColor4uiv int (void/fp) glColor4uiv ( *v -- )
libGL glColor4usv int (void/fp) glColor4usv ( *v -- )

libGL glTexCoord1d df (void/fp) glTexCoord1d ( ds -- )
libGL glTexCoord1f sf (void/fp) glTexCoord1f ( ds -- )
libGL glTexCoord1i int (void/fp) glTexCoord1i ( ds -- )
libGL glTexCoord1s int (void/fp) glTexCoord1s ( ds -- )

libGL glTexCoord2d df df (void/fp) glTexCoord2d ( ds dt -- )
libGL glTexCoord2f sf sf (void/fp) glTexCoord2f ( ds dt -- )
libGL glTexCoord2i int int (void/fp) glTexCoord2i ( ds dt -- )
libGL glTexCoord2s int int (void/fp) glTexCoord2s ( ds dt -- )

libGL glTexCoord3d df df df (void/fp) glTexCoord3d ( ds dt dr -- )
libGL glTexCoord3f sf sf sf (void/fp) glTexCoord3f ( ds dt dr -- )
libGL glTexCoord3i int int int (void/fp) glTexCoord3i ( ds dt dr -- )
libGL glTexCoord3s int int int (void/fp) glTexCoord3s ( ds dt dr -- )

libGL glTexCoord4d df df df df (void/fp) glTexCoord4d ( ds dt dr dq -- )
libGL glTexCoord4f sf sf sf sf (void/fp) glTexCoord4f ( ds dt dr dq -- )
libGL glTexCoord4i int int int int (void/fp) glTexCoord4i ( ds dt dr dq -- )
libGL glTexCoord4s int int int int (void/fp) glTexCoord4s ( ds dt dr dq -- )

libGL glTexCoord1dv int (void/fp) glTexCoord1dv ( *s -- )
libGL glTexCoord1fv int (void/fp) glTexCoord1fv ( *s -- )
libGL glTexCoord1iv int (void/fp) glTexCoord1iv ( *s -- )
libGL glTexCoord1sv int (void/fp) glTexCoord1sv ( *s -- )

libGL glTexCoord2dv int (void/fp) glTexCoord2dv ( *s -- )
libGL glTexCoord2fv int (void/fp) glTexCoord2fv ( *s -- )
libGL glTexCoord2iv int (void/fp) glTexCoord2iv ( *s -- )
libGL glTexCoord2sv int (void/fp) glTexCoord2sv ( *s -- )

libGL glTexCoord3dv int (void/fp) glTexCoord3dv ( *s -- )
libGL glTexCoord3fv int (void/fp) glTexCoord3fv ( *s -- )
libGL glTexCoord3iv int (void/fp) glTexCoord3iv ( *s -- )
libGL glTexCoord3sv int (void/fp) glTexCoord3sv ( *s -- )

libGL glTexCoord4dv int (void/fp) glTexCoord4dv ( *s -- )
libGL glTexCoord4fv int (void/fp) glTexCoord4fv ( *s -- )
libGL glTexCoord4iv int (void/fp) glTexCoord4iv ( *s -- )
libGL glTexCoord4sv int (void/fp) glTexCoord4sv ( *s -- )

libGL glRasterPos2d df df (void/fp) glRasterPos2d ( dx dy -- )
libGL glRasterPos2f sf sf (void/fp) glRasterPos2f ( fx fy -- )
libGL glRasterPos2i int int (void/fp) glRasterPos2i ( x y -- )
libGL glRasterPos2s int int (void/fp) glRasterPos2s ( x y -- )

libGL glRasterPos3d df df df (void/fp) glRasterPos3d ( dx dy dz -- )
libGL glRasterPos3f sf sf sf (void/fp) glRasterPos3f ( fx fy fz -- )
libGL glRasterPos3i int int int (void/fp) glRasterPos3i ( x y z -- )
libGL glRasterPos3s int int int (void/fp) glRasterPos3s ( x y z -- )

libGL glRasterPos4d df df df df (void/fp) glRasterPos4d ( dx dy dz dw -- )
libGL glRasterPos4f sf sf sf sf (void/fp) glRasterPos4f ( fx fy fz fw -- )
libGL glRasterPos4i int int int int (void/fp) glRasterPos4i ( x y z w -- )
libGL glRasterPos4s int int int int (void/fp) glRasterPos4s ( x y z w -- )

libGL glRasterPos2dv int (void/fp) glRasterPos2dv ( *v -- )
libGL glRasterPos2fv int (void/fp) glRasterPos2fv ( *v -- )
libGL glRasterPos2iv int (void/fp) glRasterPos2iv ( *v -- )
libGL glRasterPos2sv int (void/fp) glRasterPos2sv ( *v -- )

libGL glRasterPos3dv int (void/fp) glRasterPos3dv ( *v -- )
libGL glRasterPos3fv int (void/fp) glRasterPos3fv ( *v -- )
libGL glRasterPos3iv int (void/fp) glRasterPos3iv ( *v -- )
libGL glRasterPos3sv int (void/fp) glRasterPos3sv ( *v -- )

libGL glRasterPos4dv int (void/fp) glRasterPos4dv ( *v -- )
libGL glRasterPos4fv int (void/fp) glRasterPos4fv ( *v -- )
libGL glRasterPos4iv int (void/fp) glRasterPos4iv ( *v -- )
libGL glRasterPos4sv int (void/fp) glRasterPos4sv ( *v -- )

libGL glRectd df df df df (void/fp) glRectd ( dx1 dy2 dx2 dy2 -- )
libGL glRectf sf sf sf sf (void/fp) glRectf ( fx1 fy1 fx2 fy2 -- )
libGL glRecti int int int int (void/fp) glRecti ( x1 y1 x2 y2 -- )
libGL glRects int int int int (void/fp) glRects ( x1 y1 x2 y2 -- )

libGL glRectdv int (void/fp) glRectdv ( *v -- )
libGL glRectfv int (void/fp) glRectfv ( *v -- )
libGL glRectiv int (void/fp) glRectiv ( *v -- )
libGL glRectsv int (void/fp) glRectsv ( *v -- )

\ Vertex Arrays                                        04oct97py

libGLext glVertexPointer int int int int (void/fp) glVertexPointer ( s t stride ptr 
-- )
libGLext glNormalPointer int int int (void/fp) glNormalPointer ( type stride ptr -- )
libGLext glColorPointer int int int int (void/fp) glColorPointer ( s t stride ptr -- )
libGLext glIndexPointer int int int (void/fp) glIndexPointer ( type stride ptr -- )
libGLext glTexCoordPointer int int int int (void/fp) glTexCoordPointer ( s t str ptr 
-- )
libGLext glEdgeFlagPointer int int (void/fp) glEdgeFlagPointer ( stride ptr -- )
libGLext glGetPointerv int int (void/fp) glGetPointerv ( pname params -- )
libGLext glArrayElement int (void/fp) glArrayElement ( i -- )
libGLext glDrawArrays int int int (void/fp) glDrawArrays ( mode first count -- )
libGLext glDrawElements int int int int (void/fp) glDrawElements ( m c type indices 
-- )
libGLext glInterleavedArrays int int int (void/fp) glInterleavedArrays
                                      ( format stride ptr -- )

\ Lighting                                             04oct97py

libGL glShadeModel int (void/fp) glShadeModel ( mode -- )
libGL glLightf sf int int (void/fp) glLightf ( light pname fparam -- )
libGL glLighti int int int (void/fp) glLighti ( light pname param -- )
libGL glLightfv int int int (void/fp) glLightfv ( light pname *fparam -- )
libGL glLightiv int int int (void/fp) glLightiv ( light pname *param -- )
libGL glGetLightfv int int int (void/fp) glGetLightfv ( light pname *fparam -- )
libGL glGetLightiv int int int (void/fp) glGetLightiv ( light pname *param -- )
libGL glLightModelf sf int (void/fp) glLightModelf ( pname fparam -- )
libGL glLightModeli int int (void/fp) glLightModeli ( pname param -- )
libGL glLightModelfv int int (void/fp) glLightModelfv ( pname *param -- )
libGL glLightModeliv int int (void/fp) glLightModeliv ( pname *param -- )
libGL glMaterialf sf int int (void/fp) glMaterialf ( face pname fparam -- )
libGL glMateriali int int int (void/fp) glMateriali ( face pname param -- )
libGL glMaterialfv int int int (void/fp) glMaterialfv ( face pname *fparam -- )
libGL glMaterialiv int int int (void/fp) glMaterialiv ( face pname *param -- )
libGL glGetMaterialfv int int int (void/fp) glGetMaterialfv ( face pname *fp -- )
libGL glGetMaterialiv int int int (void/fp) glGetMaterialiv ( face pname *p -- )
libGL glColorMaterial int int (void/fp) glColorMaterial ( face mode -- )

\ Raster functions                                     04oct97py

libGL glPixelZoom sf sf (void/fp) glPixelZoom ( xfactor yfactor -- )
libGL glPixelStoref sf int (void/fp) glPixelStoref ( pname param -- )
libGL glPixelStorei int int (void/fp) glPixelStorei ( pname param -- )
libGL glPixelTransferf sf int (void/fp) glPixelTransferf ( pname param -- )
libGL glPixelTransferi int int (void/fp) glPixelTransferi ( pname param -- )
libGL glPixelMapfv int int int (void/fp) glPixelMapfv ( map mapsize *values -- )
libGL glPixelMapuiv int int int (void/fp) glPixelMapuiv ( map mapsize *values -- )
libGL glPixelMapusv int int int (void/fp) glPixelMapusv ( map mapsize *values -- )
libGL glGetPixelMapfv int int int (void/fp) glGetPixelMapfv ( m ms *values -- )
libGL glGetPixelMapuiv int int int (void/fp) glGetPixelMapuiv ( m ms *values -- )
libGL glGetPixelMapusv int int int (void/fp) glGetPixelMapusv ( m ms *values -- )
libGL glBitmap int int int int int int int (void/fp) glBitmap
          ( w h xorig yorig xmove ymove bitmap -- )
libGL glReadPixels int int int int int int int (void/fp) glReadPixels
          ( x y w h format type pixels -- )
libGL glDrawPixels int int int int int (void/fp) glDrawPixels ( w h format type *pixs -- )
libGL glCopyPixels int int int int int (void/fp) glCopyPixels ( x y w h type -- )

\ Stenciling                                           04oct97py

libGL glStencilFunc int int int (void/fp) glStencilFunc ( mask ref func -- )
libGL glStencilMask int (void/fp) glStencilMask ( mask -- )
libGL glStencilOp int int int (void/fp) glStencilOp ( zpass zfail fail -- )
libGL glClearStencil int (void/fp) glClearStencil ( s -- )

\ Texture Mapping                                      04oct97py

libGL glTexGend df int int (void/fp) glTexGend ( coord pname dparam -- )
libGL glTexGenf sf int int (void/fp) glTexGenf ( coord pname fparam -- )
libGL glTexGeni int int int (void/fp) glTexGeni ( coord pname param -- )
libGL glTexGendv int int int (void/fp) glTexGendv ( coord pname *param -- )
libGL glTexGenfv int int int (void/fp) glTexGenfv ( coord pname *param -- )
libGL glTexGeniv int int int (void/fp) glTexGeniv ( coord pname *param -- )
libGL glGetTexGendv int int int (void/fp) glGetTexGendv ( coord pname *param -- )
libGL glGetTexGenfv int int int (void/fp) glGetTexGenfv ( coord pname *param -- )
libGL glGetTexGeniv int int int (void/fp) glGetTexGeniv ( coord pname *param -- )
libGL glTexEnvf sf int int (void/fp) glTexEnvf ( target pname fparam -- )
libGL glTexEnvi int int int (void/fp) glTexEnvi ( target pname param -- )
libGL glTexEnvfv int int int (void/fp) glTexEnvfv ( target pname *fparam -- )
libGL glTexEnviv int int int (void/fp) glTexEnviv ( target pname *param -- )
libGL glGetTexEnvfv int int int (void/fp) glGetTexEnvfv ( target pname *param -- )
libGL glGetTexEnviv int int int (void/fp) glGetTexEnviv ( target pname *param -- )
libGL glTexParameterf sf int int (void/fp) glTexParameterf ( target pname fparam -- )
libGL glTexParameteri int int int (void/fp) glTexParameteri ( target pname param -- )
libGL glTexParameterfv int int int (void/fp) glTexParameterfv ( target pname *fparam -- )
libGL glTexParameteriv int int int (void/fp) glTexParameteriv ( target pname *param -- )
libGL glGetTexParameterfv int int int (void/fp) glGetTexParameterfv ( target pname *fparam -- )
libGL glGetTexParameteriv int int int (void/fp) glGetTexParameteriv ( target pname *param -- )

libGL glGetTexLevelParameterfv int int int int (void/fp) glGetTexLevelParameterfv
           ( target level pname *params -- )
libGL glGetTexLevelParameteriv int int int int (void/fp) glGetTexLevelParameteriv
           ( target level pname *params -- )
libGL glTexImage1D int int int int int int int int (void/fp) glTexImage1D
           ( target level component width border format type *pixels -- )
libGL glTexImage2D int int int int int int int int int (void/fp) glTexImage2D
           ( target level component width height border format type *pixels -- )
libGL glGetTexImage int int int int int (void/fp) glGetTexImage
           ( target level format type *pixels -- )
libGLext glGenTextures int int (void/fp) glGenTextures ( n *textures -- )
libGLext glDeleteTextures int int (void/fp) glDeleteTextures ( n *textures -- )
libGLext glBindTexture int int (void/fp) glBindTexture ( target textures -- )
libGLext glPrioritizeTextures int int int (void/fp) glPrioritizeTextures
           ( n *textures *priorities -- )
libGLext glAreTexturesResident int int int (int/fp) glAreTexturesResident
           ( n *textures *residences -- flag )
libGLext glIsTexture int (int/fp) glIsTexture ( texture -- flag )
libGLext glTexSubImage1D int int int int int int int (void/fp) glTexSubImage1D
           ( target level xoffset width format type *pixels -- )
libGLext glTexSubImage2D int int int int int int int int int (void/fp) glTexSubImage2D
           ( target level xoffset yoffset widht height format type *pixels -- )
libGLext glCopyTexImage1D int int int int int int int (void/fp) glCopyTexImage1D
           ( target level intformat x y w border -- )
libGLext glCopyTexImage2D int int int int int int int int (void/fp) glCopyTexImage2D
           ( target level intformat x y w h border -- )
libGLext glCopyTexSubImage1D int int int int int int (void/fp) glCopyTexSubImage1D
           ( target level xoffset x y w -- )
libGLext glCopyTexSubImage2D int int int int int int int int (void/fp) glCopyTexSubImage2D
           ( target level xoffset yoffset x y w h -- )

\ Evaluators                                           04oct97py

libGL glMap1d int int int df df int (void/fp) glMap1d ( t du1 du2 stride order *points -- )
libGL glMap1f int int int sf sf int (void/fp) glMap1f ( t fu1 fu2 stride order *points -- )
libGL glMap2d int int int df df int int df df int (void/fp) glMap2d
           ( *points vorder vstride dv2 dv1 uorder ustride du2 du1 target -- )
libGL glMap2f int int int sf sf int int sf sf int (void/fp) glMap2f
           ( *points vorder vstride fv2 fv1 uorder ustride fu2 fu1 target -- )
libGL glGetMapdv int int int (void/fp) glGetMapdv ( target query *v -- )
libGL glGetMapfv int int int (void/fp) glGetMapfv ( target query *v -- )
libGL glGetMapiv int int int (void/fp) glGetMapiv ( target query *v -- )
libGL glEvalCoord1d df (void/fp) glEvalCoord1d ( du -- )
libGL glEvalCoord1f sf (void/fp) glEvalCoord1f ( fu -- )
libGL glEvalCoord1dv int (void/fp) glEvalCoord1dv ( *du -- )
libGL glEvalCoord1fv int (void/fp) glEvalCoord1fv ( *fu -- )
libGL glEvalCoord2d df df (void/fp) glEvalCoord2d ( du dv -- )
libGL glEvalCoord2f sf sf (void/fp) glEvalCoord2f ( fu fv -- )
libGL glEvalCoord2dv int (void/fp) glEvalCoord2dv ( *du -- )
libGL glEvalCoord2fv int (void/fp) glEvalCoord2fv ( *fu -- )
libGL glMapGrid1d df df int (void/fp) glMapGrid1d ( un du1 du2 -- )
libGL glMapGrid1f sf sf int (void/fp) glMapGrid1f ( un fu1 fu2 -- )
libGL glMapGrid2d df df int df df int (void/fp) glMapGrid2d ( un du1 du2 vn dv1 dv2 -- )
libGL glMapGrid2f sf sf int sf sf int (void/fp) glMapGrid2f ( fv2 fv1 vn fu2 fu1 un -- )
libGL glEvalPoint1 int (void/fp) glEvalPoint1 ( i -- )
libGL glEvalPoint2 int int (void/fp) glEvalPoint2 ( i j -- )
libGL glEvalMesh1 int int int (void/fp) glEvalMesh1 ( mode i1 i2 -- )
libGL glEvalMesh2 int int int int int (void/fp) glEvalMesh2 ( mode i1 i2 j1 j2 -- )

\ Fog                                                  04oct97py

libGL glFogf sf int (void/fp) glFogf ( pname param -- )
libGL glFogi int int (void/fp) glFogi ( pname param -- )
libGL glFogfv int int (void/fp) glFogfv ( pname *param -- )
libGL glFogiv int int (void/fp) glFogiv ( pname *param -- )

\ Selection and Feedback                               04oct97py

libGL glFeedbackBuffer int int int (void/fp) glFeedbackBuffer
                      ( size type *buffer -- )
libGL glPassThrough int (void/fp) glPassThrough ( token -- )
libGL glSelectBuffer int int (void/fp) glSelectBuffer ( size *buffer -- )
libGL glInitNames (void/fp) glInitNames ( -- )
libGL glLoadName int (void/fp) glLoadName ( name -- )
libGL glPushName int (void/fp) glPushName ( name -- )
libGL glPopName (void/fp) glPopName ( -- )

[IFDEF] MESA \ x11
libGLext glBlendEquationEXT int (void/fp) glBlendEquationEXT ( mode -- )
libGLext sf sf sf sf (void/fp) glBlendColorEXT glBlendColorEXT ( a b g r -- )
libGLext sf sf (void/fp) glPolygonOffsetEXT glPolygonOffsetEXT
                        ( bias factor -- )
[THEN]

\ GL_MESA_window_pos                                   04oct97py

[IFDEF] x11

[IFDEF] MESA
libGL glWindowPos2iMESA int int (void/fp) glWindowPos2iMESA ( x y -- )
libGL glWindowPos2sMESA int int (void/fp) glWindowPos2sMESA ( x y -- )
libGL glWindowPos2fMESA sf sf (void/fp) glWindowPos2fMESA ( fx fy -- )
libGL glWindowPos2dMESA df df (void/fp) glWindowPos2dMESA ( dx dy -- )
libGL glWindowPos2ivMESA int (void/fp) glWindowPos2ivMESA ( *v -- )
libGL glWindowPos2svMESA int (void/fp) glWindowPos2svMESA ( *v -- )
libGL glWindowPos2fvMESA int (void/fp) glWindowPos2fvMESA ( *v -- )
libGL glWindowPos2dvMESA int (void/fp) glWindowPos2dvMESA ( *v -- )
libGL glWindowPos3iMESA int int int (void/fp) glWindowPos3iMESA ( x y z -- )
libGL glWindowPos3sMESA int int int (void/fp) glWindowPos3sMESA ( x y z -- )
libGL glWindowPos3fMESA sf sf sf (void/fp) glWindowPos3fMESA ( fx fy fz -- )
libGL glWindowPos3dMESA df df df (void/fp) glWindowPos3dMESA ( dx dy dz -- )
libGL glWindowPos3ivMESA int (void/fp) glWindowPos3ivMESA ( *v -- )
libGL glWindowPos3svMESA int (void/fp) glWindowPos3svMESA ( *v -- )
libGL glWindowPos3fvMESA int (void/fp) glWindowPos3fvMESA ( *v -- )
libGL glWindowPos3dvMESA int (void/fp) glWindowPos3dvMESA ( *v -- )
libGL glWindowPos4iMESA int int int int (void/fp) glWindowPos4iMESA ( x y z w -- )
libGL glWindowPos4sMESA int int int int (void/fp) glWindowPos4sMESA ( x y z w -- )
libGL glWindowPos4fMESA sf sf sf sf (void/fp) glWindowPos4fMESA ( fx fy fz fw -- )
libGL glWindowPos4dMESA df df df df (void/fp) glWindowPos4dMESA ( dx dy dz dw -- )
libGL glWindowPos4ivMESA int (void/fp) glWindowPos4ivMESA ( *v -- )
libGL glWindowPos4svMESA int (void/fp) glWindowPos4svMESA ( *v -- )
libGL glWindowPos4fvMESA int (void/fp) glWindowPos4fvMESA ( *v -- )
libGL glWindowPos4dvMESA int (void/fp) glWindowPos4dvMESA ( *v -- )

\ GL_MESA_resize_buffers                               04oct97py

libGL glResizeBuffersMESA (void/fp) glResizeBuffersMESA ( -- )

\ XMesa special extensions

\ 3 libGL XMesaGetBackBuffer XMesaGetBackBuffer ( xim* pixmap* b -- status )
\ 2 libGL XMesaFindBuffer XMesaFindBuffer ( d dpy -- buffer )
[THEN]

[THEN]

\ glX calls                                            04oct97py

[IFDEF] x11
libGL glXChooseVisual int int int (int/fp) glXChooseVisual
                        ( dpy screen attriblist -- XVisualInfo )
libGL glXCreateContext int int int int (int) glXCreateContext
                        ( dpy vis shareList direct -- GLXContext )
libGL glXDestroyContext int int (void) glXDestroyContext ( dpy ctx -- )
libGL glXMakeCurrent int int int (int) glXMakeCurrent
                        ( dpy drawable ctx -- flag )
libGL glXCopyContext int int int int (void) glXCopyContext ( dpy src dst mask -- )
libGL glXSwapBuffers int int (void) glXSwapBuffers ( dpy drawable -- )
libGL glXCreateGLXPixmap int int int (int) glXCreateGLXPixmap
                              ( dpy visual pixmap -- GLXPixmap )
libGL glXDestroyGLXPixmap int int (void) glXDestroyGLXPixmap
                                             ( dpy pixmap -- )
libGL glXQueryExtension int int int (int) glXQueryExtension
                                    ( dpy errorb event -- flag )
libGL glXQueryVersion int int int (int) glXQueryVersion ( dpy maj min -- flag )
libGL glXIsDirect int int (int) glXIsDirect ( dpy ctx -- flag )
libGL glXGetConfig int int int int (void) glXGetConfig ( dpy visual attrib value -- )
libGL glXGetCurrentContext (int) glXGetCurrentContext
                                               ( -- GLXContext )
libGL glXGetCurrentDrawable (int) glXGetCurrentDrawable
                                              ( -- GLXDrawable )
libGL glXWaitGL (void/fp) glXWaitGL ( -- )
libGL glXWaitX  (void/fp) glXWaitX ( -- )
libGL glXUseXFont int int int int (void/fp) glXUseXFont ( font first count list -- )
libGL glXQueryExtensionsString int int (int) glXQueryExtensionsString
                                        ( dpy screen -- string )
libGL glXQueryServerString int int int (int) glXQueryServerString
                                   ( dpy screen name -- string )
libGL glXGetClientString int int (int) glXGetClientString
                                          ( name dpy -- string )
\ 4 libGL glXCreateGLXPixmapMESA glXCreateGLXPixmapMESA
                         ( cmap pixmap visual dpy -- GLXPixmap )
\ 2 libGL glXReleaseBuffersMESA glXReleaseBuffersMESA
                                               ( w dpy -- flag )
[THEN]

[IFDEF] libGLU
    libGLU gluNewTess (ptr) gluNewTess ( -- tess )
    libGLU gluDeleteTess ptr (void) gluDeleteTess ( tess -- )
    libGLU gluTessBeginContour ptr (void) gluTessBeginContour ( tess -- )
    libGLU gluTessBeginPolygon ptr ptr (void) gluTessBeginPolygon ( tess data -- )
    libGLU gluTessCallback ptr int ptr (void) gluTessCallback ( tess which cfun -- )
    libGLU gluTessEndContour ptr (void) gluTessEndContour ( tess -- )
    libGLU gluTessEndPolygon ptr (void) gluTessEndPolygon ( tess -- )
    libGLU gluTessNormal df df df ptr (void/fp) gluTessNormal ( tess rx ry rz -- )
    libGLU gluTessProperty df int ptr (void/fp) gluTessProperty ( tess wich rdata -- )
    libGLU gluTessVertex ptr ptr ptr (void) gluTessVertex ( tess loc data -- )
[THEN]

legacy on

[IFDEF] win32
win32api also
struct{
  2 nSize
  2 nVersion
  cell dwFlags
  1 iPixelType
  1 cColorBits
  1 cRedBits
  1 cRedShift
  1 cGreenBits
  1 cGreenShift
  1 cBlueBits
  1 cBlueShift
  1 cAlphaBits
  1 cAlphaShift
  1 cAccumBits
  1 cAccumRedBits
  1 cAccumGreenBits
  1 cAccumBlueBits
  1 cAccumAlphaBits
  1 cDepthBits
  1 cStencilBits
  1 cAuxBuffers
  1 iLayerType
  1 bReserved
  cell dwLayerMask
  cell dwVisibleMask
  cell dwDamageMask
} PIXELFORMATDESCRIPTOR
\ ChoosePixelFormat                                    15oct97py
$4 Constant PFD_DRAW_TO_WINDOW
$8 Constant PFD_DRAW_TO_BITMAP
$10 Constant PFD_SUPPORT_GDI
$20 Constant PFD_SUPPORT_OPENGL
$1 Constant PFD_DOUBLEBUFFER
$2 Constant PFD_STEREO
$40000000 Constant PFD_DOUBLEBUFFER_DONTCARE
$80000000 Constant PFD_STEREO_DONTCARE
0 Constant PFD_TYPE_RGBA
1 Constant PFD_TYPE_COLORINDEX
0 Constant PFD_MAIN_PLANE
1 Constant PFD_OVERLAY_PLANE
-1 Constant PFD_UNDERLAY_PLANE
 
\ wglUseFontOutlines                                   15oct97py
0 Constant WGL_FONT_LINES
1 Constant WGL_FONT_POLYGONS

\ PIXELFORMATDESCRIPTOR structure                      15oct97py
$40 Constant PFD_GENERIC_FORMAT
$80 Constant PFD_NEED_PALETTE
$100 Constant PFD_NEED_SYSTEM_PALETTE
$400 Constant PFD_SWAP_COPY
$200 Constant PFD_SWAP_EXCHANGE

2 gdi32 ChoosePixelFormat ChoosePixelFormat
                        ( ppfd hdc -- n )
3 gdi32 SetPixelFormat SetPixelFormat
                        ( ppfd n hdc -- n )
1 libGL wglCreateContext wglCreateContext
                      ( hdc -- GLXContext )
1 libGL wglDeleteContext wglDeleteContext ( ctx -- )
2 libGL wglMakeCurrent wglMakeCurrent
                                    ( ctx hdc -- flag )
1 gdi32 SwapBuffers SwapBuffers ( drawable -- )
6 gdi32 CreateDIBitmap CreateDIBitmap
                              ( ... -- GLXPixmap )
6 gdi32 CreateDIBSection CreateDIBSection
                              ( ... -- GLXPixmap )
4 gdi32 DescribePixelFormat DescribePixelFormat
                                ( ... -- )
0 libGL wglGetCurrentContext wglGetCurrentContext
                                               ( -- GLXContext )
0 libGL wglGetCurrentDC wglGetCurrentDC
                                              ( -- GLXDrawable )
0 libGL wglUseFontOutlines wglUseFontOutlinesA
0 libGL wglUseFontBitmaps wglUseFontBitmapsA
previous
[THEN]

\ GLU functions                                        01nov97py

\ 7 libGLU gluBuild2DMipmaps gluBuild2DMipmaps
         ( data type format height width comps target -- int )

previous

Module;
