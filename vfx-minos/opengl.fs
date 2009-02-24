\ Mesa Open GL library interface                       04oct97py

Vocabulary OpenGL

also OpenGL definitions

library: libGL.so.1
library: libGLU.so.1

: ?texture  true ; immediate

: !gl-lib ( "name" -- )
    bl word drop ;

\ functions                                            04oct97py

LocalExtern: glClearIndex void glClearIndex( float ); ( f -- )
LocalExtern: glClearColor void glClearColor( float , float , float , float ); ( r g b a -- )
LocalExtern: glClear void glClear( int ); ( mask -- )
LocalExtern: glIndexMask void glIndexMask( int ); ( mask -- )
LocalExtern: glColorMask void glColorMask( int , int , int , int ); ( r g b a -- )
LocalExtern: glAlphaFunc void glAlphaFunc( float , int ); ( f r -- )
LocalExtern: glBlendFunc void glBlendFunc( int , int ); ( d s -- )
LocalExtern: glLogicOp void glLogicOp( int ); ( opcode -- )
LocalExtern: glCullFace void glCullFace( int ); ( mode -- )
LocalExtern: glFrontFace void glFrontFace( int ); ( mode -- )
LocalExtern: glPointSize void glPointSize( float ); ( sf -- )
LocalExtern: glLineWidth void glLineWidth( float ); ( wf -- )
LocalExtern: glLineStipple void glLineStipple( int , int ); ( factor pattern -- )
LocalExtern: glPolygonMode void glPolygonMode( int , int ); ( face mode -- )
LocalExtern: glPolygonOffset void glPolygonOffset( float , float ); ( funits ffactor -- )
LocalExtern: glPolygonStipple void glPolygonStipple( int ); ( *mask -- )
LocalExtern: glGetPolygonStipple void glGetPolygonStipple( int ); ( *mask -- )
LocalExtern: glEdgeFlag void glEdgeFlag( int ); ( flag -- )
LocalExtern: glEdgeFlagv void glEdgeFlagv( int ); ( *flag -- )
LocalExtern: glScissor void glScissor( int , int , int , int ); ( x y w h -- )
LocalExtern: glClipPlane void glClipPlane( int , int ); ( plane *equation -- )
LocalExtern: glGetClipPlane void glGetClipPlane( int , int ); ( plane *eqation -- )
LocalExtern: glDrawBuffer void glDrawBuffer( int ); ( mode -- )
LocalExtern: glReadBuffer void glReadBuffer( int ); ( mode -- )
LocalExtern: glEnable void glEnable( int ); ( cap -- )
LocalExtern: glDisable void glDisable( int ); ( cap -- )
LocalExtern: glIsEnabled int glIsEnabled( int ); ( cap -- flag )
LocalExtern: glEnableClientState void glEnableClientState( int ); ( cap -- )
LocalExtern: glDisableClientState void glDisableClientState( int ); ( cap -- )
LocalExtern: glGetBooleanv void glGetBooleanv( int , int ); ( pname params -- )
LocalExtern: glGetDoublev void glGetDoublev( int , int ); ( pname params -- )
LocalExtern: glGetFloatv void glGetFloatv( int , int ); ( pname params -- )
LocalExtern: glGetIntegerv void glGetIntegerv( int , int ); ( pname params -- )
LocalExtern: glPushAttrib void glPushAttrib( int ); ( mask -- )
LocalExtern: glPopAttrib void glPopAttrib( void ); ( -- )
LocalExtern: glPushClientAttrib void glPushClientAttrib( int ); ( mask -- )
LocalExtern: glPopClientAttrib void glPopClientAttrib( void ); ( -- )
LocalExtern: glRenderMode void glRenderMode( int ); ( mode -- n )
LocalExtern: glGetError void glGetError( void ); ( -- n )
LocalExtern: glGetString void glGetString( int ); ( name -- string )
LocalExtern: glFinish void glFinish( void ); ( -- )
LocalExtern: glFlush void glFlush( void ); ( -- )
LocalExtern: glHint void glHint( int , int ); ( target mode -- )

\ Depth Buffer                                         04oct97py

LocalExtern: glClearDepth void glClearDepth( double ); ( ddepth -- )
LocalExtern: glDepthFunc void glDepthFunc( int ); ( func -- )
LocalExtern: glDepthMask void glDepthMask( int ); ( flag -- )
LocalExtern: glDepthRange void glDepthRange( double , double ); ( dnear dfar -- )

\ Accumulation Buffer                                  04oct97py

LocalExtern: glClearAccum void glClearAccum( float , float , float , float ); ( fr fg fb fa -- )
LocalExtern: glAccum void glAccum( int , float ); ( op fvalue -- )

