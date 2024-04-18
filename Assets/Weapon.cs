using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage;
    [SerializeField] private ObjectPool poolOfBullets;
    [SerializeField] private Transform transformShootPoint;
    public float cooldownTime = 0.5f; // Adjust the cooldown time as needed
    public float reloadTime = 2.0f; // Adjust the reload time as needed
    public int maxAmmo = 10; // Adjust the maximum ammo count
    public bool startShooting;
    private int currentAmmo;
    private float lastShootTime;
    private bool isReloading;

    private void Start()
    {
        currentAmmo = maxAmmo;
        lastShootTime = -cooldownTime; // To allow shooting immediately
    }

    private void Update()
    {
        if (startShooting)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Check if there is enough ammo and if enough time has passed since the last shot
        if (currentAmmo > 0 && Time.time - lastShootTime > cooldownTime)
        {
            // Shoot a bullet
            poolOfBullets.GetProjectile(transformShootPoint.position, transformShootPoint.rotation);

            // Update the last shoot time
            lastShootTime = Time.time;

            // Decrease ammo count
            currentAmmo--;
        }
        else if (currentAmmo <= 0 && !isReloading)
        {
            // Handle reloading when ammo is empty and not already reloading
            Reload();
        }
    }

    private void Reload()
    {
        // Set the reloading flag to true
        isReloading = true;

        // Reload logic here
        // For simplicity, we'll set the ammo to the maxAmmo count after a delay
        Invoke("CompleteReload", reloadTime);
    }

    private void CompleteReload()
    {
        // Reset the reloading flag
        isReloading = false;

        // Set the ammo to the maxAmmo count
        currentAmmo = maxAmmo;
    }
}
