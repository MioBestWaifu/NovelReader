using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mio.Translation.Japanese.Edrdg
{
    public static class PropertyConverter
    {
        public static KanjiProperty StringToKanjiProperty(string property)
        {
            return property switch
            {
                "ateji (phonetic) reading" => KanjiProperty.Ateji,
                "word containing irregular kana usage" => KanjiProperty.IrregularKanaUsage,
                "word containing irregular kanji usage" => KanjiProperty.IrregularKanjiUsage,
                "irregular okurigana usage" => KanjiProperty.IrregularOkuriganaUsage,
                "word containing out-dated kanji or kanji usage" => KanjiProperty.OutdatedKanjiUsage,
                "rarely used kanji form" => KanjiProperty.RarelyUsedKanji,
                "search-only kanji form" => KanjiProperty.SearchOnlyKanji,
                _ => throw new ArgumentException("Invalid kanji property")
            };
        }

        public static string KanjiPropertyToString(KanjiProperty property)
        {
            return property switch
            {
                KanjiProperty.Ateji => "Ateji (phonetic) reading",
                KanjiProperty.IrregularKanaUsage => "Irregular kana usage",
                KanjiProperty.IrregularKanjiUsage => "Irregular kanji usage",
                KanjiProperty.IrregularOkuriganaUsage => "Irregular okurigana usage",
                KanjiProperty.OutdatedKanjiUsage => "Out-dated kanji or kanji usage",
                KanjiProperty.RarelyUsedKanji => "Rarely used kanji form",
                KanjiProperty.SearchOnlyKanji => "Search-only kanji form",
                _ => throw new ArgumentException("Invalid kanji property")
            };
        }

        public static FieldProperty StringToFieldProperty(string property)
        {
            return property switch
            {
                "agriculture" => FieldProperty.Agriculture,
                "anatomy" => FieldProperty.Anatomy,
                "archeology" => FieldProperty.Archeology,
                "architecture" => FieldProperty.Architecture,
                "art, aesthetics" => FieldProperty.Art,
                "astronomy" => FieldProperty.Astronomy,
                "audiovisual" => FieldProperty.Audiovisual,
                "aviation" => FieldProperty.Aviation,
                "baseball" => FieldProperty.Baseball,
                "biochemistry" => FieldProperty.Biochemistry,
                "biology" => FieldProperty.Biology,
                "botany" => FieldProperty.Botany,
                "boxing" => FieldProperty.Boxing,
                "Buddhism" => FieldProperty.Buddhism,
                "business" => FieldProperty.Business,
                "card games" => FieldProperty.CardGames,
                "chemistry" => FieldProperty.Chemistry,
                "Chinese mythology" => FieldProperty.ChineseMythology,
                "Christianity" => FieldProperty.Christianity,
                "civil engineering" => FieldProperty.CivilEngineering,
                "clothing" => FieldProperty.Clothing,
                "computing" => FieldProperty.Computing,
                "crystallography" => FieldProperty.Crystallography,
                "dentistry" => FieldProperty.Dentistry,
                "ecology" => FieldProperty.Ecology,
                "economics" => FieldProperty.Economics,
                "electricity, elec. eng." => FieldProperty.Electricity,
                "electronics" => FieldProperty.Electronics,
                "embryology" => FieldProperty.Embryology,
                "engineering" => FieldProperty.Engineering,
                "entomology" => FieldProperty.Entomology,
                "figure skating" => FieldProperty.FigureSkating,
                "film" => FieldProperty.Film,
                "finance" => FieldProperty.Finance,
                "fishing" => FieldProperty.Fishing,
                "food, cooking" => FieldProperty.FoodCooking,
                "gardening, horticulture" => FieldProperty.GardeningHorticulture,
                "genetics" => FieldProperty.Genetics,
                "geography" => FieldProperty.Geography,
                "geology" => FieldProperty.Geology,
                "geometry" => FieldProperty.Geometry,
                "go (game)" => FieldProperty.GoGame,
                "golf" => FieldProperty.Golf,
                "grammar" => FieldProperty.Grammar,
                "Greek mythology" => FieldProperty.GreekMythology,
                "hanafuda" => FieldProperty.Hanafuda,
                "horse racing" => FieldProperty.HorseRacing,
                "Internet" => FieldProperty.Internet,
                "Japanese mythology" => FieldProperty.JapaneseMythology,
                "kabuki" => FieldProperty.Kabuki,
                "law" => FieldProperty.Law,
                "linguistics" => FieldProperty.Linguistics,
                "logic" => FieldProperty.Logic,
                "martial arts" => FieldProperty.MartialArts,
                "mahjong" => FieldProperty.Mahjong,
                "manga" => FieldProperty.Manga,
                "mathematics" => FieldProperty.Mathematics,
                "mechanical engineering" => FieldProperty.MechanicalEngineering,
                "medicine" => FieldProperty.Medicine,
                "meteorology" => FieldProperty.Meteorology,
                "military" => FieldProperty.Military,
                "mineralogy" => FieldProperty.Mineralogy,
                "mining" => FieldProperty.Mining,
                "motorsport" => FieldProperty.Motorsport,
                "music" => FieldProperty.Music,
                "noh" => FieldProperty.Noh,
                "ornithology" => FieldProperty.Ornithology,
                "paleontology" => FieldProperty.Paleontology,
                "pathology" => FieldProperty.Pathology,
                "pharmacology" => FieldProperty.Pharmacology,
                "philosophy" => FieldProperty.Philosophy,
                "photography" => FieldProperty.Photography,
                "physics" => FieldProperty.Physics,
                "physiology" => FieldProperty.Physiology,
                "politics" => FieldProperty.Politics,
                "printing" => FieldProperty.Printing,
                "professional wrestling" => FieldProperty.ProfessionalWrestling,
                "psychiatry" => FieldProperty.Psychiatry,
                "psychoanalysis" => FieldProperty.Psychoanalysis,
                "psychology" => FieldProperty.Psychology,
                "railway" => FieldProperty.Railway,
                "Roman mythology" => FieldProperty.RomanMythology,
                "Shinto" => FieldProperty.Shinto,
                "shogi" => FieldProperty.Shogi,
                "skiing" => FieldProperty.Skiing,
                "sports" => FieldProperty.Sports,
                "statistics" => FieldProperty.Statistics,
                "stock market" => FieldProperty.StockMarket,
                "sumo" => FieldProperty.Sumo,
                "surgery" => FieldProperty.Surgery,
                "telecommunications" => FieldProperty.Telecommunications,
                "trademark" => FieldProperty.Trademark,
                "television" => FieldProperty.Television,
                "veterinary terms" => FieldProperty.VeterinaryTerms,
                "video games" => FieldProperty.VideoGames,
                "zoology" => FieldProperty.Zoology,
                _ => throw new ArgumentException("Invalid field property")
            };
        }

        public static MiscProperty StringToMiscProperty(string property)
        {
            return property switch
            {
                "abbreviation" => MiscProperty.Abbreviation,
                "archaic" => MiscProperty.Archaic,
                "character" => MiscProperty.Character,
                "children's language" => MiscProperty.ChildrensLanguage,
                "colloquial" => MiscProperty.Colloquial,
                "company name" => MiscProperty.CompanyName,
                "creature" => MiscProperty.Creature,
                "dated term" => MiscProperty.DatedTerm,
                "deity" => MiscProperty.Deity,
                "derogatory" => MiscProperty.Derogatory,
                "document" => MiscProperty.Document,
                "euphemistic" => MiscProperty.Euphemistic,
                "event" => MiscProperty.Event,
                "familiar language" => MiscProperty.FamiliarLanguage,
                "female term or language" => MiscProperty.FemaleTermOrLanguage,
                "fiction" => MiscProperty.Fiction,
                "formal or literary term" => MiscProperty.FormalOrLiteraryTerm,
                "given name or forename, gender not specified" => MiscProperty.GivenNameOrForename,
                "group" => MiscProperty.Group,
                "historical term" => MiscProperty.HistoricalTerm,
                "honorific or respectful (sonkeigo) language" => MiscProperty.HonorificOrRespectfulLanguage,
                "humble (kenjougo) language" => MiscProperty.HumbleLanguage,
                "idiomatic expression" => MiscProperty.IdiomaticExpression,
                "jocular, humorous term" => MiscProperty.Jocular,
                "legend" => MiscProperty.Legend,
                "manga slang" => MiscProperty.MangaSlang,
                "male term or language" => MiscProperty.MaleTermOrLanguage,
                "mythology" => MiscProperty.Mythology,
                "Internet slang" => MiscProperty.InternetSlang,
                "object" => MiscProperty.Object,
                "obsolete term" => MiscProperty.ObsoleteTerm,
                "onomatopoeic or mimetic word" => MiscProperty.OnomatopoeicOrMimeticWord,
                "organization name" => MiscProperty.OrganizationName,
                "other" => MiscProperty.Other,
                "full name of a particular person" => MiscProperty.PersonFullName,
                "place name" => MiscProperty.PlaceName,
                "poetical term" => MiscProperty.PoeticalTerm,
                "polite (teineigo) language" => MiscProperty.PoliteLanguage,
                "product name" => MiscProperty.ProductName,
                "proverb" => MiscProperty.Proverb,
                "quotation" => MiscProperty.Quotation,
                "rare term" => MiscProperty.RareTerm,
                "religion" => MiscProperty.Religion,
                "sensitive" => MiscProperty.Sensitive,
                "service" => MiscProperty.Service,
                "ship name" => MiscProperty.ShipName,
                "slang" => MiscProperty.Slang,
                "railway station" => MiscProperty.RailwayStation,
                "family or surname" => MiscProperty.Surname,
                "word usually written using kana alone" => MiscProperty.UsuallyKanaAlone,
                "unclassified name" => MiscProperty.UnclassifiedName,
                "vulgar expression or word" => MiscProperty.VulgarExpressionOrWord,
                "work of art, literature, music, etc. name" => MiscProperty.WorkOfArtName,
                "rude or X-rated term (not displayed in educational software)" => MiscProperty.RudeOrXRatedTerm,
                "yojijukugo" => MiscProperty.Yojijukugo,

                _ => throw new ArgumentException("Invalid misc property")
            };
        }

        public static KanaProperty StringToKanaProperty(string property)
        {
            return property switch
            {
                "gikun (meaning as reading) or jukujikun (special kanji reading)" => KanaProperty.Gikun,
                "word containing irregular kana usage" => KanaProperty.IrregularKanaUsage,
                "out-dated or obsolete kana usage" => KanaProperty.OutdatedKanaUsage,
                "rarely used kana form" => KanaProperty.RarelyUsedKanaForm,
                "search-only kana form" => KanaProperty.SearchOnlyKanaForm,
                _ => throw new ArgumentException("Invalid kana property")
            };
        }

    }
}
