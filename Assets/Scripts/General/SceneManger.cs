using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManger : MonoBehaviour
{
    [SerializeField] private string _titleSceneName;
    [SerializeField] private string _menuSceneName;
    [SerializeField] private string _victorySceneName;
    [SerializeField] private string _defeatSceneName;
    [SerializeField] private string _playSceneName;

    public void OnSwitchTitle()
    {
        SceneManager.LoadScene(_titleSceneName);
    }
    public void OnSwitchPlay()
    {
        SceneManager.LoadScene(_playSceneName);
    }
    public void OnSwitchMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
    }
    public void OnSwitchVictory()
    {
        SceneManager.LoadScene(_victorySceneName);
    }
    public void OnSwitchDefeat()
    {
        SceneManager.LoadScene(_defeatSceneName);
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}
