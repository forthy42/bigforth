#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include opensched-types.fs
menu-window class opensched
public:
  early widget
  early open
  early all-open
  early modal-open
  infotextfield ptr newjob
  viewport ptr task-list
  infotextfield ptr newresource
  viewport ptr resource-list
 ( [varstart] ) cell var resources
cell var jobs
cell var vacations
cell var milestones
cell var sched-path
cell var filename
sizeof global var globals
sizeof task-graph var graph
method list-resources
method list-jobs
method print
method load-sched ( [varend] ) 
how:
  : open       screen self new >o show o> ;
  : all-open   screen self new >o show up@ app ! o> ;
  : modal-open screen self new >o show stop o> ;
class;

component class file-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" File Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" File Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" File Menu" open-application ;
class;

component class help-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Help Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" Help Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" Help Menu" open-application ;
class;

component class resource-title
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Resource Title" open-component ;
  : dialog   new DF[ 0 ]DF s" Resource Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" Resource Title" open-application ;
class;

component class job-title
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Job Title" open-component ;
  : dialog   new DF[ 0 ]DF s" Job Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" Job Title" open-application ;
class;

component class edit-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Edit Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" Edit Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" Edit Menu" open-application ;
class;

component class project
public:
  early widget
  early open
  early dialog
  early open-app
  infotextfield ptr prefix
  infotextfield ptr year
  infotextfield ptr month
  infotextfield ptr day
  tbutton ptr text
  tbutton ptr tex
  tbutton ptr html
  tbutton ptr weekly
  tbutton ptr monthly
  tbutton ptr slippage
  tbutton ptr res
  tbutton ptr tasks
  tbutton ptr tids
  tbutton ptr mile
  tbutton ptr deps
  tbutton ptr vacs
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Project Properties" open-component ;
  : dialog   new DF[ 0 ]DF s" Project Properties" open-dialog ;
  : open-app new DF[ 0 ]DF s" Project Properties" open-application ;
class;

component class process-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Process Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" Process Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" Process Menu" open-application ;
class;

component class graphs
public:
  early widget
  early open
  early dialog
  early open-app
  infotextfield ptr task
  infotextfield ptr year
  infotextfield ptr month
  infotextfield ptr day
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Graph Properties" open-component ;
  : dialog   new DF[ 0 ]DF s" Graph Properties" open-dialog ;
  : open-app new DF[ 0 ]DF s" Graph Properties" open-application ;
class;

graphs implements
 ( [methodstart] ) : show ( -- )
  current-widget @ opensched with graph endwith >r
  r@ task-graph name" @ IF
      r@ task-graph name" $@ task assign  THEN
  r> task-graph date @
  &100 /mod &100 /mod 0 year assign 0 month assign 0 day assign
  :: show ; ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ ST[  ]ST ( MINOS ) T" " S" Task Graph:" infotextfield new  ^^bind task
            ^^ SN[  ]SN ( MINOS ) &0. ]N ( MINOS ) S" End Year" infotextfield new  ^^bind year
            ^^ SN[  ]SN ( MINOS ) &0. ]N ( MINOS ) S" Month" infotextfield new  ^^bind month
            ^^ SN[  ]SN ( MINOS ) &0. ]N ( MINOS ) S" Day" infotextfield new  ^^bind day
          &3 habox new &1 hskips -&1 borderbox
        &2 vabox new &1 vskips
          $10 $2 *hfilll $10 $1 *vfil glue new 
          ^^ S[ current-widget @ opensched with graph endwith >r
task get r@ task-graph name" $!
year get drop &100 * month get drop + &100 * day get drop +
r> task-graph date !
close ]S ( MINOS ) S" OK" button new 
          $10 $1 *hfilll $10 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) S" Cancel" button new 
          $10 $2 *hfilll $10 $1 *vfil glue new 
        &5 hatbox new
      &2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

process-menu implements
 ( [methodstart] ) Variable tmpbuf ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ ^ opensched with
   s" opensched " tmpbuf $!
   filename $@ tmpbuf $+!
   0 tmpbuf $@ + c!
   tmpbuf $@ drop [ also dos ] system [ previous ] drop
endwith ]S ( MINOS ) S" OpenSched" menu-entry new 
        ^^ S[ ^ opensched with
   s" latex " tmpbuf $!
   globals global prefix $@ tmpbuf $+!
   s" .tex &" tmpbuf $+!
   0 tmpbuf $@ + c!
   tmpbuf $@ drop [ also dos ] system [ previous ] drop
endwith ]S ( MINOS ) S" LaTeX" menu-entry new 
        ^^ S[ ^ opensched with
   s" xdvi " tmpbuf $!
   globals global prefix $@ tmpbuf $+!
   s" .dvi &" tmpbuf $+!
   0 tmpbuf $@ + c!
   tmpbuf $@ drop [ also dos ] system [ previous ] drop
endwith ]S ( MINOS ) S" xdvi" menu-entry new 
      &3 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

