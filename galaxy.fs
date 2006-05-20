\ compute rotation behaviour of galaxies               21may00py

\needs float     import float
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs

float also glconst also opengl also

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

: init-stars ( -- )  s" " stars $!
  star# 1+ elements stars $!len  stars $@ erase ;

\ helper words

: pick-circle ( -- x y r )
    BEGIN
        frnd f2* !1 f- frnd f2* !1 f-
        fover f**2 fover f**2 f+ fdup !1 f> fdup f0= or  WHILE
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

FVariable oldgauss !0 oldgauss f!

: fgauss ( -- f )
    oldgauss f@ fdup f0> IF  !0 oldgauss f!  EXIT  THEN
    fdrop pick-circle
    fdup fln -2 fm* fswap f/ !.1 f*
    funder f* oldgauss f! f* ;

: set-bulge ( n d1 d2 -- ) { f: d1 f: d2 |
    0 ?DO
	BEGIN  frnd f2* !1 f-
	    frnd f2* !1 f-
	    frnd f2* !1 f-
	    fover2 f**2 fover2 f**2 f+ fover f**2 f+
	    !1 f> WHILE  fdrop fdrop fdrop  REPEAT
	fover2 f**2 fover2 f**2 f+ fover f**2 f+ fsqrt fsqrt
	I star
	fdup d1 f* fswap d2 f* { f: x1 f: x2 |
	x1 f* dup element x df!
	x1 f* dup element y df!
	f**2 x2 f*     element z df! }
    LOOP } ;

\ units: million meters, earth masses
\ rotations per day

: set-earth ( n -- )  6.37739715e 6.35607896e -13.4987667264019E-6
    { f: d1 f: d2 f: as |
    0 ?DO
	BEGIN  frnd f2* !1 f-
	    frnd f2* !1 f-
	    frnd f2* !1 f-
	    fover2 f**2 fover2 f**2 f+ fover f**2 f+
	    !1 f> WHILE  fdrop fdrop fdrop  REPEAT
	I star
	!0           dup element ax df!
	fover  as f* dup element ay df!
	fdup   as f* dup element az df!
	d1 f* dup element z df!
	d1 f* dup element y df!
	d2 f*     element x df!
    LOOP }
    331950e 149450e f**2 f/ msum+ f! ;

Variable spiral-dist spiral-dist on

: set-spiral ( n1 n2 di ds dz dp sf -- ) { f: di f: ds f: dz f: dp f: sf }
    swap ?DO  BEGIN  frnd f**2 ( t )
            frnd fover fover !1 f+ 1/f f> spiral-dist @ and WHILE
            fdrop fdrop  REPEAT ( ft fr )
        rnd >r
        funder !1 f+ f/
        r@ $1 and IF fnegate THEN
        fswap di f+ ds f*
        frnd !.5 f- !.6 f* fover f* f+ \ <<<
        frnd f**2 !-5 f* fexp dp f* !10 f* fover f/ 
        frnd !.5 f- f* fover f/
        fover ds f/ di f- f**2 pi f2/ f* sf f* f+ \ spiral factor
        fgauss ( was frnd ) dp f* f+
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

$000 Value central#
 
: set-msum ( -- ) ~~
  star# 0 ?DO  I star dup >xyz
      >x df@ >y df@ >z df@ vabs 1/f central# fm*
      0 star star# elements bounds ?DO  dup I <>
          IF  I  dxyz@abs 1/f f+  THEN
              I -dxyz@abs 1/f f+
      sizeof element +LOOP  drop
      star# 2* 1- central# + fm/ I star element msum df!
  pause LOOP ;

!0.5 msum+ f!

: set-a ( -- ) ~~
  star# 0 ?DO  I star dup >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* fnegate vscale
      0 star star# elements bounds ?DO  dup I <>
          IF   I dxyz@ vdup-abs
               fdup fsqrt f* 1/f vscale v+  THEN
          I -dxyz@ vdup-abs
          fdup fsqrt f* 1/f vscale v+
      sizeof element +LOOP drop
      I star !1 star# 2* 1- central# + fm/ vscale
      dup element az df!
      dup element ay df!
          element ax df!
  pause LOOP ;

Variable dirsens  dirsens on

: approx ( f addr -- )
    dup df@ fover f- f2/ f- df! ;

