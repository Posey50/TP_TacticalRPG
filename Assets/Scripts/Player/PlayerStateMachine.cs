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
    /// Stats current state to Active
    /// </summary>
    public void ChangeToActive()
    {
        ChangeState(StateActive);
    }

    /// <summary>
    /// Stats current state to Inactive
    /// </summary>
    public void ChangeToInactive()
    {
        ChangeState(StateInactive);
    }

    /// <summary>
    /// Changes the player's state. Also calls the states OnExit and OnEnter
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(IState newState)
    {
        if (_currentState.Equals(newState))
        {
            return;
        }

        _currentState.OnExit(this);

        _currentState = newState;

        _currentState.OnEnter(this);
    }
}
