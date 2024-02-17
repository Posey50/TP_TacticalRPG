using UnityEngine;

public class EntityAnimator : MonoBehaviour
{
    /// <summary>
    /// Main component of the entity.
    /// </summary>
    private Entity _entity;

    /// <summary>
    /// Animator component of the entity.
    /// </summary>
    private Animator _animator;

    void Start()
    {
        _entity = GetComponent<Entity>();
        _animator = GetComponent<Animator>();

        _entity.StartMoving += Walk;
        _entity.StopMoving += StopWalk;
        _entity.StartAttacking += Attack;
        _entity.TakeDamages += Hit;
        _entity.IsDead += Dies;
    }

    /// <summary>
    /// Called to start entity walking.
    /// </summary>
    private void Walk()
    {
        _animator.SetTrigger("EntityStartMoving");
    }

    /// <summary>
    /// Called to stop entity walking.
    /// </summary>
    private void StopWalk()
    {
        _animator.SetTrigger("EntityStopMoving");
    }

    /// <summary>
    /// Called to make the entity attacking.
    /// </summary>
    private void Attack()
    {
        _animator.SetTrigger("EntityAttack");
    }

    /// <summary>
    /// Called to make the entity taking damages.
    /// </summary>
    private void Hit()
    {
        _animator.SetTrigger("EntityHit");
    }

    /// <summary>
    /// Called to make the entity diyng.
    /// </summary>
    private void Dies()
    {
        _animator.SetTrigger("EntityDies");
    }
}
