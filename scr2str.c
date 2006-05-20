/*
 * Screen to stream converter and vice versa
 * used to filter screen files for CVS
 * written in C, so that I can run it on systems without installing Forth
 */

#include <stdio.h>
#include <string.h>
#include <errno.h>

#define B_BLK 0x400
#define C_L 0x40

int backscan(char * block, int n, char c)
{
  for(n--; n>=0 && (block[n] == c); n--);
  return n < 0 ? 0 : n + (block[n] != c);
}

int ctrlmask(char * p, char * o, int n)
{
  int i, k=0;

  for(i=0; i<n; i++) {
    switch(p[i]) {
    case '\0' ... ' '-1:
      *o++ = '^';
      *o++ = p[i] + '@';
      k+=2;
      break;
    case '^':
      *o++ = '^';
      *o++ = '~';
      k+=2;
      break;
    default:
      *o++ = p[i];
      k++;
      break;
    }
  }
  return k;
}

int ctrlunmask(char * p, char * o, int n)
{
  int i, k=0;

  for(i=0; i<n; i++) {
    switch(p[i]) {
    case '^':
      if(p[++i] == '~') {
	*o++ = '^';
      } else {
	*o++ = p[i] - '@';
      }
      k++;
      break;
    default:
      *o++ = p[i];
      k++;
      break;
    }
  }
  return k;
}

void scr2str(FILE * in, FILE * out)
{
  char block[B_BLK];
  char line[C_L*2];
  int n, i, k;

  do {
    memset(block, ' ', B_BLK);
    n = fread(block, 1, B_BLK, in);
    if(n == 0) break;
    n = backscan(block, n, ' ');
    for(i=0; i < n; i+= C_L) {
      k = backscan(block+i, C_L, ' ');
      k = ctrlmask(block+i, line, k);
      fwrite(line, 1, k, out);
      fwrite("\n", 1, 1, out);
    }
    fwrite("\f\n", 2, 1, out);
  } while(1);
}

void str2scr(FILE * in, FILE * out)
{
  char block[B_BLK];
  char line[2*C_L+3];
  int n, i=0, k=0;

  memset(block, ' ', B_BLK);

  while(!feof(in)) {
    fgets(line, 2*C_L+2, in);
    if(feof(in)) break;
    if(!strcmp(line, "\f\n")) {
      fwrite(block, 1, B_BLK, out);
      memset(block, ' ', B_BLK);
      k = 0;
    } else {
      n = strlen(line);
      if(n)
	n -= (line[n-1]=='\n');
      n = ctrlunmask(line, line, n);
      memcpy(block+k, line, n);
      k += C_L;
    }
  }
}

int main(int argc, char ** argv, char ** env)
{
  int arglen = strlen(argv[0])-strlen("scr2str");
  int what = !strcmp(argv[0]+arglen, "scr2str");

  FILE * in = stdin;
  FILE * out = stdout;

  if(argc > 1) {
    if(strcmp(argv[1], "-")) {
      if(!(in = fopen(argv[1], "rb"))) {
	fprintf(stderr, "Error opening input file: %s\n", strerror(errno));
	return -1;
      }
    }
  }
  if(argc > 2) {
    if(!(out = fopen(argv[2], "wb"))) {
      fprintf(stderr, "Error opening output file: %s\n", strerror(errno));
      return -1;
    }
  }
  if(what)
    scr2str(in, out);
  else
    str2scr(in, out);

  return 0;
}
