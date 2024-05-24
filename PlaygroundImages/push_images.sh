#!/bin/bash
set -e

hostname='aaronsarkissian'

for dockerfile in */*/*/Dockerfile; do
    tagPath=$(dirname "$dockerfile")
    imagePath=$(dirname "$tagPath")
    lang=$(dirname $imagePath)
    distro=$(basename "$tagPath")
    version=$(basename "$imagePath")
    tag=${version}-${distro}
    imageName="${hostname}/${lang}:${tag}"

    echo
    echo "Pushing $imageName"
    docker push "$imageName"
done