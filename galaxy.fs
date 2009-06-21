\ compute rotation behaviour of galaxies               21may00py

\needs float     import float
\needs output-file  include fileop.fb
[IFDEF] glcanvas
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
[THEN]
float also
[IFDEF] glcanvas
glconst also opengl also
[THEN]

1 dfloats Constant dfloat

struct{
  dfloat x
  dfloat y
  dfloat z
  dfloat msum
  dfloat ax
  dfloat ay
  dfloat az
  dfloat ax+
  dfloat ay+
  dfloat az+
} element

: elements  sizeof element * ; macro

\ star structure

FVariable msum+

Variable stars

: star ( i -- )  elements stars @ cell+ + ; macro

$100 Value star#
$000 Value central#

: init-stars ( -- )  s" " stars $!
  star# 1+ elements stars $!len  stars $@ erase ;

\ helper words

: pick-circle ( -- x y r )
    BEGIN
        frnd f2* 1e f- frnd f2* 1e f-
        fover f**2 fover f**2 f+ fdup 1e f> fdup f0= or  WHILE
        fdrop fdrop fdrop
    REPEAT ;

: xyz@ ( n -- x y z )
  dup element x df@
  dup element y df@
      element z df@ ;
: -xyz@ ( n -- x y z )
  dup element x df@ fnegate
  dup element y df@ fnegate
      element z df@ fnegate ;
: xyz! ( x y z n -- )
  dup element z df!
  dup element y df!
      element x df! ;

FVariable oldgauss 0e oldgauss f!

: fgauss ( -- f )
    oldgauss f@ fdup f0> IF  0e oldgauss f!  EXIT  THEN
    fdrop pick-circle
    fdup fln -2 fm* fswap f/ .1e f*
    funder f* oldgauss f! f* ;

: set-bulge ( n d1 d2 -- ) { f: d1 f: d2 |
    0 ?DO
	BEGIN  frnd f2* 1e f-
	    frnd f2* 1e f-
	    frnd f2* 1e f-
	    fover2 f**2 fover2 f**2 f+ fover f**2 f+
	    1e f> WHILE  fdrop fdrop fdrop  REPEAT
	fover2 f**2 fover2 f**2 f+ fover f**2 f+ fsqrt fsqrt fsqrt
	I star
	fdup d1 f* fswap d2 f* { f: x1 f: x2 |
	x1 f* dup element x df!
	x1 f* dup element y df!
	( f**2 ) x2 f*     element z df! }
    LOOP } ;

\ units: kg, meters, seconds
\ rotations per day

: set-earth-msum+ ( -- )
    331950e 149450e6 f**2 f/ \ sun
    7.349e22 5.974e24 f/ 384.400e6 f**2 f/ f+ \ moon, smaller
    star# central# + star# fm*/ msum+ f! ;

: set-earth ( -- )  6.37739715e6 6.35607896e6 fover 5.31800028336e-9 f*
    { f: d1 f: d2 f: as |
    star# 0 ?DO
	BEGIN  frnd f2* 1e f-
	    frnd f2* 1e f-
	    frnd f2* 1e f-
	    fover2 f**2 fover2 f**2 f+ fover f**2 f+
	    1e f> WHILE  fdrop fdrop fdrop  REPEAT
	I star
	0e           dup element az df!
	fover  as f* dup element ay df!
	fdup   as f* dup element ax df!
	d1 f* dup element x df!
	d1 f* dup element y df!
	d2 f*     element z df!
    LOOP }
    star# 0.13e fm* f>s to central#
    set-earth-msum+ ;

Variable spiral-dist spiral-dist on

: set-spiral ( n1 n2 di ds dz dp sf -- ) { f: di f: ds f: dz f: dp f: sf }
    swap ?DO  BEGIN  frnd f**2 ( t )
            frnd fover fover 1e f+ 1/f f> spiral-dist @ and WHILE
            fdrop fdrop  REPEAT ( ft fr )
        rnd >r
        funder 1e f+ f/
        r@ $1 and IF fnegate THEN
        fswap di f+ ds f*
        frnd .5e f- .6e f* fover f* f+ \ <<<
        frnd f**2 -5e f* fexp dp f* 10e f* fover f/ 
        frnd .5e f- f* fover f/
        fover ds f/ di f- f**2 pi f2/ f* sf f* f+ \ spiral factor
        fgauss ( was frnd ) dp f* f+
        r> $2 and IF pi f+ THEN
        fsincos frot funder f* f-rot f*
        I star dup element x df!
               dup element y df!
        dz f*      element z df!
    LOOP ;

