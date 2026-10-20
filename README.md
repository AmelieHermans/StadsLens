# StadsLens

Mobiele stadsgids: kies een wandelroute, scan gebouwen met de camera (Custom Vision),
luister naar de geschiedenis (Azure AI Speech) in het Nederlands of Engels (Azure AI Translator).

## Mappenstructuur

```
StadsLens/
├── backend/
│   ├── StadsLens.Backend.slnx
│   └── src/
│       ├── StadsLens.Api/              → Presentation: HTTP-endpoints, DI-compositie
│       │   ├── Controllers/            (Routes, Landmarks, Users, Visits)
│       │   ├── Properties/
│       │   ├── Program.cs
│       │   └── appsettings.json        (connection string)
│       ├── StadsLens.Application/      → Use cases / businesslogica
│       │   ├── Interfaces/
│       │   │   ├── Repositories/       (IRouteRepository, ... = ports)
│       │   │   └── Services/           (IRouteService, ...)
│       │   ├── Services/               (RouteService, ...)
│       │   ├── Mappings/               (AutoMapper MappingProfile)
│       │   └── DependencyInjection.cs  (AddApplication)
│       ├── StadsLens.Contracts/        → Request/response-DTO's voor de app
│       │   ├── Routes/  Landmarks/  Users/  Visits/
│       ├── StadsLens.Domain/           → Entiteiten, geen afhankelijkheden
│       │   └── Entities/               (Route, Landmark, User, Visit)
│       └── StadsLens.Infrastructure/   → EF Core + SQL Server
│           ├── Data/
│           │   ├── StadsLensDbContext.cs
│           │   └── Configurations/     (Fluent API per entiteit)
│           ├── Repositories/           (RouteRepository, ...)
│           ├── Migrations/             (na eerste `dotnet ef migrations add`)
│           └── DependencyInjection.cs  (AddInfrastructure)
└── frontend/
    ├── StadsLens.Frontend.slnx
    └── src/
        └── StadsLens.Mobile/           → .NET MAUI + MVVM Community Toolkit
            ├── Constants/              (ApiSettings = Ngrok-URL, PageRoutes)
            ├── Messages/               (WeakReferenceMessenger-messages)
            ├── Models/
            ├── Services/               (NavigationService, StadsLensApiService)
            ├── ViewModels/             (BaseViewModel + 1 per pagina)
            ├── Views/                  (8 pagina's, lege code-behind)
            ├── Resources/  Platforms/
            ├── AppShell.xaml
            └── MauiProgram.cs          (DI-registratie + routes)
```

### Afhankelijkheden (wijzen naar binnen)

```
Api ──► Application ──► Domain
 │          │
 │          └──► Contracts
 └──► Infrastructure ──► Application, Domain
```

Request-flow: `Controller → Service → Repository → DbContext`.

Relaties: `Route 1→n Landmark`, `User 1→n Visit`, `Landmark 1→n Visit`.

## Starten

```bash
# database aanmaken
cd backend
dotnet ef migrations add InitialCreate -p src/StadsLens.Infrastructure -s src/StadsLens.Api
dotnet ef database update -p src/StadsLens.Infrastructure -s src/StadsLens.Api

# API draaien + tunnel
dotnet run --project src/StadsLens.Api --launch-profile http
ngrok http 5158
```

In Development vult de API een lege database automatisch met een voorbeeldroute (`DbSeeder`).

De app kiest de API-URL in `frontend/src/StadsLens.Mobile/Constants/ApiSettings.cs`:

- `NgrokUrl` leeg → lokale API: `http://localhost:5158` (Windows) of `http://10.0.2.2:5158` (Android-emulator).
- `NgrokUrl` ingevuld → die tunnel (nodig voor een echt toestel).
