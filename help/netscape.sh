#!/bin/bash

(netscape -remote "OpenURL $1" || netscape $1) >/dev/null 2>/dev/null
