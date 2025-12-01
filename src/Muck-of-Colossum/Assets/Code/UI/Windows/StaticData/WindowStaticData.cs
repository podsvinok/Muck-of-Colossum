using System.Collections.Generic;
using UnityEngine;

namespace Code.UI.Windows.StaticData
{
    [CreateAssetMenu(menuName = "StaticData/WindowsStaticData", fileName = "WindowsStaticData")]
    public class WindowStaticData : ScriptableObject
    {
        public List<WindowConfig> Configs;
    }
}