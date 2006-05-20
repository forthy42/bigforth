\ Generate wave form                                   03sep97py

\ generate pattern with:
\ bigforth 'include genwave.fs $2000 genwave bye' >wave.trc
\ load with "wave wave.trc"

include random.fs
include fileop.fb fileop also

: .rs  .r space ;
: genwave ( n -- ) base push
  ."           X    X  X        P        P    P   P   P"
  $40 0 DO ."     +" LOOP cr
  ."           1    2  3        1        2    3   4   5"
  $40 0 DO ."     " 'A i + emit  LOOP cr
  ." --------------------------------------------------"
  $40 0 DO ." -----" LOOP cr
  0 ?DO  decimal I 8 .r ." :" hex
         I $F and 2 .rs    I $1356CA * 8 >> $3A and 20 - 4 .rs
         I 1 and 2 .rs     I 8 / 8 .rs
         I random 8 .rs    I $1356CA * 4 >> $A3 and 4 .rs
         $20 random 3 .rs  $20 random I and 3 .rs
\         I $1356CA * 9 .rs
         $40 0 DO j i 1+ mod 4 .rs LOOP
         cr
  LOOP ;

