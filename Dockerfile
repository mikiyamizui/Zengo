FROM mcr.microsoft.com/dotnet/core/sdk:latest AS base
RUN apt-get update \
        && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
        && rm -rf /var/lib/apt/lists/*

FROM base AS test-env
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet build -c Release
ENTRYPOINT [ "dotnet", "test", "test/Test.csproj" ]
