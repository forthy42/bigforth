\ helper words for VFX Forth

\ =============
\ *! vfxharness
\ *T Portability layer for VFX Forth
\ =============
\ *P The code in *\i{Minos/vfx-minos/VFXharness,fth} contains
\ ** the main Forth host dependencies required to port Minos
\ ** and Theseus to VFX Forth for Linux.

\ *P In order to allow some redefinitions to be sensitive
\ ** to whether the Minos or VFX Forth notation is to be used,
\ ** the flag *\fo{MinosMode?} is provided. If you want to
\ ** revert to the original VFX Forth behaviour, clear this
\ ** flag.

only forth definitions

1 value MinosMode?	\ -- flag
\ *G Some words can operate in Minos mode or in the native host
\ ** mode.


\ *********
\ *S Blocks
\ *********
\ *P The Minos/Theseus editor supports blocks. Block support is
\ ** taken from a library file.

include %Lib%/BLOCKS.FTH


\ *****************
\ *S Floating point
\ *****************

: ans-float	\ --
\ *G Set VFX Floats to ANS mode, in which '.' is both the
\ ** double number separator and the floating point separator.
  '.' dp-char !
  '.' fp-char !  ;

: mpe-float	\ --
\ *G Set VFX Floats to MPE mode, in which ',' is the
\ ** double number separator and '.' is the floating
\ ** point separator.
  ',' dp-char !
  '.' fp-char !  ;

mpe-float
include %lib%/Ndp387.fth
\ *G Compile floating point library.
ans-float

: float ;	\ --
\ *G Indicates that floats have been compiled.
 : fx$		\ F: f -- ; -- caddr len
\ *G Convert a floating point number to ASCII text. This
\ ** still needs furthr processing according to the required
\ ** presentation mode to insert a decimal point and so on.
  pad dup f>ascii ;

Code f>r	\ F: f -- ; R: -- f
\ *G Transfer a float to the return stack.
	pop edx
	add esp, # #-12
	fstp fword 0 [esp]
	push edx
	fnext,
end-code

Code fr>	\ R: f -- ; F: -- f
\ *G Transfer a float from the return stack.
	pop edx
	fld fword 0 [esp]
	add esp, # #12
	push edx
	fnext,
end-code

code fr@	\ R: f -- f ; F: -- f
\ *G Copy a float from the return stack.
  fld   fword 4 [esp]
  fnext,
end-code

: fm*		\ F: f -- f' ; n --
\ *G Multiply a float by an int.
  s>f f*  ;

: fm/		\ F: f -- f' ; n --
\ *G Divide a float by an int.
  s>f f/  ;

: fm**		\ F: f -- f' ; n --
\ *G Raise a float to the power of the int.
  s>f f**  ;

Variable 'f>fs
: f>fs		\ F: f -- ; -- fs
\ *G Convert a native float to its 32 bit form on the data stack.
 'f>fs sf! 'f>fs @ ;
: fs>f		\ fs -- ; F: -- f
\ *G Convert a 32 bit float on the data stack to a native float.
 'f>fs ! 'f>fs sf@ ;

-idata


\ *******************
\ *S Floored division
\ *******************
\ *P Many graphics operations in Minos require floored division.
\ ** Because ANS Forth permits Forth systems to default to either
\ ** symmetric or floored division, a set of operations that
\ ** always use floored division is defined.

: /modf ( n1 n2 -- rem quot )
\ *G Floored version of *\fo{/MOD}.
  >r s>d r> fm/mod ;
: /f ( n1 n2 -- quot )
\ *G Floored version of *\fo{/}.
  /modf nip ;
: modf ( n1 n2 -- rem )
\ *G Floored version of *\fo{MOD}.
  /modf drop ;
: */modf ( a b c -- rem quot )
\ *G Floored version of *\fo{*/MOD}.
  >r m* r> fm/mod ;
: */f ( a b c -- n )
\ *G Floored version of *\fo{*/}.
  */modf nip ;

Synonym m/mod fm/mod  ( d n -- rem quot )
\ *G This name is widely used, so we force it to be floored.


