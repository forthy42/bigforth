#!/bin/bash

DATE=$(LC_ALL=C date '+%d%b%Y' | tr '[A-Z]' '[a-z]')
DISTPLUS="minos-float.fs theseus*.fs COPYING LGPLv3"
DRAGONDIST="$(echo pattern/dragon{,-back,-head,-wing,-claw}.png pattern/{bark,normal-w1,back,focus}.png)"
ICONDIST=icons/*.png
HELPERS="$(grep ^include *.m | cut -f2 -d' ' | sort -u)"
MINOSCORE="$(vfxlin include startx.fs save xvfxlin bye | grep Including | cut -f2 -d' ')"

tar zcf vfx-minos-$DATE.tar.gz *.m $MINOSCORE $DISTPLUS $DRAGONDIST $ICONDIST $HELPERS
