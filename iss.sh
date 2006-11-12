#!/bin/bash
# This is the horror shell script to create an automatic install for
# Windoze.
# Note that I use sed to create a setup file
# Some files have special names and positions in my development
# environment, I replace them with normal file names in the process.

# use iss.sh >bigforth.iss
# copy the resulting *.iss to the location of your Windows installation
# of bigFORTH, and start the setup compiler there.

cat <<EOT
; This is the setup script for bigFORTH on Windows
; Setup program is Inno Setup

[Setup]
AppName=bigFORTH
AppVerName=bigFORTH $(eval echo $(cat version.h)) + MINOS $(eval echo $(cat minos-version.h)) from $(date '+%d%b%Y' | tr '[A-Z]' '[a-z]')
AppCopyright=Copyright © 1998-$(date '+%Y') by Bernd Paysan
DefaultDirName={pf}\bigforth
DefaultGroupName=bigFORTH
AllowNoIcons=1
InfoBeforeFile=COPYING
Compression=bzip
DisableStartupPrompt=yes

[Messages]
WizardInfoBefore=License Agreement
InfoBeforeLabel=bigFORTH is free software.
InfoBeforeClickLabel=You don't have to accept the GPL to run the program. You only have to accept this license if you want to modify, copy, or distribute this program.

[Dirs]
Name: "{app}\icons"
Name: "{app}\help"
Name: "{app}\pattern"
Name: "{app}\doc"

[Files]
; Parameter quick reference:
;   "Source filename", "Dest. filename", Copy mode, Flags
$((cd ..; eval ls $(cd bigforth; make dist-files)) | sed \
  -e 's#/#\\#g' \
  -e 's#^bigforth\\\(README\)$#Source: "\1.txt"; DestDir:"{app}"; Flags: isreadme#g' \
  -e 's,^bigforth\\\(..*\)\\\([^\\]*\)$,Source: "\1\\\2"; DestDir: "{app}\\\1",g' \
  -e 's#^bigforth\\\(.*\)$#Source: "\1"; DestDir: "{app}"#g' \
  -e 's#\(icons\\.*\)\.png#\1.icn#g' \
  -e 's#\(pattern\\.*\)\.png#\1.ppm#g' \
  | grep -v 'icons\\edit..*\.icn' | sort -u)

[Icons]
; Parameter quick reference:
;   "Icon title", "File name", "Parameters", "Working dir (can leave blank)",
;   "Custom icon filename (leave blank to use the default icon)", Icon index
Name: "{group}\MINOS"; Filename: "{app}\xbigforth.exe"; WorkingDir: "{app}"; IconFilename: "{app}\minos.ico"
Name: "{group}\Theseus"; Filename: "{app}\xbigforth.exe"; Parameters: "theseus.fs"; WorkingDir: "{app}"; IconFilename: "{app}\minos.ico"
Name: "{group}\bigFORTH"; Filename: "{app}\bigforth.exe"; WorkingDir: "{app}"
Name: "{group}\Remake Kernel"; Filename: "{app}\bigforth.exe"; Parameters: "##include forth.fb save-target forthker bye"; WorkingDir: "{app}"
Name: "{group}\Remake bigFORTH"; Filename: "{app}\forthker.exe"; Parameters: "##include startup.fb warning on savesystem bigforth bye"; WorkingDir: "{app}"
Name: "{group}\Remake Float Module"; Filename: "{app}\bigforth.exe"; Parameters: "##include float.fb m' float savemod float bye"; WorkingDir: "{app}"
Name: "{group}\Remake Minos"; Filename: "{app}\bigforth.exe"; Parameters: "##use float.fb path ;. include startx.fb savesystem xbigforth bye"; WorkingDir: "{app}"
Name: "{group}\Uninstall bigFORTH"; Filename: "{uninstallexe}"

[Registry]
; Parameter quick reference:
;   "Root key", "Subkey", "Value name", Data type, "Data", Flags

[Run]
Filename: "{app}\forthker.exe"; Workingdir: "{app}"; Parameters: "include starup.fb ' .blk is .status warning on savesystem bigforth bye"
Filename: "{app}\bigforth.exe"; WorkingDir: "{app}"; Parameters: "##use x.fs path ;. include float.fb m' float savemod float bye"
Filename: "{app}\bigforth.exe"; WorkingDir: "{app}"; Parameters: "##use x.fs use float.fb path ;. include glconst.fs m' glconst savemod glconst bye"
Filename: "{app}\bigforth.exe"; WorkingDir: "{app}"; Parameters: "##use x.fs use float.fb use glconst.fs path ;. include startx.fb warning on savesystem xbigforth bye"
Filename: "{app}\xbigforth.exe"; WorkingDir: "{app}"; Parameters: "##path ';{app};.' include adjust.m"

[UninstallDelete]
Type: files; Name: "{app}\bigforth.fi"
Type: files; Name: "{app}\xbigforth.fi"
Type: files; Name: "{app}\float.fm"
Type: files; Name: "{app}\glconst.fm"
Type: files; Name: "{app}\test.out"
Type: files; Name: "{app}\wave.trc"
EOT
