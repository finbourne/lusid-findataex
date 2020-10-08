# Regression Tests

Regression tests have been separated from other tests due to their running time. Typically expected to be between
30-60s per test. This is due to making actual calls to BBG DL. As per DL docs we are required to not poll for results
too often and they recommendation of minimum of 30s.

Separating to this directory allows developers to run them only when required. These are expected to run in CI.