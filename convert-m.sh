#!/bin/bash
# Convert .m files from old to new format

for i in $*
do
    echo $i:
    xbigforth theseus.fs $i -e "theseus also s\" $i\" dump-file bye"
done
