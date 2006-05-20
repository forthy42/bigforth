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
             dup I points sf@+ sf@ fs>f glVertex3f
             I points sf@+ sf@ glTexCoord2f  -2 +LOOP
    glEnd fsneg
    !0 !0 !-1 glNormal3f
    GL_TRIANGLE_FAN glBegin
        $E $0 DO  dup I points sf@+ sf@ fs>f glVertex3f
             I points sf@+ sf@ glTexCoord2f   2 +LOOP
    glEnd fsneg
\ outer side
    GL_TRIANGLE_STRIP glBegin
    $C $2 DO
        fsneg dup I points sf@+ sf@ fs>f glVertex3f
                  I points cell+ sf@ dup fs>f glTexCoord2f
        fsneg dup I points sf@+ sf@ fs>f glVertex3f
                  I points cell+ sf@ dup fs>f glTexCoord2f
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
Create .white !1   f>fs , !1   f>fs , !1   f>fs , !1  f>fs ,
Create .red   !&.8 f>fs , !&.1 f>fs , !&0  f>fs , !1  f>fs ,
Create .green !&0  f>fs , !&.8 f>fs , !&.2 f>fs , !1  f>fs ,
Create .blue  !&.2 f>fs , !&.2 f>fs , !&1  f>fs , !1  f>fs ,

Create textures 0 , 0 , 0 ,

here ," pattern/normal-w1.ppm"
here ," pattern/back.ppm"
here ," pattern/focus.ppm"
Create patterns A, A, A,

Create texpts
    0  ,  0 ,    0 , #1 ,
    #1 ,  0 ,   #1 , #1 ,

: load-texture ( addr u -- )
    r/o open-file throw >r
    pad $100 r@ read-line throw 2drop
    pad $100
    BEGIN  drop dup $100 r@ read-line  throw drop  over c@ '#
           <>  UNTIL
    0. 2swap >number 1 /string 0. 2swap >number 2drop drop nip
    pad $100 r@ read-line throw 2drop
    ( w h )
    2dup * 3 * dup NewPtr tuck swap r@ read-file throw drop
    r> close-file throw
    dup >r -rot >r >r
    GL_TEXTURE_2D 0 3 r> r> 0 GL_RGB GL_UNSIGNED_BYTE r>
    glTexImage2D DisposPtr
\    -rot >r >r GL_UNSIGNED_BYTE GL_RGB r> r>
\    swap 3 GL_TEXTURE_2D
\    gluBuild2DMipmaps drop
    GL_TEXTURE_2D GL_TEXTURE_MIN_FILTER GL_NEAREST 
        glTexParameteri
    GL_TEXTURE_2D GL_TEXTURE_MAG_FILTER GL_NEAREST
        glTexParameteri
    GL_TEXTURE_2D GL_TEXTURE_WRAP_S GL_REPEAT
        glTexParameteri
    GL_TEXTURE_2D GL_TEXTURE_WRAP_T GL_REPEAT
       glTexParameteri ;

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

    3 textures glGenTextures

    3 0 DO
        GL_TEXTURE_2D textures Ith glBindTexture
        patterns Ith count load-texture
    LOOP

    &20 !&.2  !&.73 !&.87 !&.2 create-tooth    &20 create-gear
    &10 !&.10 !&.33 !&.47 !&.4 create-tooth    &10 create-gear
    &10 !&.26 !&.33 !&.47 !&.1 create-tooth    &10 create-gear ;

: rotation ( teeth -- fn )
    &86400 swap / timer@ * &360 um* d>f !$.00000001 f* ;

: call-gear ( n r+ tx ty tz texture color -- )
    cells textures + @ GL_TEXTURE_2D swap glBindTexture
    >r f>fs f>fs f>fs r>
    GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv
    glPushMatrix
    fs>f fs>f fs>f glTranslatef
    fs>f !0 !0 !1 glRotatef
    glCallList
    glPopMatrix ;

: draw-gear ( o g0 g1 g2 alx aly alz pitch bend roll zoom texture
              -- )
 { g0 g1 g2 alx aly alz alp alb alr zoom texture |
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

         texture IF
            GL_TEXTURE_2D glEnable
            .white .white .white
        ELSE
            GL_TEXTURE_2D glDisable
            .blue .green .red
        THEN

       !&9  -&5 rotation f+ f>fs >r
        !-&9 -&5 rotation f+ f>fs >r
        !0  &10  rotation f+ f>fs >r

        !-&.6  !-&.4 !0 g0 r> rot 0 call-gear
        !&.62  !-&.4 !0 g1 r> rot 1 call-gear
        !-&.6  !&.84 !0 g2 r> rot 2 call-gear
    endwith } ;

previous previous previous previous previous
