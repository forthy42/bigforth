\ gl test window

dos also memory also
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

Create .white 1e  f>fs , 1e  f>fs , 1e  f>fs , 1e  f>fs ,
Create .red   .8e f>fs , .1e f>fs , 0e  f>fs , 1e  f>fs ,
Create .green 0e  f>fs , .8e f>fs , .2e f>fs , 1e  f>fs ,
Create .blue  .2e f>fs , .2e f>fs , 1e  f>fs , 1e  f>fs ,

: rotation ( speed teeth -- fn )
    #2160 * * s>f [ pi 2147483648e f/ ] Fliteral f* ;

3d-turtle with
  F : gear ( ri rm ro h teeth color -- ) 4*
      { f: ri f: rm f: ro f: h n |
        pi f2* n fm/ set-dphi
        h f2/ fnegate forward
        n open-path
        n 0 DO  ri add-r  LOOP
        GL_FLAT glShadeModel
        2 0 DO
            next-round n 4/ 0 DO
                rm set-r  ro set-r  ro set-r  rm set-r
            LOOP  h forward
        LOOP
        2 0 DO
            h fnegate forward
            next-round n 0 DO  ri set-r  LOOP
        LOOP  GL_SMOOTH glShadeModel
        close-path } ;
endwith

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

Variable wait' 6 wait' !

: .text ( n -- )
?texture [IF]
  .white .color GL_TEXTURE_2D swap glBindTexture ;
[ELSE]  drop ;  [THEN]

3d-turtle with
  F : init-texture ( -- t1 t2 t3 )
?texture [IF]
      3 textures dup 2over swap
      set-texture S" pattern/normal-w1" load-texture
      set-texture S" pattern/back"      load-texture
      set-texture S" pattern/focus"     load-texture ;
[ELSE]  0 0 0 ;  [THEN]
endwith

-1 Value test-list

: draw-gear  ( o alx aly alz pitch bend roll zoom speed
               shade tx sx sy sz t1 t2 t3 -- )
    { alx aly alz alp alb alr zoom speed shade txt sx sy sz t1 t2 t3 }
    glcanvas with
        5e 60e w @ h @ 3d-turtle new  3d-turtle with

            shade 0 = IF  triangles  THEN
            shade 1 = IF  textured   1 wait' +! THEN
            shade 2 = IF  lines      wait' off  THEN

            test-list 0< IF  1 swap glGenLists TO test-list  THEN

            test-list GL_COMPILE glNewList

            0 5e 5e -10e get-xyz GL_POSITION 0 set-light

            zoom 100 + s>f 0.08e f* forward

            pi 180e f/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            .01e sx fm* .01e sy fm* .01e sz fm* scale-xyz

                          xy-texture  
            txt   1 = IF  rphi-texture  THEN
            txt   2 = IF  zphi-texture  THEN

            -.6e  -.4e 0e forward-xyz
            >matrix  0e    5  speed rotation f+ roll-left
            shade 1 <> IF    .red   .color
                  ELSE  t1 .text  THEN
            .2e  .73e .87e .2e   20  gear
            matrix@
            0e 1.22e 0e forward-xyz  9e  -10  speed rotation f+ roll-left
            shade 1 <> IF    .green .color
                  ELSE  wait' @ 3 < IF  t1  ELSE  t2  THEN .text  THEN
            .10e .33e .47e .5e
                  0e    5  speed rotation f+ fsin .2e f* f+ f**2 10  gear
            matrix>
            1.22e 0e 0e forward-xyz  -9e -10  speed rotation f+ roll-left
            shade 1 <> IF    .blue  .color
                  ELSE  wait' @ 3 < IF  t1  ELSE
                        wait' @ 6 < IF  t2  ELSE
                                        t3  THEN THEN  .text  THEN
            .26e .33e .47e .5e
                  0e    5  speed rotation f+ fsin .2e f* f- f**2 10  gear
            glEndList                     \ cr .time
            test-list glCallList          \    .time
        dispose endwith
    endwith ;

previous previous previous previous previous
