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

