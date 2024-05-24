FROM mono:6.12.0.182

RUN apt-get purge -y curl && apt-get autoclean -y && \
  apt-get autoremove -y && \
  rm -rf /var/lib/apt/lists/*

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
