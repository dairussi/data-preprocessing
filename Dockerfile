FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

RUN apt-get update && apt-get install -y \
    libc6-dev \
    libstdc++6 \
    && rm -rf /var/lib/apt/lists/*

COPY *.csproj .
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o out

RUN dotnet tool install --global dotnet-ef

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

RUN apt-get update && apt-get install -y \
    libc6-dev \
    libstdc++6 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

COPY --from=build /root/.dotnet /root/.dotnet
ENV PATH="${PATH}:/root/.dotnet/tools"


ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "DataPreprocessing.dll"]