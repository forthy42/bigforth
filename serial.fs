\ serial driver for PS 901 db                                    09aug99py

\ tty settings: 56.7k, 8N1, rtscts

base @ 8 base !
0000001 Constant B50   
0000002 Constant B75   
0000003 Constant B110  
0000004 Constant B134  
0000005 Constant B150  
0000006 Constant B200  
0000007 Constant B300  
0000010 Constant B600  
0000011 Constant B1200 
0000012 Constant B1800 
0000013 Constant B2400 
0000014 Constant B4800 
0000015 Constant B9600 
0000016 Constant B19200
0000017 Constant B38400
000000010001 Constant B57600
000000010002 Constant B115200
020000000000 Constant CRTSCTS
000000000060 Constant CS8
000000000200 Constant CREAD
000000004000 Constant CLOCAL
000000010017 Constant CBAUD
000000000001 Constant IGNBRK
000000000004 Constant IGNPAR
base !

5 Constant VTIME
6 Constant VMIN

also DOS also
legacy on
2 libc tcgetattr tcgetattr
3 libc tcsetattr tcsetattr
2 libc tcflow tcflow
3 libc ioctl ioctl

struct{
  cell c_iflag           /* input mode flags */
  cell c_oflag           /* output mode flags */
  cell c_cflag           /* control mode flags */
  cell c_lflag           /* local mode flags */
  1 c_line               /* line discipline */
  1 c_cc 31 \            /* control characters */
  cell c_ispeed          /* input speed */
  cell c_ospeed          /* output speed */
} termios

Create t_old  sizeof termios allot
Create t_buf  sizeof termios allot

: set-baud ( baud fd -- )  >r
  t_old r@ tcgetattr drop
  t_old t_buf sizeof termios move
\  t_buf sizeof termios erase
  [ IGNBRK IGNPAR or         ] Literal    t_buf termios c_iflag !
  0                                       t_buf termios c_oflag !
  [ CRTSCTS CS8 or CREAD or CLOCAL or ] Literal or
                                          t_buf termios c_cflag !
  0                                       t_buf termios c_lflag !
  1 t_buf termios c_cc VMIN + c!
  0 t_buf termios c_cc VTIME + c!
    28800 t_buf termios c_cflag @ $F and <<
    dup t_buf termios c_ispeed ! t_buf termios c_ospeed !
  t_buf 1 r> tcsetattr drop ;

: reset-baud ( fd -- )
  t_old 1 rot tcsetattr drop ;

$541B Constant FIONREAD

: check-read ( fd -- n )  >r
  0 sp@ FIONREAD r> filehandle @ ioctl drop ;

previous previous
