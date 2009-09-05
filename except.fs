\          *** print post mortem dump ***              28feb94py

: dumped  ftab 8 cells + @ cell+ ;

| : 0.r  0 swap  <#  0 ?DO  #  LOOP  #> type ;
| : .header  ( -- )  3 S" AXCXDXBXSPBPSIDI"  bounds
    DO  spaces I 2 type  7  2 +LOOP  drop ;
| : .regs    ( -- )  dumped 8 cells +
    7 FOR  cell- dup @ 8 0.r space  NEXT  drop ;
| : .ip  ( -- )  ."  at: " dumped 8 cells + @
    ( swap 4 0.r ': emit ) dup 8 .r
    [IFDEF] disline  dup IF space disline THEN  [THEN]  drop ;
| : .flags ( -- )
    ." EF:"   dumped $A cells + @ 5 .r 
    ."  int " dumped $B cells + @ 3 .r ;
| : .stack ( -- )  ." Stack: "
    backtrace 8 cells + $10 cells
    dumped 1 cells + dup @
    dumped 3 cells + @ >
    IF  cell+ 2@  ELSE  2@ swap  THEN
    task's s0 @ - negate cell+ min 0 max
    bounds  ?DO  I @ .  cell +LOOP ;
| : .rstack ( -- )  ." Return: "
    backtrace 8 cells bounds  ?DO  I @ .  cell +LOOP ;
    
: .except ( -- ) dumped $B cells + @ 0< ?EXIT
    base push hex  cr .header cr .regs cr .flags .ip cr
    ['] .stack catch drop cr
    ['] .rstack catch drop
    backtrace $18 cells + off ;

\ backtrace dump                                       09oct95py

| : in_voc?  ( thread addr -- nfa / false )
    >r  BEGIN  @ dup 0= IF  rdrop  EXIT  THEN
               r@ over cell+ name> dup 2- wx@ abs over +
               within  UNTIL  cell+ rdrop ;
: in_which? ( addr -- len nfa count/false ) cell swap dup @
  [ ' push 5 + @ ] ALiteral =
  IF  swap 3 * swap cell+ 2@ swap
      0 <# #S drop bl hold  #S
      s" push " 1-  FOR  dup I + c@ hold  NEXT  drop  #>  EXIT THEN
  @ context @ over noop in_voc?
  dup IF nip count $1F and  EXIT  THEN  drop voc-link
  BEGIN  @ dup  WHILE  2dup 8 - swap in_voc? dup
         IF  -rot 2drop count $1F and  EXIT  THEN
         drop  REPEAT  nip ;

| : "name ( nfa count / 0 -- )  ?dup
    IF    $add  S"  " $add
    ELSE  0 <# bl hold #S '$ hold #> $add  THEN ;
: "back ( addr -- addr len )  base push hex  dup off $sum !
  S" Level: " $add  backtrace 8 cells bounds
  ?DO  I in_which? dup 0= IF  I @ swap  THEN  "name
  +LOOP  $sum @ count ;

: .back relinfo $100 - "back type
    backtrace $18 cells + off ;

also dos
: (file-error ( string -- )
    loaderr @ isfile@ and IF
	isfile@ filename >len type
	':' emit scr @ 0 .r ':' emit r# @ 0 .r ':' emit
    THEN
    loaderr off
    (error cr .back .except dumped $B cells + on ;
previous

' (file-error errorhandler !