#!/bin/bash

set -e
run_cmd="dotnet StoreAPI.dll --server.urls http://*:80"

sleep 30

>&2 echo "SQL Server is up - executing command"
exec $run_cmd
