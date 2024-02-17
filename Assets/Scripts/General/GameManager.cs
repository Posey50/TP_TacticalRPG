using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance = null;

    public static GameManager Instance => _instance;

    /// <summary>
    /// List of all instances of entity in the game.
    /// </summary>
    [field: SerializeField]
    public List<Entity> EntitiesInGame { get; set; } = new();

    /// <summary>
    /// List of all instances of entity as playable in the game.
    /// </summary>
    [field: SerializeField]
    public List<Entity> PlayableEntitiesInGame { get; set; } = new();

    /// <summary>
    /// List of all instances of entity as enemy in the game.
    /// </summary>
    [field: SerializeField]
    public List<Entity> EnemiesInGame { get; set; } = new();

    /// <summary>
    /// A value indicating that the game is over.
    /// </summary>
    public bool GameIsOver { get; private set; }

    /// <summary>
    /// A value indicating that the game is on pause.
    /// </summary>
    public bool GameIsOnPause { get; private set; }

    // Observer
    public delegate void GameProgressDelegate();

    public event GameProgressDelegate GameOverEvent;

    public event GameProgressDelegate GamePauseEvent;

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
    /// Called to stop the game.
    /// </summary>
    public void GameOver(bool hasWon)
    {
        GameIsOver = true;
        GameOverEvent?.Invoke();

        if (hasWon)
        {
            SceneManager.Instance.SwitchToVictoryScene();
        }
        else
        {
            SceneManager.Instance.SwitchToDefeatScene();
        }
    }

    /// <summary>
    /// Called to pause the game.
    /// </summary>
    public void GamePause()
    {
        GameIsOnPause = true;
        GamePauseEvent?.Invoke();
    }
}
