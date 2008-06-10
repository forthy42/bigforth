/* generic bigforth loader in C
   is used for OS/2, Linux and Windows 95 */

#ifdef linux
#define _GNU_SOURCE
#endif

#if defined(BSD) || defined(linux)
#define unix
#endif

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <setjmp.h>
#include <signal.h>
#ifdef unix
#include <locale.h>
#endif

#ifdef _WIN32
#include <windows.h>
HFILE sinp, sout, serr;
#endif

#define max(a, b) (a > b) ? a : b;

#define VERBOSE

int verbose=0;
#ifdef VERBOSE
# ifdef _WIN32
#  define PRINTV(x...)  if(verbose) { char buf[100]; sprintf(buf, x), \
                                      _hwrite(sout, buf, strlen(buf)); }
#  define PRINTVX(n, x...)  if(verbose>=n) { char buf[100]; sprintf(buf, x), \
                                      _hwrite(sout, buf, strlen(buf)); }
# else
#  define PRINTV(x...)  if(verbose) fprintf(stderr, x)
#  define PRINTVX(n, x...)  if(verbose>=n) fprintf(stderr, x)
# endif
#else
#  define PRINTV(x...)
#  define PRINTVX(n, x...)
#endif

/* OS-dependend part:

   bigFORTH needs the following file system and memory management
   primitives for loading:

   get amount of availabe memory
   allocate: read, write, stack and execute
   open file by name (r/o, binary)
   seek in a file to absolute position
   read in a number of bytes
   close the file

*/

#define CLEN(string) string,strlen(string)

#ifdef OS2 /* create OS/2 loader */

#define INCL_SUB
#define INCL_DOS
#include <os2.h>

#define OSFILE  HFILE

#define available_mem(size)   (size=0x01000000)
#define alloc_mem(size,heap)  DosAllocMem((void *)&(heap), size, fALLOC)
/* (heap=(int *) sbrk(size)) */

#define open_by_name(name,handle) \
({ \
     ULONG usAction=OPEN_ACTION_FAIL_IF_NEW; \
     DosOpen(name, &handle, &usAction, 0L, 0, FILE_OPEN, 0x00C0, 0L); \
})

#define seek_to(handle,pos) \
({ \
     long dummy; \
     DosSetFilePtr(handle, pos, FILE_BEGIN, &dummy); \
     dummy; \
})

#define read_bytes(handle,to,size) \
({ \
     long dummy; \
     DosRead(handle, to, size, &dummy); \
     dummy; \
})

#define close_file(handle)    DosClose(handle);

#define PANIC(string) \
({ \
     long dummy; \
     DosWrite(1, CLEN(string), &dummy); \
})

#define LOADER
#endif /* OS/2 */


#ifdef unix /* Create Unix loader */

#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <unistd.h>
#include <sys/mman.h>
/* #include <asm/sigcontext.h> */
#include <string.h>

#define OSFILE  int

int bytes=0;
static void* heapstart=(void*)0x10000000;

#define available_mem(size)        (size=0x01000000)
#define alloc_mem(size,heap)       (heap=(int *)mmap(heapstart, size, PROT_EXEC|PROT_READ|PROT_WRITE, MAP_ANON|MAP_PRIVATE, -1, 0)); heapstart+=size;
#define open_by_name(name,handle)  (handle=open(name, O_RDONLY))
#define seek_to(handle,pos)        ({ long _pos = pos; if(bytes!=_pos) lseek(handle, _pos, SEEK_SET); bytes=_pos; })
#define read_bytes(handle,to,size) (bytes+=read(handle, to, size))
#define close_file(handle)         (close(handle))

#define PANIC(string) \
   (write(fileno(stderr), CLEN(string)))

#define LOADER
#endif


#ifdef _WIN32 /* Generic loader */

#define OSFILE  HFILE

#define available_mem(size)        (size=0x0100000)
#ifndef STACK_ALLOC
static void* heapstart=(void*)0x10000000;

#define alloc_mem(size,heap)       (heap=(int *) VirtualAlloc(heapstart, size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_WRITECOPY)); heapstart+=size;
#else
int * stackalloc(int size)
{
   asm("movl 8(%ebp),%eax\n\
stlop:  cmpl $0,(%esp)\n\
        addl $-0x1000,%esp\n\
        addl $-0x1000,%eax\n\
        jb stlop\n\
        movl %esp,%eax\n\
        leave\n\
        ret");
}
#define alloc_mem(size,heap)       (heap=stackalloc(size+0x1000)) 
#endif
#define open_by_name(name,handle)  (handle=_lopen(name, OF_READ))
#define seek_to(handle,pos)        (_llseek(handle, pos, FILE_BEGIN))
#define read_bytes(handle,to,size) (_lread(handle, to, size))
#define close_file(handle)         (_lclose(handle))

#define PANIC(string) \
   (_hwrite(serr, string, strlen(string)))
#define PANIC1(string, n) \
   ({ char buf[100], sprintf(buf, string, n); \
      _hwrite(serr, buf, strlen(buf)) })
#undef linux

#define LOADER
#endif
/* end OS-dependend part */

typedef int BIGFORTH(int *, char *, void **);
typedef int THROW(int);
typedef BIGFORTH *PBIGFORTH;
typedef THROW *PTHROW;

char * cstring(char * string)
{
  static char name[32];
  
  memcpy(name, string+1, (int)(*string) & 0x1F);
  name[(int)(*string)]=0;
  
  return name;
}

