#!/bin/bash

runtime=$1
compiler=$2
file=$3
output=$4
addtionalArg=$5

INPUTFILES=/usercode/inputs/*
OUTPUTFILES=/usercode/outputs

exec  1> $"/usercode/logfile.txt"
exec  2> $"/usercode/errors"

if [ "$output" = "" ]; then
    for f in $INPUTFILES
    do
        ct=$(echo $f | grep -o -E 'inputFile[0-9]+' | grep -o -E '[0-9]+')
        if [[ "$compiler" == *"psql"* ]]; then
            timeout "$runtime" $compiler < /usercode/$file 1> $OUTPUTFILES/output$ct 2> $OUTPUTFILES/error$ct
            status=$?
            if [ "$status" -eq 143 ]; then
                echo -e "\nExecution Timed Out!" >> /usercode/errors
                echo -e "4" > /usercode/errorCode
            elif [ "$status" -ne 0 ];  then
                echo -e "5" > /usercode/errorCode
            fi
        fi
        head -c 2048 $OUTPUTFILES/output$ct > $OUTPUTFILES/new_output
        rm $OUTPUTFILES/output$ct
        mv $OUTPUTFILES/new_output $OUTPUTFILES/output$ct
    done
fi

maxsize=300000 # 300KB
filesize=$(stat -c%s /usercode/logfile.txt)
if (( filesize > maxsize )); then
    head -c 2048 /usercode/logfile.txt > /usercode/small_logfile.txt
    rm /usercode/logfile.txt
    mv /usercode/small_logfile.txt /usercode/logfile.txt
    echo -e "\nExecution Timed Out!" > /usercode/errors
fi

mv /usercode/logfile.txt /usercode/completed
pg_ctl stop -m immediate