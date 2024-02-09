using System.Collections.Generic;
using UnityEngine;

public class MainNameEnnemy : MonoBehaviour, IComportement
{
    public EntityData EntityData;
    public Entity Entity;
    public EnnemyStateMachine EnemyStateMachine;

    public List<Square> path;
    public Square startSquare;
    public Material startMaterial;

    public Square Square;
    private int _minDistanceToPlayer;

    public bool CanAttack;
     


    public void Start()
    {
        startSquare.GetComponent<MeshRenderer>().material = startMaterial;
    }

    /// <summary>
    /// Choisi le joueur le + proche de l'ennemi
    /// </summary>
    public void ChosePlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            path = AStarManager.Instance.CalculateShortestPathBetween(startSquare, Square /*à remplacer par EntityOnThisSquare*/); //--> renvoie liste de square entre le point A et B

            int distance = path.Count;

            if (distance < _minDistanceToPlayer)
            {
                _minDistanceToPlayer = distance;
            }
        }

        MPToPlayer();
    }

    /// <summary>
    /// Check si ennemy MP > ou = à la distance du joueur le plus proche
    /// </summary>
    public void MPToPlayer()
    {
        if (_minDistanceToPlayer > EntityData.MPs)
        {
            _minDistanceToPlayer = EntityData.MPs;
        }
        else if (_minDistanceToPlayer == EntityData.MPs)
        {
            _minDistanceToPlayer--;
        }

        startSquare = path[_minDistanceToPlayer];

        Entity.DecreasePM(_minDistanceToPlayer);
        Entity.Move(startSquare);
    }


    /// <summary>
    /// Check si l'ennemi est à coté du joueur, si c'est le cas il choisi une attack random sinon termine le tour
    /// </summary>
    public void ChoseAnAction()
    {
        for (int i = 0; i <= Square.Neighbors.Count; i++)
        {
            if (path[_minDistanceToPlayer] = Square.Neighbors[i])
            {
                CanAttack = true;
            }
            else if(Square.Neighbors.Count == i)
            {
                BattleManager.Instance.NextEntityTurn();
            }
        }

        if (CanAttack)
        {
            for (int i = 0; i <= EntityData.Actions.Count; i++)
            {
                int randomAction = Random.Range(0, EntityData.Actions.Count);

                if (EntityData.Actions[randomAction].ActionBase.PaCost <= EntityData.APs)
                {
                    Debug.Log($"Ennemy effectue {EntityData.Actions[randomAction].ActionBase.Name}");
                    Entity.Attack(path[_minDistanceToPlayer]);
                    Entity.DecreasePA(EntityData.Actions[randomAction].ActionBase.PaCost);
                }
                else if(EntityData.Actions.Count == i)
                {
                    Debug.Log("Ennemy effectue n'a pu assez de PA");
                    BattleManager.Instance.NextEntityTurn();
                }
            }
        }
    }
}

