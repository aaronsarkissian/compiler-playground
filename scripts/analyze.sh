#!/bin/bash

runtime=$1
compiler=$2
file=$3
output=$4
addtionalArg=$5

exec  1> $"/usercode/logfile.txt"
exec  2> $"/usercode/errors"

if [[ $@ == *"eslint"* ]]; then
    cp /home/files/.eslintrc.json /usercode/
fi

if [ "$output" = "" ]; then
    if [[ $compiler == *"g++"* ]] || [[ $compiler == *"gcc"* ]] || [[ $@ == *"cs"* ]] || [[ $@ == *"java"* ]] || [[ $@ == *"go"* ]]; then
        $compiler /usercode/$file
    else
        timeout "$runtime" $compiler /usercode/$file
    fi
    status=$?
    if [ "$status" -eq 143 ]; then
        echo -e "\nExecution Timed Out!" >> /usercode/errors
        echo -e "4" > /usercode/errorCode
    elif [ "$status" -ne 0 ];  then
        echo -e "5" > /usercode/errorCode
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

if [[ $@ == *"cs"* ]] && [ ! -s /usercode/logfile.txt ]; then
    echo "not empty" > /usercode/logfile.txt
fi
if [[ $@ == *"java"* ]] && [ ! -s /usercode/errors ]; then
    echo "not empty" > /usercode/errors
fi
if [[ $@ == *"go"* ]] && [ ! -s /usercode/errors ]; then
    echo "not empty" > /usercode/errors
fi

if [[ $@ == *"g++"* ]] || [[ $@ == *"gcc"* ]] || [[ $@ == *"java"* ]] || [[ $@ == *"go"* ]]; then
    mv /usercode/errors /usercode/analyze
elif [[ $@ == *"eslint"* ]] || [[ $@ == *"cs"* ]]; then
    mv /usercode/logfile.txt /usercode/analyze
else
    mv /usercode/logfile.txt /usercode/analyze
fi