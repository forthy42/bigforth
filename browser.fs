\ generate object tree                                 25sep99py

Module class-browser

also MINOS

: object-view ( object -- view )  body> >name 6 - w@ ;

: rightcase ( addr1 u1 -- addr2 u2 )
    scratch place
    0 scratch count bounds ?DO
        IF    I c@ dup tolower dup I c! <>
        ELSE  true  THEN
    LOOP  drop scratch count ;

: fl  [defined] tflush [IF] tflush [THEN] screen sync ;

ficon: open-icon icons/open"
ficon: close-icon icons/close"
ficon: text-icon icons/text"
ficon: blue-dot icons/blue-dot"
ficon: red-dot icons/red-dot"
ficon: green-dot icons/green-dot"

viewport ptr class-struct

also editor

: view-object ( class -- )
  dup object-view swap object" view-name ;

: view-word ( name-field -- )
    [defined] ">tib [IF]
	cell+ count $1F and ">tib debugging view
    [ELSE] drop [THEN] ;

: struct-list ( voc addr u class -- )
  >r text-label new 2 rot
  BEGIN  @ dup  WHILE
         r@ swap dup >r DT[ view-word ]DT
         r@ cell+ name> 4 object ' init over compare 0=
         IF    green-dot
         ELSE  r@ cell+ c@ $20 and
               IF  red-dot  ELSE  blue-dot  THEN
         THEN
         r@ cell+ count $1F and rightcase icon-button new
         swap 1+  r>  REPEAT  drop
  0 1 *filll 2dup glue new
  swap vabox new rdrop ;

: show-structure ( class -- )
  >r r@ @ 2@
  s" public:"  r@ struct-list swap
  s" private:" r> struct-list
  2 hatbox new
  class-struct with  assign resized  endwith ;

: browser-title ( object -- )
  s" Class Browser: " window title!
  object" rightcase window title+! ;

: object-button ( object -- ) >r
  ^^ r@ DT[ dup view-object dup show-structure browser-title ]DT
  text-icon r> object" rightcase icon-button new ;

: objects ( root -- o )  depth 1- >r >r
  BEGIN  r@ child@ dup
         IF    recurse \ flipbox
               dup 0 combined ' -flip
                     combined ' +flip toggle new
               open-icon close-icon toggleicon new
               1 vabox new hfixbox swap
               2dup Sskip new swap 2 habox new swap 
               r@ object-button
               2 habox new  swap  2 vresize new
         ELSE  drop r@ object-button
         THEN
         r> next@ dup >r 0=
  UNTIL  rdrop depth r> - vabox new ;

\ object builder window                                25sep99py

: browser ( -- )  finstall
  screen self window new window with
    ^ to ^^
    1 1 viewport new DS[
      & object objects
      0 1 *filll 2dup glue new
      2 vabox new ]DS
      hrtsizer new
      1 1 viewport new dup F bind class-struct DS[
        s" Class structure" text-label new ]DS
      2 hasbox new  dup hasbox with &200 hsize ! endwith
    2 habox new
    S" Class Browser" assign
    &200 $18 geometry show
  endwith ;

previous previous

export browser ;

Module;
