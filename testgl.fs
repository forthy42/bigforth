\ gl test window

minos also

screen self window : win0

win0 self &360 scale-var new win0 self hscaler : scx -&180 scx offset !
win0 self &360 scale-var new win0 self hscaler : scy -&180 scy offset !
win0 self &360 scale-var new win0 self hscaler : scz -&180 scz offset !
    
\needs float import float
float also
| import glconst glconst also opengl also

: array  Create cells allot  DOES> swap cells + ;

$E array points

: r,phi ( r angle -- x y )
    fsincos f>r fover f* fswap fr> f* f>fs f>fs ;

: fsneg $80000000 xor ; macro

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
    [ !1 f>fs ] Literal 0 0 glNormal3f drop
    GL_POLYGON glBegin drop
        $0 $C DO  dup I points 2@ glVertex3f drop  -2 +LOOP
    glEnd drop fsneg
    [ !-1 f>fs ] Literal 0 0 glNormal3f drop
    GL_POLYGON glBegin drop
        $E $0 DO  dup I points 2@ glVertex3f drop   2 +LOOP
    glEnd drop fsneg
\ outer side
    $A $2 DO
        I 3 and 0= IF
            0 [ !1 f>fs ] Literal 0 glNormal3f drop
        ELSE
            0 I 2+ points 2@ fs>f I points 2@ fs>f f-
            fs>f fs>f f- f>fs f>fs swap
            glNormal3f drop
        THEN
        GL_QUADS glBegin drop
            dup I    points 2@ glVertex3f drop
            dup I 2+ points 2@ glVertex3f drop fsneg
            dup I 2+ points 2@ glVertex3f drop
            dup I    points 2@ glVertex3f drop fsneg
        glEnd drop
    2 +LOOP
\ inner side
    0 [ !-1 f>fs ] Literal 0 glNormal3f drop
    GL_SMOOTH glShadeModel drop
    GL_QUADS glBegin drop
        dup $C points 2@ glVertex3f drop
        dup $0 points 2@ glVertex3f drop fsneg
        dup $0 points 2@ glVertex3f drop
        dup $C points 2@ glVertex3f drop drop
    glEnd drop
;

3 array teeth

: gear ( tooth teeth -- )
    glPushMatrix drop
    0 ?DO
        [ !1 f>fs ] Literal 0 0 !360 i' fm/ f>fs glRotatef drop
        dup glCallList drop
    LOOP drop
    glPopMatrix drop ;

: create-tooth ( teeth r0 r1 r2 rw -- n )
    1 glGenLists GL_COMPILE over glNewList drop
    swap tooth glEndList drop ;

Create .pos   !5  f>fs , !5  f>fs , !10 f>fs , !0  f>fs ,
Create .red   !.8 f>fs , !.1 f>fs , !0  f>fs , !1  f>fs ,
Create .green !0  f>fs , !.8 f>fs , !.2 f>fs , !1  f>fs ,
Create .blue  !.2 f>fs , !.2 f>fs , !1  f>fs , !1  f>fs ,

3 array gears

: create-gears ( -- )
    .pos GL_POSITION GL_LIGHT0 glLightfv drop
    
    GL_CULL_FACE  glEnable drop
    GL_LIGHTING   glEnable drop
    GL_LIGHT0     glEnable drop
    GL_DEPTH_TEST glEnable drop
    GL_NORMALIZE  glEnable drop
        
\    GL_FLAT glShadeModel drop

    &20 !1   !3.65 !4.35 !1  create-tooth 0 teeth !
    &10 !.5  !1.65 !2.35 !2  create-tooth 1 teeth !
    &10 !1.3 !1.65 !2.35 !.5 create-tooth 2 teeth !
    
    1 glGenLists GL_COMPILE over glNewList drop 0 gears !
    0 teeth @ &20 gear
    glEndList drop

    1 glGenLists GL_COMPILE over glNewList drop 1 gears !
    1 teeth @ &10 gear
    glEndList drop

    1 glGenLists GL_COMPILE over glNewList drop 2 gears !
    2 teeth @ &10 gear
    glEndList drop

    GL_NORMALIZE glEnable drop ;

0 gears off

: rotation ( teeth -- n )
    &86400 swap / timer@ * &360 um* d>f !$100000000 f/ ;

:noname
    glcanvas with
        0 gears @ 0= IF  create-gears  THEN

        h @ w @ 0 0 glViewport drop

        GL_PROJECTION glMatrixMode drop
        glLoadIdentity drop

        w @ h @ >
        IF
            w @ s>f h @ fm/
            !60 f>fd !5 f>fd !1 f>fd !-1 f>fd fdup f>fd fnegate f>fd
        ELSE
            h @ s>f w @ fm/
            !60 f>fd !5 f>fd fdup f>fd fnegate f>fd !1 f>fd !-1 f>fd
        THEN
        glFrustum drop
            
        GL_MODELVIEW glMatrixMode drop
        glLoadIdentity drop
        !-40 f>fs 0 0 glTranslatef drop
        GL_COLOR_BUFFER_BIT GL_DEPTH_BUFFER_BIT or glClear drop

        [ !1 f>fs ] Literal >r
        0 0 r@ scx get nip nip s>f f>fs glRotatef drop
        0 r@ 0 scy get nip nip s>f f>fs glRotatef drop
        r> 0 0 scz get nip nip s>f f>fs glRotatef drop
        glPushMatrix drop
        !0 f>fs !-2 f>fs !-3 f>fs glTranslatef drop
        [ !1 f>fs ] Literal 0 0 &10 rotation f>fs glRotatef drop
        .red GL_AMBIENT_AND_DIFFUSE GL_FRONT glMaterialfv drop
        0 gears @ glCallList drop
        glPopMatrix drop
        glPushMatrix drop
        !0 f>fs !-2 f>fs !3.1 f>fs glTranslatef drop
        [ !1 f>fs ] Literal 0 0 &5 rotation fnegate
        !9 f- f>fs glRotatef drop
        .green GL_AMBIENT_AND_DIFFUSE GL_FRONT glMaterialfv drop
        1 gears @ glCallList drop
        glPopMatrix drop
        glPushMatrix drop
        !0 f>fs !4.2 f>fs !-3 f>fs glTranslatef drop
        [ !1 f>fs ] Literal 0 0 &5 rotation fnegate
        !9 f+ f>fs glRotatef drop
        .blue GL_AMBIENT_AND_DIFFUSE GL_FRONT glMaterialfv drop
        2 gears @ glCallList drop
        glPopMatrix drop
    endwith ;
$C0 $2 *fill $C0 $3 *fill win0 self glcanvas : glcanvas1

    glcanvas1 self
    scx self
    scy self
    scz self
    4 win0 self vabox new
    s" Test" win0 assign

win0 map

: draw-gears
    $2000 $2000 NewTask activate
    BEGIN  &60 after >r
        glcanvas1 render  &100 0 DO  pause  LOOP
        glcanvas1 draw    &100 0 DO  pause  LOOP
        r> till
    AGAIN ;

-&30 scy assign
 &20 scx assign

draw-gears
