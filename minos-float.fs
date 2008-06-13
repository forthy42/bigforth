\ floating point numbers

[IFUNDEF] float
    import float
[THEN]
also float also minos definitions

6 Value fa-precision

number-action class float-action
  how:
    : >#f ( f -- addr u ) base push nbase @ base !
	fa-precision set-precision fx$ ;
    : store ( f -- ) >#f edit assign ;
    : fetch ( -- f ) base push nbase @ base ! edit get >float
	0= IF  !0  THEN ;
class;

: ]#f ( key sys ) postpone ; (textfield postpone endwith
  & float-action >o float-action bind-key o> ;      immediate
'- #[ sp@ 1 ins drop 1 c ]#F
'. #[ sp@ 1 ins drop 1 c ]#F
', #[ sp@ 1 ins drop 1 c ]#F
'e #[ sp@ 1 ins drop 1 c ]#F
'E #[ sp@ 1 ins drop 1 c ]#F
' :[ alias SF[                               immediate restrict
: ]SF postpone ]: float-action postpone new ;
                                             immediate restrict
' noop alias ]F 

previous previous definitions