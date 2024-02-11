using UnityEngine;

public class PlayerInactiveState : IPlayerState
{
    private PlayerStateMachine _playerStateMachine;

    /// <summary>
    /// Properly sets up the State before its use
    /// </summary>
    /// <param name="playerStateMachine"></param>
    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Enters Inactive State");
    }

    /// <summary>
    /// Properly cleans up the State after its use
    /// </summary>
    /// <param name="playerStateMachine"></param>
    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Exits Inactive State");
    }
}
