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
  tableinfotextfield ptr db
  tableinfotextfield ptr table
  tableinfotextfield ptr #name
  tableinfotextfield ptr #version
  tableinfotextfield ptr #price
 ( [varstart] ) method do-insert
database ptr sql-db
cell var db-name ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" SQL insert" open-component ;
  : open-app new DF[ 0 ]DF s" SQL insert" open-application ;
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
  #version get sql-db string,
  #price get drop sql-db int,
  sql-db ) ;
: dispose
  sql-db self  IF  sql-db dispose  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        T" test" ^^ ST[  ]ST ( MINOS ) S" Data base:" tableinfotextfield new  ^^bind db
        T" product" ^^ ST[  ]ST ( MINOS ) S" Table:" tableinfotextfield new  ^^bind table
        T" " ^^ ST[  ]ST ( MINOS ) S" name" tableinfotextfield new  ^^bind #name
        T" " ^^ ST[  ]ST ( MINOS ) S" version" tableinfotextfield new  ^^bind #version
        &0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" price" tableinfotextfield new  ^^bind #price
          ^^ S[ do-insert ]S ( MINOS ) S" Insert" button new 
          ^^ S[ close ]S ( MINOS ) S" Close" button new 
        &2 hatbox new &1 hskips
      &6 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  sql open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
