\ Error messages in Klartext                           26may95py

err$s syserrs
$03 c, ," Tight stack"
$04 c, ," Stack empty!"
$05 c, ," RS full"
$06 c, ," RS empty"
$07 c, ," LoopS full"
$08 c, ," Dictionary full"
$09 c, ," Bus Error !"
$0A c, ," Division by Zero !"
$0B c, ," Result out of range"
$0C c, ," wrong argument"
$0D c, ," don't know"
$0E c, ," compile only"
$0F c, ," protected"
$10 c, ," invalid name"
$11 c, ," holdV"
$12 c, ," parseV"
$13 c, ," too long name"
$14 c, ," Illegal Instruction !"
$15 c, ," can't do"
$16 c, ," unstructured"
$17 c, ," Address Error !"
$18 c, ," invalid number"
$19 c, ," RS imbalance"
$1A c, ," no loop pars"
$1B c, ," don't recurse"
$1C c, ," User interrupt"
$1D c, ," compiler nesting"
$1E c, ," out of use"
$1F c, ," no body!"
$20 c, ," can't tick"
$21 c, ," read error"
$22 c, ," write error"
$23 c, ," out of range!"
$24 c, ," out of file!"
$25 c, ," file I/O error"
$26 c, ," file not found"
$27 c, ," unexpected eof"
$28 c, ," wrong base"
$29 c, ," loss of precision"
$2A c, ," FP divide by zero"
$2B c, ," FP overflow"
$2C c, ," FP stack full"
$2D c, ," FP stack empty"
$2E c, ," FP invalid argument"
$2F c, ," don't compile"
$31 c, ," no more ALSO"
$32 c, ," no more TOSS"
$00 c, ," Unkown Error     "
align
' syserrs error$s 0 cells + !

[IFDEF] unix
dos 1 libc strsignal strsignal
dos 1 libc strerror strerror
forth

Create strerrbuf $80 allot

: strerror$ ( n -- )
  strerror >len strerrbuf place  strerrbuf "error ! ;
: strsignal$ ( n -- )
  strsignal >len strerrbuf place  strerrbuf "error ! ;

' strsignal$ error$s 1 cells + !
' strerror$  error$s 4 cells + !
[THEN]

err$s memerr$
$01 c, ," Not enough memory!"
$02 c, ," Invalid Address!"
$03 c, ," Invalid Handle!"
$04 c, ," SetPtrSize not possible!"
$00 c, ," Internal Error     "
' memerr$ error$s 2 cells + !

[IFDEF] go32
err$s go32errs
$01 c, ," inv. function code"    
$02 c, ," file not found"
$03 c, ," path not found"        
$04 c, ," too many open files"
$05 c, ," access denied"         
$06 c, ," invalid handle"
$07 c, ," out of memory"         
$08 c, ," insufficient memory"
$09 c, ," inv. segment selector" 
$0A c, ," inv. environment"
$0B c, ," inv. file format"      
$0C c, ," inv. file access code"
$0D c, ," inv. data"
$0F c, ," inv. drive id"
$11 c, ," not same device"       
$12 c, ," no matching dir entry"
$21 c, ," already locked"        
$50 c, ," file already exists"
$82 c, ," not supported under this DPMI implementation"
$FF c, ," inv. drive number"
$00 c, ," domain error     "
' go32errs error$s 4 cells + !
[THEN]