static char bigforth_header[] =
#ifdef OS2
  "\353\137"
#ifdef MINOS
  "\062"
#else
  "\040"
#endif
  "ANS bigFORTH 386-OS/2 rev. "
#include "version.h"
#ifdef MINOS
  "+MINOS rev. "
#include "minos-version.h"
#endif
  "\022compiled 24jan97py"
#endif
#ifdef unix
  "\353\137"
#ifdef MINOS
  "\062"
#else
  "\041"
#endif
  "ANS bigFORTH 386-Linux rev. "
#include "version.h"
#ifdef MINOS
  "+MINOS rev. "
#include "minos-version.h"
#endif
  "\024compiled " VERSION_DATE "py"
#endif
#ifdef _WIN32
  "\353\137"
#ifdef MINOS
  "\062"
#else
  "\041"
#endif
  "ANS bigFORTH 386-Win32 rev. "
#include "version.h"
#ifdef MINOS
  "+MINOS rev. "
#include "minos-version.h"
#endif
  "\024compiled " VERSION_DATE "py"
#endif
  "\011#00001py "
  "\035(c) 1990-2003 by Bernd Paysan"
#ifdef MINOS
  "\016MINOS+bigFORTH"
#else
  "\016plain bigFORTH"
#endif
;

#define DEFAULTDICT 0x0080000

int bfdat[10]=
{
  0,          /* mroot    */
  0,          /* heaps    */
  DEFAULTDICT,  /* dictionary size */
  0x00080000,  /* Minimal  */
  0x04000000,  /* Maximal  */
  0x00010000,  /* Stacklen */
  1,          /* argc     */
  0,          /* argv     */
  0,          /* env      */
  0x00001000,  /* Return stack len */
};

int recovery[32];
char fpdump[108];

jmp_buf throw_jmp_buf;

#define mroot ((int **)bfdat)[0]
#define heaps ((int **)bfdat)[1]
#define memdat (bfdat+2)
#define stlen (bfdat[5])
#define argc_ (bfdat[6])
#define argv_ ((char ***)bfdat)[7]
#define env_  ((char ***)bfdat)[8]
#define dict_ bfdat[2]
#define rstack_ bfdat[9]

#ifdef _WIN32
#define hinst_ *((HINSTANCE *)(((int *)bfdat)+9))
#define cmdsh_ ((int *)bfdat)[10]
#define win_ ((int *)bfdat)[11]
#define gc_ ((int *)bfdat)[12]
#define sp_ ((int *)bfdat)[13]
#endif
                
char fileerr[8]="where's ";
char file[1024];

char* linkinfo;

void makeempty(int * block, int n)
/* Marks the n bytes long block at block */
{

  PRINTVX(5, "Blocking %08x size %08x\n", (int)block, n);

  block[1]=0;
  block[0]=n;
  *(int *)(((char *)block)+n-sizeof(int))=n;
}

int * bf_alloc(int * free, int n)
/* returns next free: free+2 is pointer for object! */
{
  int m;

  PRINTVX(4, "Allocate %08x size %08x\n", (int)free, n);

  n=(n+0x1B) & -0x10;

  if(*free < n)
    {
      PANIC("Running out of Memory!\n");
      exit(1);
    }

  m=*free-n;

  makeempty(free,n);
  free[1]=-1;
  free=(int *)((int)free+n);
  makeempty((int *)free,m);
  return free;
}

int * find_module(char * name, int date, int * module)
{
  int * retmodule;

  PRINTVX(4, "Trying to find %s hash %08x", cstring(name), date);
  PRINTVX(4, " in module %s (%08lx), cs %08x\n", cstring(((char*)module)+0x30),
          (long) module, module[5]);

  if(!memcmp(name,"\003nil",4)) return NULL;

  do
    {
      if(module[5]==date && !memcmp(name,((char *)module)+0x30,1+*name))
          return module;
      PRINTVX(5, "Not found on this trial %s\n", cstring(((char*)module)+0x30));
      if(module[2] && (retmodule = find_module(name, date, (int *)(module[2]))))
        return retmodule;
      else module=(int *)(module[4]);
    }
  while(module!=NULL);

  return NULL;
}

void bf_link(int * this, int * that, unsigned short * thread)
{
  PRINTVX(3, "Linking %s ", cstring(((char*)this)+0x30));
  PRINTVX(3, "with %s\n", that ? cstring(((char*)that)+0x30) : "nil");

  asm("pushal\n\
        movl    0x10(%ebp),%ebx\n\
        movl    0x08(%ebp),%esi\n\
        movl    0x0C(%ebp),%ebp\n\
        movw    (%ebx),%ax\n\
        shll    $16,%eax\n\
        movw    4(%ebx),%ax\n\
        pushl   %ebx\n\
        movw    2(%ebx),%bx\n\
        shll    $16,%ebx\n\
        call    linkthread\n\
        popl    %ebx\n\
        movw    6(%ebx),%ax\n\
        movw    2(%ebx),%bx\n\
        shll    $16,%ebx\n\
        call    linkrel\n\
        popal");
}

