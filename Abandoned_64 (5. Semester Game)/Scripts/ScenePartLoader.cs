using UnityEngine;
using UnityEngine.SceneManagement;

public enum LoadMethod { DISTANCE, TRIGGER, EVENT}
public class ScenePartLoader : MonoBehaviour
{

    private PlayerStateMachine player;
    [SerializeField] private LoadMethod loadMethod;
    [SerializeField] private float loadRange;
    [SerializeField] private string loadSceneName;
    [SerializeField] private string EVENT_unloadSceneName;
    [SerializeField] private BoxCollider colliderArea;

    //Scene state
    private bool isLoaded;
    private bool shouldLoad;

    void Start()
    {
        player = PlayerStateMachine.Instance;
        isLoaded = CheckLoaded();
    }

    //verify if the scene is already open to avoid opening a scene twice
    private bool CheckLoaded()
    {
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == loadSceneName)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void Update()
    {
        if (loadMethod == LoadMethod.DISTANCE)
        {
            DistanceCheck();
        }
        else if (loadMethod == LoadMethod.TRIGGER)
        {
            TriggerCheck();
        }
        /*else if (loadMethod == LoadMethod.EVENT)
        {
            //wait for EventListener
        }*/
    }

    public void LoadSceneAndUnloadCurrentSceneAndDisableScenePartLoader()
    {
        SceneManager.UnloadSceneAsync(EVENT_unloadSceneName); //SceneManager.GetActiveScene()
        LoadScene();
        gameObject.SetActive(false);
    }

    void DistanceCheck()
    {
        //Checking if the player is within the range
        if (Vector3.Distance(player.transform.position, transform.position) <= loadRange)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }

    void LoadScene()
    {
        if (!isLoaded)
        {
            SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
            isLoaded = true;
        }
    }

    void UnLoadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(loadSceneName);
            isLoaded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            shouldLoad = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            shouldLoad = false;
        }
    }

    void TriggerCheck()
    {
        if (shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnLoadScene();
        }
    }

    private void OnDrawGizmos()
    {
        if (loadMethod != LoadMethod.EVENT)
        { 
            Gizmos.color = Color.yellow;
            Gizmos.matrix = colliderArea.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(colliderArea.transform.position + colliderArea.center - transform.position, colliderArea.size);
        }
    }

    private void OnSceneLoaded(Scene current, LoadSceneMode mode)
    {
        if(current.name == loadSceneName)
        {
            isLoaded = true;
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        if(current.name == loadSceneName)
        {
            isLoaded = false;
        }
    }

    private void OnEnable()
    {
        //subscribe to callback events
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //unsubscribe from callback events
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
