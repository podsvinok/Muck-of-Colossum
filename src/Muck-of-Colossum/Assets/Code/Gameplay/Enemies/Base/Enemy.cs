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
    public IEnemyMovement EnemyMovement { get; protected set; }

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<IHealth>();
        TargetDetector = GetComponent<ITargetDetector>();
        EnemyMovement = GetComponent<IEnemyMovement>();
        
        behaviorAgent.SetVariableValue("Enemy", this);
        behaviorAgent.SetVariableValue("EnemyView", view);
        behaviorAgent.SetVariableValue("MoveSpeed", config.moveSpeed);
        
        
        
        behaviorAgent.GetVariable("MoveSpeed", out var speed);
        
        Debug.Log(speed);
        
        InitializeComponents();
    }
    
    protected abstract void InitializeComponents();
}
