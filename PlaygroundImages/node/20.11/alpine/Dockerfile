FROM node:20.11.0-alpine3.19

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

RUN apk add --upgrade --no-cache bash
RUN npm i -g npm && \
  npm i -g eslint
COPY --chown=aaronsarkissian:aaronsarkissian .eslintrc.json /home/files/

WORKDIR /usercode

USER $AARON_UID