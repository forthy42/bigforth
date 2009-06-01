\ gl test window

\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

#100 Value rd-val

Create .white 1e  f>fs , 1e  f>fs , 1e  f>fs ,  1e  f>fs ,
Create .brown .8e f>fs , .4e f>fs , .2e  f>fs , 1e  f>fs ,
Create .green .4e f>fs , .9e f>fs , .5e  f>fs , 1e  f>fs ,
Create .mix   0e f>fs ,   0e f>fs ,   0e f>fs ,    1e  f>fs ,
Create .sky   .3e f>fs , .6e f>fs , .8e f>fs  , 1e  f>fs ,
Create .rot   1e  f>fs , .2e f>fs , .2e  f>fs ,  1e  f>fs ,
Create .straw .8e f>fs , .9e f>fs , .5e  f>fs , 1e  f>fs ,

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

: mix-color ( f1 -- ) 1e fover f-
  4 0 DO  fover I sfloats .green + sf@ f*
          fover I sfloats .brown + sf@ f* f+
                I sfloats .mix + sf!  LOOP
  fdrop fdrop .mix .color ;

: set-color ( n -- )
    2* s>f .8e fswap f** mix-color ;

Create front_shininess  20.0e f>fs ,
Create front_specular   1e   f>fs dup , dup , , #1 ,
Create no_specular      0e   f>fs dup , dup , , #1 ,

[defined] xft [IF]
3d-text ptr merry-text
3d-text ptr xmas-text
[THEN]

[defined] fm** 0= [IF]
    : fm** s>f f** ;
[THEN]

forward tree-branch

3d-turtle with
  F : down-branch ( n -- )
      1.6e fm** fatan .02e f* down ;
  F : merry-xmas ( -- )
      .rot .color
      pi f2/ down pi f2/ roll-left 0e 0e -.1e forward-xyz
[defined] xft [IF]
      1 2 ^ merry-text draw
      1 0 ^ xmas-text draw
[THEN]
  ;
  F : stroh-stern ( n -- ) { n f: r }
      pi n fm/ right
      r fnegate forward
      pi n 2* fm/ left
      n 0 DO  .05e r f* fdup r f2* 6 cylinder
	  pi n 1- n fm*/ right  LOOP ;
  F : rnd-angle ( -- f ) #1000 random #500 - .0001e fm* ;
  F : rnd-move ( -- )
      rnd-angle up  rnd-angle left  rnd-angle roll-left ;
  F : color-cylinder ( n r -- )
      f>r over set-color fr>
      + .01e fm* fover f* fswap .3e f* 6 segment ;
  F : gen-rnd ( n -- n flag )
      -1 over 0 ?DO  #1000 random rd-val < and  LOOP ;
  F : xmas-sphere ( -- )
      >matrix
      scale@ { f: x f: y f: z }
      1e x y f/ fsqrt x z f/ fsqrt scale-xyz
      ortho
      -.2e 0e -.3e forward-xyz  .rot .color
      GL_FRONT_AND_BACK GL_SPECULAR front_specular glMaterialfv
      pi f2/ up -.05e forward .1e 7 sphere 
      GL_FRONT_AND_BACK GL_SPECULAR no_specular glMaterialfv
      matrix> ;
  F : sub-branch ( f v -- )
      >turtle >r
      [ pi .2e f* ] Fliteral fm* right  dup down-branch
      2dup .9e dup fm**
      dup 2 > over 3 and 0= and r> and tree-branch
      turtle> ;
  F : tree-branch ( m n f flag -- )  >r
      1e 1e frot scale-xyz
      dup 4+ .01e fm* 0e 6 segment
      BEGIN  1- dup  WHILE  rnd-move
	  dup 4 1e color-cylinder
	  gen-rnd IF   1 r@ sub-branch  THEN
	  gen-rnd IF  -1 r@ sub-branch  THEN
	  [ pi .025e f* ] Fliteral down
      REPEAT
      dup 4 1e color-cylinder
      .1e  forward  end-path  2drop
      r> IF  xmas-sphere  THEN ;
  F : main-branch  1e dup 1 > tree-branch ;
  F : main-tree ( m n -- ) 0e { f: di }
      BEGIN  1- dup  WHILE
             [ pi 0.2e f* ] Fliteral roll-left
             dup 1 2e color-cylinder
             over 0 ?DO
                 2pi I I' fm*/ to di
                 >turtle
                     di roll-left pi 3 fm/ right
                     pi f2/ roll-right
                     2 over main-branch
                 turtle>
             LOOP
      REPEAT
      dup 1 2e color-cylinder
      .1e forward  end-path
      .straw .color  pi f2/ f2/ roll-left
      .04e 0e -.2e forward-xyz
      >matrix .4e #11 stroh-stern matrix> >matrix
      pi #11 fm/ right .3e #11 stroh-stern matrix> merry-xmas
      2drop ;
  F : draw-tree ( n -- ) rd-val seed !
      [ pi .2e f* ] Fliteral dup 1- fm*
      pi f2/ f2/ f+ roll-right
      pi 3 fm/ set-dphi
      6 start-path
      0.0001e 0e 6 segment
      dup .02e fm* 0e 6 segment
      dup .02e fm* 0e 6 segment
      main-tree ;      
endwith

: draw-xmas-tree  ( o alx aly alz pitch bend roll zoom speed
               shade tx -- )
{ alx aly alz alp alb alr zoom speed br rd shade }
    glcanvas with
        .sky sf@+ sf@+ sf@+ sf@ glClearColor
\            w @ h @ min
            5e 60e w @ h @ 3d-turtle new  3d-turtle with

            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc
            GL_FRONT_AND_BACK GL_SHININESS front_shininess
                            glMaterialfv

            shade 0 = IF  triangles  smooth on  THEN
            shade 1 = IF  triangles  smooth off THEN
            shade 2 = IF  lines      smooth off THEN

            0 5e 5e -10e get-xyz GL_POSITION 0 set-light

            16e forward
            pi f2/ up
            zoom .1e fm* speed fm/ scale
            -.3e speed fm* forward

            pi 180e f/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            rd to rd-val
            br speed draw-tree
        dispose endwith
    endwith ;

previous previous previous
