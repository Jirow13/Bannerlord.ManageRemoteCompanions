using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Dynamic;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;

namespace ManageRemoteCompanions
{
    internal class PatchInventoryDefaults
    {
        public static Dictionary<CharacterObject, Equipment[]> DefaultCharacterEquipments = new Dictionary<CharacterObject, Equipment[]>();

        public static void SetDefault(CharacterObject character)
        {
            DefaultCharacterEquipments[character] = new Equipment[2] { new Equipment(character.Equipment), new Equipment(character.FirstCivilianEquipment) };
        }

        public static void ResetCharacter(CharacterObject character)
        {
            if (DefaultCharacterEquipments.ContainsKey(character))
            {
                character.Equipment.FillFrom(DefaultCharacterEquipments[character][0]);
                character.FirstCivilianEquipment.FillFrom(DefaultCharacterEquipments[character][1]);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryLogic), "InitializeRosters")]
    internal class PatchInventoryInitRosters
    {

        static bool Prefix(InventoryLogic __instance, ref ItemRoster[] ____rosters, ItemRoster leftItemRoster, ItemRoster rightItemRoster, ref TroopRoster rightMemberRoster, CharacterObject initialCharacterOfRightRoster)
        {

            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.ApplyInventoryPatch && rightMemberRoster.Contains(Hero.MainHero.CharacterObject))
            {
                PatchInventoryDefaults.DefaultCharacterEquipments.Clear();


                foreach (Hero hero in Clan.PlayerClan.Heroes)
                {
                    if (hero.IsAlive && hero.IsActive && !hero.IsChild && hero != Hero.MainHero && !rightMemberRoster.Contains(hero.CharacterObject))
                    {
                        rightMemberRoster.AddToCounts(hero.CharacterObject, 1);
                        PatchInventoryDefaults.SetDefault(hero.CharacterObject);
                    }
                }

                ____rosters[0] = leftItemRoster;
                ____rosters[1] = rightItemRoster;
                typeof(InventoryLogic).GetField("<RightMemberRoster>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, rightMemberRoster);
                typeof(InventoryLogic).GetField("<InitialEquipmentCharacter>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, initialCharacterOfRightRoster);
                rightItemRoster.RemoveZeroCounts();
                typeof(InventoryLogic).GetMethod("SetCurrentStateAsInitial", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, null);
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(InventoryLogic), "ResetLogic")]
    internal class PatchInventoryReset
    {
        public static void Prefix(InventoryLogic __instance)
        {
            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.ApplyInventoryPatch)
                foreach (CharacterObject c in __instance.RightMemberRoster.Troops)
                    if (c.IsHero && !__instance.OwnerParty.MemberRoster.Contains(c))
                        PatchInventoryDefaults.ResetCharacter(c);
        }
    }
}
