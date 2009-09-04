\ gl test window

dos also memory also
\needs glconst | import glconst
\ ' noop alias debug-points
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

[IFUNDEF] debug-points
Variable maxpoints
[THEN]

pi f2/   FConstant pi/2
pi/2 f2/ FConstant pi/4

Create .white .7e  sf, .7e  sf, .7e  sf, 1e  sf,
Create .green 0e  sf, .8e sf, .2e sf, 1e  sf,
Create .brown .4e sf, .2e sf, .0e sf, 1e  sf,
Create .sky   .3e sf, .6e sf, .8e sf, 1e  sf,


: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

: .text ( n -- ) ?texture [IF]
  GL_TEXTURE_2D swap glBindTexture
[ELSE]  drop .green .color  [THEN] ;

: !text ( n flag -- )
  IF  .text  ELSE  drop  THEN ;

Variable tail-time
: time' ( -- 0..2pi )
  tail-time @ [ #24 #60 #30 * * ] Literal um* drop
  0 d>f [ !$2'-8 pi f* ] FLiteral f* ;
: time-pos ( -- 0..2pi )
  tail-time @ [ #24 #60 #5 * * ] Literal um* drop
  0 d>f [ !$2'-8 pi f* ] FLiteral f* ;
: tail-wag ( n -- f )
  >r pi r@ 1 + fm* .2e f* time' f+
  fsin r> 2+ dup * 1+ fm/ 30e f* ;

3d-turtle with
  F : dragon-segment ( ri ro n -- )
      { f: ri f: ro | next-round
        ro set-r
        ri .1e set-rp  dphi sf@ phi sf@ f+ phi sf!
        1 DO  ri set-r  LOOP
        2pi phi sf! ro set-r
        0e phi sf! } ;
  F : tail-compensate ( n -- f )  0e
      0 DO  I 2+ tail-wag f+ [ 1.1e 1/f ] Fliteral f*  LOOP
      [ 1.1e 20e f** ] Fliteral f* fnegate ;
  F : dragon-tail ( ri r+ h n -- ri h )
      zphi2-texture
      { f: ri f: r+ f: h n |
      [ 1.05e -20e f** ] Fliteral
      [ 1.1e  -20e f** ] Fliteral 1e scale-xyz
      h -#15 fm* #20 tail-compensate h -#25 fm* forward-xyz
      n 1+ 0 DO  add  LOOP
      20 0 DO 0e  i 2+ tail-wag  h forward-xyz
              [ pi #90 fm/ ] Fliteral up
              ri fdup I 1 and 0= IF  r+ f+ THEN n dragon-segment
              1.05e 1.1e 1e scale-xyz
              .025e ri f+ to ri
      LOOP  ri r+ h } ;
  F : dragon-wamp ( ri r+ h ri+ n -- ri' )
      { f: ri f: r+ f: h f: ri+ n |
      8 0 DO  h forward
              ri fdup I 1 and 0= IF  r+ f+ THEN n dragon-segment
              ri+ ri f+ to ri -0.02e ri+ f+ to ri+
      LOOP  ri ri+ .02e f+ f- } ;
  F : dragon-neck-part ( ri r+ h factor angle n m -- ri' )
      swap { f: ri f: r+ f: h f: factor f: angle n |
      0 ?DO  h forward  angle left
             [ pi #30 fm/ ] Fliteral
             angle f0< IF    time' fsin .02e f* f+ down
                       ELSE  time' fsin -.02e f* f+ down
                       THEN
             factor ri f* to ri
             ri fdup I 1 and 0= IF  r+ f+ THEN n dragon-segment
      LOOP ri } ;
  F : dragon-neck ( ri r+ h angle n -- )
      { f: r+ f: h f: angle n |
      r+ h .82e angle             n 4 dragon-neck-part
      r+ h .92e angle f2/ fnegate n 6 dragon-neck-part
      fdrop close-path } ;
    Create head-xy
          0.28e sf, 0.0e sf,
          0.30e sf, 0.5e sf,
          0.25e sf, 0.6e sf,
          0.05e sf, 0.6e sf,
          0.00e sf, 0.5e sf,
          -.05e sf, 0.6e sf,
          -.10e sf, 0.6e sf,
          -.15e sf, 0.5e sf,
          -.05e sf, 0.0e sf,
  F : dragon-head ( t1 shade -- )  !text zphi-texture .66e z-off sf!
      pi 6 fm/ down  1.2e .4e .4e scale-xyz  -.65e forward
      .5e x-text sf!
      18 start-path
      6 0 DO
          I 5 = IF    .25e
                ELSE  I 0= IF 0e ELSE .35e THEN  THEN forward
          >matrix
          pi 0.1e f* I 2* 5 - fm* fcos
          fdup .5e f+ 1e scale-xyz
          next-round
          head-xy 18 cells bounds DO
              I sf@ I cell+ sf@ set-xy
              2 cells +LOOP
          head-xy dup 16 cells + DO
              I sf@ I cell+ sf@ 1e-6 f+ fnegate set-xy
              -2 cells +LOOP
          matrix>
      LOOP
      1e x-text sf!
      close-path zphi2-texture ;
  F : wing-step { f: f2 f: f3 |
      next-round
      0e f3 .05e f* f2 f-              set-xy
      f3 .1e f* f2 fnegate             set-xy
      f3 f2/ f2 fnegate                set-xy
      f3 f3 .125e f*                   set-xy
      f3 .001e f- f3 .125e f* .001e f+ set-xy
      f3 f2/ 0e                        set-xy
      f3 .1e f* 0e                     set-xy
      0e f3 .05e f*                    set-xy } ;
  F : wing-fold ( f1 f2 -- )
      time' [ pi 5 fm/ ] FLiteral f- fcos f+ f* down ;
  F : wing ( -- )
      .9e scale 8 start-path
      .02e 1.2e wing-step .3e forward
      pi #10 fm/ down  pi #8 fm/ roll-left
      time' fsin 1.2e f* right
      .02e 1e wing-step
      pi 5 fm/ up  pi #10 fm/ right  1e forward
      pi 5 fm/ down  pi #20 fm/ left
      time' fcos -.25e f* .5e f- roll-left
      time' fcos [ pi 6 fm/ ] FLiteral f* down
      [ pi 5 fm/ ] FLiteral up
      .02e .8e wing-step
      [ pi 5 fm/ ] FLiteral down
      time' 1e f- fcos 1e f+ [ pi 8 fm/ ] Fliteral f* right
      pi -.3333e f* -1.0e wing-fold
      pi #10 fm/ left 1e forward
      pi .21e f* -.8e wing-fold  pi .2e f* up
      9 0 DO
              I 4 mod 2 < IF  pi #60 fm/  ELSE  pi #30 fm/  THEN
              1.2e wing-fold
              pi #30 fm/ right
              .01e forward
              .2e z-off sf@ f+ z-off sf!
              .02e 1.8e .2e I fm* f+
              I 4 mod 2 <> IF  1.1e f*  THEN
              I 4 mod 0= IF  1.3e f*  ELSE
                  time' fcos .1e f* roll-right  THEN
              wing-step
              I 4 mod    IF  time' fcos .1e f* roll-left   THEN
      LOOP
      0e [ 1.8e .2e 6 fm* f+ 1.5e f* ] FLiteral  wing-step
      close-path ;
  F : right-wing ( h -- )
      pi/4 roll-right pi/2 right
      2e f*  forward pi .3e f* roll-left
      zp-texture .1e y-text sf! wing ;
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
      .01e fdup 3e f* 6 dragon-segment
      8 FOR  .01e I fm* fdup 3e f* 6 dragon-segment
             [ pi .075e f* ] Fliteral up .2e forward
             [ pi .075e f* ] Fliteral up  NEXT
      close-path  2dup !text ;
  F : right-leg ( text-claw flag text flag rel -- ) { f: rel |
      2dup !text
      -.25e .45e -1.2e forward-xyz pi .05e f* right
      time' rel f+ fsin .033e f* down
      1.5e 1e 1e scale-xyz  leg -.05e forward
      pi/2 down .05e forward pi .45e f* down
      time' rel f+ fsin -.033e f* down
      .5e .5e 1.33e scale-xyz leg
      -.15e forward pi .7e f* up
      time' rel f+ fsin .1e f* down
      -.15e forward .5e 1.5e .5e scale-xyz leg
      -.3e forward pi .3e f* up
      >matrix
      [ pi .2e f* ] Fliteral roll-left >matrix
      pi/2 down .4e .4e .2e scale-xyz leg matrix>
      -.1e 0e 0e forward-xyz .66e claw matrix@
      >matrix pi/2 down .6e .6e .3e scale-xyz leg matrix>
      -.2e 0e 0e forward-xyz 1e claw
      matrix>  pi .2e f* roll-right >matrix
      pi/2 down .4e .4e .2e scale-xyz leg matrix>
      -.1e 0e 0e forward-xyz .66e claw } ;
  F : dragon-body ( t0 s t3 s t1 s t3 s t2 s n -- ) >r !text
      time' fsin .1e f* 0e 0e forward-xyz
      pi f2* .2e f- r@ 1- fm/ set-dphi
      r@ 4 + open-path
      .1e .3e .2e r@ dragon-tail
      r> { f: ri f: r+ f: h n |
      ri r+ h .06e n dragon-wamp fdrop
      >turtle
         ri r+ h [  10e grad>rad ] Fliteral n dragon-neck
         2dup dragon-head  2swap !text
      turtle>  >matrix
         ri r+ h [ -10e grad>rad ] Fliteral n dragon-neck
         dragon-head
      matrix>
      h 2e f* forward
      5 pick 5 pick !text
      >turtle  h right-wing   turtle>
      >turtle  h -6e f* forward  0e right-leg   turtle>
      1e -1e 1e scale-xyz  flip-clock
      5 pick 5 pick !text
      >turtle  h right-wing   turtle>
      >turtle  h -6e f* forward  pi/4 right-leg  turtle>
      flip-clock  2drop 2drop 2drop } ;
endwith

Variable foo 4 foo !

: switch-text ( t0 t1 t2 t3 n -- tn )
\  foo @ 2/ 4 u< IF  drop foo @ 2/  THEN
  pick >r 2drop 2drop r> ;

-1 Value test-list

Create front_shininess  60.0e sf,
Create front_specular   .7e   f>fs dup , dup , , #1 ,

3d-turtle with
    F : no-smooth  smooth off ;
    
    Create shades T] textured triangles lines points no-smooth [
endwith

Variable dragons

1 floats Constant .float

struct{
.float x
.float y
.float phi
.float dphi
.float v
} .dragon

Create dragon-struct  0e f, 0e f, 0e f, pi #10 fm/ fnegate f, 3e f,

dragon-struct sizeof .dragon dragons $!

Variable last-time

: update-dragons ( -- )  tail-time @
    last-time @ 0= IF  last-time !  EXIT  THEN
    dup last-time @ - swap last-time ! s>f
    [ #24 #60 #60 * * s>f !$1'-8 f* ] FLiteral f*
    dragons $@ bounds ?DO
	fdup I .dragon v f@ f*
	I .dragon phi f@ fsincos frot funder
	f* f-rot f*
	I .dragon x f@ f+ I .dragon x f!
	I .dragon y f@ f+ I .dragon y f!
	fdup I .dragon dphi f@ f*
	I .dragon phi f@ f+ I .dragon phi f!
	sizeof .dragon +LOOP
    fdrop ;

: draw-dragon ( o alx aly alz pitch bend roll zoom
               shade sx sy sz t0 t1 t2 t3 -- )
{ alx aly alz alp alb alr zoom speed shade sx sy sz t0 t1 t2 t3 t4 |
    glcanvas with
        .sky sf@+ sf@+ sf@+ sf@ glClearColor
        2.8e 200e w @ h @ 3d-turtle new  3d-turtle with

            speed tail-time !

            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc
            GL_FOG_COLOR .sky glFogfv

            GL_FRONT_AND_BACK GL_SHININESS front_shininess
                            glMaterialfv
            GL_FRONT_AND_BACK GL_SPECULAR front_specular
                            glMaterialfv
            GL_TEXTURE_2D GL_TEXTURE_MIN_FILTER GL_NEAREST glTexParameteri
            GL_TEXTURE_2D GL_TEXTURE_MAG_FILTER GL_LINEAR glTexParameteri

            shade $F0 and 4 >> .01e fm* set-fog

            smooth on  shade 7 and cells shades + perform

            shade 7 and 0<> IF    .green .color
                            ELSE  .white .color 1 foo +!  THEN

            0 5e -5e -10e get-xyz GL_POSITION 0 set-light

            8e forward

            zoom 0.02e fm* scale

            pi #180 fm/
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
                 alr fm* roll-left

            .01e sx fm* .01e sy fm* .01e sz fm* scale-xyz

            test-list 0< IF  1 glGenLists TO test-list  THEN

            shade 8 and
	    IF  test-list GL_COMPILE glNewList  THEN
	    dragons $@ bounds ?DO
		>matrix
		0e I .dragon x f@ I .dragon y f@ forward-xyz
		I .dragon phi f@ right
		I .dragon dphi f@ I .dragon v f@ f2/ f**2 f* fatan roll-left

		t3  shade 7 and 0=
                t4  over
                t0  over
                t1  over
                t2  over
                t1  over
		
		shade 8 >> dragon-body
		matrix>
	    sizeof .dragon +LOOP
	    shade 8 and
            IF  glEndList                \ cr .time
                test-list glCallList     \    .time
                test-list 1 glDeleteLists
	    THEN
	    update-dragons
        turtle>
    endwith } ;

previous previous previous previous previous
