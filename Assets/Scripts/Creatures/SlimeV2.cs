using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum SlimeAI
{
    Wandering,
    Idle,
    LookAtFriend,
    COUNT
}

[RequireComponent(typeof(CanSee))]
public class SlimeV2 : MonoBehaviour
{
    [Header("AI Slime parameters")]
    NavMeshAgent agent;
    Transform friend;
    SlimeV2 friendSlimeAI;
    public SlimeAI slimeState;
    public float minChangeTime;
    public float maxChangeTime;
    float changeTime;

    [Header("Etat du slime, hors AI State")]
    bool fleeing;
    bool stunned;
    bool lookingAtPlayer;
    bool InteractingWithFriend;
    [HideInInspector]
    public bool binding;
    [HideInInspector]
    public bool friendInteractPossible;

    [Header("Slimes parameters")]
    float distToPlayer;
    public float fleePlayerDist;
    CanSee slimeCanSee;
    
    public float timeToForgotPlayer;
    float seePlayerTime;

    public float baseInteractWithFriendTime;
    float interactFriendTime;

    public float baseStunTime;
    float stunnedTime;

    [Header("Slime Emotions")]
    public Material baseEmotionMaterial;
    Material myEmotionMaterial;
    public float delayBTWemotions;
    float timeBTWemotions;
    public Texture[] emotionsTextures;
    public ParticleSystem emotionParticle;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        slimeCanSee = GetComponent<CanSee>();
        myEmotionMaterial = new Material(baseEmotionMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stunned && !fleeing && !lookingAtPlayer && !binding)
        {
            if (!InteractingWithFriend)
            {
                friendInteractPossible = true;
            }
            else
            {
                friendInteractPossible = false;
            }
            

            changeTime -= Time.deltaTime;
            if (changeTime < 0)
            {
                slimeState = (SlimeAI)Random.Range(0, (int)SlimeAI.COUNT);
                SlimeStateChange();
                changeTime = Random.Range(minChangeTime, maxChangeTime);
            }

            switch (slimeState)
            {
                case SlimeAI.Wandering:

                    break;

                case SlimeAI.Idle:
                    agent.destination = transform.position;
                    break;

                case SlimeAI.LookAtFriend:
                    if (friend != null)
                    {
                        if(Vector3.Distance(transform.position, friend.position) < 2.5f 
                            && (friendSlimeAI.slimeState == SlimeAI.Idle 
                                || friendSlimeAI.slimeState == SlimeAI.LookAtFriend)
                            && !InteractingWithFriend)
                        {
                            InteractingWithFriend = true;
                            friendSlimeAI.InteractingWithFriend = true;
                            interactFriendTime = baseInteractWithFriendTime;
                            friendSlimeAI.interactFriendTime = baseInteractWithFriendTime+0.2f;
                            changeTime += baseInteractWithFriendTime;
                            friendSlimeAI.changeTime += baseInteractWithFriendTime;
                            
                            agent.destination = transform.position;
                        }
                        else
                        {
                            agent.destination = friend.position;
                        }
                    }
                    break;
            }
        }
        else
        {
            friendInteractPossible = false;
        }

        //Si une cible est trouvée et que le Slime n'était pas en train de fuir ou immobilisé 
        if (slimeCanSee.targetFinded && !binding && !stunned && !fleeing)
        {
            //Si le joueur est trop proche, on s'enfuit
            if( Vector3.Distance(transform.position, slimeCanSee.target.position) < 6f)
            {
                fleeing = true;
                lookingAtPlayer = false;
                
                //Faire pop un "!"

            }
            //Si il est éloigné de nous mais assez proche pour être visible, on le regarde (spawn "?")
            else if(!lookingAtPlayer)
            {
                lookingAtPlayer = true;

                //Faire pop un "?"

            }
        }

        //On suit le Player des yeux (observer), le Slime tourne pour le regarder puis l'oublie au bout de quelques secondes s'il ne le voit plus
        if (lookingAtPlayer)
        {
            //Faire s'orienter le Slime dans la direction du Player

            //Timer si le slime ne voit plus le joueur
        }


        if (binding || stunned || lookingAtPlayer)
        {
            agent.destination = transform.position;
        }
        
        if(fleeing)
        {
            
        }

        if (InteractingWithFriend)
        {
            Debug.Log("Interacting with friend");
            if(interactFriendTime > 0f)
            {
                interactFriendTime -= Time.deltaTime;
                if(timeBTWemotions > 0f)
                {
                    timeBTWemotions -= Time.deltaTime;
                }
                else
                {
                    timeBTWemotions = delayBTWemotions;
                    Texture nextEmotion = emotionsTextures[Random.Range(0, emotionsTextures.Length - 1)];
                    DoEmotion(nextEmotion);
                }
            }
            else
            {
                StopInteractingWithFriend();
            }
        }
        
    }

    void SlimeStateChange()
    {
        StopInteractingWithFriend();

        switch (slimeState)
        {
            case SlimeAI.Wandering:
                agent.destination = transform.position +
                    Quaternion.AngleAxis(
                        Random.Range(0, 360),
                        Vector3.up
                    ) * Vector3.forward * 10;
                break;

            case SlimeAI.Idle:

                break;

            case SlimeAI.LookAtFriend:
                SlimeV2[] otherSlimes = FindObjectsOfType<SlimeV2>();
                List<SlimeV2> nearSlimes = new List<SlimeV2>();
                foreach (SlimeV2 slime in otherSlimes)
                {
                    if(Vector3.Distance(transform.position, slime.transform.position) < 40f && slime != this && slime.friendInteractPossible)
                    {
                        nearSlimes.Add(slime);
                    }
                }
                if (nearSlimes.Count > 0)
                {
                    friend = nearSlimes[Random.Range(0, nearSlimes.Count)].transform;
                    friendSlimeAI = friend.GetComponent<SlimeV2>();
                } 
                else
                {
                    slimeState = SlimeAI.Wandering;
                    agent.destination = transform.position +
                    Quaternion.AngleAxis(
                        Random.Range(0, 360),
                        Vector3.up
                    ) * Vector3.forward * 10;
                }
                break;
        }
    }

    public void BindingWithPlayer()
    {
        fleeing = false;
        stunned = false;
        lookingAtPlayer = false;
        binding = true;
    }

    public void DoEmotion(Texture emotionTexture)
    {
        Debug.Log(emotionTexture.name);
        //myEmotionMaterial.mainTexture = emotionTexture;
        //emotionParticle.GetComponent<ParticleSystemRenderer>().material = myEmotionMaterial;

        //emotionParticle.GetComponent<ParticleSystemRenderer>().material.mainTexture = emotionTexture;

        emotionParticle.GetComponent<Renderer>().material.SetTexture("_BaseMap", emotionTexture);

        emotionParticle.Play();
    }

    public void StopInteractingWithFriend()
    {
        if(friend != null)
        {
            SlimeV2 friendSlimeAI = friend.GetComponent<SlimeV2>();
            friendSlimeAI.InteractingWithFriend = false;
            if (friendSlimeAI.friendInteractPossible)
            {
                friendSlimeAI.SlimeStateChange();
            }
            
            InteractingWithFriend = false;
            friend = null;
        }
    }
}