void linkit(unsigned char * nametable)
{
  unsigned short tablesize;
  int namesize;
  int *this, *that;
  char* endtable;

  while(*((unsigned short *)nametable)) {
    tablesize = *((unsigned short *)nametable);
    nametable += sizeof(short);
    PRINTVX(2, "Link module %s, table size %04x\n",cstring(nametable),tablesize);
    endtable=nametable+tablesize;
    
    namesize = ((int)*nametable)+1;
    this=that=find_module(nametable, *(int *)(nametable + namesize), mroot);
    PRINTVX(3, "Module %s found\n", cstring(((char*)this)+0x30));
    nametable += namesize + sizeof(int);
    
    bf_link(this, that, (unsigned short *)nametable);
    
    nametable += 4*sizeof(short);
    
    while(((unsigned int)nametable < (unsigned int)endtable) && that) {
      namesize = 1 + *nametable;
      that=find_module(nametable, *(int *)(nametable + namesize), mroot);
      nametable += namesize;
      if(that) nametable += sizeof(int);
      
      bf_link(this, that, (unsigned short *)nametable);
      
      nametable += 4*sizeof(short);
      
      PRINTVX(4, "Table rest length %04x\n",(int)endtable - (int)nametable);
    }
  }
}

int * loadmod(int * freemem, OSFILE handle, int * modptr, long filepos)
{
  int * module;
  
  PRINTVX(3, "Loading to %08x from %08x module %08x pos %08lx\n",
         (int)freemem, (int)handle, (int)modptr, filepos);

  do {
    if(filepos) seek_to(handle, filepos);
    
    module=freemem+3;
    
    read_bytes(handle, module, 8L);

    if(module[1] == DEFAULTDICT) {
      PRINTVX(4, "Replace dict size %x with %x\n", module[1], dict_);
      module[1] = dict_;
    }
    
    PRINTVX(3, "Load Module with size %08x and DP %08x\n",module[1],module[0]);
    
    freemem=bf_alloc(freemem, module[1] + ((module[1]-1) >> 3) +
                     1 + sizeof(int));
    
    memset(((char *)module)+module[1], 0, ((module[1]-1)>>3) + 1);
    
    module[-1]=1;
    
    read_bytes(handle, module+2, module[0] - 2*sizeof(int));
    
    PRINTV("Loaded module %s\n", cstring((char*)module+0x30));
    
    read_bytes(handle, linkinfo, 2);
    
    PRINTVX(3, "Loading link table %04x\n",*(unsigned short *)linkinfo);

    read_bytes(handle, linkinfo+2, *(unsigned short *)linkinfo);
    
    linkinfo += 2 + *(unsigned short *)linkinfo;
    
    if(modptr == NULL) {
      mroot=module;
    } else {
      int* parent = modptr-2;
      while(parent[4])
        parent=(int *)parent[4];
      parent[4]=(int)module;
      module[3]=(int)modptr;
    }
    {
      int submodule=module[2];
      module[2]=0;
      if(submodule)
        freemem=loadmod(freemem, handle, module, submodule);
    }
    
    filepos=module[4];
    
    PRINTVX(4, "Loading next module at %08lx to %08lx\n", filepos, (long) freemem);
    
    module[4]=0;
  } while(filepos);

  return freemem;
}


/* This loader is used for OSes with dynamic link libraries (such as
   Windows, OS/2 or Linux), and therefore it presents a wrapper for
   DLL loading

   The wrapper runs over a table containing the following functionality:
     - LoadModule
     - GetProcAddr
   for standard module loading
     - type
     - getkey
     - at
     - at?
     - form
     - bye
   for kernal basic IO
   and optionally
     - set_recovery
   for basic exception recovery.
*/

#ifdef OS2  /* Create OS/2-specific wrapper */

long bf_getkey(flag)
{
  KBDKEYINFO inRecord;

  KbdCharIn(&inRecord, flag ? 0 : 1, 0); /* Read Key, but don't wait */

  if(inRecord.fbStatus)
    return ((inRecord.chChar) | (inRecord.chScan << 8) |
            (inRecord.fsState << 16));
  else
    return 0L;
}

void bf_type(long length, char * addr)
{
  long dummy;

  DosWrite(0, addr, length, &dummy);
}

long bf_at_query()
{
  unsigned short row, col;

  VioGetCurPos (&row, &col, 0);

  return ((long)row << 16) | (long)col;
}

static long form=0;

long bf_form()
{
  VIOMODEINFO mode;

  if(form == 0)
    {
      if(VioGetMode (&mode, 0))
        {
          mode.row=25; mode.col=80;
        }
      form = (mode.row << 16) | (long)mode.col;
    }

  return form;
}

void bf_at(int col, int row)
{
  VioSetCurPos (row, col, 0);
}

long bf_get_library(int length, char * addr)
{
  char name[length+1];
  HMODULE lib;
  int rc;

  memcpy(name, addr, length);
  name[length]=0;
  
  rc=DosLoadModule("", 0, name, &lib);

  return (rc ? 0 : lib);
}

long bf_proc_addr(HMODULE lib, int length, char * addr)
{
  char name[length+1];
  PFN proc;
  int rc;

  memcpy(name, addr, length);

  name[length]=0;
  
  rc=DosQueryProcAddr (lib, 0, name, &proc);

  return (long)(rc ? 0 : proc);
}

void prep_terminal()
{
  KBDINFO kbdinfo;

  KbdGetStatus(&kbdinfo, 0);

  kbdinfo.fsMask = (kbdinfo.fsMask & 0xFFF0) | 6; /* noecho, raw mode */

  KbdSetStatus(&kbdinfo, 0);

  (void)bf_form();
}