: set-a+ ( -- ) ~~
  star# 0 ?DO  I star dup >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* vscale
      0 star star# elements bounds ?DO  dup I <>
          IF  I dup  a+@  dxyz@abs 1/f vscale v+  THEN
              I dup -a+@ -dxyz@abs 1/f vscale v+
      sizeof element +LOOP drop
      I star !1 star# 2* 1- central# + fm/
      dup element msum df@ msum+ f@ f+ f/ vscale
      dup element az+ approx
      dup element ay+ approx
          element ax+ approx
  pause LOOP ;

: set-masses ( n dp sf -- )  >r >r init-stars
    star# swap &100 */ -4 and dup >r !1 !.33 set-bulge
    r> star# !.3 !2.25 !.2 r> !.01 fm* r> !.01 fm* set-spiral ;

&30 &40 &100 set-masses

Variable vis-mass
Variable vis-a+
Variable vis-a
0 Value vis-array
Variable vis-max
Variable a-pos $20 a-pos !

$40 Value vismax
: visminmax 0 max vismax 1- min ;
: vis* vismax 4/ fm* !.5 f+ ff>s visminmax ;
: vis'* vismax $40 * fm* !.5 f+ ff>s ;

: fsqsum ( x y z -- d ) f**2 fswap f**2 f+ fswap f**2 f+ ;
: !vis-array ( addr -- )  to vis-array
    s" " vis-array $!
    vismax 2+ 2* cells vis-array $!len  vis-array $@ erase ;
: vis+ ( val i -- )
    star xyz@ fsqsum fsqrt vis* 1+ 2* cells
    vis-array $@ drop + >r 2 r@ +! dup 2* r@ cell+ +!
    r> 2 cells - >r  1 r@ +! dup r@ cell+ +!
    r> 4 cells + >r  1 r@ +!     r> cell+ +! ;
: vis@ ( n -- x )
    2* 1+ cells vis-array $@ drop + 2@
    swap dup IF  /  ELSE  nip  THEN ;
: draw-vis-array
  ^ canvas with  vismax dup $100 * steps 0 vismax $100 * home!
      rgb> drawcolor  path
      0 vismax 0 ?DO  I vis@
                      dup >r - negate
                      1 swap to r>  LOOP
      drop stroke
  endwith ( decimal vis-max ? ) ;
: visualize-mass  vis-mass !vis-array
  star# 0 ?DO  I star element msum df@
               !1 f* vis'* I vis+  LOOP
  $00 $FF $00  draw-vis-array ;
: sv* ( x y z star -- v )
    dup element z df@ f* fswap
    dup element y df@ f* f+ fswap
    dup element x df@ f* f+
    xyz@ fsqsum fsqrt f/ fabs ;
: visualize-a#  vis-a !vis-array
    star# 0 ?DO
        I star a@ dirsens @ IF
            I star sv*
        ELSE fsqsum fsqrt THEN
        !.5 f* vis'* I vis+  LOOP ;
: visualize-a
    visualize-a#  $00 $00 $FF   draw-vis-array ;
: visualize-a+#  vis-a+ !vis-array
    star# 0 ?DO
	I star a+@ dirsens @ IF
            I star sv*
        ELSE fsqsum fsqrt THEN
        !.5 f* vis'* I vis+  LOOP ;
: visualize-a+
    visualize-a+#  $FF $00 $00  draw-vis-array ;

\ test structure

Variable test-disc

: disc ( i -- )  elements test-disc @ cell+ + ; macro

$1000 Value disc#
$20 Value slice#
FVariable step# !0.003 step# f!

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
	    !0         element z df!
	LOOP
    slice# +LOOP ;

init-disc

: disc-msum ( -- ) ~~
  disc# 0 ?DO  I disc >xyz
      >x df@ >y df@ >z df@ vabs 1/f central# fm*
      0 star star# elements bounds ?DO
          I  dxyz@abs 1/f f+
          I -dxyz@abs 1/f f+
      sizeof element +LOOP
      star# 2* central# + fm/ I disc element msum df!
  pause LOOP ;

: disc-a ( -- ) ~~
  disc# 0 ?DO  I disc >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* vscale
      0 star star# elements bounds ?DO
          I  dxyz@ vdup-abs fdup fsqrt f* 1/f vscale v+
          I -dxyz@ vdup-abs fdup fsqrt f* 1/f vscale v+
      sizeof element +LOOP
      I disc !1 star# 2* central# + fm/ vscale
      dup element az df!
      dup element ay df!
          element ax df!
  pause LOOP ;

