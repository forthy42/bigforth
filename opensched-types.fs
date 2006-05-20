\ OpenSched GUI load/store part                        15apr01py

include fileop.fb

Variable job-root
Variable resource-root
Variable list-root
Variable current-widget

\ structures                                           15apr01py

struct{
  ptr next
  ptr id
  ptr name"
  cell selected
  cell days
  cell complete
  ptr description
  ptr candidate
  ptr depends
  ptr schedule
  ptr nety
} job

struct{
  ptr next
  ptr item
} list"

struct{
  ptr next
  ptr id
  ptr name"
  cell selected
  cell efficiency
  cell rate
  ptr note
} resource

struct{
  ptr prefix
  cell start
  cell reports
  cell shows
} global

struct{
  ptr name"
  cell date
} task-graph

\ list handling                                        15apr01py

: append!  ( element list -- )
  BEGIN  dup @  WHILE  @  REPEAT ! ;

also memory

: new-job ( addr u -- addr' )
  job-root sizeof job $400 NewFix >r
  r@ job id $!
  s" " r@ job name" $!
  s" " r@ job description $! r> ;

: new-resource ( addr u -- addr' )
  resource-root sizeof resource $400 NewFix >r
  r@ resource id $!
  &100 r@ resource efficiency !
  s" " r@ resource name" $!
  s" " r@ resource note $! r> ;

: new-list ( addr u -- addr' )
  list-root sizeof list" $400 NewFix >r
  r@ list" item $! r> ;

: free-list ( list -- ) dup >r
  @  BEGIN  dup  WHILE  dup @ swap list-root DelFix  REPEAT
  drop r> off ;

previous

\ item list                                            16apr01py

: scan-list ( head list -- head )  >r
  BEGIN  r@ job selected @
         IF  r@ job id $@ new-list over append!  THEN
         r> @  dup  WHILE  >r  REPEAT  drop ;

Variable list$
: list-string ( list -- addr u ) s" " list$ $!
  BEGIN  @ dup  WHILE
         list$ $@len IF  s"  " list$ $+!  THEN
         dup job id $@ list$ $+!  REPEAT  drop
  list$ $@ dup 0= IF  2drop s" <none>"  THEN ;

: list! ( head reslist button -- ) >r
  over free-list scan-list list-string
  r> button with assign endwith ;

\ display part                                         15apr01py

: .id ( addr -- )  job id $@ type space ;

: .list ( list -- )
    BEGIN  @ dup  WHILE  dup .id  REPEAT  drop ;

: .job ( addr -- ) base push decimal
  cr ." task " dup .id
  '" emit dup job name" $@ type '" emit space
  dup job days @ 0 u.r
  cr ." describe " dup .id
  '" emit dup job description $@ type '" emit
  dup job complete @ 
  ?dup IF  cr ." complete " over .id 0 .r  THEN
  dup job candidate dup @
  IF  cr ." candidate " over .id .list  ELSE  drop  THEN
  dup job depends dup @
  IF  cr ." depends " over .id .list  ELSE  drop  THEN
  cr drop ;

: d.2 ( n -- ) base push decimal  0 <# # # '. hold #s #> type ;

: .resource ( addr -- )  base push decimal
  cr ." resource " dup resource id $@ type space
  '" emit dup resource name" $@ type '" emit
  dup resource efficiency @ &100 <>
  IF  cr ." efficiency " dup .id
      dup resource efficiency @ d.2  THEN
  dup resource rate @
  IF  cr ." rate " dup .id
      dup resource rate @ 0 .r  THEN
  dup resource note @ ?dup IF  @  THEN
  IF  cr ." resource_note " dup .id
      '" emit dup resource note $@ type '" emit  THEN
  drop ;

\ print globals                                        21apr01py

: .iso-date ( n char -- ) base push decimal >r
  0 <# # # r@ hold # # r> hold #s #> type ;

: .prefix ( addr -- ) global prefix $@ type ;
: .files ( addr n suffix n type n -- ) { suffix n1 typ n2 }
  cr typ n2 type ." report "
  over .prefix ." _tasks"  suffix n1 type
  dup   8 and IF  cr ." weekly_" typ n2 type space over .prefix
                  ." _weekly" suffix n1 type  THEN
  dup $10 and IF  cr ." monthly_" typ n2 type space over .prefix
                  ." _monthly" suffix n1 type  THEN
  dup $20 and IF  cr ." slippage_" typ n2 type space
                  over .prefix
                  ." _slippage" suffix n1 type  THEN
  2drop ;

: .globals ( addr -- )
  cr ." startdate " dup global start @ bl .iso-date
  cr ." dateformat iso"
  dup global reports @
  dup   1 and IF  2dup s" .txt"  s" text" .files  THEN
  dup   2 and IF  2dup s" .tex"  s" tex"  .files  THEN
  dup   4 and IF  2dup s" .html" s" html" .files  THEN drop
  dup global shows @
  dup   1 and IF  cr ." show_resource_notes"  THEN
  dup   2 and IF  cr ." show_task_notes"      THEN
  dup   4 and IF  cr ." show_task_ids"        THEN
  dup   8 and IF  cr ." show_milestone_ids"   THEN
  dup $10 and IF  cr ." show_dependencies"    THEN
  dup $20 and IF  cr ." show_vacations"       THEN
  2drop ;

\ print task graph                                     26apr01py

: .task-graph ( global tgraph -- )
  dup task-graph name" @ 0= IF  2drop  EXIT  THEN
  cr ." taskgraph " over global start @ '- .iso-date space
  dup task-graph date @ '- .iso-date space
  swap global prefix $@ type '_ emit
  task-graph name" $@ type ." .eps" cr ;

\ print master TeX file                                22apr01py

: .tex ( graph global -- )
  ." %% OpenSched GUI 0.1 created this file" cr
  ." \documentclass[american]{article}" cr
  ." \usepackage[T1]{fontenc}" cr
  ." \usepackage[latin1]{inputenc}" cr
  ." \usepackage{babel}" cr
  ." \usepackage{supertabular}" cr
  ." \usepackage{graphics}" cr
  ." \begin{document}" cr
  ." \section{Tasks}" cr
  ." \input{" dup global prefix $@ type ." _tasks.tex}" cr
  dup global reports @ $10 and
  IF  ." \section{Monthly}" cr
      ." \input{" dup global prefix $@ type ." _monthly.tex}" cr
  THEN  dup global reports @ 8 and
  IF  ." \section{Weekly}" cr
      ." \input{" dup global prefix $@ type ." _weekly.tex}" cr
  THEN  dup global reports @ $20 and
  IF  ." \section{Slippage}" cr
      ." \input{" dup global prefix $@ type ." _slippage.tex}"
      cr  THEN
  over task-graph name" @ IF
      ." \section{Gantt-Chart}" cr
      ." \resizebox*{1\textwidth}{!}{\includegraphics{"
      dup global prefix $@ type ." _"
      over task-graph name" $@ type ." .eps}}" cr
  THEN
  ." \end{document}" cr
  2drop ;
