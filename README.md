# Workstation Lock/Unlock Logger

Logs Windows workstation lock and unlock events to timestamped text files.

Triggered by Task Scheduler on Event IDs **4800** (locked) and **4801** (unlocked).

## Usage

```
WorkstationLogger.exe lock      # workstation locked   → appends to logoff.txt
WorkstationLogger.exe unlock    # workstation unlocked → appends to logon.txt
```

Log files are written next to the exe. A `---` separator is inserted when the day changes.

## Log file format

```
Monday 04-05-2026 09:12:44
Monday 04-05-2026 17:31:02
---
Tuesday 05-05-2026 08:55:19
```

## Build

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8).

```
dotnet publish WorkstationLogger/ -c Release -r win-x64 --self-contained true -o publish/
```

The output is a single self-contained `publish\WorkstationLogger.exe` — no .NET runtime required on the target machine.

## Task Scheduler setup

Create two triggers, one for each event:

| Event ID | Trigger | Action |
|----------|---------|--------|
| 4800 | Workstation locked | `WorkstationLogger.exe lock` |
| 4801 | Workstation unlocked | `WorkstationLogger.exe unlock` |

**Important:** Set the task's "Start in" directory to the folder containing `WorkstationLogger.exe` so the log files are written next to it.

Steps:
1. Open **Task Scheduler** → Create Task
2. Triggers tab → New → **On an event** → Log: `Security`, Source: `Microsoft-Windows-Security-Auditing`, Event ID: `4800`
3. Actions tab → New → Program: path to `WorkstationLogger.exe`, Arguments: `lock`, Start in: folder containing the exe
4. Repeat for Event ID `4801` with argument `unlock`

## Tests

```
dotnet test WorkstationLogger.Tests/
```
