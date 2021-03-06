﻿using System.Collections.Generic;
using System.Linq;

namespace LevelExtender.Framework.ItemBonus
{
    class GathererItemBonus : ItemBonusBySkillData
    {
        public GathererItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Foraging;
            Profession = ItemBonusProfession.GathererMoreForage;
            ItemCategories = DefaultItemCategories.GreensForaging;
        }
    }
    class ForesterTapperItemBonus : ItemBonusBySkillData
    {
        public ForesterTapperItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Foraging;
            Profession = ItemBonusProfession.ForesterTapperSyrupWorthMore;
            ItemCategories = DefaultItemCategories.Syrup;
        }
    }

    class ForagingItemBonuses
    {
        public List<GathererItemBonus> GathererItemBonuses => new List<GathererItemBonus>
        {
            new GathererItemBonus
            {
                MinLevel = 10,
                Chance = 0.25,
                Value = 2
            },
            new GathererItemBonus
            {
                MinLevel = 20,
                Chance = 0.25,
                Value = 2
            },
            new GathererItemBonus
            {
                MinLevel = 30,
                Chance = 0.30,
                Value = 2
            },
            new GathererItemBonus
            {
                MinLevel = 50,
                Chance = 0.40,
                Value = 3
            },
            new GathererItemBonus
            {
                MinLevel = 70,
                Chance = 0.40,
                Value = 3
            },
            new GathererItemBonus
            {
                MinLevel = 100,
                Chance = 0.60,
                Value = 4
            },
        };
        public List<ForesterTapperItemBonus> ForesterTapperItemBonuses => new List<ForesterTapperItemBonus>
        {
            new ForesterTapperItemBonus
            {
                MinLevel = 20,
                Value = 30
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 30,
                Value = 35
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 50,
                Value = 40
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 70,
                Value = 45
            },
            new ForesterTapperItemBonus
            {
                MinLevel = 100,
                Value = 50
            },
        };
        public List<IEnumerable<ItemBonusBySkillData>> RegisterItemBonuses()
        {
            return new List<IEnumerable<ItemBonusBySkillData>> {
                GathererItemBonuses.Cast<ItemBonusBySkillData>(),
                ForesterTapperItemBonuses.Cast<ItemBonusBySkillData>(),
            };
        }
    }
}
