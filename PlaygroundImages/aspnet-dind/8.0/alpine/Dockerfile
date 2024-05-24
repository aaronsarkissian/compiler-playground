# Docker Azure TAG: aaronsarkissian/aspnet-dind:8.0
# Alpine 3.20

FROM docker:26.1.3-dind-alpine3.20

ENV \
    # UID of the non-root user 'app'
    APP_UID=1654 \
    # Configure web servers to bind to port 8080 when present
    ASPNETCORE_HTTP_PORTS=8080 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    # Set the invariant mode since ICU package isn't included (see https://github.com/dotnet/announcements/issues/20)
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

RUN apk add --upgrade --no-cache \
    ca-certificates-bundle \
    \
    # .NET dependencies
    libgcc \
    libssl3 \
    libstdc++ \
    zlib

# Create a non-root user and group
RUN addgroup \
    --gid=$APP_UID \
    app \
    && adduser \
    --uid=$APP_UID \
    --ingroup=app \
    --disabled-password \
    app

# .NET Runtime version
ENV DOTNET_VERSION=8.0.3

# Install .NET Runtime
RUN wget -O dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Runtime/$DOTNET_VERSION/dotnet-runtime-$DOTNET_VERSION-linux-musl-x64.tar.gz \
    && dotnet_sha512='bbed0cf924d103e15d07e069522fc89d921e8d91adccbd4e161345b52fc8bdb26837a18c83d06ccd092d14d3df0e6acbe3b8d348e0825822807a1cbc1c8f549f' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -oxzf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# ASP.NET Core version
ENV ASPNET_VERSION=8.0.3

# Install ASP.NET Core
RUN wget -O aspnetcore.tar.gz https://dotnetcli.azureedge.net/dotnet/aspnetcore/Runtime/$ASPNET_VERSION/aspnetcore-runtime-$ASPNET_VERSION-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='438ed9f5fef9cc63bae18f52af4209a80d8265ef6f9c7b92661e5276538b76163a79e6c59f5fe3d40133e8cdbed7ba50135ce365194358f4abe9df9231a124a5' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && tar -oxzf aspnetcore.tar.gz -C /usr/share/dotnet ./shared/Microsoft.AspNetCore.App \
    && rm aspnetcore.tar.gz