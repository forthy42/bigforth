\ *****************************************************************
\                   Loading all necessary modules
\ *****************************************************************
\needs float import float also float
\ \needs locals| | include locals.fs
\ \needs $@ | include string.fs
[IFUNDEF] farray.fs include farray.fs [THEN]
[IFUNDEF] string.fs include string.fs [THEN]
\ only forth definitions also assembler
\ warning off
\ \needs locals| | include locals.fs
\ warning on
\ \needs $@ | include string.fs
\ previous also float
\ [IFUNDEF] farray.fs include farray.fs [THEN]
\ *****************************************************************
\                Words necessary to define variables
\ *****************************************************************
forth also
: file->dim ( addr u -- x y z l )
    r/o open-file throw 4 cells allocate throw
    { fid t_pad |
    t_pad 4 cells fid read-file throw drop
    t_pad dup @ swap cell+ dup @ swap cell+ dup @ swap cell+ @
    fid close-file throw
    t_pad free throw } ;
 previous
\ 0 value отладка

\ *****************************************************************
\                          Global variables
\ *****************************************************************
\ ---------------------------User-defined--------------------------
\ A file used by this program should be saven in the following
\ binary format:
\ First 32 bytes (4 32-bit words)
\ x y and z dimensions of the array
\ l - length of an element in bytes
\ The array that follows should be saved as
\ [a[:,1,1]..a[:,n,1]]..[a[:,1,m]..a[:,n,m]]
\ -----------------------------------------------------------------
variable 3dfile
s" brainsmall.dat"
3dfile $!

3dfile $@ file->dim
constant lformat
constant zdim
constant ydim
constant xdim

\ define fetching operation depending on the size of an element
lformat 1 = [IF]
    ' c@ alias fetch@
[THEN]
lformat 2 = [IF]
    ' w@ alias fetch@
[THEN]
lformat 4 = [IF]
    ' @ alias fetch@
[THEN]

