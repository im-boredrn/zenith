using System;
using System.Collections.Generic;
using System.Text;
using StatType = zenith.Core.Assimilation.StatOutput.StatType;
namespace zenith.GUI
{
    public class GUITotals
    {


        public Dictionary<StatType, float> GUIStats { get; set; } = new();
     
    }
}
