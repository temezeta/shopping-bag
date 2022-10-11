## Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
- [MS SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Recommended: [SQL Server Management Studio]()

## Recommended IDE

- Visual Studio 2022
  - Should include .NET 6 SDK
  - Only Web packages are required

## Getting started

1. Install database tools & [create a new database](https://docs.microsoft.com/en-us/sql/relational-databases/databases/create-a-database?view=sql-server-ver16)
2. Install Visual Studio 2022 & select Web development (top left)
3. Clone repository
4. Open `shopping-bag.sln` with Visual Studio
5. In user secrets or appsettings file (don't commit), change the connection string to match the database you created.
6. Install dotnet ef tools in PowerShell etc. `dotnet tool install --global dotnet-ef`
7. Select `shopping-bag` from the top bar dropdown with green Play-button
8. Click the button to start backend
9. Swagger page should open in browser

## Getting started with Docker

1. Follow steps 1-6 from above
2. Install [Docker Desktop](https://docs.docker.com/desktop/windows/wsl/)
3. Ensure WSL 2 support is enabled and virtualization (SVM etc.) is enabled in BIOS
4. If you can start Docker Desktop and `wsl -l -v` command returns version 2 distro you should be fine
5. You need to have a SSL cert at path `%USERPROFILE%\.aspnet\https\aspnetapp.pfx`. Use [this](https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-6.0) guide to setup dev-cert at that path. You may need to delete your previous one. `dotnet dev-certs https --clean`
6. See the `docker-compose.yml` for the password to set up for the dev-cert.
7. Modify `appsettings.Docker.json` with any missing details. If you run via command line, user secrets cannot be found.
8. Build `shopping-bag.csproj` project by right clicking it in Visual Studio Solution Explorer and Build (or run `docker-compose up` in the src directory)
9. Select `docker-compose` from the top bar dropdown with green Play-button
10. Click the button to start backend
11. You can test that the API is working by calling for example `Get All Offices`-endpoint

## Structure

Currently the backend is structured as follows from top to bottom:

- Controller:
  - Receives requests from frontend
  - Returns responses `ActionResult<ResponseType>`
  - Handles authentication
  - Passes & receives information from services
    - As such, contains some logic on what services to call
- Service:
  - Contains logic on how to process data
  - Modifies database context
- Models
  - Internal classes which represents the objects fetched from database
  - <b>Never returned from a controller</b>
- DTOs (Data transfer object)
  - Classes representing the return types of controllers
  - Mapped from models using auto mapper or manually
- Config
  - Random stuff related to project startup & configuration
- Misc files:
  - appsettings.json
    - Contains application default configuration variables
      - Don't change, use appsettings.Development.json or user secrets instead
      - To set user secrets right-click `shopping-bag` in Solutions Explorer and click `Manage User Secrets`

## Troubleshooting

- If whichever `shopping-bag` or `docker-compose` is not available in the top bar dropdown with Green Play-button
  - In Solution Explorer right click that project and `Set as Startup Project`
