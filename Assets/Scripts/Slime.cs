using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Slime : MonoBehaviour
{
    public enum AIslime {
        Idle,
        Wandering,
        LookAtFriend,
        Animation,
        COUNT
    }
    NavMeshAgent agent;
	Transform friend;

    [Header("Slime AI")]
	public AIslime slimeState;
	public float minChangeTime;
	public float maxChangeTime;
	float changeTime;

    [Header("Slime metamorphose")]
    public TextMeshProUGUI copyTMP;
    public string copyMessage;
    public bool allowPossess; //à utiliser afin de définir si le monstre peut être copié ou non

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        allowPossess = true; // à modifier 
    }

    // Update is called once per frame
    void Update()
    {
        changeTime -= Time.deltaTime;
		if (changeTime <0)
		{
			slimeState = (AIslime) Random.Range(0, (int) AIslime.COUNT);
            SlimeStateChange();
			changeTime = Random.Range(minChangeTime, maxChangeTime);
		}

        switch(slimeState) {
            
            case AIslime.Idle :
                agent.destination = transform.position;
            break;

            case AIslime.Wandering :
                
            break;

            case AIslime.LookAtFriend :
				if (friend != null)
				{
					agent.destination = friend.position;
				}
            break;

            case AIslime.Animation :

            break;

        }
    }

    void SlimeStateChange(){
        switch (slimeState)
        {
            case AIslime.Idle :

            break;

            case AIslime.Wandering :
                agent.destination = transform.position + 
                    Quaternion.AngleAxis(
                        Random.Range(0,360),
                        Vector3.up
                    ) * Vector3.forward*10;
            break;

            case AIslime.LookAtFriend :
                Slime[] agents = FindObjectsOfType<Slime>();
			    friend = agents[Random.Range(0,agents.Length)].transform;
            break;

            case AIslime.Animation :

            break;
        }
    }

    private void OnDrawGizmos()
    {
        switch (slimeState)
        {
            case AIslime.Idle :
                Gizmos.color = Color.blue;
            break;

            case AIslime.Wandering :
                Gizmos.color = Color.red;
            break;

            case AIslime.LookAtFriend :
                Gizmos.color = Color.green;
            break;

            case AIslime.Animation :
                Gizmos.color = Color.yellow;
            break;
        }   

        if (agent != null)
        {
            Gizmos.DrawSphere(agent.destination, 0.5f);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && allowPossess)
        {
            copyTMP.text = copyMessage;
            copyTMP.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && allowPossess)
        {
            copyTMP.gameObject.SetActive(false); 
        }
    }
}
