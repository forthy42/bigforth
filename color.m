#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class color
public:
  button ptr color-ok
 ( [varstart] ) cell var color ( [varend] ) 
how:
  : params   DF[ color-ok self ]DF s" Color" ;
class;

component class result
public:
  infotextfield ptr choice
  button ptr choice-ok
 ( [varstart] ) cell var color ( [varend] ) 
how:
  : params   DF[ choice-ok self ]DF s" Your Choice" ;
class;

result implements
( [methodstart] ) : assign ( addr u -- ) color $! ;
: show color $@ choice assign super show ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        T" " ^^ ST[  ]ST ( MINOS ) X" Choice" infotextfield new  ^^bind choice
          ^^ S[ bye ]S ( MINOS ) X" OK" button new  ^^bind choice-ok
          ^^ S[ close ]S ( MINOS ) X" Cancel" button new 
        #2 hatbox new #1 hskips
      #2 hatbox new panel
    ( [dumpend] ) ;

class;

color implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ TN[ 0 color ]T[ ( MINOS )  ]TN ( MINOS ) X" Red" rbutton new 
          ^^ TN[ 1 color ]T[ ( MINOS )  ]TN ( MINOS ) X" Green" rbutton new 
          ^^ TN[ 2 color ]T[ ( MINOS )  ]TN ( MINOS ) X" Blue" rbutton new 
          ^^ TN[ 3 color ]T[ ( MINOS )  ]TN ( MINOS ) X" Alpha" rbutton new 
        #4 varbox new
          $10 $1 *hfill $0 $1 *vfill glue new 
          ^^ S[ drop color @ 0 = IF s" Red"   THEN
color @ 1 = IF s" Green" THEN
color @ 2 = IF s" Blue"  THEN
color @ 3 u>= IF s" Alpha" THEN
result open-app ]S ( MINOS ) X" OK" button new  ^^bind color-ok
          ^^ S[ close ]S ( MINOS ) X" Cancel" button new 
          $10 $1 *hfill $0 $1 *vfill glue new 
        #4 vabox new #1 vskips
      #2 habox new panel
    ( [dumpend] ) ;

class;

: main
  color open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
