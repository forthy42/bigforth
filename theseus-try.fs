\ try a MINOS program interactively                    12apr98py

Vocabulary minos-try

also minos-try also definitions

: var ( n -- )  Create here swap dup allot erase ;
: method  postpone forward ;

\ Create pointers                                      12apr98py

: create-ptrs ( o -- )
    combined with childs self n @ endwith
    0 ?DO
        >slider-o >backing-o box-o?
        IF
            recurse
        ELSE
	    all-descs find-object
            descriptors with  create-ptr  endwith
        THEN
    LOOP drop ;

toss toss definitions