\ Transformation                                       04oct97py

LocalExtern: glMatrixMode void glMatrixMode( int ); ( mode -- )
LocalExtern: glOrtho void glOrtho( double , double , double , double , double , double );                   ( dleft dright dbottom dtop dnear doublear -- )
LocalExtern: glFrustum void glFrustum( double , double , double , double , double , double );                   ( dleft dright dbottom dtop dnear dfar -- )
LocalExtern: glViewport void glViewport( int , int , int , int ); ( x y w h -- )
LocalExtern: glPushMatrix void glPushMatrix( void ); ( -- )
LocalExtern: glPopMatrix void glPopMatrix( void ); ( -- )
LocalExtern: glLoadIdentity void glLoadIdentity( void ); ( -- )
LocalExtern: glLoadMatrixd void glLoadMatrixd( int ); ( *m -- )
LocalExtern: glLoadMatrixf void glLoadMatrixf( int ); ( *m -- )
LocalExtern: glMultMatrixd void glMultMatrixd( int ); ( *m -- )
LocalExtern: glMultMatrixf void glMultMatrixf( int ); ( *m -- )
LocalExtern: glRotated void glRotated( double , double , double , double ); ( da dx dy dz -- )
LocalExtern: glRotatef void glRotatef( float , float , float , float ); ( fa fx fy fz -- )
LocalExtern: glScaled void glScaled( double , double , double ); ( dx dy dz -- )
LocalExtern: glScalef void glScalef( float , float , float ); ( fx fy fz -- )
LocalExtern: glTranslated void glTranslated( double , double , double ); ( dx dy dz -- )
LocalExtern: glTranslatef void glTranslatef( float , float , float ); ( fx fy fz -- )

\ Display Lists                                        04oct97py

LocalExtern: glIsList int glIsList( int ); ( list -- flag )
LocalExtern: glDeleteLists void glDeleteLists( int , int ); ( range list -- )
LocalExtern: glGenLists int glGenLists( int ); ( range -- u )
LocalExtern: glNewList void glNewList( int , int ); ( list mode -- )
LocalExtern: glEndList void glEndList( void ); ( -- )
LocalExtern: glCallList void glCallList( int ); ( list -- )
LocalExtern: glCallLists void glCallLists( int , int , int ); ( n type *lists -- )
LocalExtern: glListBase void glListBase( int ); ( base -- )

\ Drawing Functions                                    04oct97py

LocalExtern: glBegin void glBegin( int ); ( mode -- )
LocalExtern: glEnd void glEnd( void ); ( -- )

LocalExtern: glVertex2d void glVertex2d( double , double ); ( dx dy -- )
LocalExtern: glVertex2f void glVertex2f( float , float ); ( fx fy -- )
LocalExtern: glVertex2i void glVertex2i( int , int ); ( x y -- )
LocalExtern: glVertex2s void glVertex2s( int , int ); ( x y -- )

LocalExtern: glVertex3d void glVertex3d( double , double , double ); ( dx dy dz -- )
LocalExtern: glVertex3f void glVertex3f( float , float , float ); ( fx fy fz -- )
LocalExtern: glVertex3i void glVertex3i( int , int , int ); ( x y z -- )
LocalExtern: glVertex3s void glVertex3s( int , int , int ); ( x y z -- )

LocalExtern: glVertex4d void glVertex4d( double , double , double , double ); ( fx fy fz fw -- )
LocalExtern: glVertex4f void glVertex4f( float , float , float , float ); ( fx fy fz fw -- )
LocalExtern: glVertex4i void glVertex4i( int , int , int , int ); ( x y z w -- )
LocalExtern: glVertex4s void glVertex4s( int , int , int , int ); ( x y z w -- )

LocalExtern: glVertex2dv void glVertex2dv( int ); ( *v -- )
LocalExtern: glVertex2fv void glVertex2fv( int ); ( *v -- )
LocalExtern: glVertex2iv void glVertex2iv( int ); ( *v -- )
LocalExtern: glVertex2sv void glVertex2sv( int ); ( *v -- )

LocalExtern: glVertex3dv void glVertex3dv( int ); ( *v -- )
LocalExtern: glVertex3fv void glVertex3fv( int ); ( *v -- )
LocalExtern: glVertex3iv void glVertex3iv( int ); ( *v -- )
LocalExtern: glVertex3sv void glVertex3sv( int ); ( *v -- )

LocalExtern: glVertex4dv void glVertex4dv( int ); ( *v -- )
LocalExtern: glVertex4fv void glVertex4fv( int ); ( *v -- )
LocalExtern: glVertex4iv void glVertex4iv( int ); ( *v -- )
LocalExtern: glVertex4sv void glVertex4sv( int ); ( *v -- )

