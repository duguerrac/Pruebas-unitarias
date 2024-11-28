# Imagen base para la etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar los archivos de solución y de proyecto
COPY *.sln .
COPY apiFestivos.*/*.csproj ./

# Mover archivos .csproj a sus directorios correspondientes
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}/; done

# Restaurar las dependencias
RUN dotnet restore

# Copiar el resto del código fuente y compilar
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Imagen base para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "apiFestivos.Presentacion.dll"]
