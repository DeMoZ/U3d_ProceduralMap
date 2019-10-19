using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class BotDriver : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        StartCoroutine(FixedUpdate_());
    }
    // int cnt, count = 10;
    float deltaTime = 0;
   
    IEnumerator FixedUpdate_()
    {
        while (true)
        {
            //yield return StartCoroutine(AllBotsCycleYield(10));
            deltaTime = Time.deltaTime;
            for (int i = ProjectIOSingletone.Get().goList.Count - 1; i > -1; i--)
            {
                // cnt--;

                if (ProjectIOSingletone.Get().goList[i] != ProjectIOSingletone.Get().ThePlayerB)
                    ProjectIOSingletone.Get().goList[i].UpdateBot(deltaTime);
            }
            yield return null;
        }
    }

    //IEnumerator AllBotsCycleYield(int cycle)
    //{
    //    for (int i = ProjectIOSingletone.Get().goList.Count - 1; i > -1; i--)
    //    {
    //        cnt--;
    //        if (cnt <= 0)
    //        {
    //            // yield return new WaitForEndOfFrame();
    //            yield return null;
    //            deltaTime += Time.deltaTime;
    //            cnt = cycle;
    //        }
    //        deltaTime += Time.deltaTime;
    //        // ProjectIOSingletone.Get().goList[i].GetComponent<BodyMove>().BotUpdate(deltaTime);
    //        if (ProjectIOSingletone.Get().goList[i] != ProjectIOSingletone.Get().ThePlayerB)
    //            ProjectIOSingletone.Get().goList[i].UpdateBot(deltaTime);
    //    }
    //    deltaTime = 0;
    //    yield return null;
    //}
    void OnEnable()
    {
        StaticEvents.eventHit.AddListener(BodyHit);
        StaticEvents.eventKill.AddListener(BodyKill);
    }

    void OnDisable()
    {
        StaticEvents.eventHit.RemoveListener(BodyHit);
        StaticEvents.eventKill.RemoveListener(BodyKill);
    }
    void BodyHit(object[] damageInfo)
    {

    }
    void BodyKill(object[] damageInfo)
    {

        if ((Body)damageInfo[1] && (Body)damageInfo[2])
        {
            Debug.Log("Driver:  " + ((Body)damageInfo[2]).name + " Killed by " + ((Body)damageInfo[1]).name);
            object[] healHit = new object[3];
            healHit[0] =(float) -5;
            healHit[1] = (Body)null;
            healHit[2] = (Body)null;
            //((Body)damageInfo[1]).SetHit(-5);
            ((Body)damageInfo[1]).SetHit(healHit);

        }
    }
}
