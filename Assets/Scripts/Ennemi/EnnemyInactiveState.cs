using UnityEngine;

public class EnnemyInactiveState : MonoBehaviour, IEnnemyState
{
    public void OnEnter(EnnemyStateMachine ennemyStateMachine)
    {
        Debug.Log("Ennemy InactiveState");
    }

    public void OnExit(EnnemyStateMachine ennemyStateMachine)
    {
        Debug.Log("Ennemy Exit InactiveState");
    }
}
