using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusW.Shutterino.App
{
    public class ShutterinoViewModel
    {
        private ShutterinoLogic _logic;

        public ShutterinoViewModel(ShutterinoLogic logic)
        {
            _logic = logic;
        }
    }
}
