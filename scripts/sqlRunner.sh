#!/bin/bash

runtime=$1
compiler=$2
file=$3
output=$4
addtionalArg=$5

exec  1> $"/usercode/logfile.txt"
exec  2> $"/usercode/errors"

if [ "$output" = "" ]; then
    if [[ "$compiler" == *"psql"* ]]; then
        timeout "$runtime" $compiler < /usercode/$file > /usercode/dbresult.txt
        status=$?
        if [ "$status" -eq 143 ]; then
            echo -e "\nExecution Timed Out!" >> /usercode/errors
            echo -e "4" > /usercode/errorCode
        elif [ "$status" -ne 0 ];  then
            echo -e "5" > /usercode/errorCode
        fi
    fi
fi

sed '/\${sql_html_output}/{r /usercode/dbresult.txt
    d;}' /usercode/sqlHtmlTemplate.html > /usercode/htmlLogfile.txt

mv /usercode/htmlLogfile.txt /usercode/logfile.txt

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