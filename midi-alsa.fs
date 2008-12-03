\ Midi player class for ALSA sequencer                 24jun02py

[IFUNDEF] midi-player
    include midi-classes.fs
[THEN]

\ ALSA libasound interface                             06jul02py

Vocabulary alsa-midi
also dos also memory also alsa-midi definitions

struct{
byte type         /* event type */
byte flags        /* event flags */
byte tag          /* arbitrary tag */
byte queue        /* schedule queue */
cell time_s    /* schedule time */
cell time_ns
short source       /* source address: client, port */
short dest         /* destination address: client, port */
40 string data        /* event data... */
} snd_seq_event


struct{
        cell client                     /* client number to inquire */
        cell type                       /* client type */
        1 name 63 \                     /* client name */
        cell filter                     /* filter flags */
        1 multicast_filter 7 \          /* multicast filter bitmap */
        1 event_filter 31 \             /* event filter bitmap */
        cell num_ports                  /* RO: number of ports */
        cell event_lost                 /* number of lost events */
        64 \                            /* for future use */
} snd_seq_client_info

struct{
        1 client   1 port               /* client/port numbers */
        1 name 63 \                     /* port name */
        cell capability                 /* port capability bits */
        cell type                       /* port type bits */
        cell midi_channels              /* channels per MIDI port */
        cell midi_voices                /* voices per MIDI port */
        cell synth_voices               /* voices per SYNTH port */

        cell read_use                   /* R/O: subscribers for output (from thi
s port) */
        cell write_use                  /* R/O: subscribers for input (to this p
ort) */

        cell kernel                     /* reserved for kernel use (must be NULL
) */
        cell flags                      /* misc. conditioning */
        1 time_queue                    /* queue # for timestamping */
        59 \                            /* for future use */
} snd_seq_port_info

Create cinfo  sizeof snd_seq_client_info allot
Create pinfo  sizeof snd_seq_port_info allot
	    
library libasound libasound.so.2

legacy off

libasound open int int int int (int) snd_seq_open
 ( *handle name streams mode -- f )
libasound close int (int) snd_seq_close ( handle -- flag )
libasound connect_to int int int int (int) snd_seq_connect_to
 ( seq port client port -- f )
libasound client_id int (int) snd_seq_client_id ( handle -- id )
libasound create_simple_port int int int int
 (int) snd_seq_create_simple_port ( handle name caps type -- f )
libasound set_output_buffer_size int int
 (int) snd_seq_set_output_buffer_size ( handle size -- f )
libasound alloc_named_queue int int
 (int) snd_seq_alloc_named_queue ( handle name -- queue )
libasound set_client_name int int
 (int) snd_seq_set_client_name ( handle name -- f )
libasound event_output_direct int int
 (int) snd_seq_event_output_direct ( handle ev -- f )
libasound event_output_buffer int int
 (int) snd_seq_event_output_buffer ( handle ev -- f )
libasound drain_output int
 (int) snd_seq_drain_output ( handle -- f )
libasound query_next_client int int
 (int) snd_seq_query_next_client ( handle info -- f )
libasound query_next_port int int
 (int) snd_seq_query_next_port ( handle info -- f )

libasound queue_tempo_sizeof
 (int) snd_seq_queue_tempo_sizeof ( -- size )
libasound queue_tempo_set_tempo int int
 (void) snd_seq_queue_tempo_set_tempo ( queue tempo -- )
libasound queue_tempo_set_ppq int int
 (void) snd_seq_queue_tempo_set_ppq ( queue ppq -- )
libasound set_queue_tempo int int int
 (int) snd_seq_set_queue_tempo ( handle q tempo -- f )
libasound get_queue_tempo int int int
 (int) snd_seq_get_queue_tempo ( handle q tempo -- f )

\ Midi player implementation                           06jul02py

also memory

&17 Value wavetable
&00 Value waveport

