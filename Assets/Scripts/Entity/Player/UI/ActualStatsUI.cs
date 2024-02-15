using TMPro;
using UnityEngine;

public class ActualStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textActualMp;
    [SerializeField] private TextMeshProUGUI _textActualPa;

    void Start()
    {
        
    }

    public void NotifyUdatePA(int newPA)
    {
        _textActualPa.text = newPA.ToString();
    }
    
    public void NotifyUdateMP(int newMP)
    {
        _textActualMp.text = newMP.ToString();
    }
}
