using UnityEngine;

public class EnnemyActiveState : IEnnemyState
{
    public void OnEnter(EnnemyStateMachine ennemyStateMachine)
    {
        Debug.Log("Ennemy ActiveState");
    }

    public void OnExit(EnnemyStateMachine ennemyStateMachine)
    {
        Debug.Log("Ennemy Exit ActiveState");
    }
}