midi-player implements
  Create ev     here sizeof snd_seq_event dup allot erase
  Create _tempo here queue_tempo_sizeof   dup allot erase

  : open-seq  seq 0" hw" 3 0 open drop ;
  : sync-wait ( -- )  ticks @ 0= ?EXIT
      start-time @ real-ticks 2@
      precision @ 0=
      IF    division @ um/mod nip &5 / 1- &50 * ms>time 
      ELSE  division @ &10 ms>time / um/mod nip  THEN
      + till ;
  : sync seq @ drain_output drop sync-wait ;
  : create-port ( -- ) seq @ 0" Midi player" $63 2
    create_simple_port port ! ;
  : connect-seq seq @ port @ wavetable waveport connect_to drop ;
  : hex. ( -- )  base push hex . ;
  : list-ports ( -- )  cinfo @ pinfo c! -1 pinfo 1+ c!
      BEGIN  seq @ pinfo query_next_port  0= WHILE
	      pinfo snd_seq_port_info client c@ .
	      pinfo snd_seq_port_info port c@ . ."  '" 
	      pinfo snd_seq_port_info name >len type ." ' "
	      pinfo snd_seq_port_info capability @ hex.
	      pinfo snd_seq_port_info type @ hex.
	      pinfo snd_seq_port_info capability @ $42 = \ player
	      wavetable 0= waveport 0= and and IF
		  pinfo snd_seq_port_info client c@ to wavetable
		  pinfo snd_seq_port_info port c@ to waveport
		  '* emit
	      THEN cr
      REPEAT ;
  : list-clients ( -- )  cinfo off  0 to wavetable  0 to waveport
      BEGIN  seq @ cinfo query_next_client  0= WHILE
	      cinfo snd_seq_client_info client ?
	      cinfo snd_seq_client_info num_ports ?
	      cinfo snd_seq_client_info name >len type cr
	      list-ports
      REPEAT ;
  : search-ports ( -- )  cinfo @ pinfo c! -1 pinfo 1+ c!
      BEGIN  seq @ pinfo query_next_port  0= WHILE
	      pinfo snd_seq_port_info capability @ $42 = \ player
	      wavetable 0= waveport 0= and and IF
		  pinfo snd_seq_port_info client c@ to wavetable
		  pinfo snd_seq_port_info port c@ to waveport
	      THEN
      REPEAT ;
  : search-clients ( -- )  cinfo off  0 to wavetable  0 to waveport
      BEGIN  seq @ cinfo query_next_client  0= WHILE
	      search-ports
      REPEAT ;
  : set-ev-header ( command -- )
              [ ev snd_seq_event type      ] ALiteral c!
      port @  [ ev snd_seq_event source 1+ ] ALiteral c!
      $FE     [ ev snd_seq_event dest      ] ALiteral c!
      $FD     [ ev snd_seq_event dest   1+ ] ALiteral c!
      queue @ [ ev snd_seq_event queue     ] ALiteral c!
      ticks @ [ ev snd_seq_event time_s    ] ALiteral !
    ( direct out ) ;
  : alloc-queue  seq @ 0" bigFORTH" alloc_named_queue queue ! ;
  : set-name seq @ 0" bigFORTH Midi player"
    set_client_name drop ;
  : 64k-buffer  seq @ $10000 set_output_buffer_size drop ;
  : seq-open ( -- )
      open-seq create-port
      alloc-queue set-name 64k-buffer
      search-clients connect-seq ;
  : seq-close ( -- ) seq @ close drop seq off ;

\ Timer and tempo                                      06jul02py

  : timer, ( clock -- ) dup ticks !
      &33 set-ev-header
      $00     [ ev snd_seq_event dest      ] ALiteral c!
      $00     [ ev snd_seq_event dest   1+ ] ALiteral c!
              [ ev snd_seq_event data      ] ALiteral !
      seq @ ev event_output_direct drop
      &31     [ ev snd_seq_event type      ] ALiteral c!
              [ ev snd_seq_event data      ] ALiteral off
      seq @ ev event_output_direct drop ;
  : tempo, ( tempo -- )  sync dup tempo !
      _tempo swap queue_tempo_set_tempo
      seq @ queue @ _tempo set_queue_tempo drop ;

