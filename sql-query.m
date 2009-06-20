#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include sql-classes.fs
component class sql
public:
  tableinfotextfield ptr db
  tableinfotextfield ptr querys
  tableinfotextfield ptr froms
  tableinfotextfield ptr where
  infotextfield ptr order-by
  infotextfield ptr using
  button ptr query
  button ptr ok
  button ptr new-sql
  button ptr xa
  button ptr xb
  button ptr xd
  vviewport ptr querydpy
  text-label ptr emptyl
 ( [varstart] ) method do-query
database ptr sql-db
cell var db-name ( [varend] ) 
how:
  : params   DF[ query self ]DF X" SQL query" ;
class;

include sql.fs
sql implements
 ( [methodstart] ) : do-query
  db-name @ IF  db get db-name $@ compare 0=  ELSE  true  THEN
  IF  db get 2dup db-name $!  database new bind sql-db  THEN
  querys get nip 0=  IF  s" *"  ELSE  querys get  THEN
  sql-db select
  froms get sql-db from
  where get sql-db where
  order-by get using get sql-db order-using
  sql-db )
  sql-db entry-box
  querydpy with assign resized endwith ;
: dispose
  sql-db self  IF  sql-db dispose  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
          T" test" ^^ ST[  ]ST ( MINOS ) X" Data base:" tableinfotextfield new  ^^bind db
          T" " ^^ ST[  ]ST ( MINOS ) X" Select:" tableinfotextfield new  ^^bind querys
          T" product" ^^ ST[  ]ST ( MINOS ) X" From:" tableinfotextfield new  ^^bind froms
          T" " ^^ ST[  ]ST ( MINOS ) X" Where:" tableinfotextfield new  ^^bind where
            T" " ^^ ST[  ]ST ( MINOS ) X" Order:" infotextfield new  ^^bind order-by
            T" " ^^ ST[  ]ST ( MINOS ) X" Using:" infotextfield new  ^^bind using
          #2 habox new vfixbox  #1 hskips
            ^^ S[ do-query ]S ( MINOS )  TT" Start query" X" Query" button new  ^^bind query
            ^^ S[ close ]S ( MINOS )  TT" Close query dialog" X" Close" button new  ^^bind ok
            ^^ S[ sql open ]S ( MINOS )  TT" Open new dialog" X" New" button new  ^^bind new-sql
          #3 hatbox new #1 hskips
            ^^ S[ s" relname, usename" querys assign
s" pg_class, pg_user" froms assign
s" relname !~ '^pg_' and relowner = usesysid" where assign
s" " order-by assign
s" " using assign
do-query ]S ( MINOS )  TT" Query all tables" X" Tables" button new  ^^bind xa
            ^^ S[ s" c.relname, a.attnum, a.attname, t.typname, a.attlen, a.attnotnull" querys assign
s" pg_class c, pg_attribute a, pg_type t" froms assign
s" c.relname !~ '^pg_' and a.attnum > 0 and a.attrelid = c.oid and a.atttypid = t.oid" where assign
s" c.relname, a.attnum" order-by assign
s" " using assign
do-query ]S ( MINOS )  TT" Query all elements" X" Entries" button new  ^^bind xb
            ^^ S[ s" " querys assign
s" " froms assign
s" " where assign
s" " order-by assign
s" " using assign ]S ( MINOS )  TT" Clear query dialog" X" Clear" button new  ^^bind xd
          #3 hatbox new vfixbox  #1 hskips
        #7 vabox new vfixbox  panel
        1 1 vviewport new  ^^bind querydpy DS[ 
          X" No query" text-label new  ^^bind emptyl
        #1 habox new ]DS ( MINOS ) 
      #2 vabox new
    ( [dumpend] ) ;
class;

: main
  sql open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
