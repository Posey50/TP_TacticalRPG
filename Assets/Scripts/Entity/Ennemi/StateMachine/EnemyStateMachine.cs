using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    /// <summary>
    /// Main component of the enemy.
    /// </summary>
    public EnemyMain EnemyMain { get; private set; }

    /// <summary>
    /// Manager of the battle.
    /// </summary>
    public BattleManager BattleManager { get; private set; }

    /// <summary>
    /// Active state of the enemy.
    /// </summary>
    public EnemyActiveState ActiveState { get; private set; } = new ();

    /// <summary>
    /// Inactive state of the enemy.
    /// </summary>
    public EnemyInactiveState InactiveState { get; private set; } = new ();
    
    /// <summary>
    /// Current state of the enemy.
    /// </summary>
    public IEnemyState CurrentState { get; private set; }

    public void Start()
    {
        EnemyMain = GetComponent<EnemyMain>();
        BattleManager = BattleManager.Instance;

        EnemyMain.TurnIsEnd += DesactiveEntity;

        // Sets state by default
        ChangeState(InactiveState);
    }

    /// <summary>
    /// Called to desactive the entity.
    /// </summary>
    private void DesactiveEntity()
    {
        ChangeState(InactiveState);
    }

    /// <summary>
    /// Called to change the current state.
    /// </summary>
    /// <param name="newState"> New state to set. </param>
    public void ChangeState(IEnemyState newState)
    {
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState.OnEnter(this);
    }
}
