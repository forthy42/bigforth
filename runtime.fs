\ This is an adaption of the matrix multiplication benchmark for using
\ run-time code generation (inspired by lee&leone96)

\ @InProceedings{lee&leone96,
\   author = 	 {Peter Lee and Mark Leone},
\   title = 	 {Optimizing ML with Run-Time Code Generation},
\   crossref =	 {sigplan96},
\   pages =	 {137--148}
\ }
\ @Proceedings{sigplan96,
\   booktitle = 	 "SIGPLAN '96 Conference on Programming Language
\ 		  Design and Implementation",
\   title = 	 "SIGPLAN '96 Conference on Programming Language
\ 		  Design and Implementation",
\   year = 	 "1996",
\   key = 	 "PLDI '96"
\ }

\ The original version is in comments.
\ The results with Gforth on a DecStation 5000/200 (25MHz R3000) are very nice:
\ original program:		 45.3s user time
\ with run-time code generation: 21.4s user time
\ NOTE: This version needs 0.5MB data space and a lot of code space, too.

\ A classical benchmark of an O(n**3) algorithm; Matrix Multiplication
\
\ Part of the programs gathered by John Hennessy for the MIPS
\ RISC project at Stanford. Translated to forth by  Marty Fraeman,
\ Johns Hopkins University/Applied Physics Laboratory.

\ MM forth2c doesn't have it !
: mybounds  over + swap ;

1 cells constant cell

variable seed

: initiate-seed ( -- )  74755 seed ! ;
: random  ( -- n )  seed @ 1309 * 13849 + 65535 and dup seed ! ;

200 constant row-size
row-size cells constant row-byte-size

row-size row-size * constant mat-size
mat-size cells constant mat-byte-size

align create ima mat-byte-size allot
align create imb mat-byte-size allot
align create imr mat-byte-size allot

: initiate-matrix ( m[row-size][row-size] -- )
  mat-byte-size mybounds do
    random dup 120 / 120 * - 60 - i !
  cell +loop
;

1 [IF]
: over@   over @ ; macro
: *+cell+over@  * + >r cell+ r> over @ ; macro
: *+cell+  * + >r cell+ r> ; macro

: gen-innerproduct ( a[row][*] -- xt )
\ xt is of type ( b[*][column] -- n )
\ this would be a candidate for using ]] ... [[
 >r :noname r>
 0 POSTPONE literal
 POSTPONE over@
 row-size 1- 0 do
   dup @ POSTPONE literal POSTPONE *+cell+over@ row-byte-size +
  loop
  dup @ POSTPONE Literal POSTPONE *+cell+
  drop
 POSTPONE nip POSTPONE ;
;

Variable <here

: main  ( -- )
  initiate-seed
  ima initiate-matrix
  imb initiate-matrix 
  here <here !
  imr ima mat-byte-size mybounds do
   i gen-innerproduct swap
    imb row-byte-size mybounds do ( r xt )
      i 2 pick execute over ! cell+
    cell +loop
    <here @ dp !
    nip \ !! forget the xt
  row-size cells +loop
  drop
;
[ELSE]
: innerproduct ( a[row][*] b[*][column] -- int)
  0 row-size 0 do ( a b int )
    >r over @ over @ * r> + >r
    cell+ swap row-byte-size + swap
    r>
  loop
  >r 2drop r>
;

: main  ( -- )
  initiate-seed
  ima initiate-matrix
  imb initiate-matrix 
  imr ima mat-byte-size mybounds do
    imb row-byte-size mybounds do
      j i innerproduct over ! cell+ 
    cell +loop
  row-size cells +loop
  drop
;
[THEN]


