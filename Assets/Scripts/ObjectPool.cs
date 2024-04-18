using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int poolSize = 10;

    private List<GameObject> projectilePool;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        projectilePool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    public GameObject GetProjectile(Vector2 startPosition, Quaternion startRotation)
    {
        foreach (GameObject projectile in projectilePool)
        {
            if (!projectile.activeInHierarchy)
            {
                projectile.transform.position = startPosition;
                projectile.transform.rotation = startRotation;
                projectile.SetActive(true);
                return projectile;
            }
        }

        GameObject newProjectile = Instantiate(projectilePrefab);
        projectilePool.Add(newProjectile);
        return newProjectile;
    }
}
