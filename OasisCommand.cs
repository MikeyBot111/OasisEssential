using Rocket.API;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using Steamworks;
using SDG.Unturned;

namespace OasisEssential
{

    public class OasisCommandCV : IRocketCommand
    {
        #region Declarations
        public List<string> Permissions => new List<String>() { "OasisEssential.cv" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "cv";
        public string Syntax => "cv (příkaz)";
        public string Help => "Zahají globalní hlasování.";
        public List<string> Aliases => new List<string>();
       

        #endregion
        public void Execute(IRocketPlayer caller, string[] command)
        {
            #region Help
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                foreach (Vote vote in OasisPlugin.Instance.Configuration.Instance.Votes)
                {
                    UnturnedChat.Say(caller, vote.Help, Color.yellow);
                }
                return;
            }
            #endregion

            #region Commands
            foreach (Vote vote in OasisPlugin.Instance.Configuration.Instance.Votes)
            {
                if (vote.Command == command[0].ToString().ToLower() || vote.Alias == command[0].ToString().ToLower())
                {
                    if (!player.HasPermission("cv." + vote.Command) && !player.IsAdmin)
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cv_nopermission"), Color.red);
                        return;
                    }

                    if (!vote.Enabled)
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cv_notenabled"), Color.red);
                        return;
                    }

                    if (vote.OnCooldown)
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cv_oncooldown", Math.Round((vote.OnCooldownTime - DateTime.Now).TotalSeconds), 2), Color.red);
                        return;
                    }

                    if (vote.Players.Contains(player.CSteamID))
                    {
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cv_alreadyvoted"), Color.red);
                        return;
                    }

                    float prc = vote.Players.Count / Provider.clients.Count;

                    if (!vote.Started)
                    {
                        vote.Started = true;
                        vote.StartedTime = DateTime.Now;
                        vote.Players.Add(player.CSteamID);

                        prc = vote.Players.Count / Provider.clients.Count;

                        UnturnedChat.Say(OasisPlugin.Instance.Translate("cv_votestart", vote.Name), Color.yellow);
                        UnturnedChat.Say(OasisPlugin.Instance.Translate("cv_voteinfo", vote.Name, vote.Alias, Math.Round(prc, 2) * 100), Color.yellow);
                        UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cv_playervote"), Color.green);
                        return;
                    }

                    vote.Players.Add(player.CSteamID);

                    prc = vote.Players.Count / Provider.clients.Count;

                    UnturnedChat.Say(OasisPlugin.Instance.Translate("cv_voteinfo", vote.Name, vote.Alias, Math.Round(prc, 2) * 100), Color.yellow);
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cv_playervote"), Color.green);
                }
            }
            #endregion
        }
    }
}