project implements
 ( [methodstart] ) : show ( -- )
  current-widget @ opensched with globals endwith >r
  r@ global prefix dup @ IF  $@ prefix assign  ELSE  drop  THEN
  r@ global start @
  &100 /mod &100 /mod 0 year assign 0 month assign 0 day assign
  r@ global reports @
  dup   1 and IF  text set     THEN
  dup   2 and IF  tex set      THEN
  dup   4 and IF  html set     THEN
  dup   8 and IF  weekly set   THEN
  dup $10 and IF  monthly set  THEN
  dup $20 and IF  slippage set THEN drop
  r> global shows @
  dup   1 and IF  res set    THEN
  dup   2 and IF  tasks set  THEN
  dup   4 and IF  tids set   THEN
  dup   8 and IF  mile set   THEN
  dup $10 and IF  deps set   THEN
  dup $20 and IF  vacs set   THEN  drop
  :: show ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ ST[  ]ST ( MINOS ) T" " S" Project Prefix:" infotextfield new  ^^bind prefix
          ^^ SN[  ]SN ( MINOS ) &0. ]N ( MINOS ) S" Start Year" infotextfield new  ^^bind year
          ^^ SN[  ]SN ( MINOS ) &0. ]N ( MINOS ) S" Month" infotextfield new  ^^bind month
          ^^ SN[  ]SN ( MINOS ) &0. ]N ( MINOS ) S" Day" infotextfield new  ^^bind day
        &3 hatab new &1 hskips -&1 borderbox
            S" Reports:" text-label new 
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Text" tbutton new  ^^bind text
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" TeX" tbutton new  ^^bind tex
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" HTML" tbutton new  ^^bind html
          &4 hatbox new
            S" Details:" text-label new 
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Weekly" tbutton new  ^^bind weekly
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Monthly" tbutton new  ^^bind monthly
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Slippage" tbutton new  ^^bind slippage
          &4 hatbox new
        &2 vatbox new -&1 borderbox
            S" Show:" text-label new 
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Resources" tbutton new  ^^bind res
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Tasks" tbutton new  ^^bind tasks
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Task-IDs" tbutton new  ^^bind tids
          &4 hatab new
            S" " text-label new 
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Milestones" tbutton new  ^^bind mile
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Dependencies" tbutton new  ^^bind deps
            ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Vacations" tbutton new  ^^bind vacs
          &4 hatab new
        &2 vabox new -&1 borderbox
          $10 $2 *hfilll $10 $1 *vfil glue new 
          ^^ S[ current-widget @ opensched with globals endwith >r
prefix get r@ global prefix $!
year get drop &100 * month get drop + &100 * day get drop +
r@ global start !
text get 1 and  tex get 2 and or  html get 4 and or
weekly get 8 and or  monthly get $10 and or
slippage get $20 and or
r@ global reports !
res get 1 and  tasks get 2 and or  tids get 4 and or
mile get 8 and or  deps get $10 and or  vacs get $20 and or
r> global shows !
close ]S ( MINOS ) S" OK" button new 
          $10 $1 *hfilll $10 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) S" Cancel" button new 
          $10 $2 *hfilll $10 $1 *vfil glue new 
        &5 hatbox new
      &5 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

edit-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ ^ current-widget ! project open ]S ( MINOS ) S" Project..." menu-entry new 
        ^^ S[ ^ current-widget ! graphs open ]S ( MINOS ) S" Task Graph..." menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

job-title implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" Id" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Days" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Complete" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Name" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Description" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Candidates" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Dependencies" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Schedules" lbutton new 
        $0 $1 *hfilll $0 $1 *vfilll glue new 
      &9 hatab new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

resource-title implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" Id" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Name" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Efficiency" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Rate" lbutton new 
        ^^ S[  ]S ( MINOS ) S" Note" lbutton new 
        $0 $1 *hfilll $0 $1 *vfilll glue new 
      &6 hatab new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

help-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" About OpenSched" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" About Open Schedule GUI" menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

file-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ ^ opensched with
s" Sched File" s" " sched-path @
IF  sched-path $@  ELSE  s" *.sched"  THEN
^ S[ 2swap sched-path $! filename $! ^ current-widget !
     load-sched ]S fsel-dialog
endwith ]S ( MINOS ) S" Load file..." menu-entry new 
        ^^ S[ ^ opensched with
filename @ IF  print
ELSE   s" Sched File" s" "  sched-path @
       IF  sched-path $@  ELSE  s" *.sched"  THEN
       ^ S[ 2swap sched-path $! filename $! print ]S
       fsel-dialog
THEN  endwith ]S ( MINOS ) S" Save" menu-entry new 
        ^^ S[ ^ opensched with
s" Sched File" filename @ IF  filename $@  ELSE  s" "  THEN
sched-path @
IF  sched-path $@  ELSE  s" *.sched"  THEN
^ S[ 2swap sched-path $! filename $! print ]S
fsel-dialog
endwith ]S ( MINOS ) S" Save as..." menu-entry new 
        ^^ S[ close ]S ( MINOS ) S" Quit" menu-entry new 
      &4 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

