#!/bin/bash
set -e

echo "Building all the Docker Images"

hostname='aaronsarkissian'

for dockerfile in */*/*/Dockerfile; do
    tagPath=$(dirname "$dockerfile")
    imagePath=$(dirname "$tagPath")
    lang=$(dirname $imagePath)
    distro=$(basename "$tagPath")
    version=$(basename "$imagePath")
    tag=${version}-${distro}
    imageName="${hostname}/${lang}:${tag}"

    # Build image
    (
        cd "$tagPath"
        docker build -t "$imageName" .
    )
done