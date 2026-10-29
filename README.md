# Simple Trading

## 📘 Project Overview

Simple Trading is a web API for tracking trades and their results. It works with the
[Simple Trading Client App](https://github.com/stefan8893/simple-trading-client-app-v3).

The API lets users manage trades, profiles, assets, currencies, references, and user settings. Trades can be searched,
filtered, sorted, and paged. Trade results can also be calculated and analyzed from position prices.

## ⚙️ Technical Implementation

### Architecture

The domain defines the trading concepts and operations, while the API and data-access layers act as adapters that
expose this functionality to external requests and persist its data. This separation makes the application's core
logic easier to reason about, test, and evolve independently.

### Dynamic Trade Search

Trade searches support filters on trade properties. A filter contains a **property**, a comparison **operator**, and a **value**.
For example, `Size -gt [5000]` finds trades with a size greater than 5000.

The API checks that each filter is valid and that the property supports the requested operator. It then turns the
filters into database predicates.

For now, multiple filters are combined with `and`, so every condition must match. `or` expressions and parentheses
are not supported yet. A future version could allow nested conditions like this:

```text
Size -gt [5000] and (Result -eq [Win] or ProfitLoss -gt [40])
```

### Interactor Source Generator

`SimpleTrading.Domain.Generators` is an incremental Roslyn source generator that runs at compile time. It adds the
generic `IInteractor` interfaces and finds concrete classes that implement them. Interactors must end with
`Interactor`; otherwise, the generator reports the build error `ST0001`.

For each interactor, the generator creates a matching interface and proxy. For example, `AddTradeInteractor` gets an
`IAddTrade` interface and an `AddTradeInteractorProxy`. The generated interface exposes an `Execute` method with an
optional `CancellationToken`.

Validation, logging, cancellation, and response adaptation are cross-cutting concerns. The generated proxies apply
an Aspect-Oriented Programming (AOP) approach to these concerns.

If an interactor has request-model validators, the proxy runs them before calling the interactor. When validation
fails, it returns a `ValidationResult` and does not execute the interactor. The generated response type includes the
validation result. If the interactor already returns a `OneOf` containing `ValidationResult`, the generator reuses
that type.

`OneOf` represents a result that can be one of several explicitly listed types. For example, an interactor might
return `OneOf<Completed, NotFound, Conflict>`. If the proxy also needs to report validation failures, its generated
interface exposes `OneOf<Completed, NotFound, Conflict, ValidationResult>`. The proxy converts each result from
the interactor into the matching case of this response type, so callers can handle success, missing resources,
conflicts, and validation errors through one return value.

The proxy also writes a log message when execution starts. It forwards the caller's `CancellationToken` to the
interactor, allowing the interactor and its downstream operations to respond when the caller cancels the request.

### Web API CLI

The Web API includes command-line tools built with `System.CommandLine`. Run them from the repository root:

```bash
dotnet run --project src/SimpleTrading.WebApi -- <COMMAND>
```

Database commands use the configured `DbProvider` and its matching connection string.

| Command | Description |
| --- | --- |
| `create-db` | Applies migrations and creates the database schema. Add `--drop-existing` to delete an existing database first. For SQL Server, you must also add `--force`. |
| `seed-data` | Adds master data to the database. Run this after `create-db` if the initial data is needed. |
| `drop-db` | Deletes the configured database. For SQL Server, you must add `--force`. |
| `generate-client` | Generates a C# or TypeScript client from the API's OpenAPI description. |

For example, create a database and add its master data:

```bash
dotnet run --project src/SimpleTrading.WebApi -- create-db
dotnet run --project src/SimpleTrading.WebApi -- seed-data
```
> [!IMPORTANT]  
> The `--force` option can delete a SQL Server database. Use it with care, especially if the configured connection
points to production.
    
To generate a client, specify its language and file name. You can pass `--output-dir` more than once. If you omit it,
the client is written to the current directory:

```bash
dotnet run --project src/SimpleTrading.WebApi -- generate-client --target TypeScript --file-name simple-trading-api-client.ts --output-dir ./generated
```

### Deployment and Releases

GitHub Actions builds and tests the solution on every branch push, then publishes a code coverage report. When CI
passes on `main`, the deployment workflow checks whether the commits since the last version tag require a new release.

Versions follow **Semantic Versioning**. The workflow uses `thenativeweb/get-next-version` to derive the next version
from **Conventional Commit** messages. It is not the `semantic-release` package, but uses the same general approach
of basing releases on commit history:

| Commit type | Version change | Example |
| --- | --- | --- |
| `fix:` | Patch | `fix: Correct trade result calculation` |
| `feat:` | Minor | `feat: Add trade export` |
| `feat!:` or a breaking-change commit | Major | `feat!: Replace the trade search API` |
| `chore:` | No release by itself | `chore: Update dependencies` |

For example, a `fix:` commit changes `1.2.3` to `1.2.4`; a `feat:` commit changes it to `1.3.0`. When a release is
needed, the workflow creates and pushes a version tag. If there are no changes that require a new version, it skips
packaging and deployment.

For each release, the workflow builds a versioned Docker image and publishes it to GitHub Container Registry. It then
applies migrations to the production SQL Server database and deploys the image to Azure Web Apps. The workflow also
publishes the TypeScript API client to GitHub Packages and keeps the ten most recent versions of each package.

### Testing

The solution includes tests for the domain use cases, data access, web API, and source generator. Tests use an
in-memory **SQLite** database, which provides relational behavior without a separate database server.

Run all test projects with coverage from the repository root:

```bash
./test.sh
```

Or, on a system with PowerShell:

```powershell
./test.ps1
```

Both scripts accept `Release` or `Debug` as the configuration. Use `--no-build` with `test.sh` or `-NoBuild` with
`test.ps1` to skip building when the selected configuration has already been built.

### Database Setup

The application supports three database systems:

1. **SQL Server** for production
2. **PostgreSQL** for local development
3. **SQLite** for testing

#### Production (SQL Server)

In production, the application connects to an **Azure SQL Database** using the SQL Server provider.

#### Local Development (PostgreSQL)

PostgreSQL is used for local development because it is already installed on the development machine. No additional
database system is required.

#### Testing (SQLite)

Tests use an **in-memory SQLite database**. EF Core's separate `Microsoft.EntityFrameworkCore.InMemory` provider does
not support essential SQL features such as referential integrity. SQLite offers a good balance of speed and
realistic database behavior.

The `DbProvider` configuration value selects the provider. The application then uses the connection string with the
matching provider name.

### Adding Migrations

To add a migration, run this command from the `SimpleTrading.WebApi` project directory. Run it once for each provider
that needs the migration:

```bash
dotnet ef migrations add '<MIGRATION_NAME>' --startup-project . --project ../SimpleTrading.DataAccess.<DB_PROVIDER> -- --dbprovider <DB_PROVIDER>
```

Replace `<DB_PROVIDER>` with **SqlServer**, **Postgres**, or **Sqlite**.