include opensched.fs
opensched implements
 ( [methodstart] ) : list-jobs ( -- )
  jobs @ dup 0= IF
      drop s" Task List" text-label new task-list assign  EXIT
  THEN
  >r job-title widget 1
  BEGIN  ^ TV[ r@ job selected ]T[ ]TV r@ job id     $@ tbutton new
         ^ IV[ r@ job days        ]IV  r@ job days     @ 0  textfield new
         ^ IV[ r@ job complete    ]IV  r@ job complete @ 0  textfield new
         ^ VT[ r@ job name"       ]VT
               r@ job name"       $@ textfield new
         ^ VT[ r@ job description ]VT
               r@ job description $@ textfield new
         0 r@ job candidate DT[ dpy dpy self with resources @ endwith
                                ^ list! ]DT
           r@ job candidate list-string button new
         0 r@ job depends   DT[ dpy dpy self with jobs @ endwith
                                ^ list! ]DT
           r@ job depends   list-string button new
         0 r@ job schedule  DT[ drop ]DT s" edit" button new
         $0 $1 *hfilll 2dup glue new
         9 hatab new swap 1+
         r> @ dup  WHILE  >r  REPEAT  drop
  $0 $1 *hfilll 2dup glue new swap 1+
  vabox new task-list assign ;
: list-resources ( -- )
  resources @ dup 0= IF
      drop s" Resources" text-label new task-list assign  EXIT
  THEN
  >r resource-title widget 1
  BEGIN  ^ TV[ r@ resource selected ]T[ ]TV r@ resource id $@ tbutton new
         ^ VT[ r@ resource name" ]VT
               r@ resource name" $@ textfield new
         ^ IV[ r@ resource efficiency ]IV
               r@ resource efficiency @ 0 textfield new
         ^ IV[ r@ resource rate ]IV
               r@ resource rate @ 0 textfield new
         ^ VT[ r@ resource note ]VT
               r@ resource note $@ textfield new
         $0 $1 *hfilll 2dup glue new
         6 hatab new swap 1+
         r> @ dup  WHILE  >r  REPEAT  drop
  $0 $1 *hfilll 2dup glue new swap 1+
  vabox new resource-list assign ;
Variable textmp
: print [ also fileop ]
  filename $@ r/w output-file +buffer
  globals .globals cr
  globals graph .task-graph
  resources  BEGIN  @ dup  WHILE  dup .resource  REPEAT
  drop cr
  jobs  BEGIN  @ dup  WHILE  dup .job  REPEAT  drop cr
  eot
  filename $@ '/ -scan textmp $!
  globals global prefix $@ textmp $+! s" .tex" textmp $+!
  textmp $@ r/w output-file +buffer graph globals .tex eot
  [ previous ] ;
: load-sched  ( -- )  filename $@ include-sched
  list-jobs list-resources ;
: check-cr ( addr u -- flag )  2dup #cr scan nip IF  
        here -rot bounds ?DO
           I c@ dup #cr = IF  drop  ELSE  c,  THEN  LOOP
        dup here over - rot dp ! true
  ELSE  2drop false  THEN ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        M: file-menu widget S"  File " menu-title new 
        M: edit-menu widget S"  Edit " menu-title new 
        M: process-menu widget S"  Process " menu-title new 
        $190 $1 *hfilll $1 $1 *vfil rule new 
        M: help-menu widget S"  Help " menu-title new 
      &5 hbox new vfixbox  &2 borderbox
            ^^ ST[ newjob get check-cr IF  
    new-job jobs append! list-jobs
    s" " newjob assign  THEN ]ST ( MINOS ) T" " S" New Job:" infotextfield new  ^^bind newjob
              ^^ S[  ]S ( MINOS ) S" Delete Selected Jobs" button new 
            &1 habox new hfixbox 
          &2 habox new vfixbox  panel
            1 1 viewport new  ^^bind task-list DS[ 
              S" Job List" text-label new 
            &1 vabox new ]DS ( MINOS ) 
            $0 $0 *hpix $A0 $1 *vfilll glue new 
          &2 habox new
          vxrtsizer new 
        &3 vasbox new
          ^^ ST[ newresource get check-cr IF  
    new-resource resources append! list-resources
    s" " newresource assign  THEN ]ST ( MINOS ) T" " S" New Resource:" infotextfield new  ^^bind newresource
            ^^ S[  ]S ( MINOS ) S" Detele Selected Resources" button new 
          &1 habox new hfixbox 
        &2 habox new vfixbox  panel
          1 1 viewport new  ^^bind resource-list DS[ 
            S" Resources" text-label new 
          &1 vabox new ]DS ( MINOS ) 
          $0 $0 *hpix $A0 $1 *vfilll glue new 
        &2 habox new
      &3 vabox new
    ( [dumpend] ) ;
  : title$  s" Open Schedule GUI" ;
  : init  super init  ^ to ^^
    widget 1 0 modal new  title$ assign ;
class;

: main
  opensched all-open
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