LocalExtern: glNormal3b void glNormal3b( int , int , int ); ( nx ny nz -- )
LocalExtern: glNormal3d void glNormal3d( double , double , double ); ( dnx dny dnz -- )
LocalExtern: glNormal3f void glNormal3f( float , float , float ); ( nx ny nz -- )
LocalExtern: glNormal3i void glNormal3i( int , int , int ); ( nx ny nz -- )
LocalExtern: glNormal3s void glNormal3s( int , int , int ); ( nx ny nz -- )

LocalExtern: glNormal3bv void glNormal3bv( int ); ( *v -- )
LocalExtern: glNormal3dv void glNormal3dv( int ); ( *v -- )
LocalExtern: glNormal3fv void glNormal3fv( int ); ( *v -- )
LocalExtern: glNormal3iv void glNormal3iv( int ); ( *v -- )
LocalExtern: glNormal3sv void glNormal3sv( int ); ( *v -- )

LocalExtern: glIndexd void glIndexd( double ); ( dc -- )
LocalExtern: glIndexf void glIndexf( float ); ( fc -- )
LocalExtern: glIndexi void glIndexi( int ); ( c -- )
LocalExtern: glIndexs void glIndexs( int ); ( c -- )
LocalExtern: glIndexub void glIndexub( int ); ( c -- )

LocalExtern: glIndexdv void glIndexdv( int ); ( *c -- )
LocalExtern: glIndexfv void glIndexfv( int ); ( *c -- )
LocalExtern: glIndexiv void glIndexiv( int ); ( *c -- )
LocalExtern: glIndexsv void glIndexsv( int ); ( *c -- )
LocalExtern: glIndexubv void glIndexubv( int ); ( *c -- )

LocalExtern: glColor3b void glColor3b( int , int , int ); ( r g b -- )
LocalExtern: glColor3d void glColor3d( double , double , double ); ( r g b -- )
LocalExtern: glColor3f void glColor3f( float , float , float ); ( r g b -- )
LocalExtern: glColor3i void glColor3i( int , int , int ); ( r g b -- )
LocalExtern: glColor3s void glColor3s( int , int , int ); ( r g b -- )
LocalExtern: glColor3ub void glColor3ub( int , int , int ); ( r g b -- )
LocalExtern: glColor3ui void glColor3ui( int , int , int ); ( r g b -- )
LocalExtern: glColor3us void glColor3us( int , int , int ); ( r g b -- )

LocalExtern: glColor4b void glColor4b( int , int , int , int ); ( r g b a -- )
LocalExtern: glColor4d void glColor4d( double , double , double , double ); ( r g b a -- )
LocalExtern: glColor4f void glColor4f( float , float , float , float ); ( r g b a -- )
LocalExtern: glColor4i void glColor4i( int , int , int , int ); ( r g b a -- )
LocalExtern: glColor4s void glColor4s( int , int , int , int ); ( r g b a -- )
LocalExtern: glColor4ub void glColor4ub( int , int , int , int ); ( r g b a -- )
LocalExtern: glColor4ui void glColor4ui( int , int , int , int ); ( r g b a -- )
LocalExtern: glColor4us void glColor4us( int , int , int , int ); ( r g b a -- )

LocalExtern: glColor3bv void glColor3bv( int ); ( *v -- )
LocalExtern: glColor3dv void glColor3dv( int ); ( *v -- )
LocalExtern: glColor3fv void glColor3fv( int ); ( *v -- )
LocalExtern: glColor3iv void glColor3iv( int ); ( *v -- )
LocalExtern: glColor3sv void glColor3sv( int ); ( *v -- )
LocalExtern: glColor3ubv void glColor3ubv( int ); ( *v -- )
LocalExtern: glColor3uiv void glColor3uiv( int ); ( *v -- )
LocalExtern: glColor3usv void glColor3usv( int ); ( *v -- )

LocalExtern: glColor4bv void glColor4bv( int ); ( *v -- )
LocalExtern: glColor4dv void glColor4dv( int ); ( *v -- )
LocalExtern: glColor4fv void glColor4fv( int ); ( *v -- )
LocalExtern: glColor4iv void glColor4iv( int ); ( *v -- )
LocalExtern: glColor4sv void glColor4sv( int ); ( *v -- )
LocalExtern: glColor4ubv void glColor4ubv( int ); ( *v -- )
LocalExtern: glColor4uiv void glColor4uiv( int ); ( *v -- )
LocalExtern: glColor4usv void glColor4usv( int ); ( *v -- )

