\ gl test window

dos also memory also
\needs float import float
float also
| import glconst glconst also opengl also

: array  Create cells allot  DOES> swap cells + ;

$E array points

: r,phi ( r angle -- x y )
    fsincos f>r fover f* fswap fr> f* f>fs f>fs ;

: fsneg $80000000 xor ; macro

!1 f>fs Constant #1

: >points ( r0 r1 r2 angle -- )
    f>r
    2 fpick f>fs 0        $0 points 2!
    fover   f>fs 0        $2 points 2!
    fdup fr@ r,phi        $4 points 2!
    fr@ f2* r,phi         $6 points 2!
    fdup fr@ 3 fm* r,phi  $8 points 2!
    fr@ 4 fm* r,phi       $A points 2!
    fr> 4 fm* r,phi       $C points 2! ;

: tooth ( teeth r0 r1 r2 rw -- )
    GL_FLAT glShadeModel drop
    f2/ f>fs >r pi fm/ f2/ >points r>
    \ front and back side
    #1 0 0 glNormal3f drop
    GL_TRIANGLE_FAN glBegin drop
         $0 $C DO
             dup I points 2@ glVertex3f drop  -2 +LOOP
    glEnd drop fsneg
    [ !-1 f>fs ] Literal 0 0 glNormal3f drop
    GL_TRIANGLE_FAN glBegin drop
        $E $0 DO  dup I points 2@ glVertex3f drop   2 +LOOP
    glEnd drop fsneg
\ outer side
    GL_QUAD_STRIP glBegin drop
    $C $2 DO
        fsneg dup I points 2@ glVertex3f drop
        fsneg dup I points 2@ glVertex3f drop
        I 3 and 0= IF
            0 #1 0
        ELSE
            0 I 2+ points 2@ fs>f I points 2@ fs>f f-
            fs>f fs>f f- f>fs f>fs swap
        THEN
        glNormal3f drop
    2 +LOOP
    glEnd drop
\ inner side
    GL_SMOOTH glShadeModel drop
    GL_QUAD_STRIP glBegin drop fsneg
        0 $0 points 2@ swap fsneg swap fsneg glNormal3f drop
        fsneg dup $0 points 2@ glVertex3f drop
        fsneg dup $0 points 2@ glVertex3f drop
        0 $C points 2@ swap fsneg swap fsneg glNormal3f drop
        fsneg dup $C points 2@ glVertex3f drop
        fsneg dup $C points 2@ glVertex3f drop
    glEnd 2drop ;

: gear ( tooth teeth -- )
    glPushMatrix drop
    0 ?DO
        #1 0 0 !360 i' fm/ f>fs glRotatef drop
        dup glCallList drop
    LOOP drop
    glPopMatrix drop ;

: create-tooth ( teeth r0 r1 r2 rw -- n )
    1 glGenLists \ dup 0= IF ." no list" cr THEN
    GL_COMPILE over glNewList drop
    swap tooth glEndList drop ;

Create .pos   !5  f>fs , !5  f>fs , !10 f>fs , !0  f>fs ,
Create .red   !.8 f>fs , !.1 f>fs , !0  f>fs , !1  f>fs ,
Create .green !0  f>fs , !.8 f>fs , !.2 f>fs , !1  f>fs ,
Create .blue  !.2 f>fs , !.2 f>fs , !1  f>fs , !1  f>fs ,

Create textures 0 , 0 , 0 ,

: create-gear ( list teeth -- gear )
    1 glGenLists \ dup 0= IF ." no list" cr THEN
    >r GL_COMPILE r@ glNewList drop
    gear  glEndList drop r> ;

: create-gears ( -- gear0 gear1 gear2 )
    .pos GL_POSITION GL_LIGHT0 glLightfv drop

    GL_CULL_FACE
    GL_LIGHTING
    GL_LIGHT0
    GL_DEPTH_TEST
    GL_NORMALIZE
    5 0 DO  glEnable drop  LOOP

    &20 !.2  !.73 !.87 !.2 create-tooth    &20 create-gear
    &10 !.10 !.33 !.47 !.4 create-tooth    &10 create-gear
    &10 !.26 !.33 !.47 !.1 create-tooth    &10 create-gear ;

: rotation ( teeth -- fn )
    &86400 swap / timer@ * &360 um* d>f !$.00000001 f* ;

: call-gear ( n r+ tx ty tz color -- )
    GL_AMBIENT_AND_DIFFUSE GL_FRONT glMaterialfv drop
    glPushMatrix drop
    f>fs f>fs f>fs glTranslatef drop
    >r #1 0 0 r> glRotatef drop
    glCallList drop
    glPopMatrix drop ;

: draw-gear ( o g0 g1 g2 alx aly alz pitch bend roll zoom -- )
 { g0 g1 g2 alx aly alz alp alb alr zoom |
    glcanvas with
        h @ w @ 0 0 glViewport drop
        
        GL_PROJECTION glMatrixMode drop
        glLoadIdentity drop

        !60 f>fd !5 f>fd
        w @ h @ >
        IF
            w @ s>f h @ fm/
            !1 f>fd !-1 f>fd fdup f>fd fnegate f>fd
        ELSE
            h @ s>f w @ fm/
            fdup f>fd fnegate f>fd !1 f>fd !-1 f>fd
        THEN
        glFrustum drop
        
        GL_MODELVIEW glMatrixMode drop
        glLoadIdentity drop
        zoom 100 + negate s>f !0.08 f* f>fs 0 0 glTranslatef 
        drop
        GL_COLOR_BUFFER_BIT GL_DEPTH_BUFFER_BIT or glClear drop
    
        0 0 #1 alx s>f f>fs glRotatef drop
        0 #1 0 aly s>f f>fs glRotatef drop
        #1 0 0 alz s>f f>fs glRotatef drop
        0 0 #1 alp s>f f>fs glRotatef drop
        0 #1 0 alb s>f f>fs glRotatef drop
        #1 0 0 alr s>f f>fs glRotatef drop

        !9  -&5 rotation f+ f>fs >r
        !-9 -&5 rotation f+ f>fs >r
        !0  &10 rotation f+ f>fs >r

        !-.6  !-.4 !0 g0 r> .red   call-gear
        !.62  !-.4 !0 g1 r> .green call-gear
        !-.6  !.84 !0 g2 r> .blue  call-gear
    endwith } ;

previous previous previous previous previous
