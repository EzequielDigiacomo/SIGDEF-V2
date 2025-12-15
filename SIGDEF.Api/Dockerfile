# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar el archivo de solución
COPY ["SIGDEF.sln", "./"]

# 2. Copiar los archivos .csproj de cada proyecto
COPY ["01.Api/SIGDEF.Api.csproj", "01.Api/"]
COPY ["02.Controlador/SIGDEF.Controlador.csproj", "02.Controlador/"]
COPY ["03.AccessDatos/SIGDEF.AccessDatos.csproj", "03.AccessDatos/"]
COPY ["04.Entidades/SIGDEF.Entidades.csproj", "04.Entidades/"]
COPY ["05.Pagos/SIGDEF.Pagos.csproj", "05.Pagos/"]

# 3. Restaurar dependencias
RUN dotnet restore "SIGDEF.sln"

# 4. Copiar TODO el código fuente
COPY . .

# 5. Publicar el proyecto API (proyecto de entrada)
WORKDIR "/src/01.Api"
RUN dotnet publish "SIGDEF.Api.csproj" -c Release -o /app/publish

# Etapa 2: Runtime (imagen más liviana)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Variables de entorno para Render/Railway/etc
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Puerto que usará la app
EXPOSE 8080

# Copiar los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Comando de inicio
ENTRYPOINT ["dotnet", "SIGDEF.Api.dll"]