LocalExtern: glTexCoord1d void glTexCoord1d( double ); ( ds -- )
LocalExtern: glTexCoord1f void glTexCoord1f( float ); ( ds -- )
LocalExtern: glTexCoord1i void glTexCoord1i( int ); ( ds -- )
LocalExtern: glTexCoord1s void glTexCoord1s( int ); ( ds -- )

LocalExtern: glTexCoord2d void glTexCoord2d( double , double ); ( ds dt -- )
LocalExtern: glTexCoord2f void glTexCoord2f( float , float ); ( ds dt -- )
LocalExtern: glTexCoord2i void glTexCoord2i( int , int ); ( ds dt -- )
LocalExtern: glTexCoord2s void glTexCoord2s( int , int ); ( ds dt -- )

LocalExtern: glTexCoord3d void glTexCoord3d( double , double , double ); ( ds dt dr -- )
LocalExtern: glTexCoord3f void glTexCoord3f( float , float , float ); ( ds dt dr -- )
LocalExtern: glTexCoord3i void glTexCoord3i( int , int , int ); ( ds dt dr -- )
LocalExtern: glTexCoord3s void glTexCoord3s( int , int , int ); ( ds dt dr -- )

LocalExtern: glTexCoord4d void glTexCoord4d( double , double , double , double ); ( ds dt dr dq -- )
LocalExtern: glTexCoord4f void glTexCoord4f( float , float , float , float ); ( ds dt dr dq -- )
LocalExtern: glTexCoord4i void glTexCoord4i( int , int , int , int ); ( ds dt dr dq -- )
LocalExtern: glTexCoord4s void glTexCoord4s( int , int , int , int ); ( ds dt dr dq -- )

LocalExtern: glTexCoord1dv void glTexCoord1dv( int ); ( *s -- )
LocalExtern: glTexCoord1fv void glTexCoord1fv( int ); ( *s -- )
LocalExtern: glTexCoord1iv void glTexCoord1iv( int ); ( *s -- )
LocalExtern: glTexCoord1sv void glTexCoord1sv( int ); ( *s -- )

LocalExtern: glTexCoord2dv void glTexCoord2dv( int ); ( *s -- )
LocalExtern: glTexCoord2fv void glTexCoord2fv( int ); ( *s -- )
LocalExtern: glTexCoord2iv void glTexCoord2iv( int ); ( *s -- )
LocalExtern: glTexCoord2sv void glTexCoord2sv( int ); ( *s -- )

LocalExtern: glTexCoord3dv void glTexCoord3dv( int ); ( *s -- )
LocalExtern: glTexCoord3fv void glTexCoord3fv( int ); ( *s -- )
LocalExtern: glTexCoord3iv void glTexCoord3iv( int ); ( *s -- )
LocalExtern: glTexCoord3sv void glTexCoord3sv( int ); ( *s -- )

LocalExtern: glTexCoord4dv void glTexCoord4dv( int ); ( *s -- )
LocalExtern: glTexCoord4fv void glTexCoord4fv( int ); ( *s -- )
LocalExtern: glTexCoord4iv void glTexCoord4iv( int ); ( *s -- )
LocalExtern: glTexCoord4sv void glTexCoord4sv( int ); ( *s -- )

LocalExtern: glRasterPos2d void glRasterPos2d( double , double ); ( dx dy -- )
LocalExtern: glRasterPos2f void glRasterPos2f( float , float ); ( fx fy -- )
LocalExtern: glRasterPos2i void glRasterPos2i( int , int ); ( x y -- )
LocalExtern: glRasterPos2s void glRasterPos2s( int , int ); ( x y -- )

LocalExtern: glRasterPos3d void glRasterPos3d( double , double , double ); ( dx dy dz -- )
LocalExtern: glRasterPos3f void glRasterPos3f( float , float , float ); ( fx fy fz -- )
LocalExtern: glRasterPos3i void glRasterPos3i( int , int , int ); ( x y z -- )
LocalExtern: glRasterPos3s void glRasterPos3s( int , int , int ); ( x y z -- )

LocalExtern: glRasterPos4d void glRasterPos4d( double , double , double , double ); ( dx dy dz dw -- )
LocalExtern: glRasterPos4f void glRasterPos4f( float , float , float , float ); ( fx fy fz fw -- )
LocalExtern: glRasterPos4i void glRasterPos4i( int , int , int , int ); ( x y z w -- )
LocalExtern: glRasterPos4s void glRasterPos4s( int , int , int , int ); ( x y z w -- )

LocalExtern: glRasterPos2dv void glRasterPos2dv( int ); ( *v -- )
LocalExtern: glRasterPos2fv void glRasterPos2fv( int ); ( *v -- )
LocalExtern: glRasterPos2iv void glRasterPos2iv( int ); ( *v -- )
LocalExtern: glRasterPos2sv void glRasterPos2sv( int ); ( *v -- )

