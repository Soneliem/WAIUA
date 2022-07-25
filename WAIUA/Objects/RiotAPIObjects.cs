using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WAIUA.Objects;

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
    [JsonExtensionData] public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class ExternalSessions
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

public class MatchIDResponse
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

    [JsonPropertyName("PostGameDetails")]
    [JsonIgnore]
    public object PostGameDetails { get; set; }

    [JsonPropertyName("Players")] public RiotLivePlayer[] Players { get; set; }

    [JsonPropertyName("MatchmakingData")]
    [JsonIgnore]
    public object MatchmakingData { get; set; }
}

public class PreMatchResponse
{
    [JsonPropertyName("ID")] public Guid Id { get; set; }

    [JsonPropertyName("Version")] public long Version { get; set; }

    [JsonPropertyName("Teams")] public PreTeam[] Teams { get; set; }

    [JsonPropertyName("AllyTeam")] public PreTeam AllyTeam { get; set; }

    [JsonPropertyName("EnemyTeam")] public object EnemyTeam { get; set; }

    [JsonPropertyName("ObserverSubjects")] public object[] ObserverSubjects { get; set; }

    [JsonPropertyName("MatchCoaches")] public object[] MatchCoaches { get; set; }

    [JsonPropertyName("EnemyTeamSize")] public long EnemyTeamSize { get; set; }

    [JsonPropertyName("EnemyTeamLockCount")]
    public long EnemyTeamLockCount { get; set; }

    [JsonPropertyName("PregameState")] public string PregameState { get; set; }

    [JsonPropertyName("LastUpdated")] public string LastUpdated { get; set; }

    [JsonPropertyName("MapID")] public string MapId { get; set; }

    [JsonPropertyName("MapSelectPool")] public object[] MapSelectPool { get; set; }

    [JsonPropertyName("BannedMapIDs")] public object[] BannedMapIDs { get; set; }

    [JsonPropertyName("CastedVotes")] public object CastedVotes { get; set; }

    [JsonPropertyName("MapSelectSteps")] public object[] MapSelectSteps { get; set; }

    [JsonPropertyName("MapSelectStep")] public long MapSelectStep { get; set; }

    [JsonPropertyName("Team1")] public string Team1 { get; set; }

    [JsonPropertyName("GamePodID")] public string GamePodId { get; set; }

    [JsonPropertyName("Mode")] public string Mode { get; set; }

    [JsonPropertyName("VoiceSessionID")] public string VoiceSessionId { get; set; }

    [JsonPropertyName("MUCName")] public string MucName { get; set; }

    [JsonPropertyName("QueueID")] public string QueueId { get; set; }

    [JsonPropertyName("ProvisioningFlowID")]
    public string ProvisioningFlowId { get; set; }

    [JsonPropertyName("IsRanked")] public bool IsRanked { get; set; }

    [JsonPropertyName("PhaseTimeRemainingNS")]
    public long PhaseTimeRemainingNs { get; set; }

    [JsonPropertyName("StepTimeRemainingNS")]
    public long StepTimeRemainingNs { get; set; }

    [JsonPropertyName("altModesFlagADA")] public bool AltModesFlagAda { get; set; }

    [JsonPropertyName("TournamentMetadata")]
    public object TournamentMetadata { get; set; }
}

public class PreTeam
{
    [JsonPropertyName("TeamID")] public string TeamId { get; set; }

    [JsonPropertyName("Players")] public RiotPrePlayer[] Players { get; set; }
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

public class RiotLivePlayer
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("TeamID")] public string TeamId { get; set; }

    [JsonPropertyName("CharacterID")] public Guid CharacterId { get; set; }

    [JsonPropertyName("PlayerIdentity")] public PlayerIdentity PlayerIdentity { get; set; }

    [JsonPropertyName("SeasonalBadgeInfo")]
    public SeasonalBadgeInfo SeasonalBadgeInfo { get; set; }

    [JsonPropertyName("IsCoach")] public bool IsCoach { get; set; }
}