\ *********************
\ *S Miscellaneous math
\ *********************

$7FFFFFFF Constant mi	\ -- n
\ *G Returns MAXINT, the largest positive integer.

Synonym ud/mod mu/mod ( ud1 u2 -- urem udquot )
\ *G Rename to preserve Minos code base.

: d*     ( ud1 ud2 -- udprod )
\ *G Unsignsigned multiply of two doubles to produce a third.
  >r swap >r 2dup um*
  2swap r> * swap r> * + + ;

: 0max  0 max ;
\ *G A micro-optimisation for *\fo{0 MAX}.
: 0min  0 min ;
\ *G A micro-optimisation for *\fo{0 MIN}.
: 8*  ( n -- 8*n ) 3 lshift ;
\ *G A micro-optimisation for *\fo{8 *}.
: 3*  ( n -- 3*n ) dup 2* + ;
\ *G A micro-optimisation for *\fo{3 *}.


\ *******************
\ *S Headerless words
\ *******************

: |  ( -- )  ;
\ *G Used in the form below to indicate that the word can be
\ ** headerless. If the host Forth does not support headerless
\ ** words, this becomes a *\fo{NOOP}.
\ *C   | : <name> ...  ;


\ ***************************************
\ *S Memory access with address increment
\ ***************************************

: c@+		\ caddr -- char caddr'
\ *G Fetch a char/byte and increment the address.
  count swap ;
: w@+		\ addr -- w addr'
\ *G Fetch a 16 bit word and increment the address.
  dup w@ swap 2+ ;
: @+		\ addr -- x addr'
\ *G Fetch a cell and increment the address.
  dup @ swap cell+ ;

: c!+		\ b addr -- addr'
\ *G Store a char/byte and increment the address.
  tuck c! char+ ;
: w!+		\ w addr -- addr'
\ *G Store a 16 bit word and increment the address.
  tuck w! 2+ ;
: !+		\ x addr -- addr'
\ *G Store a cell and increment the address.
  tuck ! cell+ ;

: wextend	\ w -- w'
\ *G Sign extend a 16 bit word to a cell.
  dup $8000 and negate or ;
: cextend	\ b -- b'
\ *G Sign extend an 8 bit byte to a cell.
  dup $80 and negate or ;

: cx@		\ addr -- sb
\ *G Fetch a byte and sign extend it.
  c@ cextend ;
: wx@		\ addr -- sw
\ *G Fetch a 16 bit word and sign extend it.
  w@ wextend ;
: wx@+		\ addr -- sw addr'
\ *G Fetch a 16 bit word, sign extend it and increment the address.
  dup wx@ swap 2+ ;


\ ********************
\ *S Memory allocation
\ ********************
\ *P Minos uses its own versions of memory access words.
\ ** Most of these words simply *\fo{THROW} on error. These
\ ** words were inspired by the Mac OS functions.

vocabulary memory	\ --
\ *G Vocabulary holding the memory access words.

also memory definitions
: NewPtr ( len -- addr )
\ *G Allocate a block of memory.
  dup cell+ allocate throw !+ ;
: DisposPtr ( addr -- )
\ *G Free a mmory block.
  ?dup IF cell- free throw THEN ;
: DelFix ( addr root -- )
  dup @ 2 pick ! ! ;
: NewFix  ( root len n -- addr )
\ *G Allocates a new element of length *\i{len} from a pool
\ ** specified by the cell-sized variable at *\i{root}. If the
\ ** pool has no free elements, *\i{n} new elements will be
\ ** created and added to the pool.
  BEGIN
    2 pick @ ?dup  0=
   WHILE
    2dup * NewPtr
    over 0 ?DO
      dup 4 pick DelFix 2 pick +
    LOOP
    drop
  REPEAT
  >r drop r@ @ rot ! r@ swap erase r>  ;

Variable Masters	 \ -- addr
\ *G The root of a pool.
:noname Masters off ; atcold

: NewMP ( -- MP )
\ *G Allocate a new master pointer, referred to as *\i{mp} in
\ ** the stack comments for other words.
  Masters cell $200 NewFix ;