void deprep_terminal()
{
  KBDINFO kbdinfo;

  KbdGetStatus(&kbdinfo, 0);

  kbdinfo.fsMask = (kbdinfo.fsMask & 0xFFF0) | 0xA; /* noecho, cooked mode */

  KbdSetStatus(&kbdinfo, 0);
}

void bf_bye(int ret)
{
  deprep_terminal();
  exit(ret);
}

void install_signal_handlers()
{
  /* dummy */
}

#endif /* OS/2 specific wrapper */

#ifdef unix
#include <dlfcn.h>
#include <signal.h>
#include <sys/types.h>
#include <sys/ioctl.h>
#include <fcntl.h>
#include <sys/file.h>
#include <errno.h>
#ifndef VMIN
#include <termios.h>
#endif
extern int errno;

static struct termios otio;
int readline_echoing_p = 1;

#ifndef CTRL
#define CTRL(key)       ((key)^'@')
#endif

static int eof_char = CTRL('D');
static int terminal_prepped = 0;

static unsigned long cout=0, maxcur=0;

void get_winsize()
{
  unsigned short rows=0, cols=0;
#ifdef TIOCGWINSZ
  struct winsize size;
  
  if (ioctl (1, TIOCGWINSZ, (char *) &size) >= 0) {
    rows = size.ws_row;
    cols = size.ws_col;
  }
#endif
  if((rows == 0) || (cols == 0)) {
    char *s;
    if ((s=getenv("LINES"))) {
      rows=atoi(s);
    }
    if ((s=getenv("COLUMNS"))) {
      cols=atoi(s);
    }
  }
  if (rows==0) rows=24;
  if (cols==0) cols=80;

  maxcur = ((long)rows << 16) | (long)cols;
}

static void change_winsize(int sig, siginfo_t *info, void *_)
{
  //  signal(sig,change_winsize);
#ifdef TIOCGWINSZ
  get_winsize();
#endif
}

void prep_terminal();

long get_screenpos()
{
  unsigned short row=0, col=0;
  char inchar;

  if(!terminal_prepped)  prep_terminal();

  write(fileno(stdin), "\033[6n", 4);

  while(read(fileno(stdin), &inchar, 1) && inchar !='R')
    {
      switch(inchar)
        {
        case '\033': break;
        case '[': break;
        case ';': row=col; col=0; break;
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9': col=col*10+inchar-'0'; break;
        default: /* something went wrong */ break;
        }
    }
  row--; col--;

  return ((long)row << 16) | (long)col;
}

void prep_terminal ()
{
  int tty = fileno (stdin);

  struct termios tio;
  sigset_t set, oset;

  if (terminal_prepped)
    return;

  setlocale(LC_ALL, "");

  sigemptyset (&set);
  sigemptyset (&oset);
  sigaddset (&set, SIGINT);
  sigprocmask (SIG_BLOCK, &set, &oset);

  tcgetattr (tty, &tio);

  otio = tio;

  readline_echoing_p = (tio.c_lflag & ECHO);

  tio.c_lflag &= ~(ICANON | ECHO);

  if (otio.c_cc[VEOF] != _POSIX_VDISABLE)
    eof_char = otio.c_cc[VEOF];
   
  if ((tio.c_cflag & CSIZE) == CS8)
    tio.c_iflag &= ~(ISTRIP | INPCK);

  tio.c_iflag &= ~(ICRNL | INLCR);
  tio.c_lflag |= ISIG;
  tio.c_cc[VMIN] = 1;
  tio.c_cc[VTIME] = 0;

  tio.c_cc[VLNEXT] = _POSIX_VDISABLE;

  tcsetattr (tty, TCSADRAIN, &tio);
  tcflow (tty, TCOON);

  terminal_prepped = 1;

  sigprocmask (SIG_SETMASK, &oset, (sigset_t *)NULL);

#ifdef CLEAR_SCREEN
  write(fileno(stdin), "\033[2J\033[H", 7);
  cout = 0;
#else
  if(isatty(tty)) {
    cout = get_screenpos();
  }
#endif

  get_winsize();
}

void deprep_terminal ()
{
  int tty = fileno (stdin);
  sigset_t set, oset;

  if (!terminal_prepped)
    return;

  sigemptyset (&set);
  sigemptyset (&oset);
  sigaddset (&set, SIGINT);
  sigprocmask (SIG_BLOCK, &set, &oset);

  tcsetattr (tty, TCSADRAIN, &otio);
  tcflow (tty, TCOON);

  terminal_prepped = 0;


  sigprocmask (SIG_SETMASK, &oset, (sigset_t *)NULL);
}

long pending = -1L;

long key_avail (stream)
        FILE *stream;
{
  int tty = fileno (stream);
  long chars_avail = pending;
  int result;

  if(!terminal_prepped)  prep_terminal();

  result = ioctl (tty, FIONREAD, &chars_avail);

  return chars_avail;
}

unsigned char getkey(stream)
     FILE *stream;
{
  int result;
  unsigned char c;

  if(!terminal_prepped)  prep_terminal();

  if (pending < 0)
    {
      result = read (fileno (stream), &c, sizeof (char));

      if (result == sizeof (char))
        return c;

      if (errno != EINTR)
        return (4);

      if (result == 0)
        return (0);
    }

  result = (int) pending;
  pending = -1L;

  return result;
}

long bf_getkey(int flag)
{
  return((flag || key_avail(stdin)) ? (long)getkey(stdin) : 0L);
}

long bf_at_query()
{
  if(!terminal_prepped)  prep_terminal();

  return cout;
}

