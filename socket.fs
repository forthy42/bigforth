\ socket interface

also DOS also

legacy off

libc gethostbyname int (int) gethostbyname ( name -- hostent )
libc socket int int int (int) socket ( class type proto -- fd )
libc connect int int int (int) connect ( fd sock size -- err )
libc htonl int (int) htonl ( x -- x' )

struct{
    cell h_name
    cell h_aliases
    cell h_addrtype
    cell h_length
    cell h_addr_list
} hostent

struct{
    cell family+port
    cell sin_addr
    cell padding1
    cell padding2
} sockaddr_in

Create sockaddr-tmp
sockaddr-tmp sizeof sockaddr_in dup allot erase

: c-string ( addr u -- addr' )
    tuck pad swap move pad + 0 swap c! pad ;

: host>addr ( addr u -- x )
    \ converts a internet name into a IPv4 address
    \ the resulting address is in network byte order
    c-string gethostbyname dup 0= abort" address not found"
    hostent h_addr_list @ @ @ ;

2 Constant PF_INET
1 Constant SOCK_STREAM
6 Constant IPPROTO_TCP

: open-socket ( addr u port -- fid )
    htonl PF_INET [ base c@ 0= ] [IF] $10 lshift [THEN]
    or sockaddr-tmp sockaddr_in family+port !
    2dup !fid >r
    host>addr sockaddr-tmp sockaddr_in sin_addr !
    PF_INET SOCK_STREAM IPPROTO_TCP socket
    dup 0<= abort" no free socket" >r
    r@ sockaddr-tmp $10 connect 0< abort" can't connect"
    r> r@ filehandle ! $7FFFFFFF r@ filesize ! r> ;

previous previous
