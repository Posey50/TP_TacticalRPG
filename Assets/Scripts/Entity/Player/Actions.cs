using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    /// <summary>
    /// Spell currently selected.
    /// </summary>
    [field: SerializeField]
    public Spell SelectedSpell { get; set; } = new();

    /// <summary>
    /// Current range where the player can attack.
    /// </summary>
    public List<Square> CurrentRange { get; set; }

    /// <summary>
    /// Main component of the playable entity.
    /// </summary>
    private PlayerMain _playerMain;

    // Event for the range
    public delegate void CurrentRangeDelegate(List<Square> newRange);

    public event CurrentRangeDelegate RangeChanged;

    private void Start()
    {
        SelectedSpell = null;
        _playerMain = GetComponent<PlayerMain>();

        _playerMain.StateMachine.ActiveState.RightClickPressed += UnselectSpell;
        _playerMain.TurnIsEnd += UnselectSpell;
    }

    /// <summary>
    /// Called when a new spell is selected.
    /// </summary>
    /// <param name="newSpell"></param>
    public void UpdateSelectedSpell(Spell newSpell)
    {
        if (!_playerMain.IsInAction)
        {
            // Unselects all in the cursor
            _playerMain.Cursor.UnselectAll();

            // Gets the new spell
            SelectedSpell = newSpell;

            if (SelectedSpell != null)
            {
                // Gets the new range
                CurrentRange = RangeManager.Instance.CalculateRange(_playerMain.SquareUnderTheEntity, newSpell.SpellDatas.MinRange, newSpell.SpellDatas.MaxRange);
            }
            else
            {
                CurrentRange = null;
            }

            // Anounces that the range has changed
            RangeChanged?.Invoke(CurrentRange);
        }
    }

    /// <summary>
    /// Called to unselect the current selected spell.
    /// </summary>
    public void UnselectSpell()
    {
        UpdateSelectedSpell(null);      
    }
}
