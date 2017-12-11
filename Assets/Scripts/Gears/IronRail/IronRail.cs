using UnityEngine;
using System.Collections;
using Mogo.Util;

public class IronRail : GearParent
{
    public Animation[] trackAnimations;
    public int[] defaultRollTimes;

    void Start()
    {
        if (trackAnimations.Length != defaultRollTimes.Length)
        {
            LoggerHelper.Debug("trackAnimations.Length != defaultRollTimes.Length");
            return;
        }
        for (int i = 0; i < trackAnimations.Length; i++)
        {
            SetNextRoll(trackAnimations[i], defaultRollTimes[i]);
        }
	}

    private void SetNextRoll(Animation track, int defaultRollTime)
    {
        uint rollTime = (uint)(RandomHelper.GetRandomInt((int)(0.8 * defaultRollTime), (int)(1.2 * defaultRollTime)));
        TimerHeap.AddTimer<Animation, int>(rollTime, 0, RollTrack, track, defaultRollTime);
    }

    private void RollTrack(Animation track, int defaultRollTime)
    {
        if (track != null)
        {
            track.CrossFade("10107_Trap01");
            SetNextRoll(track, defaultRollTime);
        }
    }
}
