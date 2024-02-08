using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnnemyStateMachine
{
    public IEnnemyState Current;
    public ActiveState ActiveState;
    public InactiveState InactiveState;

    public void ChangeState(IEnnemyState newState)
    {

    }
}
