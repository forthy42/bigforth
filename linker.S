	.file	"linker.S"
#if defined(__linux__) && defined(__ELF__)
.section .note.GNU-stack,"",%progbits
#endif
.text

/*******************************
 *         Link Module         *
 *******************************

  eAX: startoffset, eSI: Source, eBP: link
  uses: eDI: bitString,
  next: eax, rel: ebx, pos: ecx, addr: ebx. */

.globl	linkthread
linkthread:
	movl	%esi,%edi
	addl	4(%esi),%edi
	xorl	%ecx,%ecx
	jmp	Llink2
Llink1:	movw	(%ecx,%esi),%ax
	movw	2(%ecx,%esi),%bx
	addl	%ebp,%ebx
	bts	%ecx,(%edi)
	movl	%ebx,(%ecx,%esi)
	subl	%ebp,%ebx
Llink2:	cmpw	%ax,%cx
	movl	%eax,%ecx
	jne	Llink1
	ret

.globl	linkrel
linkrel:
	movl	%esi,%edi
	addl	4(%esi),%edi
	subl	%esi,%ebp
	subl	$4,%ebp
	xorl	%ecx,%ecx
	jmp	Llink4
Llink3:	movw	(%ecx,%esi),%ax
	movw	2(%ecx,%esi),%bx
	pushl	%ebx
	addl	%ebp,%ebx
	subl	%ecx,%ebx
	bts	%ecx,(%edi)
	movl	%ebx,(%ecx,%esi)
	incl	%ecx
	bts	%ecx,(%edi)
	decl	%ecx
	popl	%ebx
Llink4:	cmpw	%ax,%cx
	movl	%eax,%ecx
	jne	Llink3
	leal	4(%esi,%ebp),%ebp
	ret
