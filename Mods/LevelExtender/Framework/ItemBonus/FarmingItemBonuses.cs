using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelExtender.Common;
using StardewModdingAPI;

namespace LevelExtender.Framework.ItemBonus
{
    internal class TillerItemBonus : ItemBonusFromSkill
    {
        public TillerItemBonus()
        {
            skillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.TillerCropsWorthMore;
            ItemCategories = DefaultItemCategories.Crops;
        }
    }
    internal class TillerArtisanItemBonus : TillerItemBonus
    {
        public TillerArtisanItemBonus()
        {
            skillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.TillerArtisanWorthMore;
            ItemCategories = DefaultItemCategories.Artisan;
        }
    }
    internal class TillerAgriculturistItemBonus : TillerItemBonus
    {
        public TillerAgriculturistItemBonus()
        {
            skillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.TillerAgriculturistCropsGrowFaster;
        }
    }
    internal class RangerItemBonus : ItemBonusFromSkill
    {
        public RangerItemBonus()
        {
            skillType = SkillTypes.DefaultSkillTypes.Farming;
            ItemBonusType = ItemBonusType.RancherAnimalProductsWorthMore;
            ItemCategories = DefaultItemCategories.AnimalProducts;
        }
    }

    internal static class FarmingItemBonuses
    {
        public static List<TillerItemBonus> TillerItemBonuses => new List<TillerItemBonus>
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
        public static List<TillerArtisanItemBonus> ArsianItemBonuses => new List<TillerArtisanItemBonus>
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
        public static List<RangerItemBonus> RangerItemBonuses => new List<RangerItemBonus>
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
        public static bool ApplyMoreDrops(IEnumerable<LESkill> skills, StardewValley.Object item) 
        {
            var ret = false;
            ret = ItemBonuses.ApplyMoreDrops(TillerItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(ArsianItemBonuses, skills, item) || ret;
            ret = ItemBonuses.ApplyMoreDrops(RangerItemBonuses, skills, item) || ret;
            return ret;
        }
        public static bool ApplyWorthMore(IEnumerable<LESkill> skills, StardewValley.Object item, long specificPlayerID, ref int newprice)
        {
            var ret = false;
            ret = ItemBonuses.ApplyWorthMore(TillerItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(ArsianItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            ret = ItemBonuses.ApplyWorthMore(RangerItemBonuses, skills, item, specificPlayerID, ref newprice) || ret;
            return ret;
        }
    }
}
