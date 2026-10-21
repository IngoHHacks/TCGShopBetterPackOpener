# TCG Shop Better Pack Opener

A mod for TCG Card Shop Simulator that makes the pack opening machines return high-value packs to the player, configurable to different thresholds and card types.

## Installation

### Automatic Installation

1. Download any mod manager that supports Thunderstore, such as [Thunderstore Mod Manager](https://www.overwolf.com/app/thunderstore-thunderstore_mod_manager), [r2modman](https://thunderstore.io/c/tcg-card-shop-simulator/p/ebkr/r2modman/), or [Gale](https://thunderstore.io/c/tcg-card-shop-simulator/p/Kesomannen/GaleModManager/).
2. Search for "TCG Shop Better Pack Opener" in the mod manager and install it, or go to the [Thunderstore listing](https://thunderstore.io/c/tcg-card-shop-simulator/p/IngoH/TCGShopBetterPackOpener/) and select "Install with App".
3. Run the game via the mod manager.

### Manual Installation

1. Install [BepInEx](https://thunderstore.io/c/tcg-card-shop-simulator/p/BepInEx/BepInExPack/) if you haven't already.  
   - Refer to the BepInEx manual installation instructions (found on the Thunderstore listing) for help if needed.
2. Download the latest release of the mod from the [Thunderstore listing](https://thunderstore.io/c/tcg-card-shop-simulator/p/IngoH/TCGShopBetterPackOpener/) or the [GitHub releases page](https://github.com/IngoHHacks/TCGShopBetterPackOpener/releases/latest).
3. Extract the contents of the downloaded zip file's `plugins` folder into your game's `BepInEx/plugins` folder. If the `plugins` folder does not exist, create it.
4. Download and install the [Configuration_Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/latest) mod in the same way. This is a required dependency.
5. Run the game.

## Before You Start

It's always a good idea to manually back up your save files when using mods, just in case. You can find your save files in the `%USERPROFILE%\AppData\LocalLow\OPNeonGames\Card Shop Simulator` folder. The risk should be minimal since the mod doesn't touch the regular save files and the game has a built-in backup system, but it's still a good idea to back up your save files before using mods.  

If you don't want to read the rest of this README, you can just install the mod and start playing. The only thing you may want to know is that you can't cancel opening packs returned by the pack opening machines.

## How It Works (Technical Details)

- Having the good cards just pop up in the pack opening machine screen is really boring. This mod improves that by making the pack opening machines return the fun packs to the player so they can open them manually and enjoy the experience of opening packs with good cards in them.
- The threshold can be configured in the mod's configuration file as seen below. The default threshold is $100. Additionally, packs can be returned based on their contents matching a keyword filter, which is also configurable in the mod's configuration file.
- Pack opening machines work by opening packs inside them evenly distributed over their process time. If a high-value pack is detected, the mod will cancel the cards from being added to the pack opening machine's output and instead queue the pack to be returned to the player once the player interacts with the pack opening machine again when the pack opening machine is done processing.
- Queued packs to return from the pack opening machines are stored in a separate save file, so they will persist across game sessions as long as the mod is installed.

## Limitations & Practical Notes

- You can't cancel opening packs returned by the pack opening machines. After the collection screen is closed, the pack opening machine screen will appear and stopping the pack from being opened is disabled. You might be stuck for a while if the pack opening machines return a lot of packs to you at once.
- Moving the pack opening machines around or boxing them up will retain the queued packs to return during the session. However, boxed up pack opening machines will not save their queued packs to return when the game is closed.
- If all packs match the filter, the last one will not be returned by the pack opening machines since the pack opening machines won't work otherwise.

## Configuration

The mod currently has the following configuration options available in the `BepInEx/config/net.ingoh.tcgcardshopsimulator.betterpackopener.cfg` file:
- `Enabled`: Whether the mod is enabled or not. Default is `true`. Disabling the mod only disables pack opening machines queuing new high-value packs to return. Packs that have already been queued will still be given to the player if the mod is disabled but still installed. Default is `true`.
- `ReturnThreshold`: If the total market value of a pack is equal to or above this value, the pack opening machines will return it. This value is in dollars. For Yen (JPY), divide the in-game Yen value by 100 to get the dollar value. For Yuan (CNY), divide by 2.5. For Ringgits (MYR) and Australian Dollars (AUD), divide by 2. Other currencies are equivalent to USD. Default is `100.0`.
- `AlwaysReturnFilter`: A comma-separated list of card keyword groups that will always have packs containing its cards returned by the pack opening machines, regardless of their value. See below for details. Can be left empty to disable this feature. Default is empty.
- `DisplayReturnedAmount`: Whether pack opening machines will display the amount of packs queued to be returned to the player. Default is `true`.
- `DebugKeybind`: The keybind to toggle the debug GUI. Can be a combination, e.g. `F2 + LeftAlt` Default is `F2`.

### Keywords

Keywords work in groups, separated by commas. For each group, the card/pack must match ALL keywords in the group for the pack to be returned. For example, `ghost god, foil fullart` translates to `ghost AND god OR foil AND fullart`. Below is a list of keywords that can be used in the `AlwaysReturnFilter` configuration option.  
For all keywords except `god`, the pack will be returned if it contains at least one card that matches the keyword. For `god`, the pack will only be returned if all cards in the pack match the other keywords, or if no other keywords are specified, the pack is considered a "God Pack" as specified in keyword description below.
Keywords are case-insensitive, unordered, and dashes and underscores are ignored. For example, `first-edition foil` and `FOIL FirstEdition` are equivalent. For simplicity, all keywords are listed in lowercase and without dashes or underscores.

#### Expansion Keywords

- `tetramon`: Tetramon (base) Expansion
- `destiny`: Destiny Expansion
- `ascension`: Ascension Expansion
- `ghost`: Ghost Expansion (Ghost cards)

#### Border Keywords

- `basic`: Basic Border
- `firstedition`: First Edition Border (Only available in Tetramon and Destiny)
- `silver`: Silver Border (For Ascension, this also matches Silver Full Art cards)
- `gold`: Gold Border
- `ex`: EX Border (For Ascension, this matches EX Full Art cards)
- `fullart`: Full Art (For Ascension, this matches all Full Art cards, including Silver and EX Full Art cards)
- `borderless`: Borderless (Full Art, only available in Ascension)

You can append `+` to any of the border keywords to match that border type and all higher border types. For example, `silver+` will match Silver, Gold, EX, and Full Art cards.  
For `+`, the order of border types from lowest to highest is the same as the order listed above, with Basic being the lowest and Borderless being the highest. The only exception is in Ascension where Silver Full Art is not considered as `gold+` or `ex+`.

#### Other Keywords

- `foil`: Foil card
- `god`: God Pack (All cards in the pack match the specified keywords. Using this without other keywords defaults to `god firstedition+,god foil,god ghost`.)

For a keyword group to be valid, it must contain at least one non-basic Border Keyword, Other Keyword, or the `ghost` Expansion Keyword. For example, `foil destiny` is valid, but `basic tetramon` is not valid. Invalid keyword groups will be ignored without any warning or error message.

## Acknowledgements
    
- The unknown person in Insym's YouTube chat who suggested the idea
- Insym, who told about above person's idea during his livestream

## License

This mod is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.  
The mod uses the [Newtonsoft.Json](https://www.newtonsoft.com/json) library, which is also licensed under the MIT License. See the [Newtonsoft.Json GitHub repository License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md). Newtonsoft.Json is bundled with the release, so you don't need to install it separately.

You are free to redistribute this mod on other mod hosting sites like Nexus Mods (I'm not touching Nexus Mods myself, thank you very much), but please include a link to the GitHub repository.

## Contact

[@ingoh](https://discord.com/users/381801621950169089) on Discord  
[@ingohhacks](t.me/ingohhacks) on Telegram  
[contact@ingoh.net](mailto:contact@ingoh.net) via email