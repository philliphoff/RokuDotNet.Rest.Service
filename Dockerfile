FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
ENV ROKU_REST_SERVICE_CONNECTIONSTRING="<CONNECTION STRING>"
ENV ROKU_REST_SERVICE_DEVICEKEY="<DEVICE KEY>"
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/RokuDotNet.Rest.Service.csproj", "src/"]
RUN dotnet restore "src/RokuDotNet.Rest.Service.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "RokuDotNet.Rest.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RokuDotNet.Rest.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RokuDotNet.Rest.Service.dll"]
