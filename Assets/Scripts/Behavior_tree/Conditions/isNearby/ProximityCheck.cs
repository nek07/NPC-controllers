using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Базовый класс для проверки близости
public abstract class ProximityCheck : MonoBehaviour
{
    protected bool IsNearbyByTag(string tag, float detectionRange = 2.0f)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    // Абстрактный метод для реакции
    public abstract void React();
}



