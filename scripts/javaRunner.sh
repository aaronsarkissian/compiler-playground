#!/bin/bash

# This script disassembles the compiled java files
# find the main function and execute it

cd /usercode/

for classfile in *.class; do
    classname=${classfile%.*}
    
    #Execute fgrep with -q option to not display anything on stdout when the match is found
    if javap -public $classname | fgrep -q -e 'public static void main(java.lang.String[])' \
                                           -e 'public static final void main(java.lang.String[])' \
                                           -e 'public static void main(java.lang.String...)' \
                                           -e 'public static final void main(java.lang.String...)'; then
        java $classname "$@"
    fi
done
