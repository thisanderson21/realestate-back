## 🏠 Descripción del Proyecto

**MILLION** es una aplicación modular para la **gestión de propiedades inmobiliarias**, desarrollada con **.NET 8** bajo una **arquitectura limpia (Clean Architecture)** que separa las responsabilidades en capas bien definidas.  

Además, implementa el **patrón CQRS (Command and Query Responsibility Segregation)** para dividir las operaciones de lectura y escritura, mejorando la escalabilidad, mantenibilidad y testabilidad del sistema.

`Api`, `Application`, `Domain`, `Infrastructure`, y `Million.Tests`.

---
## 🚀 Requisitos Previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/downloads)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community) o Docker

```bash

  MILLION/
  ├── 📁 Api/                    # Capa de presentación (controladores, configuración)
  │   ├── Controllers/
  │   ├── Program.cs
  │   └── appsettings.json
  ├── 📁 Application/            # Lógica de aplicación (CQRS)
  │   ├── Commands/
  │   ├── Queries/
  │   ├── DTOs/
  │   └── Interfaces/
  ├── 📁 Domain/                 # Entidades de negocio
  │   └── Entities/
  ├── 📁 Infrastructure/         # Acceso a datos e implementación de repositorios
  │   ├── Persistence/
  │   └── Repositories/
  └── 📁 Million.Tests/          # Pruebas unitarias (NUnit)
      ├── Application/
      ├── Infrastructure/
      └── Domain/
```

## 🗄️ Configuración de la Base de Datos

### 🔹 Opción 1: MongoDB Local

Instala **MongoDB Community Server** desde el sitio oficial:  
👉 [https://www.mongodb.com/try/download/community](https://www.mongodb.com/try/download/community)

---

### 🔹 Opción 2: Docker Local

Ejecuta **MongoDB** en un contenedor con el siguiente comando:

```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```


## 🚀 Ejecución del Proyecto


### 🔹Restaurar dependencias
```bash
dotnet restore
```

### 🔹Compilar la solución
```bash
dotnet build
```

### 🔹Ejecuta la **API** junto con el **seeder** para generar datos aleatorios de prueba con el siguiente comando:

```bash
cd Api

dotnet run
```

## 🌐 Endpoints de la API

Una vez en ejecución, la API estará disponible en:

- **HTTP:** [http://localhost:5214](http://localhost:5214)
- **Swagger UI:** [http://localhost:5214/swagger](http://localhost:5214/swagger)

API PROPERTIES
http://localhost:5214/api/Properties

## 🧪 Pruebas y Cobertura

### 🔹 Ejecutar todas las pruebas
```bash
dotnet test
```
### 🔹 Ejecutar pruebas con cobertura
```bash
dotnet test --collect:"XPlat Code Coverage" -p:CoverletOutputFormat=cobertura -p:CoverletOutput=./TestResults/coverage.cobertura.xml

```


### 🔹 Generar reporte HTML de cobertura
```bash
reportgenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:coverage-report
```


## 👨‍💻 Autor

**Anderson Sepúlveda**  
Proyecto desarrollado como **prueba técnica**.  

📧 **andersonvargas383@gmail.com**
