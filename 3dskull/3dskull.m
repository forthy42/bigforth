#! /home/pliz/soft/bin/xbigforth -m=900M
\ automatic generated code
\ do not edit

also editor also minos also forth

component class skull
    public:
    early widget
    early open
    early dialog
    early open-app
    glcanvas ptr zyprojection
    glcanvas ptr xyprojection
    glcanvas ptr zxprojection
    text-label ptr labelX
    text-label ptr labelY
    text-label ptr labelZ
    ( [varstart] ) cell var zoom
    cell var yz-task
    cell var xz-task
    cell var xy-task
    cell var yz-texture
    cell var xz-texture
    cell var xy-texture    
    cell var xclicked
    cell var yclicked
    cell var zclicked
    cell var mbutton
    cell var mstate ( [varend] ) 
  how:
    : open     new DF[ 0 ]DF s" CT scan" open-component ;
    : dialog   new DF[ 0 ]DF s" CT scan" open-dialog ;
    : open-app new DF[ 0 ]DF s" CT scan" open-application ;
class;

include 3dskull_m.fs

skull implements
( [methodstart] ) : make-yz-task
    zyprojection render &100 0 DO  pause  LOOP
    zyprojection draw   &100 0 DO  pause  LOOP ;
: make-xy-task
    xyprojection render &100 0 DO  pause  LOOP
    xyprojection draw   &100 0 DO  pause  LOOP ;
: make-xz-task
    zxprojection render &100 0 DO  pause  LOOP
    zxprojection draw   &100 0 DO  pause  LOOP ;
: assign
    0 zclicked ! 0 xclicked ! 0 yclicked !
    0 xz-texture ! 0 xy-texture ! 0 yz-texture ! ;

: shift-origin ( x y x' y' -- x y ) rot swap - -rot - swap ;
: rec->sq ( x y w h -- x y )
    min 2/ dup >r + swap r> + swap ;

: sq->textr ( x y w h txdim -- x y )
    s>f fdup
    rot s>f s>f f/ f* f>s
    -rot swap s>f s>f f/ f* f>s swap ;
: textr->array ( x y xoffset yoffset -- x' y' )
    rot swap - -rot - swap ;
