using UnityEngine;

public class EnemyMain : Entity
{
    /// <summary>
    /// State machine of the enemy.
    /// </summary>
    public EnemyStateMachine EnemyStateMachine { get; private set; }

    /// <summary>
    /// Brain of the enemy.
    /// </summary>
    public Brain Brain { get; private set; }

    /// <summary>
    /// Datas of the enemy entity.
    /// </summary>
    [SerializeField]
    private new EntityDatas EntityDatas;

    public void Start()
    {
        GameManager.Instance.EntitiesInGame.Add(this);
        GameManager.Instance.EnemiesInGame.Add(this);

        EnemyStateMachine = GetComponent<EnemyStateMachine>();
        Brain = GetComponent<Brain>();

        // Set the entity datas with database given
        base.EntityDatas = this.EntityDatas;
    }

    /// <summary>
    /// Called to makes the entity start reflecting.
    /// </summary>
    public void StartReflexion()
    {
        Brain.StartCoroutine(Brain.EnemyPattern());
    }
}