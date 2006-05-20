\ sunny boy communication

struct{
 2 source
 2 dest
 1 ctrl
 1 pktcnt
 1 cmd
 256 data
} sma-data

Create sma-buffer  sizeof sma-data allot

\ control bits

$80 Constant group
$40 Constant answer
$20 Constant note
$10 Constant direct

\ commands

Create sendsize $100 allot
Create recsize  $100 allot
: command ( n ssize rsize -- )  rot dup Constant
    tuck recsize + c! sendsize + c! ;

 1 0 12 command GET_NET
 2 4 12 command SEARCH_DEV
 3 6  4 command CFG_NETADDR
 6 0 12 command GET_NET_START
 9 0 23 command GET_CINFO
10 4  0 command SYN_ONLINE
11 3  3 command GET_DATA
12 5  5 command SET_DATA
13 3  3 command GET_SINFO
30 0 26 command GET_BINFO
31 7  7 command GET_BIN
32 7  7 command SET_BIN