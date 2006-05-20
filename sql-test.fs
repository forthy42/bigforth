\ Test file for SQL access                             01apr98py

include sql.fs

\ create a database                                    01apr98py

sh createdb test

\ create database entries

s" test" database : db

s" customer" db drop
s" product" db drop
s" invoice" db drop

s" customer"    db create(
s" id"          db :int
s" name"     40 db :string
s" street"   40 db :string
s" country"   4 db :string
s" zip"         db :int
s" town"     40 db :string
                db )

s" product"     db create(
s" id"          db :int
s" name"     40 db :string
s" revision" 10 db :string
s" price"       db :float
                db )

s" invoice"     db create(
s" id"          db :int
s" customer"    db :int
s" product"     db :int
s" pieces"      db :int
                db )

\ Create some entries                                  01apr98py

s" customer" db insert(
1 db int,
s" Bernd Paysan" db string,
s" Stockmannstr. 14" db string,
s" D" db string,
81477 db int,
s" München" db string,
db )

s" customer" db insert(
2 db int,
s" Bill Gates" db string,
s" Windows Lane 95" db string,
s" USA" db string,
80586 db int,
s" Redmond" db string,
db )

s" product" db insert(
1 db int,
s" bigFORTH-68k ST" db string,
s" 1.22" db string,
25600 db int,
db )

s" product" db insert(
2 db int,
s" bigFORTH-386 DOS" db string,
s" 1.22" db string,
25600 db int,
db )

s" product" db insert(
3 db int,
s" bigFORTH-386 Linux" db string,
s" 1.22" db string,
25600 db int,
db )

s" name" db select s" price" db select s" product" db from db )
cr db .entries db clear
s" *" db select s" customer" db from db )
cr db .entries db clear