\ Note control                                         06jul02py
  
  : note, ( vol note chn type -- ) set-ev-header
      [ ev snd_seq_event data ] ALiteral c!+ c!+ c!
      seq @ ev event_output_buffer drop ;
  : ctrl, ( value ctrl chn type -- ) set-ev-header
      [ ev snd_seq_event data ] ALiteral c!+ 3+ !+ !
      seq @ ev event_output_buffer drop ;
  
  : start,  ( vol note chn -- )
      over2 over2 over2 7 << + note-buf @ + c! 6 note, ;
  : stop,   ( vol note chn -- )
      0     over2 over2 7 << + note-buf @ + c! 7 note, ;
  : key,    ( vol note chn -- )       8 note, ;
  : seq_control, ( program ctrl chn -- ) &10 ctrl, ;
  : seq_patch,   ( program chn -- )   0 swap &11 ctrl, ;
  : seq_press,   ( value chn -- )     0 swap &12 ctrl, ;
  : seq_pitch,   ( value chn -- )     0 swap &13 ctrl, ;
  : seq_ctrl,    ( value chn -- )     0 swap &14 ctrl, ;

\ sysex                                                06jul02py
  
  | Create sysexbuf  $F0 c, $100 allot
  : sysex, ( buf len -- ) tuck
      sysexbuf 1+ swap move sysexbuf swap 1+
      &130 set-ev-header
      [ ev snd_seq_event data ] ALiteral noop !+ !
      seq @ ev event_output_direct drop ;

\ Timer commands                                       06jul02py
  
  : start-timer ( bpm ticks -- )
      _tempo rot queue_tempo_set_ppq tempo, 0 timer, ;
  : tstart, ( -- ) division @ &10000 / tempo @ start-timer ;
  : tstop,  ( -- )
      &32     [ ev snd_seq_event type      ] ALiteral c!
      port @  [ ev snd_seq_event source 1+ ] ALiteral c!
      $00     [ ev snd_seq_event dest      ] ALiteral c!
      $00     [ ev snd_seq_event dest   1+ ] ALiteral c!
      queue @ [ ev snd_seq_event queue     ] ALiteral c!
              [ ev snd_seq_event data      ] ALiteral off
      seq @ ev event_output_direct drop ;
  : twait, ( n -- )  drop ;
class;

\ Midi Track implementation                            06jul02py

