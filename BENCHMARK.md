# Taxi Traffic large-city benchmark (developer-only)

This branch tests parallel resident eligibility and includes a passive speed logger.
**No Harmony patches. No changes to vanilla TaxiDispatch or RideNeeder timing.**
The logger is active whether TaxiTraffic controls are ON or OFF.

## Build

Run from the repository root:

```bash
dotnet build Code/TaxiTraffic.csproj -c Release -p:TaxiPerfTest=true -t:Rebuild
```

Use the output `Code/bin/Release/net48/TaxiTraffic.dll` in place of the existing TaxiTraffic DLL. Do **not** load both.
The test build is a Release build with an extra `TAXI_PERF_TEST` compilation symbol, not a Debug build.

## Short test

1. Keep one untouched city save, ideally starting before rush hour. Use the same playset and AdvancedSimulationSpeed target **8x** for both runs.
2. **OFF:** Taxi Traffic settings Residents 0%, Commuters OFF, Tourists OFF. Reload the original save and run through 9 AM.
3. Exit/restart and **ON:** set your normal controls (e.g. Residents 50%, Commuters ON, Tourists OFF). Reload that **same original save** and run through 9 AM.
4. Copy TaxiTraffic.log after each run and label them OFF and ON. Do not overwrite the original city save.

The settings toggle itself is live; the reload is for a **fair comparison**, not because TaxiTraffic needs restarting to apply settings.

## Log records

- `[TAXI-BENCHMARK]` confirms that the passive logger started.
- `[TAXI-SPEED]` every ~10 real seconds: taxi control ON/OFF, city time, requested speed, actual smooth speed, simulation frame.
- `[TAXI-SPEED-HOUR]`: per-game-hour average, median, minimum, maximum (only when ASS target is 8x).
- `[TAXI-PERF]`: ~10-second totals of resident eligibility candidates and changes, reapply candidates, RideNeeder candidates, requests stopped/intercepted.

`[TAXI-PERF]` reports **workload counts**, not completed Burst job CPU time. The existing debug scheduling timers also do not measure completed job execution. Assess actual city impact using speed samples.

If running NoCustomersFix alongside TaxiTraffic, keep its balancing setting unchanged in both trials; NCF is **not needed** to measure TaxiTraffic with this build.

## Public Release

Rebuild without the performance symbol:

```bash
dotnet build Code/TaxiTraffic.csproj -c Release -p:TaxiPerfTest=false -t:Rebuild
```

The passive speed logger and workload log code are excluded from this standard Release DLL.
