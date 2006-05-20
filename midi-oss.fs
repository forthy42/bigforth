\ MIDI player class                                    31dec97py

DOS also Memory also Forth

2 libc ioctl ioctl ( arg fd -- r )
3 libc fcntl fcntl ( arg1 arg2 fd -- r )

[IFUNDEF] midi-player
    include midi-classes.fs
[THEN]

(midi-track class midi-track
    cell var data
    cell var len
    cell var index
    cell var ticks
    cell var cmd

    midi-player ptr parent
how:
    | Create (cmdlen  0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c,
                      2 c, 2 c, 2 c, 2 c, 1 c, 1 c, 2 c, 0 c,
    : cmdlen  ( cmd -- len ) 4 >> (cmdlen + c@ ;

    : set ( parent -- )  bind parent ;
    : dispose ( -- )  data @ IF  data HandleOff  THEN ;

    : dp ( -- addr )  data @ index @ + ;
    : rb ( -- c )  dp c@  1 index +! ;
    : rvl ( -- n )
      0 BEGIN  index @ len @ u>= ?EXIT
               7 <<  rb dup $80 and  WHILE  $7F and or  REPEAT
      $7F and or ;

    : meta-event ( len data -- )
      dup $51 = IF  2drop rb 8 << rb or 8 << rb or
                    parent tempo !  EXIT  THEN
      dup $2F = IF  2drop -1 ticks !  EXIT  THEN
\      dup $58 = IF  drop index +!  EXIT  THEN
\      dup 1 8 within  IF  drop index +!  EXIT  THEN
      drop index +! ;

    : eval-command ( cmd/dev n -- )
        over $F0 and $90 =  IF
            over $F and 7 << dp c@ $7F and +
            dp 1+ c@ $7F and swap parent note-buf @ + c!  THEN
        over $F0 and $80 =  IF
            over $F and 7 << dp c@ $7F and +
            0 swap parent note-buf @ + c!  THEN
        swap parent out  0 ?DO  rb parent out  LOOP ;
    : play ( tick -- tick' )
      dup -1 = ?EXIT
      BEGIN  index @ len @ u>=  IF  -1 ticks !  THEN
             dup ticks @ u>= WHILE
             dp c@ $80 and  IF  rb cmd !  THEN
             cmd @ dup cmdlen dup
             IF    eval-command
             ELSE  drop $FF =
                   IF  rb rvl swap meta-event
             THEN  THEN  rvl ticks +!
      REPEAT  drop ticks @ ;
    : restart ( -- )  index off rvl ticks ! ;
    : assign ( addr u -- )
      dup len ! dup data Handle!
      data @ swap move  index off  rvl ticks ! ;
class;

midi-player implements
\ Constants                                            31dec97py
    | $01 Constant TMR_WAIT_REL
    | $02 Constant TMR_WAIT_ABS
    | $03 Constant TMR_STOP
    | $04 Constant TMR_START
    | $05 Constant TMR_CONTINUE
    | $06 Constant TMR_TEMPO
    | $08 Constant TMR_ECHO
    | $09 Constant TMR_CLOCK
    | $0A Constant TMR_SPP
    | $0B Constant TMR_TIMESIG

    | $05 Constant SEQ_MIDIPUTC
    | $0B Constant SEQ_BALANCE
    | $0C Constant SEQ_VOLMODE

    | $80 Constant EV_SEQ_LOCAL
    | $81 Constant EV_TIMING
    | $92 Constant EV_CHN_COMMON
    | $93 Constant EV_CHN_VOICE    
    | $94 Constant EV_SYSEX

    | $FE Constant SEQ_PRIVATE
    | $FF Constant SEQ_EXTENDED

    | $80 Constant MIDI_NOTEOFF
    | $90 Constant MIDI_NOTEON
    | $A0 Constant MIDI_KEY_PRESSURE
    | $B0 Constant MIDI_CTL_CHANGE
    | $C0 Constant MIDI_PGM_CHANGE
    | $D0 Constant MIDI_CHN_PRESSURE
    | $E0 Constant MIDI_PITCH_BEND
    | $F0 Constant MIDI_SYSTEM_PREFIX

    | 0 'Q 8 >> or Constant SNDCTL_SEQ_RESET
    | 1 'Q 8 >> or Constant SNDCTL_SEQ_SYNC

    | Create gm_reset  5 c, $7E c, $7F c, $09 c, $01 c, $F7 c,

\ write bytes/words/longs out                          31dec97py
    : sync-wait ( -- )  ticks @ 0= ?EXIT
      start-time @ real-ticks 2@
      precision @ 0=
      IF    division @ um/mod nip &5 / 1- &50 * ms>time 
      ELSE  division @ &10 ms>time / um/mod nip  THEN
      + till ;
    : sync ( -- )  buffer @ len @ seq @ fwrite drop
      sync-wait len off ;
    : need ( n -- )  len @ + maxlen @ > IF  sync  THEN ;
    : sc, ( c -- )  buffer @ len @ + c! 1 len +! ;
    : sw, ( w -- )  buffer @ len @ + w! 2 len +! ;
    : s,  ( x -- )  buffer @ len @ +  ! 4 len +! ;

    : patch-buf ( n -- addr )  note-buf @ + ;
    
\ basic commands                                       01jan98py
    : out,    ( byte dev -- )  pause
      4 need  SEQ_MIDIPUTC sc, swap sc, sc, 0 sc, ;

    : out     ( byte -- )  0 out, ;
    : pressure, ( vel chn dev -- )
      >r MIDI_CHN_PRESSURE + r@ out, r> out, ;
    : start,  ( vol note chn dev -- )
      >r MIDI_NOTEON  + r@ out, r@ out, r> out, ;
    : stop,   ( vol note chn dev -- )
      >r MIDI_NOTEOFF + r@ out, r@ out, r> out, ;
    : key,    ( pressure note chn dev -- )
      >r MIDI_KEY_PRESSURE + out, r@ out, r> out, ;
    : seq_patch, ( pgn chn dev -- )
      >r MIDI_PGM_CHANGE + r@ out, r> out, ; 
    : seq_control, ( value controller chn dev -- )
      >r MIDI_CTL_CHANGE + r@ out, r@ out, r> out, ;
    : seq_pitch, ( value chn dev -- )
      >r MIDI_PITCH_BEND r@ out,
      dup $FF and r@ out, 8 >> r> out, ;

    : sysex,  ( buf len dev -- )
      MIDI_SYSTEM_PREFIX over out, -rot bounds
      ?DO  I c@ over out,  LOOP  drop ;

\ timer commands                                       31dec97py
    : timer, ( parm ev -- )
      8 need EV_TIMING sc, sc, 0 sw, s, ;

    : tstart, ( -- ) 0 TMR_START     timer, ;
    : tstop,  ( -- ) 0 TMR_STOP      timer, ;
    : tcont,  ( -- ) 0 TMR_CONTINUE  timer, ;
    : twait,  ( n -- ) TMR_WAIT_ABS  timer, ;
    : tdelta, ( n -- ) TMR_WAIT_REL  timer, ;
    : echo,   ( key -- ) TMR_ECHO    timer, ;
    : tempo,  ( n -- ) TMR_TEMPO     timer, ;
    : tsig,   ( sig -- ) TMR_TIMESIG timer, ;

\ split file into tracks                               03jan98py

    : mfb ( -- addr )  filebuf @ fileptr @ + ;
    : rb ( -- c )  mfb c@  1 fileptr +! ;
    : rw ( -- w )  rb   8 << rb or ;
    : rl ( -- l )  rw $10 << rw or ;
    : read-tracks ( tracklen format ntrks division -- )
      link[] track @ IF  dispose[] track  THEN
      over midi-track new[] bind[] track
      over 0 DO  ^ i track set  LOOP
      &10000 * division ! dup tracks !
      0 ?DO  rl 'MTrk <> ?LEAVE
             rl mfb over  I track assign
             fileptr +!
      LOOP 2drop ;
    : >tracks ( -- )
      rl dup 'RIFF = IF  drop $10 fileptr +! rl  THEN
      dup 'MThd = IF  drop rl rw rw rw read-tracks  EXIT  THEN ;

\ high level commands                                  31dec97py

    : init ( -- )  $400 buffer Handle! $400 maxlen !
      $800 note-buf Handle!  note-buf @ $800 erase
      &500000 tempo ! ;
    : seq-open ( -- )
      0" /dev/sequencer" w/o fopen dup 0< IF  ior throw  THEN
      seq !  SNDCTL_SEQ_RESET seq @ ioctl drop
      $10 0 DO  0 i 0 seq_patch,  LOOP  sync ;
    : seq-close ( -- )  seq @ ?dup
      IF  tstop, gm_reset count 0 sysex, sync
          SNDCTL_SEQ_RESET over ioctl drop
          $800 4 2 pick fcntl drop
          fclose drop  seq off
      THEN ;

    : file ( addr u -- )
      stop
      r/o open-file throw >r
      r@ filesize @ dup filelen ! filebuf Handle!
      filebuf @ filelen @ r@ read-file throw drop
      fileptr off  r> close-file throw
      >tracks filebuf HandleOff filelen off ;

    : dispose ( -- )  stop
      filebuf @ IF  filebuf HandleOff  THEN
      buffer HandleOff
      note-buf HandleOff
      seq @ fclose throw ;

\ playing                                              31dec97py

    : start ( -- )
        stop
        $1000 dup NewTask activate
        up@ midi-task !
        seq-open  playing on
        gm_reset count 0 sysex,
        BEGIN  ticks @ -1 <> playing @ and  WHILE
            &50 play-delta
        REPEAT
        seq-close
        ticks off start-ticks off start-time off
        tracks @ 0 DO  i track restart  LOOP
        midi-task off ;
    : stop ( -- ) playing off
      BEGIN  midi-task @ WHILE  &20 wait  REPEAT
      note-buf @ $800 erase ;

    : play ( tick -- tick' )
      -1 tracks @
      0 ?DO  over I track play umin  LOOP  nip ;
    : play-delta ( n -- )
      seq @ 0= IF  seq-open  THEN
      ticks @ 0= IF  gm_reset count 0 sysex,
                     0 play dup start-ticks ! ticks !
                     0. real-ticks 2!
                     timer@ start-time ! tstart, sync  THEN
      ticks @ swap real-ticks 2@ division @ um/mod nip + >r
      BEGIN  play dup -1 <>
      WHILE  real-ticks 2@ division @ um/mod nip r@ u<
      WHILE  dup ticks @ -
             real-ticks 2@ division @ um/mod nip >r
             tempo @ um* real-ticks 2@ d+ real-ticks 2!
             real-ticks 2@ division @ um/mod nip
             dup r> u> IF  twait,  ELSE  drop  THEN
             dup ticks ! sync
             pause
      REPEAT  ELSE  dup ticks !  THEN  drop rdrop sync ;
class;

previous previous Forth