: set-s0 ( n1 n2 di ds dz dp sf -- ) { f: di f: ds f: dz f: dp f: sf }
    swap ?DO  BEGIN  frnd f**2 ( t )
            frnd f2* f2* fexp 12e f/ spiral-dist @ 0= WHILE
            fdrop fdrop  REPEAT ( ft fr )
        rnd >r
	funder 1e f+ f/
        r@ $1 and IF fnegate THEN
        fswap di f+ ds f*
	frnd pi f*
        r> $2 and IF pi f+ THEN
        fsincos frot funder f* f-rot f*
        I star dup element x df!
               dup element y df!
        dz f*      element z df!
    LOOP ;

: DFVariable  Create 1 dfloats allot ;

DFVariable >x
DFVariable >y
DFVariable >z

: >xyz ( n -- )
  dup element x df@ >x df!
  dup element y df@ >y df!
      element z df@ >z df! ;

\ : dxyz@ ( addr -- x y z )
\   dup element x df@ >x df@ f-
\   dup element y df@ >y df@ f-
\       element z df@ >z df@ f- ;
Code dxyz@ ( addr -- fx fy fz )
     .fl 0 element x AX D) fld  .fl >x #) fsub
     .fl 0 element y AX D) fld  .fl >y #) fsub
     .fl 0 element z AX D) fld  .fl >z #) fsub
     AX pop  next end-code macro
Code dxyz@abs ( addr -- fr² )
     .fl 0 element x AX D) fld  .fl >x #) fsub  0 ST fmul
     .fl 0 element y AX D) fld  .fl >y #) fsub  0 ST fmul  1 STP fadd
     .fl 0 element z AX D) fld  .fl >z #) fsub  0 ST fmul  1 STP fadd
     AX pop  next end-code macro
Code -dxyz@ ( addr -- -fx -fy -fz )
     .fl 0 element x AX D) fld  .fl >x #) fadd  fchs
     .fl 0 element y AX D) fld  .fl >y #) fadd  fchs
     .fl 0 element z AX D) fld  .fl >z #) fadd  fchs
     AX pop  next end-code macro
Code -dxyz@abs ( addr -- fr² )
     .fl 0 element x AX D) fld  .fl >x #) fadd  0 ST fmul
     .fl 0 element y AX D) fld  .fl >y #) fadd  0 ST fmul  1 STP fadd
     .fl 0 element z AX D) fld  .fl >z #) fadd  0 ST fmul  1 STP fadd
     AX pop  next end-code macro
: a@ ( n -- x y z )
  dup element ax df@
  dup element ay df@
      element az df@ ;
\ : a+@ ( n -- x y z )
\   dup element ax df@ dup element ax+ df@ f+
\   dup element ay df@ dup element ay+ df@ f+
\   dup element ay df@     element az+ df@ f+ ;

Code a+@ ( n -- fx fy fz )
     .fl 0 element ax AX D) fld  .fl 0 element ax+ AX D) fadd
     .fl 0 element ay AX D) fld  .fl 0 element ay+ AX D) fadd
     .fl 0 element az AX D) fld  .fl 0 element az+ AX D) fadd
     AX pop  Next end-code macro

Code -a+@ ( n -- fx fy fz )
     .fl 0 element ax AX D) fld  .fl 0 element ax+ AX D) fadd  fchs
     .fl 0 element ay AX D) fld  .fl 0 element ay+ AX D) fadd  fchs
     .fl 0 element az AX D) fld  .fl 0 element az+ AX D) fadd  fchs
     AX pop  Next end-code macro

