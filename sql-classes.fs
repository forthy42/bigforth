\ SQL database class                                   01apr98py

debugging class database
public:
    cell var tmpbuf
    cell var conn
    cell var res
    method exec
    method fields
    method field@
    method tuples
    method tuple@
    method clear
\ SQL commands                                         01apr98py
    cell var +in
    cell var state
    method create(
    method insert(
    method )
    method drop

    method :string
    method :int
    method :float
    method :date
    method :time
    method string,
    method int,
    method float,
    method date,
    method time,

    method select
    method select-distinct
    method select-as
    method from
    method where
    method group
    method order
    method order-using

    method .heads
    method .entry
    method .entries
    method entry-box

    method begin
    method end
class;