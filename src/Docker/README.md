# Database

This folder contains Docker file for the MS SQL Server database.
The Dockerfile for the API itself is in the main directory.

## Getting started

1. Change the database password in the `docker-compose.yml`
2. Run `docker-compose up` in the folder
3. Add the connection string for the Docker container to the API `appsettings.json`.
4. The format is `"Server=host.docker.internal,1433;Database=master;User Id=sa;Password=DatabasePassword;"`
5. The format also depends on where you are running the container. This is for local environment.
