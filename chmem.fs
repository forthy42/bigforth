\ change memory usage                                  15sep97py

$3   Constant +scr
$298 Constant +pos

: K 1024 * ;
: M K K ;

: chmem ( n -- )  use  +scr block +pos + ! update flush ;
: mem? ( -- ) use +scr block +pos + @ . ;

