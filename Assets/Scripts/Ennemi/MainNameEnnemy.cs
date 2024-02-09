using System.Collections.Generic;
using UnityEngine;

public class MainNameEnnemy : MonoBehaviour, IComportement
{
    public Entity Entity;
    public EnnemyStateMachine EnemyStateMachine;

    public List<Square> path;
    public Square startSquare;
    public Material startMaterial;

    public Square Square;
    private int _minDistanceToPlayer;


    public void Start()
    {
        startSquare.GetComponent<MeshRenderer>().material = startMaterial;
    }
    public void ChosePlayer()
    {
        for (int i = 0; i < 3; i++)
        {
            path = AStarManager.Instance.CalculateShortestPathBetween(startSquare, Square /*� remplacer par EntityOnThisSquare*/); //--> renvoie liste de square entre le point A et B
            
            int distance = path.Count;

            if(distance < _minDistanceToPlayer)
            {
                _minDistanceToPlayer = distance;
            }
        }

        MPToPlayer();
    }

    public void MPToPlayer()
    {
        if(_minDistanceToPlayer > Entity.MPs)
        {
            _minDistanceToPlayer = Entity.MPs;
        }
        else if(_minDistanceToPlayer == Entity.MPs)
        {
            _minDistanceToPlayer--;
        }

        startSquare = path[_minDistanceToPlayer];

        Entity.DecreasePM(_minDistanceToPlayer);
        Entity.Move(startSquare);
    }
    public void ChoseAnAction()
    {
        //if(Square.Neighbors)
    }
}

/* 
 choix du joueur le + proche
 d�placement vers lui

 si il est assez proche --> attaque
        choix de l'attaque dans l'actionPool
            --> attaque � distance
        ou
            --> attaque au corps � corps

    si il � encore des points d'actions --> attaque � nouveau
            --> attaque � distance
        ou
            --> attaque au corps � corps

 sinon fin du tour

 */

