\ xchar reference implementation: UTF-8 (and ISO-LATIN-1)

\ environmental dependency: characters are stored as bytes
\ environmental dependency: lower case words accepted

base @ hex

80 Value maxascii

: xc-size ( xc -- n )
    dup      maxascii u< IF  drop 1  EXIT  THEN \ special case ASCII
    $800  2 >r
    BEGIN  2dup u>=  WHILE  5 lshift r> 1+ >r  dup 0= UNTIL  THEN
    2drop r> ;

: xc@+ ( xcaddr -- xcaddr' u )
    count  dup maxascii u< IF  EXIT  THEN  \ special case ASCII
    7F and  40 >r
    BEGIN   dup r@ and  WHILE  r@ xor
	    6 lshift r> 5 lshift >r >r count
\	    dup C0 and 80 <> abort" malformed character"
	    3F and r> or
    REPEAT  r> drop ;

: xc!+ ( xc xcaddr -- xcaddr' )
    over maxascii u< IF  tuck c! char+  EXIT  THEN \ special case ASCII
    >r 0 swap  3F
    BEGIN  2dup u>  WHILE
	    2/ >r  dup 3F and 80 or swap 6 rshift r>
    REPEAT  7F xor 2* or  r>
    BEGIN   over 80 u< 0= WHILE  tuck c! char+  REPEAT  nip ;

: xc!+? ( xc xcaddr u -- xcaddr' u' flag )
    >r over xc-size r@ over u< IF ( xc xc-addr1 len r: u1 )
	\ not enough space
	drop nip r> false
    ELSE
	>r xc!+ r> r> swap - true
    THEN ;

\ scan to next/previous character

: xchar+ ( xcaddr -- xcaddr' )  xc@+ drop ;
: xchar- ( xcaddr -- xcaddr' )
    BEGIN  1 chars - dup c@ C0 and maxascii <>  UNTIL ;

: xstring+ ( xcaddr u -- xcaddr u' )
    over + xchar+ over - ;
: xstring- ( xcaddr u -- xcaddr u' )
    over + xchar- over - ;

: +xstring ( xc-addr1 u1 -- xc-addr2 u2 )
    over dup xchar+ swap - /string ;
: -xstring ( xc-addr1 u1 -- xc-addr2 u2 )
    over dup xchar- swap - /string ;

\ skip trailing garbage

: x-size ( xcaddr -- u )
    \ length of UTF-8 char starting at u8-addr (accesses only u8-addr)
    c@
    dup $80 u< IF drop 1 exit THEN
    dup $c0 u< IF drop 1 EXIT THEN \ really is a malformed character
    dup $e0 u< IF drop 2 exit THEN
    dup $f0 u< IF drop 3 exit THEN
    dup $f8 u< IF drop 4 exit THEN
    dup $fc u< IF drop 5 exit THEN
    dup $fe u< IF drop 6 exit THEN
    drop 1 ; \ also malformed character

: -trailing-garbage ( xcaddr u1 -- xcaddr u2 )
    2dup + dup xchar- ( addr u1 end1 end2 )
    2dup dup x-size + = IF \ last character ok
	2drop
    ELSE
	nip nip over -
    THEN ;

\ utf key and emit

: xkey ( -- xc )
    key dup maxascii u< IF  EXIT  THEN  \ special case ASCII
    7F and  40 >r
    BEGIN  dup r@ and  WHILE  r@ xor
	    6 lshift r> 5 lshift >r >r key
\	    dup C0 and 80 <> abort" malformed character"
	    3F and r> or
    REPEAT  r> drop ;

: xemit ( xc -- )
    dup maxascii u< IF  emit  EXIT  THEN \ special case ASCII
    0 swap  3F
    BEGIN  2dup u>  WHILE
	    2/ >r  dup 3F and 80 or swap 6 rshift r>
    REPEAT  7F xor 2* or
    BEGIN   dup 80 u< 0= WHILE emit  REPEAT  drop ;

\ utf size

\ uses wcwidth ( xc -- n )

: wc, ( n low high -- )  1+ , , , ;

Create wc-table \ derived from wcwidth source code, for UCS32
0 0300 0357 wc,
0 035D 036F wc,
0 0483 0486 wc,
0 0488 0489 wc,
0 0591 05A1 wc,
0 05A3 05B9 wc,
0 05BB 05BD wc,
0 05BF 05BF wc,
0 05C1 05C2 wc,
0 05C4 05C4 wc,
0 0600 0603 wc,
0 0610 0615 wc,
0 064B 0658 wc,
0 0670 0670 wc,
0 06D6 06E4 wc,
0 06E7 06E8 wc,
0 06EA 06ED wc,
0 070F 070F wc,
0 0711 0711 wc,
0 0730 074A wc,
0 07A6 07B0 wc,
0 0901 0902 wc,
0 093C 093C wc,
0 0941 0948 wc,
0 094D 094D wc,
0 0951 0954 wc,
0 0962 0963 wc,
0 0981 0981 wc,
0 09BC 09BC wc,
0 09C1 09C4 wc,
0 09CD 09CD wc,
0 09E2 09E3 wc,
0 0A01 0A02 wc,
0 0A3C 0A3C wc,
0 0A41 0A42 wc,
0 0A47 0A48 wc,
0 0A4B 0A4D wc,
0 0A70 0A71 wc,
0 0A81 0A82 wc,
0 0ABC 0ABC wc,
0 0AC1 0AC5 wc,
0 0AC7 0AC8 wc,
0 0ACD 0ACD wc,
0 0AE2 0AE3 wc,
0 0B01 0B01 wc,
0 0B3C 0B3C wc,
0 0B3F 0B3F wc,
0 0B41 0B43 wc,
0 0B4D 0B4D wc,
0 0B56 0B56 wc,
0 0B82 0B82 wc,
0 0BC0 0BC0 wc,
0 0BCD 0BCD wc,
0 0C3E 0C40 wc,
0 0C46 0C48 wc,
0 0C4A 0C4D wc,
0 0C55 0C56 wc,
0 0CBC 0CBC wc,
0 0CBF 0CBF wc,
0 0CC6 0CC6 wc,
0 0CCC 0CCD wc,
0 0D41 0D43 wc,
0 0D4D 0D4D wc,
0 0DCA 0DCA wc,
0 0DD2 0DD4 wc,
0 0DD6 0DD6 wc,
0 0E31 0E31 wc,
0 0E34 0E3A wc,
0 0E47 0E4E wc,
0 0EB1 0EB1 wc,
0 0EB4 0EB9 wc,
0 0EBB 0EBC wc,
0 0EC8 0ECD wc,
0 0F18 0F19 wc,
0 0F35 0F35 wc,
0 0F37 0F37 wc,
0 0F39 0F39 wc,
0 0F71 0F7E wc,
0 0F80 0F84 wc,
0 0F86 0F87 wc,
0 0F90 0F97 wc,
0 0F99 0FBC wc,
0 0FC6 0FC6 wc,
0 102D 1030 wc,
0 1032 1032 wc,
0 1036 1037 wc,
0 1039 1039 wc,
0 1058 1059 wc,
1 0000 1100 wc,
2 1100 115f wc,
0 1160 11FF wc,
0 1712 1714 wc,
0 1732 1734 wc,
0 1752 1753 wc,
0 1772 1773 wc,
0 17B4 17B5 wc,
0 17B7 17BD wc,
0 17C6 17C6 wc,
0 17C9 17D3 wc,
0 17DD 17DD wc,
0 180B 180D wc,
0 18A9 18A9 wc,
0 1920 1922 wc,
0 1927 1928 wc,
0 1932 1932 wc,
0 1939 193B wc,
0 200B 200F wc,
0 202A 202E wc,
0 2060 2063 wc,
0 206A 206F wc,
0 20D0 20EA wc,
2 2329 232A wc,
0 302A 302F wc,
2 2E80 303E wc,
0 3099 309A wc,
2 3040 A4CF wc,
2 AC00 D7A3 wc,
2 F900 FAFF wc,
0 FB1E FB1E wc,
0 FE00 FE0F wc,
0 FE20 FE23 wc,
2 FE30 FE6F wc,
0 FEFF FEFF wc,
2 FF00 FF60 wc,
2 FFE0 FFE6 wc,
0 FFF9 FFFB wc,
0 1D167 1D169 wc,
0 1D173 1D182 wc,
0 1D185 1D18B wc,
0 1D1AA 1D1AD wc,
2 20000 2FFFD wc,
2 30000 3FFFD wc,
0 E0001 E0001 wc,
0 E0020 E007F wc,
0 E0100 E01EF wc,
here wc-table - Constant #wc-table

\ inefficient table walk:

: wcwidth ( xc -- n )
    wc-table #wc-table over + swap ?DO
	dup I 2@ within IF  I 2 cells + @  UNLOOP EXIT  THEN
    3 cells +LOOP  1 ;

: x-width ( xcaddr u -- n )
    0 rot rot over + swap ?DO
	I xc@+ swap >r wcwidth +
    r> I - +LOOP ;

: char  ( "name" -- xc )  bl word count drop xc@+ nip ;
: [char] ( "name" -- rt:xc )  char postpone Literal ; immediate

\ switching encoding is only recommended at startup
\ only two encodings are supported: UTF-8 and ISO-LATIN-1

80 Constant utf-8
100 Constant iso-latin-1

: set-encoding  to maxascii ;
: get-encoding  maxascii ;

base !

[defined] char@ [IF]
    ' xc@+ IS char@
[THEN]