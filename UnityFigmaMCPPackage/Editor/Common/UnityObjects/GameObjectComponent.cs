using System.ComponentModel;

namespace UnityFigmaMCP.Common
{
    public class GameObjectComponent
    {
        [Description("Prefab asset path to instantiate instead of creating from scratch")]
        public string Prefab;

        [Description("Figma component key for asset binding lookup")]
        public string Key;
    }
}
