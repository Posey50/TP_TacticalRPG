using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMain : Entity
{
    public PlayerInput PlayerInput { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public Pointer Pointer { get; private set; }

    [SerializeField]
    private EntityData EntityData;

    private void Start()
    {
        PlayerInput = GetComponent<PlayerInput>();
        StateMachine = GetComponent<PlayerStateMachine>();
        Pointer = GetComponent<Pointer>();

        base.EntityData = EntityData;

        Pointer.SetStartSquare(SquareUnderTheEntity);
    }

    /// <summary>
    /// Resets the players Movement and Action point at the end of its turn
    /// </summary>
    public override void ResetPoints()
    {
        MPs = EntityData.MPs;
        APs = EntityData.APs;
    }
}
