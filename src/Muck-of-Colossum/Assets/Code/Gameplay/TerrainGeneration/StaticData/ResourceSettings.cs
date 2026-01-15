using System;
using Code.Gameplay.ResourceSystem;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "Resource Settings", menuName = "TerrainGenerationSettings/Resource Settings")]
    public class ResourceSettings : UpdatableData
    {
        public ResourceGroup[] resourceGroups;
    }
}