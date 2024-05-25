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

# Interpreted
if [ "$output" = "" ]; then
    for f in $INPUTFILES
    do
        ct=$(echo $f | grep -o -E 'inputFile[0-9]+' | grep -o -E '[0-9]+')
        if [ "$compiler" = "php" ]; then
            timeout "$runtime" "$compiler" /usercode/$file -< "$f" 1> $OUTPUTFILES/output$ct 2> /dev/null
            status=$?
            if [ "$status" -eq 143 ]; then
                echo -e "\nExecution Timed Out!" >> /usercode/errors
                echo -e "4" > /usercode/errorCode
            elif [ "$status" -ne 0 ];  then
                echo -e "5" > /usercode/errorCode
            fi
        else
            timeout "$runtime" "$compiler" /usercode/$file -< "$f" 1> $OUTPUTFILES/output$ct 2> $OUTPUTFILES/error$ct
            status=$?
            if [ "$status" -eq 143 ]; then
                echo -e "\nExecution Timed Out!" >> /usercode/errors
                echo -e "\nExecution Timed Out!" >> $OUTPUTFILES/error$ct
                echo -e "4" > /usercode/errorCode
            elif [ "$status" -ne 0 ];  then
                echo -e "5" > /usercode/errorCode
            fi
        fi
        head -c 2048 $OUTPUTFILES/output$ct > $OUTPUTFILES/new_output$ct
        rm $OUTPUTFILES/output$ct
        mv $OUTPUTFILES/new_output$ct $OUTPUTFILES/output$ct
    done
# Compiled
else
    #In case of compile outputs (stdout, stderr), redirect them to the errors file
    $compiler /usercode/$file $addtionalArg &> /usercode/errors
    if [ "$?" -eq 0 ];  then
        for f in $INPUTFILES
        do
            ct=$(echo $f | grep -o -E 'inputFile[0-9]+' | grep -o -E '[0-9]+')
            timeout "$runtime" "$output" -< "$f" 1> $OUTPUTFILES/output$ct 2> $OUTPUTFILES/error$ct
            status=$?
            if [ "$status" -eq 143 ]; then
                echo -e "\nExecution Timed Out!" >> /usercode/errors
                echo -e "\nExecution Timed Out!" >> $OUTPUTFILES/error$ct
                echo -e "4" > /usercode/errorCode
            elif [ "$status" -ne 0 ];  then
                echo -e "5" > /usercode/errorCode
            fi
            head -c 2048 $OUTPUTFILES/output$ct > $OUTPUTFILES/new_output$ct
            rm $OUTPUTFILES/output$ct
            mv $OUTPUTFILES/new_output$ct $OUTPUTFILES/output$ct
        done
    else
        echo -e "2" > /usercode/errorCode
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