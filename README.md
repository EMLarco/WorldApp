# WorldApp - Aplicacion web con ASP.NET Core MVC y base de datos world

## Descripcion

WorldApp es una aplicacion web desarrollada con ASP.NET Core MVC que permite gestionar informacion de paises, ciudades e idiomas a partir de la base de datos de ejemplo world. La aplicacion incluye:

- Autenticacion de usuarios (registro, inicio de sesion, cierre de sesion) con ASP.NET Core Identity.
- CRUD completo sobre la tabla principal City (crear, leer, actualizar, eliminar logicamente).
- Relacion uno a muchos entre City y Country (una ciudad pertenece a un pais).
- Paginacion en el listado de ciudades con selector de tamaño de pagina y limite de 5 numeros visibles.
- Busqueda por nombre de ciudad o pais.
- Eliminacion logica (soft delete) usando el campo IsDeleted.
- Validaciones con DataAnnotations y mensajes de error personalizados.
- Manejo de errores controlado sin mostrar excepciones tecnicas al usuario.
- Diseño responsivo con Bootstrap 5, iconos FontAwesome y fondo de imagen semitransparente.

## Tecnologias utilizadas

- .NET SDK 8.0 o superior (compatible con .NET 10)
- ASP.NET Core MVC
- Entity Framework Core 8.0.x
- SQL Server (Developer / Express)
- Bootstrap 5.3
- FontAwesome 6.0
- jQuery Validation 1.19.5

## Estructura del proyecto

<img width="1103" height="1124" alt="image" src="https://github.com/user-attachments/assets/2680c4c4-228a-4a63-9a6e-c7bac35d599b" />

## Instalacion y ejecucion local

### Requisitos previos

- .NET 8 SDK o superior.
- SQL Server (Developer o Express).
- SQL Server Management Studio (SSMS) opcional.
- Git.

### Pasos

1. Clonar el repositorio
   git clone https://github.com/EMLarco/WorldApp.git
   cd WorldApp

2. Configurar la base de datos
   - Abrir SSMS y conectarse a la instancia local.
   - Ejecutar el script de la base de datos world (incluido en la carpeta Database/ o proporcionado por el docente).
   - Verificar que las tablas country, city y countrylanguage tengan datos.
   - Asegurar que la columna IsDeleted exista en cada tabla (si no, ejecutar ALTER TABLE ADD IsDeleted BIT NOT NULL DEFAULT 0).

3. Configurar la cadena de conexion
   Editar appsettings.json:
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR;Database=world;Trusted_Connection=True;Encrypt=False;"
     }
   }
   Reemplazar TU_SERVIDOR por el nombre de la instancia (ej. DESKTOP-8O8V3AI\MSSQLSERVER01).

4. Restaurar paquetes y compilar
   dotnet restore
   dotnet build

5. Crear las tablas de Identity (AspNetUsers, etc.)
   Si no existen, ejecutar el script manual proporcionado en la documentacion o usar migraciones:
   dotnet ef migrations add InitialIdentity
   dotnet ef database update

6. Ejecutar la aplicacion
   dotnet run
   Abrir el navegador en https://localhost:5001 (o el puerto indicado).

## Credenciales de prueba

- Registro: cualquier correo electronico y contraseña con minimo 6 caracteres y al menos un digito.
- Usuario de prueba (crearlo manualmente):
   Email: prueba@example.com
   Contrasena: Prueba123

## Capturas de pantalla

<img width="2559" height="1305" alt="image" src="https://github.com/user-attachments/assets/187430ec-bdcb-4f4c-96bc-4d8a3e8bd762" />

## Licencia

Este proyecto se distribuye bajo la licencia MIT.
