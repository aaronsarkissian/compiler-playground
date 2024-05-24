FROM python:3.9.18-slim-bookworm

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

COPY requirements.txt /tmp/

#Install python libs from requirements
RUN pip install --upgrade pip && pip install -r /tmp/requirements.txt

WORKDIR /usercode

USER $AARON_UID