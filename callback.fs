\ callbacks for bigFORTH                             11jan2007py

Vocabulary callbacks

Code (callback
    R:  AX pop  SI push  UP push  OP push  sys-sp A#) push
    -$2000 SP D) SI lea
    [IFDEF] win32
	3 [FOR] -$1000 SP D) SP lea  SP ) CX mov  [NEXT]
	$2000 SI D) SP lea  [THEN]
    ;c: 'up @ up!  rp@ $3F00 - sys-sp !
    s0 @ >r sp@ 1 cells + s0 !  rp@ 6 cells + swap  catch
    ?dup IF  r> s0 ! r> sys-sp !  throw  THEN
    r> s0 !  >c: R:  sys-sp A#) pop  OP pop  UP pop  SI pop
    ret  end-code

: calldoes, ( -- )
  DOES>  Create postpone (callback
    @ compile, compile, 0 postpone ; ;
: callback  Create  here 0 , calldoes,
    also callbacks :noname ;

also callbacks definitions

Code (int)   AX DX mov  AX pop
    Next end-code macro 0 :ax T&P
' (int) alias (float)
' (int) alias (void)
Code int   AX push  DX ) AX mov  cell # DX add
    Next end-code macro :ax 0 T&P
Code df  .fl DX ) fld  8 # DX add
    Next end-code macro
Code sf  .fs DX ) fld  4 # DX add
    Next end-code macro

previous definitions

: callback;  postpone ; swap ! previous ; immediate

\ pointer calls                                      11jan2007py

| Code .save2   BP -4 SI D) mov  -4 SI D) SI lea
    sys-sp A#) BP mov  Next end-code macro

also dos

: fptr  ind-call on  s-offset off  direction off
    : compile .save2
    legacy @ IF  legacy @ 0< IF  compile <rev>  THEN
	swap compile ints compile (int)  THEN ;

previous

: func@ ( xt -- cfptr ) dup 2- wx@ abs + &11 - cfa@ ;
: func' ' func@ ;
: [func'] postpone ['] postpone func@ ; immediate restrict
\ converts a C binding to its function pointer

\ example
false [IF]
callback 2:1 (int) int int callback;
: cb-test  ." Testing callbacks:" .s ." gives " + .s cr ;
: cb-test2  ." Testing callbacks:" .s ." gives " + .s cr abort" failed" ;
' cb-test 2:1 c_plus
' cb-test2 2:1 c_plus2
dos legacy off fptr 2:1call int int (int) forth
1 2 c_plus 2:1call .
1 2 c_plus2 2:1call .
[THEN]
