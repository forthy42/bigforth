\ oof.fs	Object Oriented FORTH
\ 		This file is (c) 1996,2000,2005 by Bernd Paysan
\			e-mail: bernd.paysan@gmx.de
\
\		Please copy and share this program, modify it for your system
\		and improve it as you like. But don't remove this notice.
\
\		Thank you.
\

\ This is the version for MPE's VFX

\ Loadscreen                                           27dec95py

decimal

: ]             \ -- 
\ *G Switch compiler into compilation state.
  state on  discard-sinline
;

: IMMEDIATE     \ --
\ *G Mark the last defined word as immediate. Immediate words
\ ** will execute whenever encountered regardless of
\ ** *\fo{STATE}.
  doNotSin immediate
;

\ debugging tool

: my.s ( ... -- ... )  base @ >r hex ." <" depth 0 .r ." > "
    depth 0 max $10 min
    dup 0  ?DO  dup i - pick .  LOOP  drop r> base ! ;
: (~~) ( in line source -- )  cr
    .SourceName ." :" 0 .r ." ," 0 .r space my.s ;
: ~~ ( -- )
    >in @ postpone Literal LINE# @ postpone Literal
    'SourceFile @ postpone Literal
    postpone (~~) ; immediate
: [~~]  >in @ LINE# @ 'SourceFile @ (~~) ; immediate

: \G postpone \ ; immediate
: ?EXIT  postpone IF  postpone EXIT postpone THEN ; immediate
: 8aligned ( n1 -- n2 )  7 + -8 and ;
: F also Forth bl word find dup 0= abort" Not found!"
    0< state @ and IF  compile,  ELSE  execute  THEN
    previous ; immediate

Vocabulary Objects  also Objects also definitions

Vocabulary oo-types  oo-types also

0 cells Constant :wordlist
1 cells Constant :parent
2 cells Constant :child
3 cells Constant :next
4 cells Constant :method#
5 cells Constant :var#
6 cells Constant :newlink
7 cells Constant :iface
8 cells Constant :init

0 cells Constant :inext
1 cells Constant :ilist
2 cells Constant :ilen
3 cells Constant :inum

\ cell +user op
: op currobj ;

Forth definitions
: op! ( o -- )  op ! ;

Create ostack 0 , 16 cells allot

: ^ ( -- o )
    state @ IF  postpone op postpone @  ELSE op @  THEN ; immediate
: o@ ( -- o )
    state @ IF  postpone ^ postpone @  ELSE  ^ @  THEN  ; immediate
: >o ( o -- )
    state @
    IF    postpone ^ postpone >r postpone op!
    ELSE  1 ostack +! ^ ostack dup @ cells + ! op!
    THEN  ; immediate
: o> ( -- )
    state @
    IF    postpone r> postpone op!
    ELSE  ostack dup @ cells + @ op! -1 ostack +!
    THEN  ; immediate
: size@  ( objc -- size )  :var# + @ 8aligned ;
: o[] ( n -- ) o@ size@ * ^ + op! ;

Objects definitions

\ Coding                                               27dec95py

0 Constant #static
1 Constant #method
2 Constant #early
3 Constant #var
4 Constant #defer

false Value oset?

: o+,   ( addr offset -- )
  postpone Literal postpone ^ postpone +
  oset? IF  postpone op!  ELSE  postpone >o  THEN  drop ;
: o*,   ( addr offset -- )
  postpone Literal postpone * postpone Literal postpone +
  oset? IF  postpone op!  ELSE  postpone >o  THEN ;
: o@+, ( n -- )  postpone o@ postpone Literal postpone + ;
: ^+, ( n -- )  postpone ^ postpone Literal postpone + ;
: o+@,  ( addr offset -- )
    ^+, postpone @
    oset? IF  postpone op!  ELSE  postpone >o  THEN drop ;
: ^*@  ( offset -- addr )  ^ + @ tuck @ size@ * + ;
: o+@*, ( addr offset -- )
    postpone Literal postpone ^*@
    oset? IF  postpone op!  ELSE  postpone >o  THEN drop ;

\ variables / memory allocation                        30oct94py

Variable lastob
Variable class-o
Variable lastparent   0 lastparent !
Variable vars
Variable methods
Variable decl  0 decl !
Variable 'link

: crash  true abort" unbound method" ;

: link, ( addr -- ) align here 'link !  , 0 , 0 , ;

0 link,

\ type declaration                                     30oct94py

