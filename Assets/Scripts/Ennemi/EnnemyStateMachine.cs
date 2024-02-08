using UnityEngine;
public class EnnemyStateMachine : MonoBehaviour
{
    IEnnemyState _currentEnnemyState;

    EnnemyActiveState _ennemyActiveState;
    EnnemyInactiveState ennemyIinactiveState;

    public void Start()
    {
        _currentEnnemyState = ennemyIinactiveState;
        _currentEnnemyState.OnEnter(this);
    }
    public void ChangeState(IEnnemyState newState)
    {
        _currentEnnemyState.OnExit(this);
        _currentEnnemyState = newState;
        _currentEnnemyState.OnEnter(this);
    }
}
