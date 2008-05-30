\ floating point numbers

[IFUNDEF] float
    import float
[THEN]
also float also minos definitions

6 Value fa-precision

number-action class float-action
  how:
    : >#f ( fd -- addr u ) fd>f base push nbase @ base !
	fa-precision set-precision fx$ ;
    : store ( fd -- ) >#f edit assign ;
    : fetch ( -- fd ) edit get base push nbase @ base ! >float
	IF  f>fd  ELSE  0.  THEN ;
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
' f>fd alias ]F 

previous previous definitions