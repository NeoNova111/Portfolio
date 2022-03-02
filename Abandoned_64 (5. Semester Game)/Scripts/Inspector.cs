using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    [SerializeField] private float inspectDistance = 20;
    [SerializeField] private LayerMask ignoreLayers = -1;
    [SerializeField] private LayerMask inspectable;
    private CameraController cameraController;
    private DebugModeManager debugModeManager;
    private UIManager userInterfaceManager;
    [SerializeField] private GameObject TargetObject;
    // Start is called before the first frame update
    void Start()
    {
        cameraController = CameraController.Instance;
        debugModeManager = DebugModeManager.Instance;
        userInterfaceManager = UIManager.Instance;

        //inverts the layermask to actually ignore the layers selected
        ignoreLayers = ~ignoreLayers;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugModeManager.DebugModeActive)
        {
            IInspectable inspectable = LookForInspectable();
            if (inspectable != null)
            {
                if(inspectable.DeveloperComment != userInterfaceManager.DeveloperCommentText)
                {
                    userInterfaceManager.UpdateDeveloperComment(inspectable.DeveloperComment);
                    Debug.Log("IsLookingAtObject");
                    TargetObject.SetActive(true);
                }
            }
            else if(userInterfaceManager.DeveloperCommentPanelIsActive)
            {
                userInterfaceManager.HideDeveloperComment();
                TargetObject.SetActive(false);
            }
        }
        
    }
    public void DisableTargetVisualizer()
    {
        TargetObject.SetActive(false);
    }
    public void EnableTargetVisualizer()
    {
        TargetObject.SetActive(true);
    }
    private IInspectable LookForInspectable()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraController.MainCamera.transform.position, cameraController.MainCamera.transform.forward, out hit, inspectDistance, ignoreLayers))
        {
            Debug.DrawLine(cameraController.MainCamera.transform.position, hit.transform.position, Color.green);

            TargetObject.transform.position = hit.collider.gameObject.transform.position+new Vector3(0,1,0);
            
            return hit.transform.GetComponent<IInspectable>();
        }
        else
        {
            return null;
        }
  
    }
}
