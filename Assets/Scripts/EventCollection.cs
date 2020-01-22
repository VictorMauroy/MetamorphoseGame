using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCollection : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnSlimeBallImpact;
    public UnityEvent OnProjectileHitEvent;

    [Header("Particles")]
    public ParticleSystem impactParticles;
    public ParticleSystem[] projectileHitParticles;

    // Start is called before the first frame update
    void Start()
    {
        SlimeBallExplosion.OnImpact += DoSlimeBallImpact;
        Projectile.OnProjectileHit += DoProjectileHit;
    }
    
    void OnDestroy()
    {
        SlimeBallExplosion.OnImpact -= DoSlimeBallImpact;
        Projectile.OnProjectileHit -= DoProjectileHit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Action à faire à chaque impact d'un projectile sur un collider. Son, particule system, script.
    void DoProjectileHit(GameObject projectile){
        switch (projectile.GetComponent<Projectile>().projectileType)
        {
            case ProjectileTypes.Rock :
                Instantiate(projectileHitParticles[0], projectile.transform.position, Quaternion.identity);
            break;

            case ProjectileTypes.Smoke :
                Instantiate(projectileHitParticles[1], projectile.transform.position, Quaternion.AngleAxis(90, Vector3.left));
            break;

            case ProjectileTypes.Lethargic :
                Instantiate(projectileHitParticles[2], projectile.transform.position, Quaternion.AngleAxis(90, Vector3.left));
            break;
        }
    }

    void DoSlimeBallImpact(Transform impactPoint){
        Instantiate(impactParticles, impactPoint.position, Quaternion.identity);
        OnSlimeBallImpact.Invoke();
    }
}
