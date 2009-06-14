\ OpenSched GUI load/store part                        15apr01py

: search-job ( addr u -- job )
  current-widget @ opensched with jobs @ endwith  >r
  BEGIN  r@  WHILE  2dup r@ job id $@ compare  WHILE  r> @ >r
         REPEAT  THEN
  r@ 0= IF  rdrop new-job dup
        current-widget @ opensched with jobs endwith append!
  ELSE  2drop r>  THEN ;

: search-resource ( addr u -- resource )
  current-widget @ opensched with resources @ endwith  >r
  BEGIN  r@ WHILE  2dup r@ resource id $@ compare  WHILE
         r> @ >r  REPEAT  THEN
  r@ 0= IF  rdrop new-resource dup
     current-widget @ opensched with resources endwith append!
  ELSE  2drop r>  THEN ;

: job:      bl word count search-job ;
: resource: bl word count search-resource ;

\ add string/number                                    15apr01py

: add" ( addr -- )
  '" parse 2drop '" parse rot $! ;
: add# ( addr -- )  base push decimal
  bl word count s>number drop swap ! ;
: add#2 ( addr -- )  base push decimal
  bl word count s>number drop
  dpl @ 0 max 2 min 2 ?DO  #10 *  LOOP  swap ! ;
: add-list ( addr -- )
  BEGIN  bl word count  dup WHILE
         new-list over append!  REPEAT  2drop drop ;

\ job/resource generation                              15apr01py

Variable dummy
: globals ( -- addr )
  current-widget @ opensched with globals endwith ;
: report ( -- )  bl word count 2dup '_ scan nip -
  globals global prefix $! ;
: or! ( n addr -- ) dup >r @ or r> ! ;
: show: ( n -- )  Create ,
  DOES> @ globals global shows or! ;

Vocabulary scanner  also scanner definitions

: startdate ( -- )
  dummy add#  dummy @ #100 *
  dummy add#  dummy @ + #100 *
  dummy add#  dummy @ + 
  globals global start ! ;

: dateformat ( -- )  bl word drop ; \ must be "iso"

\ reports                                              22apr01py

: textreport ( -- ) report 1 globals global reports or! ;
: texreport  ( -- ) report 2 globals global reports or! ;
: htmlreport ( -- ) report 4 globals global reports or! ;

: weekly_tex   ( -- )   8 globals global reports or!
  postpone \ ;
: monthly_tex  ( -- ) $10 globals global reports or!
  postpone \ ;
: slippage_tex ( -- ) $20 globals global reports or!
  postpone \ ;

' weekly_tex Alias weekly_text
' weekly_tex Alias weekly_html
' monthly_tex Alias monthly_text
' monthly_tex Alias monthly_html
' slippage_tex Alias slippage_text
' slippage_tex Alias slippage_html

  1 show: show_resource_notes
  2 show: show_task_notes
  4 show: show_task_ids
  8 show: show_milestone_ids
$10 show: show_dependencies
$20 show: show_vacations

\ tasks                                                22apr01py

: task      job: >r  r@ job name" add" r> job days add# ;
: describe  job: job description add" ;
: complete  job: job complete add# ;
: candidate job: job candidate add-list ;
: depends   job: job depends add-list ;

: efficiency    resource: resource efficiency add# ;
: rate          resource: resource rate add# ;
: resource_note resource: resource note add" ;
: resource      resource: resource name" add" ;

\ task graph                                           26apr01py

: taskgraph ( -- )
  current-widget @ opensched with graph endwith  >r
  bl word drop \ start date - stub
  bl word count 2dup bounds ?DO
     I c@ '- = IF  '. I c!  THEN  LOOP
  s>number drop  r@ task-graph date !
  bl word count '_ scan '_ skip '. -scan '. -skip
  r> task-graph name" $! ;

previous definitions

\ read in .sched file                                  22apr01py

: sched-load ( addr u -- )
  r/w open-file throw $200 input-file
  BEGIN  refill WHILE  '# parse 2drop >in @ #tib ! >in off
\         cr .s ." |" source type
         interpret  REPEAT
  loadfile @ close-file throw ;

: include-sched ( addr u -- )
  only previous scanner also
  ['] sched-load catch IF  2drop  THEN ;

