# Tests

## Configuration

Requires all the same config as the main api project because it fires up a captive api host.

## Smoke

These test the API HTTP endpoints with a real client, with the API running in a
captive web host.

There shouldn't be too many of these as they are slow, but the should test the
major paths through the API to give confidence everything is connected
properly, (e.g. is the dependency injection wired up and the database
connected).

## DbIntegration

These test a class (usually a "service" class) against a real database. This
tests the fragile boundary across EF and real SQL.

There should be more of these, but they are still fairly expensive.

## Unit

These test classes in isolation. As much as possible should be pushed down to
these, leaving just enough smoke & integration tests to give confidence in the
whole system.
