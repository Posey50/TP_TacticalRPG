using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OnEnter(PlayerStateMachine playerMachine);

    public void OnExit(PlayerStateMachine playerMachine);
}
