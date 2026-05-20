# RnzTrauer Console Configuration Migration

## Bug
The console application depended on a JSON file beside the executable for both operational defaults and secrets. This made credential handling unsafe and deployment-sensitive.

## Task
Migrate the console startup configuration from `RNZ_Config.json` to `System.Configuration` app settings plus .NET user secrets.

## Feature
- Non-sensitive defaults now come from `App.config` `appSettings`.
- Sensitive values can be provided through user secrets and override app settings.
- Startup validation now reports missing configuration keys early.
- A sample `UserSecrets.sample.json` is included for onboarding.

## Backlog Impact
- The legacy JSON loader remains in core for other consumers and existing tests.
- If additional loaders need the same pattern, extract a shared configuration abstraction later.

## Validation
- Add unit tests for secret overlay behavior and required-field validation.
- Build and run the relevant test project before completion.
