\ gl test window

dos also memory also
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

&100 Value rd-val

Create .white !&1  f>fs , !&1  f>fs , !&1  f>fs , !1  f>fs ,
Create .brown !&.8 f>fs , !&.4 f>fs , !&.2  f>fs , !1  f>fs ,
Create .green !&.5 f>fs , !&.8 f>fs , !&.4  f>fs , !1  f>fs ,

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

3d-turtle with
  F : leaf ( -- )
      .green .color !.2 3 sphere .brown .color ;
  F : (tree ( m n -- )  recursive
      BEGIN  dup  WHILE
             pi !.3 f* roll-left
             dup !.03 fm* dup !.1 fm* 6 segment
             over 1 ?DO
                &1000 random rd-val <
                IF  2pi I I' fm*/ { f: di |
                    >turtle
                       di roll-left pi 5 fm/ right di roll-right
                       2dup 1- (tree
                    turtle> }  THEN
                LOOP
             pi 5 fm/ right
      1- REPEAT  close-path leaf 2drop ;
  F : draw-tree ( n -- )  rd-val seed !
      pi 3 fm/ set-dphi
      6 start-path
      !0.0001 !0 6 segment
      dup !.03 fm* !0 6 segment
      dup !.03 fm* !0 6 segment
      (tree ;      
endwith

Variable wait'

: .text ( n -- )
?texture [IF]
  .white .color GL_TEXTURE_2D glBindTexture ;
[ELSE]  drop ;  [THEN]

3d-turtle with
  F : init-texture ( -- t1 )
?texture [IF]
      1 textures dup set-texture
      S" pattern/bark.ppm" load-texture ;
[ELSE]  0 ;  [THEN]
endwith

: draw-gear  ( o alx aly alz pitch bend roll zoom speed
               shade tx sx sy sz t1 -- )
{ alx aly alz alp alb alr zoom speed br rd shade t1 |
    glcanvas with
        !5 !60 w @ h @ 3d-turtle new  3d-turtle with

            .brown .color
            GL_SMOOTH glShadeModel
            shade 0 = IF  triangles  smooth on  THEN
            shade 1 = IF  textured   1 wait' +!
                          t1 .text   smooth on  THEN
            shade 2 = IF  lines      wait' off  THEN

            0 !5 !5 !-10 get-xyz GL_POSITION 0 set-light

            !200 !0.08 f* forward
            pi f2/ up !-2 forward
            zoom !.1 fm* speed fm/ scale

            pi !180 f/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            zphi-texture  
            y-text df@ !2 f* y-text df!
            x-text df@ !3 f* x-text df!

            rd to rd-val
            br speed draw-tree
        dispose endwith
    endwith } ;

previous previous previous previous previous
