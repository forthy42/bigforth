\ oof.fs	Object Oriented FORTH
\ 		This file is (c) 1996,2000,2005 by Bernd Paysan
\			e-mail: bernd.paysan@gmx.de
\
\		Please copy and share this program, modify it for your system
\		and improve it as you like. But don't remove this notice.
\
\		Thank you.
\

\ This is the version for MPE's VFX

\ =======
\ *! boof
\ *T Objekt Oriented Forth
\ =======
\ *P The OOP package documented here here was written by Bernd
\ ** Paysan for his *\i{bigFORTH} system. Since then it has been
\ ** ported to other Forth systems. The package is now known as
\ ** *\i{BerndOOF} for short. The source code is in the file
\ ** *\i{oof.fs}.


\ ***************************************
\ *S What is Object Oriented Programming?
\ ***************************************
\ *P The buzzword of the late 80s and 90s in the IT industry was
\ ** without doubt “object oriented”. No operating system, no
\ ** application, and certainly no programming language, that isn’t
\ ** object oriented. Forth isn’t excluded as publications like Dick
\ ** Pountain’s *\i{Object–Oriented FORTH} show clearly.

\ *P Ewald Rieger had ported Pountain’s OOF to bigFORTH and gave it to
\ ** me on the Forth–Tagung ’91 to look at it. Since this OOF lacked
\ ** several features, I completely rewrote it, to make this
\ ** interesting programming paradigma available for bigFORTH. This
\ ** system is in use since 1992, and has proved to be useful even in
\ ** the rough world of real–time programming. Ewald Rieger uses it to
\ ** control an automatic chromatography system.

\ *P The encapsulation of data and algorithm to form an object has
\ ** shown its usefulness — or more proven essential — especially
\ ** for changing hardware configurations, as they are typical for
\ ** many tasks in practice.

\ *P To give you an impression of what is possible with object
\ ** oriented programming, and how to do it, the following is an
\ ** introduction using a small example. The sources are in the
\ ** file *\i{oofsampl.fth}. I use a small collection of data
\ ** types that are known: integers, lists, arrays, and pointers.
\ ** Object oriented programming hides behind a jargon that,
\ ** after a closer look,has some similatities with concepts
\ ** like modularity and definition of clean interfaces.


\ ====================
\ *N The Class Concept
\ ====================
\ *P The core principle of object oriented programming is to
\ ** encapsulate data and the procedures that operate on the data
\ ** into an object. Ideally the procedures (*\b{methods}) that
\ ** access an object are the only way to operate on the data.
\ ** The interface to the object then consists of the names of
\ ** the methods (*\b{messages}) and the parameters that are sent with
\ ** the message. Since many objects return results, the stack is
\ ** used conventionally for the *\b{message passing} and the
\ ** method is called like a Forth word — with a detour over the
\ ** object, that manages the encapsulation.

\ *P It would be a waste to program each method for each single
\ ** object; especially since many objects have the same or
\ ** similar structure and are only distinguished by the data
\ ** itself. Such similar objects are summed up as a *\b{class}.
\ ** A class is so to speak a template (or a form) for an object;
\ ** after *\b{instantiation} a real object is created from a
\ ** class with space for data, and the methods common for all
\ ** objects of that class.

\ *P Often you need only minor modifications to a class to obtain
\ ** a new one, and therefore you use *\b{inheritance} to create
\ ** a *\b{sublcass} — a derivative. Additional variables and
\ ** modified or new methods are just appended to the class.

\ *P All objects of a class and its subclasses have a common
\ ** message protocol, thus they understand the same messages and
\ ** react similarly. The differences in detail are called
\ ** *\b{polymorphism}. So may each graphical object have the
\ ** method *\b{draw_me}, but one object may draw a circle,
\ ** another a point or rectangle.

\ *P If you emphasise the common protocol to all objects of a
\ ** class hierarchy, you create the protocol separated from the
\ ** implementation of the subclasses and call the class, which
\ ** contains only protocol but no implementation, an
\ ** *\b{abstract data type}. Such a data type is the class
\ ** data presented in the following listing:

\ *E Memory also Forth
\ ** object class data     \ abstract data class
\ **   cell var ref        \ reference counter
\ ** public:
\ **   method !  method @  method .
\ **   method null   method atom?   method #
\ ** how:
\ **   : atom? ( -- flag ) true ;
\ **   : # ( -- n ) 0 ;
\ **   : null ( -- addr ) new ;
\ ** class;

\ *P Here I must add that in *\i{BerndOOF} all classes are eventually
\ ** originated from the same parent class, the class object. Also,
\ ** classes and objects aren’t only used for operating on data,
\ ** but also for creating new subclasses and instantiation of
\ ** objects. Therefore a class is just an object without a data
\ ** area. A class can create new subclasses using the method
\ ** *\fo{class}.

\ *P The description of a class consists of two parts: a
\ ** declaration of variables and methods, and the implementation
\ ** of the methods. All variables, all polymorphic and externally
\ ** accessible methods must be declared; helper methods could
\ ** be declared optionally. In the implementation part,
\ ** undeclared methods are automatically declaired as *\fo{EARLY}
\ ** (private).