: NewHandle ( len -- mp )
\ *G Allocate a new dangling memory area, which is pointed to by
\ ** by the returned master pointer.
  NewPtr NewMP tuck ! ;
: DisposHandle ( addr -- )
\ *G Free a a dangling memory area and associated master pointer.
  dup @ DisposPtr Masters DelFix ;
: Handle! ( len mp -- )
\ *G Allocate a block of *\i{len} bytes and associate it with *\i{mp}.
  >r NewPtr r> ! ;
: SetHandle ( addr mp -- )
\ *G Set the given master pointer.
  ! ;
: HandleOff ( addr -- )
\ *G Free the block whose pointer is at *\i{addr}.
  dup @ DisposPtr off ;
: Hlock ( mp -- )
\ *G Lock the pool. A dummy in most implementations.
  drop ;
: Hunlock ( addr -- )
\ *G Unlock the pool. A dummy in most implementations.
  drop ;
: SetHandleSize ( mp size -- )
\ *G Resize the pool.
  swap >r
  r@ @ cell- over cell+ resize throw !+ r> ! ;
: GetHandleSize ( mp -- size )
\ *G Get the size of the pool.
  cell- @ ;

previous definitions


\ ************
\ *S Overrides
\ ************
\ *P The words in this section override the VFX Forth versions

variable Seed	\ -- addr
\ *G A dummy variable used to satisfy references.

: RANDOM 	\ n1 -- n2
\ *G Generate a random number *\i{n2) in the range 0..n1-1.
\ ** The VFX Forth word that directly performs this is *\fo{CHOOSE}.
  choose
;


\ **********************
\ *S Miscellaneous tools
\ **********************

: pin ( x n -- )
\ *G Store x in the nth stack slot. The inverse of *\fo{PICK}.
  2+ cells sp@ + ! ;

: \G postpone \ ; immediate
\ *G A documenting comment used by gForth.

: ?EXIT		\ --
\ *G Equivalent to *\fo{IF  EXIT  THEN}.
  postpone IF  postpone EXIT postpone THEN ; immediate

: 8aligned ( n1 -- n2 )
\ *G Align *\i{n1} to a boundary of eight.
  7 + -8 and ;

: F  ( "<name>" -- )
\ *G Compiles *\fo{name} with *\fo{FORTH} as the first vocabulary
\ ** in the search path.
    also Forth bl word find dup 0= abort" Not found!"
    0< state @ and IF  compile,  ELSE  execute  THEN
    previous ; immediate

synonym AVariable Variable
\ *G A variable holding an address which may need relocation.
synonym AValue Value
\ *G A value holding an address which may need relocation.
synonym A, ,
\ *G For an address which may need relocation.
synonym AConstant Constant
\ *G An address which may need relocation.
synonym ALiteral Literal
\ *G An address which may need relocation.

synonym Patch Defer
\ *G Equivakent to *\fo{Defer}.
synonym << lshift
\ *G Equivakent to *\fo{lshift}.
synonym >> arshift
\ *G Equivakent to *\fo{arshift} which is as *\fo{rshift}, but
\ ** performs an arithmetic right shift.
synonym toss previous
\ *G Equivakent to *\fo{previous}.
synonym extend s>d
\ *G Equivakent to *\fo{s>d}.
: v! ! ;

: cont  ( addr -- )  >r ;
\ *G Causes a branch to *\i{addr}. When that code *\fo{EXIT}s,
\ ** execution resumes after the *\fo{cont}.
: (push) r> swap dup @ >r >r cont r> r> swap ! ;
: push	\ addr --
\ *G Save the contents of *\i{addr} on the return stack,
\ ** execute the rest of the word and then restore the contents
\ ** of *\i{addr}.
  postpone (push)  discard-sinline  ; immediate

: &	\ -- addr
\ *G Return the address of the data area of word rather than
\ ** its xt.
  ' >body state @ IF postpone Literal THEN ; immediate

