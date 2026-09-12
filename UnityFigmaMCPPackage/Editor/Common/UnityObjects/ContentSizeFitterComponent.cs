using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class ContentSizeFitterComponent
    {
        [Description("Horizontal fit: \"unconstrained\", \"preferred\" or \"minimum\"")]
        public string HFit { get; set; }

        [Description("Vertical fit: \"unconstrained\", \"preferred\" or \"minimum\"")]
        public string VFit { get; set; }
    }
}
