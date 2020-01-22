using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ImpactEvent(Transform impactPoint);

public class SlimeBallExplosion : MonoBehaviour
{
    public Material wallMaterial;
    public static ImpactEvent OnImpact;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SlimeSurface")
        {
            other.GetComponent<WallInteraction>().SwitchMaterial(wallMaterial);
            //other.GetComponent<Renderer>().material = wallMaterial;
        }
        if(OnImpact != null) OnImpact(this.transform);
        Destroy(gameObject);
    }
}
