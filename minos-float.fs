\ floating point numbers

[IFUNDEF] float
    import float
[THEN]
also float also minos definitions

number-action class float-action
  how:
    : ># ( fd -- addr u ) fd>f base push nbase @ base ! ff$ ;
    : store ( fd -- ) ># edit assign ;
    : fetch ( -- fd ) edit get base push nbase @ base ! >float drop f>fd ;
    : key ( key sh -- ) drop base push nbase @ base !
      dup shift-keys? IF  drop  EXIT  THEN  dup find-key dup
      IF    cell+ @ caller send drop
      ELSE  drop dup '. <>  IF  digit? nip 0= ?EXIT  THEN
	  sp@ 1 edit with ins drop 1 c drop endwith
      THEN  stroke @ called send ;
class;

' :[ alias SF[                               immediate restrict
: ]SF postpone ]: float-action postpone new ;
                                             immediate restrict
' f>fd alias ]F 

previous previous definitions