public class RiotPrePlayer
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("CharacterID")]
    [JsonIgnore]
    public Guid CharacterId { get; set; }

    [JsonPropertyName("CharacterSelectionState")]
    public string CharacterSelectionState { get; set; }

    [JsonPropertyName("PregamePlayerState")]
    public string PregamePlayerState { get; set; }

    [JsonPropertyName("CompetitiveTier")] public long CompetitiveTier { get; set; }
    [JsonPropertyName("PlayerIdentity")] public PlayerIdentity PlayerIdentity { get; set; }

    [JsonPropertyName("SeasonalBadgeInfo")]
    public SeasonalBadgeInfo SeasonalBadgeInfo { get; set; }

    [JsonPropertyName("IsCaptain")] public bool IsCaptain { get; set; }
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

public class MatchLoadoutsResponse
{
    [JsonPropertyName("Loadouts")] public LoadoutElement[] Loadouts { get; set; }
}

public class PreMatchLoadoutsResponse
{
    [JsonPropertyName("Loadouts")] public LoadoutLoadout[] Loadouts { get; set; }

    [JsonPropertyName("LoadoutsValid")]
    [JsonIgnore]
    public bool LoadoutsValid { get; set; }
}

public class LoadoutElement
{
    [JsonPropertyName("CharacterID")]
    [JsonIgnore]
    public Guid CharacterId { get; set; }

    [JsonPropertyName("Loadout")] public LoadoutLoadout Loadout { get; set; }
}

public class LoadoutLoadout
{
    [JsonPropertyName("Sprays")] public Sprays Sprays { get; set; }

    [JsonPropertyName("Items")] public Dictionary<string, ItemValue> Items { get; set; }
}

public class ItemValue
{
    [JsonPropertyName("ID")] public Guid Id { get; set; }

    [JsonPropertyName("TypeID")] public Guid TypeId { get; set; }

    [JsonPropertyName("Sockets")] public Dictionary<string, Socket> Sockets { get; set; }
}

public class Socket
{
    [JsonPropertyName("ID")] public Guid Id { get; set; }

    [JsonPropertyName("Item")] public SocketItem Item { get; set; }
}

public class SocketItem
{
    [JsonPropertyName("ID")] public Guid Id { get; set; }

    [JsonPropertyName("TypeID")] public Guid TypeId { get; set; }
}

public class Sprays
{
    [JsonPropertyName("SpraySelections")] public SpraySelection[] SpraySelections { get; set; }
}

public class SpraySelection
{
    [JsonPropertyName("SocketID")] public Guid SocketId { get; set; }

    [JsonPropertyName("SprayID")] public Guid SprayId { get; set; }

    [JsonPropertyName("LevelID")] public Guid LevelId { get; set; }
}

public class CompetitiveUpdatesResponse
{
    [JsonPropertyName("Version")] public long Version { get; set; }

    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("Matches")] public CompetitiveUpdates[] Matches { get; set; }
}

public class CompetitiveUpdates
{
    [JsonPropertyName("MatchID")] public Guid MatchId { get; set; }

    [JsonPropertyName("MapID")] public string MapId { get; set; }

    [JsonPropertyName("SeasonID")]
    [JsonIgnore]
    public string? SeasonId { get; set; }

    [JsonPropertyName("MatchStartTime")] public long MatchStartTime { get; set; }

    [JsonPropertyName("TierAfterUpdate")] public int TierAfterUpdate { get; set; }

    [JsonPropertyName("TierBeforeUpdate")] public int TierBeforeUpdate { get; set; }

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

    [JsonPropertyName("AFKPenalty")] public int AfkPenalty { get; set; }
}

public class MmrResponse
{
    [JsonPropertyName("Version")] public long Version { get; set; }

    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("NewPlayerExperienceFinished")]
    public bool NewPlayerExperienceFinished { get; set; }

    [JsonPropertyName("QueueSkills")] public QueueSkills QueueSkills { get; set; }

    [JsonPropertyName("LatestCompetitiveUpdate")]
    public CompetitiveUpdates LatestCompetitiveUpdate { get; set; }

    [JsonPropertyName("IsLeaderboardAnonymized")]
    public bool IsLeaderboardAnonymized { get; set; }

    [JsonPropertyName("IsActRankBadgeHidden")]
    public bool IsActRankBadgeHidden { get; set; }
}

