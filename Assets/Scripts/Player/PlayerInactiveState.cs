using UnityEngine;

public class PlayerInactiveState : MonoBehaviour, IState
{
    private PlayerStateMachine _playerStateMachine;

    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Enters Inactive State");
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Exits Inactive State");
    }
}
