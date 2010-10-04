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
    57600 Constant B57600
    115200 Constant B115200
    230400 Constant B230400
    460800 Constant B460800
    500000 Constant B500000
    576000 Constant B576000
    921600 Constant B921600
    
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
    also DOS also
    legacy on
    2 libc tcgetattr tcgetattr
    3 libc tcsetattr tcsetattr
    2 libc tcflow tcflow
    3 libc ioctl ioctl
    legacy off
    
    [IFDEF] linux
    struct{
    cell c_iflag           /* input mode flags */
    cell c_oflag           /* output mode flags */
    cell c_cflag           /* control mode flags */
    cell c_lflag           /* local mode flags */
    32 string c_cc         /* line discipline */
    cell c_ispeed          /* input speed */
    cell c_ospeed          /* output speed */
    } termios
    [ELSE]
    struct{
    cell c_iflag           /* input mode flags */
    cell c_oflag           /* output mode flags */
    cell c_cflag           /* control mode flags */
    cell c_lflag           /* local mode flags */
    20 string c_cc         /* line discipline */
    cell c_ispeed          /* input speed */
    cell c_ospeed          /* output speed */
    } termios
    [THEN]

    Create t_old  sizeof termios cell+ allot
    Create t_buf  sizeof termios cell+ allot
    
    [IFDEF] osx
        0 Constant B0
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
	57600 Constant B57600
	115200 Constant B115200
	$00060000 Constant CRTSCTS
	$300 Constant CS8
        $400 Constant CSTOPB
	$800 Constant CREAD
	$8000 Constant CLOCAL
	0 Constant CBAUD
	1 Constant IGNBRK
	4 Constant IGNPAR
	
	17 Constant VTIME
	16 Constant VMIN
	
	$4004667F Constant FIONREAD
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
    0010001 Constant B57600
    0010002 Constant B115200
    0010003 Constant B230400
    0010004 Constant B460800
    0010005 Constant B500000
    0010006 Constant B576000
    0010007 Constant B921600
    0010010 Constant B1000000
    0010011 Constant B1152000
    0010012 Constant B1500000
    0010013 Constant B2000000
    0010014 Constant B2500000
    0010015 Constant B3000000
    0010016 Constant B3500000
    0010017 Constant B4000000
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
    
    $5409 Constant TCSBRK
    $540B Constant TCFLSH
    $541B Constant FIONREAD
    [THEN]
    
    : set-baud ( baud fd -- )  >r
        t_old r@ tcgetattr drop
        t_old t_buf sizeof termios move
        \  t_buf sizeof termios erase
        [ IGNPAR                   ] Literal    t_buf termios c_iflag !
        0                                       t_buf termios c_oflag !
        [ 0 CS8 or CSTOPB or CREAD or CLOCAL or ] Literal [IFDEF] linux or [THEN]
        t_buf termios c_cflag !
        0                                       t_buf termios c_lflag !
        1 t_buf termios c_cc VMIN + c!
        0 t_buf termios c_cc VTIME + c!
    [IFDEF] linux
        28800 t_buf termios c_cflag @ $F and <<
    [THEN]
        dup t_buf termios c_ispeed ! t_buf termios c_ospeed !
        t_buf 1 r> tcsetattr drop ;
    
    : reset-baud ( fd -- )
        t_old 1 rot tcsetattr drop ;

    : check-read ( fd -- n )  >r
        0 sp@ FIONREAD r> filehandle @ ioctl drop ;
    
    previous previous
[THEN]
