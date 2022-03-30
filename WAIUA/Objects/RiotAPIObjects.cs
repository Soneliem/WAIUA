using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WAIUA.Objects;

public class UserInfoResponse
{
    [JsonPropertyName("country")] public string Country { get; set; }
    [JsonPropertyName("sub")] public Guid Sub { get; set; }
    [JsonPropertyName("email_verified")] public bool Email_verified { get; set; }
    [JsonPropertyName("player_plocale")] public string Player_plocale { get; set; }
    [JsonPropertyName("country_at")] public long? Country_at { get; set; }
    [JsonPropertyName("phone_number_verified")] public bool Phone_number_verified { get; set; }
    [JsonPropertyName("account_verified")] public bool Account_verified { get; set; }
    [JsonPropertyName("ppid")] public Guid? Ppid { get; set; }
    [JsonPropertyName("player_locale")] public string Player_locale { get; set; }
    [JsonPropertyName("age")] public int Age { get; set; }
    [JsonPropertyName("jti")] public string Jti { get; set; }
    [JsonExtensionData] public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class EntitlementsResponse
{
    [JsonPropertyName("accessToken")] public string AccessToken { get; set; }

    [JsonPropertyName("entitlements")] public object[] Entitlements { get; set; }

    [JsonPropertyName("issuer")] public Uri Issuer { get; set; }

    [JsonPropertyName("subject")] public Guid Subject { get; set; }

    [JsonPropertyName("token")] public string Token { get; set; }
}

public class ExternalSessionsResponse
{
    // [JsonExtensionData] public Dictionary<string, ExternalSessions> RandString { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public partial class ExternalSessions
{
    [JsonPropertyName("exitCode")] public int ExitCode { get; set; }

    [JsonPropertyName("exitReason")] public object ExitReason { get; set; }

    [JsonPropertyName("isInternal")] public bool IsInternal { get; set; }

    [JsonPropertyName("launchConfiguration")]
    public LaunchConfiguration LaunchConfiguration { get; set; }

    [JsonPropertyName("patchlineFullName")]
    public string PatchlineFullName { get; set; }

    [JsonPropertyName("patchlineId")] public string PatchlineId { get; set; }

    [JsonPropertyName("phase")] public string Phase { get; set; }

    [JsonPropertyName("productId")] public string ProductId { get; set; }

    [JsonPropertyName("version")] public string Version { get; set; }
}

public class LaunchConfiguration
{
    [JsonPropertyName("arguments")] public string[] Arguments { get; set; }

    [JsonPropertyName("executable")] public string Executable { get; set; }

    [JsonPropertyName("locale")] public string Locale { get; set; }

    [JsonPropertyName("voiceLocale")] public object VoiceLocale { get; set; }

    [JsonPropertyName("workingDirectory")] public string WorkingDirectory { get; set; }
}

public class LiveMatchIDResponse
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("MatchID")] public Guid MatchId { get; set; }

    [JsonPropertyName("Version")] public long Version { get; set; }
}

public class LiveMatchResponse
{
    [JsonPropertyName("MatchID")] public Guid MatchId { get; set; }

    [JsonPropertyName("Version")] public long Version { get; set; }

    [JsonPropertyName("State")] public string State { get; set; }

    [JsonPropertyName("MapID")] public string MapId { get; set; }

    [JsonPropertyName("ModeID")] public string ModeId { get; set; }

    [JsonPropertyName("ProvisioningFlow")] public string ProvisioningFlow { get; set; }

    [JsonPropertyName("GamePodID")] public string GamePodId { get; set; }

    [JsonPropertyName("AllMUCName")] public string AllMucName { get; set; }

    [JsonPropertyName("TeamMUCName")] public string TeamMucName { get; set; }

    [JsonPropertyName("TeamVoiceID")] public string TeamVoiceId { get; set; }

    [JsonPropertyName("IsReconnectable")] public bool IsReconnectable { get; set; }

    [JsonPropertyName("ConnectionDetails")]
    public ConnectionDetails ConnectionDetails { get; set; }

    [JsonPropertyName("PostGameDetails")] public object PostGameDetails { get; set; }

    [JsonPropertyName("Players")] public RiotPlayer[] Players { get; set; }

    [JsonPropertyName("MatchmakingData")] public object MatchmakingData { get; set; }
}

public class ConnectionDetails
{
    [JsonPropertyName("GameServerHosts")] public string[] GameServerHosts { get; set; }

    [JsonPropertyName("GameServerHost")] public string GameServerHost { get; set; }

