using System.Collections.Generic;
using System.Linq;

namespace LevelExtender.Framework.ItemBonus
{
    class FisherItemBonus : ItemBonusBySkillData
    {
        public FisherItemBonus()
        {
            SkillId = SkillsList.DefaultSkillNames.Fishing;
            Profession = ItemBonusProfession.FisherFishWorthMore;
            ItemCategories = DefaultItemCategories.Fish;
        }
    }

    class FishingItemBonuses : IItemBonusesRegistration
    {
        public List<FisherItemBonus> FisherAnglerItemBonuses => new List<FisherItemBonus>
        {
            new FisherItemBonus
            {
                MinLevel = 20,
                Value = 75
            },
            new FisherItemBonus
            {
                MinLevel = 30,
                Value = 90
            },
            new FisherItemBonus
            {
                MinLevel = 50,
                Value = 100
            },
            new FisherItemBonus
            {
                MinLevel = 70,
                Value = 120
            },
            new FisherItemBonus
            {
                MinLevel = 100,
                Value = 150
            },
        };
        public List<IEnumerable<ItemBonusBySkillData>> RegisterItemBonuses()
        {
            return new List<IEnumerable<ItemBonusBySkillData>> {
                FisherAnglerItemBonuses.Cast<ItemBonusBySkillData>(),
            };
        }
    }
}
