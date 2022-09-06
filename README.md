## Requirements

[.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)

## Recommended IDE

- Visual Studio 2022
  - Should include .NET 6 SDK
  - Only Web packages are required

## Getting started

1. Install Visual Studio 2022 & select Web development (top left)
2. Clone repository
3. Open `shopping-bag.sln` with Visual Studio
4. Select `shopping-bag` from the top bar dropdown with green Play-button
5. Click the button to start backend
6. Swagger page should open in browser

## Structure

Currently the backend is structured as follows from top to bottom:

- Controller:
  - Receives requests from frontend
  - Returns responses `ActionResult<ResponseType>`
  - Handles authentication
  - Passes & receives information from services
    - As such, contains some logic on what services to call
- Service:
  - Combines information from multiple stores
  - Contains logic on how to process data
- Store:
  - Interfaces directly with the database
  - One store per one database table
  - No functionality outside of database calls
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
