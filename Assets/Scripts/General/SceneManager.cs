using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Singleton
    private static SceneManager _instance = null;

    public static SceneManager Instance => _instance;

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

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called to switch to the title scene.
    /// </summary>
    public void SwitchToTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Called to switch to the menu scene.
    /// </summary>
    public void SwitchToMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Called to switch to the game scene.
    /// </summary>
    public void SwitchToGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Called to switch to the victory scene.
    /// </summary>
    public void SwitchToVictoryScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }

    /// <summary>
    /// Called to switch to the defeat scene.
    /// </summary>
    public void SwitchToDefeatScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(4);
    }
}
