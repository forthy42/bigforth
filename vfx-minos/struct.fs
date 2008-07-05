\ bigForth structure creation                          18aug96py

: align-by ( w l -- w' ) dup 1- swap invert and invert negate
  cell min >r 1- r@ / 1+ r> * ;

: struct: ( w+ length -- w+ )
    dup >r align-by r>
    swap Create dup , +
    DOES> @ + ;

: ?lit, ( -- ) state @ IF postpone Literal THEN ;
  
: struct ( -- length ) ' >body @ ;
: sizeof ( -- length ) struct ?lit, ; immediate

Variable #struct
Vocabulary (struct

also (struct definitions

: { ( len -- len len len ) dup dup 1 #struct +! ;     immediate
: | ( len +len actlen -- len maxlen len ) max over ;  immediate
: } #struct @  IF  -1 #struct +! max nip EXIT  THEN
  get-current previous definitions previous
  Create immediate swap , ,
  DOES> cell+ @ >r get-order r> swap 1+ set-order '
    state @ IF  compile,  ELSE  execute  THEN
    previous ;     immediate

: byte 1 struct: ;
: short 2 struct: ;
: cell 1 cells struct: ;
: ptr cell ;
: double 2 cells struct: ;
: string chars struct: ;

previous definitions

: struct{ ( -- )
  get-order get-current swap 1+ set-order  wordlist set-current
  also (struct #struct off 0 ;

1 constant byte
2 constant short
\ 4 constant cell
cell constant ptr
8 constant double
