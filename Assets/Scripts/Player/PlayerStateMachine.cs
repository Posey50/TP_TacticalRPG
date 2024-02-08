using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerInactiveState _stateInactive;
    private PlayerActiveState _stateActive;

    private IState _currentState;

    // Start is called before the first frame update
    void Start()
    {
        _currentState = _stateInactive;

        _currentState.OnEnter(this);
    }

    public void ChangeState(IState newState)
    {
        _currentState.OnExit(this);

        _currentState = newState;

        _currentState.OnEnter(this);
    }
}
