#!/bin/bash

ctfMode=`printenv AppSettings:Ctf:Enabled`

export ctfMode=${ctfMode:-false}

# PostgreSQL environment variables
export POSTGRES_USER=${POSTGRES_USER:-sa}
export POSTGRES_PASSWORD=${POSTGRES_PASSWORD:-Your_Password123}
export POSTGRES_DB=${POSTGRES_DB:-securebank}
export PGDATA=/var/lib/postgresql/data

# Initialize PostgreSQL if not already initialized
if [ ! -s "$PGDATA/PG_VERSION" ]; then
    mkdir -p "$PGDATA"
    chown postgres:postgres "$PGDATA"
    chmod 700 "$PGDATA"
    su postgres -c "initdb -D $PGDATA"

    # Configure authentication
    echo "host all all 0.0.0.0/0 md5" >> "$PGDATA/pg_hba.conf"
    echo "host all all 127.0.0.1/32 trust" >> "$PGDATA/pg_hba.conf"
    echo "local all all trust" >> "$PGDATA/pg_hba.conf"
    echo "listen_addresses='127.0.0.1'" >> "$PGDATA/postgresql.conf"

    # Start PostgreSQL temporarily to create user and databases
    su postgres -c "pg_ctl -D $PGDATA -w start"

    su postgres -c "psql -c \"CREATE USER $POSTGRES_USER WITH PASSWORD '$POSTGRES_PASSWORD' SUPERUSER CREATEDB;\""

    su postgres -c "psql -c \"CREATE DATABASE securebank OWNER $POSTGRES_USER;\""
    su postgres -c "psql -c \"CREATE DATABASE store OWNER $POSTGRES_USER;\""

    su postgres -c "pg_ctl -D $PGDATA -w stop"
fi

# Start PostgreSQL in the background
su postgres -c "pg_ctl -D $PGDATA -w start"

scripts/securebank.sh &
scripts/storeapi.sh &
scripts/maildev.sh &

# Keep the container running
wait
