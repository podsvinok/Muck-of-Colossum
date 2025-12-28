using System;
using Code.Gameplay.Levels;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Code.Gameplay
{
    public class TestEnemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        private ILevelDataProvider levelData;

        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }
        
        private void Update()
        {
            if (!levelData.Player) return;
            agent.enabled = true;
            if (agent.isOnNavMesh)
                agent.SetDestination(levelData.Player.transform.position);
        }
    }
}