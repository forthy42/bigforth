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

: r ( phi -- )  fcos epsilon f@ f* !1 f+ k f@ fswap f/ ;

: range ( -- maxphi )  epsilon f@ 1/f fnegate facos ;

: to-sun ( r -- scale )  f**2 bg-scaler f* 1/f ;

: diff-a ( r -- a )
  sun-earth fswap f- f**2
  sun-mass fswap f/ bg-scaler f- GM f@ f* ;

: delta-a ( alpha r -- a )  fdup to-sun f>r
  fswap fsin f* diff-a fr> f* ;

: v ( r -- )  GM f@ fswap f/ f2* U f@ f**2 f+ fsqrt ;