using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class ButtonComponent
    {
        [Description("False when the button is disabled (omitted when true)")]
        public bool? Interactable;

        [Description("Transition mode: \"None\", \"ColorTint\", \"SpriteSwap\" or \"Animation\"")]
        public string Transition;
    }
}
