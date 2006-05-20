\ gl test window

dos also memory also
\needs glconst | import glconst
' noop alias debug-points
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

[IFUNDEF] debug-points
Variable maxpoints
[THEN]

pi f2/   FConstant pi/2
pi/2 f2/ FConstant pi/4

Create .white !&.7  f>fs , !&.7  f>fs , !&.7  f>fs  , !1  f>fs ,
Create .green !&0  f>fs , !&.8 f>fs , !&.2 f>fs  , !1  f>fs ,
Create .brown !&.4 f>fs , !&.2 f>fs , !&.0 f>fs  , !1  f>fs ,
Create .sky   !&.3 f>fs , !&.6 f>fs , !&.8 f>fs  , !1  f>fs ,


: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

: .text ( n -- ) ?texture [IF]
  GL_TEXTURE_2D swap glBindTexture
[ELSE]  drop .green .color  [THEN] ;

: !text ( n flag -- )
  IF  .text  ELSE  drop  THEN ;

Variable tail-time
: time' ( -- 0..2pi )
  tail-time @ [ &24 &60 &30 * * ] Literal um* drop
  0 d>f [ !$2'-8 pi f* ] FLiteral f* ;
: tail-wag ( n -- f )
  >r pi r@ 1 + fm* !.2 f* time' f+
  fsin r> 2+ dup * 1+ fm/ !30 f* ;

3d-turtle with
  F : dragon-segment ( ri ro n -- )
      { f: ri f: ro | next-round
        ro set-r
        ri !.1 set-rp  dphi sf@ phi sf@ f+ phi sf!
        1 DO  ri set-r  LOOP
        2pi phi sf! ro set-r
        !0 phi sf! } ;
  F : tail-compensate ( n -- f )  !0
      0 DO  I 2+ tail-wag f+ [ !1.1 1/f ] Fliteral f*  LOOP
      [ !1.1 !20 f** ] Fliteral f* fnegate ;
  F : dragon-tail ( ri r+ h n -- ri h )
      zphi2-texture
      { f: ri f: r+ f: h n |
      [ !1.05 !-20 f** ] Fliteral
      [ !1.1  !-20 f** ] Fliteral !1 scale-xyz
      h -&15 fm* &20 tail-compensate h -&25 fm* forward-xyz
      n 1+ 0 DO  add  LOOP
      20 0 DO !0  i 2+ tail-wag  h forward-xyz
              [ pi &90 fm/ ] Fliteral up
              ri fdup I 1 and 0= IF  r+ f+ THEN n dragon-segment
              !1.05 !1.1 !1 scale-xyz
              !.025 ri f+ to ri
      LOOP  ri r+ h } ;
  F : dragon-wamp ( ri r+ h ri+ n -- ri' )
      { f: ri f: r+ f: h f: ri+ n |
      8 0 DO  h forward
              ri fdup I 1 and 0= IF  r+ f+ THEN n dragon-segment
              ri+ ri f+ to ri !-0.02 ri+ f+ to ri+
      LOOP  ri ri+ !.02 f+ f- } ;
  F : dragon-neck-part ( ri r+ h factor angle n m -- ri' )
      swap { f: ri f: r+ f: h f: factor f: angle n |
      0 ?DO  h forward  angle left
             [ pi &30 fm/ ] Fliteral
             angle f0< IF    time' fsin !.02 f* f+ down
                       ELSE  time' fsin !-.02 f* f+ down
                       THEN
             factor ri f* to ri
             ri fdup I 1 and 0= IF  r+ f+ THEN n dragon-segment
      LOOP ri } ;
  F : dragon-neck ( ri r+ h angle n -- )
      { f: r+ f: h f: angle n |
      r+ h !.82 angle             n 4 dragon-neck-part
      r+ h !.92 angle f2/ fnegate n 6 dragon-neck-part
      fdrop close-path } ;
    Create head-xy
          !0.28 f>fs , !0.0 f>fs ,
          !0.30 f>fs , !0.5 f>fs ,
          !0.25 f>fs , !0.6 f>fs ,
          !0.05 f>fs , !0.6 f>fs ,
          !0.00 f>fs , !0.5 f>fs ,
          !-.05 f>fs , !0.6 f>fs ,
          !-.10 f>fs , !0.6 f>fs ,
          !-.15 f>fs , !0.5 f>fs ,
          !-.05 f>fs , !0.0 f>fs ,
  F : dragon-head ( t1 shade -- )  !text zphi-texture !.66 z-off sf!
      pi 6 fm/ down  !1.2 !.4 !.4 scale-xyz  !-.65 forward
      !.5 x-text sf!
      18 start-path
      6 0 DO
          I 5 = IF    !.25
                ELSE  I 0= IF !0 ELSE !.35 THEN  THEN forward
          >matrix
          pi !0.1 f* I 2* 5 - fm* fcos
          fdup !.5 f+ !1 scale-xyz
          next-round
          head-xy 18 cells bounds DO
              I sf@ I cell+ sf@ set-xy
              2 cells +LOOP
          head-xy dup 16 cells + DO
              I sf@ I cell+ sf@ !1'-6 f+ fnegate set-xy
              -2 cells +LOOP
          matrix>
      LOOP
      !1 x-text sf!
      close-path zphi2-texture ;
  F : wing-step { f: f2 f: f3 |
      next-round
      !0 f3 !.05 f* f2 f-              set-xy
      f3 !.1 f* f2 fnegate             set-xy
      f3 f2/ f2 fnegate                set-xy
      f3 f3 !.125 f*                   set-xy
      f3 !.001 f- f3 !.125 f* !.001 f+ set-xy
      f3 f2/ !0                        set-xy
      f3 !.1 f* !0                     set-xy
      !0 f3 !.05 f*                    set-xy } ;
  F : wing-fold ( f1 f2 -- )
      time' [ pi 5 fm/ ] FLiteral f- fcos f+ f* down ;
  F : wing ( -- )
      !.9 scale 8 start-path
      !.02 !1.2 wing-step !.3 forward
      pi &10 fm/ down  pi &8 fm/ roll-left
      time' fsin !1.2 f* right
      !.02 !1 wing-step
      pi 5 fm/ up  pi &10 fm/ right  !1 forward
      pi 5 fm/ down  pi &20 fm/ left
      time' fcos !-.25 f* !.5 f- roll-left
      time' fcos [ pi 6 fm/ ] FLiteral f* down
      [ pi 5 fm/ ] FLiteral up
      !.02 !.8 wing-step
      [ pi 5 fm/ ] FLiteral down
      time' !1 f- fcos !1 f+ [ pi 8 fm/ ] Fliteral f* right
      pi !-.3333 f* !-1.0 wing-fold
      pi &10 fm/ left !1 forward
      pi !.21 f* !-.8 wing-fold  pi !.2 f* up
      9 0 DO
              I 4 mod 2 < IF  pi &60 fm/  ELSE  pi &30 fm/  THEN
              !1.2 wing-fold
              pi &30 fm/ right
              !.01 forward
              !.2 z-off sf@ f+ z-off sf!
              !.02 !1.8 !.2 I fm* f+
              I 4 mod 2 <> IF  !1.1 f*  THEN
              I 4 mod 0= IF  !1.3 f*  ELSE
                  time' fcos !.1 f* roll-right  THEN
              wing-step
              I 4 mod    IF  time' fcos !.1 f* roll-left   THEN
      LOOP
      !0 [ !1.8 !.2 6 fm* f+ !1.5 f* ] FLiteral  wing-step
      close-path ;
  F : right-wing ( h -- )
      pi/4 roll-right pi/2 right
      !2 f*  forward pi !.3 f* roll-left
      zp-texture !.1 y-text sf! wing ;
  F : leg ( -- )
      pi/4 set-dphi
      9 start-path
      9 -8 DO
         64 I dup * - s>f fsqrt !$.06 f* fdup 8 dragon-segment
         I 8+ $F over - min 2 > IF  4 !$.6  ELSE  1 !$.18  THEN
         forward
      +LOOP  close-path ;
  F : claw ( fn -- )  scale  2over !text
      pi roll-left pi 3 fm/ set-dphi
      7 start-path
      !.01 fdup !3 f* 6 dragon-segment
      8 FOR  !.01 I fm* fdup !3 f* 6 dragon-segment
             [ pi !.075 f* ] Fliteral up !.2 forward
             [ pi !.075 f* ] Fliteral up  NEXT
      close-path  2dup !text ;
  F : right-leg ( text-claw flag text flag rel -- ) { f: rel |
      2dup !text
      !-.25 !.45 !-1.2 forward-xyz pi !.05 f* right
      time' rel f+ fsin !.033 f* down
      !1.5 !1 !1 scale-xyz  leg !-.05 forward
      pi/2 down !.05 forward pi !.45 f* down
      time' rel f+ fsin !-.033 f* down
      !.5 !.5 !1.33 scale-xyz leg
      !-.15 forward pi !.7 f* up
      time' rel f+ fsin !.1 f* down
      !-.15 forward !.5 !1.5 !.5 scale-xyz leg
      !-.3 forward pi !.3 f* up
      >matrix
      [ pi !.2 f* ] Fliteral roll-left >matrix
      pi/2 down !.4 !.4 !.2 scale-xyz leg matrix>
      !-.1 !0 !0 forward-xyz !.66 claw matrix@
      >matrix pi/2 down !.6 !.6 !.3 scale-xyz leg matrix>
      !-.2 !0 !0 forward-xyz !1 claw
      matrix>  pi !.2 f* roll-right >matrix
      pi/2 down !.4 !.4 !.2 scale-xyz leg matrix>
      !-.1 !0 !0 forward-xyz !.66 claw } ;
  F : dragon-body ( t0 s t3 s t1 s t3 s t2 s n -- ) >r
      2over !text
      time' fsin !.1 f* !0 !0 forward-xyz
      pi f2* !.2 f- r@ 1- fm/ set-dphi
      r@ 4 + open-path
      !.1 !.3 !.2 r@ dragon-tail
      r> { f: ri f: r+ f: h n |
      ri r+ h !.06 n dragon-wamp fdrop
      >turtle
         ri r+ h [  !10 grad>rad ] Fliteral n dragon-neck
         2dup dragon-head  2swap !text
      turtle>  >matrix
         ri r+ h [ !-10 grad>rad ] Fliteral n dragon-neck
         dragon-head
      matrix>
      h !2 f* forward
      5 pick 5 pick !text
      >turtle  h right-wing   turtle>
      >turtle  h !-6 f* forward  !0 right-leg   turtle>
      !1 !-1 !1 scale-xyz  flip-clock
      5 pick 5 pick !text
      >turtle  h right-wing   turtle>
      >turtle  h !-6 f* forward  pi/4 right-leg  turtle>
      flip-clock  2drop 2drop 2drop } ;
endwith

Variable foo 4 foo !

: switch-text ( t0 t1 t2 t3 n -- tn )
\  foo @ 2/ 4 u< IF  drop foo @ 2/  THEN
  pick >r 2drop 2drop r> ;

-1 Value test-list

Create front_shininess  !&60.0 f>fs ,
Create front_specular   !&.7   f>fs dup , dup , , #1 ,

3d-turtle with
    F : no-smooth  smooth off ;
    
    Create shades T] textured triangles lines points no-smooth textured-poly poly [
endwith

: draw-dragon ( o alx aly alz pitch bend roll zoom
               shade sx sy sz t0 t1 t2 t3 -- )
{ alx aly alz alp alb alr zoom speed shade sx sy sz t0 t1 t2 t3 |
    glcanvas with
        .sky sf@+ sf@+ sf@+ sf@ glClearColor
        !2.8 !200 w @ h @ 3d-turtle new  3d-turtle with

            speed tail-time !

            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc
            GL_FOG_COLOR .sky glFogfv

            GL_FRONT_AND_BACK GL_SHININESS front_shininess
                            glMaterialfv
            GL_FRONT_AND_BACK GL_SPECULAR front_specular
                            glMaterialfv

            shade $F0 and 4 >> !.01 fm* set-fog

            smooth on  shade 7 and cells shades + perform

            shade 7 and dup 0<> swap 5 <> and
            IF  .green .color  ELSE  .white .color 1 foo +!  THEN

            0 !5 !-5 !-10 get-xyz GL_POSITION 0 set-light

            !8 forward

            zoom !&0.02 fm* scale

            pi &180 fm/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            !.01 sx fm* !.01 sy fm* !.01 sz fm* scale-xyz

            t0 t1 t2 t3 1 switch-text  shade 7 and 0=
            t0 t1 t2 t3 0 switch-text  over
            t0 t1 t2 t3 3 switch-text  over
            t0 t1 t2 t3 3 switch-text  over
            t0 t1 t2 t3 2 switch-text  over

            test-list 0< IF  1 glGenLists TO test-list  THEN

            shade 8 and
            IF  test-list GL_COMPILE glNewList  THEN
            shade 8 >> dragon-body
            shade 8 and
            IF  glEndList                \ cr .time
                test-list glCallList     \    .time
                test-list 1 glDeleteLists
            THEN
        turtle>
    endwith } ;

previous previous previous previous previous
