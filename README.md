# Car Rental

Proyecto ASP.NET Core 8 de renta de autos con autenticación JWT conectada a la base MySQL `CarRental` existente.

## Seguridad

| Método | Ruta | Resultado correcto |
| --- | --- | --- |
| `POST` | `/api/auth/register` | `200 OK`, usuario creado como `User` |
| `POST` | `/api/auth/login` | `200 OK`, token JWT Bearer |
| `GET` | `/api/auth/me` | `200 OK` con token; `401` sin token |

Las contraseñas se guardan con PBKDF2. El registro siempre asigna el rol `User`. La API reutiliza `IMySqlDataService` y los procedimientos almacenados del proyecto.

## Ejecutar

Ejecuta `database/CarRental.sql` en MySQL Workbench una vez. Luego abre `CarRental.sln` en Visual Studio y ejecuta el perfil **CarRental Local**. La cadena local se lee de `ConnectionStrings:MysqlConnection`; no agregues contraseñas a `appsettings.json` ni al repositorio. Si necesitas configurarla, usa User Secrets de Visual Studio. En desarrollo, la clave JWT se genera en memoria si no defines `Jwt:Key`.

Swagger: `http://localhost:5231/swagger`.

## Evidencia en Postman

Importa `postman/CarRentalSecurity.postman_collection.json` y envía, en orden, **Registro - usuario 1**, **Registro - usuario 2**, **Login - usuario 1** y **Perfil protegido - validar JWT**. Las dos altas y el login esperan `200 OK`; el JWT del login se guarda en la variable de colección `accessToken`.

La colección usa cuentas de demostración. No reutilices su contraseña fuera de esta práctica.

## Countries y Customers

El script `database/CarRental.sql` crea `isoCountries`, `Customers`, `auth_roles` y `auth_users`; agrega los tres registros de ejemplo por tabla de Countries y Customers, y define los procedimientos almacenados usados por la API.

| Método | Ruta | Resultado correcto |
| --- | --- | --- |
| `GET` | `/IsoCountries` | `200 OK`, lista de países |
| `GET` | `/IsoCountries/{id}` | `200 OK`, país encontrado |
| `POST` | `/IsoCountries` | `201 Created`, país creado |
| `PUT` | `/IsoCountries` | `204 No Content`, país actualizado |
| `GET` | `/Customers` | `200 OK`, lista de clientes |
| `GET` | `/Customers/{id}` | `200 OK`, cliente encontrado |
| `POST` | `/Customers` | `201 Created`, cliente creado |
| `PUT` | `/Customers` | `204 No Content`, cliente actualizado |

Las operaciones de autenticación, Countries y Customers usan procedimientos almacenados por medio de `IMySqlDataService`. Las pruebas unitarias de los controladores están en `CarRental.Tests/Countries` y `CarRental.Tests/Customers`.

Ejecuta las pruebas con `dotnet test CarRental.sln`.
