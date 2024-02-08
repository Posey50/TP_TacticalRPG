using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActiveState : MonoBehaviour, IState
{
    public void OnEnter(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Enters Active State");
    }

    public void OnExit(PlayerStateMachine playerStateMachine)
    {
        Debug.Log("Player Exits Active State");
    }
}
