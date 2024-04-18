using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletBehaviour : MonoBehaviour
{
    public UnityEvent bulletTypeEvent;
    public int damage;
    public float distance, range, maxActiveTime;
    [SerializeField] private bool showZone;
    [SerializeField] private LayerMask whatIsHittable, whoToDamage;
    [SerializeField] private ProjectileMovement projectileMovement;
    [Header("Ricochet Bullets")]
    [SerializeField] private string destroyTag, ricochetTag; // used only in ricochet;
    public int maxReflection;
    [Header("Penetration Bullets")]
    public int penetrationCountMax;
    public float percentOfPenetrationMove;
    public GameObject insideMask;
    [Header("Explosives")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private bool blowUpIfNoEnemyFound = true;

    [HideInInspector] public Transform owner;
    private RaycastHit2D hitInfo;
    private GameObject whatIHit;
    private int penetrationCount, currentReflected;
    private float currentActiveTime;


    private void OnEnable()
    {
        currentActiveTime = maxActiveTime;
        penetrationCount = 0;
        projectileMovement.enabled = true;
        whatIHit = null;
    }

    private void Update()
    {
            currentActiveTime -= Time.deltaTime;
        if (currentActiveTime <= 0)
        {
            if (explosionEffect == null)
            {
                TurnOffBullet();
            }
        }
        bulletTypeEvent.Invoke();
    }

    public void SimpleBullet() // simple bullets
    {
        hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsHittable);
        if (hitInfo.collider != null)
        {
            whatIHit = hitInfo.collider.gameObject;
            if (whatIHit.GetComponent<HPManager>())
            {
                whatIHit.GetComponent<HPManager>().TakeDamage(damage, false);
                //if (whatIHit.transform.root.GetComponent<EnemyBehaviour>())
                {
                    //TriggerEnemyToAttack(whatIHit.transform.root.GetComponent<EnemyBehaviour>());
                }
            }
            TurnOffBullet();
        }
    }

    public void PenetrationBullet() // penetration bullets
    {
        hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsHittable);
        if (hitInfo.collider != null)
        {
            if (whatIHit == null || hitInfo.collider.gameObject != whatIHit)
            {
                whatIHit = hitInfo.collider.gameObject;
                penetrationCount++;
                if (whatIHit.GetComponent<HPManager>())
                {
                    if (whatIHit.CompareTag("Crew"))
                    {
                        Instantiate(insideMask, transform.position, transform.rotation, hitInfo.collider.transform);
                    }
                    whatIHit.GetComponent<HPManager>().TakeDamage(damage, false);
                    if (destroyTag != "")
                    {
                        TurnOffBullet();
                    }
                    //if (whatIHit.transform.root.GetComponent<EnemyBehaviour>())
                    {
                    //    TriggerEnemyToAttack(whatIHit.transform.root.GetComponent<EnemyBehaviour>());
                    }
                }
                projectileMovement.curSpeed += projectileMovement.curSpeed * percentOfPenetrationMove;
                if (penetrationCount >= penetrationCountMax)
                {
                    TurnOffBullet();
                }
            }

        }
    }

    public void ExplosiveBullet() // explosive projectiles: blow up on hit or after time
    {
        hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsHittable);
        if ((hitInfo.collider != null || currentActiveTime <= 0) && projectileMovement.enabled)
        {
            projectileMovement.enabled = false;
            explosionEffect.SetActive(true);
        }
        if (projectileMovement.isActiveAndEnabled == false)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range, whoToDamage);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].GetComponent<HPManager>())
                {
                    targets[i].GetComponent<HPManager>().TakeDamage(damage, true);
                    //if (targets[i].transform.root.GetComponent<EnemyBehaviour>() && owner != null)
                    {
                    //    TriggerEnemyToAttack(targets[i].transform.root.GetComponent<EnemyBehaviour>());
                    }
                }
            }
        }
    }

    public void AreaBullet() // Big bullets which blows up On collision
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range, whoToDamage);
        if (targets.Length > 0 || (currentActiveTime <= 0 && blowUpIfNoEnemyFound))
        {
            projectileMovement.enabled = false;
            explosionEffect.SetActive(true);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].GetComponent<HPManager>())
                {
                    targets[i].GetComponent<HPManager>().TakeDamage(damage, false);
                    //if (targets[i].transform.root.GetComponent<EnemyBehaviour>())
                    {
                        //    TriggerEnemyToAttack(targets[i].transform.root.GetComponent<EnemyBehaviour>());
                    }
                }
            }
        }
        else if(currentActiveTime <= 0 && blowUpIfNoEnemyFound == false)
        {
            TurnOffBullet();
        }
    }

    public void BouncingBullets() // bouncing / reflecting
    {
        Ray2D ray = new Ray2D(transform.position, transform.up);
        hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsHittable);
        if (Physics2D.Raycast(ray.origin, ray.direction, distance, whatIsHittable))
        {
            if (hitInfo.collider != null)
            {
                whatIHit = hitInfo.collider.gameObject;
                if (whatIHit.GetComponent<HPManager>())
                {
                    whatIHit.GetComponent<HPManager>().TakeDamage(damage, true);
                    //if (whatIHit.transform.root.GetComponent<EnemyBehaviour>())
                    {
                    //    TriggerEnemyToAttack(whatIHit.transform.root.GetComponent<EnemyBehaviour>());
                    }
                    if (destroyTag != "")
                    {
                        if (hitInfo.collider.CompareTag(destroyTag))
                        {
                            TurnOffBullet();
                        }
                    }

                }
                if (currentReflected == maxReflection)
                {
                    TurnOffBullet();
                }
                if (hitInfo.collider.CompareTag(ricochetTag))
                {
                    Vector2 reflectDir = Vector2.Reflect(ray.direction, hitInfo.normal);
                    float rot = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg - 90;
                    transform.eulerAngles = new Vector3(0, 0, rot);
                    currentReflected++;
                }
            }
        }
    }

    public void AttachingBullet() // attaching bullets
    {
        hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsHittable);
        if (hitInfo.collider != null)
        {
            //if (whatIHit.transform.root.GetComponent<EnemyBehaviour>())
            {
                //TriggerEnemyToAttack(whatIHit.transform.root.GetComponent<EnemyBehaviour>());
            }
            projectileMovement.enabled = false;
            transform.parent = hitInfo.transform;
            currentActiveTime -= Time.deltaTime;
            if (currentActiveTime <= 0)
            {
                explosionEffect.SetActive(true);
                Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range, whoToDamage);
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i].GetComponent<HPManager>())
                    {
                        targets[i].GetComponent<HPManager>().TakeDamage(damage, true);
                        //if (targets[i].transform.root.GetComponent<EnemyBehaviour>())
                        {
                        //   TriggerEnemyToAttack(targets[i].transform.root.GetComponent<EnemyBehaviour>());
                        }
                    }
                }
            }
        }
        if (currentActiveTime <= 0)
        {
            TurnOffBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // on entering trigger do dmg or go through enemies
    {
        if (collision.GetComponent<HPManager>() && collision.CompareTag(destroyTag))
        {
            collision.GetComponent<HPManager>().TakeDamage(damage, false);
            //if (whatIHit.transform.root.GetComponent<EnemyBehaviour>())
            {
            //    TriggerEnemyToAttack(whatIHit.transform.root.GetComponent<EnemyBehaviour>());
            }
        }
    }

    
    /*private void TriggerEnemyToAttack(EnemyBehaviour enemyBehaviour)
    {
        enemyBehaviour.target = owner;
        enemyBehaviour.eCurrentBehaviour = enemyBehaviour.eChasePlayers;
    }*/

    public void TurnOffBullet()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        if (showZone == true)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
            Gizmos.DrawRay(transform.position, transform.up * distance);
        }
    }

}
