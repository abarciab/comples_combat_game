using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityCoordinator : MonoBehaviour
{
    public UnityEvent OnTakeDamage;
    [SerializeField] bool testDamage;

    private void Update()
    {
        if (testDamage) {
            testDamage = false;
            OnTakeDamage.Invoke();
        }
    }

}