    [JsonPropertyName("GameServerPort")] public int GameServerPort { get; set; }

    [JsonPropertyName("GameServerObfuscatedIP")]
    public long GameServerObfuscatedIp { get; set; }

    [JsonPropertyName("GameClientHash")] public long GameClientHash { get; set; }

    [JsonPropertyName("PlayerKey")] public string PlayerKey { get; set; }
}

public class RiotPlayer
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("TeamID")] public string TeamId { get; set; }

    [JsonPropertyName("CharacterID")] public Guid CharacterId { get; set; }

    [JsonPropertyName("PlayerIdentity")] public PlayerIdentity PlayerIdentity { get; set; }

    [JsonPropertyName("SeasonalBadgeInfo")]
    public SeasonalBadgeInfo SeasonalBadgeInfo { get; set; }

    [JsonPropertyName("IsCoach")] public bool IsCoach { get; set; }
}

public class PlayerIdentity
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("PlayerCardID")] public Guid PlayerCardId { get; set; }

    [JsonPropertyName("PlayerTitleID")] public Guid PlayerTitleId { get; set; }

    [JsonPropertyName("AccountLevel")] public int AccountLevel { get; set; }

    [JsonPropertyName("PreferredLevelBorderID")]
    public Guid PreferredLevelBorderId { get; set; }

    [JsonPropertyName("Incognito")] public bool Incognito { get; set; }

    [JsonPropertyName("HideAccountLevel")] public bool HideAccountLevel { get; set; }
}

public class SeasonalBadgeInfo
{
    [JsonPropertyName("SeasonID")] public string SeasonId { get; set; }

    [JsonPropertyName("NumberOfWins")] public int NumberOfWins { get; set; }

    [JsonPropertyName("WinsByTier")] public object WinsByTier { get; set; }

    [JsonPropertyName("Rank")] public int Rank { get; set; }

    [JsonPropertyName("LeaderboardRank")] public int LeaderboardRank { get; set; }
}


public class NameServiceResponse
{
    [JsonPropertyName("DisplayName")] public string DisplayName { get; set; }

    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("GameName")] public string GameName { get; set; }

    [JsonPropertyName("TagLine")] public string TagLine { get; set; }
}
public partial class MatchLoadoutsResponse
{
    [JsonPropertyName("Loadouts")]
    public LoadoutElement[] Loadouts { get; set; }
}

public partial class LoadoutElement
{
    [JsonPropertyName("CharacterID")]
    public Guid CharacterId { get; set; }

    [JsonPropertyName("Loadout")]
    public LoadoutLoadout Loadout { get; set; }
}

public partial class LoadoutLoadout
{
    [JsonPropertyName("Sprays")]
    public Sprays Sprays { get; set; }

    [JsonPropertyName("Items")]
    public Dictionary<string, ItemValue> Items { get; set; }
}

public partial class ItemValue
{
    [JsonPropertyName("ID")]
    public Guid Id { get; set; }

    [JsonPropertyName("TypeID")]
    public Guid TypeId { get; set; }

    [JsonPropertyName("Sockets")]
    public Dictionary<string, Socket> Sockets { get; set; }
}

public partial class Socket
{
    [JsonPropertyName("ID")]
    public Guid Id { get; set; }

    [JsonPropertyName("Item")]
    public SocketItem Item { get; set; }
}

public partial class SocketItem
{
    [JsonPropertyName("ID")]
    public Guid Id { get; set; }

    [JsonPropertyName("TypeID")]
    public Guid TypeId { get; set; }
}

public partial class Sprays
{
    [JsonPropertyName("SpraySelections")]
    public SpraySelection[] SpraySelections { get; set; }
}

public partial class SpraySelection
{
    [JsonPropertyName("SocketID")]
    public Guid SocketId { get; set; }

    [JsonPropertyName("SprayID")]
    public Guid SprayId { get; set; }

    [JsonPropertyName("LevelID")]
    public Guid LevelId { get; set; }
}

public partial class CompetitiveUpdatesResponse
{
    [JsonPropertyName("Version")]
    public long Version { get; set; }

    [JsonPropertyName("Subject")]
    public Guid Subject { get; set; }

    [JsonPropertyName("Matches")]
    public CompetitiveUpdatesMatch[] Matches { get; set; }
}

public partial class CompetitiveUpdatesMatch
{
    [JsonPropertyName("MatchID")]
    public Guid MatchId { get; set; }

    [JsonPropertyName("MapID")]
    public string MapId { get; set; }

