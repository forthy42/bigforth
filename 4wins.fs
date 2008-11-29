#! /usr/local/bin/bigforth
\ four in a row game

6 Value #rows
7 Value #cols
4 Value #win
#rows 2 + Value *rows
#cols 2 + Value *cols
8 Value #depth

\ board data base

Create board     here *rows *cols * dup allot erase

\ board operations: push stone and display result

[DEFINED] cx@ 0= [IF] : cx@ ( addr -- c ) c@ dup $80 and negate or ; [THEN]

: b[] ( x y -- board[x,y] )
    *rows * + [ board *rows 1+ + ] ALiteral + ;

: .board ( -- )  cr ." -0123456"
    #rows 0 ?DO  cr I 0 .r  #cols 0 ?DO
	    J I b[] cx@ 1 min -1 max 1+
	    s" x.o" drop + c@ emit  LOOP  LOOP ;

Variable cur-stone

: seeker  DOES> @ ( addr index -- n )
    over #win 0 ?DO  over + dup cx@ cur-stone @ * 0<= ?LEAVE  LOOP
    swap >r - negate r> / 1- ;

: seek ( n -- )  Create dup , seeker  Create negate , seeker ;

1        seek >left >right
*rows    seek >up   >down
*rows 1- seek >lu   >rd
*rows 1+ seek >ld   >ru

: score? ( boardp -- score-addr )
    >r
    r@ >left r@ >right + 
    r@ >up   r@ >down  + max
    r@ >lu   r@ >rd    + max
    r@ >ld   r@ >ru    + max 1+ cur-stone @ *
    r@ c! r> ;

: stone ( side col -- score-addr )  over cur-stone !
    0 swap b[] #rows 0 skip drop 1- tuck c! score? ;

Variable gameover  gameover on

: stone? ( n col -- )  stone cx@ abs #win >= gameover ! ;

\ alpha-beta min-max strategy

Variable side  -1 side !

: <stone ( score-addr ) 0 swap c! ; [DEFINED] macro [IF] macro [THEN]
: /side   side @ negate side ! ; [DEFINED] macro [IF] macro [THEN]

\ count all square scores with the same sign

: leaf-score ( -- score )
    0 0 board *rows *cols * bounds ?DO
	I cx@ dup 0>= IF  dup * +  ELSE  swap >r dup * + r>  THEN
    LOOP side @ 0< IF swap  THEN  over swap - * 8* 7 random + ;

\ alpha-beta-min-max: Same evaluation for both sides;
\ just negate the score of the other side
\ start with minimal possible score; leave with maximal score if you win
\ otherwise check score of next half-move
\ leave if better than beta
\ update alpha if current score is less

$7FFFFFFF     Constant <best>
<best> negate Constant <worst>
<best> 1-     Constant <win>
<win> negate  Constant <lost>
<best> 2/ 1+  Constant <half-best>

Create min-max# $20 cells allot

: eval-min-max ( beta n -- score best )
    1 over cells min-max# + +!
    dup 0= IF  2drop leaf-score 0  EXIT  THEN
    /side -1 <worst> ( beta n best alpha )
    #cols 0 ?DO
	0 I b[] cx@ 0= IF
	    side @ I stone >r
	    r@ cx@ abs #win >= IF
		r> <stone 2drop I <win> LEAVE  THEN
	    2over 1- swap >r over negate swap recurse drop
	    dup <half-best> / - negate r> r> <stone
	    \ beta n best alpha score beta
	    \ if score better than beta, we are done
	    2dup > IF  drop nip nip I swap LEAVE  THEN  drop
	    \ if score better than alpha, new score is best one
	    2dup < IF  nip nip I swap  ELSE  drop  THEN
	THEN
    LOOP  swap 2swap 2drop /side ;

: c ( -- score best ) min-max# $20 cells erase
    -1 side ! <best> #depth eval-min-max
\    min-max# #depth 1+ cells bounds ?DO I ? cell +LOOP  space dup . cr
    1 over stone? ;

: 4init    gameover off board *rows *cols * erase ;

: h ( n -- )  gameover @ IF  4init cr ." New game"  THEN
    dup #cols 0 within abort" sorry, outside the field"
    0 over b[] cx@ abort" sorry, column already full"
    -1 swap stone? gameover @ 0= IF
	c drop <lost> #depth + <= IF ." I'm going to lose"
	ELSE  gameover @ IF  ." I win"  THEN  THEN
    ELSE  ." you win"  THEN
    true #cols 0 ?DO  0 I b[] cx@ 0<> and  LOOP
    IF  ." tie" gameover on  THEN  .board ;
