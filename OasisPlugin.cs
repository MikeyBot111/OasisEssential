using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using static UnityEngine.Random;

namespace OasisEssential
{
    public class OasisPlugin : RocketPlugin<OasisConfiguration>
    {
        public static string version = "1.0.0.3";
        public static OasisPlugin Instance;


        public int lastindex = 0;
        public DateTime? lastmessage = null;
        void FixedUpdate()
        {
            printMessage();
            checkForAirdrop();
            checkForVotes();
        }


        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "join_message", "[Oasis] {0} se připojil na server." },
                    { "leave_message", "[Oasis] {0} se odpojil ze server." },
                    {"gun_headshot","[Oasis] {0} dal headshot {1} pomocí {2}"},
                    {"gun","[Oasis] {0} zabil {1} pomocí {2}"},
                    {"food","[Oasis] {0} vyhladověl."},
                    {"arena",""},
                    {"shred","[Oasis] {0} byl rozdrcen."},
                    {"punch_headshot","[Oasis] {0} umlatil do hlavy {1}"},
                    {"punch","[Oasis] {0} umlatil {1}"},
                    {"bones","[Oasis] {0}"},
                    {"melee_headshot","[Oasis] {0} dal headshot {1} pomocí {2}"},
                    {"melee","[Oasis] {0} {0} zabil {1} pomocí {2}"},
                    {"water","[Oasis] {0} zemřel na dehydrataci."},
                    {"breath","[Oasis] {0} se udusil."},
                    {"zombie","[Oasis] {0} byl sežrán zombíkem."},
                    {"animal","[Oasis] {0} byl zabit zvířetem."},
                    {"grenade","[Oasis] {0} byl roztrhán granátem."},
                    {"vehicle","[Oasis] {0} zemřel v autě."},
                    {"suicide","[Oasis] {0} to skončil."},
                    {"burning","[Oasis] {0} shořel."},
                    {"landmine","[Oasis] {0} stoupl na minu."},
                    {"roadkill","[Oasis] {0} přejelo auto."},
                    {"bleeding","[Oasis] {0} vykrvácel."},
                    {"freezing","[Oasis] {0} umrzl."},
                    {"sentry","[Oasis] {0} byl zabit automatickou střílecí věží."},
                    {"charge","[Oasis] {0} bouchl spolu s c4."},
                    {"missile","[Oasis] {0} potkal raketu na cestě domů."},
                    {"splash","[Oasis] {0} byl zadupnut."},
                    {"acid","[Oasis] {0} byl rozpuštěn v kyselině."},
                    {"spark","[Oasis] {0} byl seškvařen na uhel."},
                    {"infection","[Oasis] {0} se proměnil na zombíka."},
                    {"spit","[Oasis] {0} zemřel."},
                    {"kill","[Oasis] Bohové rozhodli o osudu {0}."},
                    {"boulder","[Oasis] {0} dostal šutrem."},
                    {"boulder","[Oasis] {0} zemřel."},
                    {"null","[Oasis] {0} zemřel."},
                    {"airdrop_dropped","[Oasis] Letí airdrop!"},
                    {"cv_votestart","[Oasis] Hlasovaní pro změnu na {0} bylo zahájeno!"},
                    {"cv_votesuccess","[Oasis] Hlasovaní pro změnu na {0} bylo úspěšné hlasovalo {1}% hráčů!"},
                    {"cv_voteended","[Oasis] Hlasovaní pro změnu na {0} nebylo úspěšné hlasovalo {1}% hráčů!"},
                    {"cv_voteinfo","[Oasis] Hlasování o {0} je průběhu! Můžete hlasovat pomocí /cv {1}, už hlasovalo {2}% hráčů."},
                    {"cv_nopermission","[Oasis] Nemáš dostatek oprávnění na tento příkaz!"},
                    {"cv_notenabled","[Oasis] Tento příkaz není zapnutý!"},
                    {"cv_oncooldown","[Oasis] Tento příkaz ještě není aktivován zbývá {0} s!"},
                    {"cv_alreadyvoted","[Oasis] Už si hlasoval pro tento příkaz!"},
                    {"cv_playervote","[Oasis] Hlasoval si pro změnu nastavení!"},
                };
            }
        }

        protected override void Load()
        {
            OasisPlugin.Instance = this;

            Logger.Log("OasisEssential, Version " + OasisPlugin.version);
            Logger.Log("Checking errors... ");
            Logger.Log("PlayerJoinLeave...OK ");
            Logger.Log("DeathMessages...OK ");
            Logger.Log("CallVote...OK ");
            Logger.Log("No errors have been found!");

            U.Events.OnPlayerConnected += PlayerJoined;
            U.Events.OnPlayerDisconnected += PlayerDisconnect;
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= PlayerJoined;
            U.Events.OnPlayerDisconnected -= PlayerDisconnect;
            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        }

        private void PlayerDisconnect(UnturnedPlayer player)
        {
            if (Configuration.Instance.ShowLeaveMessage)
            {
                UnturnedChat.Say(Translate("leave_message", player.CharacterName), UnityEngine.Color.red);
            }
        }

        private void PlayerJoined(UnturnedPlayer player)
        {
            if (Configuration.Instance.ShowJoinMessage)
            {
                UnturnedChat.Say(Translate("join_message", player.CharacterName), UnityEngine.Color.green);
            }
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            if (!Configuration.Instance.ShowDeathMessages)
            {
                return;
            }
            UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(murderer);
            try {
                if (limb == ELimb.SKULL && cause.ToString() == "MELEE" || cause.ToString() == "GUN" || cause.ToString() == "PUNCH")
                {
                    if (cause.ToString() == "MELEE" || cause.ToString() == "GUN")
                    {
                        UnturnedChat.Say(Translate(cause.ToString().ToLower() + "_headshot", killer.DisplayName, player.DisplayName, UnturnedPlayer.FromCSteamID(murderer).Player.equipment.asset.itemName.ToString()), UnityEngine.Color.red);
                    } else
                    {
                        UnturnedChat.Say(Translate(cause.ToString().ToLower() + "_headshot", killer.DisplayName, player.DisplayName), UnityEngine.Color.red);
                    }
                }
                else
                {
                    if (cause.ToString() == "MELEE" || cause.ToString() == "GUN")
                    {
                        UnturnedChat.Say(Translate(cause.ToString().ToLower(), killer.DisplayName, player.DisplayName, UnturnedPlayer.FromCSteamID(murderer).Player.equipment.asset.itemName.ToString()), UnityEngine.Color.red);
                    }
                    else if (cause.ToString() == "PUNCH" || cause.ToString() == "ROADKILL")
                    {
                        UnturnedChat.Say(Translate(cause.ToString().ToLower(), killer.DisplayName, player.DisplayName), UnityEngine.Color.red);
                    }
                    else
                    {
                        UnturnedChat.Say(Translate(cause.ToString().ToLower(), player.DisplayName), UnityEngine.Color.red);
                    }
                }
            }
            catch
            {
                UnturnedChat.Say(Translate("null", player.DisplayName), UnityEngine.Color.red);
            }


        }

        private void printMessage()
        {
            try
            {
                if (State == Rocket.API.PluginState.Loaded && Configuration.Instance.Messages != null && (lastmessage == null || ((DateTime.Now - lastmessage.Value).TotalSeconds > Configuration.Instance.Interval)))
                {
                    if (lastindex > (Configuration.Instance.Messages.Length - 1)) lastindex = 0;
                    Message message = Configuration.Instance.Messages[lastindex];
                    UnturnedChat.Say(message.Text, UnturnedChat.GetColorFromName(message.Color, UnityEngine.Color.green));
                    Logger.Log(message.Text);
                    lastmessage = DateTime.Now;
                    lastindex++;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }


        private bool AlreadyDropped = false;

        private void checkForAirdrop()
        {
            if (LevelManager.hasAirdrop && !AlreadyDropped)
            {
                AlreadyDropped = true;
                UnturnedChat.Say(Translate("airdrop_dropped"), UnityEngine.Color.green);
            }
            if (!LevelManager.hasAirdrop)
            {
                AlreadyDropped = false;
            }
        }


        private void checkForVotes()
        {
            foreach (Vote vote in Configuration.Instance.Votes)
            {
                int currentPlayers = Provider.clients.Count;
                if (currentPlayers == 0)
                {
                    currentPlayers = 1;
                }
                float prc = (vote.Players.Count / currentPlayers) * 100;

                if (vote.StartedTime.AddSeconds(vote.Timer) < DateTime.Now && vote.Started || vote.Started && prc >= vote.RequiredPercent)
                {
                    vote.OnCooldown = true;
                    vote.OnCooldownTime = DateTime.Now.AddSeconds(vote.CooldownTime);
                    vote.StartedTime = DateTime.MinValue;
                    vote.Started = false;


                    if (prc >= vote.RequiredPercent)
                    {
                        UnturnedChat.Say(OasisPlugin.Instance.Translate("cv_votesuccess", vote.Name, Math.Round(prc, 2)), UnityEngine.Color.yellow);
                        R.Commands.Execute(new ConsolePlayer(), string.Join(" ", vote.Command));
                        vote.Players = new List<CSteamID>();
                        return;
                    }
                    UnturnedChat.Say(OasisPlugin.Instance.Translate("cv_voteended", vote.Name, Math.Round(prc, 2)), UnityEngine.Color.yellow);

                    vote.Players = new List<CSteamID>();
                }
                if (vote.OnCooldown)
                {
                    if (vote.OnCooldownTime < DateTime.Now)
                    {
                        vote.OnCooldown = false;
                        vote.OnCooldownTime = DateTime.MinValue;
                    }
                }
            }
        }
    }
}

