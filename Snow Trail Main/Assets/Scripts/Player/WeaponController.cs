using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using TMPro;
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
    public GameObject muzzleFlash, bulletImpactGraphic;
    public CameraShake cameraShake;
    public float cameraShakeMagnitude, cameraShakeDuration;
    public TextMeshProUGUI ammunitionInformationText;

    private void Awake() 
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update() 
    {
        Weaponinput();

        // Set ammo text
        ammunitionInformationText.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void Weaponinput()
    {
        // Determine if the weapon is fully automatic or not
        if (allowButtonHold)
            shooting = Input.GetKey (KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown (KeyCode.Mouse0);

        // Allow reload if player is using less than the mag size and not already reloading
        if (Input.GetKeyDown (KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        // Fire if ready to shoot is true
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
            //Debug.Log (raycastHit.collider.name);

            if (raycastHit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Hit enemy");
                Destroy(raycastHit.transform.gameObject);
                //raycastHit.collider.GetComponent<Enemy>().TakeDamage(damage);
            }
            
        }

        // Shake the camera
        cameraShake.Shake (cameraShakeDuration, cameraShakeMagnitude);

        // Graphics
        //var bulletImpactClone = (GameObject) Instantiate (bulletImpactGraphic, raycastHit.point, Quaternion.Euler(0, 180, 0));
        var muzzleFlashClone = (GameObject) Instantiate (muzzleFlash, attackPoint.position, attackPoint.rotation);

        bulletsLeft--;
        bulletsShot--;
        
        //Destroy (bulletImpactClone, 0.5f);
        Destroy (muzzleFlashClone, 0.5f);

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