    [JsonPropertyName("SeasonID")]
    public Guid SeasonId { get; set; }

    [JsonPropertyName("MatchStartTime")]
    public long MatchStartTime { get; set; }

    [JsonPropertyName("TierAfterUpdate")]
    public int TierAfterUpdate { get; set; }

    [JsonPropertyName("TierBeforeUpdate")]
    public int TierBeforeUpdate { get; set; }

    [JsonPropertyName("RankedRatingAfterUpdate")]
    public int RankedRatingAfterUpdate { get; set; }

    [JsonPropertyName("RankedRatingBeforeUpdate")]
    public int RankedRatingBeforeUpdate { get; set; }

    [JsonPropertyName("RankedRatingEarned")]
    public int RankedRatingEarned { get; set; }

    [JsonPropertyName("RankedRatingPerformanceBonus")]
    public int RankedRatingPerformanceBonus { get; set; }

    [JsonPropertyName("CompetitiveMovement")]
    public string CompetitiveMovement { get; set; }

    [JsonPropertyName("AFKPenalty")]
    public int AfkPenalty { get; set; }
}


public partial class MmrResponse
{
    [JsonPropertyName("Version")]
    public long Version { get; set; }

    [JsonPropertyName("Subject")]
    public Guid Subject { get; set; }

    [JsonPropertyName("NewPlayerExperienceFinished")]
    public bool NewPlayerExperienceFinished { get; set; }

    [JsonPropertyName("QueueSkills")]
    public QueueSkills QueueSkills { get; set; }

    [JsonPropertyName("LatestCompetitiveUpdate")]
    public LatestCompetitiveUpdate LatestCompetitiveUpdate { get; set; }

    [JsonPropertyName("IsLeaderboardAnonymized")]
    public bool IsLeaderboardAnonymized { get; set; }

    [JsonPropertyName("IsActRankBadgeHidden")]
    public bool IsActRankBadgeHidden { get; set; }
}

public partial class LatestCompetitiveUpdate
{
    [JsonPropertyName("MatchID")]
    public Guid MatchId { get; set; }

    [JsonPropertyName("MapID")]
    public string MapId { get; set; }

    [JsonPropertyName("SeasonID")]
    public Guid SeasonId { get; set; }

    [JsonPropertyName("MatchStartTime")]
    public long MatchStartTime { get; set; }

    [JsonPropertyName("TierAfterUpdate")]
    public int TierAfterUpdate { get; set; }

    [JsonPropertyName("TierBeforeUpdate")]
    public int TierBeforeUpdate { get; set; }

    [JsonPropertyName("RankedRatingAfterUpdate")]
    public int RankedRatingAfterUpdate { get; set; }

    [JsonPropertyName("RankedRatingBeforeUpdate")]
    public int RankedRatingBeforeUpdate { get; set; }

    [JsonPropertyName("RankedRatingEarned")]
    public int RankedRatingEarned { get; set; }

    [JsonPropertyName("RankedRatingPerformanceBonus")]
    public int RankedRatingPerformanceBonus { get; set; }

    [JsonPropertyName("CompetitiveMovement")]
    public string CompetitiveMovement { get; set; }

    [JsonPropertyName("AFKPenalty")]
    public int AfkPenalty { get; set; }
}

public partial class QueueSkills
{
    [JsonPropertyName("competitive")]
    public Competitive Competitive { get; set; }

    [JsonPropertyName("custom")]
    public Custom Custom { get; set; }

    [JsonPropertyName("deathmatch")]
    public Custom Deathmatch { get; set; }

    [JsonPropertyName("ggteam")]
    public Custom Ggteam { get; set; }

    [JsonPropertyName("newmap")]
    public Custom Newmap { get; set; }

    [JsonPropertyName("onefa")]
    public Custom Onefa { get; set; }

    [JsonPropertyName("seeding")]
    public Seeding Seeding { get; set; }

    [JsonPropertyName("snowball")]
    public Custom Snowball { get; set; }

    [JsonPropertyName("spikerush")]
    public Custom Spikerush { get; set; }

    [JsonPropertyName("unrated")]
    public Custom Unrated { get; set; }
}

public partial class Competitive
{
    [JsonPropertyName("TotalGamesNeededForRating")]
    public int TotalGamesNeededForRating { get; set; }

    [JsonPropertyName("TotalGamesNeededForLeaderboard")]
    public int TotalGamesNeededForLeaderboard { get; set; }

    [JsonPropertyName("CurrentSeasonGamesNeededForRating")]
    public int CurrentSeasonGamesNeededForRating { get; set; }

