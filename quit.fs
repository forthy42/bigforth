minos also forth
screen self window : quit
quit self op! minos
s" Do you really want to leave?" text-label new forth
2fill
0 :noname ." Yes" cr bye ; simple new s" Yes" button new
dup Constant default
2skip
0 :noname ." No" cr bye ; simple new s" No" button new
2fill
5 hatbox new
2 default modal new panel
s" quit" quit assign
screen w @ quit hglue drop - 0 quit repos
quit show stop
