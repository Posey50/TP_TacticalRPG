using UnityEngine;

public class PlayerMain : Entity
{
    public EntityData EntityData;
    public PlayerStateMachine StateMachine;

    public override void ResetPoints()
    {
        MPs = EntityData.MPs;
        APs = EntityData.APs;
    }
}
