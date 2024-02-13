using UnityEngine;

public class PlayerInactiveState : IPlayerState
{
    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log(playerStateMachine.PlayerMain.Name + " Enters Inactive State");
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log(playerStateMachine.PlayerMain.Name + " exits Inactive State");
    }
}
