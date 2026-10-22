# Simple Trading

## 📘 Project Overview

Simple Trading is a web API for keeping track of trades and their results. It is used by the
[Simple Trading Client App](https://github.com/stefan8893/simple-trading-client-app-v3).

The API supports managing trades, profiles, assets, currencies, references, and user settings. Trades can be searched,
filtered, sorted, and paged. Trade results can be calculated and analyzed based on position prices.

## ⚙️ Technical Implementation

### Architecture

The application separates its business rules from the mechanisms used to expose and persist them. The domain defines
the trading concepts and operations, while the API and data access provide adapters for external requests and stored
data.

Dependencies point toward the domain through abstractions, keeping business rules independent of the web framework
and database. This separation makes the main application concerns easier to understand, test, and evolve
independently.

### Dynamic Trade Search

Trade searches support dynamic property filters. Each filter specifies a trade property, a comparison operator, and a
comparison value, e.g. `Size -gt [5000]`. The API validates the filter syntax and supported property/operator combinations, then builds
database predicates for the matching trades.

Multiple filters are currently combined using `and` semantics only: every filter must match for a trade to be
included in the results. `or` expressions and grouped conditions with parentheses are not supported yet. A future
extension could support nested conditions such as:

```text
Size -gt [5000] and (Result -eq [Win] or ProfitLoss -gt [40])
```

### Interactor Source Generator

`SimpleTrading.Domain.Generators` is an incremental Roslyn source generator used by the Domain project at compile
time. It adds the generic `IInteractor` interfaces to the compilation and discovers concrete classes that implement
one of those interfaces. Interactors follow the `*Interactor` naming convention; the generator reports diagnostic
`ST0001` as a build error if an implementation does not follow it.

For each interactor, the generator creates a matching API-facing interface and a proxy class. For example,
`AddTradeInteractor` gets an `IAddTrade` interface and an `AddTradeInteractorProxy`. The generated interface exposes
an `Execute` method and includes an optional `CancellationToken`.

When FluentValidation validators exist for an interactor's request model, the proxy receives them through dependency
injection and runs them before calling the interactor. An invalid request returns a `ValidationResult` without
executing the interactor. The generated response type includes that validation result; if the interactor already
returns a `OneOf` that contains `ValidationResult`, the generator reuses it instead of adding a duplicate case.
Responses that use `OneOf` are mapped to the corresponding generated response type. The proxy also logs its
execution and passes the cancellation token to the underlying interactor.

Validation, logging, cancellation, and response adaptation are cross-cutting concerns. The generated proxies apply
an Aspect-Oriented Programming (AOP) approach to these concerns.

### Web API CLI

The Web API also provides command-line commands through `System.CommandLine`. Run a command with `dotnet run
--project src/SimpleTrading.WebApi -- <COMMAND>` from the repository root. The database commands use the configured
`DbProvider` and matching connection string.

| Command | Description |
| --- | --- |
| `create-db` | Applies the migrations and creates the database schema. Use `--drop-existing` to drop an existing database first; SQL Server deletion is blocked unless `--force` is also specified. |
| `seed-data` | Populates the database with master data. Run this after `create-db` when the initial data is needed. |
| `drop-db` | Deletes the configured database. SQL Server deletion is blocked unless `--force` is specified. |
| `generate-client` | Generates a C# or TypeScript client from the API's OpenAPI description. |

For example, create and populate a local database with:

```bash
dotnet run --project src/SimpleTrading.WebApi -- create-db
dotnet run --project src/SimpleTrading.WebApi -- seed-data
```

To generate a client, provide the target language and file name. `--output-dir` can be specified multiple times; if
omitted, the current directory is used:

```bash
dotnet run --project src/SimpleTrading.WebApi -- generate-client --target TypeScript --file-name SimpleTradingApiClient.ts --output-dir ./generated
```

Both `create-db --drop-existing --force` and `drop-db --force` can delete a SQL Server database. Use these options
with care, especially when the configured connection points to production.

### Deployment and Releases

GitHub Actions handles continuous integration and deployment. On every branch push, the CI workflow builds the
solution, runs the test projects, and publishes a code coverage report. A successful CI run
on `main` triggers the deployment workflow.

The release version follows **Semantic Versioning** and is determined from **Conventional Commit** messages since
the last version tag. The workflow uses `thenativeweb/get-next-version` for this step; it is not the
`semantic-release` package, but applies the same idea of deriving releases from commit history:

| Commit type | Version change | Example |
| --- | --- | --- |
| `fix:` | Patch | `fix: Correct trade result calculation` |
| `feat:` | Minor | `feat: Add trade export` |
| `feat!:` or a breaking-change commit | Major | `feat!: Replace the trade search API` |
| `chore:` | No release by itself | `chore: Update dependencies` |

For example, `fix: Correct trade result calculation` increases `1.2.3` to `1.2.4`, while `feat: Add trade export`
increases it to `1.3.0`. When a new version is available, the workflow creates and pushes a version tag. If the
commits do not require a new version, packaging and deployment are skipped.

For each release, the workflow builds and publishes a versioned Docker image to GitHub Container Registry. It then
updates the production SQL Server database with its migrations and deploys the image to the Azure Web App. The
TypeScript API client is published as a package to GitHub Packages. The workflow also removes older container and
client package versions, keeping the ten most recent of each.

### Testing

The solution contains tests for the domain use cases, data access, web API, and source generator. Tests use an
in-memory **SQLite** database so that relational behavior is available without requiring a separate database server.

Run all test projects with coverage from the repository root:

```bash
./test.sh
```
or
```pwsh
./test.ps1
```

### Database Setup

The application supports three database systems:

1. **SQL Server** (used in production)
2. **PostgreSQL** (used for local development)
3. **SQLite** (used for testing)

#### Production (SQL Server)

In production, the app connects to an **Azure SQL Database** via **SQL Server drivers**. From the application's
perspective, it is simply using SQL Server.

#### Local Development (PostgreSQL)

On the development machine, **PostgreSQL** is used because it is already installed. No additional database system is
required locally.

#### Testing (SQLite)

For testing purposes, the application uses an **in-memory SQLite database**. Although EF Core offers a separate
in-memory provider (`Microsoft.EntityFrameworkCore.InMemory`), it lacks support for essential SQL features like
referential integrity. SQLite provides a better balance between performance and realistic behavior in tests.

The database provider is selected through the `DbProvider` configuration value. The connection string with the
matching provider name is used for the database connection.

### Adding Migrations

To add a migration, run the following command from the `SimpleTrading.WebApi` project directory for each database
provider:

```bash
dotnet ef migrations add '<MIGRATION_NAME>' --startup-project . --project ../SimpleTrading.DataAccess.<DB_PROVIDER> -- --dbprovider <DB_PROVIDER>
```

Replace `<DB_PROVIDER>` with the desired provider: **SqlServer**, **Postgres**, or **Sqlite**.
