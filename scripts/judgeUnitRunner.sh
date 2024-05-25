#!/bin/bash

runtime=$1
compiler=$2
file=$3
output=$4
addtionalArg=$5

exec  1> $"/usercode/logfile.txt"
exec  2> $"/usercode/errors"

# Interpreted
if [ "$output" = "" ]; then
    if [ "$compiler" = "php" ]; then
        $compiler /usercode/$file 2> /dev/null
    else
        $compiler /usercode/$file
    fi
    if [ "$?" -ne 0 ];  then
        echo -e "5" >> /usercode/errorCode
    fi
# Compiled
else
    #In case of compile errors, redirect them to a file
    $compiler /usercode/$file $addtionalArg &> /usercode/errors
    if [ "$?" -eq 0 ];  then
        $output
        if [ "$?" -ne 0 ];  then
            echo -e "5" >> /usercode/errorCode
        fi
    else
        echo -e "2" >> /usercode/errorCode
    fi
fi

maxsize=100000 # 100KB
filesize=$(stat -c%s /usercode/logfile.txt)
if (( filesize > maxsize )); then
    head -c 2048 /usercode/logfile.txt > /usercode/small_logfile.txt
    rm /usercode/logfile.txt
    mv /usercode/small_logfile.txt /usercode/logfile.txt
    echo -e "\nExecution Timed Out!" > /usercode/errors
fi

mv /usercode/logfile.txt /usercode/completed