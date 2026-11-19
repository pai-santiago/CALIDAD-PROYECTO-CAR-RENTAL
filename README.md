# Car Rental

Proyecto ASP.NET Core 8 de renta de autos con autenticación JWT conectada a la base MySQL `CarRental` existente.

## Seguridad

| Método | Ruta | Resultado correcto |
| --- | --- | --- |
| `POST` | `/api/auth/register` | `200 OK`, usuario creado como `User` |
| `POST` | `/api/auth/login` | `200 OK`, token JWT Bearer |
| `GET` | `/api/auth/me` | `200 OK` con token; `401` sin token |

Las contraseñas se guardan como hash PBKDF2. El registro no acepta roles del cliente. La API reutiliza la conexión y `IMySqlDataService` del proyecto actual; al iniciar crea `auth_roles` y `auth_users` si aún no existen.

## Ejecutar

Abre `CarRental.sln` en Visual Studio y ejecuta el perfil **CarRental Local**. La cadena local se lee de `ConnectionStrings:MysqlConnection`; no agregues contraseñas a `appsettings.json` ni al repositorio. Si necesitas configurarla, usa User Secrets de Visual Studio. En desarrollo, la clave JWT se genera en memoria si no defines `Jwt:Key`.

Swagger: `http://localhost:5231/swagger`.

## Evidencia en Postman

Importa `postman/CarRentalSecurity.postman_collection.json` y envía, en orden, **Registro - usuario 1**, **Registro - usuario 2**, **Login - usuario 1** y **Perfil protegido - validar JWT**. Las dos altas y el login esperan `200 OK`; el JWT del login se guarda en la variable de colección `accessToken`.

La colección usa cuentas de demostración. No reutilices su contraseña fuera de esta práctica.
