using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMain : Entity
{
    /// <summary>
    /// Player input component of the playable entity.
    /// </summary>
    public PlayerInput PlayerInput { get; private set; }

    /// <summary>
    /// Player state machine of the playable entity.
    /// </summary>
    public PlayerStateMachine StateMachine { get; private set; }

    /// <summary>
    /// Cursor component of the playable entity.
    /// </summary>
    public Cursor Cursor { get; private set; }

    /// <summary>
    /// Actions component of the playable entity.
    /// </summary>
    public Actions Actions { get; private set; }

    /// <summary>
    /// Datas of the playable entity.
    /// </summary>
    [SerializeField]
    private new EntityDatas EntityDatas;

    private void Start()
    {
        // Entity declares itself to the game manager.
        GameManager.Instance.EntitiesInGame.Add(this);
        GameManager.Instance.PlayableEntitiesInGame.Add(this);

        // Get all components on the entity
        PlayerInput = GetComponent<PlayerInput>();
        StateMachine = GetComponent<PlayerStateMachine>();
        Cursor = GetComponent<Cursor>();
        Actions = GetComponent<Actions>();

        // Set the entity datas with database given
        base.EntityDatas = this.EntityDatas;
    }
}