LocalExtern: glRasterPos3dv void glRasterPos3dv( int ); ( *v -- )
LocalExtern: glRasterPos3fv void glRasterPos3fv( int ); ( *v -- )
LocalExtern: glRasterPos3iv void glRasterPos3iv( int ); ( *v -- )
LocalExtern: glRasterPos3sv void glRasterPos3sv( int ); ( *v -- )

LocalExtern: glRasterPos4dv void glRasterPos4dv( int ); ( *v -- )
LocalExtern: glRasterPos4fv void glRasterPos4fv( int ); ( *v -- )
LocalExtern: glRasterPos4iv void glRasterPos4iv( int ); ( *v -- )
LocalExtern: glRasterPos4sv void glRasterPos4sv( int ); ( *v -- )

LocalExtern: glRectd void glRectd( double , double , double , double ); ( dx1 dy2 dx2 dy2 -- )
LocalExtern: glRectf void glRectf( float , float , float , float ); ( fx1 fy1 fx2 fy2 -- )
LocalExtern: glRecti void glRecti( int , int , int , int ); ( x1 y1 x2 y2 -- )
LocalExtern: glRects void glRects( int , int , int , int ); ( x1 y1 x2 y2 -- )

LocalExtern: glRectdv void glRectdv( int ); ( *v -- )
LocalExtern: glRectfv void glRectfv( int ); ( *v -- )
LocalExtern: glRectiv void glRectiv( int ); ( *v -- )
LocalExtern: glRectsv void glRectsv( int ); ( *v -- )

\ Vertex Arrays                                        04oct97py

LocalExtern: glVertexPointer void glVertexPointer( int , int , int , int ); ( s t stride ptr -- )
LocalExtern: glNormalPointer void glNormalPointer( int , int , int ); ( type stride ptr -- )
LocalExtern: glColorPointer void glColorPointer( int , int , int , int ); ( s t stride ptr -- )
LocalExtern: glIndexPointer void glIndexPointer( int , int , int ); ( type stride ptr -- )
LocalExtern: glTexCoordPointer void glTexCoordPointer( int , int , int , int ); ( s t str ptr -- )
LocalExtern: glEdgeFlagPointer void glEdgeFlagPointer( int , int ); ( stride ptr -- )
LocalExtern: glGetPointerv void glGetPointerv( int , int ); ( pname params -- )
LocalExtern: glArrayElement void glArrayElement( int ); ( i -- )
LocalExtern: glDrawArrays void glDrawArrays( int , int , int ); ( mode first count -- )
LocalExtern: glDrawElements void glDrawElements( int , int , int , int ); ( m c type indices -- )
LocalExtern: glInterleavedArrays void glInterleavedArrays( int , int , int );                                      ( format stride ptr -- )

\ Lighting                                             04oct97py

LocalExtern: glShadeModel void glShadeModel( int ); ( mode -- )
LocalExtern: glLightf void glLightf( int , int , float ); ( light pname fparam -- )
LocalExtern: glLighti void glLighti( int , int , int ); ( light pname param -- )
LocalExtern: glLightfv void glLightfv( int , int , int ); ( light pname *fparam -- )
LocalExtern: glLightiv void glLightiv( int , int , int ); ( light pname *param -- )
LocalExtern: glGetLightfv void glGetLightfv( int , int , int ); ( light pname *fparam -- )
LocalExtern: glGetLightiv void glGetLightiv( int , int , int ); ( light pname *param -- )
LocalExtern: glLightModelf void glLightModelf( int , float ); ( pname fparam -- )
LocalExtern: glLightModeli void glLightModeli( int , int ); ( pname param -- )
LocalExtern: glLightModelfv void glLightModelfv( int , int ); ( pname *param -- )
LocalExtern: glLightModeliv void glLightModeliv( int , int ); ( pname *param -- )
LocalExtern: glMaterialf void glMaterialf( int , int , float ); ( face pname fparam -- )
LocalExtern: glMateriali void glMateriali( int , int , int ); ( face pname param -- )
LocalExtern: glMaterialfv void glMaterialfv( int , int , int ); ( face pname *fparam -- )
LocalExtern: glMaterialiv void glMaterialiv( int , int , int ); ( face pname *param -- )
LocalExtern: glGetMaterialfv void glGetMaterialfv( int , int , int ); ( face pname *fp -- )
LocalExtern: glGetMaterialiv void glGetMaterialiv( int , int , int ); ( face pname *p -- )
LocalExtern: glColorMaterial void glColorMaterial( int , int ); ( face mode -- )

