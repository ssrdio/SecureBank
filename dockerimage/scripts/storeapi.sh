#!/bin/bash

sleep 10s

cd StoreAPI

export DatabaseConnections__StoreMSSQL__UserId=sa
export DatabaseConnections__StoreMSSQL__UserPass=Your_Password123
export DatabaseConnections__StoreMSSQL__Server=127.0.0.1
export DatabaseConnections__StoreMSSQL__ServerPort=5432
export DatabaseConnections__StoreMSSQL__Database=store

export StoreAPI__Seed=true
export Ctf__Enabled=false
export Ctf__Seed=example
export "Ctf__FlagFormat=ctf{{{0}}}"
export Ctf__Challenges__Ssrf=true

./StoreAPI --urls http://0.0.0.0:5000