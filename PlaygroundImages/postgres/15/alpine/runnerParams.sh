#!/bin/bash

JUDGEFILE=/usercode/judgeIoRunnerSQL.sh
if [[ -f "$JUDGEFILE" ]]; then
    bash /usercode/judgeIoRunnerSQL.sh "10s" "psql -U postgres -w -q --csv" "*.sql" ""
else
    bash /usercode/sqlRunner.sh "10s" "psql -U postgres -w -H" "*.sql" ""
fi