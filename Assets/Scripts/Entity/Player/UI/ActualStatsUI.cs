using TMPro;
using UnityEngine;

public class ActualStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textActualMp;
    [SerializeField] private TextMeshProUGUI _textActualAp;
    private Entity _entity;

    void Start()
    {
        _entity = GetComponent<Entity>();
        _entity.MPChanged += NotifyUdateMP;
        _entity.APChanged += NotifyUdatePA;
    }

    public void NotifyUdatePA(int newAP)
    {
        _textActualAp.text = "AP : " + newAP.ToString();
    }

    public void NotifyUdateMP(int newMP)
    {
        _textActualMp.text = "MP : " + newMP.ToString();
    }
}
