\ gl test window

dos also memory also
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

#100 Value rd-val

Create .white 1e  sf, 1e  sf, 1e  sf, 1e  sf,
Create .brown .8e sf, .4e sf, .2e  sf, 1e  sf,
Create .green .5e sf, .8e sf, .4e  sf, 1e  sf,

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

3d-turtle with
  F : leaf ( -- )
      .green .color .2e 3 sphere .brown .color ;
  F : (tree ( m n -- )  recursive
      BEGIN  dup  WHILE
             pi .3e f* roll-left
             dup .03e fm* dup .1e fm* 6 segment
             over 1 ?DO
                #1000 random rd-val <
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
      0.0001e 0e 6 segment
      dup .03e fm* 0e 6 segment
      dup .03e fm* 0e 6 segment
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
      S" pattern/bark.png" load-texture ;
[ELSE]  0 ;  [THEN]
endwith

: draw-gear  ( o alx aly alz pitch bend roll zoom speed
               shade tx sx sy sz t1 -- )
{ alx aly alz alp alb alr zoom speed br rd shade t1 |
    glcanvas with
        5e 60e w @ h @ 3d-turtle new  3d-turtle with

            .brown .color
            GL_SMOOTH glShadeModel
            shade 0 = IF  triangles  smooth on  THEN
            shade 1 = IF  textured   1 wait' +!
                          t1 .text   smooth on  THEN
            shade 2 = IF  lines      wait' off  THEN

            0 5e 5e -10e get-xyz GL_POSITION 0 set-light

            200e 0.08e f* forward
            pi f2/ up -2e forward
            zoom .1e fm* speed fm/ scale

            pi 180e f/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            zphi-texture  
            y-text df@ 2e f* y-text df!
            x-text df@ 3e f* x-text df!

            rd to rd-val
            br speed draw-tree
        dispose endwith
    endwith } ;

previous previous previous previous previous
