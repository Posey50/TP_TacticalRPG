using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEntitiesActionOrder : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _listTextEntitesOrder;
    public void Start()
    {
        BattleManager.Instance.UpadateUIEntitiesActionOrder += Notify;
    }

    /// <summary>
    /// Update text of  entities list in their order of action.
    /// </summary>
    public void Notify(List<Entity> entities)
    {
        for (int i = 0; i < _listTextEntitesOrder.Count; i++)
        {
            _listTextEntitesOrder[i].text = (i + 1) + " " + entities[i].Name.ToString();
        }
    }
}