using UnityEngine;
public class EnemyStateMachine : MonoBehaviour
{
    public MainNameEnemy Main { get; private set; }
    public EnemyActiveState EnemyActiveState { get; private set; }
    public EnemyInactiveState EnemyIinactiveState { get; private set; }
    
    private IEnemyState _currentEnemyState;

    public void Start()
    {
        EnemyActiveState = GetComponent<EnemyActiveState>();
        EnemyIinactiveState = GetComponent<EnemyInactiveState>();

        ChangeToInactive();
    }

    /// <summary>
    /// Stats current state to Active
    /// </summary>
    public void ChangeToActive()
    {
        ChangeState(EnemyActiveState);
    }

    /// <summary>
    /// Stats current state to Inactive
    /// </summary>
    public void ChangeToInactive()
    {
        ChangeState(EnemyIinactiveState);
    }

    /// <summary>
    /// Changes the player's state. Also calls the states OnExit and OnEnter
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IEnemyState newState)
    {
        if (_currentEnemyState != null)
        {
            _currentEnemyState.OnExit(this);
        }

        _currentEnemyState = newState;

        _currentEnemyState.OnEnter(this);
    }
}
