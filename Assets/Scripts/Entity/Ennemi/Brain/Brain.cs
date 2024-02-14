using UnityEngine;
using System.Collections;

public class Brain : MonoBehaviour
{
    public virtual IEnumerator EnemyPattern()
    {
        yield return null;
    }
}
