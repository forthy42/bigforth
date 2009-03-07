\ 3D turtle graphics                                   27dec98py

[defined] VFXForth [IF]
    mpe-float
    include /usr/share/doc/VfxForth/Lib/Ndp387.fth
    ans-float
    : float ;
[ELSE]
    \needs float import float
[THEN]
\needs glconst | import glconst
[defined] x11 [IF]
\needs xconst  | import xconst
[THEN]
also memory also dos also float also glconst
[defined] x11 [IF]  also x11 [THEN]
[defined] win32 [IF]  also win32 [THEN]
also opengl also

[defined] win32 [IF]
:noname  ['] noop noop-act 1 1 1 1  glcanvas new  glcanvas with
         screen self dpy!  render  dispose  endwith  drop ;
IS dummy-canvas
[ELSE]
\    | : glarrays ; ." With gl arrays" cr
\    | : debug-points ; ." With debug points" cr
[THEN]

\ r,phi extraction                                     31dec98py
[defined] ftuck 0= [IF]  : ftuck fswap fover ;  [THEN]
[defined] f-rot 0= [IF]  : f-rot frot frot ;  [THEN]
[defined] fover2 0= [IF]  : fover2 2 fpick ;  [THEN]
[defined] f>fs  0= [IF]  Variable 'f>fs : f>fs 'f>fs sf! 'f>fs @ ;  [THEN]
[defined] fs>f  0= [IF]  : fs>f 'f>fs ! 'f>fs sf@ ;  [THEN]
[defined] %pi [IF] %pi FConstant pi [THEN]
[defined] f**2 0= [IF]  : f**2 fdup f* ;  [THEN]
[defined] fm/ 0= [IF]  : fm/ s>f f/ ;  [THEN]
[defined] fm* 0= [IF]  : fm* s>f f* ;  [THEN]
[defined] fm*/ 0= [IF]  : fm*/ s>f f/ s>f f* ;  [THEN]
[defined] f>r 0= [IF]
    Code f>r
	pop edx
	add esp, #-12
	fstp fword 0 [esp]
	push edx
	fnext,
    end-code
    Code fr>
	pop edx
	fld fword 0 [esp]
	add esp, #12
	push edx
	fnext,
    end-code
    [THEN]

[defined] r,phi>xy 0= [IF]
    : r,phi>xy ( r phi -- x y )
      fsincos frot ftuck f* f-rot f* ;
[THEN]

[defined] 9* 0= [IF]
    [defined] VFXForth [IF]
	: 9* 9 * ;
    [ELSE]
	Code 9*  AX AX *8 I) AX lea  Next end-code macro
    [THEN]
[THEN]

[defined] 3* 0= [IF]
    : 3* dup 2* + ; macro
[THEN]

\ doesn't work for -1
: >2** ( a -- n )  1  BEGIN  2dup u>  WHILE  2*  REPEAT  nip ;

[defined] VFXForth true or [IF]
    : inner-get ( addr -- sf ) 3 swap dup sf@ sfloat+
          [ 3 sfloats ] Literal bounds
          DO   dup fpick I sf@ f* f+ 1-
               [ 1 sfloats ] Literal +LOOP  drop f>fs ;
    : 2linear ( addr -- sf ) dup sf@ f* sfloat+ sf@ f+ f>fs ;
    : !point ( p z y x addr -- )
        !+ !+ !+ !+
        [ 2 cells ] Literal + [ 3 cells ] Literal erase ;
    : !normal ( z y x addr -- )
        [ 6 cells ] Literal + !+ !+ ! ;
    : .x        sf@ ; macro
    : .y  cell+ sf@ ; macro
    : .z  8+    sf@ ; macro
    : .nx dup $14 + sf@ ; macro
    : .ny dup $18 + sf@ ; macro
    : .nz dup $1C + sf@ ; macro
    : .nxsf! dup $14 + sf! ; macro
    : .nysf! dup $18 + sf! ; macro
    : .nzsf! dup $1C + sf! ; macro
    : left-over ( vl v vr -- x1 y1 z1 x2 y2 z2 )
      { vl v vr }
      vl .x v .x f-  vl .y v .y f-  vl .z v .z f-
      vr .x v .x f-  vr .y v .y f-  vr .z v .z f- ;
    : cross* ( x1 y1 z1 x2 y2 z2 -- x3 y3 z3 )
      { f: x1 f: y1 f: z1 f: x2 f: y2 f: z2 }
        y1 z2 f* z1 y2 f* f- ( x )
        z1 x2 f* x1 z2 f* f- ( y )
        x1 y2 f* y1 x2 f* f- ( z ) ;
    : get-normal ( vl v vr -- fx fy fz )
      left-over cross* ;
    : negate3 ( fx fy fz -- -fx -fy -fz )
      fnegate frot  fnegate frot  fnegate frot ;
