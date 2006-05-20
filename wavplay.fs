\ Sound -- wavplay
\ v1.2 Beu 8-Oct-2001  
 
forth definitions decimal

variable sound-fd
: *load-sound  ( ca u -- )   r/o bin open-file throw  sound-fd ! ;

: sound-size  ( -- u )  sound-fd @ file-size throw drop ;

variable sound-md
: load-sound  ( ca- u - )  *load-sound
  sound-size allocate throw sound-md ! 
  sound-md @ sound-size sound-fd @ read-file throw drop ;

\ looking for the chunks in RIFF data
: length  ( a1 -- a2 u ) 8 + dup 4 - @ ; macro
: 'fmt  ( -- a u )
  sound-md @ 12 + begin dup @ $20746d66 xor while length + repeat length ; 
: 'data ( -- a u )
  sound-md @ 12 + begin dup @ $61746164 xor while length + repeat length ; 

\ get WAVE parameters
: @channels  ( -- u )  'fmt drop 2 + w@ ;
: @speed     ( -- u )  'fmt drop 4 + @ ;
: @avg/sec   ( -- u )  'fmt drop 8 + @ ;
: @bits      ( -- u )  @avg/sec 8 @speed @channels * */ ;
: @size      ( -- u )  'data nip ;


\ Documentation:
\   man ioctl_list
\   /usr/include/linux/soundcard.h
also dos
legacy off
libc ioctl int int int (int) ioctl  ( d request *argp -- rc )

variable dsp-fd
variable stereo
variable format
variable speed
: [dsp  s" /dev/dsp" w/o bin open-file throw dup dsp-fd !
  filehandle @
  dup $C0045003 ( SNDCTL_DSP_STEREO) stereo ioctl drop
  dup $C0045005 ( SNDCTL_DSP_SETFMT) format ioctl drop 
      $C0045002 ( SNDCTL_DSP_SPEED)  speed  ioctl drop ;
: dsp]  dsp-fd @ close-file throw ; 
: play  ( a u -- )  dsp-fd @ write-file throw ;


: .info  (  ca u -- )
  cr ." Pathname:       " type
  cr ." Device:         /dev/dsp"
  cr ." Sampling Rate:  " @speed u. ." Hz"
  cr ." Mode:           " @channels 1 - if ." Stereo" else ." Mono" then
  cr ." Samples:        " @size @speed @avg/sec */ u.
  cr ." Bits:           " @bits u. cr ;


\ play RIFF-WAVE file
: wavplay  ( ca u -- )  2dup load-sound .info
  @channels 1- stereo !  @speed speed !  @bits format !
  [dsp 'data play dsp]   sound-md @ free throw ;


\ s" wav/Der Microsoft-Sound.wav" wavplay
\ s" wav/chimes.wav" wavplay
\ s" wav/chord.wav" wavplay
\ s" wav/ding.wav" wavplay
\ s" wav/logoff.wav" wavplay
\ s" wav/notify.wav" wavplay
\ s" wav/recycle.wav" wavplay
\ s" wav/start.wav" wavplay
\ s" wav/tada.wav" wavplay
\ s" wav/thinktwice.wav" wavplay
