# Integration Tests

Integration tests have been separated from other tests due to their running time and cost. Typically expected to be between
30-60s per test. This is due to making actual calls to BBG DL. As per DL docs we are required to not poll for results
too often and they recommendation of minimum of 30s.

To prevent them from slowing down development, and from accidentally incurring unexpected costs, they have been separated into this folder

They have also all been tagged with the 'Category("Unsafe")' annotation, which means that they will not run unless specifically requested.
Note that the Test Runner in VS <4.0 will still run them due to an issue in test discovery (see https://github.com/nunit/nunit3-vs-adapter/issues/658)