: disc-a+ ( -- ) ~~
  disc# 0 ?DO  I disc >xyz
      >x df@ >y df@ >z df@ vdup-abs fdup fsqrt f* 1/f
      central# fm* vscale
      0 star star# elements bounds ?DO
          I dup  a+@  dxyz@abs 1/f vscale v+
          I dup -a+@ -dxyz@abs 1/f vscale v+
      sizeof element +LOOP
      I disc !1 star# 2* central# + fm/
      dup element msum df@ msum+ f@ f+ f/ vscale
      dup element az+ df!
      dup element ay+ df!
          element ax+ df!
  pause LOOP ;

Defer disc-text

: new/old-text ( addr -- ) ^ 3d-turtle with >r
    r@ a@  dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    r@ a+@ dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    fdup f0= IF  fnip  ELSE  f/ fln [ !64 fln 1/f ] Fliteral f*  THEN
    !1 f+ !0.002 fmax !.998 fmin !0 xy-text rdrop endwith ;
: old-text ( addr -- ) ^ 3d-turtle with >r
    r@ a@  dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    r@ dxyz@abs f* fln \ !1000 f*
    [ !64 fln 1/f fnegate ] Fliteral f*
    ( !0.002 fmax !.998 fmin ) !0 xy-text rdrop endwith ;
: new-text ( addr -- ) ^ 3d-turtle with >r
    r@ a+@  dirsens @ IF  r@ sv*  ELSE fsqsum fsqrt THEN
    r@ dxyz@abs f* fln \ !1000 f*
    [ !64 fln 1/f fnegate ] Fliteral f*
    ( !0.002 fmax !.998 fmin ) !0 xy-text rdrop endwith ;

' new/old-text IS disc-text

: draw-disc ( -- ) ^ 3d-turtle with
    !0 >x df!
    !0 >y df!
    !0 >z df!
    slice# 2* open-path
    slice# 2* 0 ?DO  !.998 !0 xy-text
	I pi slice# fm*/ fsincos
	!.001 f* fswap !.001 f* !0 add-xyz  LOOP  next-round
    slice# 2* 0 ?DO  !.998 !0 xy-text
	I pi slice# fm*/ fsincos
	!.001 f* fswap !.001 f* !0 set-xyz  LOOP  next-round
    slice# 2* 0 ?DO  !.998 !0 xy-text
	I pi slice# fm*/ fsincos
	!.002 f* fswap !.002 f* !0 set-xyz  LOOP
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
	ELSE  f/ fln [ !64 fln 1/f ] Fliteral f*  THEN
	!1 f+ !0.002 fmax !.998 fmin fdup !0 xy-text
	I star xyz@ add-xyz !0 xy-text
	I star -xyz@ add-xyz
    LOOP next-round close-path
    endwith ;

Create .white !1  f>fs , !1  f>fs , !1  f>fs  , !1  f>fs ,
Create .black !0  f>fs , !0  f>fs , !0  f>fs  , !1  f>fs ,
Create .bg    !1  f>fs , !1  f>fs , !1  f>fs , !1  f>fs ,
Create .black2 !0  f>fs , !0  f>fs , !0  f>fs  , !0  f>fs ,
Create .ambient 1 , 1 , 1 ,  1 ,
Create front_shininess  !&20.0 f>fs ,

: .color ( addr -- )
  GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;
: .emission ( addr -- )
  GL_FRONT GL_EMISSION rot glMaterialfv ;

: galaxy-draw  ( o alx aly alz pitch bend roll zoom disc -- )
  { alx aly alz alp alb alr zoom disc |
    glcanvas with
        !0 !0 !0 !0 glClearColor
        !5 !200 w @ h @ 3d-turtle new  3d-turtle with

            GL_LIGHT_MODEL_AMBIENT .ambient glLightModeli
            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc
            GL_FRONT_AND_BACK GL_SHININESS front_shininess
                            glMaterialfv

            0 !5 !5 !-10 get-xyz GL_POSITION 0 set-light

            !20 forward zoom 1000 - !&0.0025 fm* fexp scale

            pi !180 f/ pi x-left
            fdup alx fm* x-left
            fdup aly fm* y-left
            fdup alz fm* z-left
            fdup alp fm* left
            fdup alb fm* up
            alr fm* roll-left
            
            .white .emission
            .black .color

            triangles
\            !3.8 !3.8 !0 $20 cylinder

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


