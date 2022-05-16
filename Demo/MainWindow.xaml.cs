using System;
using System.Collections.Generic;
using System.Windows;
using Demo.Objects;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Demo
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        [ObservableProperty] private MatchDetails _match = new();
        [ObservableProperty] private List<Player> _playerList = new();

        public MainWindow()
        {
            InitializeComponent();
            Match = new MatchDetails
            {
                GameMode = "🏅 Competitive", Map = "de_dust 3", MapImage = new Uri("https://media.valorant-api.com/maps/2c9d57ec-4431-9c5e-2939-8f9ef6dd5cba/listviewicon.png"), Server = "🌍 Antarctica 2"
            };
            PlayerList = new List<Player>
            {
                // 1
                new Player
                {
                    AccountLevel = 144,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Sova", Image = new Uri("pack://application:,,,/images/agentsimg/320b2a48-4d9b-a075-30f1-1f93a9b638fa.png")},
                    IgnData = new IgnData
                    {
                        Username = "STD Soneliem#dev",
                        TrackerEnabled = Visibility.Visible,
                        TrackerDisabled = Visibility.Collapsed
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 90,
                        PreviouspreviouspreviousGame = +20,
                        PreviouspreviousGame = -5,
                        PreviousGame = +25,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "Red",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#181E34",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/14.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/14.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/15.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/16.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/b9c9eb56-4cbd-04b7-06a8-329dc6f1e73a/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/a26e0d1d-4886-7d62-6b4f-1996e706463d/fullrender.png")
                    }
                },
                // 2
                new Player
                {
                    AccountLevel = 69,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Reyna", Image = new Uri("pack://application:,,,/images/agentsimg/a3bfb853-43b2-7238-a4f1-ad90e9e46bcc.png")},
                    IgnData = new IgnData
                    {
                        Username = "STD Laskin#SONEL",
                        TrackerEnabled = Visibility.Collapsed,
                        TrackerDisabled = Visibility.Visible
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 20,
                        PreviouspreviouspreviousGame = +17,
                        PreviouspreviousGame = -12,
                        PreviousGame = 115,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#ff4654"
                    },
                    PartyColour = "Red",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/17.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/18.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/21.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/20.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/b9c9eb56-4cbd-04b7-06a8-329dc6f1e73a/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/6153bc01-4807-c705-e576-63beb9c8e930/fullrender.png")
                    }
                },
                // 3
                new Player
                {
                    AccountLevel = 72,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Jett", Image = new Uri("pack://application:,,,/images/agentsimg/add6443a-41bd-e414-f6ad-e58d267f4e95.png")},
                    IgnData = new IgnData
                    {
                        Username = "STD Ladams#yabot",
                        TrackerEnabled = Visibility.Collapsed,
                        TrackerDisabled = Visibility.Visible
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 50,
                        PreviouspreviouspreviousGame = +20,
                        PreviouspreviousGame = +2,
                        PreviousGame = +18,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#32e2b2",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "Red",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/13.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/13.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/14.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/15.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/c805c92a-4424-69f2-a0f2-8c8c9bb33a4a/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/b2619c1c-4974-4f06-f37b-c68b1d6d7bd1/fullrender.png")
                    }
                },
                // 4
                new Player
                {
                    AccountLevel = 210,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Viper", Image = new Uri("pack://application:,,,/images/agentsimg/707eab51-4836-f488-046a-cda6bf494859.png")},
                    IgnData = new IgnData
                    {
                        Username = "STD PantryWizard#69420",
                        TrackerEnabled = Visibility.Visible,
                        TrackerDisabled = Visibility.Collapsed
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 10,
                        PreviouspreviouspreviousGame = -12,
                        PreviouspreviousGame = -20,
                        PreviousGame = -2,
                        PreviouspreviouspreviousGameColour = "#ff4654",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#ff4654"
                    },
                    PartyColour = "Red",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/14.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/13.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/12.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/11.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/e9014a77-4a74-4ea7-999c-44b0d0f84daa/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/05dc58f4-4170-c088-ba23-fd9a2ddcfa9d/fullrender.png")
                    }
                },
                // 5
                new Player
                {
                    AccountLevel = 89,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Astra", Image = new Uri("pack://application:,,,/images/agentsimg/41fb69c1-4189-7b37-f117-bcaf1e96f1bf.png")},
                    IgnData = new IgnData
                    {
                        Username = "STD SpicyCurry#GAE",
                        TrackerEnabled = Visibility.Visible,
                        TrackerDisabled = Visibility.Collapsed
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 70,
                        PreviouspreviouspreviousGame = 0,
                        PreviouspreviousGame = +17,
                        PreviousGame = +12,
                        PreviouspreviouspreviousGameColour = "#7f7f7f",
                        PreviouspreviousGameColour = "#32e2b2",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "Red",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/10.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/11.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/12.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/13.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/32dfe871-4906-d2ce-4835-2d99aaa52f84/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/689e54c0-4089-2d91-de26-61aa4286d6cf/fullrender.png")
                    }
                },
                // 6
                new Player
                {
                    AccountLevel = 95,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Omen", Image = new Uri("pack://application:,,,/images/agentsimg/8e253930-4c05-31dd-1b6c-968525494517.png")},
                    IgnData = new IgnData
                    {
                        Username = "NXT Tenz#fr",
                        TrackerEnabled = Visibility.Collapsed,
                        TrackerDisabled = Visibility.Visible
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 5,
                        PreviouspreviouspreviousGame = +26,
                        PreviouspreviousGame = -2,
                        PreviousGame = +18,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "#32e2b2",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/17.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/18.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/19.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/19.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/7e10eabf-476b-0bcb-5847-e8958d6f1132/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/86f3352f-49b1-603f-6752-60bdfcddf318/fullrender.png")
                    }
                },
                // 7
                new Player
                {
                    AccountLevel = 87,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Cypher", Image = new Uri("pack://application:,,,/images/agentsimg/117ed9e3-49f3-6512-3ccf-0cada7e3823b.png")},
                    IgnData = new IgnData
                    {
                        Username = "BrUnKx#ty",
                        TrackerEnabled = Visibility.Visible,
                        TrackerDisabled = Visibility.Collapsed
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 50,
                        PreviouspreviouspreviousGame = +20,
                        PreviouspreviousGame = -10,
                        PreviousGame = +25,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "#32e2b2",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/15.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/0.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/19.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/21.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/171f9591-4efd-9fde-d555-d3b570f64d14/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/a26e0d1d-4886-7d62-6b4f-1996e706463d/fullrender.png")
                    }
                },
                // 8
                new Player
                {
                    AccountLevel = 52,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Sage", Image = new Uri("pack://application:,,,/images/agentsimg/569fdd95-4d10-43ab-ca70-79becc718b46.png")},
                    IgnData = new IgnData
                    {
                        Username = "ToxinGamer#1111",
                        TrackerEnabled = Visibility.Collapsed,
                        TrackerDisabled = Visibility.Visible
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 70,
                        PreviouspreviouspreviousGame = +20,
                        PreviouspreviousGame = -10,
                        PreviousGame = +25,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "DarkOrange",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/15.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/0.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/15.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/15.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/171f9591-4efd-9fde-d555-d3b570f64d14/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/a26e0d1d-4886-7d62-6b4f-1996e706463d/fullrender.png")
                    }
                },
                // 9
                new Player
                {
                    AccountLevel = 3,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Reyna", Image = new Uri("pack://application:,,,/images/agentsimg/a3bfb853-43b2-7238-a4f1-ad90e9e46bcc.png")},
                    IgnData = new IgnData
                    {
                        Username = "DefNot#smurf",
                        TrackerEnabled = Visibility.Collapsed,
                        TrackerDisabled = Visibility.Visible
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 30,
                        PreviouspreviouspreviousGame = +18,
                        PreviouspreviousGame = -26,
                        PreviousGame = +25,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#32e2b2",
                        PreviousGameColour = "#32e2b2"
                    },
                    PartyColour = "DarkOrange",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/0.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/0.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/0.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/7.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("pack://application:,,,/images/assets/phantom.png"),
                        VandalImage = new Uri("pack://application:,,,/images/assets/vandal.png")
                    }
                },
                // 10
                new Player
                {
                    AccountLevel = 79,
                    Active = Visibility.Visible,
                    AgentData = new AgentData {Name = "Raze", Image = new Uri("pack://application:,,,/images/agentsimg/f94c3b30-42be-e959-889c-5aa313dba261.png")},
                    IgnData = new IgnData
                    {
                        Username = "Potato#1212",
                        TrackerEnabled = Visibility.Collapsed,
                        TrackerDisabled = Visibility.Visible
                    },
                    MatchHistoryData = new MatchHistoryData
                    {
                        RankProgress = 10,
                        PreviouspreviouspreviousGame = +8,
                        PreviouspreviousGame = -25,
                        PreviousGame = -18,
                        PreviouspreviouspreviousGameColour = "#32e2b2",
                        PreviouspreviousGameColour = "#ff4654",
                        PreviousGameColour = "#ff4654"
                    },
                    PartyColour = "DarkOrange",
                    PlayerUiData = new PlayerUIData
                    {
                        BackgroundColour = "#252A40",
                    },
                    RankData = new RankData
                    {
                        MaxRr = 100,
                        PreviouspreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/13.png"),
                        PreviouspreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/13.png"),
                        PreviousrankImage = new Uri("pack://application:,,,/images/ranksimg/13.png"),
                        RankImage = new Uri("pack://application:,,,/images/ranksimg/14.png")
                    },
                    SkinData = new SkinData
                    {
                        PhantomImage = new Uri("https://media.valorant-api.com/weaponskinchromas/b9c9eb56-4cbd-04b7-06a8-329dc6f1e73a/fullrender.png"),
                        VandalImage = new Uri("https://media.valorant-api.com/weaponskinchromas/742740d0-4e50-57e1-af32-f991c7c640f8/fullrender.png")
                    }
                }
            };
            this.DataContext = this;
        }

        
    }
}