[ELSE]
    Code inner-get ( fx fy fz addr -- sf )
         .fs AX ) fld
         .fs 1 sfloats AX D) fld   4 ST  fmul   1 STP fadd
         .fs 2 sfloats AX D) fld   3 ST  fmul   1 STP fadd
         .fs 3 sfloats AX D) fld   2 ST  fmul   1 STP fadd
         .fs -4 SP D) fstp  -4 SP D) AX mov
         Next end-code  macro
    Code 2linear ( f addr -- sf )
         .fs AX ) fmul  .fs 1 sfloats AX D) fadd
         .fs -4 SP D) fstp  -4 SP D) AX mov
         Next end-code  macro
    Code !point ( p z y x addr -- )  DX DX xor
         AX ) pop  1 cells AX D) pop  2 cells AX D) pop
         3 cells AX D) pop
         DX 6 cells AX D) mov
         DX 7 cells AX D) mov
         DX 8 cells AX D) mov
         AX pop  Next end-code macro
    Code !normal ( fx fy fz addr -- )
         6 cells AX D) pop
         7 cells AX D) pop
         8 cells AX D) pop
         AX pop  Next end-code macro
    Code .nx ( addr -- addr f )  .fs  5 sfloats AX D) fld   
         Next end-code macro  0 0 T&P
    Code .ny ( addr -- addr f )  .fs  6 sfloats AX D) fld   
         Next end-code macro  0 0 T&P
    Code .nz ( addr -- addr f )  .fs  7 sfloats AX D) fld   
         Next end-code macro  0 0 T&P
    Code .nxsf!  ( f addr -- addr )  .fs  5 sfloats AX D) fstp
         Next end-code macro  0 0 T&P
    Code .nysf!  ( f addr -- addr )  .fs  6 sfloats AX D) fstp
         Next end-code macro  0 0 T&P
    Code .nzsf!  ( f addr -- addr )  .fs  7 sfloats AX D) fstp
         Next end-code macro  0 0 T&P
    Code left-over ( vl v vr -- )  CX pop  DX pop
         .fs           DX  ) fld  .fs           CX  ) fsubr
         .fs 1 sfloats DX D) fld  .fs 1 sfloats CX D) fsubr
         .fs 2 sfloats DX D) fld  .fs 2 sfloats CX D) fsubr
         .fs           AX  ) fld  .fs           CX  ) fsubr
         .fs 1 sfloats AX D) fld  .fs 1 sfloats CX D) fsubr
         .fs 2 sfloats AX D) fld  .fs 2 sfloats CX D) fsubr
         AX pop  Next end-code
    Code cross* ( x1 y1 z1 x2 y2 z2 -- )
         4 ST fld  1 ST fmul  4 ST fld  3 ST fmul  1 STP fsubr
         .fs -1 sfloats SP D) fstp
         3 ST fld  3 ST fmul  6 ST fld  2 ST fmul  1 STP fsubr
         .fs -2 sfloats SP D) fstp
         5 ST fld  2 ST fmul  5 ST fld  4 ST fmul  1 STP fsubr
         .fs -3 sfloats SP D) fstp
         0 ST fstp  0 ST fstp  0 ST fstp
         0 ST fstp  0 ST fstp  0 ST fstp
         .fs -1 sfloats SP D) fld
         .fs -2 sfloats SP D) fld
         .fs -3 sfloats SP D) fld  Next end-code
    : get-normal ( vl v vr -- fx fy fz )
      left-over cross* ;
    Code negate3 ( fx fy fz -- -fx -fy -fz )
         fchs  1 ST fxch  fchs  2 ST fxch  fchs
         2 ST fxch  1 ST fxch  Next end-code macro
[THEN]

[defined] libGLU [IF]
    : >c ( xt -- )  dup 2- w@ + &11 - cfa@ ;
    : >c' ( xt -- offset addr )  dup 2- w@ + &10 - dup 4+ ;
    \ define a few C-callbacks

    Code glVertexTexCoord3fv ( c:addr -- )  R:  AX push  AX push
	$C SP D) AX mov  $C # AX add  AX push
	' glTexCoord2fv >c' A# AX mov  A#) AX add  AX call  AX pop
	$C SP D) AX mov  AX push
	' glVertex3fv >c' A# AX mov  A#) AX add  AX call  AX pop
	AX pop  AX pop  ret  end-code

    Code glVertexNormalTexCoord3fv ( c:addr -- )  R:  AX push  AX push
	$C SP D) AX mov  $C # AX add  AX push
	' glTexCoord2fv >c' A# AX mov  A#) AX add  AX call  AX pop
	$C SP D) AX mov  $14 # AX add  AX push
	' glNormal3fv >c' A# AX mov  A#) AX add  AX call  AX pop
	$C SP D) AX mov  AX push
	' glVertex3fv >c' A# AX mov  A#) AX add  AX call  AX pop
	AX pop  AX pop  ret  end-code
[THEN]

1e f>fs Constant #1.
pi f2* FConstant 2pi
2pi 1/f FConstant 1/2pi

\ : .matrix ( addr -- )
\   &12 sfloats bounds
\   DO  cr  I 4 sfloats bounds
\       DO  I sf@ 2e f+ 2e f- f.  1 sfloats +LOOP
\       4 sfloats +LOOP ;

Create .white #1. , #1. , #1. , #1. ,

[defined] debug-points [IF]
Variable maxpoints
Variable #points

$7FFFFFFF maxpoints !

: ?maxpoints ( addr -- )
  #points @ maxpoints @ >  IF  drop rdrop  THEN ;
: points+  1 #points +! ;
[ELSE]
    : ?maxpoints ; immediate
    : points+ ; immediate
[THEN]

\ class declaration                                    03jan99py

true Value do-mipmap

debugging class 3d-turtle
public:
    0 sfloats  var trans
    1 sfloats  var trans-0,0
    1 sfloats  var trans-1,0
    1 sfloats  var trans-2,0
    1 sfloats  var trans-3,0
    1 sfloats  var trans-0,1
    1 sfloats  var trans-1,1
    1 sfloats  var trans-2,1
    1 sfloats  var trans-3,1
    1 sfloats  var trans-0,2
    1 sfloats  var trans-1,2
    1 sfloats  var trans-2,2
    1 sfloats  var trans-3,2

    1 sfloats  var z-off
    1 sfloats  var x-text
    1 sfloats  var x-toff
    1 sfloats  var y-text
    1 sfloats  var y-toff
    1 sfloats  var phi
    1 sfloats  var dphi
    1 sfloats  var rot-mode
    cell       var flip
    cell       var point#
[defined] glarrays [IF]
    cell       var path
    cell       var #path
    cell       var #path'
    cell       var #path''
[ELSE]
    cell       var path
    cell       var path'
    cell       var path''