long bf_form()
{
  if(!maxcur)  get_winsize();

  return maxcur;
}

void bf_type(long length, char * addr)
{
  int i;

  write(fileno(stdout), addr, length);

  for(i=0; i<length; i++)
    switch(*addr++)
      {
      case '\b':  cout--; break;
      case '\t':  cout+= 8; cout &= -8; break;
      case '\n':  cout >>= 0x10; cout++;
        if(cout >= (maxcur >> 0x10))
          cout = (maxcur >> 0x10) -1;
        cout <<= 0x10; break;
      default:    cout++; break;
      }
}

void bf_at(int col, int row)
{
  char buf[20];

  if(cout != (unsigned int)((row << 16) | col)) {
    sprintf(buf,"\033[%d;%dH", row+1, col+1);
    write(fileno(stdout), buf, strlen(buf));
    
    cout = (row << 16) | col;
  }
}

long bf_get_library(int length, char * addr)
{
  char name[length+1];

  memcpy(name, addr, length);
  name[length]=0;
  
  PRINTVX(2, "Open dynamic library %s\n", name);
  return (long)dlopen(name, RTLD_LAZY | RTLD_GLOBAL); /* NOW */
}

long bf_proc_addr(long lib, int length, char * addr)
{
  char name[length+1];

  memcpy(name, addr, length);

  name[length]=0;
  
  PRINTVX(3, "Get symbol %s\n", name);
  return (long)dlsym((void *)lib, name);
}

void bf_bye(int ret)
{
  deprep_terminal();
  exit(ret);
}

static void
graceful_exit (int sig, siginfo_t *info, void *_)
{
  fprintf (stderr, "\n\n%s.\n", strsignal (sig));
  deprep_terminal();
  exit (0x80|sig);
}

static void 
signal_throw(int sig, siginfo_t *info, void *_)
{
  int code;
  struct sigcontext * sigc = (struct sigcontext *)((int *)(info+1)+5);
  long *dump1, *dump2, *dump3;

  struct {
    int signal;
    int throwcode;
  } *p, throwtable[] = {
    { SIGINT, -28 },
/*    { SIGFPE, -55 }, */
    { SIGBUS, -23 },
    { SIGSEGV, -9 },
  };

  dump1 = &(sigc->edi);
  dump2 = ((long*)(recovery))+1;
  dump3 = (long*)(sigc->fpstate);

  memcpy(dump2, dump1, 8*sizeof(long));
  dump2[3] = sigc->esp_at_signal;
  dump2 += 8;

  *dump2++ = sigc->eip;
  *dump2++ = sigc->cs;
  *dump2++ = sigc->eflags;
  *dump2++ = sigc->trapno;
  if(dump3) {
    memcpy(fpdump, (char*)(dump3), 108);
    *dump2-- = (long)fpdump;
  } else {
    *dump2-- = (long)0;
  }

  for (code=-256-sig, p=throwtable; p<throwtable+(sizeof(throwtable)/sizeof(*p)); p++)
    if (sig == p->signal) {
      code = p->throwcode;
      break;
    }
#if 1
  if(sig == SIGFPE) {
    switch(info->si_code) {
#ifdef FPE_INTDIV
    case FPE_INTDIV: code=-10; break; /* integer divide by zero */
#endif
#ifdef FPE_INTOVF
    case FPE_INTOVF: code=-11; break; /* integer overflow */
#endif
    case FPE_FLTDIV: code=-42; break; /* floating point divide by zero */
    case FPE_FLTOVF: code=-43; break; /* floating point overflow  */
    case FPE_FLTUND: code=-54; break; /* floating point underflow  */
    case FPE_FLTRES: code=-41; break; /* floating point inexact result  */
#if 0 /* defined by Unix95, but unnecessary */
    case FPE_FLTINV: /* invalid floating point operation  */
    case FPE_FLTSUB: /* subscript out of range  */
#endif
    default: code=-55; break;
    }
  }
#endif
  longjmp(throw_jmp_buf,code); /* or use siglongjmp ? */
}

static void termprep (int sig, siginfo_t *info, void *_)
{
  //  signal(sig,termprep);
  terminal_prepped=0;
}

void install_signal_handler(int sig, void (*handler)(int, siginfo_t *, void *))
     /* installs three-argument signal handler for sig */
{
  struct sigaction action;

  action.sa_sigaction=handler;
  sigemptyset(&action.sa_mask);
  action.sa_flags=SA_RESTART|SA_NODEFER|SA_SIGINFO|SA_ONSTACK; /* pass siginfo */
  sigaction(sig, &action, NULL);
}

