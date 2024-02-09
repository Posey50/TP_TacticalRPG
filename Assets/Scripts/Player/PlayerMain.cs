using UnityEngine;

public class PlayerMain : Entity
{
    public EntityData EntityData;
    public PlayerStateMachine StateMachine;
    public Pointer Pointer;

    public override void ResetPoints()
    {
        MPs = EntityData.MPs;
        APs = EntityData.APs;
    }

    public override void Move(Square destination)
    {
        _currentSquare = destination;
        Pointer.startSquare = _currentSquare;
    }
}
