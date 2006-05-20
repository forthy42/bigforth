\ extended GL canvas widget                            12jul98py

also minos also definitions

glcanvas class glcanvas+
    cell var layer
    0 var trans-matrix
    1 floats var x,x
    1 floats var x,y
    2 floats var x,z
    1 floats var y,x
    1 floats var y,y
    2 floats var y,z
    1 floats var z,x
    1 floats var z,y
    2 floats var z,z
    1 floats var +x
    1 floats var +y
    1 floats var +z
    1 floats var +w
public:
    method layer:
    method fd-z
    method rt-x
    method rt-y
    method add-point
    method point
    method del-point
how:
    : init  ( exec w w+ h h+ dpy -- )
      super init  s" " layer $!
      !1 x,x f!  !1 y,y f!  !1 z,z f!  !1 +w  f! ;
    : layer: ( -- )
      current-layer @ layer1 =  IF  layer2  ELSE  layer1  THEN
      current-layer ! ;
    : r,phi ( r angle -- fx fy )
      fsincos f>r fover f* fswap fr> f* fswap ;
    : fd-z ( x -- )  +z f@ f+ +z f! ;
class;

previous previous definitions
