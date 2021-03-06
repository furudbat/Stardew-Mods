using System.Collections.Generic;

namespace LevelExtender.Framework.ItemBonus
{
    interface IItemBonusesRegistration
    {
        List<IEnumerable<ItemBonusBySkillData>> RegisterItemBonuses();
    }
}
