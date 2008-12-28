#!/bin/bash
# Convert .m files from old to new format

for i in $*
do
    echo $i:
    xbigforth -e "include theseus.fs also theseus s\" $i\" included-minos theseus also s\" $i\" dump-file bye"
done