[THEN]
    cell       var matrix-stack
    cell       var smooth
    cell       var smooth'
    cell       var path-points
    cell       var gl-mode
    0          var 'draw-path
    defer      draw-path
    0          var 'do-texture
    defer      do-texture

    0          var last-turtle

    early scale
    early scale-xyz
    
    early left
    early right
    early up
    early down
    early roll-left
    early roll-right

    early x-left
    early x-right
    early y-left
    early y-right
    early z-left
    early z-right

    early forward-xyz
    early forward

    early degrees
    early set-dphi

    early get-xyz
    early get-xy
    early get-rpz
    early get-rp
    early get-rz
    early get-r

    early open-path
    early start-path
    early close-path
    early end-path
    early next-round

\ obsolete:
    early open-round
    early close-round
    early finish-round

    early set-xyz
    early set-xy
    early set-rpz
    early set-rp
    early set-r
    early set-rz
    early set

    early set-light
    early set-fog

    early add-xyz
    early add-xy
    early add-rpz
    early add-rp
    early add-r
    early add-rz
    early add

    early xy-text

    early drop-point

    early init-matrix

\ stacking, matrix transformation                      28dec99py

    early matrix>
    early >matrix
    early matrix@
    early clone
    early >turtle immediate
    early turtle> immediate

    early matrix*
    early 1matrix

    early pos@
    early scale@
    early ortho

\ drawing                                              30jan99py

    early textured
    early triangles
    early textured-poly
    early poly
    early lines
    early points
    early textured-points
    early textured-lines

\ auto-texturing                                       30jan99py

    early create-mipmap1
    early create-mipmap3
    early create-mipmap4

    early xy-texture
    early zphi-texture
    early zphi2-texture
    early rphi-texture
    early zp-texture

    early textures
    early del-textures
    early set-texture
    early load-texture
    early text-texture

    early flip-clock

\ high level primitives                                27dec99py

    early segment
    early cylinder
    early sphere
    
\ debugging                                            30jan99py
\    early .trans
\    early set-normal

\ class implementation                                 03jan99py
class;

debugging class 3d-text
    cell var w
    cell var h
    cell var wt
    cell var ht
    cell var texture
    method draw
how:
  : init ( w h wt ht texture -- )
      texture ! ht ! wt ! h ! w ! ;
class;

