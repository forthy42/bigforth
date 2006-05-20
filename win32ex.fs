\ handle exceptions in win32                           29sep97py

win32api also

Variable exp

Code throw-except R: -9 # AX mov  A: ' throw rel) jmp
     end-code

Code handle-except ( lp -- ) R:  DX pop  AX pop  DX push
     AX exp A#) mov  cell AX D) AX mov
     ' throw-except A#  0 EX_CONTEXT Eip AX D) mov
     EXCEPTION_CONTINUE_EXECUTION # AX mov  Next end-code

: set-exceptions ( -- )
  SEM_FAILCRITICALERRORS
  SEM_NOGPFAULTERRORBOX  or
  SEM_NOOPENFILEERRORBOX or  SetErrorMode drop
  ['] handle-except SetUnhandledExceptionFilter drop ;

toss forth

