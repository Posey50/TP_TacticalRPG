using DG.Tweening;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    /// <summary>
    /// Main component of the entity.
    /// </summary>
    private Entity _entity;

    /// <summary>
    /// Sprite renderer of the entity.
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Offset to be on the ground.
    /// </summary>
    public Vector3 YOffset { get; private set; }

    private void Start()
    {
        _entity = GetComponent<Entity>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _entity.Initialised += InitialiseSprite;
        _entity.TakeDamages += TakeDamages;
        _entity.IsHeal += IsHeal;
        _entity.IsAttacking += IsAttacking;
        _entity.MovingTo += IsMovingTo;
    }

    /// <summary>
    /// Called to initialise the sprite offset.
    /// </summary>
    private void InitialiseSprite()
    {
        YOffset = new Vector3(0, (_entity.SquareUnderTheEntity.GetComponent<Collider>().bounds.size.y / 2f) + (_spriteRenderer.bounds.size.y / 6f), 0);
        transform.position += YOffset;
    }

    /// <summary>
    /// Called when the entity takes damages.
    /// </summary>
    private void TakeDamages()
    {
        DOTween.Sequence()
            .Append(_spriteRenderer.DOColor(Color.red, 0.1f))
            .AppendInterval(0.05f)
            .Append(_spriteRenderer.DOColor(Color.white, 0.1f));
    }

    /// <summary>
    /// Called when the entity is heal.
    /// </summary>
    private void IsHeal()
    {
        DOTween.Sequence()
            .Append(_spriteRenderer.DOColor(Color.green, 0.1f))
            .AppendInterval(0.05f)
            .Append(_spriteRenderer.DOColor(Color.white, 0.1f));
    }

    /// <summary>
    /// Called when the entity is attacking.
    /// </summary>
    /// <param name="entity"> Entity to attack. </param>
    private void IsAttacking(Entity entity)
    {
        if (entity != null)
        {
            OrientateSprite(entity.transform.position);
        }
    }

    /// <summary>
    /// Called when the entity is moving.
    /// </summary>
    /// <param name="square"> Square to go to. </param>
    private void IsMovingTo(Square square)
    {
        if (square != null)
        {
            OrientateSprite(square.transform.position);
        }
    }

    /// <summary>
    /// Called to orientate the sprite on left or right.
    /// </summary>
    /// <param name="position"></param>
    private void OrientateSprite(Vector3 position)
    {
        Vector3 entityPos = transform.position;

        if ((position.x <= entityPos.x && position.z <= entityPos.z) || (position.z <= entityPos.z && position.x >= entityPos.x))
        {
            _spriteRenderer.flipX = true;
        }
        else if ((position.x <= entityPos.x && position.z >= entityPos.z) || (position.z >= entityPos.z && position.x >= entityPos.x))
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }
}