\ *P The example creates a cell sized variable called *\fo{ref},
\ ** in the private area of the class that isn’t visible from the
\ ** outside, but can be inherited (thus corresponds to *\b{protected:}
\ ** in C++). *\fo{public:}, thus publically available are the
\ ** six methods *\fo{!}, *\fo{@}, *\fo{.}, *\fo{null},
\ ** *\fo{atom?}, and *\fo{#}, which are used to store, read,
\ ** and display the value, to create a *\b{null} object, the
\ ** query whether the object is atomic or composed, and the number
\ ** of sub objects if the latter is the case.

\ *P The last three methods are already implemented, since they
\ ** are the same for all simple objects. The unimplemented
\ ** methods cannot be executed, more precisely they lead to
\ ** *\fo{abort"}. They must be implemented in real data types,
\ ** like in the following data type *\fo{integer:}.

\ ** data class int
\ **   cell var value
\ ** how:
\ **   : ! value F ! ;
\ **   : @ value F @ ;
\ **   : . @ 0 .r ;
\ **   : init ( data -- ) ! ;
\ **   : dispose
\ **     -1 ref +!  ref F @ 0<=
\ **     IF super dispose THEN ;
\ **   : null ( -- addr ) 0 new ;
\ ** class;

\ *P Here I create a new class in the same manner and allocate a
\ ** (private) variable *\fo{value}. The two methods store and
\ ** fetch (*\fo{!} and *\fo{@}) access *\fo{value}. As an
\ ** interface this is sufficient. The *\fo{F} before the words
\ ** changes the interpretation from the object vocabulary to
\ ** the normal vocabulary, thus the normal Forth kernel words
\ ** *\fo{!} and *\fo{@} are used. The method *\fo{.} is easy
\ ** to understand too. Here the *\fo{@} however is interpreted
\ ** as the access method.

\ *P I must say a few words about the methods *\fo{init} and
\ ** *\fo{dispose}: the *\fo{init} method is called in the
\ ** creation of the object and is used to initialise it.
\ ** Here in the example I initialize *\fo{value} with a number
\ ** on the stack. The *\fo{dispose} method removes an object
\ ** from the dynamic memory management. If you modify this
\ ** method, you must (unlike in C++) explicitely call the
\ ** *\fo{dispose} method of the parent class with
\ ** *\fo{super dispose}. I dispose only if the reference counter
\ ** is zero or negative, meaning that there are no further
\ ** references. Otherwise, the reference counter is just
\ ** decremented.

\ *P The *\fo{null} method now has the meaning as expected: it
\ ** creates an object with the value 0 (dynamically) and leaves
\ ** its address on the stack. The word *\fo{new} is, like
\ ** *\fo{dispose}, a method. Without additional information
\ ** (thus without class or object), the method of the current
\ ** class is used.

\ ==========================
\ *N Binding: Late or Early?
\ ==========================
\ *P Let’s take a step back and look at how methods are called.
\ ** How much must be defined at compile time, and what has to
\ ** be resolved at runtime, and then should not produce an error?

\ *P As long as it is clear which method of which class is executed,
\ ** as with *\fo{super dispose}, it will be resolved at compile
\ ** time and create a direct call to the specified method. This
\ ** is called *\b{early binding}. It’s for sure the fastest
\ ** method, but unfortunately it doesn’t allow for polymorphism.

\ *P Often it is not clear in advance which subclass the object
\ ** belongs to when you want to send a method to it. You have to
\ ** find the address of the method at runtime, then. A search
\ ** in the dictionary is prohibitively inefficient, as well as
\ ** a (possibly even sequential) search over a numerical key
\ ** isn’t what I call run time efficiency.

\ *P In *\i{BerndOOF} therefore, each object contains a pointer to a
\ ** jump table as its first element. The jump table contains the
\ ** addresses of all methods. This doesn’t only guarantee a
\ ** response time independent of the number of methods (after all,
\ ** Forth should still remain a real time language), it is also
\ ** quite fast. Especially, since this approach is so easy to
\ ** code, a simple macro can be directly inserted in the caller’s
\ ** code.

\ *P What’s prevented, or at least made much more difficult, with
\ ** this approach is *\b{multiple inheritance}. Crossing a new
\ ** child class from several parent classes is problematic anyway.
\ ** Methods and variables with the same names must be renamed if
\ ** they are not inherited from the same grandparent class, and
\ ** the offsets of variables in the object change (and so must
\ ** also be determined at runtime). Alternatively, the compiler
\ ** must ensure in advance that the necessary space for the mixed
\ ** class is already reserved in the parent classes. This is a
\ ** space/speed tradeoff that cannot be done with a one-pass
\ ** compiler, and creates difficulties even in complex systems.

\ *P A very important aspect is the real time properties of the
\ ** created code. Thus the run times must be known to the
\ ** programmer. In turn, this only leaves completely deterministic
\ ** approaches for binding. Only then the programmer does not
\ ** lose control of what he writes. C++ does not have this
\ ** property, and therefore is only of limited use for real time
\ ** applications.

\ ================================
\ *N Objects as Instance Variables
\ ================================
\ *P Quite often a straight-forward class hierarchy is good enough.
\ ** Appropriate abstract data types allow us to circumvent real
\ ** multiple inheritance issues. In the case of an emergency, you
\ ** can reach a sort of *\fo{multiple inheritance} by copying
\ ** the sources.

\ *P What is necessary though is to have objects as instance
\ ** variables in other objects, as well as by pointer and direct
\ ** reference. This can be shown using lists as example, since
\ ** they need pointers in their implementation:
\ *E forward nil
\ ** data class lists
\ ** public:
\ **   data ptr first
\ **   data ptr next
\ **   method empty?
\ **   method ?
\ ** how:
\ **   : null nil ;
\ **   : atom? false ;
\ ** class;
\ **
\ ** | lists class nil-class
\ ** how:
\ **   : empty? true ;
\ **   : dispose ;
\ **   : . ." ()" ;
\ ** class;
\ **
\ ** | nil-class : (nil
\ ** (nil self Aconstant nil
\ ** nil (nil bind first
\ ** nil (nil bind next

\ *P Here, we first create an abstract data class for lists; this
\ ** needs as both pointer to first and rest of the list as data.
\ ** Since both may normal data, also *\b{dot pairs} are allowed
\ ** as in Lisp. Would the rest of the list have been a list again,
\ ** the type isn’t necessary; that creates a pointer to the
\ ** object of the current declared class. The phrase
\ ** *\fo{lists ptr next} won’t work, since the class *\fo{lists}
\ ** isn’t completely defined at this point and therefore can’t be
\ ** executed.

\ *P Additionally to these pointers you also need a few methods:
\ ** a list could be empty, so you should be able to ask for that.
\ ** Also, it would be quite useful to display the first element
\ ** (with *\fo{?}).

\ *P A null–list is the empty list, also called *\fo{nil}. Since
\ ** this is a list, it must be declared later, therefore I
\ ** create a forward reference, which is resolved with the later
\ ** definition of *\fo{nil}.

\ *P Empty lists differ from ordinary lists quite significantly.
\ ** They always return true to *\fo{empty?}, there’s only one of
\ ** them, and this one certainly may not deleted. It displays a
\ ** pair of parentheses.

\ *P Now I create an element of the class of empty lists, and the
\ ** address of this element (put on the stack with the method
\ ** *\fo{self}) finally is called *\fo{nil}. Both the first as
\ ** the next element of the empty list is again the empty list.
\ ** That prevents crashes when a program runs over the end of
\ ** the list.

\ *P The method *\fo{bind} allows to bind object references to
\ ** an object pointer. The object pointer *\fo{first} of the
\ ** object *\fo{(nil} behaves, after being bound, exactly like
\ ** the object that it is bound to, thus *\fo{(nil} itself.
\ ** This is more interesting with real lists:

\ *E lists class linked
\ ** how:
\ **   : empty? false ;
\ **   : init ( first next -- )
\ **     dup >o 1 ref +! o> bind next
\ **     dup >o 1 ref +! o> bind first ;
\ **   : ? first . ;
\ **   : @ first @ ;
\ **   : ! first ! ;
\ **   : . self >o ’(
\ **     BEGIN
\ **       emit ? next atom? next self o> >o
\ **       IF ." . " data . o> ." )" EXIT THEN
\ **       bl empty?
\ **     UNTIL
\ **     o> drop ." )" ;
\ **   : # next # 1+ ;
\ **   : dispose
\ **     -1 ref +! ref F @ 0> 0= IF
\ **       first dispose next dispose super dispose
\ **     THEN ;
\ ** class;

\ *P A linked list certainly isn’t empty. On creation, I bind
\ ** the references *\fo{first} and *\fo{next}; appropriate object
\ ** addresses must have been put on the stack. At binding, I
\ ** increment the reference counters of the objects — now
\ ** another pointer points to them. To make them the current
\ ** object, I push them on the object stack, thereby with
\ ** *\fo{ref} their reference counter is addressed, and not the
\ ** one of the list. The object stack isn’t a real stack; only
\ ** the topmost element is put into a register, the rest is on
\ ** the return stack.

\ *P The methods *\fo{@}, *\fo{!} and *\fo{?} refer to the first
\ ** element of the list; they are only passed through. No
\ ** complex pointer arithmetic is necessary, the name of the
\ ** reference is sufficient.

\ *P To print a list, I must walk through the list. Before the
\ ** first element, I open a parenthesis, otherwise the elements
\ ** are separated by blanks. The current first element of the
\ ** list is displayed. If the next element is an atom, it must
\ ** be printed as dot pair; the list ends then. The list also
\ ** ends when the next element is the empty list. Afterwards,
\ ** only the parenthesis has to be closed, and the blank is
\ ** dropped from the stack.

\ *P Surprising is the recursion in *\fo{#}, which competes the
\ ** length of a list. It simply computes the length of the rest
\ ** of the list, increments the result and and finishes. As
\ ** soon as the list terminates with nil or an atom that has
\ ** length=0, the recursion terminates. Here you first see a
\ ** clear advantage of object oriented programming; it makes
\ ** lots of *\fo{IF..ELSE..THEN} for decisions unnecessary and
\ ** therefore eases recursions.

\ *P At deletion of a list, both parts of the list and the node
\ ** itself have to be deleted. Here also no case decision is
\ ** necessary, and the termination question, which is often
\ ** forgotten in recursive programs, isn’t asked here.

\ *P Now we just need element objects for the list. We already
\ ** have numbers, but strings would be nice, too. Here they are:

\ *E int class string
\ ** how:
\ **   : !    ( addr count -- )
\ **          value over 1+ SetHandleSize
\ **          value F @ place ;
\ **   : @    ( -- addr count ) value F @ count ;
\ **   : .    @ type ;
\ **   : init ( addr count -- )
\ **     dup 1+ value Handle! ! ;
\ **   : null S" " new ;
\ **   : dispose
\ **           ref F @ 1- 0> 0=
\ **           IF value HandleOff THEN
\ **           super dispose ;
\ ** class;

\ *P We derive the class string from *\fo{int}. I use its
\ ** instance variable value as handle, as pointer to a movable
\ ** memory area. There, the string is stored as counted string.
\ ** When storing a new string, the size of the memory block must
\ ** be adusted; at the first time, it must be allocated, and
\ ** freed at deletion. All the rest is self-explaining, I hope.

\ *P Very useful is the pointer class. You can directly create
\ ** pointer variables, but you can’t insert them into e.g. a
\ ** list.

\ *E data class pointer
\ ** public:
\ **   data ptr container
\ **   method ptr!
\ ** how:
\ **   : !    container ! ;
\ **   : @    container @ ;
\ **   : .    container . ;
\ **   : #    container # ;
\ **   : init ( data -- ) dup >o 1 ref +! o> bind container ;
\ **   : ptr! ( data -- ) container dispose init ;
\ **   : dispose
\ **     -1 ref +! ref F @ 0> 0=
\ **     IF container dispose super dispose THEN ;
\ **   : null nil new ;
\ ** class;

\ *P Analoguous to the list I create a pointer instance variable
\ ** *\fo{(pointer)}; then there’s the method *\fo{ptr!}, which
\ ** is used to assign a new object. The methods *\fo{@}, *\fo{!},
\ ** *\fo{.} and *\fo{#} are fed through to the container. The
\ ** *\fo{init} method binds a passed object to the pointer. The
\ ** method *\fo{ptr!} first releases the previous object, and
\ ** afterwards stores the new object. I certainly care about
\ ** reference counting here.

\ *P Deletion of a pointer object also means that one pointer
\ ** less points to the object (and it eventually has to be
\ ** deleted); afterwards, the pointer is deleted.

\ *P Analoguous to a pointer you can create a whole array of
\ ** pointers:
\ *E data class array
\ ** public:
\ **   data [] container
\ **   cell var range
\ ** how:
\ **   : !         ( <value> n -- ) container ! ;
\ **   : @         ( n -- <value> ) container @ ;
\ **   : .         ’[ # 0
\ **               ?DO emit I container . ’, LOOP
\ **               drop ." ]" ;
\ **   : init      ( data n -- ) range F ! bind container ;
\ **   : dispose   -1 ref +! ref F @ 0> 0=
\ **               IF  # 0
\ **                   ?DO I container dispose LOOP
\ **                   super dispose
\ **               THEN ;
\ **   : null      ( -- addr ) nil 0 new ;
\ **   : #         ( -- n ) range F @ ;
\ **   : atom?     ( -- flag ) false ;
\ ** class;

\ *P Similarly to the method *\fo{new} you create a new array of
\ ** objects with *\fo{new[]} which contains elements of this
\ ** class. The array really is an array of pointers, you can
\ ** assign other objects at any position of the array with
\ ** *\fo{bind[]}. The array index for accessing an array
\ ** variable is expected on the stack.


\ =================================
\ *N Tools and Application Examples
\ =================================
\ *P The list packet still isn’t very easy to use. I’ve written a
\ ** few small tools that eases the use — but certainly won’t
\ ** make up a complete Lisp or something like that out of it:

\ *E : cons         linked new ;
\ ** : list         nil cons ;
\ ** : car          >o lists first self o> ;
\ ** : cdr          >o lists next self o> ;
\ ** : print        >o data . o> ;
\ ** : ddrop        >o data dispose o> ;
\ ** : make-string  string new ;
\ ** : $"           state @ IF
\ **                  compile S" compile make-string exit
\ **                THEN
\ **                ’" parse make-string ; immediate

\ *P The words *\fo{cons} and *\fo{list} help to create a list.
\ ** *\fo{Cons} concatenates two objects on the stack to a
\ ** list (TOS as next, thus should be a list; NOS as first
\ ** element of the list). *\fo{List} takes an object and
\ ** together with *\fo{nil} creates a list out of it.

\ *P *\fo{Car} and *\fo{cdr} should be known from Lisp; they
\ ** return first with respect to the rest of the list.

\ *P *\fo{Print} calls the output method of an object.

\ *P *\fo{Ddrop} finally removes and deletes an object.

\ *P *\fo{Make-string} is the string constructor, analogous
\ ** to *\fo{list}.

\ *P *\fo{$"} constructs a string constant.

\ *P As example how to create a list with these tools:
\ *E $" Dies" $" ist" $" ein" list cons $" Test" list cons cons ok
\ ** dup print (Dies (ist ein) Test) ok
\ ** pointer : test ok
\ ** test . (Dies (ist ein) Test) ok
\ ** test # . 3 ok


\ ************************************
\ *S The Complete BerndOOF Description
\ ************************************
\ *P The interface to object oriented programming in *\i{BerndOOF}
\ ** divides into three parts:
\ *(
\ *B Tools to manage objects, which are themselves not object
\ ** related, and the classes from which all other classes and
\ ** objects are derived,
\ *B Tools to create instance variables and methods,
\ *B methods of the root class, to create new classes, instances,
\ ** handling of object pointers and similar things.
\ *)

\ *P Only the words of the first item above are directly
\ ** accessible from the *\fo{FORTH} vocabulary. The words of
\ ** the second item are only available during declaration of a
\ ** class, and the words of the third item are not words in the
\ ** traditional Forth sense, but are methods of objects.

\ *P *\i{BerndOOF} uses a coherent way to manage classes: classes are
\ ** objects, although with class global instance variables, that
\ ** are just used to create and manage new classes and objects.
\ ** Classes are also used to send messages to objects whose
\ ** address is stored in an object pointer, and that need
\ ** explicit context (because it’s not the current defined
\ ** object), thus are also used as a sort of type casting.

decimal

Variable ?debugHeaders
: +debugHeaders ?debugHeaders on ;
: -debugHeaders ?debugHeaders off ;
+debugHeaders

Vocabulary Objects
also Objects also definitions

Vocabulary oo-types	\ --
\ *G All the words that are used to declare classes and implement
\ ** methods are in the vocabulary *\fo{OO-TYPES}. *\fo{OO-TYPES}
\ ** must be topmost of the search order during class definition,
\ ** since otherwise conflicts would arise. For example *\fo{:}
\ ** is defined in *\fo{OO-TYPES}, the current *\fo{PUBLIC} thread,
\ ** as well as in *\fo{FORTH}.
oo-types also

0 cells Constant :wordlist
1 cells Constant :parent
2 cells Constant :child
3 cells Constant :next
4 cells Constant :method#
5 cells Constant :var#
6 cells Constant :newlink
7 cells Constant :iface
8 cells Constant :init

0 cells Constant :inext
1 cells Constant :ilist
2 cells Constant :ilen
3 cells Constant :inum

Forth definitions
: op! ( o -- )  currobj ! ;

17 cells buffer: ostack	\ -- addr
  0 ostack !

: ^ ( -- o )
\ *G Returns the pointer o to the current object.
    state @ IF  postpone currobj postpone @  ELSE currobj @  THEN ; immediate
: o@ ( -- addr )
\ *G Returns the address of the method table of the current object.
    state @ IF  postpone ^ postpone @  ELSE  ^ @  THEN  ; immediate
: >o ( o -- ) ( OS -- o )
\ *G Moves the pointer to object *\i{o} to the object stack. The
\ ** object thereby becomes the current object. Attention: the
\ ** previously used object is pushed on the return stack, object
\ ** stack accesses therefore must be balanced with other return
\ ** stack accesses like *\fo{DO ... LOOP}s and *\fo{>R} and *\fo{R>}.
    state @
    IF    postpone ^ postpone >r postpone op!
    ELSE  1 ostack +! ^ ostack dup @ cells + ! op!
    THEN  ; immediate
: o> ( -- ) ( OS o -- )
\ *G Pops the pointer to the current object from the object stack.
\ ** The previously used object is restored from the return stack
\ ** and becomes the current object.
    state @
    IF    postpone r> postpone op!
    ELSE  ostack dup @ cells + @ op! -1 ostack +!
    THEN  ; immediate
: size@  ( objc -- size )  :var# + @ 8aligned ;
: o[] ( n -- ) o@ size@ * ^ + op! ;

Objects definitions

\ Coding                                               27dec95py

false Value oset?

: o+,   ( addr offset -- )
  postpone Literal postpone ^ postpone +
  oset? IF  postpone op!  ELSE  postpone >o  THEN  drop ;
: o*,   ( addr offset -- )
  postpone Literal postpone * postpone Literal postpone +
  oset? IF  postpone op!  ELSE  postpone >o  THEN ;
: o@+, ( n -- )  postpone o@ postpone Literal postpone + ;
: ^+, ( n -- )  postpone ^ postpone Literal postpone + ;
: o+@,  ( addr offset -- )
    ^+, postpone @
    oset? IF  postpone op!  ELSE  postpone >o  THEN drop ;
: ^*@  ( offset -- addr )  ^ + @ tuck @ size@ * + ;
: o+@*, ( addr offset -- )
    postpone Literal postpone ^*@
    oset? IF  postpone op!  ELSE  postpone >o  THEN drop ;

\ variables / memory allocation                        30oct94py

Variable lastob
Variable class-o
Variable lastparent   0 lastparent !
Variable vars
Variable methods
Variable decl  0 decl !
Variable 'link

: crash  true abort" unbound method" ;

: link, ( addr -- ) align here 'link !  , 0 , 0 , ;

0 link,

\ type declaration                                     30oct94py

: vallot ( size -- offset )  vars @ >r  dup vars +!
    'link @ 0=
    IF  lastparent @ dup IF  :newlink + @  THEN  link,
    THEN
    'link @ 2 cells + +! r> ;

: valign  ( -- )  vars @ aligned vars ! ;
: vfalign ( -- )  vars @ 8aligned vars ! ;

: mallot ( -- offset )    methods @ cell methods +! ;

oo-types definitions

: (static, ( offset -- )
    >r : r> o@+,
    postpone ;  ;
: static   ( "<name>" -- ) \ oof- oof
\ *G Creates a variable that is common to all the objects of a
\ ** class. This variable is cell sized and created uninitialized
\ ** as pointer.
    mallot (static, ;

: (method, ( offset "<name>" -- )
    >r : r> o@+,
    postpone @ postpone execute
    postpone ;  ;

: method   ( "<name>" -- ) \ oof- oof
\ *G Declares a method. Methods declared like this are late
\ ** bound, if it’s not specified in the context which class
\ ** is used.
    mallot (method,  ;

: early    ( "<name>" -- ) \ oof- oof
\ *G Declares an early bound method. You can’t change such a
\ ** method in a subclass, if you want to use the same name
\ ** again, you have to declare the early method again.
    : postpone ahead postpone then s" dummy string" postpone SLiteral
      postpone ; ;

: (var, ( offset -- )
   >r : r> ^+, postpone ; ;
: var ( size "<name>" -- ) \ oof- oof
\ *G Creates an instance variable of *\i{size} bytes length.
    vallot (var, ;

: (defer, ( n -- )
    >r : r> ^+,
    postpone @ postpone execute postpone ; ;
: defer    ( "<name>" -- ) \ oof- oof
\ *G Declares an object specific method, that can execute object
\ ** specific actions. Execution tokens are assigned with *\fo{IS}.
\ ** This is e.g. useful to assign callbacks.
    valign cell vallot (defer, ;

\ recognise macros                                     06mar05py

: prefix-size ( a b -- n )
    0 >r  BEGIN  over c@ over c@ =  WHILE
	    r> 1+ >r  1+ swap 1+ swap  REPEAT
    2drop r> ;

4 (method, xxx1  8 (method, xxx2
4 (defer,  ddd1  8 (defer,  ddd2
4 (static, sss1  8 (static, sss2
$84 (method, xxx3  $108 (method, xxx4
$84 (defer,  ddd3  $108 (defer,  ddd4
$84 (static, sss3  $108 (static, sss4
early eee

' xxx1 ' xxx2 prefix-size Constant method#
' ddd1 ' ddd2 prefix-size Constant defer#
' sss1 ' sss2 prefix-size Constant static#

' xxx3 ' xxx4 prefix-size Constant method#2
' ddd3 ' ddd4 prefix-size Constant defer#2
' sss3 ' sss4 prefix-size Constant static#2

Objects definitions also oo-types

: exec1? ['] xxx1 method# tuck compare 0= ;
: exec2? ['] xxx3 method#2 tuck compare 0= ;
: exec?    ( addr -- flag )
    dup exec1? swap exec2? or ;
: static1? ['] sss1 static# tuck compare 0= ;
: static2? ['] sss3 static#2 tuck compare 0= ;
: static?  ( addr -- flag )
    dup static1? swap static2? or ;
: early?   ( addr -- flag )
    ['] eee  1 tuck compare 0= ;
: defer?   ( addr -- flag )
    dup ['] ddd1 defer# tuck compare 0=
    swap ['] ddd3 defer#2 tuck compare 0= or ;
: ifm?		\ addr -- flag
\ Is this an interface method?
  c@ $E8 =				\ CALL opcode
;

\ dealing with threads                                 29oct94py

: object-order ( wid0 .. widm m addr -- wid0 .. widn n )
    dup  IF  2@ >r recurse r> swap 1+  ELSE  drop  THEN ;

: interface-order ( wid0 .. widm m addr -- wid0 .. widn n )
    dup  IF    2@ >r recurse r> :ilist + @ swap 1+
         ELSE  drop  THEN ;

: add-order ( addr -- n )  dup 0= ?EXIT  >r
    get-order r> swap >r 0 swap
    dup >r object-order r> :iface + @ interface-order
    r> over >r + set-order r> ;

: drop-order ( n -- )  0 ?DO  previous  LOOP ;

\ object compiling/executing                           20feb95py

: o, ( xt early? -- )
  over exec1?   over and  IF
      drop method# + c@ o@ + @  compile,  EXIT  THEN
  over exec2?   over and  IF
      drop method#2 + @ o@ + @  compile,  EXIT  THEN
  over static1? over and  IF
      drop static# + c@ o@ + @  postpone Literal  EXIT THEN
  over static2? over and  IF
      drop static#2 + @ o@ + @  postpone Literal  EXIT THEN
  drop dup early? over 1+ @ and  IF  1+ dup @ + cell+  compile,
  ELSE  compile,  THEN  ;

\ : (findo    ( string -- cfa n )
\     o@ add-order >r find r> drop-order ;
: (findo    ( string -- cfa n / f ) { string -- }
    o@ >r  0  BEGIN  drop
	r> 2@ swap >r
	string count rot search-wordlist
\	string count type dup 0= IF ."  not" THEN ."  found" cr
    dup r@ 0= or  UNTIL
    r> drop dup 0= IF
	o@ :iface + @ ?dup IF  >r  BEGIN  drop
	    r> 2@ swap >r :ilist + @
	    string count rot search-wordlist
	dup r@ 0= or  UNTIL  r> drop  THEN
    THEN ;

: findo    ( string -- cfa n )
    (findo dup 0= IF  true abort" method not found!" THEN ;

false Value method?

: method,  ( object early? -- )  true to method?
    swap >o >r bl word  findo  0< state @ and
    IF  r> o,  ELSE  r> drop execute  THEN  o> false to method?  ;

: cmethod,  ( object early? -- )
    state @ dup >r
    0= IF  postpone ]  THEN
    method,
    r> 0= IF  postpone [  THEN ;

: early, ( object -- )  true to oset?  true  method,
  state @ oset? and IF  postpone o>  THEN  false to oset? ;
: late,  ( object -- )  true to oset?  false method,
  state @ oset? and IF  postpone o>  THEN  false to oset? ;

\ new,                                                 29oct94py

previous Objects definitions

Variable alloc
0 Value ohere

: oallot ( n -- )  ohere + to ohere ;

: ((new, ( link -- )			\ ))
  dup @ ?dup IF  recurse  THEN   cell+ 2@ swap ohere + >r
  ?dup IF  ohere >r dup >r :newlink + @ recurse r> r> !  THEN
  r> to ohere ;

: (new  ( object -- )
  ohere >r dup >r :newlink + @ ((new, r> r> ! ;		\ ))

: init-instance ( pos link -- pos )
    dup >r @ ?dup IF  recurse  THEN  r> cell+ 2@
    IF  drop dup >r ^ +
        >o o@ :init + @ execute  0 o@ :newlink + @ recurse o>
        r> THEN + ;

: init-object ( object -- size )
    >o o@ :init + @ execute  0 o@ :newlink + @ init-instance o> ;

: (new, ( object -- ) ohere dup >r over size@ erase (new	\ )
    r> init-object drop ;

: (new[],   ( n o -- addr ) ohere >r
    dup size@ rot over * oallot r@ ohere dup >r 2 pick -
    ?DO  I to ohere >r dup >r (new, r> r> dup negate +LOOP	\ )
    2drop r> to ohere r> ;

\ new,                                                 29oct94py

#16 cells buffer: chunks	\ -- addr
: init-oo-mem  ( -- ) chunks 16 cells erase ;
init-oo-mem
' init-oo-mem atcold

[DEFINED] DelFix 0= [IF]
: DelFix ( addr root -- ) dup @ 2 pick ! ! ;
[THEN]

[defined] NewFix 0= [IF]
: NewFix  ( root size # -- addr )
  BEGIN  2 pick @ ?dup 0=
  WHILE  2dup * allocate throw over 0
         ?DO    dup 4 pick DelFix 2 pick +
         LOOP
         drop
  REPEAT
  >r drop r@ @ rot ! r@ swap erase r> ;
[THEN]

: >chunk ( n -- root n' )
  1- -8 and dup 3 rshift cells chunks + swap 8 + ;

: Dalloc ( size -- addr )
  dup 128 > IF  allocate throw EXIT  THEN
  >chunk 2048 over / NewFix ;

: Salloc ( size -- addr ) align here swap allot ;

: dispose, ( addr size -- )
    dup 128 > IF drop free throw EXIT THEN
    >chunk drop DelFix ;

Forth definitions

: new, ( o -- addr )  dup size@
  alloc @ execute dup >r to ohere (new, r> ;

: new[], ( n o -- addr )  dup size@
  2 pick * alloc @ execute to ohere (new[], ;

: dynamic  ( -- )  ['] Dalloc alloc ! ;  dynamic
\ *G Dynamic object creation in the heap on *\fo{NEW}. This is
\ ** the default behaviour.
: static  ( -- ) ['] Salloc alloc ! ;
\ *G Static object creation in the dictionary on *\fo{NEW}. You
\ ** can compile object structures, preserve them with *\fo{SAVE}
\ ** and reuse them after program load, as long as the objects
\ ** themselves don’t use other functions to allocate dynamic
\ ** memory.

\ *P Each object consists of variables and methods that have to
\ ** be declared. The methods afterwards need to be implemented.
\ ** Visibility of variables and objects to the outside can be
\ ** selected. Private methods and variables are only visible
\ ** inside the class and subclasses, externally visible methods
\ ** and variables have to be declared *\fo{public}.

\ *P *\i{BerndOOF} separates declaration and implementation of classes.
\ ** Both together form the definition of a class.

Objects definitions

\ instance creation                                    29mar94py

: instance-does> ( -- )  DOES> state @ IF  dup postpone Literal
	oset? IF  postpone op!  ELSE  postpone >o  THEN
    THEN  early, ;

: instance, ( o -- ) instance-does>
    alloc @ >r F static new, r> alloc ! drop ;
: ptr,      ( o -- )  0 , ,
  DOES>  state @
    IF    dup postpone Literal postpone @ oset? IF  postpone op!  ELSE  postpone >o  THEN cell+
    ELSE  @  THEN late, ;

: array,  ( n o -- )  alloc @ >r static new[], r> alloc ! drop
    DOES> ( n -- ) dup dup @ size@
          state @ IF  o*,  ELSE  nip rot * +  THEN  early, ;

\ class creation                                       29mar94py

Variable voc#
Variable classlist
Variable old-current
Variable ob-interface

: voc! ( addr -- )  get-current old-current !
  add-order  2 + voc# !
  get-order wordlist tuck classlist ! 1+ set-order
  also oo-types classlist @ set-current ;

: (class-does>  DOES>  false method, ;

: (class ( parent -- )  (class-does>
    here lastob !  true decl !  0 ob-interface !
    0 ,  dup voc!  dup lastparent !
  dup 0= IF  0  ELSE  :method# + 2@  THEN  methods ! vars ! ;

: (is ( addr -- )  bl word findo drop
    dup defer? 0= abort" OO: not deferred!"
    defer# + c@ state @
    IF    ^+, postpone !
    ELSE  ^ + !  THEN ;

: goto, ( o -- ) \  method? IF  postpone r> postpone drop  THEN
    false method, ; \ should be tail call optimized

: inherit   ( -- )  bl word findo drop
    dup exec1?  IF  method# + c@ dup o@ + @ swap lastob @ + !  EXIT  THEN
    dup exec2?  IF  method#2 + @ dup o@ + @ swap lastob @ + !  EXIT  THEN
    abort" Not a polymorph method!" ;

\ instance variables inside objects                    27dec93py

: instvar,    ( addr -- ) dup , here 0 , 0 vallot swap !
    'link @ 2 cells + @  IF  'link @ link,  THEN
    'link @ >r dup r@ cell+ ! size@ dup vars +! r> 2 cells + !
    DOES>  dup 2@ swap state @ IF  o+,  ELSE  ^ + nip nip  THEN
           early, ;

: instptr>  ( -- )  DOES>  dup 2@ swap
    state @ IF  o+@,  ELSE  ^ + @ nip nip  THEN  late, ;

: instptr,    ( addr -- )  , here 0 , cell vallot swap !
    instptr> ;

: (o* ( i addr -- addr' ) dup @ size@ rot * + ;

: instarray,  ( addr -- )  , here 0 , cell vallot swap !
    DOES>  dup 2@ swap
           state @  IF  o+@*,  ELSE  ^ + @ nip nip (o*  THEN
           late, ;

\ bind instance pointers                               27mar94py

: ((link ( addr -- o addr' ) 2@ drop ^ + ;	\ ))

: (link  ( -- o addr )  bl word findo drop >body state @
    IF postpone Literal postpone ((link EXIT THEN ((link ;	\ ))

: parent? ( class o -- class class' ) @
  BEGIN  2dup = ?EXIT dup  WHILE  :parent + @  REPEAT ;

: (bound ( obj1 adr2 -- )  ! ;

: (bind ( addr -- ) \ <name>
    (link state @ IF postpone (bound EXIT THEN (bound ;	\ )

Forth definitions

: bind ( o "<name>" -- )
\ *G Binds the object *\i{o} to the object pointer *\i{<name>}.
\ ** The object *\i{o} must be derived from the class *\i{name}
\ ** or a subclass thereof.
  ' >body  state @
  IF   postpone Literal postpone (bound EXIT  THEN
  (bound ;  immediate
: link ( o -- )  ' >body  state @
  IF   postpone Literal EXIT  THEN ;  immediate
: bind2 ( o -- )  (bind ; immediate

Objects definitions

\ method implementation                                29oct94py

Variable m-name
Variable last-interface  0 last-interface !

: interface, ( -- )  last-interface @
    BEGIN  dup  WHILE  dup , @  REPEAT drop ;

: inter, ( iface -- )
    align here over :inum + @ lastob @ + !
    here over :ilen + @ dup allot move ;

: interfaces, ( -- ) ob-interface @ lastob @ :iface + !
    ob-interface @
    BEGIN  dup  WHILE  2@ inter,  REPEAT  drop ;

: lastob!  ( -- )  lastob @ dup
    BEGIN  nip dup @ here cell+ 2 pick ! dup 0= UNTIL  drop
    dup , op! ^ class-o ! o@ lastob ! ;

: thread,  ( -- )  classlist @ , ;
: var,     ( -- )  methods @ , vars @ , ;
: parent,  ( -- o parent )
    o@ lastparent @ 2dup dup , 0 ,
    dup IF  :child + dup @ , !   ELSE  , drop  THEN ;
: 'link,  ( -- )
    'link @ ?dup 0=
    IF  lastparent @ dup  IF  :newlink + @  THEN  THEN , ;
: cells,  ( -- )
  methods @ :init ?DO  ['] crash , cell +LOOP ;

\ method implementation                                20feb95py

oo-types definitions

: how:  ( -- ) \ oof- oof how-to
\ *G Changes from the declaration to the implementation part.
\ ** In this part, you initialise static variables, and implement
\ ** methods.
    decl @ 0= abort" not twice!" 0 decl !
    align  interface,
    lastob! thread, parent, var, 'link, 0 , cells, interfaces,
    dup
    IF    dup :method# + @ >r :init + swap r> :init safe/string move
    ELSE  2drop  THEN ;

: class; ( -- ) \ oof- oof end-class
\ *G Ends the implementation of a class.
\ *P The management of classes and objects is task of the classes
\ ** themselves. Therefore, the root class *\fo{OBJECT} provides
\ ** some methods and class global variables. These can be
\ ** separated into the following groups:
\ *(
\ *B Class browser
\ *B Subclass creation
\ *B Memory management, instance creation
\ *B Binding
\ *)
    decl @ IF  how:  THEN  0 'link !
    voc# @ drop-order old-current @ set-current ;

: ptr ( "<name>" -- ) \ oof- oof
\ *G Declares an object pointer, which must point to the currently
\ ** declared class or a subclass, and is initialized with *\fo{BIND}.
    Create immediate lastob @ here lastob ! instptr, ;
: asptr ( class "<name>" -- ) \ oof- oof
\ *G Casts a pointer created with PTR to the currently
\ ** declared class, and declares the casted pointer as *\i{name}.
    cell+ @ Create immediate
    lastob @ here lastob ! , ,  instptr> ;

\ Create a meaningful header for debugging purposes

: :mangled-noname
    ?debugHeaders @ IF
	^ >name count pad place  s" :" pad append bl word count pad append
	pad count header code-align hide :noname
    ELSE
	bl word drop :noname
    THEN ;

\ Object definitions

synonym Fpostpone postpone

: : ( "<name>" -- ) \ oof- oof colon
\ *G Implements the method *\fo{name}. You end the implementation
\ ** with *\fo{;}.
    decl @ abort" HOW: missing! "  class-o @ op!
    >in @ >r bl word (findo 0=		\ )
    IF  r> >in ! m-name off :
    ELSE  r> >in !
	dup exec?  over early? or  over ifm? or
        0= abort" OO-TYPES: not a method"
	m-name ! :mangled-noname
    THEN ;

Forth

: ; ( xt colon-sys -- ) \ oof- oof
    postpone ; DoNotSin
    m-name @ ?dup 0= ?EXIT  dup exec1?
    IF    method# + c@ lastob @ + !
    ELSE  dup exec2?
	IF    method#2 + @ lastob @ + !
	ELSE
	    dup early? IF  1+ dup >r - 4- r> !  EXIT  THEN
	    >body dup cell+ @ 0< IF  2@ swap lastob @ + @ + !  EXIT  THEN
	    drop
	THEN
    THEN ; immediate

previous
Forth definitions

\ object                                               23mar95py

Create object  ( ... "<method>" -- ... )  immediate  0 (class
\ *G The root of all object classes.
\ ** Executes *\fo{method} resp. compiles it dynamically bound
\ ** in the context of the current object.
         cell var  oblink       \ create offset for backlink
\ *G First instance variable: points to the method table of
\ ** an object.
         static    thread       \ method/variable wordlist
         static    parento      \ -- addr
\ *G Points to the parent class.
         static    childo       \ -- addr
\ *G Points to the last derived subclass.
         static    nexto        \ -- addr
\ *G Points to the next old subclass of the parent class.
         static    method#      \ -- addr
\ *G Number of the methods and static variables in bytes.
         static    size         \ -- addr
\ *G Number of bytes used as dynamic memory
	 static    newlink      \ -- addr
\ *G Points to a list of all sub-objects which consume memory in
\ ** the object. Used for the internal memory management.
	 static    ilist        \ interface list
	 method    init ( ... -- ) \ object- oof
\ *G Initializes an object with the given parameters. *\fo{INIT}
\ ** is also called for all objects that are declared as instance
\ ** variables, in the order of declaration, but first for the
\ ** main object. *\fo{INIT} is a polymorphic method.
         method    dispose ( -- ) \ object- oof
\ *G Frees the object’s memory space. *\fo{DISPOSE} is a
\ ** polymorphic method.
         early     class ( "name" -- ) \ object- oof
\ *G Starts declaration of a subclass called *\fo{name}.
	 early     new ( -- o )  immediate  \ object- oof
\ *G Creates a nameless object (instance) of the current class.
	 early     new[] ( n -- o )  immediate  \ object- oof new-array
\ *G Creates an array of nameless objects of the current class
\ ** with *\i{n} elements.
         early     : ( "<name>" -- ) \ object- oof define
\ *G Creates an object under the name *\fo{name}.
         early     ptr ( "name" -- ) \ object- oof
\ *G Creates a pointer to an object of the current class
\ ** (or subclass) under the name *\fo{name}. The pointer
\ ** is empty at first, use *\fo{BIND} to assign an object
\ ** to the pointer.
         early     asptr ( o "name" -- ) \ object- oof
\ *G Casts a pointer created with *\fo{PTR} to the current
\ ** class and creates the casted pointer under the name
\ ** *\fo{name}.
         early     [] ( n "name" -- ) \ object- oof array
\ *G Creates an array of objects with *\i{n} elements under the
\ ** name *\i{name}.
	 early     ::  ( "name" -- )  immediate \ object- oof scope
\ *G Binds method *\i{name} of the current class early.
\ ** Invoked directly in the implementation part of a class, it
\ ** inherits methods from other classes, and thus allows a
\ ** limited form of multiple inheritance. The method must be
\ ** defined in a common parent class. Only the code address
\ ** of the method is inherited.
         early     class? ( o -- flag ) \ object- oof class-query
\ *G Early Method: Checks class relationship. *\i{Flag} is only
\ ** true when object is in the upward derivation chain of the
\ ** object that executes *\fo{CLASS?}.
	 early     goto  ( "name" -- )  immediate \ object- oof
\ *G Used for end (tail) recursion. The method *\i{name} is
\ ** called directly, without pushing a return address (or
\ ** eventually the old object class) onto the return stack.
	 early     super  ( "name" -- ) immediate \ object- oof
\ *G Binds method *\i{name} of the parent class early. *\fo{SUPER}
\ ** is used to modify inherited behaviour, and to access the
\ ** original behaviour in the modified method. You can use
\ ** *\fo{SUPER} repeatedly to access methods higher up the
\ ** inheritance chain.
         early     self ( -- o ) \ object- oof
\ *G Returns the address of the object.
	 early     bind ( o "name" -- )  immediate \ object- oof
\ *G Stores the address object in the pointer variable *\i{name}
\ ** The object must belong to the pointer’s class or a subclass
\ ** thereof.
         early     bound ( class addr "name" -- ) \ object- oof
	 early     link ( "name" -- addr )  immediate \ object- oof
\ *G Creates a reference to the object pointer *\i{name}, thus
\ ** an address where object pointers can be stored with *\fo{!}.
	 early     is  ( xt "name" -- ) \ object- oof
				immediate
	 early     send ( xt -- ) \ object- oof
	 early     with ( o -- ) \ object- oof
				immediate
	 early     endwith ( -- ) \ object- oof
				immediate
	 early     ' ( "name" -- xt ) \ object- oof tick
				immediate
	 early     postpone ( "name" -- ) \ object- oof
				immediate
	 early     implements ( -- ) \ object- oof

\ base object class implementation part                23mar95py

how:
0 parento !
0 childo !
0 nexto !
    : class   ( -- )       Create immediate o@ (class ;
    : :       ( -- )       Create immediate o@
	decl @ IF  instvar,    ELSE  instance,  THEN ;
    : ptr     ( -- )       Create immediate o@
	decl @ IF  instptr,    ELSE  ptr,       THEN ;
    : asptr   ( addr -- )
	decl @ 0= abort" only in declaration!"
	Create immediate o@ , cell+ @ , instptr> ;
    : []      ( n -- )     Create immediate o@
	decl @ IF  instarray,  ELSE  array,     THEN ;
    : new     ( -- o )     o@ state @
	IF  Fpostpone Literal Fpostpone new,  ELSE  new,  THEN ;
    : new[]   ( n -- o )   o@ state @
	IF  Fpostpone Literal Fpostpone new[], ELSE new[], THEN ;
    : dispose ( -- )       ^ size @ dispose, ;
    : bind    ( addr -- )  (bind ;
    : bound   ( o1 o2 addr2  -- ) (bound ;
    : link    ( -- class addr ) (link ;
    : class?  ( class -- flag )  ^ parent? nip 0<> ;
    : ::      ( -- )
	state @ IF  ^ true method,  ELSE  inherit  THEN ;
    : goto    ( -- )       ^ goto, ;
    : super   ( -- )       parento true method, ;
    : is      ( cfa -- )   (is ;
    : self    ( -- obj )   ^ ;
    : init    ( -- )       ;

    : '       ( -- xt )  bl word findo drop
	state @ IF  Fpostpone Literal  THEN ;
    : send    ( xt -- )  execute ;
    : postpone ( -- )  voc# @
	o@ add-order ^ Fpostpone Literal Fpostpone >o
	Fpostpone Fpostpone  Fpostpone o>
	drop-order voc# ! ;

    : with ( -- n )  voc# @
	state @ oset? 0= and IF  Fpostpone >o  THEN
	o@ add-order voc# ! false to oset? ;
    : endwith ( n -- )
	state @ oset? 0= and IF  Fpostpone o>  THEN
	voc# @ drop-order voc# ! ;

    : implements
	o@ add-order 1+ voc# ! also oo-types
	o@ lastob ! ^ class-o !
	false to oset?   get-current old-current !
	thread @ set-current ;
class; \ object

\ interface                                            01sep96py

Objects definitions

: implement ( interface -- ) \ oof-interface- oof
    align here over , ob-interface @ , ob-interface !
    :ilist + @ >r get-order r> swap 1+ set-order  1 voc# +! ;

: inter-method, ( interface -- ) \ oof-interface- oof
    :ilist + @ bl word count 2dup s" '" str=
    dup >r IF  2drop bl word count  THEN
    rot search-wordlist
    dup 0= abort" Not an interface method!"
    r> IF  drop state @ IF  postpone Literal  THEN  EXIT  THEN
    0< state @ and  IF  compile,  ELSE  execute  THEN ;

Variable inter-list
Variable lastif
Variable inter#

Vocabulary interfaces  interfaces definitions

: method  ( -- ) \ oof-interface- oof
    mallot Create , inter# @ ,
    DOES> 2@ swap o@ + @ + @ execute ;

: how: ( -- ) \ oof-interface- oof
    align
    here lastif @ !  0 decl !
    here  last-interface @ ,  last-interface !
    inter-list @ ,  methods @ ,  inter# @ ,
    methods @ :inum cell+ ?DO  ['] crash ,  LOOP ;

: interface; ( -- ) \ oof-interface- oof
    old-current @ set-current
    previous previous ;

: : ( <methodname> -- ) \ oof-interface- oof colon
    decl @ abort" HOW: missing! "
    bl word count lastif @ @ :ilist + @
    search-wordlist 0= abort" not found"
    dup >body cell+ @ 0< 0= abort" INTERFACES: not a method"
    m-name ! :noname ;

Forth

: ; ( xt colon-sys -- ) \ oof-interface- oof
  postpone ;
  m-name @ >body @ lastif @ @ + ! ; immediate

Forth definitions

: interface-does>
    DOES>  @ decl @  IF  implement  ELSE  inter-method,  THEN ;
: interface ( -- ) \ oof-interface- oof
    Create  interface-does>
    here lastif !  0 ,  get-current old-current !
    last-interface @ dup  IF  :inum @  THEN  1 cells - inter# !
    get-order wordlist
    dup inter-list ! dup set-current swap 1+ set-order
    true decl !
    0 vars ! :inum cell+ methods !  also interfaces ;

previous previous

: private: ; \ empty stub
: public:  ( -- )  ;  \ empty stub for VFX
\ *G Switches to public declaration. All further methods and
\ ** variables are visible interfaces to the object.

\ debugging class: also empty stub

\ *P For debugging porposes, there is the *\fo{DEBUGGING} object.
\ ** It contains further methods which are helpful for debugging.
\ ** Not all methods are implemented yet.

object class debugging  ( ... "name" -- ... )
\ *G This is a helper class, that provides necessary tools to
\ ** debug objects. Otherwise like *\fo{OBJECT}.
private:
  early voc-
public:
  early words  ( -- )
\ *G Lists all the words in the public and private vocabulary.
  early m'  ( "name" -- xt )
\ *G Finds the xt of a method or object variable.
  early see  ( "name" -- )
\ *G Decompiles *\i{Name}
  early view ( "name" -- )
\ *G Opens the editor at the declaration of *\i{name}.
  early trace'  ( ... "name" -- ... )
\ *G Traces the method *\i{name}.
  early debug
  early view!
how:
  : words  ( -- ) F words ;
  : see  ( "name" -- ) F see ;
class;


\ ================
\ *S Formal Syntax
\ ================
\ *E declaration ::=
\ **   <parent> CLASS <object>
\ **   {[ private: | public: ] <creator> <selector> }
\ **   [ HOW: {: <method> <coding>  ; }]
\ ** CLASS;
\ ** <creator> <selector> ::=
\ **   STATIC <static> | METHOD <method> | EARLY <method> |
\ **   <number> VAR <var> | <object> ( : | [] | PTR ) <instance>
\ ** <parent> ::=
\ **   OBJECT | <object>
\ ** <creation> ::=
\ **   <object> (: | PTR) <instance> |
\ **   <number> <object> [] <instance>
\ ** <coding> ::=
\ **   <word> <coding> |
\ **   { <instance> } <selector> <coding>


\ ======
\ *> ###
\ ======

