\needs float | needs float.fs also float
\ also Forth
0 VALUE TYPE-ID               \ for building structures
FALSE VALUE STRUCT-ARRAY?

TRUE  VALUE is-static?     \ TRUE for statically allocated structs and arrays
: dynamic ( -- )     FALSE TO is-static? ;


\ size of a regular integer
1 CELLS CONSTANT INTEGER

\ size of a long integer
2 CONSTANT LONGINT

\ size of a double integer
\ 2 CELLS CONSTANT DOUBLE

\ size of a regular float
10 CONSTANT FLOATing

\ size of a pointer (for readability)
1 CELLS CONSTANT POINTER

\ : % BL WORD COUNT >FLOAT 0= ABORT" NAN"
\                  STATE @ IF POSTPONE FLITERAL THEN ; IMMEDIATE

\ 3.1415926536E0 FCONSTANT PI
  1.0E0 FCONSTANT F1.0

\ 1-D array definition
\    -----------------------------
\    | cell_size | data area     |
\    -----------------------------

: MARRAY ( n cell_size -- | -- addr )             \ monotype array
     CREATE
       DUP , * ALLOT
     DOES> CELL+
;

\    -----------------------------
\    | id | cell_size | data area |
\    -----------------------------

: SARRAY ( n cell_size -- | -- id addr )          \ structure array
    CREATE
      TYPE-ID ,
      DUP , * ALLOT
    DOES> DUP @ SWAP [ 2 CELLS ] LITERAL +
;

: ARRAY
     STRUCT-ARRAY? IF   SARRAY FALSE TO STRUCT-ARRAY?
     ELSE MARRAY
     THEN
;

\ word for aliasing arrays,
\  typical usage:  a{ & b{ &!  sets b{ to point to a{'s data

: &!    ( addr_a &b -- )
        SWAP CELL- SWAP >BODY  !
;

: }f   ( addr n -- addr[n])       \ word that fetches 1-D array addresses
    OVER CELL-  @
    * SWAP + ALIGNED FALIGNED ;

VARIABLE print-width      6 print-width !

: }fprint ( n addr -- )       \ print n elements of a float array
        SWAP 0 DO I print-width @ MOD 0= I AND IF CR THEN
        DUP I }f F@ F. LOOP
        DROP
;
		
: }fcopy ( 'src 'dest n -- )         \ copy one array into another
    0 DO
	OVER I }f F@
	DUP  I }f F!
    LOOP    
    2DROP
  ;
  
\ 2-D array definition,

\ Monotype
\    ------------------------------
\    | m | cell_size |  data area |
\    ------------------------------

: MMATRIX  ( n m size -- )           \ defining word for a 2-d matrix
        CREATE
          OVER , DUP ,
	  * * ALLOT
        DOES>  [ 2 CELLS ] LITERAL +
;

: _hmatrix ( n m size -- addr )
    rot over * 2 pick * [ 2 cells ] literal +
    allocate throw dup [ 2 cells ] literal + >r
    rot over ! [ 1 cells ] literal + ! r> ;

: hmatrix ( n m size -- )
    create
    rot over * 2 pick * [ 2 cells ] literal + allocate throw dup ,
    rot over ! [ 1 cells ] literal + !
  does> @ [ 2 cells ] literal +
;

: DispHmatrix
    [ 2 cells ] literal - free throw
;

: rmatrix ( addr m size -- )
    create rot dup ,
    rot over ! [ 1 cells ] literal + !    
  does>
    @ [ 2 cells ] literal +
;

: }}row-size [ 2 cells ] literal - @ ;

: }}h    ( addr i j -- addr[i][j] )    \ word to fetch 2-D array addresses
               >R >R                    \ indices to return stack temporarily
               DUP CELL- CELL- 2@     \ &a[0][0] size m
               R> * R> + *
               +
               ALIGNED FALIGNED
;
\ Structures
\    -----------------------------------
\    | id | m | cell_size |  data area |
\    -----------------------------------

: SMATRIX  ( n m size -- )           \ defining word for a 2-d matrix
        CREATE TYPE-ID ,
           OVER , DUP ,
           * * ALLOT
        DOES>  DUP @ TO TYPE-ID
               [ 3 CELLS ] LITERAL +
;


: MATRIX  ( n m size -- )           \ defining word for a 2-d matrix
     STRUCT-ARRAY? IF   SMATRIX FALSE TO STRUCT-ARRAY?
                   ELSE MMATRIX
                   THEN
;




\ : DMATRIX ( size -- )      DARRAY ;


: }}    ( addr i j -- addr[i][j] )    \ word to fetch 2-D array addresses
               >R >R                    \ indices to return stack temporarily
               DUP CELL- CELL- 2@     \ &a[0][0] size m
               R> * R> + *
               +
               ALIGNED FALIGNED
;
	
: }}fprint ( n m addr -- )       \ print nXm elements of a float 2-D array
        CR ROT ROT SWAP 0 DO
                         DUP 0 DO
                                  OVER J I  }} F@ F.
                         LOOP

                         CR
                  LOOP
        2DROP
;

: }}lprint ( n m addr -- )       \ print nXm elements of a longint 2-D array
        CR ROT ROT SWAP 0 DO
                         DUP 0 DO
                                  OVER J I  }}h w@ .
                         LOOP

                         CR
                  LOOP
        2DROP
;
\ function vector definition

\ : noop ; 

: v: CREATE ['] noop , DOES> @ EXECUTE ;
: defines   ' >BODY STATE @ IF POSTPONE LITERAL POSTPONE !
                            ELSE ! THEN ;   IMMEDIATE

: use(  STATE @ IF POSTPONE ['] ELSE ' THEN ;  IMMEDIATE
\ : &     POSTPONE use( ; IMMEDIATE


\ multiplying raw of a matrix by a vector
: raw* ( raw_index matrix vector size --> f:number ) 
  0e0 
  0 do 
      2dup i }f f@ 
	  3 pick i }} f@ 
	  f* f+ 
  loop
  rot drop
;

\ multiplying vector my a matrix
: [*]v ( new_vector matrix vector n m --> f:[ new vector ] )
  0 do
    s>f  fdup i -rot f>s raw* 
	2 pick i }f f! f>s 
  loop
  drop drop drop drop
;

