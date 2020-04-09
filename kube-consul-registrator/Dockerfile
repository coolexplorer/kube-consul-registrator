FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS Final
WORKDIR /app
COPY --from=build /app .
EXPOSE 80

CMD ["dotnet", "kube-consul-registrator.dll"]
