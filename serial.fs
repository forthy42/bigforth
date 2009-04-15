\ serial driver                                        09aug99py

[IFDEF] win32
    also DOS legacy off
    kernel32 GetCommState ptr int (int) GetCommState ( handle addr -- r )
    kernel32 SetCommState ptr int (int) SetCommState ( handle addr -- r )
    kernel32 SetCommTimeouts int ptr (int) SetCommTimeouts ( handle addr -- flag )
    kernel32 GetCommTimeouts int ptr (int) GetCommTimeouts ( handle addr -- flag )
    previous

    $80000000 Constant GENERIC_READ
    $40000000 Constant GENERIC_WRITE
    3 Constant OPEN_EXISTING
    
    50 Constant B50
    75 Constant B75
    110 Constant B110
    134 Constant B134
    150 Constant B150
    200 Constant B200
    300 Constant B300
    600 Constant B600
    1200 Constant B1200
    1800 Constant B1800
    2400 Constant B2400
    4800 Constant B4800
    9600 Constant B9600
    19200 Constant B19200
    38400 Constant B38400
    
    struct{
        cell DCBlength
        cell BaudRate
        cell flags
        short wReserved
        short XonLim
        short XoffLim
        byte ByteSize
        byte Parity
        byte StopBits
        byte XonChar
        byte XoffChar
        byte ErrorChar
        byte EofChar
        byte EvtChar
        short wReserved1
    } DCB
    struct{
        cell ReadIntervalTimeout
        cell ReadTotalTimeoutMultiplier
        cell ReadTotalTimeoutConstant
        cell WriteTotalTimeoutMultiplier
        cell WriteTotalTimeoutConstant
    } COMMTIMEOUTS
    
    Create t_old  sizeof DCB allot
    Create t_buf  sizeof DCB allot
    Create tout_buf  sizeof COMMTIMEOUTS allot
    
    : set-baud ( baud fd -- )  >r
        r@ t_old GetCommState drop
        1 t_old DCB flags !
        r@ tout_buf GetCommTimeouts drop
        3 tout_buf COMMTIMEOUTS ReadIntervalTimeout !
        3 tout_buf COMMTIMEOUTS ReadTotalTimeoutMultiplier !
        2 tout_buf COMMTIMEOUTS ReadTotalTimeoutConstant !
        3 tout_buf COMMTIMEOUTS WriteTotalTimeoutMultiplier !
        2 tout_buf COMMTIMEOUTS WriteTotalTimeoutConstant !
        r@ tout_buf SetCommTimeouts drop
        t_old t_buf sizeof DCB move
        t_buf DCB BaudRate !
        8 t_buf DCB ByteSize c!
        r> t_buf SetCommState drop ;
    : reset-baud ( fd -- )
        t_old SetCommState drop ;
[ELSE]
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
    000000000100 Constant CSTOPB
    000000000200 Constant CREAD
    000000004000 Constant CLOCAL
    000000004000 Constant IXANY
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
    legacy off
    
    struct{
    cell c_iflag           /* input mode flags */
    cell c_oflag           /* output mode flags */
    cell c_cflag           /* control mode flags */
    cell c_lflag           /* local mode flags */
    byte c_line            /* line discipline */
    31 string c_cc
    cell c_ispeed          /* input speed */
    cell c_ospeed          /* output speed */
    } termios
    
    Create t_old  sizeof termios allot
    Create t_buf  sizeof termios allot
    
    : set-baud ( baud fd -- )  >r
        t_old r@ tcgetattr drop
        t_old t_buf sizeof termios move
        \  t_buf sizeof termios erase
        [ IGNPAR                   ] Literal    t_buf termios c_iflag !
        0                                       t_buf termios c_oflag !
        [ 0 CS8 or CSTOPB or CREAD or CLOCAL or ] Literal or
        t_buf termios c_cflag !
        0                                       t_buf termios c_lflag !
        1 t_buf termios c_cc VMIN + c!
        0 t_buf termios c_cc VTIME + c!
        28800 t_buf termios c_cflag @ $F and <<
        dup t_buf termios c_ispeed ! t_buf termios c_ospeed !
        t_buf 1 r> tcsetattr drop ;
    
    : reset-baud ( fd -- )
        t_old 1 rot tcsetattr drop ;

    $5409 Constant TCSBRK
    $540B Constant TCFLSH
    $541B Constant FIONREAD
    
    : check-read ( fd -- n )  >r
        0 sp@ FIONREAD r> filehandle @ ioctl drop ;
    
    previous previous
[THEN]
