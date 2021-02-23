using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace LevelExtender.LEAPI
{
    public interface ILEModApi
    {
        void Spawn_Rate(double osr);
        int[] currentXP();
        int[] requiredXP();

        event EventHandler<LEXPEventArgs> OnXPChanged;
        IEnumerable<ILESkill> GetSkills();
        int GetSkillCurrentXP(string skillName);
        int GetSkillRequiredXP(string skillName);
        int GetSkillLevel(string skillName);
        int GetSkillMaxLevel(string skillName);
        int GetXPRequiredToLevel(string skillName, int level);

        //*** SKILL COMPATIBILITY ***//
        void RegisterMod(ISkillMod mod, IMonitor logMonitor);


        //*** LEGECY SKILL COMPATIBILITY ***//
        //Please use this function ONCE in the Save Loaded event (to be safe, PLEASE ADD A 5 SECOND DELAY BEFORE initialization):
        //
        //FORMAT:
        //initializeSkill(string <name of skill>, int <current xp value>, double? <xp_modifier, default = 1.0>, (optional) List<int> <current xp table>, (optional) int[] <numerical game categories the skill is related to, allows for LE skill modifiers/buffs>)
        //
        //EXAMPLE:
        //initializeSkill("cooking", 1305, 1.0, new List<int>() {100, 1000, 2000, 5000, 10000}, new int[] {-4 (fishing cat.), -105 (custom cat.)})
        //int initializeSkill(string name, int xp, double? xp_mod = null, List<int> xp_table = null, int[] cats = null);

        //Any requests to change a skill MUST FIRST check the current value of said variable and assign it to the corresponding variable
        //in your mod. (If this is not done, the skill variables across mods may not be synced)
        //
        //For getting data from a skill, use this parameter format:
        //args = {"get", "<name of skill>", "<name of variable>"} -> Example: args = {"get", "cooking", "xp"}
        //
        //For setting skill data, use this parameter format:
        //args = {"set", "<name of skill>", "<name of variable>", "<string representation of value>"} -> Example: args = {"set", "cooking", "level", "25"}
        //dynamic TalkToSkill(string[] args);
    }
}