void install_signal_handlers (void)
{
  static short sigs_to_throw [] = {
#ifdef SIGBREAK
    SIGBREAK,
#endif
#ifdef SIGINT
    SIGINT,
#endif
#ifdef SIGILL
    SIGILL,
#endif
#ifdef SIGEMT
    SIGEMT,
#endif
#ifdef SIGFPE
    SIGFPE,
#endif
#ifdef SIGIOT
    SIGIOT,
#endif
#ifdef SIGSEGV
    SIGSEGV,
#endif
#ifdef SIGALRM
    SIGALRM,
#endif
#ifdef SIGPIPE
    SIGPIPE,
#endif
#ifdef SIGPOLL
    SIGPOLL,
#endif
#ifdef SIGPROF
    SIGPROF,
#endif
#ifdef SIGBUS
    SIGBUS,
#endif
#ifdef SIGSYS
    SIGSYS,
#endif
#ifdef SIGTRAP
    SIGTRAP,
#endif
#ifdef SIGURG
    SIGURG,
#endif
#ifdef SIGUSR1
    SIGUSR1,
#endif
#ifdef SIGUSR2
    SIGUSR2,
#endif
#ifdef SIGVTALRM
    SIGVTALRM,
#endif
#ifdef SIGXFSZ
    SIGXFSZ,
#endif
  };
  static short sigs_to_quit [] = {
#ifdef SIGHUP
    SIGHUP,
#endif
#ifdef SIGQUIT
    SIGQUIT,
#endif
#ifdef SIGABRT
    SIGABRT,
#endif
#ifdef SIGTERM
    SIGTERM,
#endif
#ifdef SIGXCPU
    SIGXCPU,
#endif
  };
  unsigned int i;

#define DIM(X)          (sizeof (X) / sizeof *(X))
/*
  for (i = 0; i < DIM (sigs_to_ignore); i++)
    signal (sigs_to_ignore [i], SIG_IGN);
*/
  for (i = 0; i < DIM (sigs_to_throw); i++)
    install_signal_handler (sigs_to_throw [i], signal_throw);
  for (i = 0; i < DIM (sigs_to_quit); i++)
    install_signal_handler (sigs_to_quit [i], graceful_exit);
#ifdef SIGCONT
    install_signal_handler (SIGCONT, termprep);
#endif
#ifdef SIGWINCH
    install_signal_handler (SIGWINCH, change_winsize);
#endif
}

#endif /* Linux specific wrapper */

#ifdef _WIN32

int terminal_prepped=0;

void prep_terminal()
{
#ifndef WIN_MAIN
   sinp=GetStdHandle(STD_INPUT_HANDLE);
   sout=GetStdHandle(STD_OUTPUT_HANDLE);
   serr=GetStdHandle(STD_ERROR_HANDLE);
#else
#ifdef VERBOSE
   serr=sout=_lcreat("test.out", 0);
#endif
#endif
   terminal_prepped=1;
}

void deprep_terminal()
{
}

static unsigned long cout=0, maxcur=0;

void get_winsize()
{
   maxcur = ((long)24 << 16) | (long)80;
}

long bf_getkey(int flag)
{
#ifndef WIN_MAIN
  INPUT_RECORD conin;
  int n;
   
  if(!PeekConsoleInput(sinp, &conin, 1, &n)) return 0;

  if(!n || !ReadConsoleInput(sinp, &conin, 1, &n)) return 0;
  
  if(n && (conin.EventType == KEY_EVENT) && conin.Event.KeyEvent.bKeyDown)
    return conin.Event.KeyEvent.uChar.AsciiChar;
  else
#endif
    return 0;
}

long bf_at_query()
{
  if(!terminal_prepped)  prep_terminal();

  return cout;
}

long bf_form()
{
  if(!maxcur)  get_winsize();

  return maxcur;
}

void bf_type(long length, char * addr)
{
  int i;
#if !defined(WIN_MAIN) || defined(VERBOSE)
  _hwrite(sout, addr, length);
#endif
   
#ifndef WIN_MAIN
  for(i=0; i<length; i++)
    switch(*addr++)
      {
      case '\b':  cout--; break;
      case '\n':  cout >>= 0x10; cout++;
        if(cout >= (maxcur >> 0x10))
          cout = (maxcur >> 0x10) -1;
        cout <<= 0x10; break;
      default:    cout++; break;
      }
#endif
}

void bf_at(int col, int row)
{
#ifndef WIN_MAIN
  COORD curpos;
  
  curpos.X=col;
  curpos.Y=row+1;
   
  SetConsoleCursorPosition(sout, curpos);

  cout = (row << 16) | col;
#endif
}

long bf_get_library(int length, char * addr)
{
  char name[32];
  long ret;
   
  memcpy(name, addr, length);
  name[length]=0;
  
  if((ret=(long)GetModuleHandle(name)))
    return ret;
  return(long)LoadLibrary(name);
}

long bf_proc_addr(long lib, int length, char * addr)
{
  char name[32];

  memcpy(name, addr, length);

  name[length]=0;
  
  return (long)GetProcAddress((HMODULE)lib, name);
}

void bf_bye(int ret)
{
  deprep_terminal();
  exit(ret);
}

char * strsignal(int sig)
{
  static char signal[5];
  sprintf(signal,"%d",sig);
  return signal;
}
/*
static void
graceful_exit (int sig)
{
  PANIC1 ("\n\n%s.\n", strsignal (sig));
  deprep_terminal();
  exit (0x80|sig);
}
*/
void install_signal_handlers()
{
  /* dummy */
}
#endif

void * function_table[9] =
{
  bf_get_library,
  bf_proc_addr,
  bf_type,
  bf_getkey,
  bf_at_query,
  bf_at,
  bf_form,
  bf_bye,
  (void *) recovery
};

void go_bigforth()
{
  int throw_code;
  int throw_to;
  
  install_signal_handlers();
  recovery[0] = 0;
  recovery[12] = -1;

  if ((throw_code=setjmp(throw_jmp_buf))) {
    throw_to = recovery[0];

    if(throw_to) ((PTHROW)throw_to)(throw_code);
    else {
#ifdef _WIN32
       char buf[100];
      sprintf (buf, "\n\nbigforth: %s.\n", strsignal (throw_code));
      _hwrite(serr, buf, strlen(buf));
#else
      fprintf (stderr, "\n\nbigforth: %s.\n", strsignal (throw_code));
#endif
       deprep_terminal();
      exit (0x80|throw_code);
    }
  }
  else
    ((PBIGFORTH)mroot[6])(bfdat, &bigforth_header[0], function_table);
}

