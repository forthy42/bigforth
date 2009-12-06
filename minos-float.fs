\ floating point numbers

\needs float import float

also float also minos definitions

6 Value fa-precision

number-action class float-action
    cell var precision
  how:
    : init  fa-precision precision ! super init ;
    : >#f ( f -- addr u ) base push nbase @ base !
	precision @ set-precision fx$ ;
    : store ( f -- ) >#f edit assign ;
    : fetch ( -- f ) base push nbase @ base ! edit get >float
	0= IF  0e  THEN ;
class;

: #pre ( n o -- ) textfield with
        edit callback self float-action with
            precision !
        endwith
    endwith ;

: ]#f ( key sys ) postpone ]: (textfield postpone endwith
  & float-action >o float-action bind-key o> ;      immediate
'-' #[ sp@ 1 ins drop 1 c ]#F
'.' #[ sp@ 1 ins drop 1 c ]#F
',' #[ sp@ 1 ins drop 1 c ]#F
'e' #[ sp@ 1 ins drop 1 c ]#F
'E' #[ sp@ 1 ins drop 1 c ]#F
: ]SF postpone ]: float-action postpone new ;
[defined] DoNotSin [IF] DoNotSin [THEN]
                                             immediate restrict
[defined] alias [IF]
' :[ alias SF[                               immediate restrict
' noop alias ]F 
[ELSE]
    synonym SF[ :[
    synonym ]F noop
[THEN]

previous previous definitions