\ Raster functions                                     04oct97py

LocalExtern: glPixelZoom void glPixelZoom( float , float ); ( xfactor yfactor -- )
LocalExtern: glPixelStoref void glPixelStoref( int , float ); ( pname param -- )
LocalExtern: glPixelStorei void glPixelStorei( int , int ); ( pname param -- )
LocalExtern: glPixelTransferf void glPixelTransferf( int , float ); ( pname param -- )
LocalExtern: glPixelTransferi void glPixelTransferi( int , int ); ( pname param -- )
LocalExtern: glPixelMapfv void glPixelMapfv( int , int , int ); ( map mapsize *values -- )
LocalExtern: glPixelMapuiv void glPixelMapuiv( int , int , int ); ( map mapsize *values -- )
LocalExtern: glPixelMapusv void glPixelMapusv( int , int , int ); ( map mapsize *values -- )
LocalExtern: glGetPixelMapfv void glGetPixelMapfv( int , int , int ); ( m ms *values -- )
LocalExtern: glGetPixelMapuiv void glGetPixelMapuiv( int , int , int ); ( m ms *values -- )
LocalExtern: glGetPixelMapusv void glGetPixelMapusv( int , int , int ); ( m ms *values -- )
LocalExtern: glBitmap void glBitmap( int , int , int , int , int , int , int );          ( w h xorig yorig xmove ymove bitmap -- )
LocalExtern: glReadPixels void glReadPixels( int , int , int , int , int , int , int );          ( x y w h format type pixels -- )
LocalExtern: glDrawPixels void glDrawPixels( int , int , int , int , int ); ( w h format type *pixs -- )
LocalExtern: glCopyPixels void glCopyPixels( int , int , int , int , int ); ( x y w h type -- )

\ Stenciling                                           04oct97py

LocalExtern: glStencilFunc void glStencilFunc( int , int , int ); ( mask ref func -- )
LocalExtern: glStencilMask void glStencilMask( int ); ( mask -- )
LocalExtern: glStencilOp void glStencilOp( int , int , int ); ( zpass zfail fail -- )
LocalExtern: glClearStencil void glClearStencil( int ); ( s -- )

\ Texture Mapping                                      04oct97py

LocalExtern: glTexGend void glTexGend( int , int , double ); ( coord pname dparam -- )
LocalExtern: glTexGenf void glTexGenf( int , int , float ); ( coord pname fparam -- )
LocalExtern: glTexGeni void glTexGeni( int , int , int ); ( coord pname param -- )
LocalExtern: glTexGendv void glTexGendv( int , int , int ); ( coord pname *param -- )
LocalExtern: glTexGenfv void glTexGenfv( int , int , int ); ( coord pname *param -- )
LocalExtern: glTexGeniv void glTexGeniv( int , int , int ); ( coord pname *param -- )
LocalExtern: glGetTexGendv void glGetTexGendv( int , int , int ); ( coord pname *param -- )
LocalExtern: glGetTexGenfv void glGetTexGenfv( int , int , int ); ( coord pname *param -- )
LocalExtern: glGetTexGeniv void glGetTexGeniv( int , int , int ); ( coord pname *param -- )
LocalExtern: glTexEnvf void glTexEnvf( int , int , float ); ( target pname fparam -- )
LocalExtern: glTexEnvi void glTexEnvi( int , int , int ); ( target pname param -- )
LocalExtern: glTexEnvfv void glTexEnvfv( int , int , int ); ( target pname *fparam -- )
LocalExtern: glTexEnviv void glTexEnviv( int , int , int ); ( target pname *param -- )
LocalExtern: glGetTexEnvfv void glGetTexEnvfv( int , int , int ); ( target pname *param -- )
LocalExtern: glGetTexEnviv void glGetTexEnviv( int , int , int ); ( target pname *param -- )
LocalExtern: glTexParameterf void glTexParameterf( int , int , float ); ( target pname fparam -- )
LocalExtern: glTexParameteri void glTexParameteri( int , int , int ); ( target pname param -- )
LocalExtern: glTexParameterfv void glTexParameterfv( int , int , int ); ( target pname *fparam -- )
LocalExtern: glTexParameteriv void glTexParameteriv( int , int , int ); ( target pname *param -- )
LocalExtern: glGetTexParameterfv void glGetTexParameterfv( int , int , int ); ( target pname *fparam -- )
LocalExtern: glGetTexParameteriv void glGetTexParameteriv( int , int , int ); ( target pname *param -- )