    [JsonPropertyName("SeasonalInfoBySeasonID")]
    public SeasonalInfoBySeasonId SeasonalInfoBySeasonId { get; set; }
}

public partial class SeasonalInfoBySeasonId
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Act { get; set; }
}

public partial class ActInfo
{
    [JsonPropertyName("SeasonID")]
    public Guid SeasonId { get; set; }

    [JsonPropertyName("NumberOfWins")]
    public int NumberOfWins { get; set; }

    [JsonPropertyName("NumberOfWinsWithPlacements")]
    public int NumberOfWinsWithPlacements { get; set; }

    [JsonPropertyName("NumberOfGames")]
    public int NumberOfGames { get; set; }

    [JsonPropertyName("Rank")]
    public int Rank { get; set; }

    [JsonPropertyName("CapstoneWins")]
    public int CapstoneWins { get; set; }

    [JsonPropertyName("LeaderboardRank")]
    public int LeaderboardRank { get; set; }

    [JsonPropertyName("CompetitiveTier")]
    public int CompetitiveTier { get; set; }

    [JsonPropertyName("RankedRating")]
    public int RankedRating { get; set; }

    [JsonPropertyName("WinsByTier")]
    public Dictionary<string, int> WinsByTier { get; set; }

    [JsonPropertyName("GamesNeededForRating")]
    public int GamesNeededForRating { get; set; }

    [JsonPropertyName("TotalWinsNeededForRank")]
    public int TotalWinsNeededForRank { get; set; }
}


public partial class Custom
{
    [JsonPropertyName("TotalGamesNeededForRating")]
    public int TotalGamesNeededForRating { get; set; }

    [JsonPropertyName("TotalGamesNeededForLeaderboard")]
    public int TotalGamesNeededForLeaderboard { get; set; }

    [JsonPropertyName("CurrentSeasonGamesNeededForRating")]
    public int CurrentSeasonGamesNeededForRating { get; set; }

    [JsonPropertyName("SeasonalInfoBySeasonID")]
    public SeasonalInfoBySeasonId SeasonalInfoBySeasonId { get; set; }
}

public partial class Seeding
{
    [JsonPropertyName("TotalGamesNeededForRating")]
    public int TotalGamesNeededForRating { get; set; }

    [JsonPropertyName("TotalGamesNeededForLeaderboard")]
    public int TotalGamesNeededForLeaderboard { get; set; }

    [JsonPropertyName("CurrentSeasonGamesNeededForRating")]
    public int CurrentSeasonGamesNeededForRating { get; set; }

    [JsonPropertyName("SeasonalInfoBySeasonID")]
    public SeasonalInfoBySeasonId SeasonalInfoBySeasonId { get; set; }
}

public partial class LeaderboardsResponse
{
    [JsonPropertyName("Deployment")]
    public string Deployment { get; set; }

    [JsonPropertyName("QueueID")]
    public string QueueId { get; set; }

    [JsonPropertyName("SeasonID")]
    public Guid SeasonId { get; set; }

    [JsonPropertyName("Players")]
    public object[] Players { get; set; }

    [JsonPropertyName("totalPlayers")]
    public int TotalPlayers { get; set; }

    [JsonPropertyName("immortalStartingPage")]
    public int ImmortalStartingPage { get; set; }

    [JsonPropertyName("immortalStartingIndex")]
    public int ImmortalStartingIndex { get; set; }

    [JsonPropertyName("topTierRRThreshold")]
    public int TopTierRrThreshold { get; set; }

    [JsonPropertyName("tierDetails")]
    public Dictionary<string, TierDetail> TierDetails { get; set; }

    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("query")]
    public string Query { get; set; }
}

public partial class TierDetail
{
    [JsonPropertyName("rankedRatingThreshold")]
    public int RankedRatingThreshold { get; set; }

    [JsonPropertyName("startingPage")]
    public int StartingPage { get; set; }

    [JsonPropertyName("startingIndex")]
    public int StartingIndex { get; set; }
}

public partial class ContentResponse
{
    [JsonPropertyName("DisabledIDs")]
    public object[] DisabledIDs { get; set; }

    [JsonPropertyName("Seasons")]
    public Event[] Seasons { get; set; }

    [JsonPropertyName("Events")]
    public Event[] Events { get; set; }
}

public partial class Event
{
    [JsonPropertyName("ID")]
    public Guid Id { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("StartTime")]
    public string StartTime { get; set; }

    [JsonPropertyName("EndTime")]
    public string EndTime { get; set; }

