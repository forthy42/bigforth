\ UTF-8 handling                                       12dec04py

\ short: u8 means utf-8 encoded address

module utf-8

$80 Value maxascii

: xc-size ( xc -- n )
    dup      maxascii u< IF  drop 1  EXIT  THEN \ special case ASCII
    $800  2 >r
    BEGIN  2dup u>=  WHILE  5 lshift r> 1+ >r  dup 0= UNTIL  THEN
    2drop r> ;

: xc@+ ( xcaddr -- xcaddr' xc )
    count  dup maxascii u< ?EXIT  \ special case ASCII
    dup $C2 u< ?EXIT \ malformed UTF-8
    \ dup $C2 u< abort" malformed character"
    $7F and  $40 >r
    BEGIN   dup r@ and  WHILE  r@ xor
            6 lshift r> 5 lshift >r >r count
\           dup $C0 and $80 <> abort" malformed character"
            $3F and r> or
    REPEAT  rdrop ;

: xc!+ ( xc xcaddr -- xcaddr' )
    over maxascii u< IF  tuck c! 1+  EXIT  THEN \ special case ASCII
    >r 0 swap  $3F
    BEGIN  2dup u>  WHILE
            2/ >r  dup $3F and $80 or swap 6 rshift r>
    REPEAT  $7F xor 2* or  r>
    BEGIN   over $80 u>= WHILE  tuck c! 1+  REPEAT  nip ;

: xc!+? ( xc xcaddr u -- xcaddr' u' )
    >r over xc-size r@ over u< IF ( xc xc-addr1 len r: u1 )
        \ not enough space
        drop nip r> false
    ELSE
        >r xc!+ r> r> swap - true
    THEN ;

\ scan to next/previous character

: xchar+ ( xcaddr -- xcaddr' )  xc@+ drop ;
: xchar- ( xcaddr -- xcaddr' )
    BEGIN  1- dup c@ $C0 and maxascii <>  UNTIL ;

: +x/string ( xcaddr u -- xcaddr' u' )
    over + xchar+ over - ;
: -x/string ( xcaddr u -- xcaddr' u' )
    over + xchar- over - ;

\ utf key and emit

: xkey ( -- u )
    PCkey dup maxascii u< ?EXIT  \ special case ASCII
    $7F and  $40 >r
    BEGIN  dup r@ and  WHILE  r@ xor
            6 lshift r> 5 lshift >r >r PCkey
\           dup $C0 and $80 <> abort" malformed character"
            $3F and r> or
    REPEAT  rdrop ;

: xemit ( u -- )
    dup maxascii u< IF  PCemit  EXIT  THEN \ special case ASCII
    0 swap  $3F
    BEGIN  2dup u>  WHILE
            2/ >r  dup $3F and $80 or swap 6 rshift r>
    REPEAT  $7F xor 2* or
    BEGIN   dup $80 u>= WHILE  PCemit  REPEAT  drop ;

\ utf size

[IFUNDEF] wcwidth
   : wcwidth ( xc -- n )  drop 1 ;
[THEN]

also dos

: xc-display-width ( addr u -- n )
    0 -rot bounds ?DO
        I xc@+ swap >r wcwidth +
        r> I - +LOOP ;

previous

\ input editor

2variable 'cursave

: save-cursor ( -- )  at? 'cursave 2! ( 27 emit '7 emit ) ;
: restore-cursor ( -- )  'cursave 2@ at ( 27 emit '8 emit ) ;
: .rest ( addr pos1 -- addr pos1 )
    restore-cursor 2dup type ;
: .all ( span addr pos1 -- span addr pos1 )
    restore-cursor >r 2dup swap type r> ;

: >string  ( span addr pos1 -- span addr pos1 addr2 len )
    over 3 pick 2 pick chars /string ;
: <xcins>  ( max span addr pos1 xcchar -- max span addr pos2 )
    >r  2over r@ xc-size + u< IF  rdrop bell  EXIT  THEN
    >string over r@ xc-size + swap move 2dup chars + r@ swap xc!+ drop
    r> xc-size >r  rot r@ chars + -rot r> chars + ;
: (xcins)  ( max span addr pos1 xcchar -- max span addr pos2 )
    <xcins> .all .rest ;
: xcback  ( max span addr pos1 -- max span addr pos2 f )
    dup  IF  over + xchar- over -  0 max .all .rest
    THEN 0 ;
: xcforw  ( max span addr pos1 -- max span addr pos2 f )
    2 pick over <> IF  over + xc@+ emit over -  THEN 0 ;
: (xcdel)  ( max span addr pos1 -- max span addr pos2 )
    over + dup xchar- tuck - >r over -
    >string over r@ + -rot move
    rot r> - -rot ;
: ?xcdel ( max span addr pos1 -- max span addr pos2 0 )
  dup  IF  over2 >r (xcdel) .all over2 r> swap - spaces .rest  THEN  0 ;
: <xcdel> ( max span addr pos1 -- max span addr pos2 0 )
  2 pick over <>
    IF  xcforw drop ?xcdel EXIT  THEN 0 ;
\ : xceof  2 pick over or 0=  IF  bye  ELSE  <xcdel>  THEN ;

: xcfirst-pos  ( max span addr pos1 -- max span addr 0 0 )
  drop 0 .all .rest 0 ;
: xcend-pos  ( max span addr pos1 -- max span addr span 0 )
  drop over .all 0 ;


: xcclear-line ( max span addr pos1 -- max addr )
    drop restore-cursor swap spaces restore-cursor ;
: xcclear-tib ( max span addr pos -- max 0 addr 0 false )
    xcclear-line 0 tuck dup ;

: (xcenter)  ( max span addr pos1 -- max span addr pos2 true )
    >r 2dup swap write-history r> .all space true ;

: xckill-expand ( max span addr pos1 -- max span addr pos2 )
    prefix-found cell+ @ ?dup IF  >r
        r@ - >string over r@ + -rot move
        rot r@ - -rot .all r> spaces .rest THEN ;

: xctab-expand ( max span addr pos1 -- max span addr pos2 0 )
    key? IF  #tab (xcins) 0  EXIT  THEN
    xckill-expand 2dup extract-word dup 0= IF  nip EXIT  THEN
    search-prefix  tib-full?
    IF    7 emit  2drop  0 0 prefix-found 2!
    ELSE  dup >r
        2>r >string r@ + 2r> 2swap insert
        r@ + rot r> + -rot
    THEN
    prefix-found @ IF  bl (xcins)  THEN  0 ;

\ toplevel

: xdecode ( max span addr pos1 key -- max span addr pos2 flag )  everychar
  everychar  ctype?
  IF    (xcins) false
  ELSE  cells ctrlkeys + perform  THEN ;

: xaccept   ( addr len -- len )
    save-cursor
    dup 0< IF abs over dup 1- c@ under type ELSE 0 THEN rot over
    BEGIN  key decode  UNTIL
    .all 2drop nip ;

: utf-8-io ( -- )  $80 to maxascii
    ['] xcforw       ctrl F bindkey
    ['] xcback       ctrl B bindkey
    ['] ?xcdel       ctrl H bindkey
    ['] <xcdel>      ctrl D bindkey
    ['] <xcdel>      ctrl X bindkey
    ['] xcclear-tib  ctrl K bindkey
    ['] xcfirst-pos  ctrl A bindkey
    ['] xcend-pos    ctrl E bindkey
    ['] (xcenter)    #lf    bindkey
    ['] (xcenter)    #cr    bindkey
    ['] xctab-expand #tab   bindkey

    ['] xkey    & keyboard !
    ['] xdecode & keyboard 2 cells + !
    ['] xaccept & keyboard 3 cells + !
    ['] xemit   & display !
;

' key alias xkey
' emit alias xemit

export utf-8 maxascii xc-size xc@+ xc!+ xc!+? xchar+ xchar-
  +x/string -x/string save-cursor restore-cursor
  xkey xemit xc-display-width xdecode xaccept ;

also DOS
: utf-8-coding
    s" LC_ALL" env$ 2dup d0= IF  2drop
        s" LC_CTYPE" env$ 2dup d0= IF  2drop
            s" LANG" env$ 2dup d0= IF  2drop
                $100 to maxascii  EXIT  THEN THEN THEN
    s" UTF-8" search nip nip 0= IF  $100 to maxascii  THEN ;

cold:  utf-8-io  utf-8-coding ;

previous

utf-8-io utf-8-coding

module;
