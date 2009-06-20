#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class login-1
public:
  infotextfield ptr name
  infotextfield ptr passwort
  button ptr log-in
  button ptr cancel
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Einloggen" ;
class;

component class main-1
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Main" ;
class;

main-1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[ login-1 open ]S ( MINOS ) X" Login" button new 
          ^^ S[ close ]S ( MINOS ) X" Logout" button new 
        #2 vabox new panel
      #1 vabox new
    ( [dumpend] ) ;
class;

login-1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          T" " ^^ ST[  ]ST ( MINOS ) X" Name" infotextfield new  ^^bind name
          T" " ^^ ST[  ]ST ( MINOS ) X" Passwort" infotextfield new  ^^bind passwort
            ^^ S[ ." login " name get type
."  with Passwort " passwort get type cr  ]S ( MINOS ) X" Log mich ein" button new  ^^bind log-in
            ^^ S[ close ]S ( MINOS ) X" Lass es sein" button new  ^^bind cancel
            ^^ S[ large-font ]S ( MINOS ) X" Large Font" button new 
          #3 habox new #1 hskips
        #3 vabox new panel
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  main-1 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
