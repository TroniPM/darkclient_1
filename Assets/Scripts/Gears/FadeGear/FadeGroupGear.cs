using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class FadeGroupGear : GearParent
{
    public float mainFadeInTime;
    public float mainFadeOutTime;
    public GameObject[] mainGameObject;

    public float secondStageFadeInTime;
    public float secondStageFadeOutTime;
    public GameObject[] secondStageGameObject;

    public int stageDelay;

    protected List<FadeAgent> mainFadeAgent = new List<FadeAgent>();
    protected List<FadeAgent> secondStageFadeAgent = new List<FadeAgent>();

    protected bool hasIn;

    void Start()
    {
        gearType = "FadeGroupGear";
        ID = (uint)defaultID;

        mainFadeAgent = new List<FadeAgent>();
        secondStageFadeAgent = new List<FadeAgent>();

        GetMainGameObjectFadeAgent();
        GetSecondStageGameObjectFadeAgent();

        hasIn = false;

        AddListeners();
    }

    protected void GetMainGameObjectFadeAgent()
    {
        if (mainGameObject == null)
            return;

        if (mainGameObject.Length == null)
            return;

        foreach (var item in mainGameObject)
        {
            var agent = item.GetComponent<FadeAgent>();
            if (agent)
                mainFadeAgent.Add(agent);
        }
    }

    protected void GetSecondStageGameObjectFadeAgent()
    {
        if (secondStageGameObject == null)
            return;

        if (secondStageGameObject.Length == null)
            return;

        foreach (var item in secondStageGameObject)
        {
            var agent = item.GetComponent<FadeAgent>();
            if (agent)
                secondStageFadeAgent.Add(agent);
        }
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            hasIn = true;

            if (mainFadeAgent != null && mainFadeAgent.Count > 0)
            {
                foreach (var agent in mainFadeAgent)
                {
                    agent.SetMainFadeIn(mainFadeInTime);
                }
            }

            if (secondStageFadeAgent != null && secondStageFadeAgent.Count > 0)
            {
                foreach (var agent in secondStageFadeAgent)
                {
                    agent.SetSecondStageFadeIn((uint)stageDelay, secondStageFadeInTime);
                }
            }
        }
    }


    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (hasIn)
                return;
            else
                hasIn = true;

            if (mainFadeAgent != null && mainFadeAgent.Count > 0)
            {
                foreach (var agent in mainFadeAgent)
                {
                    agent.SetMainFadeIn(mainFadeInTime);
                }
            }

            if (secondStageFadeAgent != null && secondStageFadeAgent.Count > 0)
            {
                foreach (var agent in secondStageFadeAgent)
                {
                    agent.SetSecondStageFadeIn((uint)stageDelay, secondStageFadeInTime);
                }
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            hasIn = false;

            if (mainFadeAgent != null && mainFadeAgent.Count > 0)
            {
                foreach (var agent in mainFadeAgent)
                {
                    agent.SetMainFadeOut((uint)stageDelay, mainFadeOutTime);
                }
            }

            if (secondStageFadeAgent != null && secondStageFadeAgent.Count > 0)
            {
                foreach (var agent in secondStageFadeAgent)
                {
                    agent.SetSecondStageFadeOut(secondStageFadeOutTime);
                }
            }
        }
    }

}
