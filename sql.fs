\ PostgreSQL interface                                 30mar98py

Module SQL-lib

DOS also Memory also MINOS also

[IFDEF] glibc  library libpq libpq.so.2
[ELSE]         library libpq libpq.so.1
[THEN]

\ PostgreSQL libpq connect calls                       30mar98py

legacy off

libpq PQconnectdb <rev> int (int) PQconnectdb ( conninfo -- PGconn )
libpq PQconndefaults <rev> (int) PQconndefaults ( -- PGcinfoopt )
[IFDEF] glibc
    libpq PQsetdbLogin <rev> int int int int int int int (int) PQsetdbLogin
    ( passwd login dbname pgtty pgoptions pgport pghost -- pgconn )
[ELSE]
    libpq PQsetdb <rev> int int int int int (int) PQsetdb
    ( dbname pgtty pgoptions pgport pghost -- pgconn )
[THEN]
libpq PQfinish <rev> int (int) PQfinish ( conn -- v )
libpq PQreset <rev> int (int) PQreset ( conn -- v )
libpq PQdb <rev> int (int) PQdb ( conn -- string )
libpq PQuser <rev> int (int) PQuser ( conn -- string )
libpq PQhost <rev> int (int) PQhost ( conn -- string )
libpq PQoptions <rev> int (int) PQoptions ( conn -- string )
libpq PQport <rev> int (int) PQport ( conn -- string )
libpq PQtty <rev> int (int) PQtty ( conn -- string )
libpq PQstatus <rev> int (int) PQstatus ( conn -- constatus )
libpq PQerrorMessage <rev> int (int) PQerrorMessage ( conn -- string )
libpq PQtrace <rev> int int (int) PQtrace ( debug_port conn -- v )
libpq PQuntrace <rev> int (int) PQuntrace ( conn -- v )

\ PostgreSQL exec calls                                30mar98py

libpq PQexec <rev> int int (int) PQexec ( query conn -- PGresult )
libpq PQgetline <rev> int int int (int) PQgetline ( length string conn -- n )
libpq PQendcopy <rev> int (int) PQendcopy ( conn -- n )
libpq PQputline <rev> int int (int) PQputline ( string conn -- v )
libpq PQresultStatus <rev> int (int) PQresultStatus ( res -- execst )
libpq PQntuples <rev> int (int) PQntuples ( res -- n )
libpq PQnfields <rev> int (int) PQnfields ( res -- n )
libpq PQfname <rev> int int (int) PQfname ( field_num res -- string )
libpq PQfnumber <rev> int int (int) PQfnumber ( field_name res -- n )
libpq PQftype <rev> int int (int) PQftype ( field_num res -- oid )
libpq PQfsize <rev> int int (int) PQfsize ( field_num res -- short )
libpq PQcmdStatus <rev> int (int) PQcmdStatus ( res -- string )
libpq PQoidStatus <rev> int (int) PQoidStatus ( res -- string )
libpq PQcmdTuples <rev> int (int) PQcmdTuples ( res -- string )
libpq PQgetvalue <rev> int int int (int) PQgetvalue ( field-num tup res -- string )
libpq PQgetlength <rev> int int int (int) PQgetlength ( field-num tup res -- n )
libpq PQgetisnull <rev> int int int (int) PQgetisnull ( field-num tup res -- n )
libpq PQclear <rev> int (int) PQclear ( res -- v )
libpq PQdisplayTuples <rev> int int int int int int (int) PQdisplayTuples
        ( quiet printheader fieldsep fillallign fp res -- v )
libpq PQprintTuples <rev> int int int int int (int) PQprintTuples
        ( width terseop printattrn fout res -- v )
libpq PQprint <rev> int int int (int) PQprint ( ps res fout -- v )
libpq PQnotifies <rev> int (int) PQnotifies ( conn -- PGnotify )
libpq PQfn <rev> int int int int int int int (int) PQfn ( n args r_int? r_len r_buf fnid conn --
                    PGresult )

\ PostgreSQL authentification calls                    30mar98py

libpq fe_getauthsvc <rev> int (int) fe_getauthsvc ( errmsg -- msgtype )
libpq fe_setauthsvc <rev> int int (int) fe_setauthsvc ( errmsg name -- v )
libpq fe_getauthname <rev> int (int) fe_getauthname ( errmsg -- string )

\ PostgreSQL misc calls                                30mar98py

\ leave that for future needs

\ SQL class                                            30mar98py

[IFUNDEF] database
    include sql-classes.fs
    export database sql-lib ;
[ELSE]
    export sql-lib ;
[THEN]
        
database implements
    : $+ ( addr u -- )
      tmpbuf @ +in @ + 2dup + 0 swap c!
      swap dup +in +! move ;
    : >$ ( addr u -- )  +in off $+ ;
    : >0" ( addr u -- addr ) >$ tmpbuf @ ;
    : #+ ( n -- ) extend under dabs <<# #S rot sign #> $+ #>> ;
    : ,+ ( -- )  state @ IF  s" , " $+  THEN  state on ;

\ basic operations                                     01apr98py

    : init ( dbname u -- )  $2000 tmpbuf Handle!
[IFDEF] glibc
      >0" 0 0 rot 0 0 0 0 PQsetdbLogin
