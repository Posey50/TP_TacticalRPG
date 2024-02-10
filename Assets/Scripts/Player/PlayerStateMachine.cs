using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    /// <summary>
    /// The reference towards the player's Main
    /// </summary>
    public PlayerMain Main { get; private set; }

    public PlayerInactiveState StateInactive { get; private set; }
    public PlayerActiveState StateActive { get; private set; }

    private IState _currentState;

    
    void Start()
    {
        Main = GetComponent<PlayerMain>();

        StateInactive = GetComponent<PlayerInactiveState>();
        StateActive = GetComponent<PlayerActiveState>();

        _currentState = StateInactive;
        _currentState.OnEnter(this);
    }

    /// <summary>
    /// Changes the player's state. Also calls the states OnExit and OnEnter
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IState newState)
    {
        _currentState.OnExit(this);

        _currentState = newState;

        _currentState.OnEnter(this);
    }

    //Here for DebugPurpose
    public void SwitchSate()
    {
        if (_currentState.Equals(StateInactive))
        {
            ChangeState(StateActive);
        }
        else
        {
            ChangeState(StateInactive);
        }
    }
}
