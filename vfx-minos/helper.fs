\ helper words for VFX Forth
-idata
: | ; \ headerless becomes a noop

\ simple stores

: c@+ count swap ;
: w@+ dup w@ swap 2+ ;
: @+ dup @ swap cell+ ;

: c!+  tuck c! char+ ;
: w!+  tuck w! 2+ ;
: !+  tuck ! cell+ ;

: wextend dup $8000 and negate or ;
: cextend dup $80 and negate or ;

: cx@ c@ cextend ;
: wx@ w@ wextend ;
: wx@+ dup wx@ swap 2+ ;

\ Floating point

: ans-float
    '.' dp-char !
    '.' fp-char ! ;

ans-float

: mpe-float
    ',' dp-char !
    '.' fp-char ! ;

\ memory words

vocabulary memory

also memory definitions
: NewPtr ( len -- addr )  dup cell+ allocate throw !+ ;
: DisposPtr ( addr -- )  ?dup IF cell- free throw THEN ;
: DelFix ( addr root -- ) dup @ 2 pick ! ! ;
: NewFix  ( root len # -- addr )
  BEGIN  2 pick @ ?dup  0= WHILE  2dup * NewPtr
         over 0 ?DO  dup 4 pick DelFix 2 pick +  LOOP  drop
  REPEAT  >r drop r@ @ rot ! r@ swap erase r> ;
Variable Masters
: NewMP ( -- MP ) Masters cell $200 NewFix ;
: NewHandle ( len -- addr )  NewPtr NewMP tuck ! ;
: DisposHandle ( addr -- )  dup @ DisposPtr Masters DelFix ;
: Handle! ( len addr -- )  >r NewPtr r> ! ;
: SetHandle ( addr MP -- )  ! ;
: HandleOff ( addr -- )  dup @ DisposPtr off ;
: Hlock ( addr -- ) drop ;
: Hunlock ( addr -- ) drop ;
: SetHandleSize ( addr size -- ) swap >r
    r@ @ cell- over cell+ resize throw !+ r> ! ;
: GetHandleSize ( addr -- size ) cell- @ ;

previous definitions

synonym AVariable Variable
synonym A, ,
synonym AConstant Constant
synonym ALiteral Literal
synonym Patch Defer
synonym << lshift
synonym >> arshift
synonym toss previous
synonym extend s>d
synonym #! \

: cont >r ;
: push r> swap dup @ >r >r cont r> r> swap ! ;

: & ' >body state @ IF postpone Literal THEN ; immediate

: 0>= 0< 0= ;
: 0<= 0> 0= ;
: u>= u< 0= ;
: u<= u> 0= ;
: @+ dup @ swap cell+ ;
: rdrop postpone r> postpone drop ; immediate
: i' r> 2r> 2dup 2>r rot >r swap - $80000000 xor ;
: +i' negate 2r> rot r> + dup >r -rot 2>r 0< ;
: ith  postpone I postpone cells postpone + postpone @ ; immediate

: list> ( thread -- element )
  BEGIN  @ dup  WHILE  dup r@ execute
  REPEAT  drop rdrop ; DoNotSin \ restrict

-1 cells Constant -cell
: over2  2 pick ;

: tolower ( char -- )  dup 'A' 'Z' 1+ within IF  bl +  THEN ;
\ Structs for interpreter                              28nov92py

Variable (i)

: [DO]  ( n-limit n-index -- ) \ gforth bracket-do
  >in @ -rot
  DO   I (i) ! dup >r >in ! interpret r> swap +LOOP  drop ;
                                                      immediate

: [?DO] ( n-limit n-index -- ) \ gforth bracket-question-do
  2dup = IF 2drop postpone [ELSE] ELSE postpone [DO] THEN ;
                                                      immediate

: [+LOOP] ( n -- ) \ gforth bracket-question-plus-loop
  rdrop rdrop ;                                       immediate

: [LOOP] ( -- ) \ gforth bracket-loop
  1 rdrop rdrop ;                                     immediate

: [FOR] ( n -- ) \ gforth bracket-for
  0 swap postpone [DO] ;                              immediate

: [NEXT] ( n -- ) \ gforth bracket-next
  -1 rdrop rdrop ;                                    immediate

: [I]  (i) @ state @ IF  postpone Literal  THEN ;     immediate

[defined] $linux [IF]
    0 Constant unix
[THEN]

: \needs postpone [defined] IF postpone \ THEN ;

: ,0"   ( -- )  '"' parse  here swap dup allot move 0 c, ;

: onlyforth  only forth ;

: perform @ execute ;

: macro ; \ indicates macro
: hmacro ; \ indicates macro

: forward
    : postpone ahead postpone then s" dummy string" postpone SLiteral
    discard-sinline postpone ; DoNotSin ;

: forward?  ( xt -- flag ) c@ $E9 = ;

: : ( "name" -- ) >in @ >r bl word find IF
	dup forward? IF  1+ :noname over - 4- swap ! rdrop
	    EXIT  THEN THEN  drop
    r> >in ! : ;

: recursive  reveal ; immediate

: Module  >in @ Vocabulary >in !
    get-order get-current swap 1+ set-order
    also ' execute also definitions ;

: Module;  previous previous definitions previous ;

: v! ! ;

\ bit words

: +bit ( addr n -- ) 8 /mod swap >r + 1 r> lshift over c@ or swap c! ;
: -bit ( addr n -- ) 8 /mod swap >r + 1 r> lshift invert over c@ and swap c! ;
: bit@ ( addr n -- ) 8 /mod swap >r + 1 r> lshift swap c@ and 0<> ;

\ Address marker

synonym AValue Value

Vocabulary DOS

also DOS definitions

Extern: char * setlocale ( int , char * ); ( locale addr -- addr )
Extern: void gettimeofday ( int * , int * ); ( timeval timezone -- )

synonym env$ readenv

Create timeval   0 , 0 ,
Create timezone  0 , 0 ,

previous definitions

\ zero string

synonym 0" z"

: >len ( addr -- addr u ) dup zstrlen ;

: 0place ( addr u addr -- )
  swap 2dup + >r move 0 r> c! ;

: "lit r> dup count + >r ; DoNotSin

: -scan ( addr len char -- addr' len' ) >r
    BEGIN  1- dup WHILE  2dup + c@ r@ = UNTIL  THEN  rdrop ;

\ date & time conversion in files

LocalExtern: localtime int localtime ( char * );

Create dta $50 allot [defined] osx [IF] $100 allot [THEN]

: @time   dta [defined] osx [IF] $30 [ELSE] $38 [THEN] + @ ;
: @attr   dta $18 + w@ ;
: @length dta [defined] osx [IF] $40 [ELSE] $24 [THEN] + @ ;
: dtaname  dta @ ;

: >hms  sp@ localtime nip @+ @+ @ swap rot ;
: >ymd  sp@ localtime nip $C + @+ @+ @ ;

: >time  ( time -- addr count )  base push decimal   >hms
    0 <# # # ':' hold drop # # ':' hold drop # # #> ;

: >date ( date -- string len )  base push decimal  >ymd
  0 <#  # # 2drop  >r S" janfebmaraprmayjunjulaugsepoctnovdec"
        r> 0 max #11 min dup dup + + /string 3 min
        over + 1- DO  I c@ hold -1  +LOOP  0 # #  #> ;

\ dictionary listing functions

: /ior ( ret -- ret/-ior ) dup -1 =
    IF drop errno noop @ negate  THEN ;
: glibc ;

LocalExtern: _open int open( int , int , int );
LocalExtern: _close int close( int );
LocalExtern: getdirentries int getdirentries( int , int , int , int );
Variable dent-basep
: getdents  dent-basep  getdirentries
    dup 0= IF  dent-basep off  THEN ;
LocalExtern: lxstat int lxstat( int , int , int );
LocalExtern: xstat int xstat( int , int , int );
: lstat  swap 1 -rot lxstat ;     ( buf name -- r )
: stat   swap 1 -rot xstat ;      ( buf name -- r )
LocalExtern: fnmatch int fnmatch( int , int , int );
LocalExtern: getcwd int getcwd( int , int );
LocalExtern: fdelete int unlink( int );
LocalExtern: dcreate int mkdir( int );

: dgetpath ( buffer drive -- ior )  drop $100 getcwd 0= ;

: ?diskabort throw ;

Variable dirbuf dirbuf off
Variable dirpath
Variable direndp
Create dta $50 allot [defined] osx [IF] $100 allot [THEN]
Create pattern $80 allot
| dta 1 cells + AConstant diroff
| dta 2 cells + AConstant dirsize
| dta 3 cells + AConstant dirfd
: dirstat ( -- 0/ior )  dta @ >len 1+ direndp @ swap move
  dta $10 +  dirpath @  2dup stat
  IF  lstat  ELSE  2drop 0  THEN ;
: ?allot ( n addr -- )  dup @ IF  2drop EXIT  THEN
  [ also Memory ]  Handle! [ previous ] ;

: fsend ( -- )  dirfd @ ?dup IF  _close drop  THEN  dirfd off ;
: fsnext ( -- ior )
  BEGIN  diroff @ dirsize @ =
         IF  diroff off
             dirfd @ dirbuf @ $400 getdents
             dup 0 max dirsize ! /ior dup 0<=
             IF  fsend dup 0= or
                 EXIT  THEN  drop
         THEN  0  diroff @ dirbuf @ +
               [defined] osx [IF] 4 + [ELSE] 8 + [THEN] dup w@ diroff +!
 [defined] glibc [IF] 3 + [ELSE] [defined] osx [IF] 4 + [ELSE] 2 + [THEN] [THEN]
         dup dta !  pattern -rot swap fnmatch 0= UNTIL
  dirstat ;

: fsfirst ( C$ attr -- ior )   drop
  dup dirpath !  diroff off  dirsize off
  $400 dirbuf ?allot
  >len '/' -scan over + dup >r >len 1+ pattern swap move
  '.' r@ c! 0 r@ 1+ c! r> direndp !
  0 0 _open
  dup dirfd ! dup /ior swap -1 = ?EXIT  drop  fsnext ;
   
\ special characters

$08 Constant #bs         $0D Constant #cr
$0A Constant #lf         $1B Constant #esc
$09 Constant #tab        $07 Constant #bell

\ division - make sure everything is floored

: /mod ( n1 n2 -- rem quot ) >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;

: ud/mod ( ud1 u2 -- urem udquot )  >r 0 r@ um/mod r> swap >r
                                    um/mod r> ;
Synonym m/mod fm/mod
: d*     ( ud1 ud2 -- udprod )      >r swap >r 2dup um*
                                    2swap r> * swap r> * + + ;

\ timer handling

1 cells +user time

also DOS
: timer@  timeval timezone gettimeofday
  timeval 2@ swap $CB9CB68 um* nip swap
  $2000000 um* #675 ud/mod drop nip + ;
previous
: !time timer@ time ! ;
: ms>time ( ms -- time )
  $C6D750EB um* $3FFFFFF. d+ 6 lshift swap $1A rshift or ;
: >us ( time -- dus )  #86400000 um*
  #1000 um* >r >r #1000 um* r> + r> rot 0< s>d d- ;
: after ( ms -- time ) ms>time timer@ + ;
: till ( time -- )
    BEGIN dup timer@ - 0> WHILE  pause  REPEAT drop ;
: wait  ( ms -- )    after till ;
\ synonym ms wait
: timeout? ( time -- time f )  pause dup timer@ - 0> 0= ;
Defer idle ' 2drop IS idle

\ clearstack

: clearstack depth ndrop ;

\ constant adders

: 8+ 8 + ;
: 6+ 6 + ;
: 3+ 3 + ;

\ multitasker is needed

\ include /usr/share/doc/VfxForth/Lib/Lin32/MultiLin32.fth

\ synonym wake restart
\ synonym sleep halt

: wake  drop ;
: sleep drop ;
: stop ;

\ keyboard state

Variable kbshift

\ compile only

: restrict ;

: BUT  swap ; immediate

\ loops

: FOR  0 postpone Literal postpone swap postpone DO ; immediate
: NEXT  -1 postpone Literal postpone +LOOP ; immediate

\ digit?

: toupper ( char -- char' )
    dup [char] a - [ char z char a - 1 + ] Literal u<  bl and - ;

: ?lit, ( n -- ) state @ IF postpone Literal THEN ;

: ctrl    ( -- n )  char toupper $40 xor ?lit, ;
                                            immediate

: digit?   ( char -- digit true/ false ) \ gforth
  toupper [char] 0 - dup 9 u> IF
    [ char A char 9 1 + -  ] literal -
    dup 9 u<= IF
      drop false EXIT
    THEN
  THEN
  dup base @ u>= IF
    drop false EXIT
  THEN
  true ;

Create bases  #10 c,  $10 c, %10 c, #10 c, 0 c,
\             10      16     2      10     char
: getbase  ( addr u -- addr' u' )  over c@ '#' - dup 5 u<
  IF  bases + c@ base ! 1 /string  ELSE  drop  THEN ;
: getsign  over c@ '-' = dup >r negate /string r> ;
Defer char@ ' count IS char@
: s>number  ( addr len -- d )  base push
  getsign >r  getbase  base @ 0=
  IF  over + swap char@ >r swap over - dup 0= >r 1 = >r
      c@ ''' = r> and r> or dpl !  r> 0 rdrop  EXIT  THEN
  dpl on  getsign r> xor >r  0 0 2swap
  BEGIN  dup >r >number  dup r> =
         IF  rdrop 2drop dpl off  EXIT  THEN
         dup  WHILE  dup dpl ! over c@ -3 and ',' = 0=
         IF  rdrop 2drop dpl off  EXIT  THEN  1 /string
     dup 0= UNTIL  THEN  2drop r> IF  dnegate  THEN ;

\ case?

: case? ( n1 n2 -- n1 false / true )
    over = dup IF nip THEN ;

\ sorting

: pivot@ ( addr u -- addr u pivot ) 2dup 2/ cells + @ ; macro
Defer lex       ' <= IS lex

: split< ( addr u pivot -- addr' u' addr" u" )
  >r 2dup cells bounds 0 r> 2swap
    ?DO
	nip I swap
	dup I @ lex
	IF  BEGIN  -cell +I' ?LEAVE  I' @ over lex  UNTIL
	    I @ I' @ I ! I' !  THEN
	nip I' swap
    cell +LOOP  drop >r
  r@ 2 pick - cell/ tuck - r> swap ;
: sort ( addr u -- )
    BEGIN  dup 1 >  WHILE  pivot@ split< recurse  REPEAT  2drop ;

\ Argument parsing

0 value script?

Vocabulary -options  also -options definitions

Defer -i
: --include included 2 ; ' --include IS -i
: -e evaluate 2 ;
synonym --evaluate -e

: -h  ( addr u -- n )  2drop 1  ." Image Options:" cr
  ."   FILE                              load FILE" cr
  ."   -e STRING, --evaluate STRING      interpret STRING" cr
  bye ;
synonym --help -h

get-current Constant '-options
Forth definitions -options

Variable arg#
: arg ( n -- addr u ) argv[ >len ;
: do-arg ( addr u addr u -- n )
    2dup '-options search-wordlist
    IF  nip nip execute  ELSE  2swap 2drop -i 1-  THEN ;
: interpret-args ( -- )  argc 1 ?DO  I arg# !
	I 1+ I' = IF  s" "  ELSE  I 1+ arg  THEN
    I arg do-arg  +LOOP ;
previous

: exe ;

\ execute parsing - from Gforth compat/

wordlist constant execute-parsing-wordlist

get-current execute-parsing-wordlist set-current

\ X is prepended to the string, then the string is EVALUATEd
: X ( xt -- )
    previous execute
    source >in ! drop ; immediate \ skip remaining input

set-current

: >order ( wid -- )
  >r get-order 1+ r> swap set-order ;

: execute-parsing ( ... c-addr u xt -- ... )
    >r dup >r
    dup 2 chars + allocate throw >r  \ construct the string to be EVALUATEd
    s" X " r@ swap chars move
    r@ 2 chars + swap chars move
    r> r> 2 + r> rot dup >r rot ( xt c-addr1 u1 r: c-addr1 )
    execute-parsing-wordlist >order  \ make sure the right X is executed
    ['] evaluate catch               \ now EVALUATE the string
    r> free throw throw ;            \ cleanup

\ single quote string

: .' ''' parse postpone SLiteral postpone type ; immediate
: s' ''' parse postpone SLiteral ; immediate
