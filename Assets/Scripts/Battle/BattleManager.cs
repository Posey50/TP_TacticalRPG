using UnityEngine;
using System.Collections.Generic;
using System;

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
    public List<Entity> PlayableEntitiesInBattle { get; set; } = new();

    /// <summary>
    /// List of all enemies in battle.
    /// </summary>
    public List<Entity> EnemiesInBattle { get; set; } = new();

    /// <summary>
    /// List of entities in their order of action.
    /// </summary>
    public List<Entity> EntitiesInActionOrder { get; set; } = new ();

    /// <summary>
    /// List of squares where enemies will spawn.
    /// </summary>
    [field: SerializeField]
    public List<Square> EnemiesSquares { get; set; } = new ();

    /// <summary>
    /// List of squares where playable entities will spawn.
    /// </summary>
    [field: SerializeField]
    public List<Square> PlayerSquares { get; set; } = new ();

    /// <summary>
    /// The current active entity in the battle.
    /// </summary>
    [field: SerializeField]
    public Entity CurrentActiveEntity { get; set; }

    /// <summary>
    /// Event to Update the UI entities order.
    /// </summary>
    public event Action<List<Entity>> UpadateUIEntitiesActionOrder;

    /// <summary>
    /// Component that containes all steps of the battle.
    /// </summary>
    private BattleSteps _battleSteps;

    // Observer
    public delegate void BattleManagerDelegate();

    public event BattleManagerDelegate AllEntitiesInit;

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

        _battleSteps = GetComponent<BattleSteps>();

        _battleSteps.PlacePlayers();
        _battleSteps.PlaceEnnemies();

        // Anounces that all entities in battle are initialised
        AllEntitiesInit?.Invoke();

        NewBattleTurn();
    }

    /// <summary>
    /// Called when an entity has end her turn.
    /// </summary>
    public void NextEntityTurn()
    {
        if (EntitiesInActionOrder.Count > 0)
        {
            UpadateUIEntitiesActionOrder?.Invoke(EntitiesInActionOrder);

            CurrentActiveEntity = EntitiesInActionOrder[0];

            if (EntitiesInActionOrder[0].TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
            {
                playerStateMachine.ChangeState(playerStateMachine.ActiveState);
            }
            else if (EntitiesInActionOrder[0].TryGetComponent<EnemyStateMachine>(out EnemyStateMachine enemyStateMachine))
            {
                enemyStateMachine.ChangeState(enemyStateMachine.ActiveState);
            }
        }
        else
        {
            NewBattleTurn();
        }
    }

    /// <summary>
    /// Called to end the turn of the current active entity.
    /// </summary>
    public void EndOfTheCurrentEntityTurn()
    {
        CurrentActiveEntity.EndOfTheTurn();
    }

    /// <summary>
    /// Called when an entity dies and removes the entity from the battle.
    /// </summary>
    /// <param name="deadEntity"> The dead entity to remove. </param>
    public void EntityDeath(Entity deadEntity)
    {
        // Removes entity from datas of the battle
        if (PlayableEntitiesInBattle.Contains(deadEntity))
        {
            PlayableEntitiesInBattle.Remove(deadEntity);
        }
        else if (EnemiesInBattle.Contains(deadEntity))
        {
            EnemiesInBattle.Remove(deadEntity);
        }

        if (EntitiesInActionOrder.Contains(deadEntity))
        {
            EntitiesInActionOrder.Remove(deadEntity);
        }

        // Resets the square under the entity
        deadEntity.SquareUnderTheEntity.LeaveSquare();
        deadEntity.SquareUnderTheEntity.ResetColor();

        // Desactive the entity
        deadEntity.gameObject.SetActive(false);
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
        
        UpadateUIEntitiesActionOrder?.Invoke(EntitiesInActionOrder);

        NextEntityTurn();
    }
}
