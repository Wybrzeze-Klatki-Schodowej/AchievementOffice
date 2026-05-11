# AchievementOffice

An office praise system, where employees can share their achievements, confirm others' achievements and give shout-outs.

## Tech Stack
+ C#
+ React + TypeScript
+ PostgreSQL
+ Docker

## Getting started
1. Clone the repo
```bash
git clone https://github.com/Wybrzeze-Klatki-Schodowej/AchievementOffice
```
2. Build and run the environment for development
```bash
docker compose up -d --build
```

3. Open your browser
   + Frontend: http://localhost:5173
   + Backend: http://localhost:8080

## How to migrate database
Execute in order those commands.
```bash
docker compose exec backend /bin/bash
cd AchievementOffice
dotnet ef migrations add <name_of_migration>
dotnet ef database update
```

## Definition of Done

1. Meets the requirements defined in Jira.
2. Works correctly and does not break existing functionality.
3. All tests pass.
4. Passes CI/CD checks.
5. Approved in code review by at least two reviewers.
6. Fully integrated with the system.
7. Code is clean and follows project conventions.
8. Designed to be extendable.
9. Ready for demonstration (demo-ready).
