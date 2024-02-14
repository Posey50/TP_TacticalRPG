public class EnemyActiveState : IEnemyState
{
    /// <summary>
    /// State machine of the enemy.
    /// </summary>
    private EnemyStateMachine _enemyStateMachine;

    public void OnEnter(EnemyStateMachine enemyStateMachine)
    {
        _enemyStateMachine = enemyStateMachine;
        _enemyStateMachine.EnemyMain.StartReflexion();
    }

    public void OnExit(EnemyStateMachine enemyStateMachine)
    {
        
    }
}
