\ gl test window

dos also memory also
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

Create .white !&1  f>fs , !&1  f>fs , !&1  f>fs , !1  f>fs ,
Create .red   !&.8 f>fs , !&.1 f>fs , !&0  f>fs , !1  f>fs ,
Create .green !&0  f>fs , !&.8 f>fs , !&.2 f>fs , !1  f>fs ,
Create .blue  !&.2 f>fs , !&.2 f>fs , !&1  f>fs , !1  f>fs ,

: rotation ( speed teeth -- fn )
    &2160 * * s>f [ pi !$.00000002 f* ] Fliteral f* ;

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
      set-texture S" pattern/normal-w1.ppm" load-texture
      set-texture S" pattern/back.ppm"      load-texture
      set-texture S" pattern/focus.ppm"     load-texture ;
[ELSE]  0 0 0 ;  [THEN]
endwith

-1 Value test-list

: draw-gear  ( o alx aly alz pitch bend roll zoom speed
               shade tx sx sy sz t1 t2 t3 -- )
{ alx aly alz alp alb alr zoom speed shade txt sx sy sz t1 t2 t3 |
    glcanvas with
        !5 !60 w @ h @ 3d-turtle new  3d-turtle with

            shade 0 = IF  triangles  THEN
            shade 1 = IF  textured   1 wait' +! THEN
            shade 2 = IF  lines      wait' off  THEN

            test-list 0< IF  1 swap glGenLists TO test-list  THEN

            test-list GL_COMPILE glNewList

            0 !5 !5 !-10 get-xyz GL_POSITION 0 set-light

            zoom 100 + s>f !0.08 f* forward

            pi !180 f/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            !.01 sx fm* !.01 sy fm* !.01 sz fm* scale-xyz

            !0    5  speed rotation f+
            !-9 -10  speed rotation f+
            !9  -10  speed rotation f+ { f: rr f: br f: gr |

                          xy-texture  
            txt   1 = IF  rphi-texture  THEN
            txt   2 = IF  zphi-texture  THEN

            !-.6  !-.4 !0 forward-xyz
            >matrix  rr roll-left
            shade 1 <> IF    .red   .color
                  ELSE  t1 .text  THEN
            !.2  !.73 !.87 !.2   20  gear
            matrix@
            !0 !1.22 !0 forward-xyz  gr roll-left
            shade 1 <> IF    .green .color
                  ELSE  wait' @ 3 < IF  t1  ELSE  t2  THEN .text  THEN
            !.10 !.33 !.47 !.5
                  rr fsin !.2 f* f+ f**2 10  gear
            matrix>
            !1.22 !0 !0 forward-xyz  br roll-left
            shade 1 <> IF    .blue  .color
                  ELSE  wait' @ 3 < IF  t1  ELSE
                        wait' @ 6 < IF  t2  ELSE
                                        t3  THEN THEN  .text  THEN
            !.26 !.33 !.47 !.5
                  rr fsin !.2 f* f- f**2 10  gear }
            glEndList                     \ cr .time
            test-list glCallList          \    .time
        dispose endwith
    endwith } ;

previous previous previous previous previous