int convsize(char *s, long elemsize, int * param)
/* converts s of the format [0-9]+[bekMG]? (e.g. 25k) into the number
   of bytes.  the letter at the end indicates the unit, where e stands
   for the element size. default is e */
{
  char *endp;
  int n;

  n = strtoul(s, &endp, 0);
  if (endp!=NULL) {
    switch(*endp++) {
    case 'b': break;
    case 'k': n<<=10; break;
    case 'M': n<<=20; break;
    case 'G': n<<=30; break;
    default: endp--;
    case 'e': n*=elemsize; break;
    }
  } else {
    n*=elemsize;
  }
  *param = n;

  if(endp != NULL && *endp==',') {
    return endp+1-s;
  } else {
    return 0;
  }
}

static char * helpstring =
"Usage: %s [engine options] ['--'] [image arguments]\n\
Engine Options:\n\
  -h, --help                        Print this message and exit\n\
  -v, --verbose                     Print debugging information during startup\n\
  -i FILE, --image-file FILE        Use image FILE instead of `%s.fi'\n\
  -m SIZE, --mem-size SIZE          Specify Forth heap size\n\
  -s SIZE, --stack-size SIZE        Specify stack size\n\
  -r SIZE, --rstack-size SIZE       Specify return stack size\n\
  -d SIZE, --dictionary-size SIZE   Specify Forth dictionary size\n\
SIZE arguments consist of an integer followed by a unit. The unit can be\n\
  `b' (byte), `e' (element; default), `k' (KB), `M' (MB), or `G' (GB)\n\
";

#define ARG2  { argv[2] = argv[0]; argc -= 2; argv += 2; continue; }
#define ARG1  { argv[1] = argv[0]; argc--; argv++; continue; }
#define ARGN(n) \
if(n>0) { argv[n] = argv[0]; argc -= n; argv += n; continue; } \
else { argv[1] += -n; continue; }

int checkarg(char ** argv, char * argstring, int * param)
{
  int n;
  if(argv[1][0] == '-' && argv[1][1] == argstring[0]) {
    if(argv[1][2] == '=') {
      n = convsize(argv[1]+3, 1, param);
      return n ? -(n+3) : 1;
    } else if(argv[1][2] == '\0') {
      n = convsize(argv[2], 1, param);
      return 2;
    } else return 0;
  } else if(!strncmp(argv[1], "--", 2) &&
	    !strncmp(argv[1]+2, argstring, strlen(argstring))) {
    if(argv[1][2+strlen(argstring)] == '=') {
      n = convsize(argv[1]+3+strlen(argstring), 1, param);
      return n ? -(n+strlen(argstring)) : 1;
    } else if(argv[1][2+strlen(argstring)] == '\0') {
      convsize(argv[2], 1, param);
      return 2;
    } else return 0;
  }
  return 0;
}

#define CHECK_ARGS() \
  while(argc > 1) { int n; \
    if((n=checkarg(argv, "mem-size", memdat+2))) { ARGN(n); } \
    else if((n=checkarg(argv, "stack-size", memdat+3))) { ARGN(n); } \
    else if((n=checkarg(argv, "rstack-size", &rstack_))) { \
        memdat[2] = max(memdat[2], 2*rstack_); \
        ARGN(n); } \
    else if((n=checkarg(argv, "dictionary-size", &dict_))) { \
        memdat[2] = max(memdat[2], 2*dict_); \
        ARGN(n); } \
    else if(!strncmp(argv[1], "--image-file=", 13)) { \
        strcpy(file, argv[1]+13); \
        ARG1; \
    } \
    else if(!strncmp(argv[1], "-i=", 3)) { \
        strcpy(file, argv[1]+3); \
        ARG1; \
    } \
    else if(!strcmp(argv[1], "--image-file") || !strcmp(argv[1], "-i")) { \
        strcpy(file, argv[2]); \
        ARG2; \
    } \
    if(argc > 1) { \
      if (!strcmp(argv[1], "--verbose") || !strcmp(argv[1], "-v")) { \
        verbose++; \
        ARG1; \
      } else if(!strcmp(argv[1], "--")) { \
        ARG1; \
        break; \
      } else if(!strcmp(argv[1], "--help") || !strcmp(argv[1], "-h")) { \
        printf(helpstring, argv[0], argv[0]); \
        break; \
      } else { \
        break; \
      } \
    } else  { \
      break; \
    } \
  }

#ifdef _WIN32
#ifdef WIN_MAIN
int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
                   LPSTR lpCmdLine, int nCmdShow)
