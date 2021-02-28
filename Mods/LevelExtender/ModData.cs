using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelExtender
{
    [Serializable]
    public class ModData {
        // Fishing
        public int FXP { get; set; } = 0;
        public int FLV { get; set; } = 0;

        // Farming
        public int FaXP { get; set; } = 0;
        public int FaLV { get; set; } = 0;

        // Mining
        public int MXP { get; set; } = 0;
        public int MLV { get; set; } = 0;

        //Combat
        public int CXP { get; set; } = 0;
        public int CLV { get; set; } = 0;

        // Foraging
        public int FoXP { get; set; } = 0;
        public int FoLV { get; set; } = 0;
    }
}