LocalExtern: glGetTexLevelParameterfv void glGetTexLevelParameterfv( int , int , int , int );           ( target level pname *params -- )
LocalExtern: glGetTexLevelParameteriv void glGetTexLevelParameteriv( int , int , int , int );           ( target level pname *params -- )
LocalExtern: glTexImage1D void glTexImage1D( int , int , int , int , int , int , int , int );           ( target level component width border format type *pixels -- )
LocalExtern: glTexImage2D void glTexImage2D( int , int , int , int , int , int , int , int , int );           ( target level component width height border format type *pixels -- )
LocalExtern: glGetTexImage void glGetTexImage( int , int , int , int , int );           ( target level format type *pixels -- )
LocalExtern: glGenTextures void glGenTextures( int , int ); ( n *textures -- )
LocalExtern: glDeleteTextures void glDeleteTextures( int , int ); ( n *textures -- )
LocalExtern: glBindTexture void glBindTexture( int , int ); ( target textures -- )
LocalExtern: glPrioritizeTextures void glPrioritizeTextures( int , int , int );           ( n *textures *priorities -- )
LocalExtern: glAreTexturesResident int glAreTexturesResident( int , int , int );           ( n *textures *residences -- flag )
LocalExtern: glIsTexture int glIsTexture( int ); ( texture -- flag )
LocalExtern: glTexSubImage1D void glTexSubImage1D( int , int , int , int , int , int , int );           ( target level xoffset width format type *pixels -- )
LocalExtern: gkTexSubImage2D void glTexSubImage2D( int , int , int , int , int , int , int , int , int );           ( target level xoffset yoffset widht height format type *pixels -- )
LocalExtern: glCopyTexImage1D void glCopyTexImage1D( int , int , int , int , int , int , int );           ( target level intformat x y w border -- )
LocalExtern: glCopyTexImage2D void glCopyTexImage2D( int , int , int , int , int , int , int , int );           ( target level intformat x y w h border -- )
LocalExtern: glCopyTexSubImage1D void glCopyTexSubImage1D( int , int , int , int , int , int );           ( target level xoffset x y w -- )
LocalExtern: glCopyTexSubImage2D void glCopyTexSubImage2D( int , int , int , int , int , int , int , int );           ( target level xoffset yoffset x y w h -- )

\ Evaluators                                           04oct97py

LocalExtern: glMap1d void glMap1d( int , double , double , int , int , int ); ( t du1 du2 stride order *points -- )
LocalExtern: glMap1f void glMap1f( int , float , float , int , int , int ); ( t fu1 fu2 stride order *points -- )
LocalExtern: glMap2d void glMap2d( int , double , double , int , int , double , double , int , int , int );
           ( *points vorder vstride dv2 dv1 uorder ustride du2 du1 target -- )
LocalExtern: glMap2f void glMap2f( int , float , float , int , int , float , float , int , int , int );
           ( *points vorder vstride fv2 fv1 uorder ustride fu2 fu1 target -- )
LocalExtern: glGetMapdv void glGetMapdv( int , int , int ); ( target query *v -- )
LocalExtern: glGetMapfv void glGetMapfv( int , int , int ); ( target query *v -- )
LocalExtern: glGetMapiv void glGetMapiv( int , int , int ); ( target query *v -- )
LocalExtern: glEvalCoord1d void glEvalCoord1d( double ); ( du -- )
LocalExtern: glEvalCoord1f void glEvalCoord1f( float ); ( fu -- )
LocalExtern: glEvalCoord1dv void glEvalCoord1dv( int ); ( *du -- )
LocalExtern: glEvalCoord1fv void glEvalCoord1fv( int ); ( *fu -- )
LocalExtern: glEvalCoord2d void glEvalCoord2d( double , double ); ( du dv -- )
LocalExtern: glEvalCoord2f void glEvalCoord2f( float , float ); ( fu fv -- )
LocalExtern: glEvalCoord2dv void glEvalCoord2dv( int ); ( *du -- )
LocalExtern: glEvalCoord2fv void glEvalCoord2fv( int ); ( *fu -- )
LocalExtern: glMapGrid1d void glMapGrid1d( int , double , double ); ( un du1 du2 -- )
LocalExtern: glMapGrid1f void glMapGrid1f( int , float , float ); ( un fu1 fu2 -- )
LocalExtern: glMapGrid2d void glMapGrid2d( int , double , double , int , double , double ); ( un du1 du2 vn dv1 dv2 -- )
LocalExtern: glMapGrid2f void glMapGrid2f( int , float , float , int , float , float ); ( fv2 fv1 vn fu2 fu1 un -- )
LocalExtern: glEvalPoint1 void glEvalPoint1( int ); ( i -- )
LocalExtern: glEvalPoint2 void glEvalPoint2( int , int ); ( i j -- )
LocalExtern: glEvalMesh1 void glEvalMesh1( int , int , int ); ( mode i1 i2 -- )
LocalExtern: glEvalMesh2 void glEvalMesh2( int , int , int , int , int ); ( mode i1 i2 j1 j2 -- )

