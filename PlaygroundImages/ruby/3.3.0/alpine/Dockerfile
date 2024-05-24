FROM ruby:3.3.0-alpine3.19

RUN apk add --upgrade --no-cache bash

# Create a non-root user and group
# UID of the non-root user 'aaronsarkissian'
ENV AARON_UID=4242
RUN addgroup \
  --gid=$AARON_UID \
  aaronsarkissian \
  && adduser \
  --uid=$AARON_UID \
  --ingroup=aaronsarkissian \
  --disabled-password \
  aaronsarkissian

WORKDIR /usercode

USER $AARON_UID