using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Square departure;

    public Square arrival;

    public int minRange;

    public int maxRange;

    public Material material;

    private void Start()
    {
        StartCoroutine(Wait());
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);

        List<Square> list = AStarManager.Instance.CalculateShortestPathForAMovement(departure, arrival);

        for (int i = 0; i < list.Count; i++)
        {
            list[i].GetComponent<MeshRenderer>().material = material;
        }
    }
}
