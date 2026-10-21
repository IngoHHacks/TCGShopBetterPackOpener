# Changelog

## 1.0.1
- Removed Newtonsoft.Json from the Thunderstore release package, instead making it a dependency. The GitHub release package still bundles Newtonsoft.Json for convenience.
- Added `Configuration_Manager` as a dependency to the Thunderstore release package. This is not included in the GitHub release package since it's not a library. Manually installing `Configuration_Manager` is required when manually installing this mod from the GitHub release package.