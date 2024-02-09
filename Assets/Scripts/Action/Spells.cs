using UnityEngine;

public class Spells : MonoBehaviour
{
    /// <summary>
    /// Datas of the spell.
    /// </summary>
    [SerializeField] private SpellsData _actionBase;

    /// <summary>
    /// Gets datas of the spell.
    /// </summary>
    public SpellsData ActionBase
    { get { return _actionBase; } private set { } }
}
