#!/bin/bash

DATE=$(LC_ALL=C date '+%d%b%Y' | tr '[A-Z]' '[a-z]')
DISTPLUS="minos-float.fs theseus*.fs COPYING LGPLv3"

tar zcf vfx-minos-$DATE.tar.gz *.m $(for i in *.m; do [ -f ${i%m}fs ] && echo ${i%m}fs; done) $(vfxlin include startx.fs save xvfxlin bye | grep Including | cut -f2 -d' ') $DISTPLUS
