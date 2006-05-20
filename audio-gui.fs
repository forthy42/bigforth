\ Audio GUI
\
\ $Id: audio-gui.fs 198 2006-05-17 15:18:30Z berndp $

: we ( -- ) ;
: wd ( -- ) ;

: spi! ( data addr -- ) 2drop ;
: spi@ ( addr -- data ) drop -1 ;

: spiw! ( data addr -- ) 2drop ;
: spiw@ ( addr -- data ) drop $FFFF ;
