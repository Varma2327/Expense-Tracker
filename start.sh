#!/bin/sh
# Use Railway's runtime port if provided; default 8080 for local docker runs
export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}"
exec dotnet FinanceManager.Web.dll