\ 3d array that'll hold the final CT cube
xdim ydim zdim lformat * * * allocate throw constant cube{{{

\ find the closest power of 2 to the array size
: nearest2pow ( n -- nearest bigger 2^{c} )
    2 begin 2* 2dup <= until nip ;

xdim ydim max nearest2pow value texdimxy
zdim xdim max nearest2pow value texdimzx
zdim ydim max nearest2pow value texdimzy

\ reserve texture array
texdimxy dup 3 * * allocate throw constant imtext-xy 
texdimzy dup 3 * * allocate throw constant imtext-zy
texdimzx dup 3 * * allocate throw constant imtext-zx

\ calsulate texture offsets
texdimxy xdim - 2/ constant xy-offset-x
texdimxy ydim - 2/ constant xy-offset-y
texdimxy xy-offset-y * xy-offset-x + 3 * value xy-offset

texdimzx xdim - 2/ constant zx-offset-x
texdimzx zdim - 2/ constant zx-offset-z
texdimzx zx-offset-x * zx-offset-z + 3 * value zx-offset

texdimzy ydim - 2/ constant zy-offset-y
texdimzy zdim - 2/ constant zy-offset-z
texdimzy zy-offset-y * zy-offset-z + 3 * value zy-offset

forward gmax
\ *****************************************************************
\                              Words
\ *****************************************************************
\ maximal element of my 2D array
: max-arr ( array xdim ydim -- max )
    0 -rot * 0 do
	swap dup lformat i * + aligned fetch@ rot 2dup
	> if drop
	else nip then
    loop
    nip ;
\ minimal element of my 2D array
: min-arr ( array xdim ydim -- max )
    0 -rot * 0 do
	swap dup lformat i * + aligned fetch@ rot 2dup
	< if drop
	else nip then
    loop
    nip ;

: maxxz ( array xdim zdim -- max )
    \   0 -rot * 0 do
    drop 2drop gmax
    \    loop nip
;
: maxyz ( array ydim zdim -- max )
    \   0 -rot * 0 do
    drop 2drop gmax
    \    loop nip
;

: gulp->array ( array fid x y z l -- )
    { fd x y z l |
    4 cells extend fd reposition-file throw
    x y z l * * * fd read-file throw drop } ;

\ functions to work with palletes are written basen on
\ the source code of http://amide.sourceforge.net/
: temp1 ( datum max min -- f: tmp )
    rot >r dup r> swap - s>f - s>f f/ ;
: red-temp ( datum max min -- r g b )
    >r 2dup r@ temp1 r> 2dup - !255 s>f f/ 
    { datum max min f: temp f: scale |
    temp !1.0 f> if $FF $FF $FF exit then
    temp !0.0 f< if 0 0 0 exit then    
    temp !0.7 f>= if $FF else datum min - s>f scale f* !0.7 f/ f>s then
    temp !0.5 f>= if
	datum s>f min s>f !2 f/ f- max s>f !2 f/ f-
	scale f* !2 f* f>s else 0  then    
    temp !0.5 f>= if
    	datum s>f min s>f !2 f/ f- max s>f !2 f/ f-
	scale f* !2 f* f>s else 0 then } ;
: temp2 ( datum max min -- f: tmp )
    temp1 !500 f*
    fdup !255 f> if !511 fswap f- then
    fdup !0 f< if fdrop !0 then ;
: bwb-temp ( datum max min -- r g b )
    temp2 f>s dup dup ;
: blue-temp ( datum max min -- r g b )
    red-temp swap rot ;
: green-temp ( datum max min -- r g b )
    red-temp rot swap ;
: hot-metal-temp ( datum max min -- r g b )
    >r 2dup r@ temp1 r>
    { datum max min f: temp |
    temp !1.0 f> if $FF $FF $FF exit then
    temp !0.0 f< if 0 0 0 exit then
    temp [ !182 !255 f/ ] fliteral f>= if $FF else temp !255 f* [ !255 !182 f/ ] fliteral f* f>s then
    temp [ !128 !255 f/ ] fliteral f<
    if $00 else
	temp [ !219 !255 f/ ] fliteral f>=
	if
	    $FF
	else
	    temp [ !128 !255 f/ ] fliteral f- !255 f* [ !255 !91 f/ ] fliteral f* f>s
	then
    then
    temp [ !192 !255 f/ ] fliteral f>= if $FF else
	temp [ !192 !255 f/ ] fliteral f- !255 f* [ !255 !63 f/ ] fliteral f* f>s then } ;
: hot-blue-temp ( datum max min -- r g b )
    hot-metal-temp swap rot ;
: old-temp ( datum max min -- r g b )
    - swap s>f !255 f* s>f f/ fround f>s dup 0<> if 1- then dup dup ;    
: swap3 ( a b c -- c b a )
    -rot swap ;

' bwb-temp alias current-temp
\ array to image area in memory
: fillimagexy ( arr{{ xdim ydim graynorm itext -- )
    xy-offset +
    { arr{{ xd yd gn itext |
    yd 0 do
	xd 0 do
	    arr{{ xd yd * 1- i j xd * - - lformat * +
	    fetch@ gn 0 current-temp
	    itext j texdimxy 3 * * + i 3 * +
	    dup >r  c!
	    r@  1+ c!
	    r>  2+ c!
	loop
    loop
    } ;

: xzslice ( i j n -- addr-increment )
    xdim * + swap [ ydim xdim * ] literal * + lformat * ;
: fillimagezx ( n arr{{ xdim zdim graynorm itext -- )
    zx-offset +
    { n arr{{ xd zd gn itext |
    xd 0 do
	zd 0 do
	    arr{{ i j n xzslice +
	    fetch@ gn 0 current-temp
	    itext j texdimzx 3 * * + i 3 * +
	    dup >r  c!
	    r@  1+ c!
	    r>  2+ c!	    
	loop
    loop } ;

: yzslice ( i j n -- addr-increment )
    swap xdim * + swap [ ydim xdim * ] literal * + lformat * ;    
: fillimagezy ( n arr{{ ydim zdim graynorm itext -- )
    zy-offset +
    { n arr{{ yd zd gn itext |
    yd 0 do
	zd 0 do
	    arr{{ i j n yzslice +
	    fetch@ gn 0 current-temp
	    itext j texdimzy 3 * * + i 3 * +
	    dup >r  c!
	    r@  1+ c!
	    r>  2+ c!	    	    
	loop
    loop } ;

: xy->textr ( zn -- )
    dup zdim <= if
	cube{{{ xdim ydim lformat * * rot * +
	dup xdim ydim max-arr xdim ydim rot
	imtext-xy dup texdimxy dup 3 * * erase fillimagexy
    else drop ." index exceeds matrix dimension" cr then ;

: zy->textr ( xn -- )
    dup xdim <= if
	cube{{{ ydim zdim
	cube{{{ ydim zdim maxyz imtext-zy dup texdimzy dup 3 * * erase fillimagezy
    else drop ." index exceeds matrix dimension" cr then ;

: zx->textr ( yn -- )
    dup ydim <= if 
	cube{{{ xdim zdim
	cube{{{ xdim zdim maxxz imtext-zx dup texdimzx dup 3 * * erase fillimagezx
    else drop ." index exceeds matrix dimension" cr then ;

: textr->ppm ( itext xdim ydim -- )
    { im x y |
    ." P3" cr
    ." # CT" cr
    x . y . cr
    255 . cr
    y 0 do
	x 0 do
	    im j x 3 * * + i 3 * +
	    dup     c@ .
	    dup  1+ c@ .
	    2+ c@ .
	loop cr
    loop } ;


3dfile $@ r/o open-file throw value fid
!time
cube{{{ fid xdim ydim zdim lformat gulp->array
.time
fid close-file throw
cube{{{ xdim ydim * zdim max-arr constant gmax

[IFDEF] отладка
    gmax . cr
    cube{{{ xdim ydim * zdim min-arr . cr
    cr xdim . ydim . zdim . cr
    cr texdimzy . texdimxy . cr
    \            ydim 2/ 50  + zx->textr
    \            imtext-zx texdimxy texdimz textr->ppm
    \        xdim 2/  zy->textr
    \        imtext-zy texdimxy texdimz textr->ppm
    \    zdim 2/  xy->textr
    \    imtext-xy texdimxy dup  textr->ppm    
    bye    
[THEN]

\ *****************************************************************
\                              Action
\ *****************************************************************
[IFUNDEF] отладка
    zdim 2/ xy->textr    
    ydim 2/ zx->textr
    xdim 2/ zy->textr
    \ *****************************************************************
    \                             Cleanup
    \ *****************************************************************
[THEN]
