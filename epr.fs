\ Bell's inequation and EPR

\needs float  import float  float also

Fvariable c++
Fvariable c+-
Fvariable c-+
Fvariable c--
Fvariable offset
!0 offset f!

100 Value n
100 Value m

: f+! ( f addr -- )  dup f@ f+ f! ;
: fsqrt! ( addr -- ) dup f@ fsqrt f! ;
: fcos2 fcos f**2 ;
: fsincos2 ( r -- a b ) fcos f**2 !1 fover f- ;
: C(a,b) ( a+ a- b+ b- -- c++ c+- c-+ c-- )
    { f: a+ f: a- f: b+ f: b- }
    a+ b+ f* a+ b- f* a- b+ f* a- b- f* ;
: c+! ( c++ c+- c-+ c-- -- )
    c-- f+! c-+ f+! c+- f+! c++ f+! ;
: c ( c++ c-- c+- c-+ -- c )
    f+ f>r f+ fr> fover fover f- f-rot f+ f/ ;
: c-off ( -- )
    !0 c++ f! !0 c+- f! !0 c-+ f! !0 c-- f! ;
: c-sqrt ( -- )
    c++ fsqrt! c-+ fsqrt! c+- fsqrt! c-- fsqrt! ;
: C? c++ f@ c-- f@ f+ c+- f@ c-+ f@ f+ f+
    fdup f0= IF  fdrop !1  THEN
    c++ f@ fover f/ fx.
    c-- f@ fover f/ fx.
    c+- f@ fover f/ fx.
    c-+ f@ fover f/ fx. fdrop ;
: E@ ( -- e )
    c++ f@ c-- f@ c+- f@ c-+ f@ c ;
: Ei(a,b) ( a b -- ) fswap
    fsincos2 frot fsincos2 fswap C(a,b) c+! ;
: Cqm(a,b) f- fsin f**2 ;
: E(a,b) { f: a f: b } c-off
    n 0 ?DO  pi i I' fm*/ offset f@ f+ fdup a f+ fswap b f+ Ei(a,b)  LOOP
    E@ cr c? fdup f. ;
: Eqm(a,b) ( a b -- e ) fswap f- f2* fcos fnegate ;
: Eqm1(a,b) ( a b -- e )
    fover           fover           Cqm(a,b) c++ f!
    fover           fover pi f2/ f+ Cqm(a,b) c+- f!
    fover pi f2/ f+ fover           Cqm(a,b) c-+ f!
    fover pi f2/ f+ fover pi f2/ f+ Cqm(a,b) c-- f!
    fdrop           fdrop           E@ cr c? ;
: Ct(a,b) ( a b -- ) fswap
    fsincos2 fswap frot fsincos2 C(a,b) frot c+! ;
: Et(a,b) ( a b -- e ) c-off  f-
    fdup !0 Ct(a,b)
    pi f2/ Ct(a,b)
    E@ cr c? ;

\ local theories

\ complex vertical+diagonal polarization
\ first attempt: direction + width

: p2+  pi f2/ f+ ;

: Cr(a,b) ( a+ a- b+ b- r -- c++ c+- c-+ c-- )
    { f: a+ f: a- f: b+ f: b- f: r |
    a+ b+ f*  a+ b- f*  a- b+ f*  a- b- f*
    { f: c++ f: c+- f: c-+ f: c-- |
    c++ c+- f+ c-+ f+ c-- f+ 1/f r f* { f: x |
    c++ x f* c+- x f* c-+ x f* c-- x f* } } } ;
: Cr0(a,b) ( a+ a- b+ b- r -- c++ c+- c-+ c-- )
    { f: a+ f: a- f: b+ f: b- f: r |
    a+ b+ f* r f*  a+ b- f* r f*  a- b+ f* r f*  a- b- f* r f* } ;

: Clc ( f d a -- c+ c- ) frot
    f- f2* fcos f< !1 !0 IF fswap THEN ;
: Elr'(a,b) ( f a b -- ) { f: f f: a f: b |
    m 0 ?DO  !2 I m fm*/ !1 f- { f: d |
\	cr f f. a f. b f. ." :: "
	f d a Clc  f d b Clc
	d f**2 !1 fswap f- \ f.s
	Cr0(a,b) \ ." : " f.s
	c+! }
    LOOP } ;
: Elr(a,b) ( a b -- ) c-off { f: a f: b |
    n 0 ?DO pi I n fm*/ offset f@ f+ a b Elr'(a,b) LOOP
    } cr c? E@ fdup f. ;

