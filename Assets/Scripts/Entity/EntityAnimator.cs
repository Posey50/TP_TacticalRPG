using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    /// <summary>
    /// The Entity
    /// </summary>
    private Entity _entity;

    /// <summary>
    /// The Entity's animator
    /// </summary>
    private Animator _anim;

    void Start()
    {
        _entity = GetComponent<Entity>();
        _anim = GetComponent<Animator>();

        _entity.IsMove += Walk;
        _entity.StartAttack += Attack;
        _entity.DamageRecieved += Hit;

    }

    private void Walk(bool isMoving)
    {
        if (isMoving)
        {
            Debug.Log("Move");
            _anim.SetTrigger("EntityStartMoving");
        }
        else
        {
            Debug.Log("Idle");
            _anim.SetTrigger("EntityStopMoving");
        }
    }

    private void Attack()
    {
        Debug.Log("Attack");
        _anim.SetTrigger("EntityAttack");
    }

    private void Hit(int i)
    {
        Debug.Log("Hit");
        _anim.SetTrigger("EntityHit");
    }

    private void Dies()
    {
        _anim.SetTrigger("EntityDies");
    }
}