(midi-track class midi-track
    cell var data
    cell var len
    cell var index
    cell var ticks
    cell var cmd

    midi-player ptr parent
how:
\ data handling                                        06jul02py

    : set ( parent -- )  bind parent ;
    : dispose ( -- )  data @ IF  data HandleOff  THEN ;

    : dp ( -- addr )  data @ index @ + ;
    : rb ( -- c )  dp c@  1 index +! ;
    : rvl ( -- n )
      0 BEGIN  index @ len @ u>= ?EXIT
               7 << rb dup $80 and  WHILE  $7F and or  REPEAT
      $7F and or ;

\ Meta event                                           06jul02py

    : meta-event ( len data -- )
      dup $51 = IF  2drop rb 8 << rb or 8 << rb or
                    parent tempo,  EXIT  THEN
      dup $2F = IF  drop -1 ticks ! index +!  EXIT  THEN
\      dup $03 = IF  drop 0 ?DO  rb emit  LOOP  cr  EXIT  THEN  
\      dup $7F = IF  drop 0 ?DO  rb emit  LOOP  cr  EXIT  THEN  
\      dup $58 = IF  drop index +!  EXIT  THEN
\      dup 1 8 within  IF  drop index +!  EXIT  THEN
      drop index +! ;

\ note events                                          06jul02py

    : note-off ( chn -- )  >r rb rb swap r> parent stop, ;
    : note-on  ( chn -- )  >r rb rb swap r> parent start, ;
    : key      ( chn -- )  >r rb rb swap r> parent key, ;
    : control  ( chn -- )  >r rb rb r> parent seq_control, ;
    : patch    ( chn -- )  rb swap parent seq_patch, ;
    : press    ( chn -- )  rb swap parent seq_press, ;
    : pitch    ( chn -- )  >r rb rb 7 << or $2000 - r>
      parent seq_pitch, ;
    : ctrl     ( chn -- ) dup 0= IF  drop
            rb dp over parent sysex, index +!  EXIT  THEN
        rb swap parent seq_ctrl, ;

\ Eval commands                                        06jul02py

    Create eval-table
    T] note-off note-on key control patch press pitch ctrl [
    : eval-command ( cmd/dev -- )
        dup $F and swap 4 >> 7 and cells eval-table + perform ;
    : play ( tick -- tick' )
      dup -1 = ?EXIT
      BEGIN  index @ len @ u>=  IF  -1 ticks !  THEN
             dup ticks @ u>= WHILE
             dp c@ $80 and  IF  rb cmd !  THEN
             cmd @ dup $80 $FF within
             IF    eval-command
             ELSE  $FF =
                   IF  rb rvl swap meta-event
             THEN  THEN  rvl ticks +!
      REPEAT  drop ticks @ ;
    : restart ( -- )  index off rvl ticks ! ;
    : assign ( addr u -- )
      dup len ! dup data Handle!
      data @ swap move  index off  rvl ticks ! ;
class;

midi-player implements
    : mfb ( -- addr )  filebuf @ fileptr @ + ;
    : rb ( -- c )  mfb c@  1 fileptr +! ;
    : rw ( -- w )  rb   8 << rb or ;
    : rl ( -- l )  rw $10 << rw or ;
    s" krTM" drop @ Constant 'MTrk
    s" dhTM" drop @ Constant 'MThd
    s" FFIR" drop @ Constant 'RIFF
    : read-tracks ( tracklen format ntrks division -- )
        &10000 * division !
        link[] track @ IF  dispose[] track  THEN
        dup tracks !
        dup midi-track new[] bind[] track
        dup 0 DO  ^ i track set  LOOP
        0 ?DO  rl 'MTrk <> ?LEAVE
            rl mfb over  I track assign
            fileptr +!
        LOOP 2drop ;
    : >tracks ( -- )
        rl dup 'RIFF = IF  drop $10 fileptr +! rl  THEN
        dup 'MThd = IF  drop rl rw rw rw read-tracks  EXIT
        THEN  drop ;
    : patch-buf ( n -- addr )  note-buf @ + ;

\ playing                                              31dec97py

    : start ( -- )
        stop
        $1000 dup NewTask activate
        up@ midi-task !
        seq-open  playing on
        division @ &10000 / tempo @ start-timer
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

\ Playing                                              06jul02py

    : play ( tick -- tick' )
      -1 tracks @
      0 ?DO  over I track play umin  LOOP  nip ;
    : play-delta ( n -- )
      seq @ 0= IF  seq-open  THEN
      ticks @ 0= IF
          0 play dup start-ticks ! ticks !
          0. real-ticks 2!
          timer@ start-time !
          tstart, sync  THEN
      ticks @ swap real-ticks 2@ division @ um/mod nip + >r
      BEGIN  play dup -1 <>
      WHILE  real-ticks 2@ division @ um/mod nip r@ u<
      WHILE  dup ticks @ -
             real-ticks 2@ division @ um/mod nip >r
             tempo @ um* real-ticks 2@ d+ real-ticks 2!
             real-ticks 2@ division @ um/mod nip
             dup r> u> IF  twait,  ELSE  drop  THEN
             dup ticks ! sync pause
      REPEAT  ELSE  dup ticks !  THEN  drop rdrop sync ;

\ high level commands                                  31dec97py

    : init ( -- )
        $800 note-buf Handle!  note-buf @ $800 erase
        &500000 tempo ! ;
    : file ( addr u -- )
        stop
        r/o open-file throw >r
        r@ filesize @ dup filelen ! filebuf Handle!
        filebuf @ filelen @ r@ read-file throw drop
        fileptr off  r> close-file throw
        >tracks filebuf HandleOff filelen off ;
    : dispose ( -- )  stop
        link[] track @ IF  dispose[] track  THEN
        filebuf @ IF  filebuf HandleOff  THEN
        note-buf HandleOff ;

class;

previous previous previous previous definitions
