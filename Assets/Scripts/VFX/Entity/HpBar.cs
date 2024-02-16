using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image _frontHealthImage;
    [SerializeField] private Image _backHealthImage;
    [SerializeField] private Color _backHealColor;
    [SerializeField] private Color _backDmgColor;
    private Entity _entity;
    [SerializeField] private float _tweenDuration;
    [SerializeField] private float _tweenInterval;
    [SerializeField] private TextMeshProUGUI _txtDmgHeal;

    private void Start()
    {
        _txtDmgHeal.gameObject.SetActive(false);
        _entity = GetComponent<Entity>();
        _entity.DamageRecieved += OnDamageTaken;
        _entity.HealRecieved += OnHealTaken;
    }

    

    private void OnDamageTaken(int dmgTaken)
    {
        _backHealthImage.color = _backDmgColor; 
        float targetFillAmount = Mathf.InverseLerp(0, _entity.EntityDatas.MaxHP, _entity.HP);
        DOTween.Sequence()
            .Append(_frontHealthImage.DOFillAmount(targetFillAmount, _tweenDuration / 2f).SetEase(Ease.OutQuint))
            .AppendInterval(_tweenInterval)
            .Append(_backHealthImage.DOFillAmount(targetFillAmount, _tweenDuration).SetEase(Ease.OutQuint));
        StartCoroutine(SetTxtDmg(dmgTaken));
    }
    
    private void OnHealTaken(int healTaken)
    {
        _backHealthImage.color = _backHealColor;
        float targetFillAmount = Mathf.InverseLerp(0, _entity.EntityDatas.MaxHP, _entity.HP);
        DOTween.Sequence()
            .Append(_backHealthImage.DOFillAmount(targetFillAmount, _tweenDuration / 2f).SetEase(Ease.OutQuint))
            .AppendInterval(_tweenInterval)
            .Append(_frontHealthImage.DOFillAmount(targetFillAmount, _tweenDuration).SetEase(Ease.OutQuint));
        StartCoroutine(SetTxtHeal(healTaken));
    }

    private IEnumerator SetTxtDmg(int dmg)
    {
        _txtDmgHeal.gameObject.SetActive(true);
        _txtDmgHeal.color = _backDmgColor;
        _txtDmgHeal.SetText("- " + dmg);
        yield return new WaitForSecondsRealtime(2f);
        _txtDmgHeal.gameObject.SetActive(false);
    }
    private IEnumerator SetTxtHeal(int heal)
    {
        _txtDmgHeal.gameObject.SetActive(true);
        _txtDmgHeal.color = _backHealColor;
        _txtDmgHeal.SetText("- " + heal);
        yield return new WaitForSecondsRealtime(2f);
        _txtDmgHeal.gameObject.SetActive(false);
    }


    public void Test()
    {
        StartCoroutine(SetTxtHeal(15));
    }
}
