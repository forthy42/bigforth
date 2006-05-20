\\                      *** Strings ***

Dieses File enthält einige Grundworte zur Stringverarbeitung,
vor allem ein  SEARCH  für den Editor. Ebenfalls sind 2 Worte
zur Umwandlung von counted Strings (Forth) in 0-terminated
Strings, wie sie z.B. vom Betriebssystem oft benutzt werden,
vorhanden.

Beim SEARCH entscheidet die Variable  CAPS  , ob Groß- und
Kleinschreibung unterschieden wird oder nicht. Ist  CAPS  ON,
so werden große und kleine Buchstaben gefunden, die Suche dau-
ert allerdings länger.

c>0"  wandelt einen String mit führendem Countbyte in einen
mit 0 abgschlossenen, wie er vom Betriebssystem oft gebraucht
wird. 0>c" arbeitet umgekehrt.
\ String Functions Loadscreen                          11feb86we

Module string

User caps       caps off

\ capscomp                                            10mar86we

' capital 5 + @ >Label upper

Code capscomp   ( addr0 len addr1 -- n )   CX pop  DX pop
     CX CX test  0= IF  CX AX mov  Next  THEN
     SI push  DI push  AX DI mov  DX SI mov  AX AX xor
     ?DO  .b SI ) AX movzx  A: upper AX L) AL mov
          .b DI ) DX movzx  SI inc  DI inc
          A: upper DX L) AL sub  LOOPE  THEN
     AL AX movsx  DI pop  SI pop
     Next end-code

\ compare search                                       31dec92py

| Code Cscan    ( addr len chr -- addr1 len1 )  CX pop
       ?DO  drop  DX pop  SI push  DX SI mov
            A: upper AX L) DL mov
            DO  .b SI ) AX movzx  SI inc
                A: upper AX L) DL cmp  LOOPNE
            0= IF  SI dec  CX inc  THEN
            SI SP ) xchg  THEN  CX AX mov
       Next end-code

: compare   ( addr0 u0 addr1 u1 -- n )
  rot 2dup - negate >r min swap
  caps @ IF  capscomp  ELSE  -text  THEN
  dup 0= IF  drop r>  ELSE  rdrop  THEN
  dup 0> IF  drop 1  ELSE  0<  THEN ;

: str= ( addr0 u0 addr1 u1 -- flag )  compare 0= ;
: str< ( addr0 u0 addr1 u1 -- flag )  compare 0< ;
: string-prefix? ( addr0 u0 addr1 u1 -- flag )
  tuck 2>r min 2r> str= ;

: search   ( buf buflen text textlen -- restbuf restlen flag )
  dup 0= IF  2drop true  EXIT  THEN
  2over over2 2dup u< IF  drop 2drop 2drop + 0 false EXIT  THEN
  - 1+ 3 pick c@ >r
  BEGIN  caps @ IF  r@ Cscan  ELSE  r@ scan  THEN  dup
         WHILE  >r >r  2dup r@ over compare
         0= IF  >r drop 2drop r> r> r> rot + 1- rdrop true  EXIT
         THEN
         r> r>  1 /string   REPEAT
  2drop 2drop  rdrop false ;

\ delete insert replace                                29jan86we

: delete   ( buffer size count -- )
  over min >r  r@ - ( left over )  dup 0>
  IF  2dup swap dup  r@ +  -rot swap move  THEN  + r> bl fill ;

: insert   ( string length buffer size -- )
  rot over min >r  r@ - ( left over )
  over dup r@ +  rot move   r> move  ;

: replace   ( string length buffer size -- )
  rot min move ;

: blank  ( addr len -- )  bl fill ;

: +place ( addr u addr' -- )
    2dup >r >r dup c@ char+ + swap move
    r> r> dup c@ rot + swap c! ;

\ 0 terminated String operators, $sum, cpush           29jan86we

AVariable $sum                  \ pointer to stringsum

: $add      ( addr len -- )     dup >r
  $sum @ count +  swap  move   $sum @  dup c@  r> +  swap c! ;

: c>0"   ( addr -- )
  count >r  dup 1-  under  r@ move   r> + 0 swap c!  ;

: 0>c"   ( addr -- )
  dup >r  true false scan nip negate 1-  r@ dup 1+ 2 pick move
  r> c!  ;

: 0place ( addr u addr -- )
  swap 2dup + >r move 0 r> c! ;

: cpush  ( addr len --)   r> -rot
  over swap  rp@ over 1+ -  -4 and dup rp!  place  >r execute
  rp@ cell+ count 2dup r> swap move + 3+ -4 and rp! ; restrict

: ,0"   ( -- )  '" parse  here swap dup allot move 0 c, ;

: (0"   "lit 1+ ;  restrict

: 0"  '" parse  state @
  IF  compile (0"  dup 1+ c,  here swap dup allot move  0 c,  EXIT  THEN
  s^ @ place s^ @ dup c>0" ;  immediate

export caps capscomp compare search delete insert replace blank +place
       $sum $add c>0" 0>c" 0place cpush ,0" 0" (0"
       str= str< string-prefix? ;
Module;
