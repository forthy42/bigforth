\ html parser                                          30jul00py

Module html

Vocabulary tags
Vocabulary signs

also signs definitions

char & Constant amp
char < Constant gt
char > Constant lt

previous definitions

\ fonts                                                30jul00py

: h1-font ( o -- o )
  font" -*-helvetica-bold-r-normal--24-*-*-*-p-*-iso8859-1" ;
: h1i-font ( o -- o )
  font" -*-helvetica-bold-i-normal--24-*-*-*-p-*-iso8859-1" ;
Table: h1-fonts  h1-font h1-font h1i-font h1i-font [

: h2-font ( o -- o )
  font" -*-helvetica-bold-r-normal--20-*-*-*-p-*-iso8859-1" ;
: h2i-font ( o -- o )
  font" -*-helvetica-bold-i-normal--20-*-*-*-p-*-iso8859-1" ;
Table: h2-fonts  h2-font h2-font h2i-font h2i-font [

: h3-font ( o -- o )
  font" -*-helvetica-bold-r-normal--17-*-*-*-p-*-iso8859-1" ;
: h3i-font ( o -- o )
  font" -*-helvetica-bold-i-normal--17-*-*-*-p-*-iso8859-1" ;
Table: h3-fonts  h3-font h3-font h3i-font h3i-font [

: h4-font ( o -- o )
  font" -*-helvetica-bold-r-normal--14-*-*-*-p-*-iso8859-1" ;
: h4i-font ( o -- o )
  font" -*-helvetica-bold-i-normal--14-*-*-*-p-*-iso8859-1" ;
Table: h4-fonts  h4-font h4-font h4i-font h4i-font [

: n-font ( o -- o )
  font" -*-times-medium-r-normal--14-*-*-*-p-*-iso8859-1" ;
: i-font ( o -- o )
  font" -*-times-medium-i-normal--14-*-*-*-p-*-iso8859-1" ;
: b-font ( o -- o )
  font" -*-times-bold-r-normal--14-*-*-*-p-*-iso8859-1" ;
: bi-font ( o -- o )
  font" -*-times-bold-i-normal--14-*-*-*-p-*-iso8859-1" ;
Table: p-fonts  n-font b-font i-font bi-font [

: tt-font ( o -- o )
  font" -*-courier-medium-r-normal--14-*-*-*-m-*-iso8859-1" ;
: ttb-font ( o -- o )
  font" -*-courier-bold-r-normal--14-*-*-*-m-*-iso8859-1" ;
: tti-font ( o -- o )
  font" -*-courier-medium-i-normal--14-*-*-*-m-*-iso8859-1" ;
: ttbi-font ( o -- o )
  font" -*-courier-bold-i-normal--14-*-*-*-m-*-iso8859-1" ;
Table: tt-fonts  tt-font ttb-font tti-font ttbi-font [

\ tags                                                 30jul00py

Variable fontify
Variable font
Variable italics
Variable bold
Variable in-par

: normal-font       fontify @           @ font ! ;
: bold-font         fontify @ cell+     @ font ! ;
: italic-font       fontify @ 2 cells + @ font ! ;
: bold-italic-font  fontify @ 3 cells + @ font ! ;
: reset-fonts  p-fonts fontify ! normal-font bold off italics off ;

also tags definitions

: H1  h1-fonts fontify ! bold-font in-par on ;
: H2  h2-fonts fontify ! bold-font in-par on ;
: H3  h3-fonts fontify ! bold-font in-par on ;
: H4  h4-fonts fontify ! bold-font in-par on ;
: /P  reset-fonts in-par off ;
' /P Alias /H1
' /P Alias /H2
' /P Alias /H3
' /P Alias /H4
: P   in-par @ IF  /P  THEN  reset-fonts in-par on ;
: B   italic @ IF  bold-italic-font  ELSE  bold-font  THEN  bold on ;
: I   bold @ IF  bold-italic-font  ELSE  italic-font  THEN  italics on ;
: /B  italic @ IF  italic-font  ELSE  normal-font  THEN  bold off ;
: /I  bold @ IF  bold-font  ELSE  normal-font  THEN italics off ;

previous definitions

Module;
