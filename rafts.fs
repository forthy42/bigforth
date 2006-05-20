\ x86 version of RAFTS                                 23dec01py

\ this uses a variation of Mini-OOF with active object

Variable chunks
$20 1- [FOR] chunks @ here chunks ! , 4 [FOR] 0 , [NEXT] [NEXT]

: method ( m v -- m' v ) Create  over , swap cell+ swap
  DOES> ( ... -- ... ) @ o@ + @ execute ;
: var ( m v size -- m v' ) Create  over , +
  DOES> ( -- addr ) @ ^ + ;
: class ( class -- class methods vars ) dup 2@ ;
: end-class  ( class methods vars -- )
  Create  here >r , dup , 2 cells ?DO ['] noop , 1 cells +LOOP
  cell+ dup cell+ r> rot @ 2 cells /string move ;
: defines ( xt class -- ) ' >body @ + ! ;
: new ( class -- o )  chunks @ dup @ chunks ! swap over ! ;
: free ( o -- )  chunks @ over 6 cells erase over ! chunks ! ;
: :: ( class "name" -- ) ' >body @ + @ compile, ;
Create object  1 cells , 2 cells ,

\ RTL definitions                                      23dec01py

Vocabulary RTL

: [A] assembler ; immediate
: [R] RTL ;       immediate

also memory also assembler also RTL definitions

\ virtual registers                                    29dec01py

object class
   cell var offset
   cell var reg1
   cell var reg2
   cell var factor
   cell var imm
   method asm
end-class vreg

:noname reg1 @ ;                                vreg defines asm

\ register + offset                                    29dec01py

vreg class
end-class sitem

:noname offset @ reg1 @ over IF  D)  ELSE  nip )  THEN ;
                                               sitem defines asm

vreg class
end-class si-ea

:noname offset @ reg1 @ reg2 @ factor @ xor
  offset @ IF  DI)  ELSE  I) drop  THEN ;      si-ea defines asm

vreg class
end-class imm-sitem

:noname ( reg -- # reg ) >r
  offset @ reg1 @ over IF  D)  ELSE  nip )  THEN
  r@ mov imm @ # r> ;                      imm-sitem defines asm

vreg class
end-class imm-si-ea

:noname ( reg -- # reg ) >r
  offset @ reg1 @ reg2 @ factor @ xor
  offset @ IF  DI)  ELSE  I) drop  THEN
  r@ mov imm @ # r> ;                      imm-si-ea defines asm

\ stack cache                                          23dec01py

\ Each element of the stack cache contains the register where
\ the value is cached, 0 if it's not cached

Variable dstack  $10 1- [FOR] 0 , [NEXT]
Variable rstack  $10 1- [FOR] 0 , [NEXT]
Variable lstack  $10 1- [FOR] 0 , [NEXT]

Variable ax#
Variable cx#
Variable dx#
Variable others

: reg# ( reg -- addr )
  ax case? IF  ax#  EXIT  THEN
  cx case? IF  cx#  EXIT  THEN
  dx case? IF  dx#  EXIT  THEN
  drop others ;

: #stack ( n o -- )
  dup @ vreg <> IF  drop EXIT  THEN
  >o reg1 @ o>
  reg# +! drop ;

: ?stack' ( i stack -- addr )
    2dup @ + $9 + cells over +
    dup @ 0= IF  >r sitem new >o
          drop cells offset ! esi reg1 !
          ^ o> r@ ! r>
    ELSE  nip nip  THEN ;
: ?stack ( i stack -- item )  ?stack' @ ;
: ?dstack  dstack ?stack ;
: ?rstack  rstack ?stack ;
: ?lstack  lstack ?stack ;

: +stack ( stack -- item )
  dup @ 1- swap 2dup ! ?stack ;

: +dstack  dstack +stack ;
: +rstack  rstack +stack ;
: +lstack  lstack +stack ;

: >stack ( item stack -- )
  over 1 swap #stack
  dup @ 1- 2dup swap ! $9 + cells + ! ;

: >dstack  dstack >stack ;
: >rstack  rstack >stack ;
: >lstack  lstack >stack ;

: stack> ( stack -- item )
  0 over ?stack' dup @ swap off rot 1 swap +!
  -1 over #stack ;

: dstack>  dstack stack> ;
: rstack>  rstack stack> ;
: lstack>  lstack stack> ;

\ stack operations                                     28dec01py

: dup  0 ?dstack >dstack ;
: over 1 ?dstack >dstack ;
: swap dstack> dstack> swap >dstack >dstack ;
: >r   dstack> >rstack ;
: r>   rstack> >dstack ;
: r@   0 ?rstack >dstack ;

\ register allocation                                  29dec01py

: uniq? ( o -- flag )
  dup @ vreg <> IF  drop false  EXIT  THEN
  >o reg1 @ o> reg# @ 0= ;

: find-free ( -- reg/0 )
  dx# @ 0= IF  DX  EXIT  THEN
  cx# @ 0= IF  CX  EXIT  THEN
  ax# @ 0= IF  AX  EXIT  THEN
  0 ;
  
: uniq-reg ( o -- o ) >o
  BEGIN  find-free ?dup 0= WHILE
         spill-reg  REPEAT
  >r asm r@ mov r> vreg new op! reg1 ! ^ o> ;

\ arithmetic operations                                29dec01py

: aop ( xt -- )
  dstack> dstack> over uniq? swap >dstack
  0= IF  uniq-reg  THEN
  >o >r dstack> >o asm o> asm r> execute ^ o> >dstack ;
: + ['] add aop ;
: - ['] sub aop ;
