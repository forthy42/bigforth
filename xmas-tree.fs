\ gl test window

\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

&100 Value rd-val

Create .white !&1  f>fs , !&1  f>fs , !&1  f>fs ,  !1  f>fs ,
Create .brown !&.8 f>fs , !&.4 f>fs , !&.2  f>fs , !1  f>fs ,
Create .green !&.4 f>fs , !&.9 f>fs , !&.5  f>fs , !1  f>fs ,
Create .mix   !0 f>fs ,   !0 f>fs ,   !0 f>fs ,    !1  f>fs ,
Create .sky   !&.3 f>fs , !&.6 f>fs , !&.8 f>fs  , !1  f>fs ,
Create .rot   !&1  f>fs , !&.2 f>fs , !&.2  f>fs ,  !1  f>fs ,
Create .straw !&.8 f>fs , !&.9 f>fs , !&.5  f>fs , !1  f>fs ,

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

: mix-color ( f1 -- ) !1 fover f-
  4 0 DO  fover I sfloats .green + sf@ f*
          fover I sfloats .brown + sf@ f* f+
                I sfloats .mix + sf!  LOOP
  fdrop fdrop .mix .color ;

: set-color ( n -- )
    2* s>f !.8 fswap f** mix-color ;

Create front_shininess  !&20.0 f>fs ,
Create front_specular   !&1   f>fs dup , dup , , #1 ,
Create no_specular      !0   f>fs dup , dup , , #1 ,

3d-text ptr merry-text
3d-text ptr xmas-text

forward tree-branch

3d-turtle with
  F : down-branch ( n -- )
      !1.6 fm** fatan !.02 f* down ;
  F : merry-xmas ( -- )
      .rot .color
      pi f2/ down pi f2/ roll-left 0e 0e -.1e forward-xyz
      1 2 ^ merry-text draw
      1 0 ^ xmas-text draw
  ;
  F : stroh-stern ( n -- ) { n f: r |
      pi n fm/ right
      r fnegate forward
      pi n 2* fm/ left
      n 0 DO  !.05 r f* fdup r f2* 6 cylinder
	  pi n 1- n fm*/ right  LOOP } ;
  F : rnd-angle ( -- f ) &1000 random &500 - !.0001 fm* ;
  F : rnd-move ( -- )
      rnd-angle up  rnd-angle left  rnd-angle roll-left ;
  F : color-cylinder ( n r -- )
      f>r over set-color fr>
      + !.01 fm* fover f* fswap !.3 f* 6 segment ;
  F : gen-rnd ( n -- n flag )
      -1 over 0 ?DO  &1000 random rd-val < and  LOOP ;
  F : xmas-sphere ( -- )
      >matrix
      scale@ { f: x f: y f: z |
        !1 x y f/ fsqrt x z f/ fsqrt scale-xyz }
      ortho
      !-.2 !0 !-.3 forward-xyz  .rot .color
      GL_FRONT_AND_BACK GL_SPECULAR front_specular glMaterialfv
      pi f2/ up !-.05 forward !.1 7 sphere 
      GL_FRONT_AND_BACK GL_SPECULAR no_specular glMaterialfv
      matrix> ;
  F : sub-branch ( f v -- )
      >turtle >r
      [ pi !.2 f* ] Fliteral fm* right  dup down-branch
      2dup !.9 dup fm**
      dup 2 > over 3 and 0= and r> and tree-branch
      turtle> ;
  F : tree-branch ( m n f flag -- )  >r
      !1 !1 frot scale-xyz
      dup 4+ !.01 fm* !0 6 segment
      BEGIN  1- dup  WHILE  rnd-move
	  dup 4 !1 color-cylinder
	  gen-rnd IF   1 r@ sub-branch  THEN
	  gen-rnd IF  -1 r@ sub-branch  THEN
	  [ pi !.025 f* ] Fliteral down
      REPEAT
      dup 4 !1 color-cylinder
      !.1  forward  end-path  2drop
      r> IF  xmas-sphere  THEN ;
  F : main-branch  !1 dup 1 > tree-branch ;
  F : main-tree ( m n -- )
      BEGIN  1- dup  WHILE
             [ pi !0.2 f* ] Fliteral roll-left
             dup 1 !2 color-cylinder
             over 0 ?DO
                 2pi I I' fm*/ { f: di |
                 >turtle
                     di roll-left pi 3 fm/ right
                     pi f2/ roll-right
                     2 over main-branch
                 turtle> }
             LOOP
      REPEAT
      dup 1 !2 color-cylinder
      !.1 forward  end-path
      .straw .color  pi f2/ f2/ roll-left
      !.04 !0 !-.2 forward-xyz
      >matrix !.4 &11 stroh-stern matrix> >matrix
      pi &11 fm/ right !.3 &11 stroh-stern matrix> merry-xmas
      2drop ;
  F : draw-tree ( n -- )  rd-val seed !
      [ pi !.2 f* ] Fliteral dup 1- fm*
      pi f2/ f2/ f+ roll-right
      pi 3 fm/ set-dphi
      6 start-path
      !0.0001 !0 6 segment
      dup !.02 fm* !0 6 segment
      dup !.02 fm* !0 6 segment
      main-tree ;      
endwith

: draw-xmas-tree  ( o alx aly alz pitch bend roll zoom speed
               shade tx -- )
{ alx aly alz alp alb alr zoom speed br rd shade |
    glcanvas with
        .sky sf@+ sf@+ sf@+ sf@ glClearColor
\            w @ h @ min
            !5 !60 w @ h @ 3d-turtle new  3d-turtle with

            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc
            GL_FRONT_AND_BACK GL_SHININESS front_shininess
                            glMaterialfv

            shade 0 = IF  triangles  smooth on  THEN
            shade 1 = IF  triangles  smooth off THEN
            shade 2 = IF  lines      smooth off THEN

            0 !5 !5 !-10 get-xyz GL_POSITION 0 set-light

            !16 forward
            pi f2/ up
            zoom !.1 fm* speed fm/ scale
            !-.3 speed fm* forward

            pi !180 f/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            rd to rd-val
            br speed draw-tree
        dispose endwith
    endwith } ;

previous previous previous
