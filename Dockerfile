# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy entire repo and publish just the Web project
COPY . .
WORKDIR /src/FinanceManager.Web
RUN dotnet publish -c Release -o /app/out

# ---- Run stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# app files
COPY --from=build /app/out .

# startup script will bind to Railway's $PORT
COPY start.sh /app/start.sh
RUN chmod +x /app/start.sh

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["/app/start.sh"]
