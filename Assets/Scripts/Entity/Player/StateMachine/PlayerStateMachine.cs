using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    /// <summary>
    /// Main component of the playable entity.
    /// </summary>
    public PlayerMain PlayerMain { get; private set; }

    /// <summary>
    /// Manager of the battle.
    /// </summary>
    public BattleManager BattleManager { get; private set; }

    /// <summary>
    /// Manager of the spell buttons.
    /// </summary>
    public SpellButtonsManager SpellButtonsManager { get; private set; }

    /// <summary>
    /// Active state of the playable entity.
    /// </summary>
    public PlayerActiveState ActiveState = new();

    /// <summary>
    /// Inactive state of the playable entity.
    /// </summary>
    public PlayerInactiveState InactiveState = new();

    /// <summary>
    /// Current state of the playable entity.
    /// </summary>
    public IPlayerState CurrentState {  get; private set; }
  
    void Start()
    {
        PlayerMain = GetComponent<PlayerMain>();
        BattleManager = BattleManager.Instance;
        SpellButtonsManager = SpellButtonsManager.Instance;

        PlayerMain.TurnIsEnd += DesactiveEntity;

        // Sets state by default
        ChangeState(InactiveState);
    }

    /// <summary>
    /// Called to desactive the entity.
    /// </summary>
    private void DesactiveEntity()
    {
        ChangeState(InactiveState);
    }

    /// <summary>
    /// Called to change the current state.
    /// </summary>
    /// <param name="newState"> New state to set. </param>
    public void ChangeState(IPlayerState newState)
    {
        CurrentState?.OnExit(this);

        CurrentState = newState;

        CurrentState.OnEnter(this);
    }
}