// public class LatestCompetitiveUpdate
// {
//     [JsonPropertyName("MatchID")] public Guid MatchId { get; set; }
//
//     [JsonPropertyName("MapID")] public string MapId { get; set; }
//
//     [JsonPropertyName("SeasonID")] public Guid SeasonId { get; set; }
//
//     [JsonPropertyName("MatchStartTime")] public long MatchStartTime { get; set; }
//
//     [JsonPropertyName("TierAfterUpdate")] public int TierAfterUpdate { get; set; }
//
//     [JsonPropertyName("TierBeforeUpdate")] public int TierBeforeUpdate { get; set; }
//
//     [JsonPropertyName("RankedRatingAfterUpdate")]
//     public int RankedRatingAfterUpdate { get; set; }
//
//     [JsonPropertyName("RankedRatingBeforeUpdate")]
//     public int RankedRatingBeforeUpdate { get; set; }
//
//     [JsonPropertyName("RankedRatingEarned")]
//     public int RankedRatingEarned { get; set; }
//
//     [JsonPropertyName("RankedRatingPerformanceBonus")]
//     public int RankedRatingPerformanceBonus { get; set; }
//
//     [JsonPropertyName("CompetitiveMovement")]
//     public string CompetitiveMovement { get; set; }
//
//     [JsonPropertyName("AFKPenalty")] public int AfkPenalty { get; set; }
// }

public class QueueSkills
{
    [JsonPropertyName("competitive")] public Competitive Competitive { get; set; }

    [JsonPropertyName("custom")] public Custom Custom { get; set; }

    [JsonPropertyName("deathmatch")] public Custom Deathmatch { get; set; }

    [JsonPropertyName("ggteam")] public Custom Ggteam { get; set; }

    [JsonPropertyName("newmap")] public Custom Newmap { get; set; }

    [JsonPropertyName("onefa")] public Custom Onefa { get; set; }

    [JsonPropertyName("seeding")] public Seeding Seeding { get; set; }

    [JsonPropertyName("snowball")] public Custom Snowball { get; set; }

    [JsonPropertyName("spikerush")] public Custom Spikerush { get; set; }

    [JsonPropertyName("unrated")] public Custom Unrated { get; set; }
}

public class Competitive
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

public class SeasonalInfoBySeasonId
{
    [JsonExtensionData] public Dictionary<string, JsonElement> Act { get; set; }
}

public class ActInfo
{
    [JsonPropertyName("SeasonID")] public string SeasonId { get; set; }

    [JsonPropertyName("NumberOfWins")] public int NumberOfWins { get; set; }

    [JsonPropertyName("NumberOfWinsWithPlacements")]
    public int NumberOfWinsWithPlacements { get; set; }

    [JsonPropertyName("NumberOfGames")] public int NumberOfGames { get; set; }

    [JsonPropertyName("Rank")] public int Rank { get; set; }

    [JsonPropertyName("CapstoneWins")] public int CapstoneWins { get; set; }

    [JsonPropertyName("LeaderboardRank")] public int LeaderboardRank { get; set; }

    [JsonPropertyName("CompetitiveTier")] public int CompetitiveTier { get; set; }

    [JsonPropertyName("RankedRating")] public int RankedRating { get; set; }

    [JsonPropertyName("WinsByTier")] public Dictionary<string, int> WinsByTier { get; set; }

    [JsonPropertyName("GamesNeededForRating")]
    public int GamesNeededForRating { get; set; }

    [JsonPropertyName("TotalWinsNeededForRank")]
    public int TotalWinsNeededForRank { get; set; }
}

public class Custom
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

public class Seeding
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

public class LeaderboardsResponse
{
    [JsonPropertyName("Deployment")] public string Deployment { get; set; }

    [JsonPropertyName("QueueID")] public string QueueId { get; set; }

