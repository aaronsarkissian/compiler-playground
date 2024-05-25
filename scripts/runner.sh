#!/bin/bash

runtime=$1
compiler=$2
file=$3
output=$4
addtionalArg=$5

# This is how this works
# It checks the output argument, if it is empty then it is Interpreted
# if not, it is Compiled. In case of compile code, it checks to see the
# compilation output code. Success is 0.
# Then there is a snippet which checks the output size of the code.
# If it is too long it just takes the first 2K chars.
# At the end it renames the logfile to completed.
# Completed file is required so the dotnet app can search for it, read it
# and get its result. Finally delete the related files and directories.
#

exec  1> $"/usercode/logfile.txt"
exec  2> $"/usercode/errors"

# Interpreted
if [ "$output" = "" ]; then
    if [ "$compiler" = "php" ]; then
        timeout "$runtime" "$compiler" /usercode/$file -< $"/usercode/inputFile" 2> /dev/null
        status=$?
        if [ "$status" -eq 143 ]; then
            echo -e "\nExecution Timed Out!" >> /usercode/errors
            echo -e "4" > /usercode/errorCode
        elif [ "$status" -ne 0 ];  then
            echo -e "5" > /usercode/errorCode
        fi
    elif [[ $compiler == *"py"* ]] || [[ $compiler == *"python"* ]] || [[ $compiler == *"Rscript"* ]]; then
        cd /usercode
        timeout "$runtime" "$compiler" $file -< $"inputFile"
        status=$?
        if [ "$status" -eq 143 ]; then
            echo -e "\nExecution Timed Out!" >> errors
            echo -e "4" > errorCode
        elif [ "$status" -ne 0 ];  then
            echo -e "5" > errorCode
        fi
        cd ..
    else
        timeout "$runtime" "$compiler" /usercode/$file -< $"/usercode/inputFile"
        status=$?
        if [ "$status" -eq 143 ]; then
            echo -e "\nExecution Timed Out!" >> /usercode/errors
            echo -e "4" > /usercode/errorCode
        elif [ "$status" -ne 0 ];  then
            echo -e "5" > /usercode/errorCode
        fi
    fi
# Compiled
else
    #In case of compile errors, redirect them to a file
    $compiler /usercode/$file $addtionalArg &> /usercode/errors
    if [ "$?" -eq 0 ];  then
        if [[ $compiler == *"tsc"* ]]; then
            timeout "$runtime" $output -< $"/usercode/inputFile"
        else
            timeout "$runtime" "$output" -< $"/usercode/inputFile"
        fi
        status=$?
        if [ "$status" -eq 143 ]; then
            echo -e "\nExecution Timed Out!" >> /usercode/errors
            echo -e "4" > /usercode/errorCode
        elif [ "$status" -ne 0 ];  then
            echo -e "5" > /usercode/errorCode
        fi
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