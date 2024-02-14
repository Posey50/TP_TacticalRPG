using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    /// <summary>
    /// Spell currently selected.
    /// </summary>
    [field: SerializeField]
    public Spell SelectedSpell { get; set; } = new ();

    /// <summary>
    /// Current range where the player can attack.
    /// </summary>
    public List<Square> CurrentRange {  get; set; }

    /// <summary>
    /// Main component of the playable entity.
    /// </summary>
    private PlayerMain _playerMain;

    private void Start()
    {
        SelectedSpell = null;
        _playerMain = GetComponent<PlayerMain>();

        _playerMain.TurnIsEnd += UnselectSpell;
    }

    /// <summary>
    /// Called when a new spell is selected.
    /// </summary>
    /// <param name="spell"></param>
    public void UpdateSelectedSpell(Spell spell)
    {
        if (!_playerMain.IsMoving)
        {
            UnselectSpell();

            SelectedSpell = spell;

            _playerMain.Cursor.UnselectSquareForPath();

            CurrentRange = RangeManager.Instance.CalculateSimpleRange(_playerMain.SquareUnderTheEntity, spell.SpellDatas.Range);

            HighlightGroundManager.Instance.ShowRange(CurrentRange);
        }
    }

    /// <summary>
    /// Called to unselect the current selected spell.
    /// </summary>
    public void UnselectSpell()
    {
        SelectedSpell = null;

        HighlightGroundManager.Instance.HideRange();
    }
}