    [JsonPropertyName("SeasonID")] public Guid SeasonId { get; set; }

    [JsonPropertyName("Players")] public object[] Players { get; set; }

    [JsonPropertyName("totalPlayers")] public int TotalPlayers { get; set; }

    [JsonPropertyName("immortalStartingPage")]
    public int ImmortalStartingPage { get; set; }

    [JsonPropertyName("immortalStartingIndex")]
    public int ImmortalStartingIndex { get; set; }

    [JsonPropertyName("topTierRRThreshold")]
    public int TopTierRrThreshold { get; set; }

    [JsonPropertyName("tierDetails")] public Dictionary<string, TierDetail> TierDetails { get; set; }

    [JsonPropertyName("startIndex")] public int StartIndex { get; set; }

    [JsonPropertyName("query")] public string Query { get; set; }
}

public class TierDetail
{
    [JsonPropertyName("rankedRatingThreshold")]
    public int RankedRatingThreshold { get; set; }

    [JsonPropertyName("startingPage")] public int StartingPage { get; set; }

    [JsonPropertyName("startingIndex")] public int StartingIndex { get; set; }
}

public class ContentResponse
{
    [JsonPropertyName("DisabledIDs")] public object[] DisabledIDs { get; set; }

    [JsonPropertyName("Seasons")] public Event[] Seasons { get; set; }

    [JsonPropertyName("Events")] public Event[] Events { get; set; }
}

public class Event
{
    [JsonPropertyName("ID")] public Guid Id { get; set; }

    [JsonPropertyName("Name")] public string Name { get; set; }

    [JsonPropertyName("StartTime")] public string StartTime { get; set; }

    [JsonPropertyName("EndTime")] public string EndTime { get; set; }

    [JsonPropertyName("IsActive")] public bool IsActive { get; set; }

    [JsonPropertyName("DevelopmentOnly")] public bool DevelopmentOnly { get; set; }

    [JsonPropertyName("Type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }
}

public class PresencesResponse
{
    [JsonPropertyName("presences")] public Presence[] Presences { get; set; }
}

public class Presence
{
    [JsonPropertyName("actor")] public string Actor { get; set; }

    [JsonPropertyName("basic")] public string Basic { get; set; }

    [JsonPropertyName("details")] public string Details { get; set; }

    [JsonPropertyName("game_name")] public string GameName { get; set; }

    [JsonPropertyName("game_tag")] public string GameTag { get; set; }

    [JsonPropertyName("location")] public string Location { get; set; }

    [JsonPropertyName("msg")] public string Msg { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("patchline")] public string Patchline { get; set; }

    [JsonPropertyName("pid")] public string Pid { get; set; }

    [JsonPropertyName("platform")] public string Platform { get; set; }

    [JsonPropertyName("private")] public string Private { get; set; }

    [JsonPropertyName("privateJwt")] public object PrivateJwt { get; set; }

    [JsonPropertyName("product")] public string Product { get; set; }

    [JsonPropertyName("puuid")] public Guid Puuid { get; set; }

    [JsonPropertyName("region")] public string Region { get; set; }

    [JsonPropertyName("resource")] public string Resource { get; set; }

    [JsonPropertyName("state")] public string State { get; set; }

    [JsonPropertyName("summary")] public string Summary { get; set; }

    [JsonPropertyName("time")]
    [JsonIgnore]
    public long Time { get; set; }
}

public class PresencesPrivate
{
    [JsonPropertyName("isValid")] public bool IsValid { get; set; }

    [JsonPropertyName("sessionLoopState")] public string SessionLoopState { get; set; }

    [JsonPropertyName("partyOwnerSessionLoopState")]
    public string PartyOwnerSessionLoopState { get; set; }

    [JsonPropertyName("customGameName")] public string CustomGameName { get; set; }

    [JsonPropertyName("customGameTeam")] public string CustomGameTeam { get; set; }

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

    [JsonPropertyName("provisioningFlow")] public string ProvisioningFlow { get; set; }

    [JsonPropertyName("matchMap")] public string MatchMap { get; set; }

    [JsonPropertyName("partyId")] public Guid PartyId { get; set; }

    [JsonPropertyName("isPartyOwner")] public bool IsPartyOwner { get; set; }

    [JsonPropertyName("partyState")] public string PartyState { get; set; }

    [JsonPropertyName("partyAccessibility")]
    public string PartyAccessibility { get; set; }

    [JsonPropertyName("maxPartySize")] public int MaxPartySize { get; set; }

    [JsonPropertyName("queueId")] public string QueueId { get; set; }

    [JsonPropertyName("partyLFM")] public bool PartyLfm { get; set; }

    [JsonPropertyName("partyClientVersion")]
    public string PartyClientVersion { get; set; }

    [JsonPropertyName("partySize")] public int PartySize { get; set; }

    [JsonPropertyName("tournamentId")] public string TournamentId { get; set; }

    [JsonPropertyName("rosterId")] public string RosterId { get; set; }

    [JsonPropertyName("partyVersion")] public long PartyVersion { get; set; }

    [JsonPropertyName("queueEntryTime")] public string QueueEntryTime { get; set; }

    [JsonPropertyName("playerCardId")] public Guid PlayerCardId { get; set; }

    [JsonPropertyName("playerTitleId")]
    [JsonIgnore]
    public Guid PlayerTitleId { get; set; }

    [JsonPropertyName("preferredLevelBorderId")]
    public string PreferredLevelBorderId { get; set; }

    [JsonPropertyName("accountLevel")] public int AccountLevel { get; set; }

    [JsonPropertyName("competitiveTier")] public int CompetitiveTier { get; set; }

    [JsonPropertyName("leaderboardPosition")]
    public int LeaderboardPosition { get; set; }

    [JsonPropertyName("isIdle")] public bool IsIdle { get; set; }
}

public class PartyIdResponse
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("Version")] public long Version { get; set; }

