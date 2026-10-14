# Unit Conversion API

An ASP.NET Core Web API that converts numeric values between units of measurement
(e.g. meters to feet, Celsius to Fahrenheit, kilograms to pounds).

## Features

- Categories: **length, temperature, mass, volume, speed** (the first three are the required minimum)
- Units accepted by full name or symbol, case-insensitively (`meter`, `m`, `METER`, `meters`)
- Discovery endpoint listing every supported unit
- Consistent RFC 7807 `ProblemDetails` error responses
- Interactive API docs (OpenAPI + Scalar) in Development
- Health check endpoint, unit + integration tests, CI workflow

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (pinned in `global.json`)

## Run locally

```bash
dotnet restore
dotnet run --project src/UnitConversion.Api
```

The API starts on **http://localhost:5080** and opens the interactive docs at
`/scalar/v1`. The raw OpenAPI document is at `/openapi/v1.json`.

## Run the tests

```bash
dotnet test
```

## API

### Convert a value

`GET /api/v1/conversions?category={category}&from={unit}&to={unit}&value={number}`

```bash
curl "http://localhost:5080/api/v1/conversions?category=length&from=meter&to=foot&value=10"
```

```json
{
  "category": "Length",
  "from": "meter",
  "to": "foot",
  "inputValue": 10,
  "result": 32.8083989501
}
```

| Example                                                          | Result  |
|------------------------------------------------------------------|---------|
| `category=temperature&from=celsius&to=fahrenheit&value=100`      | `212`   |
| `category=temperature&from=kelvin&to=celsius&value=0`            | `-273.15` |
| `category=mass&from=kg&to=lb&value=1`                            | `2.2046226218` |
| `category=speed&from=mph&to=km/h&value=60`                       | `96.56064` |

### List supported units

`GET /api/v1/units` or `GET /api/v1/units?category=mass`

### Health

`GET /health`

### Errors

Invalid input returns `400 Bad Request` with a `ProblemDetails` body:

| Cause                                         | Example detail                                         |
|-----------------------------------------------|--------------------------------------------------------|
| Unit unknown or not in the given category     | `Unit 'kg' is not supported in category 'Length'.`     |
| Missing / non-numeric `value`, bad `category` | Validation errors keyed by parameter                   |
| Value too large to convert                    | `The value is too large to convert.`                   |

## Project structure

```
src/
  UnitConversion.Api/             HTTP layer: controllers, DTOs, error handling, startup
  UnitConversion.Core/            Domain: models, interfaces, conversion logic (no framework deps)
  UnitConversion.Infrastructure/  Unit data source (in-memory) and DI registration
tests/
  UnitConversion.Tests/           xUnit unit tests and WebApplicationFactory integration tests
```

Dependencies point inward: `Api -> Core`, `Infrastructure -> Core`. The conversion logic
knows nothing about HTTP or where unit data comes from.

## Design decisions and trade-offs

**Data-driven conversion model.** Every unit is stored as a `FactorToBase` and an
`OffsetToBase` relative to a base unit per category. Converting is
`base = value * factor + offset`, then the reverse for the target unit. Adding a unit is
one new data row, not new code, and N units need N definitions instead of N x N
conversion rules. This is what makes "hundreds of units" realistic.

**Temperature fits the same model.** Celsius, Fahrenheit and Kelvin differ by offsets as
well as scale, so the offset term exists for them. Everything else keeps an offset of 0.

**Repository abstraction.** `IUnitRepository` hides where units come from. Data is hardcoded
in `InMemoryUnitRepository` today, as allowed; moving to a database or config file later
means adding another implementation and changing one DI line, with no change to the
conversion service or controllers.

**`decimal` instead of `double`.** Unit factors such as `0.3048` are exact in decimal, and
results don't pick up binary floating-point noise. The trade-off is a smaller range
(about 7.9e28) and slower arithmetic, both irrelevant here. Values beyond the range
return a 400 rather than a 500.

**Result rounding.** Results are rounded to 15 significant digits and trailing zeros are
removed. Significant digits (not fixed decimal places) keep very small results, such as
millimeters to miles, accurate. The trade-off is that callers get a rounded value, not the
raw arithmetic result.

**Unit matching.** Name, symbol, and a simple `+s` plural are matched case-insensitively.
Irregular plurals (`feet`, `inches`) are not matched; use the singular name or symbol.
Symbols are only unique within a category (e.g. `m` is meters; milli- and mega- prefixes
would need care if added).

**Category is required.** Symbols can be ambiguous across categories, so callers state the
category explicitly and a unit from another category is rejected. This keeps lookups
simple and errors clear.

**URL versioning without a package.** Routes are prefixed with `/api/v1`. It is the
simplest approach that still leaves room for a v2; a versioning library would be worth
adding once there is a second version.

**`GET` for conversions.** The operation is a safe, idempotent read with a handful of
scalar inputs, so a query-string `GET` is cacheable and easy to try in a browser.

**Singleton services.** The repository is immutable, so it and the conversion service are
registered as singletons.

## Possible next steps

- Persist units in a database with an admin API to add or edit units
- Cache the unit catalogue if it moves to a database
- Batch conversions (`POST` with a list of requests)
- Rate limiting, structured logging sinks, and request tracing for production
