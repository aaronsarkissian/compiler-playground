#!/bin/bash
set -e

to=$1
shift

path=$(echo $3 | sed 's/.*\/usercode\/\(.*\):\/usercode/\1/')
# --memory="400m"           # 400MB
# --cpus="1.0"              # 1 Core
# --ulimit fsize=50000000   # 50MB

if [[ $@ == *"py"* ]] || [[ $@ == *"python"* ]]; then
    chown -R aaronsarkissian:aaronsarkissian /usercode/$path
    cont=$(docker run --ulimit fsize=20000000 --rm --memory="1500m" -d "$@")
elif [[ $@ == *"POSTGRES"* ]]; then
    chown -R postgres:postgres /usercode/$path
    cont=$(docker run --ulimit fsize=50000000 --rm --memory="1500m" --shm-size=256MB -d "$@")
elif [[ $@ == *"java"* ]]; then
    chown -R aaronsarkissian:aaronsarkissian /usercode/$path
    cont=$(docker run --ulimit fsize=40000000 --rm --memory="1800m" -d "$@")
else
    chown -R aaronsarkissian:aaronsarkissian /usercode/$path
    cont=$(docker run --ulimit fsize=20000000 --rm --memory="800m" -d "$@")
fi

code=$(timeout "$to" docker wait "$cont" || true)
docker kill $cont &> /dev/null
if [ -z "$code" ]; then
    
    maxsize=100000 # 100KB
    filesize=$(stat -c%s /usercode/$path/logfile.txt)
    if (( filesize > maxsize )); then
        head -c 2048 /usercode/$path/logfile.txt > /usercode/$path/small_logfile.txt
        rm /usercode/$path/logfile.txt
        mv /usercode/$path/small_logfile.txt /usercode/$path/logfile.txt
    fi
    
    echo -e "\nExecution Timed Out!" >> /usercode/$path/errors
    echo -e "4" > /usercode/$path/errorCode
else
    echo exited: $code
fi

docker rm $cont &> /dev/null
exit 0
