\ MIDI class files                                     03jan98py

debugging class (midi-track
    method play
    method set
    method assign
    method restart
class;

\ MIDI player class                                    03jan98py

debugging class midi-player
public:
    cell var filebuf
    cell var filelen
    cell var fileptr
    cell var buffer
    cell var len
    cell var maxlen
    cell var seq
    cell var queue
    cell var port
    cell var tracks
    cell var ticks
    cell var start-ticks
    cell var tempo
    cell var division
    cell var start-time
    2 cells var real-ticks
    cell var playing
    cell var precision
    cell var note-buf
    cell var midi-task

    (midi-track [] track

    method file
    method start
    method stop
    method seek
    method sync
    method out
    method play
    method play-delta

    early pressure,
    early start,
    early stop,
    early key,
    early seq_control,
    early seq_patch,
    early seq_press,
    early seq_pitch,
    early seq_ctrl,
    early sysex,
    early tstart,
    early tstop,
    early tcont,
    early twait,
    early tdelta,
    early echo,
    early tempo,
    early tsig,
    early seq-open
    early seq-close

    method patch-buf
class;