3d-turtle implements
    : init-matrix ( -- )
      trans     #12 sfloats erase
      1e  trans-2,0 ( #02 sfloats + ) sf!
      1e  trans-1,1 ( #05 sfloats + ) sf!
      -1e trans-3,2 ( #11 sfloats + ) sf! ;
    : init-OpenGL ( -- )
      GL_CW glFrontFace
      GL_LESS glDepthFunc  depth >r
      GL_CULL_FACE  GL_LIGHTING  GL_DEPTH_TEST GL_NORMALIZE
      depth r> - 0 ?DO  glEnable  LOOP
      GL_TEXTURE_2D GL_TEXTURE_MAG_FILTER GL_NEAREST
          glTexParameteri
      GL_TEXTURE_2D GL_TEXTURE_MIN_FILTER GL_NEAREST
          glTexParameteri
      GL_TEXTURE_ENV GL_TEXTURE_ENV_MODE GL_MODULATE glTexEnvi

      GL_FOG_HINT                   GL_FASTEST glHint
      GL_PERSPECTIVE_CORRECTION_HIN GL_FASTEST glHint
      GL_POLYGON_SMOOTH_HINT        GL_FASTEST glHint

      GL_FRONT GL_FILL glPolygonMode
      GL_LINE_SMOOTH glEnable
      
      GL_FOG_DENSITY  0e     glFogf
      GL_FOG_COLOR    .white glFogfv
      GL_FOG_MODE     GL_EXP2 glFogi ;
    : init-device ( fnear ffar w h -- ) { f: near f: far w h }
      0 0 w h glViewport
      GL_PROJECTION glMatrixMode glLoadIdentity

      GL_FOG_START near  glFogf
      GL_FOG_END   far   glFogf
      
      w h >  IF
         w s>f h fm/ fdup fnegate fswap -1e 1e
      ELSE
         -1e 1e h s>f w fm/ fdup fnegate fswap
      THEN  far near glFrustum
      
      GL_COLOR_BUFFER_BIT GL_DEPTH_BUFFER_BIT or glClear
      GL_MODELVIEW glMatrixMode glLoadIdentity ;

    : flip-clock ( -- )  flip @ 0= flip !
      flip @ IF GL_CCW ELSE GL_CW THEN  glFrontFace
      glFlush ;

\ matrix operations                                    10jan99py

    | $10 Constant maxstack
    | #20 sfloats Constant /matrix

    : matrix? ( -- ) matrix-stack @ 0=
      IF    [ /matrix maxstack * cell+ ] Literal
            matrix-stack Handle!  matrix-stack @ off
      ELSE  matrix-stack dup @ @
            maxstack + 1+ $-10 and /matrix * cell+
            SetHandleSize
      THEN ;
    : matrix-sp ( -- addr )
      matrix? matrix-stack @ @+ swap /matrix * + ;
    : >matrix ( -- )
      trans matrix-sp /matrix move
      1 matrix-stack @ +! ;
    : matrix> ( -- )
      -1 matrix-stack @ +!
      matrix-sp trans /matrix move ;
    : matrix@ ( -- )
      matrix-sp /matrix - trans /matrix move ;

\ scale operations                                     10jan99py

    : scale-xyz ( fx fy fz -- )
      trans [ #12 sfloats ] Literal bounds
      DO  2 I sfloat+ [ 3 sfloats ] Literal bounds
          DO   dup fpick I sf@ f* I sf! 1-
               [ 1 sfloats ] Literal +LOOP  drop
          [ 4 sfloats ] Literal +LOOP  fdrop fdrop fdrop ;

    : scale ( f -- )  fdup fdup scale-xyz ;
    
\ rotation primitives                                  10jan99py

    : do-rotate ( fs fc v1 v2 -- )
      [ 3 sfloats ] Literal bounds
      DO  fover I sf@ f* fover dup sf@ f* f+ f-rot
          fover dup sf@ f* fover I sf@ f*
          fswap f- I sf! frot dup sf!
          sfloat+ [ 1 sfloats ] Literal +LOOP
      drop fdrop fdrop ;

    : do-turn ( fs fc v1 v2 -- )
      [ #12 sfloats ] Literal bounds
      DO  fover I sf@ f* fover dup sf@ f* f+ f-rot
          fover dup sf@ f* fover I sf@ f*
          fswap f- I sf! frot dup sf!
          [ 4 sfloats ] Literal +
          [ 4 sfloats ] Literal +LOOP
      drop fdrop fdrop ;

\ turn operations                                      31dec98py

    : phi>xy ( f -- f1 f2 )
      rot-mode sf@ f* fsincos 1e f- 1e f+ ;
    : degrees ( f -- )  1/2pi f* rot-mode ! ;

    : left ( f -- )       phi>xy  trans-2,0 trans-3,0 do-turn ;
    : down ( f -- )       phi>xy  trans-1,0 trans-3,0 do-turn ;
    : roll-left ( f -- )  phi>xy  trans-1,0 trans-2,0 do-turn ;

    : right ( f -- )      fnegate left ;
    : up ( f -- )         fnegate down ;
    : roll-right ( f -- ) fnegate roll-left ;

\ rotate operations                                    10jan99py

    : x-left ( f -- )   phi>xy  trans-1,1 trans-1,2 do-rotate ;
    : y-left ( f -- )   phi>xy  trans-1,0 trans-1,2 do-rotate ;
    : z-left ( f -- )   phi>xy  trans-1,0 trans-1,1 do-rotate ;

    : x-right ( f -- )  fnegate x-left ;
    : y-right ( f -- )  fnegate y-right ;
    : z-right ( f -- )  fnegate z-left ;

\ simple operations                                    27dec98py

    : forward-xyz ( fx fy fz -- )
      fdup z-off sf@ f+ z-off sf!
      trans [ #12 sfloats ] Literal bounds
      DO  3 I sf@ I sfloat+ [ 3 sfloats ] Literal bounds
          DO   dup fpick I sf@ f* f+ 1-
               [ 1 sfloats ] Literal +LOOP  drop I sf!
          [ 4 sfloats ] Literal +LOOP  fdrop fdrop fdrop ;

    : forward ( fz -- )  0e 0e frot forward-xyz ;

\ complex operation                                    16feb99py

    : matrix* ( -- )   -1 matrix-stack @ +!
      trans-1,0 [ 3 sfloats ] Literal bounds
      DO  matrix-sp [ #12 sfloats ] Literal bounds
          DO  J 0e I sfloat+ [ 3 sfloats ] Literal bounds
              DO   dup sf@ I sf@ f* f+ [ 4 sfloats ] Literal +
                   [ 1 sfloats ] Literal +LOOP  drop
              [ 4 sfloats ] Literal +LOOP
          fswap frot I [ #12 sfloats ] Literal bounds
          DO  I sf!  [ 4 sfloats ] Literal +LOOP
          [ 1 sfloats ] Literal +LOOP
      trans  matrix-sp [ #12 sfloats ] Literal bounds
      DO  I sf@ dup sf! [ 4 sfloats ] Literal +
          [ 4 sfloats ] Literal +LOOP  drop ;

    : 1matrix ( -- )  >matrix init-matrix ;

\ point extraction                                     31dec98py

    : pos@ ( -- fx fy fz )
      trans-0,1 sf@
      trans-0,0 sf@
      trans-0,2 sf@ fnegate ;
    : sqsum ( addr n -- )
      0e 4* sfloats bounds
      ?DO  I sf@ fdup f* f+ [ 4 sfloats ] Literal +LOOP ;
    : scale@ ( -- fsx2 fsy2 fsz2 )
      trans-1,0 3 sqsum
      trans-2,0 3 sqsum
      trans-3,0 3 sqsum ;

    : get-xyz ( fx fy fz -- z' y' x' )
      do-texture
      trans-0,2 inner-get
      trans-0,1 inner-get
      trans-0,0 inner-get
      fdrop fdrop fdrop ;

\ orthogonalize matrix                                 28dec99py

    : ortho ( -- ) \ x x z -> y  y x z -> x
      scale@ { f: x f: y f: z }
      trans-3,0 sf@ trans-3,1 sf@ trans-3,2 sf@
      trans-1,0 sf@ trans-1,1 sf@ trans-1,2 sf@  cross*
      trans-2,2 sf! trans-2,1 sf! trans-2,0 sf!
      trans-2,0 sf@ trans-2,1 sf@ trans-2,2 sf@
      trans-3,0 sf@ trans-3,1 sf@ trans-3,2 sf@  cross*
      trans-1,2 sf! trans-1,1 sf! trans-1,0 sf!
      x y z f* f/ y x z f* f/ fsqrt 1e scale-xyz ;

\ points relative to current turtle position           03jan99py

    : set-dphi ( fphi -- )  rot-mode sf@ f* dphi sf! ;

    : get-xy ( fx fy -- z' y' x' )  0e get-xyz ;
    : get-rpz ( fr fphi fz -- z' y' x' )
      f-rot rot-mode sf@ f* fdup phi sf! r,phi>xy frot get-xyz ;
    : get-rp ( fr fphi -- z' y' x' ) 0e get-rpz ;
    : get-rz ( fr fz -- z' y' x' )
      fswap phi sf@ r,phi>xy frot get-xyz
      dphi sf@ phi sf@ f+ phi sf! ;
    : get-r ( fr -- z' y' x' ) 0e get-rz ;

\ path address                                         03jan99py

\ path layout:
\ oldpoint x y z  tx ty  nx ny nz

[defined] glarrays [IF]
    : path+   ( offset -- addr )  9* 1+ cells path @ + ; macro
    : cur-point   ( n -- addr )  #path  @ + path+ ; macro
    : prev-point  ( n -- addr )  #path' @ + path+ ; macro
    | 9 cells Constant /point
[ELSE]
    : cur-point   ( n -- addr )  9* 1+ cells path   @ + ; macro
    : prev-point  ( n -- addr )  9* 1+ cells path'  @ + ; macro
    : path+  cur-point ;
    | 9 cells Constant /point
[THEN]

\ ligth                                                10jan99py

    : set-light ( par1..4 par n -- ) GL_LIGHT0 +
        dup glEnable >r >r
        sp@ r> r> swap rot glLightfv 2drop 2drop ;
    : set-fog ( fdensity -- )
        fdup f0= IF  fdrop  GL_FOG  glDisable  EXIT  THEN
        GL_FOG_DENSITY glFogf  GL_FOG glEnable ;

\ point setting                                        03jan99py

    : path#  path @ ; macro
    : do-point ( z' y' x' -- )
      point# @  path# @ path+ !point 1 path# +! ;

\ point setting                                        03jan99py

    : drop-point ( -- )  1 point# +! ;

    : add-xyz ( fx fy fz --  )   get-xyz do-point ;
    : add-xy ( fx fy --  )       get-xy  do-point ;
    : add-rpz ( fr fphi fz -- )  get-rpz do-point ;
    : add-rp ( fr fphi -- )      get-rp  do-point ;
    : add-rz ( fr fz -- )        get-rz  do-point ;
    : add-r ( fr -- )            get-r   do-point ;
    : add ( -- )  0e add-r ;

    : set-xyz ( fx fy fz --  )   add-xyz drop-point ;
    : set-xy ( fx fy --  )       add-xy  drop-point ;
    : set-rpz ( fr fphi fz -- )  add-rpz drop-point ;
    : set-rp ( fr fphi -- )      add-rp  drop-point ;
    : set-rz ( fr fz -- )        add-rz  drop-point ;
    : set-r ( fr -- )            add-r   drop-point ;
    : set ( -- )  0e set-r ;

\ path handling                                        03jan99py

    : open-round ( -- )
[defined] glarrays [IF]
      #path' @ #path'' !  #path @ #path' !
      2 path# +! path# @ #path !
[ELSE]
      path'' @ IF  path'' HandleOff  THEN
      path' @  IF  path' @ path'' SetHandle path' off  THEN
      path @   IF  path  @ path'  SetHandle path off  THEN
      path-points @  4+ 9* 1+ cells  dup path Handle!
      path @ swap erase 1 path @ !
[THEN]
      point# off ;
    : open-path ( n -- )
        smooth @ dup smooth' @ <>
        IF  dup IF  GL_SMOOTH  ELSE  GL_FLAT  THEN
            glShadeModel
        THEN  smooth' !  \ ugly workaround
        path-points ! open-round ;
    : fs- ( fs1 fs2 -- fs3 )
        fs>f fs>f f- f>fs ; macro
    : look-at ( -- z' y' x' )
      trans-3,2 sf@ f>fs
      trans-3,1 sf@ f>fs
      trans-3,0 sf@ f>fs ;
    : look-back ( -- z' y' x' )
      trans-3,2 sf@ fnegate f>fs
      trans-3,1 sf@ fnegate f>fs
      trans-3,0 sf@ fnegate f>fs ;
    : start-path ( n -- )
        look-back { z y x }
        dup open-path  0 ?DO  add  LOOP
        path-points @ 2+ 1 ?DO
            z y x I path+ !normal
        LOOP ;

\ auto-texturing                                       30jan99py

    : !text   ( x y -- )  swap path# @ path+ 4 cells + 2! ;
    : x-text@ ( f -- tx )  x-text 2linear ;
    : y-text@ ( f -- tx )  y-text 2linear ;
    : xy-text ( fx fy -- )  y-text@ x-text@ swap !text ;
    : do-xy-text ( fx fy fz -- fx fy fz )
      fover2 x-text@ fover y-text@ !text ;
    : do-zphi-text ( fx fy fz -- fx fy fz )
      fdup z-off sf@ f+ x-text@
      fover2 fover2 fswap fatan2 y-text@ !text ;
    : do-zphi2-text ( fx fy fz -- fx fy fz )
      fdup z-off sf@ f+ x-text@ phi sf@ y-text@ !text ;
    : do-zp-text ( fx fy fz -- fx fy fz )
      fdup z-off sf@ f+ x-text@  point# @ s>f y-text@ !text ;
    : do-rphi-text ( fx fy fz -- fx fy fz )
      fover2 f**2 fover2 f**2 f+ fsqrt x-text@
      fover2 fover2 fswap fatan2 y-text@ !text ;

    : >texture  ( addr f -- )
      IS do-texture y-text sf! 0e z-off sf! ;
    : xy-texture    ['] do-xy-text    1e    >texture ;
    : zphi-texture  ['] do-zphi-text  1/2pi >texture ;
    : zphi2-texture ['] do-zphi2-text 1/2pi >texture ;
    : zp-texture    ['] do-zp-text    1e    >texture ;
    : rphi-texture  ['] do-rphi-text  1/2pi >texture ;
    : no-texture    ['] noop          1e    >texture ;

\ texture loading (ppm)                                31jan99py

    : textures ( n -- t1 .. tn )
?texture [IF]
        dup >r 0 ?DO  0  LOOP  sp@ r> swap glGenTextures
[ELSE]  0 ?DO  0  LOOP  [THEN] ;
    : del-textures ( t1 .. tn n -- )
?texture [IF]
        >r sp@ r@ swap glDeleteTextures r> [THEN]
      0 ?DO  drop  LOOP ;
    : set-texture ( n -- )
?texture [IF]
      GL_TEXTURE_2D swap glBindTexture ;
[ELSE]  drop ;  [THEN]
    : create-mipmap1 ( addr w h -- addr )
      0 { w h n }
      BEGIN  dup >r
             GL_TEXTURE_2D n GL_ALPHA8 w h
             0 GL_ALPHA GL_UNSIGNED_BYTE
             r> glTexImage2D
             w 1 > h 1 > and do-mipmap and  IF
                 dup dup h w * bounds ?DO
                     I w bounds ?DO
                         I c@ I 1+ c@ + I w + c@ + I w + 1+ c@ + 4/
                         swap c!+
                     2 +LOOP
                     w 2* +LOOP  drop
                 w 2/ to w h 2/ to  n 1+ to n  false
             ELSE  true  THEN  UNTIL ;
    : create-mipmap3 ( addr w h -- addr )
      over 3* 0 { w h w3 n }
      BEGIN  dup >r
             GL_TEXTURE_2D n GL_RGB8 w h
             0 GL_BGR GL_UNSIGNED_BYTE
             r> glTexImage2D
             w 1 > h 1 > and do-mipmap and  IF
                 dup dup h w3 * bounds ?DO
                     I w3 bounds ?DO
                         I c@ I 3+ c@ + I w3 + c@ + I w3 + 3+ c@ + 4/
                         swap c!+
                         I 1+ c@ I 4+ c@ + I 1+ w3 + c@ + I w3 + 4+ c@ + 4/
                         swap c!+
                         I 2+ c@ I 5 + c@ + I 2+ w3 + c@ + I w3 + 5 + c@ + 4/
                         swap c!+
                     6 +LOOP
                     w3 2* +LOOP  drop
                 w 2/ to w  h 2/ to h  w3 2/ to w3  n 1+ to n  false
             ELSE  true  THEN  UNTIL ;
    : create-mipmap4 ( addr w h -- addr )
      over 4* 0 { w h w4 n }
      BEGIN  dup >r
             GL_TEXTURE_2D n GL_RGBA w h
             0 GL_BGRA GL_UNSIGNED_BYTE
             r> glTexImage2D
             w 1 > h 1 > and do-mipmap and  IF
                 dup dup h w4 * bounds ?DO
                     I w4 bounds ?DO
                         I c@ I 4+ c@ + I w4 + c@ + I w4 + 4+ c@ + 4/
                         swap c!+
                         I 1+ c@ I 5 + c@ + I 1+ w4 + c@ + I w4 + 5 + c@ + 4/
                         swap c!+
                         I 2+ c@ I 6 + c@ + I 2+ w4 + c@ + I w4 + 6 + c@ + 4/
                         swap c!+
                         I 3+ c@ I 7 + c@ + I 3+ w4 + c@ + I w4 + 7 + c@ + 4/
                         swap c!+
                     8 +LOOP
                     w4 2* +LOOP  drop
                 w 2/ to w  h 2/ to h  w4 2/ to w4  n 1+ to n  false
             ELSE  true  THEN  UNTIL ;
    : load-texture-ppm ( fd -- )
?texture [IF]
      >r
      scratch $100 r@ read-line throw 2drop
      scratch $100
      BEGIN  drop dup $100 r@ read-line  throw drop  over c@ '#'
             <>  UNTIL
      0. 2swap >number 1 /string 0. 2swap >number 2drop drop nip
      scratch $100 r@ read-line throw 2drop
      ( w h )
      2dup * 3 * dup NewPtr tuck swap r@ read-file throw drop
      r> close-file throw ( w h addr )
      -rot over2 over2 over2 * 3* <>.24 create-mipmap3 DisposPtr
[ELSE]  close-file throw  [THEN] ;
[defined] has-png [IF]
    : load-texture-png ( fd -- )
\       & pngflags push $0015 to pngflags
        read-png-image 4 and IF
            create-mipmap4
        ELSE
            create-mipmap3
        THEN  DisposPtr ;
[THEN]
    : load-texture ( addr u -- )
[defined] has-png [IF]
         s" .png" suffix? IF  load-texture-png  EXIT  THEN
[ELSE]
         s" .ppm" suffix? IF  load-texture-ppm  EXIT  THEN
	 2drop
[THEN]
;

\ text drawing                                       23jul2005py

[defined] xft [IF]  also xconst also xft
    : map>addrwh ( image -- addr w h ) >r
        r@ XImage data @
        r@ XImage width @
        r> XImage height @ ;
    : text-texture ( addr u font-object -- text-o )
	1 textures dup >r set-texture
        font with 2dup size 2swap 2over
	    >2** swap >2** swap 2swap 2over
	    8 -rot pixmap new >r
            $10 color !
            r@ displays with xywh $10 box endwith
            0 0 r@ draw r>
	    endwith ( greymap )
        pixmap with get 2drop dispose endwith
        dup >r map>addrwh
        create-mipmap1 r> XDestroyImage drop
        r> 3d-text new ;
    previous previous
[THEN]

\ normalization                                        03jan99py

    : set-normal ( vl v vr -- )
      get-normal flip @ IF  negate3  THEN
      glNormal3f ;
    : set-normal+! ( vl v vr -- )  over >r
      get-normal flip @ IF  negate3  THEN  
      r> .nz f+ .nzsf! ( z )
         .ny f+ .nysf! ( y )
         .nx f+ .nxsf! ( x ) drop ;

\ point access                                         27feb99py

    : point ( -- addr )  cell+ ; macro
    : prevpoint ( -- addr )
      [ /point negate cell+ ] Literal + ; macro
    : nextpoint ( -- addr )
      [ /point cell+ ] Literal + ; macro
    : oldpoint ( addr -- addr )
      @ 9* cells + ; macro
    : oldprevpoint ( addr -- addr )
      /point - @ 9* cells + ; macro
    : oldnextpoint ( addr -- addr )
      /point + @ 9* cells + ; macro
  
\ texture path primitives                              25feb99py

    : path-bound ( p' p p+ -- pold pnewhi pnewlo )
[defined] glarrays [IF]
      >r swap path+ cell+ swap 1- path+ r> 2- path+ swap ;
[ELSE]
      drop swap $B cells + swap
      @+ /point + swap 9* cells bounds ;
[THEN]
    : compute-normals ( p' p p+ -- p )
      path-bound ?DO
          I point over I oldpoint over2 I oldprevpoint  set-normal+!
          dup I oldnextpoint over I oldpoint I point    set-normal+!
          I prevpoint  I point over2  I oldpoint        set-normal+!
          dup I oldpoint  I point  I nextpoint          set-normal+!
          /point +LOOP  drop ;
    : flat-vertex ( addr -- )  ?maxpoints
      glVertex3fv ;
    : text-vertex ( addr -- )  ?maxpoints
      dup $C + glTexCoord2fv  glVertex3fv ;
    : text-normal-vertex ( addr -- )  ?maxpoints
      dup $C + glTexCoord2fv
      dup $14 + glNormal3fv  glVertex3fv ;
    : normal-vertex ( addr -- )  ?maxpoints
      dup $14 + glNormal3fv  glVertex3fv ;
    : normal-1 ( addr i -- addr )  1 bounds
      DO  I point over I oldpoint  over2 I oldprevpoint  LOOP
      set-normal ;
    : normal-2 ( addr i -- addr )  1 bounds
      DO  I prevpoint  I point over2  I oldpoint  LOOP
      set-normal ;

\ path drawing                                         27feb99py

    : draw-textured-path ( p'' p' p -- )
      gl-mode @ glBegin
      dup  [defined] glarrays [IF]  path+  [THEN]
      #15 cells + dup @ #1. = IF  off  ELSE  drop  THEN
      2dup <> smooth @ and
      IF  2dup path# @ 1+ compute-normals  THEN
      path-bound smooth @
      IF    ?DO  dup I oldpoint text-normal-vertex
                 I point        text-normal-vertex
                 points+
            /point +LOOP
      ELSE  ?DO  I normal-1
                 dup I oldpoint text-vertex
                 I normal-2
                 I point        text-vertex
                 points+
            /point +LOOP
      THEN  drop
      glEnd ;

    : draw-triangle-path ( p'' p' p -- )
      gl-mode @ glBegin
      2dup <> smooth @ and
      IF  2dup path# @ 2+ compute-normals  THEN
      path-bound smooth @
      IF    ?DO  dup I oldpoint normal-vertex
                 I point        normal-vertex
                 points+
            /point +LOOP
      ELSE  ?DO  I normal-1
                 dup I oldpoint flat-vertex
                 I normal-2
                 I point        flat-vertex
                 points+
            /point +LOOP
      THEN  drop
      glEnd ;

    : draw-point-path ( p'' p' p -- )
      gl-mode @ glBegin
      path-bound ?DO
          I normal-2  I point  flat-vertex
          points+
          /point +LOOP  drop
      glEnd ;

    : draw-textured-point-path ( p'' p' p -- )
      gl-mode @ glBegin
      path-bound ?DO
          ( I normal-2 ) I point  text-vertex
          points+
          /point +LOOP  drop
      glEnd ;

    : draw-line-path ( p'' p' p -- )
      gl-mode @ glBegin
      path-bound ?DO
          I normal-1
          dup I oldpoint     flat-vertex
          dup I oldprevpoint flat-vertex
          I normal-2
          dup I oldpoint     flat-vertex
          I point            flat-vertex
          points+
          /point +LOOP  drop
      glEnd ;

    : draw-textured-line-path ( p'' p' p -- )
      gl-mode @ glBegin
      path-bound ?DO
          I normal-1
          dup I oldpoint     text-vertex
          dup I oldprevpoint text-vertex
          I normal-2
          dup I oldpoint     text-vertex
          I point            text-vertex
          points+
          /point +LOOP  drop
      glEnd ;

\ polygon tesselation

  [defined] libGLU [IF]
    : draw-textured-poly-path ( p'' p' p -- )
	[defined] gluNewTess [IF]
	    0e 0e 0e get-xyz >r >r >r  0e 0e 1e get-xyz
	    fs>f r> fs>f f-
	    fs>f r> fs>f f-
	    fs>f r> fs>f f- f>fs f>fs f>fs { nx ny nz }
	    gluNewTess >r
	    r@ GLU_TESS_VERTEX ['] glVertexNormalTexCoord3fv gluTessCallback
	    r@ GLU_TESS_BEGIN ['] glBegin >c gluTessCallback
	    r@ GLU_TESS_EDGE_FLAG ['] glEdgeFlag >c gluTessCallback
	    r@ GLU_TESS_END ['] glEnd >c gluTessCallback
	    r@ GLU_TESS_ERROR ['] noop gluTessCallback
\	    r@ GLU_TESS_WINDING_RULE GLU_TESS_WINDING_POSITIVE s>f gluTessProperty
	    r@ 0 gluTessBeginPolygon
	    r@ gluTessBeginContour
	    path-bound 2dup - /point / 3* dfloats NewPtr
	    r> swap dup >r 2swap
	    ?DO
		I point nx ny nz 3 pick !normal
		sf@+ swap df!+ swap
		sf@+ swap df!+ swap
		sf@+ swap df!+ nip -3 dfloats +
		2dup I point gluTessVertex
		points+
	    /point +LOOP >r >r drop
	    r@ gluTessEndContour
	    r@ gluTessEndPolygon
	    r> rdrop r> DisposPtr gluDeleteTess
	[ELSE]	    
	    gl-mode @ glBegin
	    path-bound ?DO
		I point  text-vertex
		points+
	    /point +LOOP  drop
	    glEnd
	[THEN] ;
    [THEN]
    
\ drawing modes                                        25feb99py

    : textured ( -- ) ['] draw-textured-path IS draw-path
      GL_TEXTURE_2D glEnable GL_QUAD_STRIP gl-mode ! ;
[defined] libGLU [IF]
    : textured-poly ( -- ) ['] draw-textured-poly-path IS draw-path
      GL_TEXTURE_2D glEnable GL_POLYGON gl-mode ! ;
    : poly ( -- ) ['] draw-textured-poly-path IS draw-path
      GL_TEXTURE_2D glDisable GL_POLYGON gl-mode ! ;
[THEN]
    : triangles ( -- ) ['] draw-triangle-path IS draw-path
      GL_TEXTURE_2D glDisable GL_QUAD_STRIP gl-mode ! ;
    : points ( -- ) ['] draw-point-path IS draw-path
      GL_TEXTURE_2D glDisable GL_POINTS gl-mode ! ;
    : textured-points ( -- ) ['] draw-textured-point-path IS draw-path
      GL_TEXTURE_2D glEnable GL_POINTS gl-mode ! ;
    : lines ( -- ) ['] draw-line-path IS draw-path
      GL_TEXTURE_2D glDisable GL_LINES gl-mode ! ;
    : textured-lines ( -- ) ['] draw-textured-line-path IS draw-path
      GL_TEXTURE_2D glEnable GL_LINES gl-mode ! ;

\ close pathes and rounds                              30jan99py

    : finish-round ( -- )  -1 path# +!
[defined] glarrays [IF]
      path# @ 1- path+  #path @ 1- path+  /point move
      #path'' @ IF  #path'' @ #path' @ #path @ draw-path  THEN
      #path'' off
[ELSE]
      path# @ 1- path+  path @ cell+  /point move
      path'' @ IF  path'' @ path' @ path @ draw-path
          path'' HandleOff  THEN
[THEN] ;
    : close-round ( -- )
      [defined] glarrays [IF]
          #path @ path+ path# @ path+ /point 2* move 
          #path' @
      [ELSE]
          path @ @+ /point + swap path+ /point 2* move
          path' @
      [THEN]
      IF  point# @ path# @ path+ !  THEN
      1 path @ +!  finish-round ;
    : next-round ( -- )  close-round open-round ;

[defined] glarrays [IF]
    : close-path ( -- )
      #path'' @  IF  close-round  THEN
      #path' @   IF  #path' @ #path @ dup draw-path  THEN
      #path'' off #path' off #path off  0 path# ! ;
[ELSE]
    : close-path ( -- )
      path'' @   IF  close-round  THEN
      path' @    IF  path' @ path @ dup draw-path  THEN
      path @     IF  path     HandleOff  THEN
      path' @    IF  path'    HandleOff  THEN
      path'' @   IF  path''   HandleOff  THEN ;
[THEN]
    : end-path ( -- )
        look-at { z y x }
        next-round  path-points @ 0 ?DO
            set
        LOOP  close-round
        path-points @ 2+ 1 ?DO
            z y x I path+ !normal
        LOOP  close-path ;

\ debugging aids                                       03jan99py

\    : .trans  trans .matrix ;

\ cloning                                              14feb99py

    | Variable clone-init
    : clone-handle ( addr -- )
      dup @ 0= IF  drop  EXIT  THEN  >r
      r@ @ r@ GetHandleSize dup r@ Handle!
      r> @ swap move ;
    : clone ( -- o )
[defined] glarrays [IF]
      #path'' @  IF  close-round  THEN
[ELSE]
      path'' @   IF  close-round  THEN
[THEN]
      clone-init on
      trans 3d-turtle new 3d-turtle with
          trans last-turtle over - move
          path     clone-handle
[defined] glarrays [ 0= ] [IF]
          path'    clone-handle
          path''   off
[THEN]
          matrix-stack clone-handle
      ^ endwith  clone-init off ;

    : >turtle ( -- )  postpone clone postpone with ;
    : turtle> ( -- )  postpone dispose postpone endwith ;

\ high level primitives                                27dec99py

    : segment ( r d n -- )  forward
      { f: r } next-round  0 DO  r set-r  LOOP ;
    : sphere ( r n -- )
      pi dup fm/ set-dphi
      dup 2* start-path
      dup 1 DO  >matrix I'
          pi I I' fm*/ fover f>r fsincos f>r fover f*
          fswap 1e fr> f- f* 2* segment
          fr>  matrix>
      LOOP  f2* forward
      drop end-path ;
    : cylinder ( r1 r2 d n -- ) { f: r1 f: r2 f: d }
      2pi dup fm/ set-dphi
      dup start-path
      r1 .01e f* 0e dup segment  \ ugly workaround
      r1 0e dup segment
      r1 0e dup segment
      r2 d  dup segment
      r2 0e     segment
      end-path ;

\ init and dispose                                     10jan99py
  
    : init ( fnear ffar w h / -- )
      clone-init @ ?EXIT
[defined] debug-points [IF]
      #points off
[THEN]
[defined] glarrays [IF]
      [ $8000 4+ 9* cells ] Literal path Handle!
      0 path# !
[THEN]
      1e x-text sf!  1e y-text sf!
      1e rot-mode sf! init-matrix init-OpenGL init-device
      triangles no-texture ;
    : dispose ( -- )  close-path
[defined] glarrays [IF]
      path         @ IF  path         HandleOff  THEN
[THEN]
      matrix-stack @ IF  matrix-stack HandleOff  THEN
      super dispose ;

class;

3d-text implements
    : draw ( dx dy dpy -- ) 3d-turtle with r>
	    smooth push smooth off  z-off push
	    x-text push y-text push x-toff push y-toff push
	    'draw-path push 'do-texture push
	    GL_TEXTURE_2D glIsEnabled >r >r ^
	endwith >r
	w @ s>f wt @ fm/
	h @ s>f ht @ fm/
	.005e w @ fm* .005e h @ fm* 1e texture @ r> 3d-turtle with
	    set-texture textured xy-texture
	    >matrix scale-xyz
	    swap s>f f2/ fnegate s>f f2/ fnegate 0e forward-xyz
	    y-text sf! x-text sf!
	    0e fdup x-toff sf! y-toff sf!
	    3 open-path
	    0e 0e set-xy 0e 1e set-xy next-round
	    1e 0e set-xy 1e 1e set-xy next-round close-path
	    matrix>
	endwith GL_TEXTURE_2D r> IF  glEnable  ELSE  glDisable  THEN ;
class;

previous previous previous previous previous previous previous
