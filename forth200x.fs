\ forth 200x stuff

: Synonym  Header  -2 allot bl word find dup 0= IF no.extensions THEN
    dup 0> IF  immediate  THEN
    1 and 0= IF  restrict  THEN  A,
    $20 last @ dup >r c@ or r> c!  reveal ;

\ defer@/defer!/action-of

: defer@ ( xt -- xt )  >body @ ;
: defer! ( xt1 xt2 -- )  >body ! ;
Synonym action-of what's

\ ekey stuff

: ekey ( -- )  key kbshift @ $18 lshift or ;
: ekey? ( -- ) key? ;

$01000000 Constant k-shift-mask
$02000000 Constant k-capslock-mask
$04000000 Constant k-ctrl-mask
$08000000 Constant k-alt-mask
$10000000 Constant k-numlock-mask
$40000000 Constant k-mouse-mask
$80000000 Constant k-fn-mask

: ekey>char ( u -- u false | c true )
    dup $C0000000 and 0= over $FFFFFF and $100 < and
    dup IF  swap $FF and swap  THEN ;
: ekey>xchar ( u -- u false | xc true )
    dup $C0000000 and 0=
    dup IF  swap $FFFFFF and swap  THEN ;
: ekey>fkey ( u1 -- u2 f )
    dup k-fn-mask and 0<>
    dup IF
	drop [ k-capslock-mask k-numlock-mask or k-fn-mask or
	       k-mouse-mask or invert ] Literal and
	true
    THEN ;

: k-fns: ( start n -- ) bounds ?DO  I Constant  LOOP ;

0 AValue keycode-start
0 AValue keycode-end

: simple-fkey-string ( u -- addr u )
    keycode-end  BEGIN  dup keycode-start >  WHILE
	    2dup name> 4 + @ =  IF  nip count $1F and  EXIT  THEN
	    cell- @ cell+
    REPEAT  2drop s" f-unknown" ;

: fkey. ( u -- ) \ gforth fkey-dot
    \ Print an fkey as string
    dup $FFFFFF and
    simple-fkey-string type
    dup k-shift-mask and IF ."  k-shift-mask or" THEN
    dup k-ctrl-mask  and IF ."  k-ctrl-mask or"  THEN
    k-alt-mask       and IF ."  k-alt-mask or"   THEN ;

last @ to keycode-start
    
$FF50 Constant k-home
$FF51 Constant k-left
$FF52 Constant k-up
$FF53 Constant k-right
$FF54 Constant k-down
$FF55 Constant k-prior
$FF56 Constant k-next
$FF57 Constant k-end

$FF63 Constant k-insert

$FFBE 12 k-fns: k-f1 k-f2 k-f3 k-f4 k-f5 k-f6 k-f7 k-f8 k-f9 k-f10 k-f11 k-f12

last @ to keycode-end

\ structures

: begin-structure ( -- here )  $AAAAAAAA Constant lastcfa @ 4 + ;
: end-structure ( here size -- )  swap ! ;
: +field ( n1 n2 "name" -- n3 )
    >r >r : r@ postpone Literal postpone + postpone ; macro r> r> + ;

: cfield:       \ n1 <"name"> -- n2 ; Exec: addr -- 'addr
\ *G Create a new field within a structure definition of size 1 CHARS.
  1 +field ;

: field:        \ n1 <"name"> -- n2 ; Exec: addr -- 'addr
\ *G Create a new field within a structure definition of size 1 CELLS.
\ ** The field is ALIGNED.
  cell +field ;

: ffield:       \ n1 <"name"> -- n2 ; Exec: addr -- 'addr
\ *G Create a new field within a structure definition of size 1 FLOATS.
\ ** The field is FALIGNED.
  10 +field ;

: sffield:      \ n1 <"name"> -- n2 ; Exec: addr -- 'addr
\ *G Create a new field within a structure definition of size 1 SFLOATS.
\ ** The field is SFALIGNED.
  4 +field ;

: dffield:      \ n1 <"name"> -- n2 ; Exec: addr -- 'addr
\ *G Create a new field within a structure definition of size 1 DFLOATS.
\ ** The field is DFALIGNED.
  8 +field ;
