using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class TreasuryModel
    {
        private int treasures;

        public int Treasures {
            get; set;
        }

        public TreasuryModel(int treasures) {
            this.treasures = treasures;
        }
    }
}
