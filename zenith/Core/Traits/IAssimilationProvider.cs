using System;
using System.Collections.Generic;
using System.Text;

namespace zenith.Core.Traits
{
    public interface IAssimilationProvider
    {

        int GetAssimCounter();
        int GetAssimStage();
        int GetAssimThreshold();
    }
}
