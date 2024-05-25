#!/bin/bash

dockerd &

#Playground Images
docker pull aaronsarkissian/c-cpp:gcc-13.2-bookworm
docker pull aaronsarkissian/cs:mono-6.12-v2
docker pull aaronsarkissian/java:22-bookworm
docker pull aaronsarkissian/python:3.9-bookworm
docker pull aaronsarkissian/php:8.3.2-alpine
docker pull aaronsarkissian/ruby:3.3.0-alpine
docker pull aaronsarkissian/swift:4.2.4-ubuntu-v2
docker pull aaronsarkissian/node:20.11-alpine
docker pull aaronsarkissian/postgres:15-alpine-v2
docker pull aaronsarkissian/go:1.21.6-alpine
docker pull aaronsarkissian/r-lang:4.3.2-debian
docker pull aaronsarkissian/ts:20.11-alpine

touch /tmp/running && dotnet PlaygroundCompiler.dll
