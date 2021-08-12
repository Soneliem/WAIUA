<p align="center">
<a href="https://github.com/Soneliem/WAIUA">
    <img src="WAIUA/Assets/logo.ico" alt="Logo" width="80" height="80">
  </a>
</p>
<h3 align="center">WAIUA</h3>
<h5 align="center">Who Am I Up Against?</h5>

  <p align="center">
    A Windows application to view player ranks and stats in a live Valorant Match
    <br />


<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
      <li><a href="#current-features">Current Features</a></li>
      <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap-and-known-bugs">Roadmap And Known Bugs</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>


## About The Project
![Screenshot](https://imgur.com/XGkE4k1.png)
A simple(for you, not me) Windows GUI app that lets you see the ranks, recent games and other info of players in a live Valorant match while you're still playing it.

### Current Features
Displays for each player:
* Current Rank
* Past three ranks (from last 3 acts, not last 3 played acts)
* W/L indicator for last three competitive games
* IGN, Agent, Card and Account Level

### Built With
* [WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/?view=netdesktop-5.0)
* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
* [.NET 5.0](https://dotnet.microsoft.com/)

I knew none of these things before making this lol

## Getting Started

To get the app up and running follow these simple steps.

### Prerequisites

* Windows 64-bit
* .NET (might be automatically installed)

### Installation

1. Download the latest .exe from the [Github releases page](https://github.com/Soneliem/WAIUA/releases)
2. Run the .exe

## Usage

Please ensure you are in a match when you try to use it. I swear if you complain about it not working and you're not in a match...

If Valorant is already running:
1. Press the big "WAIUA" button and wait about 20-30 seconds
2. Profit

If Valorant is not running:
1. Press the account button in the top left
2. Use your Riot Account details to log in. Remember to select your region from the bottom left.
3. Open Valorant LMAO
3. Press the big "WAIUA" button and wait about 20-30 seconds
4. Profit slightly less

Appologies about the long wait to load the information. I have to individually query each player's information from different APIs and then query another API to display images.

## Roadmap and Known Bugs

See the [open issues](https://github.com/Soneliem/WAIUA/issues) for a list of proposed features (and known bugs).

## Why I Made This

The main reason I made this was to detect smurfs. Being able to see an account that were bronze 1 last act and then miraculously became gold 3 this act while having an account level of 2 is a very clear indication of them being a smurf. I have chosen the set of available features carefully mainly to maintain the competitive integrity of the game. For example I could have given you access to player ranks while you're still choosing agents, but this will lead to people dodging because they don't want "bad" teammates. I could have also given you access to the enemy's last 90 games but that both breaches their privacy and could lead to bullying.

Another reason to making this was that I wanted some experience with .NET and GUI apps. This application uses C# as the backend, WPF as the frontend and .NET as the framework. I knew none of these so this was a fun journey. Because of this, the code is very messy but it does the job. I probably should have made it all asynchronously but I am lazy and I wanted to slowly get into C#. But mainly I am lazy.

## Contributing

I welcome any sort of contribution and am happy to take in any ~~hate/~~ feedback. Open source means open heart :)

## License

Distributed under the MIT License. See [LISCENSE](https://github.com/Soneliem/WAIUA/blob/master/LICENSE) for more information.

## Contact

Discord: Soneliem#4194  
Project Link: [https://github.com/Soneliem/WAIUA](https://github.com/Soneliem/WAIUA)

## Acknowledgements

* [techchrism for work on documenting Valorant endpoints](https://github.com/techchrism/valorant-api-docs)
* [RumbleMike for base C# authentication flow](https://github.com/RumbleMike/ValorantClientAPI)
* [Valorant-API.com](https://valorant-api.com/)
* [The guys on the Valorant App Developers Discord Server](https://discord.gg/a9yzrw3KAm)
* This project uses Riot's unofficial-private-notforpublicuse API for most of the information. Riot pls no kil

## DISCLAIMER
THIS PROJECT IS NOT ASSOCIATED OR ENDORSED BY RIOT GAMES. Riot Games, and all associated properties are trademarks or registered trademarks of Riot Games, Inc.
