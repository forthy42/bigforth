\ fly-by anomaly

\ Bahngleichung im Gravitationsfeld
\ Die antriebslose Bahn eines
\ Körpers mit Masse m im Schwerefeld eines Planeten mit Masse M ist
\ allgemein gegeben durch die Gleichung

\ r(θ)=k/(1 + ε cosθ).
\ r=Abstand zum Planetenzentrum
\ θ=Bahnwinkel
\ k=L²/Gm²M=U²b²/GM (Drehimpulskonstante)
\ L=Bahndrehimpuls des Körpers
\ Bahndrehimpuls: L=mUb, b ist der Abstand zwischen Planet und
\ einlaufender Asymptote der Flugbahn.
\ G=Gravitationskonstante
\ ε=√(1+(2EL²/G²M²m³)) (Exzentrizität) = √(1+(U²b/GM)²)
\ E=0.5mU², kinetische Energie des Körpers,
\ Der Bahntyp wird durch ε festgelegt:
\ ε=0 -> Kreis
\ 0 < ε < 1 -> Ellipse
\ ε=1 -> Parabel
\ ε > 1 -> Hyperbel

import float float also

332946e FConstant sun-mass \ relative to earth
1.496e12 FConstant sun-earth

sun-mass sun-earth f**2 f/ FConstant bg-scaler

\ all units are SI units, m kg s

FVariable U
FVariable b
FVariable GM  5.9736e24 6.67428e-11 f* GM f! \ earth mass
FVariable epsilon
FVariable k
FVariable phi0  0e phi0 f!
12.756e6 f2/ FConstant rearth

: >epsilon ( -- )  U f@ f**2 b f@ f* GM f@ f/ !1 f+ fsqrt
  epsilon f! ;

: >k ( -- )  U f@ b f@ f* f**2 GM f@ f/ k f! ;

: orbit ( U b -- )  b f! U f! >k >epsilon ;

: orbit2 ( peri epsilon -- )  fdup epsilon f!
  !1 f+ fswap rearth f+ f* k f!
  k f@ epsilon f@ f**2 !1 f- f/ b f!
  k f@ GM f@ f* fsqrt b f@ f/ U f! ;

: r ( phi -- r )  fcos epsilon f@ f* !1 f+ k f@ fswap f/ ;

: range ( -- maxphi )  epsilon f@ 1/f fnegate facos ;

: to-sun ( r -- scale )  f**2
    f>r fr@ 1/f bg-scaler f+ fr> f* 1/f ;
\ : to-sun ( r -- scale )  f**2 bg-scaler f* 1/f ;

: diff-a ( r -- a )
    sun-earth f- f**2 sun-mass fswap f/
    bg-scaler f- GM f@ f* ;

: delta-a ( alpha r -- a )  fdup to-sun f>r
  fswap fsin f* diff-a fr> f* ;

: v ( r -- )  GM f@ fswap f/ f2* U f@ f**2 f+ fsqrt ;

\ integration

: phi>pos ( phi -- x y )  fdup r fswap r,phi>xy ;

: delta-step ( maxphi stephpi -- delta-a )
    fswap fdup r fover phi0 f@ f+ fover delta-a { f: a |
    v { f: v |
    fswap fover fover
    f- phi>pos { f: x1 f: y1 |
    f+ phi>pos { f: x2 f: y2 |
    x1 x2 f- f**2 y1 y2 f- f**2 f+ fsqrt
    v f/ a f*
    x2 x1 f- y2 y1 f- fatan2 phi0 f@ f+ fcos f* } } } } ;

: integrate ( n -- result )
  range fdup dup 2* fm/ { f: maxphi f: stepphi } 0e
    dup negate 1+ DO
	I abs I' 1999 2000 */ <  IF
	    maxphi I I' fm*/ stepphi delta-step f+
	THEN
  LOOP ;

: phis ( n m -- )
    dup negate 1+ ?DO
	pi I I' 1- fm*/ phi0 f!
	dup integrate phi0 f@ pi f/ 180e f* f.
	( phi0 f@ fcos f* ) 1e3 f* f. cr
    LOOP drop ;

\ Examples:

: galileo  959.9e3 2.47e  orbit2 ;
: NEAR     538.8e3 1.81e  orbit2 ;
: cassini  1173e3  5.8e   orbit2 ;
: rosetta  1954e3  1.327e orbit2 ;