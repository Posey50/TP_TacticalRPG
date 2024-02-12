using UnityEngine;

public class SpellButtonsManager : MonoBehaviour
{
    // Singleton
    private static SpellButtonsManager _instance = null;

    public static SpellButtonsManager Instance => _instance;

    [field: SerializeField]
    public SpellButton[] SpellButtons { get; private set; }

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
}
