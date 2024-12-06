
# API Key Rotation Demo

This project demonstrates how to securely rotate API keys using HashiCorp Vault and a .NET 8.0 application. The application interacts with Vault to store and retrieve API keys, ensuring that sensitive credentials are stored securely and rotated automatically.

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Project Structure](#project-structure)
4. [Setting Up Vault](#setting-up-vault)
   1. [Manual Setup](#manual-setup)
   2. [Docker Setup](#docker-setup)
5. [Running the Application](#running-the-application)
6. [Testing](#testing)
7. [Setting Up Docker for Vault and the Application](#setting-up-docker-for-vault-and-the-application)
   1. [Run Vault in Docker](#run-vault-in-docker)
   2. [Dockerize the Application](#dockerize-the-application)
8. [Additional Resources](#additional-resources)

## Overview

This project showcases an example of how to handle API key rotation in a secure manner. It uses **HashiCorp Vault** to store API keys and provides a .NET 8.0 application that interacts with Vault to fetch and rotate API keys. By following this demo, you can see how easy it is to integrate Vault with a .NET application to rotate secrets in a secure, automated fashion.

### What is HashiCorp Vault?
HashiCorp Vault is a tool designed to securely store and access secrets, such as API keys, tokens, passwords, and certificates. Vault can dynamically generate secrets, manage them securely, and ensure they are rotated automatically. In this project, Vault is used to securely store and rotate API keys.

### What is API Key Rotation?
API key rotation is the practice of periodically changing API keys to reduce the risk of compromised credentials. By automatically rotating API keys, organizations ensure that old, potentially compromised keys are no longer usable and reduce the risk of data breaches.

## Features

- **API Key Rotation:** Secure handling and rotation of API keys.
- **HashiCorp Vault Integration:** Stores and retrieves API keys securely.
- **.NET 8.0 Application:** Built with .NET 8.0 for simplicity and modern development practices.
- **Testing:** Includes unit tests for verifying the application logic.

## Project Structure

```plaintext
api-key-rotation-demo/
├── README.md                   # Project documentation
├── api-key-rotation-demo.sln    # .NET solution file
├── src/
│   ├── ApiKeyRotation/          # Main .NET application
│   │   ├── Program.cs           # Application entry point
│   │   ├── ApiKeyRotation.csproj # .NET project file
│   │   ├── Services/            # Vault services and business logic
│   └── ...
└── tests/
    ├── ApiKeyRotation.Tests/    # Unit tests for API key rotation
    │   ├── VaultServiceTests.cs # Unit tests for Vault service
    │   ├── ApiKeyRotation.Tests.csproj
    └── ...
```

## Setting Up Vault

### Manual Setup

1. **Install HashiCorp Vault:**
   Follow the instructions from the [HashiCorp Vault official documentation](https://www.vaultproject.io/docs/install) to install Vault on your machine.

2. **Start Vault Server:**
   Start Vault in development mode (for local testing) using the following command:
   ```bash
   vault server -dev
   ```
   This will run Vault in dev mode and listen on `http://127.0.0.1:8200`.

3. **Authenticate with Vault:**
   Get the root token that was generated when you started Vault (by default, it is `root_token`).

   Set the Vault address and token environment variables:
   ```bash
   export VAULT_ADDR=http://127.0.0.1:8200
   export VAULT_TOKEN=root_token
   ```

4. **Enable the Secrets Engine:**
   Enable the KV (Key-Value) secrets engine for API key storage:
   ```bash
   vault secrets enable -path=secret kv
   ```

5. **Store a Sample API Key:**
   Store a sample API key to simulate the API key rotation:
   ```bash
   vault kv put secret/api-key id="example-key-id" value="initial-api-key"
   ```

### Docker Setup

If you prefer using Docker to set up Vault and the application, follow these steps:

1. **Create a `docker-compose.yml` file:**
   Create the file in your project root:

   ```yaml
   version: '3.7'

   services:
     vault:
       image: vault:1.13.0
       container_name: vault
       ports:
         - "8200:8200"
       environment:
         VAULT_DEV_ROOT_TOKEN_ID: root_token
         VAULT_DEV_LISTEN_ADDRESS: '0.0.0.0:8200'
       volumes:
         - vault-data:/vault/data
       cap_add:
         - IPC_LOCK
       restart: unless-stopped

   volumes:
     vault-data:
       driver: local
   ```

2. **Start Vault with Docker Compose:**
   Run the following command to start the Vault server:
   ```bash
   docker-compose up -d
   ```

3. **Enable the KV Secrets Engine:**
   After the container is up, run:
   ```bash
   docker exec -it vault vault secrets enable -path=secret kv
   ```

4. **Set Environment Variables:**
   Set the Vault token and address in your environment:
   ```bash
   export VAULT_TOKEN=root_token
   export VAULT_ADDR=http://127.0.0.1:8200
   ```

## Running the Application

The .NET application can interact with Vault using the configured Vault server.

1. **Build the .NET Application:**
   Navigate to the `src/ApiKeyRotation` directory and build the application:
   ```bash
   dotnet build
   ```

2. **Run the Application:**
   To run the application, use the following command:
   ```bash
   dotnet run --project src/ApiKeyRotation/ApiKeyRotation.csproj
   ```

   The application will attempt to fetch an API key from Vault and rotate it.

## Testing

The project includes unit tests for verifying the logic in the `VaultService` class.

1. **Run the Unit Tests:**
   To run the tests, use the following command from the root of the project:
   ```bash
   dotnet test
   ```

2. **Test Results:**
   After running the tests, you should see the test results indicating that the tests passed.

## Setting Up Docker for Vault and the Application

### Run Vault in Docker

To run Vault in Docker using `docker-compose.yml` as mentioned earlier, run:

```bash
docker-compose up -d
```

This will bring up a Vault server running on `http://127.0.0.1:8200` with the default root token of `root_token`.

### Dockerize the Application

#### Dockerfile for the Application

Here’s how to create a `Dockerfile` to containerize the .NET application:

1. **Create the Dockerfile in the `src/ApiKeyRotation` directory:**

   ```dockerfile
   # Use the official .NET SDK image for building the application
   FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

   WORKDIR /src

   # Copy the project file and restore dependencies
   COPY ["ApiKeyRotation/ApiKeyRotation.csproj", "ApiKeyRotation/"]
   RUN dotnet restore "ApiKeyRotation/ApiKeyRotation.csproj"

   # Copy the rest of the application and build it
   COPY . .
   WORKDIR "/src/ApiKeyRotation"
   RUN dotnet publish "ApiKeyRotation.csproj" -c Release -o /app

   # Use the official .NET runtime image for running the application
   FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
   WORKDIR /app

   COPY --from=build /app .

   ENTRYPOINT ["dotnet", "ApiKeyRotation.dll"]
   ```

2. **Build the Docker Image:**

   In the project root directory, run:

   ```bash
   docker build -t api-key-rotation-demo .
   ```

3. **Run the Application in Docker:**

   Run the following command to start the container:

   ```bash
   docker run -d --env-file .env --name api-key-rotation-demo api-key-rotation-demo
   ```

   This will run the application and connect it to the Vault instance.

## Additional Resources

- [Vault Documentation](https://www.vaultproject.io/docs)
- [Docker Documentation](https://docs.docker.com/)
- [VaultSharp .NET Library](https://github.com/rajanadar/VaultSharp)
- [xUnit Testing Framework](https://xunit.net/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)