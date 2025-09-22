# ===== 1) BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia SOLO el csproj correcto (en raíz) y restaura
COPY Bank.Api.csproj ./
RUN dotnet restore Bank.Api.csproj

# Copia el resto del código y publica
COPY . .
RUN dotnet publish Bank.Api.csproj -c Release -o /out /p:UseAppHost=false

# ===== 2) RUNTIME =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Bank.Api.dll"]


