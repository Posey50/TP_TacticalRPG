public interface IEnemyState
{
    /// <summary>
    /// Called when the enemy enters in this state.
    /// </summary>
    /// <param name="enemyMachine"> State machine of the enemy. </param>
    public void OnEnter(EnemyStateMachine enemyMachine);

    /// <summary>
    /// Called when the enemy exits in this state.
    /// </summary>
    /// <param name="enemyMachine"> State machine of the enemy. </param>
    public void OnExit(EnemyStateMachine enemyMachine);
}
