# 🎮Playtesters.API

**Playtesters.API** is a lightweight, secure RESTful service built with **.NET 8** and **Entity Framework Core (SQLite)**.

It’s designed for indie developers or small teams who need a simple way to manage **playtesters**, **access keys**, and **access validation history** for private or early-access game builds.

> This project was created to support the roguelike action game I'm building with my best friend from school.

## ✨Features

- Create and manage tester accounts with unique access keys (GUIDs).
- Track successful access validations with timestamp and IP address.
- Secure admin endpoints using an API key stored in `.env`.
- Public endpoint for game clients to validate access keys.
- Easy to integrate with Unity or any custom launcher/client.
- Organized structure using use cases, DTOs, validators, and minimal APIs.

## 🧰Tech Stack
- [.NET 8 (Web API)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Entity Framework Core (SQLite)](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite)
- [Swashbuckle (Swagger)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [SimpleResults](https://github.com/DevD4v3/SimpleResults)
- [DotEnv.Core](https://github.com/DevD4v3/dotenv.core)

## 🔐Authentication

All admin endpoints require the following header:
```http
X-Api-Key: <your-admin-key>
```
The admin key must be defined in your `.env` file:
```.env
API_KEY=your-admin-key
```

Only the endpoint `/api/testers/validate-access` is publicly accessible by game clients.

## 🎮Unity Integration

The public `/api/testers/validate-access` endpoint can be called directly from your Unity project to validate tester access before allowing gameplay or enabling private build features.

## 📘 HTTP Request Examples

You can use this [Playtesters.API.http](https://github.com/DevD4v3/Playtesters.API/blob/master/src/Playtesters.API.http) file (VS Code / Rider / Visual Studio compatible) to test every endpoint of the API.

## 📄License

MIT License — feel free to use, modify, and extend it for your own projects.
