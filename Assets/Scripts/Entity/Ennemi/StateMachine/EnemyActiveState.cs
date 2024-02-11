using UnityEngine;

public class EnemyActiveState : IEnemyState
{                                             
    public void OnEnter(EnemyStateMachine enemyStateMachine)
    {
        Debug.Log("Enemy ActiveState");
    }

    public void OnExit(EnemyStateMachine enemyStateMachine)
    {
        Debug.Log("Enemy Exit ActiveState");
    }
}
