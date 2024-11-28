# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar los archivos de la solución y restaurar las dependencias
COPY *.sln .
COPY apiFestivos.*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}/; done
RUN dotnet restore

# Copiar todo el código fuente y construir la solución
COPY . .
WORKDIR /source/apiFestivos.Presentacion
RUN dotnet publish -c Release -o /app

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "apiFestivos.Presentacion.dll"]

