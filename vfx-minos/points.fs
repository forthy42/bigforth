\ useful utilities                                     26jul98py

: >xyxy ( x y w h -- x y x y )  2over rot + -rot + 1- swap 1- ;
: >xywh ( x y x y -- x y w h )
  2over rot - -rot - 1+ swap negate 1+ ;
: p+ ( x y x' y' -- xs ys )
    >r swap >r + r> r> + ;
: p- ( x y x' y' -- xs ys )
    >r swap >r - r> r> - ;
: p2/ ( x y -- x' y' )
    2/ swap 2/ swap ;
: xywh- ( x y w h dwh -- x' y' w' h' ) >r
  r@ 2* dup p- 2swap r> dup p+ 2swap ;
