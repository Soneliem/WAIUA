using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WAIUA.Objects;

public class Urls
{
    public string Name { get; init; }
    public string Filepath { get; init; }
    public string Url { get; init; }
}

public class VapiVersionResponse
{
    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("data")] public VapiVersion Data { get; set; }
}

public class VapiVersion
{
    [JsonPropertyName("manifestId")] public string ManifestId { get; set; }

    [JsonPropertyName("branch")] public string Branch { get; set; }

    [JsonPropertyName("version")] public string Version { get; set; }

    [JsonPropertyName("buildVersion")] public long BuildVersion { get; set; }

    [JsonPropertyName("riotClientVersion")]
    public string RiotClientVersion { get; set; }

    [JsonPropertyName("buildDate")] public string BuildDate { get; set; }
}

public class ValApiMapsResponse
{
    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("data")] public VapiMaps[] Data { get; set; }
}

public class VapiMaps
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("coordinates")] public string Coordinates { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("listViewIcon")] public Uri ListViewIcon { get; set; }

    [JsonPropertyName("splash")] public Uri Splash { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }

    [JsonPropertyName("mapUrl")] public string MapUrl { get; set; }

    [JsonPropertyName("xMultiplier")] public double XMultiplier { get; set; }

    [JsonPropertyName("yMultiplier")] public double YMultiplier { get; set; }

    [JsonPropertyName("xScalarToAdd")] public double XScalarToAdd { get; set; }

    [JsonPropertyName("yScalarToAdd")] public double YScalarToAdd { get; set; }

    [JsonExtensionData] public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class ValApiAgentsResponse
{
    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("data")] public ValApiAgents[] Data { get; set; }
}

public class ValApiAgents
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("developerName")] public string DeveloperName { get; set; }

    [JsonPropertyName("characterTags")] public string[] CharacterTags { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("displayIconSmall")] public Uri DisplayIconSmall { get; set; }

    [JsonPropertyName("bustPortrait")] public Uri BustPortrait { get; set; }

    [JsonPropertyName("fullPortrait")] public Uri FullPortrait { get; set; }

    [JsonPropertyName("killfeedPortrait")] public Uri KillfeedPortrait { get; set; }

    [JsonPropertyName("background")] public Uri Background { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }

    [JsonPropertyName("isFullPortraitRightFacing")]
    public bool IsFullPortraitRightFacing { get; set; }

    [JsonPropertyName("isPlayableCharacter")]
    public bool IsPlayableCharacter { get; set; }

    [JsonPropertyName("isAvailableForTest")]
    public bool IsAvailableForTest { get; set; }

    [JsonPropertyName("isBaseContent")] public bool IsBaseContent { get; set; }

    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class ValApiSkinsResponse
{
    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("data")] public ValApiSkins[] Data { get; set; }
}

public class ValApiCardsResponse
{
    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("data")] public ValApiCards[] Data { get; set; }
}

public class ValApiSkins
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("fullRender")] public Uri FullRender { get; set; }

    [JsonPropertyName("swatch")] public Uri Swatch { get; set; }

    [JsonPropertyName("streamedVideo")] public Uri StreamedVideo { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }

    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class ValApiCards
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("isHiddenIfNotOwned")]
    public bool IsHiddenIfNotOwned { get; set; }

    [JsonPropertyName("themeUuid")] public Guid? ThemeUuid { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("smallArt")] public Uri SmallArt { get; set; }

    [JsonPropertyName("wideArt")] public Uri WideArt { get; set; }

    [JsonPropertyName("largeArt")] public Uri LargeArt { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }
}

public class ValApiSpraysResponse
{
    [JsonPropertyName("status")] public long Status { get; set; }

    [JsonPropertyName("data")] public Datum[] Data { get; set; }
}

public class Datum
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("category")]
    [JsonIgnore]
    public string? Category { get; set; }

    [JsonPropertyName("themeUuid")] public Guid? ThemeUuid { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("fullIcon")] public Uri FullIcon { get; set; }

    [JsonPropertyName("fullTransparentIcon")]
    public Uri FullTransparentIcon { get; set; }

    [JsonPropertyName("animationPng")] public Uri AnimationPng { get; set; }

    [JsonPropertyName("animationGif")] public Uri AnimationGif { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }

    [JsonPropertyName("levels")] public Level[] Levels { get; set; }
}

public class Level
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("sprayLevel")] public long SprayLevel { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }
}

public class ValApiRanksResponse
{
    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("data")] public ValApiRanks[] Data { get; set; }
}

public class ValApiRanks
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("assetObjectName")] public string AssetObjectName { get; set; }

    [JsonPropertyName("tiers")] public Tier[] Tiers { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }

    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class Tier
{
    [JsonPropertyName("tier")] public int TierTier { get; set; }

    [JsonPropertyName("tierName")] public string TierName { get; set; }

    [JsonPropertyName("division")] public string Division { get; set; }

    [JsonPropertyName("divisionName")] public string DivisionName { get; set; }

    [JsonPropertyName("smallIcon")] public Uri SmallIcon { get; set; }

    [JsonPropertyName("largeIcon")] public Uri LargeIcon { get; set; }

    [JsonPropertyName("rankTriangleDownIcon")]
    public Uri RankTriangleDownIcon { get; set; }

    [JsonPropertyName("rankTriangleUpIcon")]
    public Uri RankTriangleUpIcon { get; set; }

    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

public class ValApiGamemodeResponse
{
    [JsonPropertyName("status")] public long Status { get; set; }

    [JsonPropertyName("data")] public ValApiGamemode[] Data { get; set; }
}

public class ValApiGamemode
{
    [JsonPropertyName("uuid")] public Guid Uuid { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("duration")] public string Duration { get; set; }

    [JsonPropertyName("allowsMatchTimeouts")]
    public bool AllowsMatchTimeouts { get; set; }

    [JsonPropertyName("isTeamVoiceAllowed")]
    public bool IsTeamVoiceAllowed { get; set; }

    [JsonPropertyName("isMinimapHidden")] public bool IsMinimapHidden { get; set; }

    [JsonPropertyName("orbCount")] public long OrbCount { get; set; }

    [JsonPropertyName("teamRoles")] public string[] TeamRoles { get; set; }

    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    [JsonPropertyName("displayIcon")] public Uri DisplayIcon { get; set; }

    [JsonPropertyName("assetPath")] public string AssetPath { get; set; }
}