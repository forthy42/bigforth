\ vt100 key interpreter                                30jun98py

Create translate $100 allot
translate $100 erase
Create transcode $100 allot
transcode $100 erase

Variable fcode

: trans  ( char index -- ) translate + c! ;
: tcode  ( char index -- ) transcode + c! ;

: vt100-decode ( max span addr pos1 -- max span addr pos2 flag )
    key dup '[' = IF  drop 0  base @ >r decimal
        BEGIN  key dup digit?  WHILE  nip swap &10 * +  REPEAT
        r> base !
        dup '~' =  IF  drop transcode  ELSE  nip translate  THEN
        over fcode ! + c@ dup  IF  decode  THEN
    ELSE 'O' = IF
	    key 'P' - &11 +
	    transcode over fcode ! + c@ dup IF  decode  THEN
	ELSE  false  THEN  THEN ;

ctrl B 'D trans
ctrl F 'C trans
ctrl P 'A trans
ctrl N 'B trans

ctrl A 1 tcode
ctrl D 3 tcode
ctrl E 4 tcode

' vt100-decode  ctrlkeys $1B cells + !

Table: (Ftast
  .s    order words file? files pwd
  path  free? flush noop  decimal hex [

| : .names $30 bounds DO  i @ >name count $1F and under type
                          8 swap - spaces  cell +LOOP ;

: help ( -- ) FORTHstart 2+
  5 0 DO cr count 2dup cols over - 2/ spaces type + LOOP drop
  cr ." Function keys:" cr  (Ftast .names  cr
  ." State: " state @ IF ." compile" ELSE ." interpret" THEN cr
  ." Order: " order cr ." Datei: " file? cr ." Stack: " .s cr
  ." leave the system with BYE" cr ;
| : <help>  >r >r >r >r help r> r> r> r> 0 ;

| : <func>  ( m s addr pos -- m s addr pos )  clrline
    fcode @ dup &17 &22 within
    IF    1-   ELSE  dup &23 &25 within 2* + THEN  &11 -
    dup 0 &13 within 0=  IF  drop false  EXIT  THEN
    cells (Ftast +
    -rot >r >r -rot >r >r perform r> r> r> r>
    prompt cr over 3 pick type row over at 0 ;

: F' ( n -- ) ' swap 1- 0 max &19 min cells (Ftast + ! ;

' <func>    ctrlkeys $1C cells + !
\ ' <help>    ctrlkeys $1C cells + !

$1C &11 tcode
$1C &12 tcode
$1C &13 tcode
$1C &14 tcode
$1C &15 tcode
$1C &17 tcode
$1C &18 tcode
$1C &19 tcode
$1C &20 tcode
$1C &21 tcode
$1C &23 tcode
$1C &24 tcode

\ expand tabs                                          18oct94py

Create prefix-found  0 , 0 ,

| : word-lex ( nfa1 nfa2 -- -1/0/1 )
    dup 0=  IF  2drop 1  EXIT  THEN
    cell+ >r cell+ count $1F and
    dup r@ c@ $1F and =
    IF  r> char+ capscomp 0<=  EXIT  THEN
    nip r> c@ $1F and < ;

\ expand tabs                                          31oct94py

: search-prefix  ( addr len1 -- suffix len2 )  0 >r  context
  BEGIN  BEGIN  dup @ over  cell- @ =  WHILE  cell-  REPEAT
         dup >r -rot r> @ @
         BEGIN  dup  WHILE  >r dup r@ cell+ c@ $1F and <=
                IF  2dup r@ cell+ char+ capscomp  0=
                    IF  r> dup r@ word-lex
                        IF  dup prefix-found @ word-lex
                            0>= IF  rdrop dup >r  THEN
                        THEN >r
                    THEN
                THEN  r> @
         REPEAT  drop rot cell-  dup vp u> 0=
  UNTIL  drop r> dup prefix-found ! ?dup
  IF    cell+ count $1F and rot /string rot drop
  ELSE  2drop s" "  THEN
  dup prefix-found @ 0<> - prefix-found cell+ ! ;

\ expand tabs                                          18oct94py

: kill-expand ( max span addr pos1 -- max span addr pos2 )
    prefix-found cell+ @  0 ?DO  <bs> drop  LOOP ;

: tib-full? ( max span addr pos addr' len' --
              max span addr pos addr1 u flag )
    5 pick over 4 pick + prefix-found @ 0<> - < ;

: extract-word ( addr len -- addr' len' )  dup >r
    BEGIN  1- dup 0>=  WHILE  2dup + c@ bl =  UNTIL  THEN  1+
    tuck + r> rot - ;

: <tab> ( max span addr pos1 -- max span addr pos2 0 )
    kill-expand  2dup extract-word  dup 0= IF  nip  EXIT  THEN
    search-prefix  tib-full?
    IF    7 emit  2drop  0 0 prefix-found 2!
    ELSE  bounds ?DO  I c@ tolower <ins>  LOOP  THEN
    prefix-found @ IF  bl <ins>  THEN  0 ;

| : kill-prefix  ( key -- key )
    dup $FF and #tab <> IF  0 0 prefix-found 2!  THEN ;

: bindkey ( xt key -- )  cells ctrlkeys + ! ;

' kill-prefix IS everychar
' <tab> #tab bindkey

\ history support                                      16oct94py

also dos

0 Value history \ history file fid

2Variable forward^
2Variable backward^

: force-open ( addr len -- fid )
    2dup r/w open-file
    IF
        drop r/w create-file throw
    ELSE
        nip nip
    THEN ;

[IFDEF] unix
: history-file ( -- addr u )
    s" BIGFORTHHIST" env$ 2dup d0= IF  2drop
        up@ udp @ + >r
        s" HOME" env$ r@ place
        s" /.bigforth-history" r@ +place
        r> count
    THEN ;
[ELSE]
: history-file ( -- addr u )
    s" BIGFORTHHIST" env$ 2dup d0= IF  2drop
        up@ udp @ + >r
        s" TEMP" env$ ?dup 0= IF  drop s" C:" THEN  r@ place
        s" \bigforth.his" r@ +place
        r> count
    THEN ;
[THEN]

: get-history ( addr len -- )
  force-open to history
  history file-size throw
  2dup forward^ 2! backward^ 2! ;

: ?history ( -- history )
  history 0= IF  history-file get-history  THEN
  history ;

\ moving in history file                               16oct94py

: backspaces 0 ?DO del LOOP ;
: clear-line ( max span addr pos1 -- max addr )
  backspaces over spaces swap backspaces ;

: clear-tib ( max span addr pos -- max 0 addr 0 false )
  clear-line 0 tuck dup ;

: hist-pos    ( -- ud )  ?history file-position throw ;
: hist-setpos ( ud -- )  ?history reposition-file throw ;

: get-line ( addr len -- len' flag )
  swap ?history read-line throw ;

: next-line  ( max span addr pos1 -- max span addr pos2 false )
  clear-line
  forward^ 2@ 2dup hist-setpos backward^ 2!
  2dup get-line drop
  hist-pos  forward^ 2!
  tuck 2dup type 0 ;

: find-prev-line ( max addr -- max span addr pos2 )
  backward^ 2@ forward^ 2!
  over 2 + negate s>d backward^ 2@ d+ 0. dmax 2dup hist-setpos
  BEGIN
      backward^ 2!   2dup get-line  WHILE
      hist-pos 2dup forward^ 2@ d<  WHILE
      rot drop
  REPEAT  2drop  THEN  tuck ;

: prev-line  ( max span addr pos1 -- max span addr pos2 false )
    clear-line find-prev-line 2dup type 0 ;

Create lfpad #lf c,

: write-history ( addr u -- ) >r >r
    ?history file-size throw hist-setpos
    r> r> history write-line drop ( throw )
    hist-pos backward^ 2!
    history flush-file drop ( throw ) ;

: (enter)  ( max span addr pos1 -- max span addr pos2 true )
    >r 2dup swap write-history r> true ;

' next-line  ctrl N bindkey
' prev-line  ctrl P bindkey
' (enter)    #lf    bindkey
' (enter)    #cr    bindkey

previous
