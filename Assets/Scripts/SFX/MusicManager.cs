using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Singleton
    private static MusicManager _instance = null;

    public static MusicManager Instance => _instance;

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
}
