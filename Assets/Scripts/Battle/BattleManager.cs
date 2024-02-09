using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // Singleton
    private static BattleManager _instance = null;

    public static BattleManager Instance => _instance;

    /// <summary>
    /// List of all entities controlled by the player.
    /// </summary>
    public List<Entity> PlayableEntitiesInBattle { get; set; }

    /// <summary>
    /// List of all ennemies in battle.
    /// </summary>
    public List<Entity> EnnemiesInBattle { get; set; }

    /// <summary>
    /// List of entities in their order of action.
    /// </summary>
    public List<Entity> EntitiesInActionOrder { get; set; }

    /// <summary>
    /// List of squares where ennemies will spawn.
    /// </summary>
    public List<Square> EnnemiesSquares { get; set; }

    /// <summary>
    /// List of squares where playable entities will spawn.
    /// </summary>
    public List<Square> PlayerSquares { get; set; }
}
