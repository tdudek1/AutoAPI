FROM microsoft/dotnet:2.1.1-aspnetcore-runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
COPY AutoAPI.Web/out/ .
ENTRYPOINT ["dotnet", "AutoAPI.Web.dll"]