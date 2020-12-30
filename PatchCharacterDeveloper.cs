﻿using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;

namespace ManageRemoteCompanions
{
    [HarmonyPatch(typeof(SkillVM), "IsPerkAvailable")]
    internal class PatchIsPerkAvailable
    {
        public static void Postfix(SkillVM __instance, PerkObject perk, ref bool __result)
        {
            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.CharacterUpgrades)
                __result = perk.RequiredSkillValue <= __instance.Level;
        }
    }

    /* Looks like this is no longer needed as of 1.5.6, as TW is allowing remote party skill mgmt?
    [HarmonyPatch(typeof(CampaignUIHelper), "GetAddFocusHintString")]
    internal class PatchRefreshCanAddFocus
    {
        public static void Prefix(ref bool isInSamePartyAsPlayer)
        {
            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.CharacterUpgrades)
                isInSamePartyAsPlayer = true;
        }
    }
    */

    [HarmonyPatch(typeof(CharacterVM), "CanAddFocusToSkillWithFocusAmount")]
    internal class PatchSkillFocus
    {
        public static void Postfix(CharacterVM __instance, int currentFocusAmount, ref bool __result)
        {
            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.CharacterUpgrades)
                __result = currentFocusAmount < 5 && __instance.UnspentCharacterPoints > 0;
        }
    }

    [HarmonyPatch(typeof(CharacterAttributeItemVM), "RefreshWithCurrentValues")]
    internal class PatchAttributePoints
    {
        public static void Postfix(CharacterAttributeItemVM __instance)
        {
            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.CharacterUpgrades)
                __instance.CanAddPoint = __instance.AttributeValue < 10 && __instance.UnspentAttributePoints > 0;
        }
    }
}
