using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zenith.Core.Progression
{
    public interface IStageProvider
    {


        int GetStage();
        float GetStageMultiplier();
        string GetStageName();
        float GetIgniteChanceMultiplier();
        float GetResistanceMultiplier();
        float GetMiningSpeedMultiplier();
      //  float GetArmorWSAMultiplier();

        event Action OnStageUp;
    }
}
