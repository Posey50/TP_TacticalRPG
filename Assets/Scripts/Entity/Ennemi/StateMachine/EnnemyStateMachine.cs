using UnityEngine;
public class EnnemyStateMachine : MonoBehaviour
{
    private IEnnemyState _currentEnnemyState;

    public EnnemyActiveState EnnemyActiveState { get; private set; }
    public EnnemyInactiveState EnnemyIinactiveState { get; private set; }
   

    public void Start()
    {
        EnnemyActiveState = GetComponent<EnnemyActiveState>();
        EnnemyIinactiveState = GetComponent<EnnemyInactiveState>();

        _currentEnnemyState = EnnemyIinactiveState;
        _currentEnnemyState.OnEnter(this);
    }
    public void ChangeState(IEnnemyState newState)
    {
        _currentEnnemyState.OnExit(this);
        _currentEnnemyState = newState;
        _currentEnnemyState.OnEnter(this);
    }
}
