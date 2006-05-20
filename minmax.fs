\ Vorschlag zur Optimierung
1 [IF]
code min  ( n1 n2 -- )  dx pop  ax dx cmp  dx ax cmovl  next end-code macro
code max  ( n1 n2 -- )  dx pop  ax dx cmp  dx ax cmovnle  next end-code macro
code umin  ( n1 n2 -- )  dx pop  ax dx cmp  dx ax cmovb  next end-code macro
code umax  ( n1 n2 -- )  dx pop  ax dx cmp  dx ax cmovnbe  next end-code macro
code abs  ( n -- +n )  cwd  dx ax xor  dx ax sub  next  end-code macro
[THEN]


\ serialize instructions
code 0ticks  ( -- )  ax push  bx push  cpuid  bx pop  ax pop  next end-code macro

\ read time-stamp counter
code ticks  ( -- ud )  ax push  rdtsc  ax push  dx ax mov  next end-code macro

: .ticks  ( ud1 ud2 -- )  2swap d- ud. ;


\ Benchmarks
: cal   0ticks ticks ticks .ticks ;
: bmin1  0ticks ticks -1 2 min drop ticks .ticks ;
: bmin2  0ticks ticks 1 -2 min drop ticks .ticks ;
: bmax1  0ticks ticks -1 2 max drop ticks .ticks ;
: bmax2  0ticks ticks 1 -2 max drop ticks .ticks ;
: bumin1  0ticks ticks -1 2 umin drop ticks .ticks ;
: bumin2  0ticks ticks 1 -2 umin drop ticks .ticks ;
: bumax1  0ticks ticks -1 2 umax drop ticks .ticks ;
: bumax2  0ticks ticks 1 -2 umax drop ticks .ticks ;
: babs1  0ticks ticks  42 abs drop ticks .ticks ;
: babs2  0ticks ticks -42 abs drop ticks .ticks ;

.( cal:) cal cal cal cal cal cr
.( min:) bmin1 bmin1 bmin1 bmin1 bmin1 cr
.( min:) bmin2 bmin2 bmin2 bmin2 bmin2 cr
.( max:) bmax1 bmax1 bmax1 bmax1 bmax1 cr
.( max:) bmax2 bmax2 bmax2 bmax2 bmax2 cr
.( umin:) bumin1 bumin1 bumin1 bumin1 bumin1 cr
.( umin:) bumin2 bumin2 bumin2 bumin2 bumin2 cr
.( umax:) bumax1 bumax1 bumax1 bumax1 bumax1 cr
.( umax:) bumax2 bumax2 bumax2 bumax2 bumax2 cr
.( abs:) babs1 babs1 babs1 babs1 babs1 cr
.( abs:) babs2 babs2 babs2 babs2 babs2 cr
