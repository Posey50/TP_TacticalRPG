using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// The database which represents this entity.
    /// </summary>
    public EntityDatas EntityDatas { get; protected set; }

    /// <summary>
    /// Name of the entity.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Class of the entity
    /// </summary>
    public string Class { get; protected set; }

    /// <summary>
    /// Movement points which determines the number of movement that an entity cans do in one turn.
    /// </summary>
    [field: SerializeField]
    public int MP { get; protected set; }

    /// <summary>
    /// Action points which determines the total ammount of action points that an entity can use in one turn.
    /// </summary>
    [field: SerializeField]
    public int AP { get; protected set; }

    /// <summary>
    /// Health points of the entity.
    /// </summary>
    [field: SerializeField]
    public int HP { get; protected set; }

    /// <summary>
    /// Speed of the entity which determines the order of action in a turn.
    /// </summary>
    public int Speed { get; protected set; }

    /// <summary>
    /// List of spells that the entity can use.
    /// </summary>
    public List<Spell> Spells { get; protected set; } = new ();

    /// <summary>
    /// Square on which the entity is located.
    /// </summary>
    [field: SerializeField]
    public Square SquareUnderTheEntity { get; set; }

    /// <summary>
    /// A value indicating if the entity is moving.
    /// </summary>
    public bool IsMoving { get; private set; }

    /// <summary>
    /// Speed at which the entity moves.
    /// </summary>
    protected float _moveSpeed;

    /// <summary>
    /// Manager of the sprite of the entity.
    /// </summary>
    private SpriteManager _spriteManager;

    // Observer
    public delegate void EntityActionsDelegate();

    public event EntityActionsDelegate Initialised;
    public event EntityActionsDelegate Moved;
    public event EntityActionsDelegate StopMoved;
    public event EntityActionsDelegate StartAttack;
    public event EntityActionsDelegate TakeDamages;
    public event EntityActionsDelegate IsHeal;
    public event EntityActionsDelegate TurnIsEnd;
    public event EntityActionsDelegate IsDead;

    public delegate void MovingDatasDelegate(Square square);

    public event MovingDatasDelegate MovingTo;

    public delegate void AttackingDatasDelegate(Entity entity);

    public event AttackingDatasDelegate IsAttacking;

    public delegate void DatasDelegate(int datas);

    public event DatasDelegate DamageReceived;
    public event DatasDelegate HealReceived;
    public event DatasDelegate MPChanged;
    public event DatasDelegate APChanged;

    /// <summary>
    /// Called to hydrate the entity with their datas.
    /// </summary>
    public void InitEntity()
    {
        _spriteManager = GetComponent<SpriteManager>();

        Name = EntityDatas.Name;
        Class = EntityDatas.Class;
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;
        HP = EntityDatas.MaxHP;
        Speed = EntityDatas.Speed;
        Spells = EntityDatas.Spells;
        _moveSpeed = EntityDatas.MoveSpeed;

        // Anounces that the entity is initialised
        Initialised?.Invoke();

        MPChanged?.Invoke(MP);
        APChanged?.Invoke(AP);
    }

    /// <summary>
    /// Makes the entity following a path.
    /// </summary>
    /// <param name="path"> Path to follow. </param>
    public async UniTask FollowThePath(List<Square> path)
    {
        if(path != null)
        {
            IsMoving = true;

            // Anounces that the entity is moving
            Moved?.Invoke();
            MovingTo?.Invoke(path[path.Count - 1]);

            SquareUnderTheEntity.LeaveSquare();

            Vector3[] pathToFollow = AStarManager.Instance.ConvertSquaresIntoPositions(path).ToArray();

            // Raises the path
            for (int i = 0; i < pathToFollow.Length; i++)
            {
                pathToFollow[i] += _spriteManager.YOffset;
            }

            await transform.DOPath(pathToFollow, _moveSpeed * pathToFollow.Length, PathType.Linear, PathMode.Full3D).SetEase(Ease.Linear).OnWaypointChange((int i) => { if (i > 0) path[i - 1].ResetColor(); }).AsyncWaitForCompletion();

            DecreaseMP(path.Count);

            SquareUnderTheEntity = path[^1];
            SquareUnderTheEntity.SetEntity(this);

            // Anounces that the entity is not anymore moving
            StopMoved?.Invoke();

            IsMoving = false;
        }
    }

    /// <summary>
    /// Decreases MP of the entity by the amount given.
    /// </summary>
    /// <param name="amount"> MP to decrease. </param>
    public void DecreaseMP(int amount)
    {
        if (amount <= MP)
        {
            MP -= amount;

            // Anounces that MP has changed
            MPChanged?.Invoke(MP);
        }
    }

    /// <summary>
    /// Decreases AP of the entity by the amount given.
    /// </summary>
    /// <param name="amount"> AP to decrease. </param>
    public void DecreaseAP(int amount)
    {
        if (amount <= AP)
        {
            AP -= amount;

            // Anounces that AP has changed
            APChanged?.Invoke(AP);
        }
    }

    /// <summary>
    /// Attack an entity with the spell given.
    /// </summary>
    /// <param name="spell"> Spell used. </param>
    /// <param name="entityToAttack"> Entity to attack. </param>
    public void Attack(Spell spell, Entity entityToAttack)
    {
        // Anounces that the entity is attacking
        StartAttack?.Invoke();

        entityToAttack.TakeAttack(spell);

        DecreaseAP(spell.SpellDatas.PaCost);
    }

    /// <summary>
    /// Recieves a spell and does the the effect of the spell.
    /// </summary>
    /// <param name="spellReceived"> Spell received by the entity. </param>
    public void TakeAttack(Spell spellReceived)
    {
        if (spellReceived.SpellDatas.Type == Type.heal)
        {
            HealHP(spellReceived.SpellDatas.Damages);
        }
        else
        {
            TakeDamage(spellReceived.SpellDatas.Damages);
        }
    }

    /// <summary>
    /// Applies damages of a spell.
    /// </summary>
    /// <param name="damages"> Damages to take. </param>
    public void TakeDamage(int damages)
    {
        HP -= damages;

        // Anounces that the entity has taken damages
        TakeDamages?.Invoke();
        DamageReceived?.Invoke(damages);

        if (HP <= 0)
        {
            HP = 0;

            // Anounces that entity is dead
            IsDead?.Invoke();
            BattleManager.Instance.EntityDeath(this);
        }
    }

    /// <summary>
    /// Applies the healing of a spell.
    /// </summary>
    /// <param name="heal"> HP to heal. </param>
    public void HealHP(int heal)
    {
        // Prevents the healing over the maximum of HP
        HP = Mathf.Clamp(HP + heal, 0, EntityDatas.MaxHP);

        // Anounces that the entity has been heal
        IsHeal?.Invoke();
        HealReceived?.Invoke(heal);
    }
    
    /// <summary>
    /// Resets MP and AP of the entity.
    /// </summary>
    public void ResetPoints()
    {
        MP = EntityDatas.MP;
        AP = EntityDatas.AP;

        // Anounces that MP and AP have changed
        MPChanged?.Invoke(MP);
        APChanged?.Invoke(AP);
    }

    /// <summary>
    /// Called when the entity has end its turn.
    /// </summary>
    public void EndOfTheTurn()
    {
        // Anounces that the turn is ended
        TurnIsEnd?.Invoke();

        BattleManager battleManager = BattleManager.Instance;

        battleManager.EntitiesInActionOrder.Remove(this);
        battleManager.NextEntityTurn();
        ResetPoints();
    }
}
