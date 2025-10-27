## Weather Dashboard

A **full-stack Weather Dashboard** built using **React (Vite)** on the front-end and **.NET 8 Web API (C#)** on the back-end.  
It demonstrates clean architecture, async API integration, retry and caching patterns, and modern UI design — with complete unit & integration tests.

---

## Features

### Frontend (React)
- Built with **Vite + React 18** for fast development
- Uses **Context API + Custom Hook (useWeatherDashboard)** for state management
- Provides **hourly and 7-day forecasts**
- Fully **responsive** with **internal CSS (no Tailwind/PostCSS)**
- Displays clear **success/error messages** with smooth fade animations
- Allows setting and persisting a **default city**

### Backend (C# / .NET 8)
- REST API fetching real weather data from [WeatherAPI.com](https://www.weatherapi.com/)
- Clean separation: **Models**, **DTOs**, **Services**, **Controllers**
- **Polly Retry + Timeout Policies** for resiliency
- **IMemoryCache** for performance and rate-limit safety
- **AutoMapper** for Model ↔ DTO conversion
- **Logging & Error Handling** via `ILogger`
- **Swagger/OpenAPI** integrated
- **Unit Tests** (NUnit) for service, controller, and DTO deserialization

---

## Architecture Overview

```
Weather-Dashboard/
│
├── WeatherDashboardAPI/             # .NET 8 Web API
│   ├── Controllers/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   ├── Interfaces/
│   └── Program.cs
│
├── weatherdashboard/                # React Frontend (Vite)
│   ├── src/
│   │   ├── api/WeatherService.js
│   │   ├── components/
│   │   ├── context/WeatherContext.jsx
│   │   ├── hooks/useWeatherDashboard.js
│   │   ├── pages/Dashboard.jsx
│   │   └── styles/Dashboard.css
│   └── vite.config.js
│
├── weatherdashboardapi.tests/       # NUnit test project
│   ├── WeatherServiceTests.cs
│   ├── WeatherDashboardControllerTests.cs
│   └── WeatherDashboardAPI.Tests.csproj
│
├── .gitignore
└── README.md
```

---

## Backend Setup (.NET API)

### Prerequisites
- .NET 8 SDK  
- A free API key from [WeatherAPI.com](https://www.weatherapi.com/)  

## Configuration and Secrets Setup

The Weather Dashboard integrates with external weather APIs (such as **WeatherAPI** or **OpenWeather**) and requires an API key for fetching data.  
For security reasons, **no real API keys or credentials are committed to this repository**.

### 🔐 API Key Setup

You must configure your own API key locally before running the backend.

#### Option 1 — .NET User Secrets (Recommended)

1. Open a terminal in the **API project folder** (e.g., `WeatherDashboardAPI/`):
   ```bash
  1) Open a terminal:  dotnet user-secrets init
  2) Add your API key securely: dotnet user-secrets set "WeatherSettings:ApiKey" "<YOUR_API_KEY>"
  3) Verify it was stored: dotnet user-secrets list

### Run the API
```bash
cd WeatherDashboardAPI
dotnet restore
dotnet run
```
The Swagger URL will automatically open using the below URL - 
Swagger UI → [http://localhost:5172/swagger/](http://localhost:5172/swagger/)

---

## Frontend Setup (React)

### Prerequisites
- Node.js 18+  
- NPM or Yarn

### Setup & Run
```bash
cd weatherdashboard
npm install
npm run dev
```

The app runs at [http://localhost:5173](http://localhost:5173)

If the API runs on a different port, set this in `.env`:
```bash
VITE_API_BASE_URL=http://localhost:5172

> Note: For local testing, the API runs on **http://localhost:5172** (HTTPS disabled for simplicity).  
> In a production environment, `UseHttpsRedirection()` can be re-enabled and certificates trusted.
```

---

## Testing

### Backend Tests (NUnit)
Run all tests:
```bash
cd WeatherDashboardAPI.tests
dotnet test
```

Includes:
- `WeatherServiceTests` — Retry, Caching, and Timeout logic  
- `ControllerTests` — Validations, Status codes  
- `DTOTests` — Serialization/deserialization checks

### Frontend Tests (Vitest + React Testing Library)
Run:
```bash
cd weatherdashboard
npm test
```

Includes:
- `Dashboard.test.jsx` — Integration: search, default city, error handling  
- `SearchBar.test.jsx` — Input + button validation  
- `useWeatherDashboard.test.js` — Hook logic for success/error states  

---

## Core Design Highlights

| Layer | Purpose | Tools Used |
|-------|----------|------------|
| **DTO Layer** | Clean contracts for API responses | `AutoMapper` |
| **Service Layer** | Fetch, cache, retry & parse weather data | `HttpClientFactory`, `Polly`, `IMemoryCache` |
| **Controller Layer** | Validate, log, and expose API endpoints | ASP.NET Core MVC |
| **React Context + Hook** | Global state & async logic management | `useContext`, `useEffect`, `useState` |
| **Testing Layer** | Confidence in logic & UX | `NUnit`, `Vitest`, `@testing-library/react` |

---

## Resiliency Setup (Polly Example)

```csharp
builder.Services.AddHttpClient<IWeatherService, WeatherService>()
    .AddPolicyHandler(HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));
```

This ensures the API:
- Retries transient failures twice (exponential backoff)
- Cancels requests taking > 5 seconds
- Logs retries and timeout events

---

## Key Challenges & Solutions

| Challenge | Solution |
|------------|-----------|
| **Long API latency (~14s)** | Removed duplicate retry logic; added timeout; handled gracefully |
| **Validation not showing** | Introduced centralized state hook with temporary messages |
| **Mobile UI issues** | Refactored internal CSS grid + responsive typography |
| **DTO / mapping errors** | Added AutoMapper and nullable-safe DTOs |
| **Intermittent test failures** | Used `waitFor()` and `findByText()` for async assertions |

---

## Future Enhancements

- 🌐 Add Docker + Compose for single-command setup  
- 🔐 Move API key to Azure Key Vault or `dotnet user-secrets`  
- 📊 Add weather trend charts using Recharts  
- 📈 Add SonarQube and CI/CD (Azure DevOps or GitHub Actions)  
- ☁️ Deploy on Azure App Service + Static Web App combo  

---

## License

MIT License © 2025 — Developed by **Srinivasa Chaitanya Ponnada**  
For learning and demonstration purposes only.

---

## Preview

![Weather Dashboard Preview](/weatherdashboard/public/preview.png)

---

## Summary

This project demonstrates:
- Full-stack proficiency (React + .NET)
- Resilient backend design with retry, caching, timeout
- Clean separation with DTOs, Services, and Controller layers
- Frontend state abstraction using custom hooks
- Complete automated testing on both sides

=======
# Weather-Dashboard
