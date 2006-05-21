#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class login-1
public:
  early widget
  early open
  early open-app
  infotextfield ptr name
  infotextfield ptr passwort
  button ptr log-in
  button ptr cancel
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ log-in self ]DF s" Einloggen" open-component ;
  : open-app new DF[ log-in self ]DF s" Einloggen" open-application ;
class;

component class main-1
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Main" open-component ;
  : open-app new DF[ 0 ]DF s" Main" open-application ;
class;

main-1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[ login-1 open ]S ( MINOS ) S" Login" button new 
          ^^ S[ close ]S ( MINOS ) S" Logout" button new 
        &2 vabox new panel
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

login-1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ ST[  ]ST ( MINOS ) T" " S" Name" infotextfield new  ^^bind name
          ^^ ST[  ]ST ( MINOS ) T" " S" Passwort" infotextfield new  ^^bind passwort
            ^^ S[ ." login " name get type
."  with Passwort " passwort get type cr  ]S ( MINOS ) S" Log mich ein" button new  ^^bind log-in
            ^^ S[ close ]S ( MINOS ) S" Lass es sein" button new  ^^bind cancel
            ^^ S[ large-font ]S ( MINOS ) S" Large Font" button new 
          &3 habox new &1 hskips
        &3 vabox new panel
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  main-1 open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
