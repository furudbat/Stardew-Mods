using System.Collections.Generic;
using System.Linq;

namespace LevelExtender.Framework.ItemBonus
{
    class TillerItemBonus : ItemBonusBySkillData
    {
        public TillerItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Farming;
            Profession = ItemBonusProfession.TillerCropsWorthMore;
            ItemCategories = DefaultItemCategories.Crops;
        }
    }
    class TillerArtisanItemBonus : TillerItemBonus
    {
        public TillerArtisanItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Farming;
            Profession = ItemBonusProfession.TillerArtisanWorthMore;
            ItemCategories = DefaultItemCategories.Artisan;
        }
    }
    class TillerAgriculturistItemBonus : TillerItemBonus
    {
        public TillerAgriculturistItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Farming;
            Profession = ItemBonusProfession.TillerAgriculturistCropsGrowFaster;
        }
    }
    class RangerItemBonus : ItemBonusBySkillData
    {
        public RangerItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Farming;
            Profession = ItemBonusProfession.RancherAnimalProductsWorthMore;
            ItemCategories = DefaultItemCategories.AnimalProducts;
        }
    }
    class BetterQualityCropsItemBonus : ItemBonusBySkillData
    {
        public BetterQualityCropsItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Farming;
            Profession = ItemBonusProfession.BetterQuality;
            ItemCategories = DefaultItemCategories.Crops;
            ExtraItems = DefaultItems.ExtraCrops;
        }
    }

    class FarmingItemBonuses : IItemBonusesRegistration
    {
        public List<TillerItemBonus> TillerItemBonuses => new List<TillerItemBonus>
        {
            new TillerItemBonus
            {
                MinLevel = 10,
                Value = 15
            },
            new TillerItemBonus
            {
                MinLevel = 20,
                Value = 20
            },
            new TillerItemBonus
            {
                MinLevel = 30,
                Value = 30
            },
            new TillerItemBonus
            {
                MinLevel = 50,
                Value = 40
            },
            new TillerItemBonus
            {
                MinLevel = 70,
                Value = 60
            },
            new TillerItemBonus
            {
                MinLevel = 100,
                Value = 80
            },
        };
        public List<TillerArtisanItemBonus> ArsianItemBonuses => new List<TillerArtisanItemBonus>
        {
            new TillerArtisanItemBonus
            {
                MinLevel = 20,
                Value = 30
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 30,
                Value = 40
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 50,
                Value = 60
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 70,
                Value = 85
            },
            new TillerArtisanItemBonus
            {
                MinLevel = 100,
                Value = 110
            },
        };
        public List<RangerItemBonus> RangerItemBonuses => new List<RangerItemBonus>
        {
            new RangerItemBonus
            {
                MinLevel = 10,
                Value = 40
            },
            new RangerItemBonus
            {
                MinLevel = 20,
                Value = 45
            },
            new RangerItemBonus
            {
                MinLevel = 30,
                Value = 50
            },
            new RangerItemBonus
            {
                MinLevel = 50,
                Value = 55
            },
            new RangerItemBonus
            {
                MinLevel = 70,
                Value = 60
            },
            new RangerItemBonus
            {
                MinLevel = 100,
                Value = 80
            },
        };
        public List<BetterQualityCropsItemBonus> BetterQualityCropsItemBonuses => new List<BetterQualityCropsItemBonus>
        {
            new BetterQualityCropsItemBonus
            {
                MinLevel = 15,
                Values = {
                    new ItemBonusValue { Chance = 0.38, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.23, Value = ItemQuality.Gold },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 20,
                Values = {
                    new ItemBonusValue { Chance = 0.44, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.25, Value = ItemQuality.Gold },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 30,
                Values = {
                    new ItemBonusValue { Chance = 0.60, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.33, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.01, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 40,
                Values = {
                    new ItemBonusValue { Chance = 0.75, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.45, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.05, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 50,
                Values = {
                    new ItemBonusValue { Chance = 0.89, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.55, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.09, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 60,
                Values = {
                    new ItemBonusValue { Chance = 1.0, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.65, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.14, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 70,
                Values = {
                    new ItemBonusValue { Chance = 1.0, Value = ItemQuality.Silver },
                    new ItemBonusValue { Chance = 0.75, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.18, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 80,
                Values = {
                    new ItemBonusValue { Chance = 1.0, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.25, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 90,
                Values = {
                    new ItemBonusValue { Chance = 1.0, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.38, Value = ItemQuality.Irudium },
                }
            },
            new BetterQualityCropsItemBonus
            {
                MinLevel = 100,
                Values = {
                    new ItemBonusValue { Chance = 1.0, Value = ItemQuality.Gold },
                    new ItemBonusValue { Chance = 0.51, Value = ItemQuality.Irudium },
                }
            },
        };

        public List<TillerAgriculturistItemBonus> TillerAgriculturistItemBonuses => new List<TillerAgriculturistItemBonus>
        {
            new TillerAgriculturistItemBonus
            {
                MinLevel = 20,
                Value = 5
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 30,
                Value = 9
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 40,
                Value = 12
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 50,
                Value = 15
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 60,
                Value = 18
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 70,
                Value = 21
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 80,
                Value = 24
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 90,
                Value = 27
            },
            new TillerAgriculturistItemBonus
            {
                MinLevel = 100,
                Value = 30
            },
        };

        public List<IEnumerable<ItemBonusBySkillData>> RegisterItemBonuses()
        {
            return new List<IEnumerable<ItemBonusBySkillData>> {
                TillerItemBonuses.Cast<ItemBonusBySkillData>(),
                ArsianItemBonuses.Cast<ItemBonusBySkillData>(),
                BetterQualityCropsItemBonuses.Cast<ItemBonusBySkillData>(),
                TillerAgriculturistItemBonuses.Cast<ItemBonusBySkillData>(),
            };
        }
    }
}
