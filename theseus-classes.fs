\ MINOS class declarations

debugging class links
    ptr next
    gadget ptr linked
    boxchar ptr linker
    method find-linked
    method find-linker
    method update-linked
    method update-linker
    method dump
class;

debugging class descriptor
public:
    gadget ptr item
    
    method edit-field
    method null
    method make
    method dump
    method post-dump
    method assign
    method get
    early delay
class;

debugging class hint-name
public:
    ptr next
    object ptr hint
    cell var name
    method find-name
    method update-hint
    method update-name
class;

stredit class codeedit
public:
    cell var ^content
    method add-lines
    method backup
class;

habox class resource:dialog
public:
    ptr next-resource
    terminal ptr callwind
    textfield ptr name-field
    textfield ptr title-field
    toggleicon ptr show-state
    ticonbutton ptr show-vars
    icon-but ptr menu-icon
    infotextfield ptr CF-field
    infotextfield ptr IF-field
    cell var shown
    combined ptr topbox
    codeedit ptr var-edit
    codeedit ptr methods-edit
    vabox ptr var-box
    vabox ptr methods-box
    cell var var-content
    cell var methods-content
    cell var class-file
    cell var implementation-file
    cell var default

    method dump-declaration
    method dump-implementation
    method dump-script
    method script?
    method find-name

    method base-class
    method dump-contents
    method dump-names'
    method link-designer
    method add-box
    method >cur
class;

resource:dialog class resource:menu-window
public:
    combined ptr menubox
    early .default
class;

menu-window class designer
public:
    method open
    method open-file
    method inside
    
    descriptor ptr action
    descriptor ptr toggle
    descriptor ptr slider
    descriptor ptr string
    descriptor ptr text
    descriptor ptr num
    descriptor ptr hglue
    descriptor ptr vglue
    descriptor ptr step
    descriptor ptr icon
    
    resource:dialog ptr resources

    viewport ptr pane
    backing ptr back
    combined ptr box
    widget ptr widget
    combined ptr status
    combined ptr topbox
    combined ptr menubox
    rule ptr end-rule
    terminal ptr callwind
    hint-name ptr cur-box-name
    descriptor ptr cur-dpy
    textfield ptr cur-box-edit
    textfield ptr cur-dpy-edit

    cell var +boxmode
    cell var +activate
    cell var +radio
    cell var +tabbing
    cell var +tabular
    cell var +hskip
    cell var +vskip
    cell var +hfixbox
    cell var +vfixbox
    cell var +flipbox
\    cell var +rzbox
    cell var +borderw
    cell var +noborder

    cell var file-name

    cell var save-state  \ 0: saved  1: autosaved  -1: unsaved
class;

vabox class designerbox
public:
    method <box>
    cell var cursor
    early draw-decor
class;
