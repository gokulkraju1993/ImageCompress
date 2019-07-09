FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

FROM microsoft/dotnet:2.1-sdk AS build

# install System.Drawing native dependencies



WORKDIR /src
COPY ["ImageCompress/ImageCompress.csproj", "ImageCompress/"]
RUN dotnet restore "ImageCompress/ImageCompress.csproj"
COPY . .
WORKDIR "/src/ImageCompress"
RUN dotnet build "ImageCompress.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ImageCompress.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ImageCompress.dll"]


