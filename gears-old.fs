\ gl test window

dos also memory also
\needs float import float
\needs glconst | import glconst
float also glconst also opengl also

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
    f2/ f>fs >r pi fm/ f2/ >points r>
    GL_FLAT glShadeModel
    \ front and back side
    !0 !0 !1 glNormal3f
    GL_TRIANGLE_FAN glBegin
         $0 $C DO
             dup I points sf@+ sf@ fs>f glVertex3f  -2 +LOOP
    glEnd fsneg
    !0 !0 !-1 glNormal3f
    GL_TRIANGLE_FAN glBegin
        $E $0 DO  dup I points sf@+ sf@ fs>f glVertex3f   2 +LOOP
    glEnd fsneg
\ outer side
    GL_TRIANGLE_STRIP glBegin
    $C $2 DO
        dup fsneg I points sf@+ sf@ fs>f glVertex3f
        dup       I points sf@+ sf@ fs>f glVertex3f
        I 3 and 0= IF
            !0 !1 !0
        ELSE
            I 2+ points 2@ fs>f I points 2@ fs>f f-
            fs>f fs>f f- fswap !0
        THEN
        glNormal3f
    2 +LOOP
    glEnd
\ inner side
    GL_SMOOTH glShadeModel
    GL_TRIANGLE_STRIP glBegin fsneg
        $0 points sf@+ fnegate sf@ fnegate !0 glNormal3f
        fsneg dup $0 points sf@+ sf@ fs>f glVertex3f
        fsneg dup $0 points sf@+ sf@ fs>f glVertex3f
        $C points sf@+ fnegate sf@ fnegate !0 glNormal3f
        fsneg dup $C points sf@+ sf@ fs>f glVertex3f
        fsneg dup $C points sf@+ sf@ fs>f glVertex3f
    glEnd  drop ;

: gear ( tooth teeth -- )
    glPushMatrix
    0 ?DO
        !&360 i' fm/ !0 !0 !1 glRotatef
        dup glCallList
    LOOP drop
    glPopMatrix ;

: create-tooth ( teeth r0 r1 r2 rw -- n )
    1 glGenLists \ dup 0= IF ." no list" cr THEN
    dup >r GL_COMPILE glNewList
    tooth glEndList r> ;

Create .pos   !&5  f>fs , !&5  f>fs , !&10 f>fs , !0  f>fs ,
Create .red   !&.8 f>fs , !&.1 f>fs , !&0  f>fs , !1  f>fs ,
Create .green !&0  f>fs , !&.8 f>fs , !&.2 f>fs , !1  f>fs ,
Create .blue  !&.2 f>fs , !&.2 f>fs , !&1  f>fs , !1  f>fs ,

Create textures 0 , 0 , 0 ,

: create-gear ( list teeth -- gear )
    1 glGenLists \ dup 0= IF ." no list" cr THEN
    dup >r GL_COMPILE glNewList
    gear  glEndList r> ;

: create-gears ( -- gear0 gear1 gear2 )
    GL_LIGHT0 GL_POSITION .pos glLightfv

    GL_CULL_FACE
    GL_LIGHTING
    GL_LIGHT0
    GL_DEPTH_TEST
    GL_NORMALIZE
    5 0 DO  glEnable  LOOP

    &20 !&.2  !&.73 !&.87 !&.2 create-tooth    &20 create-gear
    &10 !&.10 !&.33 !&.47 !&.4 create-tooth    &10 create-gear
    &10 !&.26 !&.33 !&.47 !&.1 create-tooth    &10 create-gear ;

: rotation ( teeth -- fn )
    &4320 * swap / timer@ * 
    &360 um* d>f !$.00000001 f*  ;

: call-gear ( n r+ tx ty tz color -- )
    GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv
    glPushMatrix
    glTranslatef
    fs>f !0 !0 !1 glRotatef
    glCallList
    glPopMatrix ;

: draw-gear ( o g0 g1 g2 alx aly alz pitch bend roll zoom speed 
              -- )
 { g0 g1 g2 alx aly alz alp alb alr zoom speed |
    glcanvas with
        0 0 w @ h @ glViewport
        
        GL_PROJECTION glMatrixMode
        glLoadIdentity

        w @ h @ >  IF
            w @ s>f h @ fm/ fdup fnegate fswap !-1 !1
        ELSE
            !-1 !1 h @ s>f w @ fm/ fdup fnegate fswap
        THEN
        !&5 !&60 glFrustum
        
        GL_MODELVIEW glMatrixMode
        glLoadIdentity
        !0 !0 zoom 100 + negate s>f !0.08 f* glTranslatef
        GL_COLOR_BUFFER_BIT GL_DEPTH_BUFFER_BIT or glClear
    
        alx s>f !1 !0 !0 glRotatef
        aly s>f !0 !1 !0 glRotatef
        alz s>f !0 !0 !1 glRotatef
        alp s>f !1 !0 !0 glRotatef
        alb s>f !0 !1 !0 glRotatef
        alr s>f !0 !0 !1 glRotatef

        !&9  -&5 speed rotation f+ f>fs >r
        !-&9 -&5 speed rotation f+ f>fs >r
        !0  &10  speed rotation f+ f>fs >r

        !-&.6  !-&.4 !0 g0 r> .red   call-gear
        !&.62  !-&.4 !0 g1 r> .green call-gear
        !-&.6  !&.84 !0 g2 r> .blue  call-gear
    endwith } ;

previous previous previous previous previous
