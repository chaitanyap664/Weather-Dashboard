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

The Weather Dashboard integrates with external weather APIs (such as **WeatherAPI**) and requires an API key for fetching data.  
For security reasons, **no real API keys or credentials are committed to this repository**.

## 🔑 API Key Setup (WeatherAPI.com)

This project uses the **[WeatherAPI.com](https://www.weatherapi.com/)** service to fetch live weather and forecast data.  
You’ll need your own **free API key** to run the backend successfully.

### 🪄 How to Get Your API Key
1. Go to [https://www.weatherapi.com/](https://www.weatherapi.com/).
2. Click **“Sign Up”** (it’s free).
3. After signing in, open the **“My Account → API Keys”** section.
4. Copy your personal API key — it will look like: 'abcd1234567890efghijklmnop';

### How to Configure the API Key

#### Option 1 — Using `appsettings.json`
Edit your file at:

Replace the placeholder with your actual key:
```json
{
  "WeatherSettings": {
    "APIBaseURL": "https://api.weatherapi.com/v1/forecast.json",
    "WeatherApiKey": "<YOUR_API_KEY>",
    "CacheSeconds": 600
  }
}

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

- Add Docker + Compose for single-command setup  
- Move API key to Azure Key Vault or `dotnet user-secrets`  
- Add weather trend charts using Recharts  
- Add SonarQube and CI/CD (Azure DevOps or GitHub Actions)  
- Deploy on Azure App Service + Static Web App combo  

---
## Scaling Strategy

The **Weather Dashboard** application is designed with a clean, modular architecture — consisting of a **React front-end** and a **.NET 8 Web API**.  
While the current setup efficiently supports moderate user traffic, it can be **scaled horizontally and hardened for production** with minimal changes.

---

### Current Design Highlights

- **Frontend:** React + Vite + Tailwind CSS hosted on Azure Static Web Apps (CDN-backed for global reach).  
- **Backend:** ASP.NET Core Web API hosted on Azure App Service, leveraging:
  - `IMemoryCache` for 5-minute per-city caching.
  - Polly retry and timeout policies for transient error handling.
  - Configurable base URL and API key stored in `appsettings.json` or environment variables.
- **Resilience:** Graceful handling of timeouts, invalid city names, and API failures.
- **Architecture:** Stateless API design ensures easy horizontal scaling.

---

### Future Scalability Enhancements

| Area | Enhancement | Benefit |
|------|--------------|----------|
| **Caching Layer** | Replace `IMemoryCache` with **Redis / Azure Cache for Redis** (`IDistributedCache`) | Enables shared caching across all API instances; improves consistency and performance under load |
| **Hosting** | Enable **Azure App Service Auto-Scaling** | Automatically adjusts instance count based on CPU or request load |
| **Resilience** | Add **Circuit Breaker** and **Rate Limiter** (via Polly / .NET middleware) | Prevents cascading failures and throttles high-frequency requests |
| **Observability** | Integrate **Azure Application Insights** | End-to-end monitoring of request latency, dependency health, and exception tracking |
| **Configuration & Secrets** | Use **Azure Key Vault** for secrets and API keys | Centralized, secure management of sensitive configuration |
| **Deployment** | Introduce **CI/CD pipelines** (GitHub Actions / Azure DevOps YAML) | Automated build, test, and environment deployments (Dev / UAT / Prod) |
| **Security** | Add **JWT / Entra ID (Azure AD)** authentication (if user personalization required) | Protects APIs and supports user-specific default city settings |
| **Cache Refresh Policy** | Implement **stale-while-revalidate** or **refresh-ahead background service** | Keeps frequently requested city data up-to-date with minimal latency |
| **External API Management** | Integrate **Azure API Management** | Centralized traffic control, caching, and usage analytics |

---

### Scalability Principles Followed

- **Stateless API:** Each request is independent, simplifying horizontal scaling.  
- **Caching & Retry Policies:** Reduce dependency load and improve response speed.  
- **Separation of Concerns:** Clear distinction between UI, API, and data layers.  
- **Environment Configurations:** All critical values (API keys, URLs, cache durations) are configurable via environment or settings files.  
- **Extensibility:** Easy to introduce new features or replace components (e.g., Redis, distributed tracing) without changing core logic.

---

### Summary

The current architecture already achieves an optimal balance between **simplicity**, **performance**, and **reliability** for demonstration and small-scale production use.  
As traffic grows, enhancements such as **distributed caching**, **auto-scaling**, and **advanced observability** can be added seamlessly to evolve the system into a robust, enterprise-grade platform.

---


## License

MIT License © 2025 — Developed by **Srinivasa Chaitanya Ponnada**  

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
