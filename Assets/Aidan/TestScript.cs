using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    EntityCoordinator entityScript;

    private void Start()
    {
        entityScript = GetComponent<EntityCoordinator>();
        if (entityScript == null) return;
        entityScript.OnTakeDamage.AddListener(TakeDamage);
    }

    void TakeDamage()
    {
        print("ow");
    }
}
