using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    /// <summary>
    /// Called to launch the menu.
    /// </summary>
    public void LaunchMenu()
    {
        SceneManager.Instance.SwitchToMenuScene();
    }
}
