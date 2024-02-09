using UnityEngine;

public class MainNameEnnemy : MonoBehaviour, IComportement
{
    private EntityData _entityData;
    public void ChoseAnAction()
    {

    }
}

/* 
 choix du joueur le + proche
 déplacement vers lui

 si il est assez proche --> attaque
        choix de l'attaque dans l'actionPool
            --> attaque à distance
        ou
            --> attaque au corps à corps

    si il à encore des points d'actions --> attaque à nouveau
            --> attaque à distance
        ou
            --> attaque au corps à corps

 sinon fin du tour

 */

