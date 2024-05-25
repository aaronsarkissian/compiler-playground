FROM aaronsarkissian/aspnet-dind:8.0-alpine AS base
WORKDIR /app
COPY startup.sh /app
WORKDIR /app/scripts
COPY scripts/ /app/scripts
RUN apk add --no-cache bash
EXPOSE 8778

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/PlaygroundCompiler.Web/PlaygroundCompiler.Web.csproj", "PlaygroundCompiler.Web/"]
COPY ["src/PlaygroundCompiler.Infrastrucutre/PlaygroundCompiler.Infrastrucutre.csproj", "PlaygroundCompiler.Infrastrucutre/"]
RUN dotnet restore "PlaygroundCompiler.Web/PlaygroundCompiler.Web.csproj"
COPY src/ .
WORKDIR "/src/PlaygroundCompiler.Web"
RUN dotnet build "PlaygroundCompiler.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlaygroundCompiler.Web.csproj" --os linux --arch x64 -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN addgroup -g 70 -S postgres; \
	adduser -u 70 -S -D -G postgres -H -h /var/lib/postgresql -s /bin/sh postgres;

ENV AARON_UID=4242
RUN addgroup \
	--gid=$AARON_UID \
	aaronsarkissian \
	&& adduser \
	--uid=$AARON_UID \
	--ingroup=aaronsarkissian \
	--disabled-password \
	aaronsarkissian

ENTRYPOINT ["./startup.sh"]
