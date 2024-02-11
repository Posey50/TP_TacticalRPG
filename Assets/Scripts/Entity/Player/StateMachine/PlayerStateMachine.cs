using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    /// <summary>
    /// The reference towards the player's Main
    /// </summary>
    public PlayerMain Main { get; private set; }

    public PlayerInactiveState StateInactive = new();
    public PlayerActiveState StateActive = new();

    private IPlayerState _currentState;

    
    void Start()
    {
        Main = GetComponent<PlayerMain>();

        ChangeState(StateInactive);
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
    private void ChangeState(IPlayerState newState)
    {
        if (_currentState != null)
        {
            _currentState.OnExit(this);
        }

        _currentState = newState;

        _currentState.OnEnter(this);
    }
}
