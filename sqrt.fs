\ integer square root

: sqrt ( u -- u )  0 0
  $10 0 DO  >r d2* d2* r> 2* >r
            r@ 2* 1+ 2dup u>= IF  - r> 1+ >r  ELSE  drop  THEN
            r> LOOP  nip nip ;

Code sqrt ( u -- u )
     BX push  SI push  BX BX xor  DX DX xor  $10 # CX mov
     BEGIN  1 BX *4 I#) SI lea  AX AX add  DX DX adc
            AX AX add  DX DX adc  SI DX cmp
            u>= IF  SI DX sub  THEN  cmc  BX BX adc  
     LOOP  BX AX mov  SI pop  BX pop
     Next end-code
