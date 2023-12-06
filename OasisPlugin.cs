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
        public static string version = "1.0.0.4";
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
                    { "join_message": "[Oasis] {0} has joined the server." },
                    { "leave_message": "[Oasis] {0} has left the server." },
                    { "gun_headshot": "[Oasis] {0} headshot {1} with {2}" },
                    { "gun": "[Oasis] {0} killed {1} with {2}" },
                    { "food": "[Oasis] {0} has starved." },
                    { "arena": "" },
                    { "shred": "[Oasis] {0} was shredded." },
                    { "punch_headshot": "[Oasis] {0} headshot {1}" },
                    { "punch": "[Oasis] {0} punched {1}" },
                    { "bones": "[Oasis] {0}" },
                    { "melee_headshot": "[Oasis] {0} headshot {1} with {2}" },
                    { "melee": "[Oasis] {0} killed {1} with {2}" },
                    { "water": "[Oasis] {0} died of dehydration." },
                    { "breath": "[Oasis] {0} suffocated." },
                    { "zombie": "[Oasis] {0} was eaten by a zombie." },
                    { "animal": "[Oasis] {0} was killed by an animal." },
                    { "grenade": "[Oasis] {0} was blown up by a grenade." },
                    { "vehicle": "[Oasis] {0} died in a car." },
                    { "suicide": "[Oasis] {0} ended it." },
                    { "burning": "[Oasis] {0} burned." },
                    { "landmine": "[Oasis] {0} stepped on a mine." },
                    { "roadkill": "[Oasis] {0} was run over by a car." },
                    { "bleeding": "[Oasis] {0} bled out." },
                    { "freezing": "[Oasis] {0} froze to death." },
                    { "sentry": "[Oasis] {0} was killed by an automatic turret." },
                    { "charge": "[Oasis] {0} exploded with C4." },
                    { "missile": "[Oasis] {0} met a missile on the way home." },
                    { "splash": "[Oasis] {0} was stomped." },
                    { "acid": "[Oasis] {0} was dissolved in acid." },
                    { "spark": "[Oasis] {0} was charred to a crisp." },
                    { "infection": "[Oasis] {0} turned into a zombie." },
                    { "spit": "[Oasis] {0} died." },
                    { "kill": "[Oasis] The gods have decided the fate of {0}." },
                    { "boulder": "[Oasis] {0} got hit by a boulder." },
                    { "null": "[Oasis] {0} died." },
                    { "airdrop_dropped": "[Oasis] Airdrop incoming!" },
                    { "cv_votestart": "[Oasis] Vote for change to {0} has started!" },
                    { "cv_votesuccess": "[Oasis] Vote for change to {0} was successful, {1}% of players voted!" },
                    { "cv_voteended": "[Oasis] Vote for change to {0} was unsuccessful, {1}% of players voted!" },
                    { "cv_voteinfo": "[Oasis] Voting for {0} is in progress! You can vote using /cv {1}, {2}% of players have already voted." },
                    { "cv_nopermission": "[Oasis] You do not have enough permission for this command!" },
                    { "cv_notenabled": "[Oasis] This command is not enabled!" }
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

