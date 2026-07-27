# Industrial OPC UA Data Logger

A Windows desktop application (VB.NET, .NET 10) that connects to an OPC UA server, monitors live industrial process data in real time, persists it to a SQL Server database, and visualizes historical trends per tag. Built against the OPC Foundation's own reference stack rather than a simplified third-party wrapper library.

## Features

- **OPC UA client built from the OPC Foundation's reference stack** — session and certificate management, security configuration, and identity handling implemented directly against the official library, not abstracted away by a simplified wrapper.
- **Dynamic node browsing** — the application discovers and resolves all monitored tags at runtime by browsing the server's node tree. No NodeIds are hardcoded, so it adapts to whatever tags the connected server actually exposes.
- **Real-time subscription-based monitoring** — uses OPC UA's subscription/`MonitoredItem` model to receive live push notifications when a value changes, rather than polling the server on a timer.
- **Thread-safe UI updates** — subscription notifications arrive on a background thread from the OPC UA client library. The application correctly marshals these onto the UI thread (`InvokeRequired`/`Invoke`) before touching any controls, avoiding cross-thread exceptions and race conditions.
- **Client-handle-to-tag mapping** — OPC UA notifications only carry a numeric `ClientHandle`, not a tag name. The application maintains its own mapping, built at subscription time, to resolve incoming notifications back to the correct tag and UI element.
- **Persistent SQL Server logging** — live data is written to a normalized two-table schema, isolated in its own `DataService` class:
  - `logs` — an append-only historical record of every value change, with `name`, `value`, and `timeAccessed` columns.
  - `currentValues` — one row per tag, holding the latest known value, updated in place on every change.
  - `logs.name` is a foreign key referencing `currentValues.name`, enforcing that every logged change corresponds to a known tag.
- **Per-tag history visualization** — checking a tag in the UI opens a dedicated window showing its recent history as a live, auto-refreshing chart (via ScottPlot), built from a time-windowed SQL query rather than the full unbounded history.
- **Clean separation of concerns** — the application is split across three focused classes: `OPCClient` (OPC UA protocol logic only), `DataService` (all SQL Server communication), and the UI layer (`Form1`, `HistoryForm`), with no class doing more than one job.

## Tech Stack

- **Language/Runtime:** VB.NET, .NET 10
- **UI:** Windows Forms
- **Industrial Protocol:** OPC UA (OPC Foundation UA-.NETStandard reference stack)
- **Database:** SQL Server Express, accessed via ADO.NET (`SqlConnection`/`SqlCommand`)
- **Charting:** ScottPlot (WinForms)
- **Test Server:** Prosys OPC UA Simulation Server

## How It Works

1. On startup, the application configures itself as an OPC UA client (application identity, certificate stores, security settings) and establishes a session with the configured OPC UA server.
2. It browses the server's node tree to locate the simulation tags and resolves each one's NodeId dynamically — nothing is hardcoded.
3. A subscription is created, and one `MonitoredItem` is registered per tag. Each item's server-assigned `ClientHandle` is recorded against its tag name in a local dictionary.
4. As tag values change on the server, the server pushes notifications to the client. Each notification is marshaled onto the UI thread, matched back to its tag via the handle dictionary, and used to update the corresponding label.
5. Each change is also logged via `DataService`: a new row is appended to `logs`, and the corresponding row in `currentValues` is updated with the latest value and timestamp.
6. Checking a tag in the UI opens a `HistoryForm` for that tag, which queries `logs` for a recent time window, renders the result as a chart, and refreshes it automatically every second via a `Timer`.

### A real debugging note

Early history-graph testing showed a single tag returning hundreds of rows within a supposedly one-minute time window — clearly wrong, since live logging showed roughly one update per second. Tracing it back, the OPC UA `SourceTimestamp` field being logged didn't reflect real-time processing — it stayed fixed to an earlier date rather than updating with each new value from the simulator. The fix was logging the application's own current time at the moment each notification was received, rather than trusting the server-provided source timestamp.

## Getting Started

### Prerequisites

- Windows with .NET 10 SDK installed
- Visual Studio (or another IDE with VB.NET/.NET support)
- [Prosys OPC UA Simulation Server](https://www.prosysopc.com/products/opc-ua-simulation-server/) (free) — used as the test OPC UA server
- SQL Server Express (free) — [download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- SQL Server Management Studio (SSMS) — recommended for setting up the database and inspecting logged data

### Setup

1. **Install and run Prosys OPC UA Simulation Server.** By default it exposes an endpoint at:
   ```
   opc.tcp://localhost:53530/OPCUA/SimulationServer
   ```
   Confirm it's running before starting the application — it needs to be reachable for the app to connect and browse tags.

2. **Install SQL Server Express**, using Basic installation mode. Note the instance name (typically `SQLEXPRESS`) shown at the end of setup.

3. **Create the database and tables.** Open SSMS, connect to `localhost\SQLEXPRESS` using Windows Authentication (check "Trust Server Certificate" if prompted), and run:

   ```sql
   CREATE DATABASE OPCDataLogger;

   USE OPCDataLogger;

   CREATE TABLE currentValues (
       name VARCHAR(50) PRIMARY KEY,
       value FLOAT,
       timeAccessed DATETIME2
   );

   CREATE TABLE logs (
       name VARCHAR(50),
       value FLOAT,
       timeAccessed DATETIME2,
       FOREIGN KEY (name) REFERENCES currentValues(name)
   );
   ```

4. **Seed `currentValues` with the simulation tags** so the first update to each tag has a row to update:

   ```sql
   INSERT INTO currentValues ([name], [value], [timeAccessed]) VALUES
   ('Counter', 0, GETDATE()),
   ('Random', 0, GETDATE()),
   ('Sawtooth', 0, GETDATE()),
   ('Sinusoid', 0, GETDATE()),
   ('Square', 0, GETDATE()),
   ('Triangle', 0, GETDATE()),
   ('Constant', 0, GETDATE());
   ```

5. **Open the project in Visual Studio** and confirm the connection string in `DataService.vb` matches your SQL Server instance name and database.

6. **Build and run the application.** With Prosys running and the database seeded, the app will connect, browse the server's tags, subscribe to live updates, and begin logging. Check a tag's checkbox to open its live-updating history graph.

## Project Status

Core pipeline is complete and working: connection, dynamic browsing, real-time subscriptions, thread-safe UI updates, SQL Server logging, and per-tag history visualization.