[ELSE]
      >0" 0 0 0 0 PQsetdb
[THEN]
      conn ! ;
    : dispose ( -- )  tmpbuf HandleOff
      conn @ PQfinish F drop ;
    : exec ( addr u -- ) >0" conn @ PQexec res ! state off ;
    : fields ( -- n )  res @ PQnfields ;
    : field@ ( i -- addr u )  res @  PQfname >len ;
    : tuples ( -- n )  res @ PQntuples ;
    : tuple@ ( i j -- addr u ) swap res @  PQgetvalue >len ;
    : clear ( -- )  res @ PQclear F drop ;

\ SQL commands: table creation                         01apr98py

    : create( ( addr u -- )
      s" CREATE TABLE " >$  $+ s"  ( " $+  state off ;
    : inherits ( addr u -- )
      state @ -1 = IF  s" ) INHERITS (" $+  -2 state !
                 ELSE  s" , " $+  THEN  $+ ;
    : :string ( addr u n -- ) >r
      ,+ $+ s"  varchar(" $+ r> #+ s" )" $+ ;
    : :int ( addr u -- )
      ,+ $+ s"  int" $+ ;
    : :float ( addr u -- )
      ,+ $+ s"  real" $+ ;
    : :date ( addr u -- )
      ,+ $+ s"  date" $+ ;
    : :time ( addr u -- )
      ,+ $+ s"  time" $+ ;
    : ) ( -- )
      state @ 0>= IF  s" ;"  ELSE  s" );"  THEN  $+
      tmpbuf @ conn @ PQexec res ! state off ;
    : drop ( addr u -- )
      s" DROP TABLE " >$ $+ state off :: ) ;

\ SQL commands: field insertation                      01apr98py

    : insert( ( addr u -- )
      s" INSERT INTO " >$ $+ s"  VALUES ( " $+  state off ;
    : string, ( addr u -- )
      ,+ s" '" $+ $+ s" '" $+ ;
    : int, ( addr u -- )
      ,+ #+ ;
    : date, ( addr u -- )  string, ;
    : time, ( addr u -- )  string, ;

\ SQL commands: query                                  13apr98py

    : select ( addr u -- )
      dup 0= IF  2drop s" *"  THEN  \ default is "all"
      state @ 0=  IF  s" SELECT " >$ 1 state !
                ELSE  s" , " $+  THEN  $+ ;
    : select-distinct ( addr u -- )
      dup 0= IF  2drop s" *"  THEN  \ default is "all"
      state @ 0=  IF  s" SELECT DISTINCT " >$ 1 state !
                ELSE  s" , " $+  THEN  $+ ;
    : select-as ( addr1 u1 addr2 u2 -- ) 2swap
      dup 0= IF  2drop s" *"  THEN  \ default is "all"
      state @ 0=  IF  s" SELECT " >$ 1 state !
                ELSE  s" , " $+  THEN  $+ s"  AS " $+ $+ ;
    : from ( addr u -- )
      state @ 1 <= IF  s"  FROM " $+ 2 state !
                 ELSE  s" , " $+  THEN $+ ;
    : where ( addr u -- )
      dup 0= IF  2drop  EXIT  THEN  \ default is none
      state @ 2 <= IF  s"  WHERE " $+ 3 state !
                 ELSE  s"  AND " $+  THEN $+ ;
    : group ( addr u -- )
      dup 0= IF  2drop  EXIT  THEN  \ default is none
      state @ 3 <= IF  s"  GROUP BY " $+ 4 state !
                 ELSE  s" , " $+  THEN $+ ;
    : order ( addr u -- )
      dup 0= IF  2drop  EXIT  THEN  \ default is none
      state @ 4 <= IF  s"  ORDER BY " $+ 5 state !
                 ELSE  s" , " $+  THEN $+ ;
    : order-using ( addr1 u1 addr2 u2 -- )
      dup 0= IF  2drop  order  EXIT  THEN  \ default is order
      2swap dup 0=  IF  2drop 2drop  EXIT  THEN
      state @ 4 <= IF  s"  ORDER BY " $+ 5 state !
                 ELSE  s" , " $+  THEN $+ s"  USING " $+ $+ ;

\ Output functions                                     13apr98py

    : .heads ( -- ) fields 0 ?DO  I field@ type ." , "  LOOP ;
    : .entry ( i -- )
      fields 0 ?DO  dup I tuple@ type ." , " LOOP  F drop ;
    : .entries ( -- )
      .heads cr cr tuples 0 ?DO  I .entry cr  LOOP ;

    : entry-box ( -- o )
      fields tuples { fs ts |
      fs 0 ?DO  noop-act  I field@ button new  LOOP
      fs hatab new
      ts 0 ?DO  fs 0 ?DO  J I tuple@ text-label new  LOOP
          fs hatab new -1 borderbox  LOOP
      0 1 *filll 2dup glue new
      ts 2+ vabox new
      fs 0= IF  0 1 *filll 2dup glue new
                2 habox new  THEN } ;

\ Transaction begin and end                            23apr98py

    : begin ( -- ) s" BEGIN" exec ;
    : end ( -- ) s" END" exec ;

class;

toss toss toss sql-lib definitions

Module;