: inrange+ ( x w -- x' )
    2dup > if swap drop EXIT then
    drop dup 0 < if drop 0 EXIT then ;
: xz-validate ( x z -- x' z' )
    zdim inrange+ swap
    xdim inrange+ swap ;
: yz-validate ( y z -- y' z' )
    zdim inrange+ swap
    ydim inrange+ swap ;
: xy-validate ( x y -- x' y' )
    xdim inrange+ swap
    ydim inrange+ swap ;
: arr->window ( x y o -- x' y' )
    glcanvas with
	h @ 2/ + swap w @ 2/ + swap
    endwith ;

: redraw-pr
    xz-task @ IF make-xz-task THEN    
    yz-task @ IF make-yz-task THEN     
    xy-task @ IF make-xy-task THEN ;
: force-redraw-pr
    xz-task @ not IF make-xz-task THEN    
    yz-task @ not IF make-yz-task THEN     
    xy-task @ not IF make-xy-task THEN ;

: yzdraw
    yz-task @ 0= IF
	yz-texture @ 1
	zyprojection with
	    3d-turtle del-textures drop
	    1 3d-turtle textures dup
	    3d-turtle set-texture
	    imtext-zy texdimzy dup 3d-turtle create-mipmap3
	endwith
	yz-texture ! 1 yz-task ! make-yz-task EXIT
    THEN
    zyprojection self    
    yz-texture @ yclicked @ zclicked @
    zyprojection self arr->window
    drawzy ;
: xydraw
    xy-task @ 0= IF
	xy-texture @ 1
	xyprojection with
	    3d-turtle del-textures drop	    
	    1 3d-turtle textures dup
	    3d-turtle set-texture
	    imtext-xy texdimxy dup 3d-turtle create-mipmap3
	endwith
	xy-texture ! 1 xy-task ! make-xy-task EXIT
    THEN
    xyprojection self
    xy-texture @ yclicked @ xclicked @ xyprojection self arr->window
    drawzy ;
: xzdraw
    xz-task @ 0= IF
	xz-texture @ 1
	zxprojection with	
	    3d-turtle del-textures drop
	    1 3d-turtle textures dup
	    3d-turtle set-texture
	    imtext-zx texdimzx dup 3d-turtle create-mipmap3
	endwith
	xz-texture ! 1 xz-task ! make-xz-task EXIT THEN
    zxprojection self           
    xz-texture @ xclicked @ zclicked @ zxprojection self arr->window
    drawzy ;

: zy@ ( -- z y )	
    yclicked @ zclicked @
    zyprojection with	
	w @ h @ rec->sq
	w @ h @ min dup
	texdimzy sq->textr
	zy-offset-y zy-offset-z textr->array yz-validate
    endwith ;
: zx@ ( -- z x )	
    xclicked @ zclicked @
    zxprojection with
	w @ h @ rec->sq
	w @ h @ min dup	
	texdimzx sq->textr
	zx-offset-x zx-offset-z textr->array xz-validate
    endwith ;
: xy@ ( -- y x )	
    yclicked @ xclicked @
    xyprojection with
	w @ h @ rec->sq
	w @ h @ min dup	
	texdimxy sq->textr
	xy-offset-y xy-offset-x textr->array xy-validate
    endwith ;

: align2center ( x y w h -- x y )
    2/ rot swap - -rot 2/ - swap ;
: inrange ( x w -- x' )
    2/ >r
    dup r@ negate < if drop r> negate EXIT then
    dup r@ > if drop r> EXIT then rdrop ;
: xy-get ( x y o -- y x )
    glcanvas with
	w @ h @ align2center
	h @ w @ min dup >r
	inrange swap r> inrange swap
    endwith ;
: translate-o ( x y o -- x y o )
    dup glcanvas with -rot xywh 2drop shift-origin endwith rot ;
: coordinates ( x y o -- y x ) translate-o xy-get ;

: itoa ( n -- addr u ) extend <# #s #> ;
: xy-cross ( x y b n -- )
    2drop 2dup
    xyprojection self coordinates xclicked ! yclicked ! redraw-pr
    DOPRESS
    xyprojection self coordinates xclicked ! yclicked ! 2drop
    xy@ itoa labelX text! labelX draw
    itoa labelY text! labelY draw
    zy@ itoa labelZ text! drop labelZ draw
    redraw-pr ;
: zy-cross ( x y b n -- )
    2drop 2dup
    zyprojection self coordinates zclicked ! yclicked ! redraw-pr
    DOPRESS
    zyprojection self coordinates zclicked ! yclicked ! 2drop
    zy@ zdim swap - itoa labelZ text! labelZ draw
    itoa labelY text! labelY draw
    xy@ itoa labelX text! drop labelX draw
    redraw-pr ;
: zx-cross ( x y b n -- )
    2drop 2dup
    zxprojection self coordinates zclicked ! xclicked ! redraw-pr
    DOPRESS
    zxprojection self coordinates zclicked ! xclicked ! 2drop
    zx@ zdim swap - itoa labelZ text! labelZ draw
    itoa labelX text! labelX draw
    xy@ drop itoa labelY text! labelY draw
    redraw-pr ;

: dispose  xy-task @  IF  self dpy cleanup pause xy-task off  THEN
    yz-task @  IF  self dpy cleanup pause yz-task off  THEN
    xz-task @  IF  self dpy cleanup pause xz-task off  THEN    
    super dispose ; ( [methodend] ) 
: widget  ( [dumpstart] )
    GL[ outer with yzdraw endwith ]GL ( MINOS ) ^^ CK[ ( x y b n -- )
    dup 1 = if
	zy-cross
	zy@ zdim swap - 
	xy->textr 0 xy-task !
	zx->textr 0 xz-task !
	force-redraw-pr
	exit	  
    else
	2drop 2drop	  
    then
    ]CK ( MINOS ) $12C $1 *hfil $12C $1 *vfil glcanvas new  ^^bind zyprojection
    $10 $1 *hfil hrule new 
    GL[ outer with xydraw endwith ]GL ( MINOS ) ^^ CK[ ( x y b n -- )
    dup 1 = if
	xy-cross
	xy@ 
	zy->textr 0 yz-task !
	zx->textr 0 xz-task !	
	force-redraw-pr
	exit	  
    else
	2drop 2drop	  
    then      
    ]CK ( MINOS ) $12C $1 *hfil $12C $1 *vfil glcanvas new  ^^bind xyprojection
    &3 vabox new
    $10 $1 *vfil vrule new 
    GL[ outer with xzdraw endwith ]GL ( MINOS ) ^^ CK[ ( x y b n -- )
    dup 1 = if
	zx-cross
	zx@ zdim swap - 
	xy->textr 0 xy-task !
	zy->textr 0 yz-task !
	force-redraw-pr
	exit	  
    else
	2drop 2drop	  
    then            
    ]CK ( MINOS ) $12C $1 *hfil $12C $1 *vfil glcanvas new  ^^bind zxprojection
    $10 $1 *hfil hrule new 
    $14 $1 *hfil $12C $1 *vfil glue new 
    $F $1 *hfill $6E $1 *vfill glue new 
    S" Source array indices" text-label new 
    S" X:" text-label new 
    S" Y:" text-label new 
    S" Z:" text-label new 
    &3 vabox new
    S" " text-label new  ^^bind labelX
    S" " text-label new  ^^bind labelY
    S" " text-label new  ^^bind labelZ
    &3 vabox new
    $14 $1 *hfil $10 $1 *vfil glue new 
    &3 habox new
    $F $1 *hfill $78 $1 *vfill glue new 
    &4 vabox new
    $14 $1 *hfil $12C $1 *vfil glue new 
    &3 habox new
    &3 vabox new
    &3 habox new
    ^^ S[ imtext-xy DisposPtr
    imtext-zy DisposPtr    
    imtext-zx DisposPtr
    close
    ]S ( MINOS ) S" done" button new 
    &1 habox new vfixbox 
    &2 vabox new
    ( [dumpend] ) ;
: init  ^>^^  assign  widget 1 super init ;
class;

: main
    skull open-app
    $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
