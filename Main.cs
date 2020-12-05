﻿using HarmonyLib;
using System;
using System.Windows.Forms;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;

namespace ManageRemoteCompanions
{
    internal class Main : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                new Harmony("com.bannerlord.ManageRemoteCompanions").PatchAll();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Exception applying ManageRemoteCompanions Harmony patch:\n{0}", e.ToString()));
            }
        }
    }
}
