# Imagen base para la construcción
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# Copiar archivos de proyecto y restaurar dependencias
COPY *.sln .
COPY apiFestivos.*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}/; done
RUN dotnet restore

# Copiar todo el código fuente y compilar
COPY . .
WORKDIR /source/apiFestivos.Presentacion
RUN dotnet publish -c Release -o /app

# Imagen base para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "apiFestivos.Presentacion.dll"]
