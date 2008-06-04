#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include sql-classes.fs
component class sql
public:
  early widget
  early open
  early open-app
  infotextfield ptr db
  infotextfield ptr querys
  button ptr query
  button ptr ok
  button ptr new-sql
  vviewport ptr querydpy
  text-label ptr emptyl
 ( [varstart] ) method do-query
database ptr sql-db
cell var db-name ( [varend] ) 
how:
  : open     new DF[ query self ]DF s" SQL query" open-component ;
  : open-app new DF[ query self ]DF s" SQL query" open-application ;
class;

include sql.fs
sql implements
 ( [methodstart] ) : do-query
  db-name @ IF  db get db-name $@ compare 0=  ELSE  true  THEN
  IF  db get 2dup db-name $!  database new bind sql-db  THEN
  querys get sql-db exec
  sql-db entry-box
  querydpy with assign resized endwith ; ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ T" test" ST[  ]ST ( MINOS ) S" Data base:" infotextfield new  ^^bind db
          ^^ T" " ST[  ]ST ( MINOS ) S" Query:" infotextfield new  ^^bind querys
            ^^ S[ do-query ]S ( MINOS )  TT" Start query" S" Query" button new  ^^bind query
            ^^ S[ close ]S ( MINOS )  TT" Close query dialog" S" Close" button new  ^^bind ok
            ^^ S[ sql open ]S ( MINOS )  TT" Open new dialog" S" New" button new  ^^bind new-sql
          &3 hatbox new &1 hskips
        &3 vabox new vfixbox  panel
        1 1 vviewport new  ^^bind querydpy DS[ 
          S" No query" text-label new  ^^bind emptyl
        &1 habox new ]DS ( MINOS ) 
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  sql open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
