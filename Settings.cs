using MCM.Abstractions.Settings.Base.Global;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;


namespace ManageRemoteCompanions
{
    public class Settings : AttributeGlobalSettings<Settings>
    {
        public override string DisplayName => "Manage Remote Companions";
        public override string FolderName => "ManageRemoteCompanions";

        // Only update Id version when making changes to settings options that are not backwards compatible.
        public override string Id { get; } = "ManageRemoteCompanions";
        public override string FormatType => "json";

        [SettingPropertyBool("Enabled", Order = 1, RequireRestart = false, HintText = "Toggle this mod on or off.")]
        [SettingPropertyGroup("Enabled")]
        public bool Enabled { get; set; } = true;

        [SettingPropertyBool("Charcter Upgrades", Order = 2, RequireRestart = false, HintText = "Allows allocating perks, attribute points and focus points for remote companions.")]
        [SettingPropertyGroup("Enabled")]
        public bool CharacterUpgrades { get; set; } = true;

        [SettingPropertyBool("Manage Equipment", Order = 3, HintText = "Adds all remote companions and adult family members to the Inventory screen so their equipment can be managed.")]
        [SettingPropertyGroup("Enabled")]
        public bool ApplyInventoryPatch { get; set; } = true;

        [SettingPropertyBool("Enable Manage Troops (ALT + Click)", Order = 4, IsToggle = true, RequireRestart = false, HintText = "Enable the management of party troops from the Clan Parties screen. Press ALT + click on the party to open the management UI.")]
        [SettingPropertyGroup("Enabled/Enable Manage Troops (ALT + Click)")]
        public bool ManageTroops { get; set; } = true;

        [SettingPropertyBool("Manage Troops of Companion Parties", Order = 5, RequireRestart = false, HintText = "Enables troop management for Clan Parties.")]
        [SettingPropertyGroup("Enabled/Enable Manage Troops (ALT + Click)")]
        public bool ManageParties { get; set; } = true;

        [SettingPropertyBool("Manage Troops of Companion Caravans", Order = 6, RequireRestart = false, HintText = "Enables troop management for Clan Caravans.")]
        [SettingPropertyGroup("Enabled/Enable Manage Troops (ALT + Click)")]
        public bool ManageCaravans { get; set; } = true;

        [SettingPropertyBool("Manage Settlement Garrisons", Order = 7, RequireRestart = false, HintText = "Enables troop management for Clan Settlement Garrisons.")]
        [SettingPropertyGroup("Enabled/Enable Manage Troops (ALT + Click)")]
        public bool ManageGarrisons { get; set; } = true;
    }
}