    [JsonPropertyName("CurrentPartyID")] public Guid CurrentPartyId { get; set; }

    [JsonPropertyName("Invites")] public object Invites { get; set; }

    [JsonPropertyName("Requests")] public object[] Requests { get; set; }

    [JsonPropertyName("PlatformInfo")] public PlatformInfo PlatformInfo { get; set; }
}

public class PlatformInfo
{
    [JsonPropertyName("platformType")] public string PlatformType { get; set; }

    [JsonPropertyName("platformOS")] public string PlatformOs { get; set; }

    [JsonPropertyName("platformOSVersion")]
    public string PlatformOsVersion { get; set; }

    [JsonPropertyName("platformChipset")] public string PlatformChipset { get; set; }
}

public class PartyResponse
{
    [JsonPropertyName("ID")] public Guid Id { get; set; }

    [JsonPropertyName("MUCName")] public string MucName { get; set; }

    [JsonPropertyName("VoiceRoomID")] public string VoiceRoomId { get; set; }

    [JsonPropertyName("Version")] public long Version { get; set; }

    [JsonPropertyName("ClientVersion")] public string ClientVersion { get; set; }

    [JsonPropertyName("Members")] public Member[] Members { get; set; }

    [JsonPropertyName("State")] public string State { get; set; }

    [JsonPropertyName("PreviousState")] public string PreviousState { get; set; }

    [JsonPropertyName("StateTransitionReason")]
    public string StateTransitionReason { get; set; }

    [JsonPropertyName("Accessibility")] public string Accessibility { get; set; }

    [JsonPropertyName("CustomGameData")] public CustomGameData CustomGameData { get; set; }

    [JsonPropertyName("MatchmakingData")] public MatchmakingData MatchmakingData { get; set; }

    [JsonPropertyName("Invites")] public object Invites { get; set; }

    [JsonPropertyName("Requests")] public object[] Requests { get; set; }

    [JsonPropertyName("QueueEntryTime")] public string QueueEntryTime { get; set; }

    [JsonPropertyName("ErrorNotification")]
    public ErrorNotification ErrorNotification { get; set; }

    [JsonPropertyName("RestrictedSeconds")]
    public long RestrictedSeconds { get; set; }

