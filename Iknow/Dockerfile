FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Iknow/Iknow.csproj", "Iknow/"]
RUN dotnet restore "Iknow/Iknow.csproj"
COPY . .
WORKDIR "/src/Iknow"
RUN dotnet build "Iknow.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Iknow.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Iknow.dll"]