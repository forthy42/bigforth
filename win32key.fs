also DOS

1 kernel32 GetStdHandle GetStdHandle ( hID -- handle )
5 kernel32 ReadFile ReadFile ( oLap pNread max buf handle -- bool )
6 kernel32 PeekNamedPipe PeekNamedPipe ( pN pN pN max buf handle --
bool )

-10 Constant STD_INPUT_HANDLE
-11 Constant STD_OUTPUT_HANDLE
-12 Constant STD_ERROR_HANDLE

STD_INPUT_HANDLE GetStdHandle VALUE stdin

variable nRead
variable myC
: myKey ( -- c )
  0 nRead 1 myC stdin ReadFile IF nRead @ 0= throw  myC @ ELSE KEY
THEN ;

: myKey? ( -- tf )    \ nRead: number of available characters in pipe
  0 nRead 0 0 0 stdin PeekNamedPipe IF nRead @ 0<> ELSE KEY? THEN ;

toss forth

\ ...which is used in by my custom eval loop primitives (no echo, no
edit):

: readLineToPad ( -- n )
  pad begin
    myKey dup 10 <> while
    dup 13 <> if over c! char+ else drop then
  repeat drop pad - ;

: evalALine
  readLineToPad    \ gforth: pad 256 stdin read-line throw 0= throw
  pad swap  2dup + 0 swap c!      \ terminate for >number in evaluate
  ['] evaluate CATCH drop ;
