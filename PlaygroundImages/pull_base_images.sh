#!/bin/bash
set -e

for dockerfile in */*/*/Dockerfile; do
    image=$(grep ^FROM "$dockerfile" | awk '{print $2}')
    docker pull "$image"
done