    [JsonPropertyName("IsActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("DevelopmentOnly")]
    public bool DevelopmentOnly { get; set; }

    [JsonPropertyName("Type"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }
}

public partial class PresencesResponse
{
    [JsonPropertyName("presences")]
    public Presence[] Presences { get; set; }
}

public partial class Presence
{
    [JsonPropertyName("actor")]
    public string Actor { get; set; }

    [JsonPropertyName("basic")]
    public string Basic { get; set; }

    [JsonPropertyName("details")]
    public string Details { get; set; }

    [JsonPropertyName("game_name")]
    public string GameName { get; set; }

    [JsonPropertyName("game_tag")]
    public string GameTag { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("patchline")]
    public string Patchline { get; set; }

    [JsonPropertyName("pid")]
    public string Pid { get; set; }

    [JsonPropertyName("platform")]
    public string Platform { get; set; }

    [JsonPropertyName("private")]
    public string Private { get; set; }

    [JsonPropertyName("privateJwt")]
    public object PrivateJwt { get; set; }

    [JsonPropertyName("product")]
    public string Product { get; set; }

    [JsonPropertyName("puuid")]
    public Guid Puuid { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; }

    [JsonPropertyName("resource")]
    public string Resource { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("time"), JsonIgnore]
    public long Time { get; set; }
}

public partial class PresencesPrivate
{
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("sessionLoopState")]
    public string SessionLoopState { get; set; }

    [JsonPropertyName("partyOwnerSessionLoopState")]
    public string PartyOwnerSessionLoopState { get; set; }

    [JsonPropertyName("customGameName")]
    public string CustomGameName { get; set; }

    [JsonPropertyName("customGameTeam")]
    public string CustomGameTeam { get; set; }

    [JsonPropertyName("partyOwnerMatchMap")]
    public string PartyOwnerMatchMap { get; set; }

    [JsonPropertyName("partyOwnerMatchCurrentTeam")]
    public string PartyOwnerMatchCurrentTeam { get; set; }

    [JsonPropertyName("partyOwnerMatchScoreAllyTeam")]
    public int PartyOwnerMatchScoreAllyTeam { get; set; }

    [JsonPropertyName("partyOwnerMatchScoreEnemyTeam")]
    public int PartyOwnerMatchScoreEnemyTeam { get; set; }

    [JsonPropertyName("partyOwnerProvisioningFlow")]
    public string PartyOwnerProvisioningFlow { get; set; }

    [JsonPropertyName("provisioningFlow")]
    public string ProvisioningFlow { get; set; }

    [JsonPropertyName("matchMap")]
    public string MatchMap { get; set; }

    [JsonPropertyName("partyId")]
    public Guid PartyId { get; set; }

    [JsonPropertyName("isPartyOwner")]
    public bool IsPartyOwner { get; set; }

    [JsonPropertyName("partyState")]
    public string PartyState { get; set; }

    [JsonPropertyName("partyAccessibility")]
    public string PartyAccessibility { get; set; }

    [JsonPropertyName("maxPartySize")]
    public int MaxPartySize { get; set; }

    [JsonPropertyName("queueId")]
    public string QueueId { get; set; }

    [JsonPropertyName("partyLFM")]
    public bool PartyLfm { get; set; }

    [JsonPropertyName("partyClientVersion")]
    public string PartyClientVersion { get; set; }

    [JsonPropertyName("partySize")]
    public int PartySize { get; set; }

    [JsonPropertyName("tournamentId")]
    public string TournamentId { get; set; }

    [JsonPropertyName("rosterId")]
    public string RosterId { get; set; }

    [JsonPropertyName("partyVersion")]
    public long PartyVersion { get; set; }

    [JsonPropertyName("queueEntryTime")]
    public string QueueEntryTime { get; set; }

    [JsonPropertyName("playerCardId")]
    public Guid PlayerCardId { get; set; }

    [JsonPropertyName("playerTitleId"), JsonIgnore]
    public Guid PlayerTitleId { get; set; }

    [JsonPropertyName("preferredLevelBorderId")]
    public string PreferredLevelBorderId { get; set; }

    [JsonPropertyName("accountLevel")]
    public int AccountLevel { get; set; }

    [JsonPropertyName("competitiveTier")]
    public int CompetitiveTier { get; set; }

    [JsonPropertyName("leaderboardPosition")]
    public int LeaderboardPosition { get; set; }

    [JsonPropertyName("isIdle")]
    public bool IsIdle { get; set; }
}

