#!/bin/bash

set -e
run_cmd="dotnet SecureBank.dll --server.urls http://*:80"

echo "Going to sleep for 30 seconds waiting for SQL server run"

sleep 30

>&2 echo "SQL Server is up - executing command"
exec $run_cmd
