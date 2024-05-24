FROM r-base:4.3.2

# Create a non-root user and group
# UID of the non-root user 'aaronsarkissian'
ENV AARON_UID=4242
RUN groupadd \
  --gid=$AARON_UID \
  aaronsarkissian \
  && useradd \
  --uid=$AARON_UID \
  --gid=aaronsarkissian \
  aaronsarkissian

WORKDIR /usercode

USER $AARON_UID