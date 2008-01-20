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

332946 Constant sun-mass \ relative to earth

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

: range ( -- to from )  epsilon f@ 1/f fnegate facos fdup fnegate ;
