\ useful utilities                                     26jul98py

: >xyxy ( x y w h -- x y x y )  2over rot + -rot + 1- swap 1- ;
: >xywh ( x y x y -- x y w h )
  2over rot - -rot - 1+ swap negate 1+ ;
Code p+ ( x y x' y' -- xs ys )
     DX pop  CX pop  CX AX add  DX SP ) add
     next end-code macro
Code p- ( x y x' y' -- xs ys )
     DX pop  CX pop  CX AX sub  AX neg  DX SP ) sub
     next end-code macro
Code p2/ ( x y -- x' y' )  AX 1 # shr  SP ) 1 # shr
     Next end-code macro
: xywh- ( x y w h dwh -- x' y' w' h' ) >r
  r@ 2* dup p- 2swap r> dup p+ 2swap ;
