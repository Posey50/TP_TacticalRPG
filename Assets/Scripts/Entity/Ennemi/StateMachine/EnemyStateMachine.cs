using UnityEngine;
public class EnemyStateMachine : MonoBehaviour
{
    private IEnemyState _currentEnemyState;

    public EnemyActiveState EnemyActiveState { get; private set; }
    public EnemyInactiveState EnemyIinactiveState { get; private set; }
   

    public void Start()
    {
        EnemyActiveState = GetComponent<EnemyActiveState>();
        EnemyIinactiveState = GetComponent<EnemyInactiveState>();

        _currentEnemyState = EnemyIinactiveState;
        _currentEnemyState.OnEnter(this);
    }
    public void ChangeState(IEnemyState newState)
    {
        _currentEnemyState.OnExit(this);
        _currentEnemyState = newState;
        _currentEnemyState.OnEnter(this);
    }
}
