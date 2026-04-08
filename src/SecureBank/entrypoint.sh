#!/bin/bash

set -e

# Default to port 80 if ASPNETCORE_PORT is not set
PORT=${ASPNETCORE_PORT:-80}

# Build the server URLs - support multiple ports for flexibility
run_cmd="dotnet SecureBank.dll --urls http://0.0.0.0:${PORT}"

echo "Going to sleep for 30 seconds waiting for SQL server run"

sleep 30

>&2 echo "SQL Server is up - executing command on port ${PORT}"
exec $run_cmd
