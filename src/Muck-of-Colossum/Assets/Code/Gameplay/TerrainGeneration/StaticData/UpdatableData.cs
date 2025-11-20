using System;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    public class UpdatableData : ScriptableObject
    {
        public event Action OnValuesUpdated;
        public bool autoUpdate;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (autoUpdate)
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }

        private void NotifyOfUpdatedValues()
        {
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            OnValuesUpdated?.Invoke();
        }
#endif
    }
}