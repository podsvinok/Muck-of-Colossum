using System;
using FishNet.Object;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : NetworkBehaviour
{
    [SerializeField] protected BehaviorGraphAgent behaviorAgent;
    [SerializeField] protected EnemyConfig config;
    [SerializeField] protected EnemyView view;
    
    public BehaviorGraphAgent BehaviorAgent => behaviorAgent;
    public EnemyConfig Config => config;
    public EnemyView View => view;
    public NavMeshAgent agent { get; protected set; }
    
    public IHealth Health { get; protected set; }
    public ITargetDetector TargetDetector { get; protected set; }

    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<IHealth>();
        TargetDetector = GetComponent<ITargetDetector>();
        
        if (IsServerInitialized)
        {
            behaviorAgent.enabled = true;
            agent.enabled = true;
            
            behaviorAgent.SetVariableValue("Enemy", this);
            behaviorAgent.SetVariableValue("EnemyView", view);
            behaviorAgent.SetVariableValue("MoveSpeed", config.moveSpeed);
            
            InitializeComponents();

        }
        else
        {
            behaviorAgent.enabled = false;
            agent.enabled = false;
        }
        
        
    }
    
    protected abstract void InitializeComponents();
}
