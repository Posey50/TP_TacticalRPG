using UnityEngine;

public class EnemyInactiveState : IEnemyState
{
    public void OnEnter(EnemyStateMachine enemyStateMachine)
    {
        Debug.Log("Enemy InactiveState");
    }

    public void OnExit(EnemyStateMachine enemyStateMachine)
    {
        Debug.Log("Enemy Exit InactiveState");
    }
}
