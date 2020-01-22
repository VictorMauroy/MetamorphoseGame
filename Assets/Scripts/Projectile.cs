using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileTypes
{
    Rock,
    Smoke,
    Lethargic
}

public delegate void ProjectileEvent(GameObject projectile);

public class Projectile : MonoBehaviour
{
    public ProjectileTypes projectileType;
    public static ProjectileEvent OnProjectileHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (OnProjectileHit != null)
        {
            //Déclenche une action à chaque impact sur un collider
            OnProjectileHit(this.gameObject); 
        }


        if (other.tag == "HitableObject") //Tag à définir
        {
            //On active une fonctione qui serait prédéfinie sur l'objet en question, ex : Interact(). Uniquement pour la pierre
            if (projectileType == ProjectileTypes.Rock)
            {
                
            }

        } 
        else //Afin que les projectiles réagissent quelque que soit la surface touchée
        {
            switch (projectileType)
            {
                case ProjectileTypes.Rock :
                    //Rebond de la pierre

                break;

                case ProjectileTypes.Smoke :
                    //Exlosion de fumée (grande portée)

                break;

                case ProjectileTypes.Lethargic :
                    //Explosion de gaz léthargique (moyenne portée)

                break;

               
            } 
        }
    }
}