: sin>cl ( f -- f' )  f**2 f2* !1 f- fdup f0< fabs fsqrt
    IF fnegate THEN !1 f+ f2/ ;
: Cl1 ( f a -- c+ c- )
    f- f2* fcos fdup f0< sin>cl !0 IF  fswap  THEN ;
: Cl2 ( f a -- c+ c- )
    f- f2* fsin fdup f0> sin>cl !0 IF  fswap  THEN ;
: cl ( n f a -- c+ c- )
    fover fover cl1 f>r f>r cl2
    IF  fswap  THEN  fr> f+ fswap fr> f+ ;
: clsum ( f a -- c+ c- )
    { f: f f: a |
    0 f a cl  1 f a cl f>r frot f+ f2/ fswap fr> f+ f2/
    fover fover f+ 1/f funder f* f>r f* fr>
    } ;
: El2'(a,b) ( f a b -- ) { f: f f: a f: b |
    f a Cl1  f b Cl1 C(a,b) c+!
    f a Cl2  f b Cl2 C(a,b) c+!
    f a Cl2  f b Cl1 C(a,b) c+!
    f a Cl1  f b Cl2 C(a,b) c+!
    } ;
: El2(a,b) ( a b -- ) c-off { f: a f: b |
    n 0 ?DO  pi I n fm*/ offset f@ f+ a b El2'(a,b) LOOP
    } cr c? E@ fdup f. ;

: Cl(a,b) ( f a b -- c+ c- )
    funder f- fcos2 f>r f- fcos2
    fr@ !.5 f< IF  !1 fr> f- f>r  THEN
    fr@ fmin !1 fr@ f- f- !0 fmax fr> f2* !1 f- fover f- ;
: El'(a,b) ( a0 b0 a b -- ) { f: a0 f: b0 f: a f: b |
    a0 b0 f- fsincos2 { f: c f: s |
    a     a0 b0  Cl(a,b)  b     b0 a0  Cl(a,b)       c Cr(a,b) c+!
    a p2+ a0 b0  Cl(a,b)  b     b0 a0  Cl(a,b)       s Cr(a,b) c+!
    a     a0 b0  Cl(a,b)  b p2+ b0 a0  Cl(a,b)       s Cr(a,b) c+!
    a p2+ a0 b0  Cl(a,b)  b p2+ b0 a0  Cl(a,b)       c Cr(a,b) c+!
    } } ;
: El(a,b) ( a a' b b' -- e )
    { f: a f: a' f: b f: b' }
    c-off
    a  b  a b  El'(a,b)
    a' b  a b  El'(a,b)
    a  b' a b  El'(a,b)
\    a' b' a b  El'(a,b)
    cr c? E@ fdup f. ;

\ Correlation functions

: S { f: a f: a' f: b f: b' }
    a b E(a,b) a' b E(a,b) f+ fabs
    a b' E(a,b) a' b' E(a,b) f- fabs f+ ;
: Sl !0 !0 { f: a f: a' f: b f: b' f: t1 f: t2 }
    a a' b b' El(a,b) to t1 a' a b b' El(a,b) t1 f+ fabs to t2
    a a' b' b El(a,b) to t1 a' a b' b El(a,b) t1 f- fabs t2 f+ ;
: Sqm { f: a f: a' f: b f: b' }
    a b Eqm(a,b) a' b Eqm(a,b) f+ fabs
    a b' Eqm(a,b) a' b' Eqm(a,b) f- fabs f+ ;
: St { f: a f: a' f: b f: b' }
    a b Et(a,b) a' b Et(a,b) f+ fabs
    a b' Et(a,b) a' b' Et(a,b) f- fabs f+ ;
: Sqm1 { f: a f: a' f: b f: b' }
    a b Eqm1(a,b) a' b Eqm1(a,b) f+ fabs
    a b' Eqm1(a,b) a' b' Eqm1(a,b) f- fabs f+ ;
: Slr { f: a f: a' f: b f: b' }
    a b Elr(a,b) a' b Elr(a,b) f+ fabs
    a b' Elr(a,b) a' b' Elr(a,b) f- fabs f+ ;
: Sl2 { f: a f: a' f: b f: b' }
    a b El2(a,b) a' b El2(a,b) f+ fabs
    a b' El2(a,b) a' b' El2(a,b) f- fabs f+ ;

: p8  8 pi fm*/ ;

: S-max ( -- )    !0 2 p8 1 p8 3 p8 S ;
: Sl-max ( -- )   !0 2 p8 1 p8 3 p8 Sl ;
: Sqm-max ( -- )  !0 2 p8 1 p8 3 p8 Sqm ;
: St-max ( -- )   !0 2 p8 1 p8 3 p8 St ;
: Sqm1-max ( -- ) !0 2 p8 1 p8 3 p8 Sqm1 ;
: Slr-max ( -- )  !0 2 p8 1 p8 3 p8 Slr ;
: Sl2-max ( -- )  !0 2 p8 1 p8 3 p8 Sl2 ;