    [JsonPropertyName("EligibleQueues")] public string[] EligibleQueues { get; set; }

    [JsonPropertyName("QueueIneligibilities")]
    public object[] QueueIneligibilities { get; set; }

    [JsonPropertyName("CheatData")] public CheatData CheatData { get; set; }

    [JsonPropertyName("XPBonuses")] public object[] XpBonuses { get; set; }
}

public class CheatData
{
    [JsonPropertyName("GamePodOverride")] public string GamePodOverride { get; set; }

    [JsonPropertyName("ForcePostGameProcessing")]
    public bool ForcePostGameProcessing { get; set; }
}

public class CustomGameData
{
    [JsonPropertyName("Settings")] public Settings Settings { get; set; }

    [JsonPropertyName("Membership")] public Membership Membership { get; set; }

    [JsonPropertyName("MaxPartySize")] public long MaxPartySize { get; set; }

    [JsonPropertyName("AutobalanceEnabled")]
    public bool AutobalanceEnabled { get; set; }

    [JsonPropertyName("AutobalanceMinPlayers")]
    public long AutobalanceMinPlayers { get; set; }
}

public class Membership
{
    [JsonPropertyName("teamOne")] public TeamOne[] TeamOne { get; set; }

    [JsonPropertyName("teamTwo")] public object[] TeamTwo { get; set; }

    [JsonPropertyName("teamSpectate")] public object[] TeamSpectate { get; set; }

    [JsonPropertyName("teamOneCoaches")] public object[] TeamOneCoaches { get; set; }

    [JsonPropertyName("teamTwoCoaches")] public object[] TeamTwoCoaches { get; set; }
}

public class TeamOne
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }
}

public class Settings
{
    [JsonPropertyName("Map")] public string Map { get; set; }

    [JsonPropertyName("Mode")] public string Mode { get; set; }

    [JsonPropertyName("UseBots")] public bool UseBots { get; set; }

    [JsonPropertyName("GamePod")] public string GamePod { get; set; }

    [JsonPropertyName("GameRules")] public GameRules GameRules { get; set; }
}

public class GameRules
{
    [JsonPropertyName("AllowGameModifiers")]
    public string AllowGameModifiers { get; set; }
}

public class ErrorNotification
{
    [JsonPropertyName("ErrorType")] public string ErrorType { get; set; }

    [JsonPropertyName("ErroredPlayers")] public object ErroredPlayers { get; set; }
}

public class MatchmakingData
{
    [JsonPropertyName("QueueID")] public string QueueId { get; set; }

    [JsonPropertyName("PreferredGamePods")]
    public string[] PreferredGamePods { get; set; }

    [JsonPropertyName("SkillDisparityRRPenalty")]
    public long SkillDisparityRrPenalty { get; set; }
}

public class Member
{
    [JsonPropertyName("Subject")] public Guid Subject { get; set; }

    [JsonPropertyName("CompetitiveTier")] public long CompetitiveTier { get; set; }

    [JsonPropertyName("PlayerIdentity")] public PlayerIdentity PlayerIdentity { get; set; }

    [JsonPropertyName("SeasonalBadgeInfo")]
    public object SeasonalBadgeInfo { get; set; }

    [JsonPropertyName("IsOwner")] public bool IsOwner { get; set; }

    [JsonPropertyName("QueueEligibleRemainingAccountLevels")]
    public long QueueEligibleRemainingAccountLevels { get; set; }

    [JsonPropertyName("Pings")] public Ping[] Pings { get; set; }

    [JsonPropertyName("IsReady")] public bool IsReady { get; set; }

    [JsonPropertyName("IsModerator")] public bool IsModerator { get; set; }

    [JsonPropertyName("UseBroadcastHUD")] public bool UseBroadcastHud { get; set; }

    [JsonPropertyName("PlatformType")] public string PlatformType { get; set; }
}

public class Ping
{
    [JsonPropertyName("Ping")] public long PingPing { get; set; }

    [JsonPropertyName("GamePodID")] public string GamePodId { get; set; }
}