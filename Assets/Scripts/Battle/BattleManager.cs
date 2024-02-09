using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    // Singleton
    private static BattleManager _instance = null;

    public static BattleManager Instance => _instance;

    /// <summary>
    /// Size of each team.
    /// </summary>
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
    /// Component that containes all steps of the battle.
    /// </summary>
    [field: SerializeField]
    private BattleSteps _battleSteps;

    private void Start()
    {
        // Prevents that the team size is greater than what is possible
        if (TeamsSize > GameManager.Instance.PlayableEntitiesInGame.Count)
        {
            TeamsSize = GameManager.Instance.PlayableEntitiesInGame.Count;
        }
    }

    /// <summary>
    /// CCalled to initialise a battle.
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitBattle()
    {
        _battleSteps.PlacePlayers();
        _battleSteps.PlaceEnnemies();

        yield return null;
    }

    /// <summary>
    /// Called when an entity has end her turn.
    /// </summary>
    public void NextEntityTurn()
    {
        
    }

    /// <summary>
    /// Called at the start of the battle and when every entities have end their turn.
    /// </summary>
    /// <returns></returns>
    private IEnumerator NewBattleTurn()
    {
        yield return null;
    }
}
