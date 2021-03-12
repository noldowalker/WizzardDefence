using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class TreasuryModel
    {
        public int Treasures {
            get; set;
        }

        public int MaxTreasures {
            get; set;
        }

        public TreasuryModel(int treasures) {
            this.Treasures = treasures;
            this.MaxTreasures = treasures;
        }
    }
}
