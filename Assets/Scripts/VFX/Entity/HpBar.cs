using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        _entity = GetComponent<Entity>();
    }

    

    private void OnDamageTaken(int newHealth)
    {
        _backHealthImage.color = _backDmgColor; 
        float targetFillAmount = Mathf.InverseLerp(0, /*_entity.EntityDatas.MaxHP*/ 25, newHealth);
        DOTween.Sequence()
            .Append(_frontHealthImage.DOFillAmount(targetFillAmount, _tweenDuration / 2f).SetEase(Ease.OutQuint))
            .AppendInterval(_tweenInterval)
            .Append(_backHealthImage.DOFillAmount(targetFillAmount, _tweenDuration).SetEase(Ease.OutQuint));
    }
    
    private void OnHealTaken(int newHealth)
    {
        _backHealthImage.color = _backHealColor;
        float targetFillAmount = Mathf.InverseLerp(0, 25, newHealth);
        DOTween.Sequence()
            .Append(_backHealthImage.DOFillAmount(targetFillAmount, _tweenDuration / 2f).SetEase(Ease.OutQuint))
            .AppendInterval(_tweenInterval)
            .Append(_frontHealthImage.DOFillAmount(targetFillAmount, _tweenDuration).SetEase(Ease.OutQuint));
    }
}
