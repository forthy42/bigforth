#!/bin/bash

tar zcf vfx-minos.tar.gz *.m $(for i in *.m; do [ -f ${i%m}fs ] && echo ${i%m}fs; done) $(vfxlin include startx.fs save xvfxlin bye | grep Including | cut -f2 -d' ')
