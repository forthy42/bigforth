\ bigForth structure creation                          18aug96py

: align-by ( w l -- w' ) dup 1- swap invert and invert negate
  cell min >r 1- r@ / 1+ r> * ;

| : .struct' ( w+ length -- w+ )
    dup >r align-by r>
    swap : over ?dup IF  compile Literal compile +  THEN
    compile ; hmacro :#+ lastopt @ w!
    dup 0= IF  immediate  THEN  + ;

: sizeof ( -- length ) ' >body @ ?lit, ; immediate
: struct ( -- length ) ' >body @ ;

[IFDEF] do-local
    also do-local definitions
    : R: struct ['] local-field: ;
    previous definitions
[THEN]

| Variable #struct      slowvoc on
| Vocabulary (struct    slowvoc off

also (struct definitions

: { ( len -- len len len ) dup dup 1 #struct +! ;     immediate
: | ( len +len actlen -- len maxlen len ) max over ;  immediate
: } #struct @  IF  -1 #struct +! max nip EXIT  THEN
  current @ toss definitions toss
  Create immediate swap , A,  compile [
  DOES> cell+ @ also context ! name parser toss ;     immediate

previous definitions

| : struct-parse
    find dup IF    >r execute r> 0> ?exit
             ELSE  drop number drop  THEN
    >in @ name w@ $5C01 = IF drop + ELSE >in ! .struct' THEN
    [ lastcfa @ ] ALiteral IS parser ;
: struct{ ( -- )
  also current @ context !  align here 0 voc, current !
  also (struct #struct off 0 ['] struct-parse IS parser ;

1 constant byte
2 constant short
\ 4 constant cell
cell constant ptr
8 constant double
