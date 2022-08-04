using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RestSharp;
using Serilog.Core;

namespace WAIUA.Helpers;

public static class Constants
{
    public static ConcurrentDictionary<string, RestResponse> UrlToBody = new();

    public static readonly List<Guid> BeforeAscendantSeasons = new()
    {
        new Guid("0df5adb9-4dcb-6899-1306-3e9860661dd3"),
        new Guid("3f61c772-4560-cd3f-5d3f-a7ab5abda6b3"),
        new Guid("0530b9c4-4980-f2ee-df5d-09864cd00542"),
        new Guid("46ea6166-4573-1128-9cea-60a15640059b"),
        new Guid("fcf2c8f4-4324-e50b-2e23-718e4a3ab046"),
        new Guid("97b6e739-44cc-ffa7-49ad-398ba502ceb0"),
        new Guid("ab57ef51-4e59-da91-cc8d-51a5a2b9b8ff"),
        new Guid("52e9749a-429b-7060-99fe-4595426a0cf7"),
        new Guid("71c81c67-4fae-ceb1-844c-aab2bb8710fa"),
        new Guid("2a27e5d2-4d30-c9e2-b15a-93b8909a442c"),
        new Guid("4cb622e1-4244-6da3-7276-8daaf1c01be2"),
        new Guid("a16955a5-4ad0-f761-5e9e-389df1c892fb"),
        new Guid("97b39124-46ce-8b55-8fd1-7cbf7ffe173f"),
        new Guid("573f53ac-41a5-3a7d-d9ce-d6a6298e5704"),
        new Guid("d929bc38-4ab6-7da4-94f0-ee84f8ac141e"),
        new Guid("3e47230a-463c-a301-eb7d-67bb60357d4f"),
        new Guid("808202d6-4f2b-a8ff-1feb-b3a0590ad79f")
    };

    public static string AccessToken { get; set; }
    public static string EntitlementToken { get; set; }
    public static string Region { get; set; }
    public static string Shard { get; set; }
    public static string Version { get; set; }
    public static string LocalAppDataPath { get; set; }
    public static Guid Ppuuid { get; set; }
    public static Guid PPartyId { get; set; }
    public static Guid MatchId { get; set; }
    public static string Port { get; set; }
    public static string LPassword { get; set; }

    public static Logger Log { get; set; }
    // public static RestClient RestClient { get; set; }
}