#else
int main(int argc, char ** argv, char **env)
#endif
{
  int freemem, reg;
  int* nextfree;
  OSFILE handle;
#ifdef WIN_MAIN
  char * arg[100];
  char * env[100];
  char ** argv, *args, *oldargs;
  int argc = 0;
  argv = arg;
  {
     int i, n; char *envs=GetEnvironmentStrings();
     for(i=0; i<100; i++) {
	n=strlen(envs);
	if(n==0) { env[i] = NULL; break; };
        env[i] = envs;
	envs = envs + n + 1;
     }
  } 
  prep_terminal();

  argv[0] = args = oldargs = GetCommandLine();
  {
     int i, n = strlen(args);
     char sep_mode = '\0';
     
     for(i=0; i<n; i++) {
        if(sep_mode == '\0') {
           if(*args != ' ') {
              sep_mode = ' ';
           } else {
              oldargs=args+1;
           }
        }
        if(*args==sep_mode) {
           *args='\0';
           PRINTV("Argument %i=\"%s\"\n", argc, oldargs);
           argv[argc++]=oldargs;
           oldargs=args+1;
           sep_mode='\0';
        } else if ((*args=='"') || (*args=='`')) {
           sep_mode=*args;
           oldargs=args+1;
        }
        args++;
     }
  }
  if(strlen(oldargs)) {
     PRINTV("Argument %i=\"%s\"\n", argc, oldargs);
     argv[argc++]=oldargs;
  }
  argv[argc] = 0;
#endif

  strcpy(file,argv[0]);

#if (defined OS2) || (defined _WIN32)
  if(!memcmp(".exe", file+strlen(file)-4, 4))
     file[strlen(file)-4]=0;
#endif

  strcpy(file+strlen(file),".fi");

  CHECK_ARGS();

  if(argc > 1 && argv[1][0]=='#' && argv[1][1]=='#') {
    static char args[200];
    int i;
    PRINTV("Concatenate ## arguments\n");
    args[0] = 0;
    for(i=1; i<argc; i++) {
      strcpy(args+strlen(args), " ");
      strcpy(args+strlen(args), argv[i]);
    }
    argv[1] = "-e";
    argv[2] = args+3;
    argc=3;
  }
   
  PRINTV("Load file %s\n", file);

  argc_=argc;
  argv_=argv;
  env_ =env;
  
  freemem=memdat[2];

  alloc_mem(freemem, heaps);

  PRINTV("Running as %s\n",argv[0]);

  PRINTV("Allocated pool at %08x, size %08x\n", heaps, freemem);

  *heaps++=0; *heaps++=0;
  heaps[(freemem>>2)-4]=0;
  heaps[(freemem>>2)-3]=0;

  makeempty(heaps,freemem-4*sizeof(int));
  nextfree=heaps;
  linkinfo = (char *)(heaps+2);

  nextfree=bf_alloc(nextfree,stlen);

  open_by_name(file, handle);

  nextfree=loadmod(nextfree, handle, NULL, 0L);

  close_file(handle);

  *((short *) linkinfo) = 0;

  linkit((char *)(heaps+2));

#ifdef CLEAR_SCREEN
  cout = 0;
  bf_at_query();
#else
#ifdef _WIN32
  bf_at_query();
#endif
#endif

  go_bigforth();

  deprep_terminal();

  exit(0);
}
#else
int main(int argc, char ** argv, char **env)
{
  int freemem;
  int* nextfree;
  OSFILE handle;

  strcpy(file,argv[0]);

#if (defined OS2) || (defined _WIN32)
  if(!memcmp(".exe", file+strlen(file)-4, 4))
     file[strlen(file)-4]=0;
#endif

  strcpy(file+strlen(file),".fi");

  CHECK_ARGS();

#ifdef _WIN32
  if(argc > 1 && argv[1][0]=='#' && argv[1][1]=='#') {
    static char args[200];
    int i;
    PRINTV("Concatenate ## arguments\n");
    args[0] = 0;
    for(i=1; i<argc; i++) {
      strcpy(args+strlen(args), " ");
      strcpy(args+strlen(args), argv[i]);
    }
    argv[1] = "-e";
    argv[2] = args+3;
    argc=3;
  }
#endif
   
  PRINTV("Load file %s\n", file);

  freemem=memdat[2];

  alloc_mem(freemem, heaps);

  argc_=argc; argv_=argv; env_=env;

  PRINTV("Running as %s\n",argv[0]);

  PRINTV("Allocated pool at %08lx, size %08x\n", (long) heaps, freemem);

  *heaps++=0; *heaps++=0;
  heaps[(freemem>>2)-4]=0;
  heaps[(freemem>>2)-3]=0;

  makeempty(heaps,freemem-4*sizeof(int));
  nextfree=heaps;
  linkinfo = (char *)(heaps+2);

  nextfree=bf_alloc(nextfree,stlen);

  open_by_name(file, handle);

  if(handle < 0) {
    char * path = getenv("BIGFORTH_PATH") ?: INSTDIR;
    char buf[strlen(path)+strlen(file)+2];
    int i=0;
    if(path && (file[0] == '/')) {
      for(i=strlen(file); i>=0 && file[i] != '/'; i--);
    }
    strcpy(buf, path);
    strcpy(buf+strlen(buf), "/");
    strcpy(buf+strlen(buf), file+i);
    open_by_name(buf, handle);
    if(handle < 0) {
      write(fileno(stderr), CLEN("Could not open image file "));
      write(fileno(stderr), CLEN(buf));
      write(fileno(stderr), CLEN("\n"));
      exit(-1);
    }
  }

  nextfree=loadmod(nextfree, handle, NULL, 0L);

  close_file(handle);

  *((short *) linkinfo) = 0;

  linkit((char *)(heaps+2));

#ifdef CLEAR_SCREEN
  write(fileno(stdin), "\033[2J\033[H", 7);
  cout = 0;
  bf_at_query();
#endif

  go_bigforth();

  deprep_terminal();

  exit(0);
}
#endif
