#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include sql-classes.fs
component class sql
public:
  tableinfotextfield ptr db
  tableinfotextfield ptr table
  tableinfotextfield ptr #name
  tableinfotextfield ptr #street
  tableinfotextfield ptr #country
  tableinfotextfield ptr #zip
  tableinfotextfield ptr #town
 ( [varstart] ) method do-insert
database ptr sql-db
cell var db-name ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" SQL insert" ;
class;

include sql.fs
sql implements
 ( [methodstart] ) : do-insert
  db-name @ IF  db get db-name $@ compare 0=  ELSE  true  THEN
  IF  db get 2dup db-name $!  database new bind sql-db  THEN
  s" max(id)" sql-db select
  table get sql-db with from ) endwith
  0 0 sql-db with tuple@ s>number clear endwith
  table get sql-db insert(
  drop 1+ sql-db int,
  #name get sql-db string,
  #street get sql-db string,
  #country get sql-db string,
  #zip get drop sql-db int,
  #town get sql-db string,
  sql-db ) ;
: dispose
  sql-db self  IF  sql-db dispose  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        T" test" ^^ ST[  ]ST ( MINOS ) X" Data base:" tableinfotextfield new  ^^bind db
        T" customer" ^^ ST[  ]ST ( MINOS ) X" Table" tableinfotextfield new  ^^bind table
        T" " ^^ ST[  ]ST ( MINOS ) X" Name" tableinfotextfield new  ^^bind #name
        T" " ^^ ST[  ]ST ( MINOS ) X" Street" tableinfotextfield new  ^^bind #street
        T" " ^^ ST[  ]ST ( MINOS ) X" Country" tableinfotextfield new  ^^bind #country
        #0, ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" Zip" tableinfotextfield new  ^^bind #zip
        T" " ^^ ST[  ]ST ( MINOS ) X" Town" tableinfotextfield new  ^^bind #town
          ^^ S[ do-insert ]S ( MINOS ) X" Insert" button new 
          ^^ S[ close ]S ( MINOS ) X" Close" button new 
        #2 hatbox new #1 hskips
      #8 vabox new panel
    ( [dumpend] ) ;
class;

: main
  sql open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
