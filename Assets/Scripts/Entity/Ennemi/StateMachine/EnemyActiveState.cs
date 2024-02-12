using UnityEngine;

public class EnemyActiveState :  IEnemyState
{
    [SerializeField] private EnemyStateMachine _stateMachine;
    public void OnEnter(EnemyStateMachine enemyStateMachine)
    {
        _stateMachine = enemyStateMachine;
        _stateMachine.Main.ChosePlayer();
    }

    public void OnExit(EnemyStateMachine enemyStateMachine)
    {
        Debug.Log("Enemy Exit ActiveState");
    }

}
