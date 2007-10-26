dos also memory also
\needs glconst | import glconst
\needs 3d-turtle include 3d-turtle.fs
float also glconst also opengl also

Create .white !&1  f>fs , !&1  f>fs , !&1  f>fs , !1  f>fs ,
Create .brown !&.8 f>fs , !&.4 f>fs , !&.2  f>fs , !1  f>fs ,
Create .rot   !&1  f>fs , !&.2 f>fs , !&.2  f>fs ,  !1  f>fs ,
Create .green !&0  f>fs , !&.8 f>fs , !&.2 f>fs , !&1  f>fs ,
Create .blue !&.07  f>fs , !&.36 f>fs , !&.94 f>fs , !&1  f>fs ,
Create .bone !&0.7698 f>fs , !&0.9167 f>fs , !&0.9167 f>fs , !1 f>fs ,

Create .ambient 1 , 1 , 1 , 1 ,
Create front_shininess  !&20.0 f>fs ,
Create front_specular   !&1   f>fs dup , dup , , #1 ,
Create no_specular      !0  f>fs dup , dup , , #1 ,

: .color ( addr -- )
    GL_FRONT GL_AMBIENT_AND_DIFFUSE rot glMaterialfv ;

3d-turtle with
    F : квадрат
	{ f: st f: fst |    
	!0 set-dphi
	4 open-path
	GL_FLAT glShadeModel
	2 0 do
	    next-round	  	  
	    !0 !0 set-xy
	    !0 !0.000001 set-xy
	    !0 st set-xy
	    !0.000001 st set-xy	    
	    st forward
	loop
	GL_SMOOTH glShadeModel
	} close-path ;
    F : крест ( f: x y w h -- )
	{ f: x f: y f: w f: h |
	y x !0 forward-xyz
	GL_FLAT glShadeModel        
	2 open-path
	2 0 do
	    next-round
	    h !0  set-xy	
	    h fnegate !0 set-xy
	    !.001 forward
	loop close-path
	!-.001 forward    
	2 open-path
	2 0 do
	    next-round
	    !0 w fnegate set-xy	
	    !0 w set-xy
	    !.001 forward
	loop close-path
	GL_SMOOTH glShadeModel    
	} ;
endwith

Variable wait' 0 wait' !

: .text ( n -- )
    ?texture [IF]
	.white .color GL_TEXTURE_2D glBindTexture ;
[ELSE]  drop ;  [THEN]
include 3dskull.fs

: .emission ( addr -- )
    GL_FRONT GL_EMISSION rot glMaterialfv ;

: drawzy ( o t1 mx my -- )
    { t1 mx my |
    glcanvas with
	!3 !60 w @ h @ { w1 h1 |
	w1 h1 3d-turtle new  3d-turtle with
            GL_BLEND glEnable
            GL_SRC_ALPHA GL_ONE_MINUS_SRC_ALPHA glBlendFunc	    
	    0 !5 !5 !-10 get-xyz GL_POSITION 0 set-light
	    
	    \ crosshair (перекрестье)
	    >matrix
	    .blue .emission	
	    .rot  .color
	    lines wait' off
	    !3.000001 forward
	    w1 h1 min s>f 1/f f2* scale
	    h1 s>f f2/ fnegate w1 s>f f2/ fnegate !0 forward-xyz
	    mx s>f
	    h1 my negate + s>f
	    w1 s>f
	    h1 s>f
	    крест
	    matrix>
	    
	    \ projection
\	    .bone .emission    
	    .bone .color
	    !0 !0 !3.01 forward-xyz
	    textured 0 wait' +! t1 .text smooth on        
	    !2 scale
	    !-0.5 !-0.5 !0 forward-xyz        
	    pi f2/ up    
	    zp-texture 1 s>f 1 s>f квадрат
	    dispose endwith
    endwith } } ;