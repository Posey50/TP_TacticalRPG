using UnityEngine;

public class PlayButton : MonoBehaviour
{
    /// <summary>
    /// Called to launch the game.
    /// </summary>
    public void LaunchGame()
    {
        SceneManager.Instance.SwitchToGameScene();
    }
}
