#!/bin/bash

ctfMode=`printenv AppSettings:Ctf:Enabled`

export ctfMode=${ctfMode:-false}

scripts/securebank.sh &
scripts/storeapi.sh &
scripts/maildev.sh &

export ACCEPT_EULA=y
export SA_PASSWORD=Your_Password123

/opt/mssql/bin/sqlservr