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
using TaleWorlds.Localization;

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

    /* This Isn't Working Right */
    [HarmonyPatch(typeof(InventoryLogic), "InitializeRosters")]
    internal class PatchInventoryInitRosters
    {

        static bool Prefix(InventoryLogic __instance, ref ItemRoster[] ____rosters, ItemRoster leftItemRoster, ItemRoster rightItemRoster, ref TroopRoster rightMemberRoster, CharacterObject initialCharacterOfRightRoster)
        {
            //typeof(InventoryLogic).GetMethod("InitializeRosters", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { leftItemRoster, rightItemRoster, rightMemberRoster, initialCharacterOfRightRoster });//pass whatever you want into this method... you don't need to use the original parameter
            
            if (Settings.Instance is { } settings && settings.Enabled && Settings.Instance.ApplyInventoryPatch && rightMemberRoster.Contains(Hero.MainHero.CharacterObject))
            {
                TroopRoster newRoster = TroopRoster.CreateDummyTroopRoster();
                newRoster.Add(rightMemberRoster);
                PatchInventoryDefaults.DefaultCharacterEquipments.Clear();


                foreach (Hero hero in Clan.PlayerClan.Heroes)
                {
                    if (hero.IsAlive && hero.IsActive && !hero.IsChild && hero != Hero.MainHero && !newRoster.Contains(hero.CharacterObject))
                    {
                        newRoster.AddToCounts(hero.CharacterObject, 1);
                        PatchInventoryDefaults.SetDefault(hero.CharacterObject);
                    }
                }

                foreach (Hero hero in Clan.PlayerClan.Companions)
                {
                    if (hero.IsAlive && hero.IsPlayerCompanion && !newRoster.Contains(hero.CharacterObject))
                    {
                        newRoster.AddToCounts(hero.CharacterObject, 1);
                        PatchInventoryDefaults.SetDefault(hero.CharacterObject);
                    }
                }
                rightMemberRoster = newRoster;
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

    /*
    [HarmonyPatch(typeof(SPInventoryVM), "SPInventoryVM")]
    public class SPInventoryVMPatch
    {
        static void Postfix(SPInventoryVM __instance, SelectorVM<SelectorItemVM> ____characterList )
        {
             //_charList = null;
            //typeof(SelectorVM<SelectorItemVM>).GetField("<_characterList>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            //__instance._characterList = new SelectorVM<SelectorItemVM>(0, new Action<SelectorVM<SelectorItemVM>>(__instance.OnCharacterSelected));

            foreach (Hero hero in Clan.PlayerClan.Heroes)
            {
                var isAlreadyOnList = ____characterList.ItemList.Where(e => e.StringItem == hero.Name.ToString());
                if (hero.IsAlive && hero.IsActive && !hero.IsChild && hero != Hero.MainHero && isAlreadyOnList.ToList().Count > 0 )
                {
                    ____characterList.AddItem(new SelectorItemVM(hero.Name));
                }
            }

            /*
            foreach (Hero hero in Clan.PlayerClan.Companions)
            {
                if (hero.IsAlive && hero.IsPlayerCompanion && !newRoster.Contains(hero.CharacterObject))
                {
                    newRoster.AddToCounts(hero.CharacterObject, 1);
                    PatchInventoryDefaults.SetDefault(hero.CharacterObject);
                }
            }
            */
            //typeof(SelectorVM<SelectorItemVM>).GetField("<_characterList>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, _charList);
            /*
            return;
        }

        static bool Prepare()
        {
            return true;
        }
    }
    */

    [HarmonyPatch(typeof(SPInventoryVM), "CharacterList", MethodType.Getter)]
    internal class SPInventoryVMPatch
    {
        static void Postfix(SelectorVM<SelectorItemVM> ____characterList)
        {
            
            foreach (Hero hero in Clan.PlayerClan.Heroes)
            {
                bool isAlreadyOnList = ____characterList.ItemList.Where(e => e.StringItem == hero.Name.ToString()).Any();
                if (hero.IsAlive && hero.IsActive && !hero.IsChild && hero != Hero.MainHero && isAlreadyOnList == false)
                {
                    ____characterList.AddItem(new SelectorItemVM(hero.Name));
                }
            }

            return;
        }

        static bool Prepare()
        {
            return true;
        }
    }

}
