using SaintCoinach;
using SaintCoinach.Ex;

namespace FFXIV_Data_Exporter.Library
{
    public class Realm
    {
        public ARealmReversed RealmReversed { get; }

        public Realm(string gamePath, string language) => RealmReversed = new ARealmReversed(gamePath, GetLanguage(language));

        private Language GetLanguage(string language) => 
            language.ToLower().Trim() switch
            {
                "english" => Language.English,
                "japanese" => Language.Japanese,
                "french" => Language.French,
                "german" => Language.German,
                _ => Language.None
            };
    }
}