\ actor                                                23nov97py
debugging class actor
public: object ptr called       gadget ptr caller
        method set              ( -- )
        method reset            ( -- )
        method toggle           ( -- )
        method fetch            ( -- x1 .. xn )
        method store            ( x1 .. xn -- )
        method click            ( x y b n -- ... )
        method key              ( key sh -- )
        method enter            ( -- )
        method leave            ( -- )
        method assign           ( x1 .. xn -- )
        method set-called       ( o -- )
\ all methods send appropriate messages to called/caller
\ Use: <actor> (set|reset|toggle|fetch|store|click|enter|leave)

\ actor                                                23nov97py

how:    : init ( o -- ) bind called ;
        : set    fetch 0= IF  true  store  THEN ;
        : reset  fetch    IF  false store  THEN ;
        : toggle fetch 0= store ;
        : click  dup 0= IF  2drop 2drop  EXIT  THEN
          caller >released  IF  toggle  THEN ;
        : key ( key sh -- )  drop  dup bl = swap #cr = or
          IF  caller xywh 2drop  1 2 click  THEN ;
        : enter ( cr ." enter" ) ;
        : leave ( cr ." leave" ) ;
        : assign ;
        : set-called  bind called ;
class;

\ actor                                                23aug97py

actor class toggle
public: cell var do-set         cell var do-reset
        cell var set?
how:    : init ( o state xtset xtreset -- )
          do-reset !  do-set !  assign  super init ;
        : assign ( flag -- )  set? ! ;
        : fetch ( -- flag )  set? @ ;
        : store ( flag -- )  set? !
          set? @ IF  do-set  ELSE  do-reset  THEN  @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
      ;
        : click  dup 0= IF  2drop 2drop  EXIT  THEN
          toggle  caller >released  drop ;
class;

\ actor                                                05mar07py

actor class toggle-var
public: cell var addr           cell var xt
how:    : init ( o var xt -- ) xt ! assign super init ;
         : fetch ( -- n )  addr @ @ ;
        : store ( n -- )  addr @ ! xt @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
        : assign ( addr -- )  addr ! ;
class;
toggle-var class toggle-num
public: cell var num
how:    : assign ( o num var -- )  super assign num ! ;
        : !if ( n num addr -- )  rot IF  !  ELSE  nip on  THEN ;
        : fetch ( -- flag ) num @ addr @ @ = ;
        : store ( n -- )  num @ addr @ !if xt @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;

\ toggle bit                                           05mar07py

toggle-var class toggle-bit
public: cell var bit
how:    : fetch ( -- n )  addr @ bit @ bit@ ;
        : store ( n -- )  >r addr @ bit @
          r> IF  +bit  ELSE  -bit  THEN
          xt @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
        : assign ( addr bit -- ) bit ! addr ! ;
class;

\ actor                                                25sep99py

actor class toggle-state
public: cell var do-store       cell var do-fetch
how:    : init ( o xtstore xtfetch -- )
          do-fetch ! do-store ! super init ;
        : fetch ( -- x1 .. xn ) do-fetch @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
        : store ( x1 .. xn -- ) do-store @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;

actor class simple
public: cell var do-it
how:    : init ( o xt -- ) do-it ! super init ;
        : fetch 0 ;
        : store do-it @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 drop ;
class;

\ actor                                                25sep99py

: noop-act  0 ['] noop simple new ;

simple class click
how:    : click  store ;
        : fetch ;
        : store  do-it @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;

simple class data-act
public: cell var data
how:    : init ( o data xt -- ) swap data ! super init ;
        : store data @ super store ;
class;

\ actor                                                31aug97py

toggle-state class scale-act
public: cell var max
how:    : init ( o do-store do-fetch max -- )
          assign super init ;
        : assign ( max -- )  max ! ;
        : fetch  max @ do-fetch @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;

scale-act class slider-act
public: cell var step
how:    \ init ( o do-store do-fetch max step -- )
        : assign  step ! super assign ;
        : fetch  max @ step @ do-fetch @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;

\ actor                                                12apr98py

actor class scale-var
public: cell var max            cell var pos
how:    : init ( o pos max -- ) assign super init ;
        : assign  ( pos max -- )  max ! pos ! ;
        : fetch  max @ pos @ ;
        : store  pos !       ;
class;
scale-var class slider-var
public: cell var step
how:    : assign ( o pos max step -- ) step ! super assign ;
        : fetch  max @ step @ pos @ ;
class;

\ actor                                                24sep99py

scale-var class scale-do
public: cell var action
how:    : init ( o n max xt -- ) action ! super init ;
        : store  super store pos @ action @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;


slider-var class slider-do
public: cell var action
how:    : init ( o n max step xt -- ) action ! super init ;
        : store  super store pos @ action @
[defined] VFXFORTH [IF]
          called self >o execute o>
[ELSE]
	  called send
[THEN]
 ;
class;

\ actor simplification                                 05mar07py

[defined] VFXFORTH [IF]
    synonym S[ :[
    synonym DT[ :[
    synonym T[ :[
    synonym TS[ :[
    synonym CK[ :[
    synonym SC[ :[
    synonym SL[ :[
    synonym ]T[ :[
    synonym CP[ noop immediate
    synonym ]CP noop immediate
[ELSE]
    ' :[ alias S[                                immediate restrict
    ' :[ alias DT[                               immediate restrict
    ' :[ alias T[                                immediate restrict
    ' :[ alias TS[                               immediate restrict
    ' :[ alias CK[                               immediate restrict
    ' :[ alias SC[                               immediate restrict
    ' :[ alias SL[                               immediate restrict
    ' :[ alias ]T[                               immediate restrict
    ' noop Alias CP[ immediate
    ' noop Alias ]CP immediate
[THEN]
: ]S  postpone ]: simple postpone new ;      immediate restrict
: ]DT postpone ]: data-act postpone new ;    immediate restrict
: ]T  postpone ]: toggle postpone new ;      immediate restrict
: ]CK postpone ]: click  postpone new ;      immediate restrict
: ][  postpone ]: postpone :[ ;              immediate restrict
: ]TS  postpone ]: toggle-state postpone new ;
                                             immediate restrict
: ]N ;                                       immediate
: ]TERM ;                                    immediate

\ other simplifications                                05mar07py
: C[ ;                                       immediate restrict
: ]SC  postpone ]: scale-do postpone new ;   immediate restrict
: ]SL  postpone ]: slider-do postpone new ;  immediate restrict
: TV[  ;                                     immediate restrict
: TB[  ;                                     immediate restrict
: TN[  ;                                     immediate restrict
: ]TV  postpone ]: toggle-var postpone new ; immediate restrict
: ]TB  postpone ]: toggle-bit postpone new ; immediate restrict
: ]TN  postpone ]: toggle-num postpone new ; immediate restrict
: DF[ postpone dup postpone >o ;             immediate restrict
: ]DF postpone o> ;                          immediate restrict

