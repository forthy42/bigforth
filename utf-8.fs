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

' xc@+ IS char@

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

\ pictured output

| Create holdbuf 8 allot
: xhold ( xc -- )  holdbuf xc!+
    BEGIN  1- dup holdbuf u>=  WHILE  dup c@ hold  REPEAT  drop ;

\ scan to next/previous character

: xchar+ ( xcaddr -- xcaddr' )  xc@+ drop ;
: xchar- ( xcaddr -- xcaddr' )
    BEGIN  1- dup c@ $C0 and maxascii <>  UNTIL ;

: +x/string ( xcaddr u -- xcaddr' u' )
    over + >r xchar+ r> over - ;
: x/string- ( xcaddr u -- xcaddr' u' )
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
: wc,3 ( n low high -- )  1+ , , , ;
base @ hex

Create wc-table \ derived from wcwidth source code, for UCS32
0 0300 0357 wc,3
0 035D 036F wc,3
0 0483 0486 wc,3
0 0488 0489 wc,3
0 0591 05A1 wc,3
0 05A3 05B9 wc,3
0 05BB 05BD wc,3
0 05BF 05BF wc,3
0 05C1 05C2 wc,3
0 05C4 05C4 wc,3
0 0600 0603 wc,3
0 0610 0615 wc,3
0 064B 0658 wc,3
0 0670 0670 wc,3
0 06D6 06E4 wc,3
0 06E7 06E8 wc,3
0 06EA 06ED wc,3
0 070F 070F wc,3
0 0711 0711 wc,3
0 0730 074A wc,3
0 07A6 07B0 wc,3
0 0901 0902 wc,3
0 093C 093C wc,3
0 0941 0948 wc,3
0 094D 094D wc,3
0 0951 0954 wc,3
0 0962 0963 wc,3
0 0981 0981 wc,3
0 09BC 09BC wc,3
0 09C1 09C4 wc,3
0 09CD 09CD wc,3
0 09E2 09E3 wc,3
0 0A01 0A02 wc,3
0 0A3C 0A3C wc,3
0 0A41 0A42 wc,3
0 0A47 0A48 wc,3
0 0A4B 0A4D wc,3
0 0A70 0A71 wc,3
0 0A81 0A82 wc,3
0 0ABC 0ABC wc,3
0 0AC1 0AC5 wc,3
0 0AC7 0AC8 wc,3
0 0ACD 0ACD wc,3
0 0AE2 0AE3 wc,3
0 0B01 0B01 wc,3
0 0B3C 0B3C wc,3
0 0B3F 0B3F wc,3
0 0B41 0B43 wc,3
0 0B4D 0B4D wc,3
0 0B56 0B56 wc,3
0 0B82 0B82 wc,3
0 0BC0 0BC0 wc,3
0 0BCD 0BCD wc,3
0 0C3E 0C40 wc,3
0 0C46 0C48 wc,3
0 0C4A 0C4D wc,3
0 0C55 0C56 wc,3
0 0CBC 0CBC wc,3
0 0CBF 0CBF wc,3
0 0CC6 0CC6 wc,3
0 0CCC 0CCD wc,3
0 0D41 0D43 wc,3
0 0D4D 0D4D wc,3
0 0DCA 0DCA wc,3
0 0DD2 0DD4 wc,3
0 0DD6 0DD6 wc,3
0 0E31 0E31 wc,3
0 0E34 0E3A wc,3
0 0E47 0E4E wc,3
0 0EB1 0EB1 wc,3
0 0EB4 0EB9 wc,3
0 0EBB 0EBC wc,3
0 0EC8 0ECD wc,3
0 0F18 0F19 wc,3
0 0F35 0F35 wc,3
0 0F37 0F37 wc,3
0 0F39 0F39 wc,3
0 0F71 0F7E wc,3
0 0F80 0F84 wc,3
0 0F86 0F87 wc,3
0 0F90 0F97 wc,3
0 0F99 0FBC wc,3
0 0FC6 0FC6 wc,3
0 102D 1030 wc,3
0 1032 1032 wc,3
0 1036 1037 wc,3
0 1039 1039 wc,3
0 1058 1059 wc,3
1 0000 1100 wc,3
2 1100 115f wc,3
0 1160 11FF wc,3
0 1712 1714 wc,3
0 1732 1734 wc,3
0 1752 1753 wc,3
0 1772 1773 wc,3
0 17B4 17B5 wc,3
0 17B7 17BD wc,3
0 17C6 17C6 wc,3
0 17C9 17D3 wc,3
0 17DD 17DD wc,3
0 180B 180D wc,3
0 18A9 18A9 wc,3
0 1920 1922 wc,3
0 1927 1928 wc,3
0 1932 1932 wc,3
0 1939 193B wc,3
0 200B 200F wc,3
0 202A 202E wc,3
0 2060 2063 wc,3
0 206A 206F wc,3
0 20D0 20EA wc,3
2 2329 232A wc,3
0 302A 302F wc,3
2 2E80 303E wc,3
0 3099 309A wc,3
2 3040 A4CF wc,3
2 AC00 D7A3 wc,3
2 F900 FAFF wc,3
0 FB1E FB1E wc,3
0 FE00 FE0F wc,3
0 FE20 FE23 wc,3
2 FE30 FE6F wc,3
0 FEFF FEFF wc,3
2 FF00 FF60 wc,3
2 FFE0 FFE6 wc,3
0 FFF9 FFFB wc,3
0 1D167 1D169 wc,3
0 1D173 1D182 wc,3
0 1D185 1D18B wc,3
0 1D1AA 1D1AD wc,3
2 20000 2FFFD wc,3
2 30000 3FFFD wc,3
0 E0001 E0001 wc,3
0 E0020 E007F wc,3
0 E0100 E01EF wc,3
here wc-table - Constant #wc-table

base !

\ inefficient table walk:

: wcwidth ( xc -- n )
    wc-table #wc-table over + swap ?DO
        dup I 2@ within IF drop  I 2 cells + @  UNLOOP EXIT  THEN
    3 cells +LOOP  drop 1 ;
[THEN]

also dos

: x-width ( addr u -- n )
    0 -rot bounds ?DO
        I xc@+ swap >r wcwidth +
        r> I - +LOOP ;

previous

\ input editor

variable curpos

: cursor@ ( -- n )  at? swap form nip * + ;
: cursor! ( n -- )  form nip /mod swap at ;
: cur-correct  ( addr u -- )
    curpos @ -1 = IF  2drop  EXIT  THEN
    x-width curpos @ + cursor@ -
    form nip >r  r@ 2/ + r@ / r> * negate curpos +! ;

: save-cursor ( -- )  key? IF  -1  ELSE  cursor@  THEN  curpos ! ;
: restore-cursor ( -- )  curpos @ -1 = ?EXIT  curpos @ cursor! ;
: .rest ( addr pos1 -- addr pos1 )
    key? ?EXIT
    restore-cursor 2dup type 2dup cur-correct ;
: .all ( span addr pos1 -- span addr pos1 )
    key? ?EXIT
    restore-cursor >r 2dup swap type 2dup swap cur-correct r> ;

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
    >r 2dup swap write-history r>
    key? IF  >r 2dup swap type r>  ELSE  .all  THEN
    space true ;

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
  +x/string x/string- save-cursor restore-cursor
  xkey xemit x-width xdecode xaccept xhold ;

also DOS
: utf-8-coding  $80 to maxascii
[ [IFUNDEF] win32 ]
    s" LC_ALL" env$ 2dup d0= IF  2drop
        s" LC_CTYPE" env$ 2dup d0= IF  2drop
            s" LANG" env$ 2dup d0= IF  2drop
                $100 to maxascii  EXIT  THEN THEN THEN
    s" UTF-8" search nip nip 0= IF  $100 to maxascii  THEN
[ [THEN] ] ;

cold:  utf-8-io  utf-8-coding ;

previous

utf-8-io utf-8-coding

module;
