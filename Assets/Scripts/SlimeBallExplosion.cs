using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ImpactEvent(Transform impactPoint);

public class SlimeBallExplosion : MonoBehaviour
{
    public Material wallMaterial;
    public static ImpactEvent OnImpact;
    public ParticleSystem switchExplosion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SlimeSurface")
        {
            other.GetComponent<WallInteraction>().SwitchMaterial(wallMaterial);
            //other.GetComponent<Renderer>().material = wallMaterial;
            Instantiate(switchExplosion, other.transform.position, Quaternion.identity);
        }
        if(OnImpact != null) OnImpact(this.transform);
        Destroy(gameObject);
    }
}
