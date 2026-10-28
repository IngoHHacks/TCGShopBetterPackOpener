# Changelog

## 1.0.2
- Fixed an issue caused by the game's 1.03 update changing items to no longer have rigidbodies by default, causing pack machines to fail to spawn packs to return to the player.

## 1.0.1
- Removed Newtonsoft.Json from the Thunderstore release package, instead making it a dependency. The GitHub release package still bundles Newtonsoft.Json for convenience.
- Added `Configuration_Manager` as a dependency to the Thunderstore release package. This is not included in the GitHub release package since it's not a library. Manually installing `Configuration_Manager` is required when manually installing this mod from the GitHub release package.