#!/bin/bash

sleep 30s

cd StoreAPI

export DatabaseConnections__StoreMSSQL__UserId=sa
export DatabaseConnections__StoreMSSQL__UserPass=Your_Password123
export DatabaseConnections__StoreMSSQL__Server=127.0.0.1
export DatabaseConnections__StoreMSSQL__ServerPort=1433
export DatabaseConnections__StoreMSSQL__Databbase=store

export StoreAPI__Seed=true

./StoreAPI --urls http://0.0.0.0:5000