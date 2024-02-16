using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    [SerializeField] private bool _active;
    [SerializeField] private GameObject _gameObject;

    // Update is called once per frame
    void Update()
    {
        if (!_active)
        {
            _gameObject.SetActive(false);
        }
        else
        {
            _gameObject.SetActive(true);
        }
    }
}
