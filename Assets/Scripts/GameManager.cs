using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance = null;

    public static GameManager Instance => _instance;

    /// <summary>
    /// List of all instances of entity in the game.
    /// </summary>
    public List<Entity> EntitiesInGame {  get; private set; }

    /// <summary>
    /// List of all instances of entity as playable in the game.
    /// </summary>
    public List<Entity> PlayableEntitiesInGame { get; private set; }

    /// <summary>
    /// List of all instances of entity as enemy in the game.
    /// </summary>
    public List<Entity> EnemiesInGame { get; private set; }

    /// <summary>
    /// A value indicating that the game is over.
    /// </summary>
    public bool GameIsOver { get; private set; }

    /// <summary>
    /// A value indicating that the game is on pause.
    /// </summary>
    public bool GameIsOnPause { get; private set; }

    /// <summary>
    /// Called to stop the game.
    /// </summary>
    public void GameOver()
    {
        GameIsOver = true;
    }

    /// <summary>
    /// Called to pause the game.
    /// </summary>
    public void GamePause()
    {
        GameIsOnPause = true;
    }
}
