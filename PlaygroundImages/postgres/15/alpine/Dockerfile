FROM postgres:15.5-alpine3.19

RUN apk add --upgrade --no-cache bash

WORKDIR /docker-entrypoint-initdb.d
ADD playground.sql /docker-entrypoint-initdb.d
ADD runnerParams.sh /docker-entrypoint-initdb.d

# Existing user in base image (postgres with ID 70) 
USER postgres