: vallot ( size -- offset )  vars @ >r  dup vars +!
    'link @ 0=
    IF  lastparent @ dup IF  :newlink + @  THEN  link,
    THEN
    'link @ 2 cells + +! r> ;

: valign  ( -- )  vars @ aligned vars ! ;
: vfalign ( -- )  vars @ 8aligned vars ! ;

: mallot ( -- offset )    methods @ cell methods +! ;

oo-types definitions

: (static, ( offset -- ) >r : r> o@+, discard-sinline postpone ; ;
: static   ( -- ) \ oof- oof
    \G Create a class-wide cell-sized variable.
    mallot (static, ;
: (method, ( offset -- )  >r
    : r> o@+,
      postpone @ postpone execute discard-sinline postpone ; ;
: method   ( -- ) \ oof- oof
\G Create a method selector.
    mallot (method, ;
: early    ( -- ) \ oof- oof
\G Create a method selector for early binding.
    : postpone crash discard-sinline postpone ; doNotSin ;
: (var, ( offset -- )  >r
    : r> ^+, discard-sinline postpone ; ;
: var ( size -- ) \ oof- oof
\G Create an instance variable
    vallot (var, ;
: (defer, ( n -- ) >r
    : r> ^+,
    postpone @ postpone execute discard-sinline postpone ; ;
: defer    ( -- ) \ oof- oof
\G Create an instance defer
    valign cell vallot (defer, ;

\ recognise macros                                     06mar05py

: prefix-size ( a b -- n )
    0 >r  BEGIN  over c@ over c@ =  WHILE
	    r> 1+ >r  1+ swap 1+ swap  REPEAT
    2drop r> ;

4 (method, xxx1  8 (method, xxx2
4 (defer,  ddd1  8 (defer,  ddd2
4 (static, sss1  8 (static, sss2
$84 (method, xxx3  $108 (method, xxx4
$84 (defer,  ddd3  $108 (defer,  ddd4
$84 (static, sss3  $108 (static, sss4
early eee

' xxx1 ' xxx2 prefix-size Constant method#
' ddd1 ' ddd2 prefix-size Constant defer#
' sss1 ' sss2 prefix-size Constant static#

' xxx3 ' xxx4 prefix-size Constant method#2
' ddd3 ' ddd4 prefix-size Constant defer#2
' sss3 ' sss4 prefix-size Constant static#2

Objects definitions also oo-types

: exec1? ['] xxx1 method# tuck compare 0= ;
: exec2? ['] xxx3 method#2 tuck compare 0= ;
: exec?    ( addr -- flag )
    dup exec1? swap exec2? or ;
: static1? ['] sss1 static# tuck compare 0= ; 
: static2? ['] sss3 static#2 tuck compare 0= ;
: static?  ( addr -- flag )
    dup static1? swap static2? or ;
: early?   ( addr -- flag )
    ['] eee  1 tuck compare 0= ;
: defer?   ( addr -- flag )
    dup ['] ddd1 defer# tuck compare 0=
    swap ['] ddd3 defer#2 tuck compare 0= or ;

\ dealing with threads                                 29oct94py

: object-order ( wid0 .. widm m addr -- wid0 .. widn n )
    dup  IF  2@ >r recurse r> swap 1+  ELSE  drop  THEN ;

: interface-order ( wid0 .. widm m addr -- wid0 .. widn n )
    dup  IF    2@ >r recurse r> :ilist + @ swap 1+
         ELSE  drop  THEN ;

: add-order ( addr -- n )  dup 0= ?EXIT  >r
    get-order r> swap >r 0 swap
    dup >r object-order r> :iface + @ interface-order
    r> over >r + set-order r> ;

: drop-order ( n -- )  0 ?DO  previous  LOOP ;

\ object compiling/executing                           20feb95py

: o, ( xt early? -- )
  over exec1?   over and  IF 
      drop method# + c@ o@ + @  compile,  EXIT  THEN
  over exec2?   over and  IF 
      drop method#2 + @ o@ + @  compile,  EXIT  THEN
  over static1? over and  IF 
      drop static# + c@ o@ + @  postpone Literal  EXIT THEN
  over static2? over and  IF 
      drop static#2 + @ o@ + @  postpone Literal  EXIT THEN
  drop dup early?  IF 1+ dup @ + cell+  THEN  compile, ;

\ : (findo    ( string -- cfa n )
\     o@ add-order >r find r> drop-order ;
: (findo    ( string -- cfa n / f ) { string }
    o@ >r  0  BEGIN  drop
	r> 2@ swap >r
	string count rot search-wordlist
\	string count type dup 0= IF ."  not" THEN ."  found" cr
    dup r@ 0= or  UNTIL
    r> drop dup 0= IF
	o@ :iface + @ ?dup IF  >r  BEGIN  drop
	    r> 2@ swap >r :ilist + @
	    string count rot search-wordlist
	dup r@ 0= or  UNTIL  r> drop  THEN
    THEN ;

: findo    ( string -- cfa n )
    (findo dup 0= IF  true abort" method not found!" THEN ;

false Value method?

: method,  ( object early? -- )  true to method?
    swap >o >r bl word  findo  0< state @ and
    IF  r> o,  ELSE  r> drop execute  THEN  o> false to method?  ;

: cmethod,  ( object early? -- )
    state @ dup >r
    0= IF  postpone ]  THEN
    method,
    r> 0= IF  postpone [  THEN ;

: early, ( object -- )  true to oset?  true  method,
  state @ oset? and IF  postpone o>  THEN  false to oset? ;
: late,  ( object -- )  true to oset?  false method,
  state @ oset? and IF  postpone o>  THEN  false to oset? ;

\ new,                                                 29oct94py

previous Objects definitions

Variable alloc
0 Value ohere

: oallot ( n -- )  ohere + to ohere ;

: ((new, ( link -- )
  dup @ ?dup IF  recurse  THEN   cell+ 2@ swap ohere + >r
  ?dup IF  ohere >r dup >r :newlink + @ recurse r> r> !  THEN
  r> to ohere ;

: (new  ( object -- )
  ohere >r dup >r :newlink + @ ((new, r> r> ! ;

: init-instance ( pos link -- pos )
    dup >r @ ?dup IF  recurse  THEN  r> cell+ 2@
    IF  drop dup >r ^ +
        >o o@ :init + @ execute  0 o@ :newlink + @ recurse o>
        r> THEN + ;

: init-object ( object -- size )
    >o o@ :init + @ execute  0 o@ :newlink + @ init-instance o> ;

: (new, ( object -- ) ohere dup >r over size@ erase (new
    r> init-object drop ;

: (new[],   ( n o -- addr ) ohere >r
    dup size@ rot over * oallot r@ ohere dup >r 2 pick -
    ?DO  I to ohere >r dup >r (new, r> r> dup negate +LOOP
    2drop r> to ohere r> ;

\ new,                                                 29oct94py

Create chunks here 16 cells dup allot erase

: init-oo-mem  chunks 16 cells erase ;

[defined] DelFix 0= [IF]
: DelFix ( addr root -- ) dup @ 2 pick ! ! ;
[THEN]

[defined] NewFix 0= [IF]
: NewFix  ( root size # -- addr )
  BEGIN  2 pick @ ?dup 0=
  WHILE  2dup * allocate throw over 0
         ?DO    dup 4 pick DelFix 2 pick +
         LOOP
         drop
  REPEAT
  >r drop r@ @ rot ! r@ swap erase r> ;
[THEN]

: >chunk ( n -- root n' )
  1- -8 and dup 3 rshift cells chunks + swap 8 + ;

: Dalloc ( size -- addr )
  dup 128 > IF  allocate throw EXIT  THEN
  >chunk 2048 over / NewFix ;

: Salloc ( size -- addr ) align here swap allot ;

: dispose, ( addr size -- )
    dup 128 > IF drop free throw EXIT THEN
    >chunk drop DelFix ;

Forth definitions

: new, ( o -- addr )  dup size@
  alloc @ execute dup >r to ohere (new, r> ;

: new[], ( n o -- addr )  dup size@
  2 pick * alloc @ execute to ohere (new[], ;

: dynamic ['] Dalloc alloc ! ;  dynamic
: static  ['] Salloc alloc ! ;

Objects definitions

\ instance creation                                    29mar94py

: instance-does> ( -- )  DOES> state @ IF  dup postpone Literal
	oset? IF  postpone op!  ELSE  postpone >o  THEN
    THEN  early, ;

: instance, ( o -- ) instance-does>
    alloc @ >r F static new, r> alloc ! drop ;
: ptr,      ( o -- )  0 , ,
  DOES>  state @
    IF    dup postpone Literal postpone @ oset? IF  postpone op!  ELSE  postpone >o  THEN cell+
    ELSE  @  THEN late, ;

: array,  ( n o -- )  alloc @ >r static new[], r> alloc ! drop
    DOES> ( n -- ) dup dup @ size@
          state @ IF  o*,  ELSE  nip rot * +  THEN  early, ;

\ class creation                                       29mar94py

Variable voc#
Variable classlist
Variable old-current
Variable ob-interface

: voc! ( addr -- )  get-current old-current !
  add-order  2 + voc# !
  get-order wordlist tuck classlist ! 1+ set-order
  also oo-types classlist @ set-current ;

: (class-does>  DOES>  false method, ;

: (class ( parent -- )  (class-does>
    here lastob !  true decl !  0 ob-interface !
    0 ,  dup voc!  dup lastparent !
  dup 0= IF  0  ELSE  :method# + 2@  THEN  methods ! vars ! ;

: (is ( addr -- )  bl word findo drop
    dup defer? 0= abort" OO: not deferred!"
    defer# + c@ state @
    IF    ^+, postpone !
    ELSE  ^ + !  THEN ;

: goto, ( o -- ) \  method? IF  postpone r> postpone drop  THEN
    false method, ; \ should be tail call optimized

: inherit   ( -- )  bl word findo drop
    dup exec1?  IF  method# + c@ dup o@ + @ swap lastob @ + !  EXIT  THEN
    dup exec2?  IF  method#2 + @ dup o@ + @ swap lastob @ + !  EXIT  THEN
    abort" Not a polymorph method!" ;

\ instance variables inside objects                    27dec93py

: instvar,    ( addr -- ) dup , here 0 , 0 vallot swap !
    'link @ 2 cells + @  IF  'link @ link,  THEN
    'link @ >r dup r@ cell+ ! size@ dup vars +! r> 2 cells + !
    DOES>  dup 2@ swap state @ IF  o+,  ELSE  ^ + nip nip  THEN
           early, ;

: instptr>  ( -- )  DOES>  dup 2@ swap
    state @ IF  o+@,  ELSE  ^ + @ nip nip  THEN  late, ;

: instptr,    ( addr -- )  , here 0 , cell vallot swap !
    instptr> ;

: (o* ( i addr -- addr' ) dup @ size@ rot * + ;

: instarray,  ( addr -- )  , here 0 , cell vallot swap !
    DOES>  dup 2@ swap
           state @  IF  o+@*,  ELSE  ^ + @ nip nip (o*  THEN
           late, ;

\ bind instance pointers                               27mar94py

: ((link ( addr -- o addr' ) 2@ drop ^ + ;

: (link  ( -- o addr )  bl word findo drop >body state @
    IF postpone Literal postpone ((link EXIT THEN ((link ;

: parent? ( class o -- class class' ) @
  BEGIN  2dup = ?EXIT dup  WHILE  :parent + @  REPEAT ;

: (bound ( obj1 adr2 -- )  ! ;

: (bind ( addr -- ) \ <name>
    (link state @ IF postpone (bound EXIT THEN (bound ;

Forth definitions

: bind ( o -- )  ' >body  state @
  IF   postpone Literal postpone (bound EXIT  THEN
  (bound ;  immediate
: link ( o -- )  ' >body  state @
  IF   postpone Literal EXIT  THEN ;  immediate
: bind2 ( o -- )  (bind ; immediate

Objects definitions

\ method implementation                                29oct94py

Variable m-name
Variable last-interface  0 last-interface !

: interface, ( -- )  last-interface @
    BEGIN  dup  WHILE  dup , @  REPEAT drop ;

: inter, ( iface -- )
    align here over :inum + @ lastob @ + !
    here over :ilen + @ dup allot move ;

: interfaces, ( -- ) ob-interface @ lastob @ :iface + !
    ob-interface @
    BEGIN  dup  WHILE  2@ inter,  REPEAT  drop ;

: lastob!  ( -- )  lastob @ dup
    BEGIN  nip dup @ here cell+ 2 pick ! dup 0= UNTIL  drop
    dup , op! ^ class-o ! o@ lastob ! ;

: thread,  ( -- )  classlist @ , ;
: var,     ( -- )  methods @ , vars @ , ;
: parent,  ( -- o parent )
    o@ lastparent @ 2dup dup , 0 ,
    dup IF  :child + dup @ , !   ELSE  , drop  THEN ;
: 'link,  ( -- )
    'link @ ?dup 0=
    IF  lastparent @ dup  IF  :newlink + @  THEN  THEN , ;
: cells,  ( -- )
  methods @ :init ?DO  ['] crash , cell +LOOP ;

\ method implementation                                20feb95py

oo-types definitions

: how:  ( -- ) \ oof- oof how-to
\G End declaration, start implementation
    decl @ 0= abort" not twice!" 0 decl !
    align  interface,
    lastob! thread, parent, var, 'link, 0 , cells, interfaces,
    dup
    IF    dup :method# + @ >r :init + swap r> :init /string move
    ELSE  2drop  THEN ;

: class; ( -- ) \ oof- oof end-class
\G End class declaration or implementation
    decl @ IF  how:  THEN  0 'link !
    voc# @ drop-order old-current @ set-current ;

: ptr ( -- ) \ oof- oof
    \G Create an instance pointer
    Create immediate lastob @ here lastob ! instptr, ;
: asptr ( class -- ) \ oof- oof
    \G Create an alias to an instance pointer, cast to another class.
    cell+ @ Create immediate
    lastob @ here lastob ! , ,  instptr> ;

: Fpostpone  postpone postpone ; immediate

: : ( <methodname> -- ) \ oof- oof colon
    decl @ abort" HOW: missing! "  class-o @ op!
    >in @ >r bl word (findo 0=
    IF  r> >in ! m-name off :
    ELSE  r> drop
	dup exec? over early? or
	0= abort" not a method"
	m-name ! :noname
    THEN ;
    
Forth

: ; ( xt colon-sys -- ) \ oof- oof
    postpone ; DoNotSin
    m-name @ ?dup 0= ?EXIT  dup exec1?
    IF    method# + c@ lastob @ + !
    ELSE  dup exec2?
	IF    method#2 + @ lastob @ + !
	ELSE
	    dup 5 + c@ $C3 = IF  1+ dup >r - 4- r> !  EXIT  THEN
	    >body dup cell+ @ 0< IF  2@ swap lastob @ + @ + !  EXIT  THEN
	    drop
	THEN
    THEN ; immediate

previous
Forth definitions

\ object                                               23mar95py

Create object  immediate  0 (class \ do not create as subclass
         cell var  oblink       \ create offset for backlink
         static    thread       \ method/variable wordlist
         static    parento      \ pointer to parent
         static    childo       \ ptr to first child
         static    nexto        \ ptr to next child of parent
         static    method#      \ number of methods (bytes)
         static    size         \ number of variables (bytes)
	 static    newlink      \ ptr to allocated space
	 static    ilist        \ interface list
	 method    init ( ... -- ) \ object- oof
         method    dispose ( -- ) \ object- oof

         early     class ( "name" -- ) \ object- oof
	 early     new ( -- o ) \ object- oof
	 			immediate
	 early     new[] ( n -- o ) \ object- oof new-array
				immediate
         early     : ( "name" -- ) \ object- oof define
         early     ptr ( "name" -- ) \ object- oof
         early     asptr ( o "name" -- ) \ object- oof
         early     [] ( n "name" -- ) \ object- oof array
	 early     ::  ( "name" -- ) \ object- oof scope
	 			immediate
         early     class? ( o -- flag ) \ object- oof class-query
	 early     goto  ( "name" -- ) \ object- oof
				immediate
	 early     super  ( "name" -- ) \ object- oof
				immediate
         early     self ( -- o ) \ object- oof
	 early     bind ( o "name" -- ) \ object- oof
				immediate
         early     bound ( class addr "name" -- ) \ object- oof
	 early     link ( "name" -- class addr ) \ object- oof
				immediate
	 early     is  ( xt "name" -- ) \ object- oof
				immediate
	 early     send ( xt -- ) \ object- oof
	 early     with ( o -- ) \ object- oof
				immediate
	 early     endwith ( -- ) \ object- oof
				immediate
	 early     ' ( "name" -- xt ) \ object- oof tick
				immediate
	 early     postpone ( "name" -- ) \ object- oof
				immediate
	 early     implements ( -- ) \ object- oof

\ base object class implementation part                23mar95py

how:
0 parento !
0 childo !
0 nexto !
    : class   ( -- )       Create immediate o@ (class ;
    : :       ( -- )       Create immediate o@
	decl @ IF  instvar,    ELSE  instance,  THEN ;
    : ptr     ( -- )       Create immediate o@
	decl @ IF  instptr,    ELSE  ptr,       THEN ;
    : asptr   ( addr -- )
	decl @ 0= abort" only in declaration!"
	Create immediate o@ , cell+ @ , instptr> ;
    : []      ( n -- )     Create immediate o@
	decl @ IF  instarray,  ELSE  array,     THEN ;
    : new     ( -- o )     o@ state @
	IF  Fpostpone Literal Fpostpone new,  ELSE  new,  THEN ;
    : new[]   ( n -- o )   o@ state @
	IF  Fpostpone Literal Fpostpone new[], ELSE new[], THEN ;
    : dispose ( -- )       ^ size @ dispose, ;
    : bind    ( addr -- )  (bind ;
    : bound   ( o1 o2 addr2  -- ) (bound ;
    : link    ( -- class addr ) (link ;
    : class?  ( class -- flag )  ^ parent? nip 0<> ;
    : ::      ( -- )
	state @ IF  ^ true method,  ELSE  inherit  THEN ;
    : goto    ( -- )       ^ goto, ;
    : super   ( -- )       parento true method, ;
    : is      ( cfa -- )   (is ;
    : self    ( -- obj )   ^ ;
    : init    ( -- )       ;
    
    : '       ( -- xt )  bl word findo drop
	state @ IF  Fpostpone Literal  THEN ;
    : send    ( xt -- )  execute ;
    : postpone ( -- )  voc# @
	o@ add-order ^ Fpostpone Literal Fpostpone >o
	Fpostpone Fpostpone  Fpostpone o>
	drop-order voc# ! ;
    
    : with ( -- n )  voc# @
	state @ oset? 0= and IF  Fpostpone >o  THEN
	o@ add-order voc# ! false to oset? ;
    : endwith ( n -- )
	state @ oset? 0= and IF  Fpostpone o>  THEN
	voc# @ drop-order voc# ! ;

    : implements
	o@ add-order 1+ voc# ! also oo-types
	o@ lastob ! ^ class-o !
	false to oset?   get-current old-current !
	thread @ set-current ;
class; \ object

\ interface                                            01sep96py

Objects definitions

: implement ( interface -- ) \ oof-interface- oof
    align here over , ob-interface @ , ob-interface !
    :ilist + @ >r get-order r> swap 1+ set-order  1 voc# +! ;

: inter-method, ( interface -- ) \ oof-interface- oof
    :ilist + @ bl word count 2dup s" '" str=
    dup >r IF  2drop bl word count  THEN
    rot search-wordlist
    dup 0= abort" Not an interface method!"
    r> IF  drop state @ IF  postpone Literal  THEN  EXIT  THEN
    0< state @ and  IF  compile,  ELSE  execute  THEN ;

Variable inter-list
Variable lastif
Variable inter#

Vocabulary interfaces  interfaces definitions

: method  ( -- ) \ oof-interface- oof
    mallot Create , inter# @ ,
DOES> 2@ swap o@ + @ + @ execute ;

: how: ( -- ) \ oof-interface- oof
    align
    here lastif @ !  0 decl !
    here  last-interface @ ,  last-interface !
    inter-list @ ,  methods @ ,  inter# @ ,
    methods @ :inum cell+ ?DO  ['] crash ,  LOOP ;

: interface; ( -- ) \ oof-interface- oof
    old-current @ set-current
    previous previous ;

: : ( <methodname> -- ) \ oof-interface- oof colon
    decl @ abort" HOW: missing! "
    bl word count lastif @ @ :ilist + @
    search-wordlist 0= abort" not found"
    dup >body cell+ @ 0< 0= abort" not a method"
    m-name ! :noname ;

Forth

: ; ( xt colon-sys -- ) \ oof-interface- oof
  postpone ;
  m-name @ >body @ lastif @ @ + ! ; immediate

Forth definitions

: interface-does>
    DOES>  @ decl @  IF  implement  ELSE  inter-method,  THEN ;
: interface ( -- ) \ oof-interface- oof
    Create  interface-does>
    here lastif !  0 ,  get-current old-current !
    last-interface @ dup  IF  :inum @  THEN  1 cells - inter# !
    get-order wordlist
    dup inter-list ! dup set-current swap 1+ set-order
    true decl !
    0 vars ! :inum cell+ methods !  also interfaces ;

previous previous

: private: ; \ empty stub
: public: ;  \ empty stub

\ debugging class: also empty stub

object class debugging
private: early voc-
public:  early words  early m'      early see
         early view   early trace'  early debug
         early view!
  how:
    : words F words ;
    : see F see ;
class;
