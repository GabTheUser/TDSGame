using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInHandControl : MonoBehaviour
{
    public Weapon currentWeapon;
    public Transform weaponHolder; // Assign the parent object that holds the weapon children
    public int maximumWeapons;

    public Weapon[] weapons;
    private int currentWeaponIndex;
    private int currentWeaponsInHandAmount;
    private void Start()
    {
        // Ensure weaponHolder is assigned
        if (weaponHolder == null)
        {
            Debug.LogError("Weapon holder is not assigned to WeaponInHandControl!");
            return;
        }

        // Ensure there are child objects under weaponHolder
        if (weaponHolder.childCount == 0)
        {
            Debug.LogError("No child objects found under weaponHolder!");
            return;
        }

        // Initialize the weapons array
        weapons = new Weapon[weaponHolder.childCount];

        // Get all the Weapon components from the children of weaponHolder
        for (int i = 0; i < weaponHolder.childCount; i++)
        {
            weapons[i] = weaponHolder.GetChild(i).GetComponent<Weapon>();
        }

        // Set the initial current weapon
        SwitchWeapon(0);
    }

    public void ShootPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentWeapon.startShooting = true;
            currentWeapon.enabled = true;
        }
        else if (context.canceled)
        {
            currentWeapon.startShooting = false;
            currentWeapon.enabled = false;
        }
    }

    private void SwitchWeapon(int index)
    {
        // Ensure the index is within the valid range
        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogWarning("Invalid weapon index!");
            return;
        }

        // Disable the current weapon
        if (currentWeapon != null)
        {
            currentWeapon.startShooting = false;
            currentWeapon.enabled = false;
            currentWeapon.gameObject.SetActive(false);
        }

        // Set the new current weapon
        currentWeapon = weapons[index];

        // Enable the new current weapon
        if (currentWeapon != null)
        {
            currentWeapon.enabled = true;
            currentWeapon.gameObject.SetActive(true);
        }
    }

    public void SwitchToNextWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            SwitchWeapon(currentWeaponIndex);
        }
    }
}