\ Fog                                                  04oct97py

LocalExtern: glFogf void glFogf( int , float ); ( pname param -- )
LocalExtern: glFogi void glFogi( int , int ); ( pname param -- )
LocalExtern: glFogfv void glFogfv( int , int ); ( pname *param -- )
LocalExtern: glFogiv void glFogiv( int , int ); ( pname *param -- )

\ Selection and Feedback                               04oct97py

LocalExtern: glFeedbackBuffer void glFeedbackBuffer( int , int , int );                      ( size type *buffer -- )
LocalExtern: glPassThrough void glPassThrough( int ); ( token -- )
LocalExtern: glSelectBuffer void glSelectBuffer( int , int ); ( size *buffer -- )
LocalExtern: glInitNames void glInitNames( void ); ( -- )
LocalExtern: glLoadName void glLoadName( int ); ( name -- )
LocalExtern: glPushName void glPushName( int ); ( name -- )
LocalExtern: glPopName void glPopName( void ); ( -- )

\ glX calls                                            04oct97py

[defined] x11 [IF]
LocalExtern: glXChooseVisual int glXChooseVisual( int , int , int );                        ( dpy screen attriblist -- XVisualInfo )
LocalExtern: glXCreateContext int glXCreateContext( int , int , int , int );                        ( dpy vis shareList direct -- GLXContext )
LocalExtern: glXDestroyContext void glXDestroyContext( int , int ); ( dpy ctx -- )
LocalExtern: glXMakeCurrent int glXMakeCurrent( int , int , int );                        ( dpy drawable ctx -- flag )
LocalExtern: glXCopyContext void glXCopyContext( int , int , int , int ); ( dpy src dst mask -- )
LocalExtern: glXSwapBuffers void glXSwapBuffers( int , int ); ( dpy drawable -- )
LocalExtern: glXCreateGLXPixmap int glXCreateGLXPixmap( int , int , int );                              ( dpy visual pixmap -- GLXPixmap )
LocalExtern: glXDestroyGLXPixmap void glXDestroyGLXPixmap( int , int );                                             ( dpy pixmap -- )
LocalExtern: glXQueryExtension int glXQueryExtension( int , int , int );                                    ( dpy errorb event -- flag )
LocalExtern: glXQueryVersion int glXQueryVersion( int , int , int ); ( dpy maj min -- flag )
LocalExtern: glXIsDirect int glXIsDirect( int , int ); ( dpy ctx -- flag )
LocalExtern: glXGetConfig void glXGetConfig( int , int , int , int ); ( dpy visual attrib value -- )
LocalExtern: glXGetCurrentContext int glXGetCurrentContext( void );                                               ( -- GLXContext )
LocalExtern: glXGetCurrentDrawable int glXGetCurrentDrawable( void );                                              ( -- GLXDrawable )
LocalExtern: glXWaitGL void glXWaitGL( void ); ( -- )
LocalExtern: glXWaitX void glXWaitX( void ); ( -- )
LocalExtern: glXUseXFont void glXUseXFont( int , int , int , int ); ( font first count list -- )
LocalExtern: glXQueryExtensionsString int glXQueryExtensionsString( int , int );                                        ( dpy screen -- string )
LocalExtern: glXQueryServerString int glXQueryServerString( int , int , int );                                   ( dpy screen name -- string )
LocalExtern: glXGetClientString int glXGetClientString( int , int );                                          ( name dpy -- string )
[THEN]

LocalExtern: gluNewTess int gluNewTess( void ); ( -- tess )
LocalExtern: gluDeleteTess void gluDeleteTess( int ); ( tess -- )
LocalExtern: gluTessBeginContour void gluTessBeginContour( int ); ( tess -- )
LocalExtern: gluTessBeginPolygon void gluTessBeginPolygon( int , int ); ( tess data -- )
LocalExtern: gluTessCallback void gluTessCallback( int , int , int ); ( tess which cfun -- )
LocalExtern: gluTessEndContour void gluTessEndContour( int ); ( tess -- )
LocalExtern: gluTessEndPolygon void gluTessEndPolygon( int ); ( tess -- )
LocalExtern: gluTessNormal void gluTessNormal( int , double , double , double ); ( tess rx ry< rz -- )
LocalExtern: gluTessProperty void gluTessProperty( int , int , double ); ( tess wich rdata -- )
LocalExtern: gluTessVertex void gluTessVertex( int , int , int ); ( tess loc data -- )

\ GLU functions                                        01nov97py

previous definitions
