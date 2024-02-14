using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEntitiesActionOrder : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _listTextEntitesOrder;
    public void Start()
    {
        
    }

    /// <summary>
    /// Update text of  entities list in their order of action.
    /// </summary>
    public void UpdateUIEntitiesOrder()
    {
        for (int i = 0; i < _listTextEntitesOrder.Count; i++)
        {
            _listTextEntitesOrder[i].text = i + BattleManager.Instance.EntitiesInActionOrder[i].Name.ToString();
        }
    }
}
