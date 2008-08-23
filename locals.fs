\ Locals                                               16feb92py

\needs loffset  Variable loffset

slowvoc @ slowvoc on
| Vocabulary locals slowvoc !
| : l, loffset @ swap - here 3 - c! ;
| Code local@  ( -- x )
       AX push  0 RP D) AX mov  Next end-code macro  :ax 0 T&P
| Code local^  ( -- x )
       AX push  0 RP D) AX lea  Next end-code macro  :ax 0 T&P
| Code flocal@  ( -- x )
       .fx 0 RP D) fld  Next end-code macro
| Code local!  ( x -- )
       AX 0 RP D) mov  AX pop  Next end-code macro  0 :ax T&P
| Code flocal!  ( x -- )
       .fx 0 RP D) fstp  Next end-code macro
| Code delocal  0 RP D) RP lea  Next end-code macro
| Code (f>l ( f -- ) $C # RP sub  .fx RP ) fstp 
       Next end-code macro

: DO      8     loffset +!  compile DO     ; immediate restrict
: ?DO     8     loffset +!  compile ?DO    ; immediate restrict
: FOR     8     loffset +!  compile FOR    ; immediate restrict
: LOOP   -8     loffset +!  compile LOOP   ; immediate restrict
: +LOOP  -8     loffset +!  compile +LOOP  ; immediate restrict
: NEXT   -8     loffset +!  compile NEXT   ; immediate restrict
: >R      cell  loffset +!  compile >R     ; immediate restrict
: R>     -cell  loffset +!  compile R>     ; immediate restrict
: 2>R     8     loffset +!  compile 2>R    ; immediate restrict
: 2R>    -8     loffset +!  compile 2R>    ; immediate restrict
: RDROP  -cell  loffset +!  compile RDROP  ; immediate restrict
: F>L     &12   loffset +!  compile (f>l   ; immediate restrict

\ Locals                                               19aug93py

: local-field: ( n -- )
  last push lastcfa push
  compile delocal loffset @ over + l, loffset +!
  lastdes push  dp push
  | Create loffset @ , !length hmacro immediate
  DOES> @ compile local^ l, ;
                                             immediate restrict
: dlocal: ( -- )
  last push lastcfa push  compile 2>r lastdes push  dp push
  | Create loffset @ , !length hmacro immediate
  DOES> [ here $A - swap ] @ compile local@ dup cell- l,
                             compile local@ l, ;
                                             immediate restrict
: local: ( -- )
  last push lastcfa push  compile >r lastdes push  dp push
  | Create loffset @ , !length hmacro immediate
  DOES> [ here $A - swap ] @ compile local@ l, ;
                                             immediate restrict
: flocal: ( -- )
  last push lastcfa push  compile f>l lastdes push  dp push
  | Create loffset @ , !length hmacro immediate
  DOES> [ here $A - swap ] @ compile flocal@ l, ;
                                             immediate restrict
: <local ( -- x1 x2 x3 x4 ) current @ also locals definitions
  heap loffset @ current @ @ ;               immediate restrict
: local>  ( x1 x2 x3 x4 -- x1 x3 x4 )  >r rot current ! r> ;
                                             immediate restrict
: local;  ( x1 x3 x4 -- )
  compile delocal over l, & locals ! loffset !
  heap - negate hallot toss ;                immediate restrict
: to >in @ '
  dup cfa@ [ swap ] ALiteral = \ flocal
  IF  >body @ compile flocal! l, drop  EXIT  THEN
  dup cfa@ [ swap ] ALiteral = \ local
  IF  >body @ compile local! -1 allot l, 1 allot drop EXIT THEN
  dup cfa@ [ swap ] ALiteral = \ dlocal
  IF  >body @ dup
      compile local! -1 allot l, 1 allot cell+ 
      compile local! -1 allot l, 1 allot drop EXIT THEN
  drop >in ! compile to ;                    immediate

\ High level locals                                    19aug93py

| 4 cells Constant inlocal#
| Create inlocal  inlocal# cell+ allot  inlocal off
| Variable local?

Vocabulary do-local  also do-local definitions
' local:  AConstant w:
' dlocal: AConstant d:
' flocal: AConstant f:
[IFDEF] struct
: R: struct ['] local-field: ;
[THEN]
: } inlocal on  local? on ; immediate
: -- '} parse 2drop inlocal on local? on ; immediate
: | inlocal off local? on ; immediate
previous definitions

| Variable default-local ['] local: default-local !

| : ({ compile <local  -1
    BEGIN  >in @ >r name count & do-local search-wordlist
	dup 0<=  WHILE
	    IF    execute rdrop >in @ name drop
	    ELSE  default-local @ r>  THEN
    REPEAT
    rdrop drop execute  >in @ >r
    BEGIN  dup 0>= WHILE  >in !  execute  REPEAT
    drop  r> >in ! compile local>
    inlocal @ IF  inlocal 2 cells + 2! inlocal cell+ ! THEN ;

: {  [']  local: default-local ! ({ ;      immediate restrict
: f{ ['] flocal: default-local ! ({ ;      immediate restrict

' local; alias } immediate restrict

\ ANS Locals                                           19aug93py

: (local)  ( addr u -- )  inlocal @ 0=
  IF  compile <local inlocal on
      inlocal 3 cells + 2!  inlocal cell+ 2! THEN
  dup IF    ">tib compile local:
      ELSE  2drop  inlocal cell+ 2@  inlocal 3 cells + 2@
            compile local>
            inlocal 2 cells + 2! inlocal cell+ ! THEN ;

| : inlocal; ( -- )
    inlocal cell+ @ inlocal 2 cells + 2@ compile local; ;
| : ?local;  inlocal @
    IF  inlocal; inlocal off  THEN
    loffset off  local? off ;

: ;      ?local; compile ; ;                 immediate restrict
: DOES>  ?local; compile DOES> ;             immediate
: EXIT   local? @ IF  compile delocal 0 l,  THEN
  compile EXIT ;                             immediate restrict
: ?EXIT  local? @ IF
	compile IF  compile  EXIT  compile  THEN
    ELSE
	compile ?EXIT
    THEN ;                                   immediate restrict

: SCOPE  inlocal 2@ inlocal 2 cells + 2@ loffset @ ;
                                             immediate restrict
: ENDSCOPE  ?local; loffset ! inlocal 2 cells + 2! inlocal 2! ;
                                             immediate restrict

Assembler definitions
: ;Code  ?local; compile ;Code ;             immediate restrict
Forth definitions

: locals|
  BEGIN  name dup w@ $7C01 = 0=  WHILE
         count (local)  REPEAT  0 (local) ;  immediate restrict

[IFDEF] environment
   environment
   true to locals
   true to locals-ext
   Forth
[THEN]
