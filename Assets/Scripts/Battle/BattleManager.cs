using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // Singleton
    private static BattleManager _instance = null;

    public static BattleManager Instance => _instance;

    /// <summary>
    /// Size of each team.
    /// </summary>
    [field: SerializeField]
    public int TeamsSize { get; private set; }

    /// <summary>
    /// List of all entities controlled by the player.
    /// </summary>
    public List<Entity> PlayableEntitiesInBattle { get; set; }

    /// <summary>
    /// List of all enemies in battle.
    /// </summary>
    public List<Entity> EnemiesInBattle { get; set; }

    /// <summary>
    /// List of entities in their order of action.
    /// </summary>
    public List<Entity> EntitiesInActionOrder { get; set; }

    /// <summary>
    /// List of squares where enemies will spawn.
    /// </summary>
    public List<Square> EnemiesSquares { get; set; }

    /// <summary>
    /// List of squares where playable entities will spawn.
    /// </summary>
    public List<Square> PlayerSquares { get; set; }

    /// <summary>
    /// The current active entity in the battle.
    /// </summary>
    public Entity CurrentActiveEntity { get; set; }

    /// <summary>
    /// Component that containes all steps of the battle.
    /// </summary>
    [field: SerializeField]
    private BattleSteps _battleSteps;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
    }

    /// <summary>
    /// CCalled to initialise a battle.
    /// </summary>
    /// <returns></returns>
    public void InitBattle()
    {
        // Prevents that the team size is greater than what is possible
        if (TeamsSize > GameManager.Instance.PlayableEntitiesInGame.Count)
        {
            TeamsSize = GameManager.Instance.PlayableEntitiesInGame.Count;
        }

        _battleSteps.PlacePlayers();
        _battleSteps.PlaceEnnemies();

        NewBattleTurn();
    }

    /// <summary>
    /// Called when an entity has end her turn.
    /// </summary>
    public void NextEntityTurn()
    {
        if (EntitiesInActionOrder[0].TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
        {
            playerStateMachine.ChangeToActive();
        }
        else if (EntitiesInActionOrder[0].TryGetComponent<EnemyStateMachine>(out EnemyStateMachine ennemyStateMachine))
        {
            // TODO
            //ennemyStateMachine.ChangeToActive();
        }
    }

    /// <summary>
    /// Called at the start of the battle and when every entities have end their turn.
    /// </summary>
    /// <returns></returns>
    private void NewBattleTurn()
    {
        EntitiesInActionOrder.Clear();
        EntitiesInActionOrder.AddRange(PlayableEntitiesInBattle);
        EntitiesInActionOrder.AddRange(EnemiesInBattle);
        EntitiesInActionOrder = _battleSteps.DeterminesOrder(EntitiesInActionOrder);

        NextEntityTurn();
    }
}
