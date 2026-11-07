# OasisEssential

> **⚠️ Archived – old project, no longer maintained.**
> Written in 2023 for the **Oasis** Unturned servers. It's kept here for reference only. It may not work with current versions of Unturned or RocketMod.

A RocketMod plugin for **Unturned** with the basic features every Oasis server used.

## What it does

- **Auto-broadcast messages**: rotating chat announcements on a set interval, each with its own color
- **Join and leave messages** in global chat
- **Custom death messages**: one for each death cause (gun, headshot, zombie, bleeding, landmine, vehicle and more)
- **Airdrop announcement** when an airdrop is incoming
- **Call vote (`/cv`)**: players vote to run a server command, such as setting day or night or changing the weather. Each vote has its own minimum player count, required percentage, timer and cooldown

## Commands & permissions

| Command | Permission | Description |
|---|---|---|
| `/cv <alias>` | `OasisEssential.cv` | Start a vote, or vote in one that's running (e.g. `/cv d` for day, `/cv n` for night) |

## Config

- `ShowJoinMessage`, `ShowLeaveMessage`, `ShowDeathMessages`: turn each message type on or off
- `Interval`: seconds between broadcast messages
- `Messages`: list of `Text` + `Color` pairs
- `Votes`: list of votes (`Name`, `Alias`, `Command`, `Help`, `Enabled`, `MinimumPlayers`, `RequiredPercent`, `Timer`, `CooldownTime`)

All player-facing text can be changed in the translation file.

## Tech

- C# / .NET Framework 4.8
- RocketMod
- `lib/` holds the reference assemblies needed to build

## Build

Open `OasisEssential.sln` in Visual Studio and build it. Then copy the DLL into `Rocket/Plugins`.

## Part of the Oasis project

[OasisEssential](https://github.com/MikeyBot111/OasisEssential) · [OasisTeleport](https://github.com/MikeyBot111/OasisTeleport) · [OasisKits](https://github.com/MikeyBot111/OasisKits) · [OasisRanks](https://github.com/MikeyBot111/OasisRanks) · [OasisVault](https://github.com/MikeyBot111/OasisVault) · [OasisDiscord](https://github.com/MikeyBot111/OasisDiscord) · [OasisCustomCommands](https://github.com/MikeyBot111/OasisCustomCommands) · [WebsiteOasis](https://github.com/MikeyBot111/WebsiteOasis) · [oasis-server-discord](https://github.com/MikeyBot111/oasis-server-discord)
