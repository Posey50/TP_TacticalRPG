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

        base._entityData = EntityData;

        Pointer.SetStartSquare(_currentSquare);
    }

    /// <summary>
    /// Resets the players Movement and Action point at the end of its turn
    /// </summary>
    public override void ResetPoints()
    {
        MPs = EntityData.MPs;
        APs = EntityData.APs;
    }

    /// <summary>
    /// Override of EntityMove to use the Cursor
    /// </summary>
    /// <param name="destination"></param>
    public override void Move(Square destination)
    {
        _currentSquare.LeaveSquare();

        _currentSquare = destination;
        _currentSquare.SetEntity(this);

        Pointer.SetStartSquare(_currentSquare);
        transform.position = _currentSquare.transform.position;
    }
}
