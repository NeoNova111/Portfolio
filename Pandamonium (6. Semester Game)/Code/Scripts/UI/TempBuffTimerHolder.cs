using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBuffTimerHolder : MonoBehaviour
{
    [System.Serializable]
    public struct BuffIconTypePair
    {
        public Sprite icon;
        public BuffType type;
    }

    public struct TempBuffTimerObj
    {
        public TempBuffTimerObj(TempBuff buff, GameObject obj)
        {
            this.buff = buff;
            this.gameObject = obj;
        }

        public TempBuff buff;
        public GameObject gameObject;
    }

    [SerializeField] private GameObject buffTimer;
    [SerializeField] private BuffIconTypePair[] iconPairs;
    private Dictionary<BuffType, Sprite> buffPairingDict = new Dictionary<BuffType, Sprite>();
    private List<TempBuffTimerObj> activeBuffs = new List<TempBuffTimerObj>();

    private void Start()
    {
        foreach(var iconPair in iconPairs)
        {
            buffPairingDict.Add(iconPair.type, iconPair.icon);
        }
    }

    public void AddBuffToDisplay(TempBuff buff)
    {
        bool found = false;
        for(int i = 0; i < activeBuffs.Count; i++)
        {
            if(activeBuffs[i].buff.buffType == buff.buffType) //if there is already a item with that buffType on display it gets updated
            {
                activeBuffs[i] = new TempBuffTimerObj(buff, activeBuffs[i].gameObject);
                activeBuffs[i].gameObject.GetComponent<TempBuffTimer>().InitRemainingTime(activeBuffs[i].buff.duration, buffPairingDict[activeBuffs[i].buff.buffType]);
                found = true;
            }
        }

        if (!found)
        {
            GameObject obj = Instantiate(buffTimer, transform);
            TempBuffTimerObj timerObj = new TempBuffTimerObj(buff, obj);
            activeBuffs.Add(timerObj);
            timerObj.gameObject.GetComponent<TempBuffTimer>().InitRemainingTime(timerObj.buff.duration, buffPairingDict[timerObj.buff.buffType]);
        }
    }

    public void RemoveBuffFromDisplay(BuffType type)
    {
        //removes buff by type, since there can be only one temp buff of a certain type at any given time
        foreach(var a in activeBuffs)
        {
            if (a.buff.buffType == type)
            {
                activeBuffs.Remove(a);
                Destroy(a.gameObject);
                return;
            }
        }
    }
}