: 0>= 0< 0= ;
\ *G Equivakent to *\fo{0 >=} or *\fo{0< 0=}.
: 0<= 0> 0= ;
\ *G Equivakent to *\fo{0 <=} or *\fo{0> 0=}.
: u>= u< 0= ;
\ *G Equivakent to *\fo{u< 0=}.
: u<= u> 0= ;
\ *G Equivakent to *\fo{u> 0=}.

: rdrop  postpone r>  postpone drop ; immediate
\ *G Equivakent to *\fo{R> DROP}.

: i'	\ --
\ *G Use inside *\fo{DO ... LOOP} and friends to return the loop
\ ** limit.
  r> 2r> 2dup 2>r rot >r swap - $80000000 xor ;

: +i'	\ n -- flag
\ *G Increment the loop limit by *\i{n} and return true if
\ ** the index is now greater than the limit. The comparison
\ ** is performed using circular arithmetic.
  negate 2r> rot r> + dup >r -rot 2>r 0< ;

: ith	\ addr -- x
\ *G Use inside *\fo{DO ... LOOP} and friends to return the
\ ** contents of the *fo{I}th cell in an array.
  postpone I postpone cells postpone + postpone @ ; immediate

: (list> ( thread -- element )
  BEGIN  @ dup  WHILE  dup r@ execute
  REPEAT  drop rdrop ;

: list> ( thread -- element )
\ *G Execute the rest of the word for each element of the given
\ ** list.
  postpone (list>  discard-sinline ; immediate

-1 cells Constant -cell	\ -- -4
\ *G The ngative size of a cell.
: over2		\ a b c -- a
\ *G Equivalent to *\fo{2 PICK}.
  2 pick ;

: toupper ( char -- char' )
\ *G Convert char to upper case.
    dup [char] a - [ char z char a - 1 + ] Literal u<  bl and - ;

: tolower ( char -- char' )
\ *G Convert char to lower case.
  dup 'A' 'Z' 1+ within IF  bl +  THEN ;

[defined] $linux [IF]
    0 Constant unix
[THEN]

: \needs	\ "name" --
\ *G If *\fo{name} is not defined, interpret the rest of the line,
\ ** otherwise ignore it. Usually used in the form:
\ *C   \needs foo  include foobar.fth
  postpone [defined] IF postpone \ THEN ;

: onlyforth  ( -- )  only forth ;
\ *G Equivalent to *\fo{ONLY FORTH}, setting the basic
\ ** search order.

: perform	\ ??? addr -- ???
\ *G *\fo{EXECUTE} the xt held at *\i{addr}.
  @ execute ;

: macro ; \ indicates macro
: hmacro ; \ indicates macro

: forward	\ "name" --
\ *G Declares a forward reference. Will be replaced by *\fo{DEFER}.
    : postpone ahead postpone then s" dummy string" postpone SLiteral
    discard-sinline postpone ; ;

: forward?  ( xt -- flag )
\ *G Return true if the *\i{xt} is of a forward referenced word.
\ ** OBSOLETE.
  c@ $E9 = ;

: : ( "name" -- )
\ *G Redefinition of *\fo{:} to cope with forward references in
\ ** a very rough way. OBSOLETE.
  MinosMode? if
    >in @ >r bl word find IF
      dup forward? IF
        1+ :noname over - 4- swap ! rdrop  EXIT
      THEN
    THEN
    drop  r> >in !
  then
  : ;

: recursive	\ --
\ *G Used inside a colon definition to make the word visible
\ ** for direct recursion.
  reveal ; immediate

: +bit ( addr n -- )
\ *G Set bit *\i{n} in the bit array starting at *\i{addr}.
  8 /mod swap >r + 1 r> lshift over c@ or swap c! ;
: -bit ( addr n -- )
\ *G Clear bit *\i{n} in the bit array starting at *\i{addr}.
  8 /mod swap >r + 1 r> lshift invert over c@ and swap c! ;
: bit@ ( addr n -- flag )
\ *G Test bit *\i{n} in the bit array starting at *\i{addr}.
  8 /mod swap >r + 1 r> lshift swap c@ and 0<> ;

: ,0"   ( -- )
\ *G Lay a zero terminated string. The end of the string is not
\ ** aligned.
\ *C   ,0" <text>"
  '"' parse  here swap dup allot move 0 c, ;

synonym 0" z"	\ -- ; -- addr
\ *G Compile a zero terminated string. At run-time the address
\ ** of the first character is returned.

: >len ( addr -- addr u ) dup zstrlen ;
\ *G The equivalent of *\fo{COUNT} for a zero-terminated string.

: 0place ( caddr len addr -- )
\ *G Save the string *\i{caddr/len} as a zero-terminated string
\ ** at *\i{addr}.
  swap 2dup + >r move 0 r> c! ;

(( unused
: "lit r> dup count + >r ;
))

: -scan ( addr len char -- addr' len' )
  >r
  BEGIN  1- dup WHILE  2dup + c@ r@ = UNTIL  1+  THEN
  rdrop ;

: safe/string ( c-addr u n -- c-addr' u' )
\ *G protect /string against overflows.
    dup negate >r  dup 0> IF
	/string dup r> u>= IF  + 0  THEN
    ELSE
	/string dup r> u< IF  + 1+ -1  THEN
    THEN ;

\ **************************
\ *S Interpretive structures
\ **************************
\ *P These structures only operate in the context of a single line.

: [IFUNDEF]	\ "name" --
\ *G Equivalent to *\fo{[undefined] name [if]}.
  postpone [undefined]  postpone [if]
; immediate

: [IFDEF]	\ "name" --
\ *G Equivalent to *\fo{[defined] name [if]}.
  postpone [defined]  postpone [if]
; immediate

Variable (i)

: [DO]  ( n-limit n-index -- )
\ *G Interpreted version of *\fo{DO}.
  >in @ -rot
  DO   I (i) ! dup >r >in ! interpret r> swap +LOOP  drop ;
                                                      immediate

: [?DO] ( n-limit n-index -- )
\ *G Interpreted version of *\fo{?DO}.
  2dup = IF 2drop postpone [ELSE] ELSE postpone [DO] THEN ;
                                                      immediate

: [+LOOP] ( n -- )
\ *G Interpreted version of *\fo{+LOOP}.
  rdrop rdrop ;                                       immediate

: [LOOP] ( -- )
\ *G Interpreted version of *\fo{LOOP}.
  1 rdrop rdrop ;                                     immediate

: [FOR] ( n -- )
\ *G Interpreted version of *\fo{FOR}.
  0 swap postpone [DO] ;                              immediate

: [NEXT] ( n -- )
\ *G Interpreted version of *\fo{NEXT}.
  -1 rdrop rdrop ;                                    immediate

: [I]
\ *G Interpreted version of *\fo{I}.
  (i) @ state @ IF  postpone Literal  THEN ;     immediate


\ *****************************
\ *S Application startup chains
\ *****************************
\ *P Minos requires two startup chains to which actions
\ ** can be added. The first uses the VFX Forth *\fo{ColdChain}
\ ** mechanism. This chain is executed in compilation order, i.e.
\ ** the first word added is executed first. The second chain,
\ ** anchored by *\fo{MainChain}, is executed so that the last
\ ** word added is executed first. The *\fo{MainChain} allows
\ ** outermost operations to initialise themselves before earlier
\ ** operations are initialised.

variable MainChain	\ -- addr
\ *G Anchors the list of operations.

: withChain:	\ anchor --
\ *G Starts a *\fo{:noname} word that is executed as part of
\ ** the chanin whose anchor is given.
  align here  0 , 0 ,			\ link, xt
  dup rot addLink			\ add to chain
  >r :noname r> cell + !		\ xt to second cell
; doNotSin

: Cold:		\ --
\ *G Starts a *\fo{:noname} word that is executed as part of
\ ** *\fo{ColdChain}. The first word added is executed first.
  ColdChain withChain:
;

: Bye:		\ --
\ *G Starts a *\fo{:noname} word that is executed as part of
\ ** *\fo{ExitChain} when VFX Forth finishes.  The lasr word
\ ** added is executed first.
  ExitChain withChain:
;
: Main:		\ --
\ *G Starts a *\fo{:noname} word that is executed as part of
\ ** *\fo{MainChain}. The last word added is executed first.
  MainChain withChain:
;

: WalkMainChain	\ --
\ *G Execute all the actions anchored by *\fo{MainChain}.
  MainChain execChain
;


\ ***********************
\ *S MODULE from bigFORTH
\ ***********************

also system definitions

#16 cells buffer: ModCurrents	\ -- addr
\ *G Holds the value of *\fo{CURRENT when a module is created.

variable pMC	\ -- addr
\ *G Holds a pointer into *\fo{ModCurrents} above.

: initPMC	\ --
\ *G Initialise *\fo{pMC} above.
  ModCurrents pMC !  forth-wordlist ModCurrents !
;  ' initPMC AtCold  initPMC

: +Module	\ --
\ *G Save the owner of a new module.
  cell pMC +!  current @ pMC @ !
;

: -Module	\ --
\ *G Restore the owner of the previous module.
  cell pMC -!
;

: widOwner	\ -- wid
\ *G Return the wid of the owning module.
  pMC @ @
;

: findExport	\ caddr len -- xt
\ *G Find the given word in the module, i.e. in the *\fo{CURRENT}
\ ** wordlist.
  current @ search-wordlist 0= abort" Can't word to export"
;

: createExport	\ caddr len --
\ *G Perform *\fo{CREATE} on the string, making the word in the
\ ** owning wordlist.
  get-current >r  widOwner set-current
  ($create)
  r> set-current
;

((
: SynComp,	\ xt --
\ Compile a child of SYNONYM.
  >body @ compile,
;
))

: MakeExport	\ caddr len --
\ *G Create a new definition iwhich behaves like an existing one.
\ ** The new definition is in the owning vocabulary and is searched
\ ** for in the *\fo{CURRENT} wordlist.
  2dup createExport
    findExport dup , immediate?
    if  immediate  endif
    ['] SynComp, set-compiler
  interp>
    @ execute
;

previous definitions

also system

: Module	\ "name" --
  MinosMode? 0= if  module exit  then
  +Module
  >in @ Vocabulary >in !
  get-order get-current swap 1+ set-order
  also ' execute also definitions ;

: Module;	\ --
  previous previous definitions previous
  -Module
;

: export	\ --
  MinosMode? 0= if  export exit  then
  begin
    get-word count 2dup s" ;" str= 0=
   while
    MakeExport
  repeat
  2drop
;

previous


\ ********************************
\ *S Operating system dependencies
\ ********************************

Vocabulary DOS

also DOS definitions

LocalExtern: setlocale char * setlocale ( int , char * ); ( locale addr -- addr )
LocalExtern: gettimeofday int gettimeofday ( int * , int * ); ( timeval timezone -- r )
LocalExtern: system int system ( char * ); ( cmd -- r )

synonym env$ readenv

Create timeval   0 , 0 ,
Create timezone  0 , 0 ,

previous definitions

[defined] Target_386_Windows [if]
char ; constant pathsep	\ -- char
[else]
char : constant pathsep	\ -- char
\ *G The separator between items in a list of paths. This is a
\ ** colon for Unix-based operating systems, but varies for
\ ** others, e.g. a semi-colon is used in Windows.
[then]

\ date & time conversion in files

LocalExtern: localtime int localtime ( char * );

$50 [defined] osx [IF] $100 + [THEN] Buffer: dta

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
        r> 0 max #11 min dup dup + + safe/string 3 min
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
LocalExtern: lxstat int __lxstat( int , int , int );
LocalExtern: xstat int __xstat( int , int , int );
: lstat  swap 1 -rot lxstat ; DoNotSin    ( buf name -- r )
: stat   swap 1 -rot xstat ; DoNotSin     ( buf name -- r )
LocalExtern: fnmatch int fnmatch( int , int , int );
LocalExtern: getcwd int getcwd( int , int );
LocalExtern: fdelete int unlink( int );
LocalExtern: dcreate int mkdir( int );

: dgetpath ( buffer drive -- ior )  drop $100 getcwd 0= ;

: ?diskabort throw ;

Variable dirbuf dirbuf off
Variable dirpath
Variable direndp
$80 Buffer: pattern
: diroff   dta 1 cells + ;
: dirsize  dta 2 cells + ;
: dirfd    dta 3 cells + ;
: dirstat' dta 4 cells + ;
: dirstat ( -- 0/ior )  dta @ >len 1+ direndp @ swap move
  dirstat'  dirpath @ 2dup stat
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
	 dup dta !
     pattern -rot swap fnmatch 0= UNTIL
     dirstat ;

: fsfirst ( C$ attr -- ior )   drop
  dup dirpath !  diroff off  dirsize off  dta off
  $400 dirbuf ?allot
  >len '/' -scan over + dup >r >len 1+ pattern swap move
  '.' r@ c! 0 r@ 1+ c! r> direndp !
  0 0 _open
  dup dirfd ! dup /ior swap -1 = ?EXIT  drop  fsnext ;

\ special characters

$08 Constant #bs         $0D Constant #cr
$0A Constant #lf         $1B Constant #esc
$09 Constant #tab        $07 Constant #bell


\ timer handling

1 cells +user time

also DOS
: timer@
  timeval timezone gettimeofday drop
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
  IF  bases + c@ base ! 1 safe/string  ELSE  drop  THEN ;
: getsign  over c@ '-' = dup >r negate safe/string r> ;
Defer char@ ' count IS char@
: s>number  ( addr len -- d )  base push
  getsign >r  getbase  base @ 0=
  IF  over + swap char@ >r swap over - dup 0= >r 1 = >r
      c@ ''' = r> and r> or dpl !  r> 0 rdrop  EXIT  THEN
  dpl on  getsign r> xor >r  0 0 2swap
  BEGIN  dup >r >number  dup r> =
         IF  rdrop 2drop dpl off  EXIT  THEN
         dup  WHILE  dup dpl ! over c@ -3 and ',' = 0=
         IF  rdrop 2drop dpl off  EXIT  THEN  1 safe/string
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


\ *** Stephen: check if this is complete ***

: :[ ( compile-time: -- orig colon-sys )
    state @ IF  <headerless> @ last @
	POSTPONE AHEAD  true  ELSE  false  THEN
     postpone [ :noname ; immediate

: ]: ( compile-time: orig colon-sys -- ; run-time: -- xt )
    discard-sinline  POSTPONE ; >r
    IF ]  POSTPONE THEN  r> POSTPONE Literal
	last ! <headerless> !  discard-sinline
    ELSE  r>  THEN ( xt ) ; immediate

0 [IF]
: if-else ( ... f xt1 xt2 -- ... )
\ Postscript-style if-else
    rot IF
       drop
    ELSE
       nip
    THEN
    execute ;

: test ( f -- )
    :[ ." true" ]:
    :[ ." false" ]:
    if-else ;

1 test cr \ writes "true"
0 test cr \ writes "false"
[THEN]


\ ***************
\ debugging tools
\ ***************

: .NextToken	\ --
\ *G Display the next token in the input stream.
  >in @  parse-name type  >in !
;

: terminali/o ( -- )
\ *G Use the same output device as when the code was compiled.
\ ** Be careful!
    [ op-handle @ ] Literal op-handle ! ;

: my.s ( ... -- ... )
\ *G A horizontal display version of *\fo{.s}.
    base @ >r hex ." <" depth 0 .r ." > "
    depth 0 max $10 min
    dup 0  ?DO  dup i - pick .  LOOP  drop r> base ! ;

: (~~) ( in line source -- )
\ *G Display the source location using *\fo{terminali/o}.
    [io terminali/o  cr
    .SourceName  ." :" 0 .r ." :" 0 .r space my.s
    io] ;

: ~~ ( -- )
\ *G Compile code so that the source location is displayed at
\ ** run time.
    >in @ postpone Literal LINE# @ postpone Literal
    'SourceFile @ postpone Literal
    postpone (~~) discard-sinline ; immediate

: [~~]		\ --
\ *G Display the current source location.
    >in @ LINE# @ 'SourceFile @ (~~) ; immediate


\ ======
\ *> ###
\ ======
