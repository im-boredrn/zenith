using System;
using System.Collections.Generic;
using System.Text;
using zenith.Core.AdaptationsCore.AdaptationBehaviors;
using zenith.Core.AdaptationsCore.AdaptationData;
using zenith.Core.Definitions;

namespace zenith.Core.AdaptationsCore
{
    public sealed class ActiveAdaptation
    {

        public AdaptationState State { get; }
       public AdaptationDefinitions Definitions { get; }
        public AdaptationBehavior Behavior { get; }
        public bool IsUnlocked => State.IsUnlocked;
        
        public ActiveAdaptation(AdaptationState adaptationState, AdaptationDefinitions adaptationDefinitions, AdaptationBehavior adaptationBehavior)
        {
            this.State = adaptationState;
            this.Definitions = adaptationDefinitions;
            this.Behavior = adaptationBehavior;
        }
    }
}