: vdup  fover2 fover2 fover2 ; macro
: vabs  f**2 fswap f**2 f+ fswap f**2 f+ ; macro
: vdup-abs  fover2 f**2 fover2 f**2 f+ fover f**2 f+ ; macro
Code vscale ( f1 f2 f3 fs -- f1' f2' f3' )
     1 <ST fmul  2 <ST fmul 3 STP fmul
     Next end-code macro
Code v+ ( v1 v2 -- )
     3 STP fadd  3 STP fadd  3 STP fadd  Next end-code macro

: set-msum ( -- )
  star# 0 ?DO  I star dup >xyz
      >x df@ >y df@ >z df@ vabs 1/f central# fm*
      0 star star# elements bounds ?DO  dup I <>
          IF  I  dxyz@abs 1/f f+  THEN
              I -dxyz@abs 1/f f+
      sizeof element +LOOP  drop
      star# 2* 1- central# + fm/ I star element msum df!
  pause LOOP ;

0.5e msum+ f!

: set-a ( -- )
  star# 0 ?DO  I star dup >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* fnegate vscale
      0 star star# elements bounds ?DO  dup I <>
          IF   I dxyz@ vdup-abs
               fdup fsqrt f* 1/f vscale v+  THEN
          I -dxyz@ vdup-abs
          fdup fsqrt f* 1/f vscale v+
      sizeof element +LOOP drop
      I star 1e star# 2* 1- central# + fm/ vscale
      dup element az df!
      dup element ay df!
          element ax df!
  pause LOOP ;

Variable dirsens  dirsens on

: approx ( f addr -- )
    dup df@ fover f- f2/ f- df! ;

: set-a+ ( -- )
  star# 0 ?DO  I star dup >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* vscale
      0 star star# elements bounds ?DO  dup I <>
          IF  I dup  a+@  dxyz@abs 1/f vscale v+  THEN
              I dup -a+@ -dxyz@abs 1/f vscale v+
      sizeof element +LOOP drop
      I star 1e star# 2* 1- central# + fm/
      dup element msum df@ msum+ f@ f+ f/ vscale
      dup element az+ approx
      dup element ay+ approx
          element ax+ approx
  pause LOOP ;

0 value s0-galaxies

: set-masses ( n dp sf -- )  >r >r init-stars
    star# swap #100 */ -4 and dup >r 1e .33e set-bulge
    r> star# .3e 2.25e .2e r> .01e fm* r> .01e fm*
    s0-galaxies IF  set-s0  ELSE  set-spiral  THEN ;

#30 #40 #100 set-masses

0 [IF]
332946 Constant sun-mass

: set-masses ( -- )  2drop drop 1 to star#  init-stars
  0 star
  1e dup element x df!
  0e dup element y df!
  0e     element z df!
  sun-mass to central# ;

0 0 0 set-masses
[THEN]

Variable vis-mass
Variable vis-a+
Variable vis-a
0 Value vis-array
Variable vis-max
Variable a-pos $20 a-pos !

$40 Value vismax
: visminmax 0 max vismax 1- min ;
: vis* vismax s0-galaxies IF 12 ELSE 4 THEN fm*/
    .5e f+ ff>s visminmax ;
: vis'* vismax $40 * fm* .5e f+ ff>s ;

: fsqsum ( x y z -- d ) f**2 fswap f**2 f+ fswap f**2 f+ ;
: !vis-array ( addr -- )  to vis-array
    s" " vis-array $!
    vismax 2+ 2* cells vis-array $!len  vis-array $@ erase ;
: r#@ ( addr -- r )  xyz@ fsqsum fsqrt ;
: vis+ ( val i -- )
    star r#@ vis* 1+ 2* cells
    vis-array $@ drop + >r 2 r@ +! dup 2* r@ cell+ +!
    r> 2 cells - >r  1 r@ +! dup r@ cell+ +!
    r> 4 cells + >r  1 r@ +!     r> cell+ +! ;
: vis@ ( n -- x )
    2* 1+ cells vis-array $@ drop + 2@
    swap dup IF  /  ELSE  nip  THEN ;
: sv* ( x y z star -- v )
    dup element z df@ f* fswap
    dup element y df@ f* f+ fswap
    dup element x df@ f* f+
    xyz@ fsqsum fsqrt f/ fabs ;
[IFDEF] canvas
: draw-vis-array
  ^ canvas with  vismax dup $100 * steps 0 vismax $100 * home!
      rgb> drawcolor  path
      0 vismax 0 ?DO  I vis@ 2*
                      dup >r - negate
                      1 swap to r>  LOOP
      drop stroke
  endwith ( decimal vis-max ? ) ;
: visualize-mass  vis-mass !vis-array
  star# 0 ?DO  I star element msum df@
               1e f* vis'* I vis+  LOOP
  $00 $FF $00  draw-vis-array ;
: >dir ( fx fy fz -- sum )
    dirsens @ IF  sv*  ELSE drop fsqsum fsqrt THEN ;
: visualize-a#  vis-a !vis-array
    star# 0 ?DO
	I star a@ I star >dir
        .5e f* vis'* I vis+  LOOP ;
: visualize-a
    visualize-a#  $00 $00 $FF   draw-vis-array ;
: visualize-a+#  vis-a+ !vis-array
    star# 0 ?DO
	I star a+@ I star >dir
        .5e f* vis'* I vis+  LOOP ;
: visualize-a+
    visualize-a+#  $FF $00 $00  draw-vis-array ;
: visualize-v#  vis-a !vis-array
    star# 0 ?DO
	I star a@ I star >dir I star r#@ f* fsqrt
        .5e f* vis'* I vis+  LOOP ;
: visualize-v
    visualize-v#  $00 $FF $FF   draw-vis-array ;
: visualize-v+#  vis-a+ !vis-array
    star# 0 ?DO
	I star a+@ I star >dir I star r#@ f* fsqrt
        .5e f* vis'* I vis+  LOOP ;
: visualize-v+
    visualize-v+#  $FF $FF $00  draw-vis-array ;
: write-csv ( -- ) 6 set-precision decimal
    s" stars.csv" r/w output-file +buffer
    star# 0 ?DO
	I star r#@ fe. I star a@ I star >dir fe. I star a+@ I star >dir fe. cr
    LOOP  eot ;

\ test structure
[THEN]

Variable test-disc

: disc ( i -- )  elements test-disc @ cell+ + ; macro

$1000 Value disc#
$20 Value slice#
FVariable step# 0.003e step# f!

: init-disc ( -- )
    s" " test-disc $!
    disc# 1+ elements test-disc $!len  test-disc $@ erase
    disc# 0 ?DO
	I slice# bounds ?DO
	    I slice# mod pi slice# fm*/ fsincos
	    I slice# / 1+ dup * dup
	    s>f step# f@ f* f* fswap
	    s>f step# f@ f* f*
	    I disc dup element x df!
	           dup element y df!
	    0e         element z df!
	LOOP
    slice# +LOOP ;

: disc-msum ( -- )
  disc# 0 ?DO  I disc >xyz
      >x df@ >y df@ >z df@ vabs 1/f central# fm*
      0 star star# elements bounds ?DO
          I  dxyz@abs 1/f f+
          I -dxyz@abs 1/f f+
      sizeof element +LOOP
      star# 2* central# + fm/ I disc element msum df!
  pause LOOP ;

: disc-a ( -- )
  disc# 0 ?DO  I disc >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* vscale
      0 star star# elements bounds ?DO
          I  dxyz@ vdup-abs fdup fsqrt f* 1/f vscale v+
          I -dxyz@ vdup-abs fdup fsqrt f* 1/f vscale v+
      sizeof element +LOOP
      I disc 1e star# 2* central# + fm/ vscale
      dup element az df!
      dup element ay df!
          element ax df!
  pause LOOP ;

: disc-a+ ( -- )
  disc# 0 ?DO  I disc >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* vscale
      0 star star# elements bounds ?DO
          I dup  a+@  dxyz@abs 1/f vscale v+
          I dup -a+@ -dxyz@abs 1/f vscale v+
      sizeof element +LOOP
      I disc 1e star# 2* central# + fm/
      dup element msum df@ msum+ f@ f+ f/ vscale
      dup element az+ df!
      dup element ay+ df!
          element ax+ df!
  pause LOOP ;

[IFDEF] canvas

init-disc

Defer disc-text

: new/old-text ( addr -- ) ^ 3d-turtle with >r
    r@ a@  dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    r@ a+@ dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    fdup f0= IF  fnip  ELSE  f/ fln [ 64e fln 1/f ] Fliteral f*  THEN
    1e f+ 0.002e fmax .998e fmin 0e xy-text rdrop endwith ;
: old-text ( addr -- ) ^ 3d-turtle with >r
    r@ a@  dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    r@ dxyz@abs f* fln \ 1000e f*
    [ 64e fln 1/f fnegate ] Fliteral f*
    ( 0.002e fmax .998e fmin ) 0e xy-text rdrop endwith ;
: new-text ( addr -- ) ^ 3d-turtle with >r
    r@ a+@  dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    r@ dxyz@abs f* fln \ 1000e f*
    [ 64e fln 1/f fnegate ] Fliteral f*
    ( 0.002e fmax .998e fmin ) 0e xy-text rdrop endwith ;

' new/old-text IS disc-text

: draw-disc ( -- ) ^ 3d-turtle with
    0e >x df!
    0e >y df!
    0e >z df!
    slice# 2* open-path
    slice# 2* 0 ?DO  .998e 0e xy-text
	I pi slice# fm*/ fsincos
	.001e f* fswap .001e f* 0e add-xyz  LOOP  next-round
    slice# 2* 0 ?DO  .998e 0e xy-text
	I pi slice# fm*/ fsincos
	.001e f* fswap .001e f* 0e set-xyz  LOOP  next-round
    slice# 2* 0 ?DO  .998e 0e xy-text
	I pi slice# fm*/ fsincos
	.002e f* fswap .002e f* 0e set-xyz  LOOP
    disc# 0 ?DO
	next-round
	I slice# bounds ?DO
	    I disc disc-text I disc  xyz@ set-xyz
	LOOP
	I slice# bounds ?DO
	    I disc disc-text I disc -xyz@ set-xyz
	LOOP
    slice# +LOOP close-round close-path
    endwith ;

\ main galaxy drawing program

: draw-star  ( -- ) ^ 3d-turtle with
    star# 2* open-path next-round
    star# 0 ?DO
	I star a@ dirsens @ IF
	    I star sv*
	ELSE fsqsum fsqrt THEN
	I star a+@ dirsens @ IF
	    I star sv*
	ELSE fsqsum fsqrt THEN
	fdup f0= IF  fnip
	ELSE  f/ fln [ 64e fln 1/f ] Fliteral f*  THEN
	1e f+ 0.002e fmax .998e fmin fdup 0e xy-text
	I star xyz@ add-xyz 0e xy-text
	I star -xyz@ add-xyz
    LOOP next-round close-path
    endwith ;

Create .white 1e  sf, 1e  sf, 1e  sf, 1e  sf,
Create .black 0e  sf, 0e  sf, 0e  sf, 1e  sf,
Create .bg    1e  sf, 1e  sf, 1e  sf, 1e  sf,
Create .black2 0e  sf, 0e  sf, 0e  sf, 0e  sf,
Create .ambient 1 , 1 , 1 ,  1 ,
Create front_shininess  20.0e sf,

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;
: .emission ( addr -- )
  GL_FRONT GL_EMISSION rot glMaterialfv ;

: galaxy-draw  ( o alx aly alz pitch bend roll zoom disc -- )
  { alx aly alz alp alb alr zoom disc |
    glcanvas with
        0e 0e 0e 0e glClearColor
        5e 200e w @ h @ 3d-turtle new  3d-turtle with

            GL_LIGHT_MODEL_AMBIENT .ambient glLightModeli
            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc
            GL_FRONT_AND_BACK GL_SHININESS front_shininess
                            glMaterialfv

            0 5e 5e -10e get-xyz GL_POSITION 0 set-light

            20e forward zoom 1000 - 0.0025e fm* fexp scale

            pi 180e f/ pi x-left
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
            alr fm* roll-left
            
            .white .emission
            .black .color

            triangles
\            3.8e 3.8e 0e $20 cylinder

            textured-points smooth off
            draw-star
            .bg .emission
            .black2 .color
            disc case
		0 of  ['] noop         F IS disc-text  endof
		1 of  ['] new/old-text F IS disc-text  endof
		2 of  ['] old-text     F IS disc-text  endof
		3 of  ['] new-text     F IS disc-text  endof
	    endcase
	    disc IF  textured smooth on  draw-disc  THEN
	dispose endwith
    endwith } ;
[THEN]
