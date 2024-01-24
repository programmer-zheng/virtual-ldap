#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# 若需要将abp的前端包在Docker编译时构建，可参考 https://blog.antosubash.com/posts/abp-docker-file
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ContactsSync.sln

FROM build AS publish
RUN dotnet publish "ContactsSync.Web/ContactsSync.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactsSync.Web.dll"]