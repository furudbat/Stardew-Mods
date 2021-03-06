using System.Collections.Generic;
using System.Linq;

namespace LevelExtender.Framework.ItemBonus
{
    class MinerItemBonus : ItemBonusBySkillData
    {
        public MinerItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Mining;
            Profession = ItemBonusProfession.MinerMoreOre;
            ItemCategories = DefaultItemCategories.OresOrBars;
            Items = DefaultItems.Ores;
        }
    }
    class MinerBlacksmithItemBonus : MinerItemBonus
    {
        public MinerBlacksmithItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Mining;
            Profession = ItemBonusProfession.MinerBlacksmithBarsWorthMore;
            ItemCategories = DefaultItemCategories.OresOrBars;
            Items = DefaultItems.Bars;
        }
    }
    class MinerProspectorItemBonus : MinerItemBonus
    {
        public MinerProspectorItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Mining;
            Profession = ItemBonusProfession.MinerProspectorMoreCoal;
            ItemCategories = DefaultItemCategories.OresOrBars;
            Items = DefaultItems.Coal;
        }
    }
    class GeologistItemBonus : ItemBonusBySkillData
    {
        public GeologistItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Mining;
            Profession = ItemBonusProfession.GeologistMoreGems;
            ItemCategories = DefaultItemCategories.Gems;
            Items = DefaultItems.Any;
        }
    }
    class GeologistExcavatorItemBonus : GeologistItemBonus
    {
        public GeologistExcavatorItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Mining;
            Profession = ItemBonusProfession.GeologistExcavatorMoreGeodes;
            ItemCategories = DefaultItemCategories.Any;
            Items = DefaultItems.Geodes;
        }
    }
    class GeologistGemologistItemBonus : GeologistItemBonus
    {
        public GeologistGemologistItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Mining;
            Profession = ItemBonusProfession.GeologistGemologistGemsWorthMore;
            ItemCategories = DefaultItemCategories.GemsOrMinerals;
            Items = DefaultItems.Any;
        }
    }

    class MiningItemBonuses : IItemBonusesRegistration
    {
        public List<MinerItemBonus> MinerItemBonuses => new List<MinerItemBonus>
        {
            new MinerItemBonus
            {
                MinLevel = 10,
                Value = 1,
                Chance = 0.25
            },
            new MinerItemBonus
            {
                MinLevel = 20,
                Value = 1,
                Chance = 0.5
            },
            new MinerItemBonus
            {
                MinLevel = 30,
                Value = 2,
                Chance = 0.25
            },
            new MinerItemBonus
            {
                MinLevel = 40,
                Value = 2,
                Chance = 0.5
            },
            new MinerItemBonus
            {
                MinLevel = 50,
                Value = 2,
                Chance = 0.75
            },
            new MinerItemBonus
            {
                MinLevel = 60,
                Value = 3,
                Chance = 0.10
            },
            new MinerItemBonus
            {
                MinLevel = 70,
                Value = 3,
                Chance = 0.25
            },
            new MinerItemBonus
            {
                MinLevel = 80,
                Value = 4,
                Chance = 0.5
            },
            new MinerItemBonus
            {
                MinLevel = 90,
                Value = 4,
                Chance = 0.10
            },
            new MinerItemBonus
            {
                MinLevel = 100,
                Value = 5,
                Chance = 0.25
            },
        };
        public List<MinerBlacksmithItemBonus> MinerBlacksmithItemBonuses => new List<MinerBlacksmithItemBonus>
        {
            new MinerBlacksmithItemBonus
            {
                MinLevel = 20,
                Value = 20
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 30,
                Value = 40
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 50,
                Value = 60
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 70,
                Value = 85
            },
            new MinerBlacksmithItemBonus
            {
                MinLevel = 100,
                Value = 110
            },
        };
        public List<MinerProspectorItemBonus> MinerProspectorItemBonuses => new List<MinerProspectorItemBonus>
        {
            new MinerProspectorItemBonus
            {
                MinLevel = 20,
                Chance = 0.5,
                Value = 2
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 30,
                Chance = 0.75,
                Value = 3
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 50,
                Chance = 0.75,
                Value = 5
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 70,
                Chance = 1.0,
                Value = 10
            },
            new MinerProspectorItemBonus
            {
                MinLevel = 100,
                Chance = 1.0,
                Value = 15
            },
        };
        public List<GeologistItemBonus> GeologistItemBonuses => new List<GeologistItemBonus>
        {
            new GeologistItemBonus
            {
                MinLevel = 20,
                Chance = 0.30,
                Value = 1
            },
            new GeologistItemBonus
            {
                MinLevel = 30,
                Chance = 0.40,
                Value = 1
            },
            new GeologistItemBonus
            {
                MinLevel = 50,
                Chance = 0.50,
                Value = 2
            },
            new GeologistItemBonus
            {
                MinLevel = 70,
                Chance = 0.50,
                Value = 2
            },
            new GeologistItemBonus
            {
                MinLevel = 100,
                Chance = 0.50,
                Value = 3
            },
        };
        public List<GeologistExcavatorItemBonus> GeologistExcavatorItemBonuses => new List<GeologistExcavatorItemBonus>
        {
            new GeologistExcavatorItemBonus
            {
                MinLevel = 20,
                Chance = 0.50,
                Value = 2
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 30,
                Chance = 0.75,
                Value = 2
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 50,
                Chance = 0.75,
                Value = 3
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 70,
                Chance = 0.8,
                Value = 3
            },
            new GeologistExcavatorItemBonus
            {
                MinLevel = 100,
                Chance = 0.8,
                Value = 4
            },
        };
        public List<GeologistGemologistItemBonus> GeologistGemologistItemBonuses => new List<GeologistGemologistItemBonus>
        {
            new GeologistGemologistItemBonus
            {
                MinLevel = 20,
                Value = 30
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 30,
                Value = 40
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 50,
                Value = 50
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 70,
                Value = 60
            },
            new GeologistGemologistItemBonus
            {
                MinLevel = 100,
                Value = 70
            },
        };
        public List<IEnumerable<ItemBonusBySkillData>> RegisterItemBonuses()
        {
            return new List<IEnumerable<ItemBonusBySkillData>> {
                MinerItemBonuses.Cast<ItemBonusBySkillData>(),
                MinerBlacksmithItemBonuses.Cast<ItemBonusBySkillData>(),
                MinerProspectorItemBonuses.Cast<ItemBonusBySkillData>(),
                GeologistItemBonuses.Cast<ItemBonusBySkillData>(),
                GeologistExcavatorItemBonuses.Cast<ItemBonusBySkillData>(),
                GeologistGemologistItemBonuses.Cast<ItemBonusBySkillData>(),
            };
        }
    }
}
