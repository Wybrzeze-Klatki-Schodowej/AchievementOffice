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
2.1 Build and run the environment for development
```bash
docker compose -f docker-compose.dev.yml up --build
```
2.2 Build and run the environment for production
```bash
docker compose -f docker-compose.prod.yml up --build -d
```
3. Open browser with url address: http://localhost:5173 for development container or http://localhost for production