FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
COPY out/ .
ENTRYPOINT ["dotnet", "AutoAPI.Web.dll"]