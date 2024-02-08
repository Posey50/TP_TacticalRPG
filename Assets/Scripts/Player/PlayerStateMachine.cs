using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerMain Main { get; private set; }

    [SerializeField] private PlayerInactiveState _stateInactive;
    [SerializeField] private PlayerActiveState _stateActive;

    private IState _currentState;

    // Start is called before the first frame update
    void Start()
    {
        Main = GetComponent<PlayerMain>();

        _currentState = _stateActive;
        _currentState.OnEnter(this);
    }

    public void ChangeState(IState newState)
    {
        _currentState.OnExit(this);

        _currentState = newState;

        _currentState.OnEnter(this);
    }
}
