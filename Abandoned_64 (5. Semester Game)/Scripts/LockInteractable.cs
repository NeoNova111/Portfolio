using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LockInfo
{
    public LockInfo(int id, string sceneName, bool locked)
    {
        this.lockID = id;
        this.sceneName = sceneName;
        this.locked = locked;
    }

    public int lockID;
    public string sceneName;
    public bool locked;
}

public class LockInteractable : MonoBehaviour, IInteractable
{
    //made protected cause I might want to have this as a super class of more specific locks
    [SerializeField] protected PlayerStats playerstats;
    [SerializeField] protected GameEvent collectablesChanged;
    [SerializeField] protected GameEvent unlockedLock;
    [SerializeField] protected int lockID;
    [SerializeField] protected int keysNeeded;

    [SerializeField] protected bool interactable = true;
    [SerializeField] protected Transform targetTransform;
    protected SaveManager saveManager;
    protected bool interacting = false;
    protected LockInfo lockInfo;

    public bool Interactable { get => interactable; }
    public Transform TargetTransform { get { return targetTransform ? targetTransform : transform; } }
    public bool Interacting { get => interacting; }

    private ContextPrompt prompt;
    public ContextPrompt ContextPrompt { get => prompt; }

    protected void Start()
    {
        prompt = new ContextPrompt("Unlock", 1);
        saveManager = SaveManager.Instance;

        //temp before implementing the saving of the locks
        lockInfo = new LockInfo(lockID, gameObject.scene.name, true);

        //check if this lock is already known to the saveManager...
        if(saveManager.SceneLoadData != null)
        {
            bool containsLockInfo = false;
            foreach(LockInfo info in saveManager.SceneLoadData.lockInfo)
            {
                if (lockInfo.lockID == info.lockID && lockInfo.sceneName == info.sceneName)
                {
                    containsLockInfo = true;
                    if (!info.locked)
                    {
                        Unlock();
                    }
                    break;
                }
            }

            //...if not: add it
            if (!containsLockInfo)
            {
                saveManager.SceneLoadData.lockInfo.Add(lockInfo);
            }
        }
    }

    public void EndInteract()
    {
        //interacting = false;
    }

    public void Interact()
    {
        //interacting = true;
        if(playerstats.keyCount >= keysNeeded)
        {
            playerstats.keyCount -= keysNeeded;
            collectablesChanged.Raise();
            Unlock();

            for (int i = 0; i < saveManager.SceneLoadData.lockInfo.Count; i++)
            {
                if (lockInfo.lockID == saveManager.SceneLoadData.lockInfo[i].lockID && lockInfo.sceneName == saveManager.SceneLoadData.lockInfo[i].sceneName)
                {
                    saveManager.SceneLoadData.lockInfo[i] = lockInfo;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("not enough keys");
        }
    }

    protected void Unlock()
    {
        interactable = false;
        lockInfo.locked = false;
        Debug.Log("Unlocked: " + gameObject.name);
        unlockedLock.Raise();
        //do some unlocking
    }

    public void VisualizeTargetable()
    {
        
    }
}
