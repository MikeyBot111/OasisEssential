using Rocket.API;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine;
using Steamworks;
using System.Collections.Generic;
using System;
using SDG.Unturned;

namespace OasisEssential
{
    public class OasisConfiguration : IRocketPluginConfiguration
    {
        public bool ShowJoinMessage, ShowLeaveMessage, ShowDeathMessages;

        public int Interval;

        [XmlArrayItem("Message")]
        [XmlArray(ElementName = "Messages")]
        public Message[] Messages;

        [XmlArrayItem("Voting")]
        [XmlArray(ElementName = "Votes")]
        public Vote[] Votes;


        public void LoadDefaults()
        {
            this.ShowJoinMessage = false;
            this.ShowLeaveMessage = false;
            this.ShowDeathMessages = false;

            Interval = 300;

            Messages = new Message[]{
                new Message("[Oasis] Vote for our server using /vote and get rewards using /reward!","Cyan"),
                new Message("[Oasis] Read the rules using /rules, so you never break them!","Cyan"),
            };

            Votes = new Vote[]{
                new Vote("Den", "d", "day", "[Oasis] /cv d - Start or vote for setting the day!"),
                new Vote("Noc", "n", "night", "[Oasis] /cv n - Start or vote for setting the night!"),
                new Vote("Noc", "n", "weather storm", "[Oasis] /cv n - Start or vote for setting the rain!"),
                new Vote("Noc", "n", "weather none", "[Oasis] /cv n - Start or vote for setting the end of rain!"),
            };
        }
    }

    public sealed class Message
    {
        [XmlAttribute("Text")]
        public string Text;

        [XmlAttribute("Color")]
        public string Color;

        public Message(string text, string color)
        {
            Text = text;
            Color = color;
        }
        public Message()
        {
            Text = "";
            Color = "";
        }
    }

    public sealed class Vote
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Alias")]
        public string Alias { get; set; }

        [XmlAttribute("Command")]
        public string Command { get; set; }

        [XmlAttribute("Help")]
        public string Help { get; set; }

        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        [XmlAttribute("MinimumPlayers")]
        public int MinimumPlayers { get; set; }

        [XmlAttribute("RequiredPercent")]
        public int RequiredPercent { get; set; }

        [XmlAttribute("Timer")]
        public int Timer { get; set; }

        [XmlAttribute("CooldownTime")]
        public int CooldownTime { get; set; }

            
        public bool OnCooldown { get; set; }
        public bool Started { get; set; }
        public List<CSteamID> Players { get; set; }
        public DateTime StartedTime { get; set; }
        public DateTime OnCooldownTime { get; set; }

        public Vote(string name, string alias, string command, string help, bool enabled = true, int minPlayers = 1, int reqPercent = 50, int timer = 60, int cooldown = 300)
        {
            Name = name;
            Alias = alias;
            Command = command;
            Help = help;
            Enabled = enabled;
            MinimumPlayers = minPlayers;
            RequiredPercent = reqPercent;
            Timer = timer;
            CooldownTime = cooldown;

            OnCooldown = false;
            Started = false;
            Players = new List<CSteamID>();
            StartedTime = DateTime.MinValue;
            OnCooldownTime = DateTime.MinValue;
        }

        public Vote()
        {
            Name = "";
            Alias = "";
            Command = "";
            Help = "";
            Enabled = false;
            MinimumPlayers = 0;
            RequiredPercent = 0;
            Timer = 60;
            CooldownTime = 300;

            OnCooldown = false;
            Started = false;
            Players = new List<CSteamID>();
            StartedTime = DateTime.MinValue;
            OnCooldownTime = DateTime.MinValue;
        }

    }
}
