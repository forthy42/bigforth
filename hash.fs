\                    *** Hash-Table ***                25sep90py

Module Hash  memory also

\ SchlÅssel errechnen                                  01jan93py

&14 Value Hashbits
1 Hashbits << Value Hashlen

Label (hash ( SI:string -- AX:key )  :R  DX push
      .b lods  $1F # AX and  AX CX mov  DX DX xor  CX 1 # shr
      b IF  SI ) AH mov  SI inc  THEN  CX dec
      0>= IF  BEGIN  .w SI ) DX mov  2 # SI add  CX dec
                     DX AX *2 I) AX lea  0< UNTIL  THEN
      & Hashbits A#) CX mov  AX DX mov  AX shr  DX AX add
      & Hashlen  A#) CX mov  CX dec  CX AX and  DX pop  ret
| Code Hash ( string -- key )
       R: SI push  AX SI mov  (hash rel) call  SI pop
       Next end-code

| User Hash#
| : (hash-prehash ( string -- string )
    Defers 'prehash dup Hash Hash# ! ;
' (hash-prehash IS 'prehash

| Code lastlink! ( addr link -- )  .align
       BEGIN  AX ) DX mov  DX DX test  0<> WHILE
              DX AX mov  REPEAT
       DX pop  DX AX ) mov  AX pop  Next end-code

\ NewHash                                              25sep90py

| Variable insRule      insRule on
| Variable revealed

Variable HashPointer
Variable HashTable
Variable HashIndex

cold: insRule on
      HashPointer off  HashTable off  HashIndex off ;

\ hash vocabularies                                    16jul94py

: (reveal ( addr voc -- )  $C + @ dup 0< IF  2drop EXIT  THEN
  over Hash dup Hash# ! xor  cells >r
  HashPointer 8 $400 NewFix
  tuck cell+ ! r> HashTable @ + insRule @
  IF  dup @ 2 pick ! !  ELSE  lastlink!  THEN
  revealed on ;

: addall  voc-link  LIST>  'initvoc ;

: clearhash  ( -- )
    HashTable @ Hashlen cells bounds
    DO  I @
        BEGIN  dup  WHILE
               dup @ swap HashPointer DelFix
        REPEAT  I !
    cell +LOOP  HashIndex off ;

\ Hash-Insert                                          25sep90py

| : hash-alloc ( addr -- addr )  HashTable @ 0= IF
    Hashlen cells HashTable Handle!
    HashTable @ Hashlen cells erase  THEN
    HashIndex @ over !  1 HashIndex +!
    HashIndex @ Hashlen >=
    IF  clearhash
        Hashbits 1+ to Hashbits
        Hashlen 2*  to Hashlen
        HashTable HandleOff
        addall
    THEN ;

: (initvoc  ( addr -- )  \ dup 8 - body> . ." : " ?cr
    cell+ dup @ 0< IF  drop EXIT  THEN
    insRule @ >r  insRule off  hash-alloc
    3 cells - dup
    BEGIN  @ dup  WHILE  2dup cell+ swap (reveal  REPEAT
    2drop  r> insRule ! ;

\ Hash-Find                                            01jan93py

Code hash(find ( string thread -- string false/ NFA true )
     R: 0 # $C AX D) cmp
     ' list(find 0< jmpIF  here 4- dup relon 1+ relon
     BX push  $C AX D) BX mov  lods  SI push  DI push
     ( AX SI mov  AX DX mov ) user' Hash# UP D) DX mov
     BX DX xor  HashTable A#) BX mov
     BX DX *4 I) BX lea  ( DX AX mov )
     AX ) DX movzx  $1F # DL and
     BEGIN  BEGIN  BX ) BX mov  BX BX test
                   0= IF  DI pop  SI pop  BX pop
                          S: AX push  AX AX xor  Next :R  THEN
                   cell BX D) CX mov  CX ) CX movzx
                   $1F # CL and  CX DX cmp  0= UNTIL
            2 AX D) SI lea  4 BX D) DI mov  DI inc  DI inc
            0 # CH mov  CX dec
            repe .b cmps  0= UNTIL  4 BX D) AX mov
     DI pop  SI pop  BX pop  S: AX push  -1 # AX mov
     Next end-code

\ hash-remove                                          21mar92py

| : remove? ( dic symb addr -- dic symb flag ) dup heap?
    IF  over u<  ELSE  2 pick relinfo within  THEN ;

| : hremove ( dic sym thread -- dic sym )
    BEGIN  dup >r @ cell+ @ remove?  
           IF    r@ @ dup @ r@ ! HashPointer DelFix  r>
           ELSE  r> @  THEN
    dup @ 0= UNTIL  drop ;

| : hash-remove ( dic symb -- dic symb )
    defers custom-remove
    HashTable @ HashLen cells bounds
    DO  I @  IF I hremove THEN  cell +LOOP ;

(* .words NewHash                                      26sep90py
memory also Hash also

: .words  ( -- )  base push hex  HashLen 0
  DO  cr  i 4 .r ." : " HashTable @ i cells +
      BEGIN  @ dup  WHILE
             col cols $10 - u> IF  cr 4 spaces  THEN
             dup cell+ @ .name  REPEAT  drop
      stop? ?LEAVE  LOOP ;

: NewHash ( lblen -- )
  1 max dup to Hashbits 1 swap << to HashLen
  clearhash  HashTable HandleOff  addall ;

\ .statistic                                           26sep90py

: .statistic ( -- ) base push decimal pad $100 cells erase
  HashTable @ HashLen cells bounds
  DO  0  i  BEGIN  @ dup  WHILE  >r 1+ r>  REPEAT  drop
      cells pad + dup @ 1+ swap !  cell +LOOP
  pad $100 cells 0 -skip 1- cell/ 1+ 1 max  2dup 0
  DO  cr  i 2 .r ." : "  dup Ith 4 .r  LOOP drop cr
  ."  n: "  2dup  0  -rot cells bounds
  ?DO  i @ +  cell +LOOP  dup >r 4 .r  cr
  ." ‰x: "  nip 0  swap 0
  ?DO  pad Ith I * +  LOOP dup 4 .r  cr
  ." ˇx: "  r@ /mod 4 .r ." ."
  BEGIN  &10 * r@ /mod 0 .r  ?dup 0= UNTIL cr rdrop ;

toss toss \\\ *)
\ Installieren                                         21mar92py

export: exportVoc
  & kernel 3 cells + on
  ['] hash-remove IS custom-remove
  ['] (reveal     IS 'reveal
  ['] (initvoc    IS 'initvoc
  HashIndex @ 0= IF  addall  THEN \ Baum aufbauen
  ['] hash(find   IS ((find ;
\ Baumsuche ist installiert.

toss
Module;
