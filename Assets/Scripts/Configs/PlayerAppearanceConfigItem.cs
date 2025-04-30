namespace HFantasy.Script.Configs
{
    public enum AppearancePartType
    {
        Body,
        Hair,
        Armor,
    }

    public class AppearanceConfigItem
    {
        public int ResId;
        public string Name;
        public AppearancePartType PartType;
        public int PartTypeId;
        public string BundleName;
        public string AssetName;
    }
}
