using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // Gun stats
    public int damage;
    public int magazineSize, bulletsPerTap;
    int bulletsLeft, bulletsShot;

    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;

    public bool allowButtonHold;

    // Bools
    bool shooting, readyToShoot, reloading;

    // Reference
    public Camera playerCamera;
    public Transform attackPoint;
    public RaycastHit raycastHit;
    public LayerMask whatIsEnemy;

    // Graphics
    

    private void Awake() 
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update() 
    {
        Weaponinput();   
    }

    private void Weaponinput()
    {
        if (allowButtonHold)
            shooting = Input.GetKey (KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown (KeyCode.Mouse0);

        if (Input.GetKeyDown (KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Bullet spread
        // Can have it so if player is moving then spread = spread * 1.5f. Else, spread = "normal spread"
        float spreadOnX = Random.Range (-spread, spread);
        float spreadOnY = Random.Range (-spread, spread);

        // Calculate direction with spread
        Vector3 shootDirection = playerCamera.transform.forward + new Vector3(spreadOnX, spreadOnY, 0);

        // Raycast
        if (Physics.Raycast (playerCamera.transform.position, shootDirection, out raycastHit, range, whatIsEnemy))
        {
            Debug.Log (raycastHit.collider.name);

            //if (raycastHit.collider.CompareTag("Enemy"))
            //     raycastHit.collider.GetComponent<Enemy>().TakeDamage(damage);
        }

        bulletsLeft--;
        bulletsShot--;

        Invoke ("ResetShoot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke ("Shoot", timeBetweenShots);
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke ("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}