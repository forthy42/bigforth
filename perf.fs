also dos

0 [IF]

: make-file ( -- )
    s" xxx" r/w create-file throw
    &1000000 0 ?do
        s" test line" 2 pick write-line throw
    loop
    close-file throw ;

make-file
[THEN]

create buf 1100 chars allot

: read-files ( ubufsize -- )
    s" xxx" r/o open-file throw >r
    begin
        buf over r@ read-file throw
        0= until
    drop r> close-file throw ;

: read-lines ( ubufsize -- )
    s" xxx" r/o open-file throw >r
    begin
        buf over r@ read-line throw nip
        0= until
    drop r> close-file throw ;

\ *** NT 4.0 on P55-166 MHz

: PERF ( -- )
    cr !time  1024 read-files  .time
    cr !time    10 read-files  .time
    cr !time  1024 read-lines  .time
    cr !time    10 read-lines  .time
    cr !time     9 read-lines  .time
    
    cr !time  1100 read-files  .time
    cr !time    11 read-files  .time
    cr !time  1100 read-lines  .time
    cr !time    11 read-lines  .time
    cr !time    10 read-lines  .time ;

perf

previous
