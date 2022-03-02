using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    #region Singeton

    private static CameraController instance;

    public static CameraController Instance { get => instance; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    #endregion

    [SerializeField] private Settings settings;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera lockOnCam;
    [SerializeField] private CinemachineFreeLook thirdPersonCam;

    public CinemachineVirtualCamera LockOnCam { get => lockOnCam; }
    public CinemachineFreeLook ThirdPersonCam { get => thirdPersonCam; }

    public Camera MainCamera { get => mainCamera;  }

    [SerializeField] private LayerMask obstructionLayers;

    [Header("Lock On")]
    [SerializeField] private float lockOnLossTime = 1f;
    private float currentlockOnLossTime;
    [SerializeField][Range(0, 1)] private float lockOnThresholdX = 1;
    [SerializeField][Range(0, 1)] private float lockOnThresholdY = 1;
    [SerializeField] private float lockOnDistance = 20;
    public float LockOnDistance { get => lockOnDistance; }
    [SerializeField] private LayerMask lockOnLayers = -1;

    private bool lockedOn;
    private ITargetable lockOnTarget;

    public ITargetable LockOnTarget { get => lockOnTarget/* == null ? lockOnTarget : */; }
    public bool LockedOn { get => lockedOn; }

    private void Start()
    {
        UpdateSensitivity();
    }

    // Update is called once per frame
    private void Update()
    {
        if(lockOnTarget == null || lockOnTarget.Equals(null) || !lockedOn)
        {
            ToggleLockOn(false);
        }
        else
        {
            bool valid = lockOnTarget.Targetable && InDistance(lockOnTarget) && InScreen(lockOnTarget) && NotBlocked(lockOnTarget);

            if (valid)
            {
                currentlockOnLossTime = 0;
            }
            else
            {
                currentlockOnLossTime += Time.deltaTime;
            }

            if(currentlockOnLossTime >= lockOnLossTime)
            {
                TryRelock();
            }
        }
    }

    private void LateUpdate()
    {
        //thirdPersonCam.m_XAxis.Value = 10;
    }

    public void UpdateSensitivity()
    {
        //if (!SaveManager.Instance.SettingsSaveFileExists())
        //    return;

        if(InputDeviceManager.Instance.ActiveDeviceType == DeviceType.Controller)
        {
            thirdPersonCam.m_XAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
            thirdPersonCam.m_YAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
            thirdPersonCam.m_XAxis.m_MaxSpeed = settings.controllerSensitivity.x;
            thirdPersonCam.m_YAxis.m_MaxSpeed = settings.controllerSensitivity.y;
        }
        else
        {
            thirdPersonCam.m_XAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            thirdPersonCam.m_YAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            thirdPersonCam.m_XAxis.m_MaxSpeed = settings.keyboardSensitivity.x;
            thirdPersonCam.m_YAxis.m_MaxSpeed = settings.keyboardSensitivity.y;
        }
    }

    private void SwitchPriority()
    {
        int tempPriority = lockOnCam.Priority;
        lockOnCam.Priority = thirdPersonCam.Priority;
        thirdPersonCam.Priority = tempPriority;
    }

    public void ToggleLockOn(bool toggle)
    {
        if (toggle == lockedOn)
            return;

        lockedOn = toggle;

        if (lockedOn)
        {
            List<ITargetable> targetables = new List<ITargetable>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnDistance, lockOnLayers);

            foreach(Collider collider in colliders)
            {
                ITargetable targetable = collider.GetComponent<ITargetable>();
                if(targetable != null && targetable.Targetable && InScreen(targetable) && NotBlocked(targetable))
                {
                    targetables.Add(targetable);
                }
            }

            //findtargetables within threshold
            List<ITargetable> targetablesWithinThreshold = new List<ITargetable>();

            foreach (ITargetable targetable in targetables)
            {
                if (InScreenThreshold(targetable))
                {
                    targetablesWithinThreshold.Add(targetable);
                }
            }

            if(targetablesWithinThreshold.Count != 0)
            {
                //find closest targetable to player
                float distance;
                float smallestDistance = Mathf.Infinity;
                ITargetable closestTargetToPlayer = null;

                foreach (ITargetable targetable in targetablesWithinThreshold)
                {
                    distance = Vector3.Distance(targetable.TargetTransform.position, transform.position);
                    if (smallestDistance > distance)
                    {
                        closestTargetToPlayer = targetable;
                        smallestDistance = distance;
                    }
                }

                lockOnTarget = closestTargetToPlayer;
                lockedOn = closestTargetToPlayer != null;

                if (lockedOn)
                {
                    lockOnCam.LookAt = lockOnTarget.TargetTransform;
                    SwitchPriority();
                    //characterMovementController.FreezeConstraints(true);
                }
            }
            else
            {
                //find targetable closest to center of screen
                float hypotenuse;
                float smallestHypotenuse = Mathf.Infinity;
                ITargetable closestTargetToScreenCenter = null;

                foreach(ITargetable targetable in targetables)
                {
                    hypotenuse = CalculateHypotenuse(targetable.TargetTransform.position); 
                    if(smallestHypotenuse > hypotenuse)
                    {
                        closestTargetToScreenCenter = targetable;
                        smallestHypotenuse = hypotenuse;
                    }
                }

                lockOnTarget = closestTargetToScreenCenter;
                lockedOn = closestTargetToScreenCenter != null;

                if (lockedOn)
                {
                    lockOnCam.LookAt = lockOnTarget.TargetTransform;
                    SwitchPriority();
                    //characterMovementController.FreezeConstraints(true);
                }
            }
        }
        else
        {
            SwitchPriority();
            lockOnCam.LookAt = null;
            lockOnTarget = null;
            lockedOn = false;
            //characterMovementController.FreezeConstraints(false);
        }
    }

    public void TryRelock()
    {
        ToggleLockOn(false);
        ToggleLockOn(true);
    }

    public void SwitchLockOnLeftNext()
    {
        if (lockedOn)
        {
            List<ITargetable> targetables = new List<ITargetable>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnDistance, lockOnLayers);

            //get all targetables on the left of current target
            foreach (Collider collider in colliders)
            {
                ITargetable targetable = collider.GetComponent<ITargetable>();
                if(targetable != null)
                {
                    float relativeX = transform.InverseTransformPoint(targetable.TargetTransform.position).x;
                    if (targetable.Targetable && InScreen(targetable) && NotBlocked(targetable) && relativeX < 0 && targetable != lockOnTarget)
                    {
                        targetables.Add(targetable);
                    }
                }
            }

            if (targetables.Count != 0)
            {
                //find the targetable closets to current target
                float distance;
                float smallestDistance = Mathf.Infinity;
                ITargetable closest = null;

                foreach (ITargetable targetable in targetables)
                {
                    distance = Vector3.Distance(targetable.TargetTransform.position, LockOnTarget.TargetTransform.position);
                    if (smallestDistance > distance)
                    {
                        closest = targetable;
                        smallestDistance = distance;
                    }
                }

                lockOnTarget = closest;
                lockedOn = closest != null;

                if (lockedOn)
                {
                    lockOnCam.LookAt = lockOnTarget.TargetTransform;
                }
            }
        }
    }

    public void SwitchLockOnRightNext()
    {
        if (lockedOn)
        {
            List<ITargetable> targetables = new List<ITargetable>();
            Collider[] colliders = Physics.OverlapSphere(transform.position, lockOnDistance, lockOnLayers);

            //get all targetables on the right of current target
            foreach (Collider collider in colliders)
            {
                ITargetable targetable = collider.GetComponent<ITargetable>();
                if(targetable != null)
                {
                float relativeX = transform.InverseTransformPoint(targetable.TargetTransform.position).x;
                if (targetable.Targetable && InScreen(targetable) && NotBlocked(targetable) && relativeX > 0 && targetable != lockOnTarget)
                {
                        targetables.Add(targetable);
                }
                }
            }

            if (targetables.Count != 0)
            {
                //find the targetable closets to current target
                float distance;
                float smallestDistance = Mathf.Infinity;
                ITargetable closest = null;

                foreach (ITargetable targetable in targetables)
                {
                    distance = Vector3.Distance(targetable.TargetTransform.position, LockOnTarget.TargetTransform.position);
                    if (smallestDistance > distance)
                    {
                        closest = targetable;
                        smallestDistance = distance;
                    }
                }

                lockOnTarget = closest;
                lockedOn = closest != null;

                if (lockedOn)
                {
                    lockOnCam.LookAt = lockOnTarget.TargetTransform;
                }
            }
        }
    }

    private bool InDistance(ITargetable targetable)
    {
        float distance = Vector3.Distance(targetable.TargetTransform.position, transform.position);
        return distance <= lockOnDistance;
    }

    private bool InScreen(ITargetable targetable)
    {
        Vector3 viewPortPosition = mainCamera.WorldToViewportPoint(targetable.TargetTransform.position);

        if (viewPortPosition.x < 0 || viewPortPosition.x > 1) return false;
        if (viewPortPosition.y < 0 || viewPortPosition.y > 1) return false;
        if (viewPortPosition.z < 0) return false;

        return true;
    }

    private bool NotBlocked(ITargetable targetable)
    {
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = targetable.TargetTransform.position - origin;

        float radius = 0.15f;
        float distance = direction.magnitude;
        bool notBlocked = !Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, obstructionLayers);

        return notBlocked;
    }

    private float CalculateHypotenuse(Vector3 position)
    {
        float screenCenterX = mainCamera.pixelWidth / 2;
        float screenCenterY = mainCamera.pixelHeight / 2;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);
        float xDelta = screenCenterX - screenPosition.x;
        float yDelta = screenCenterY - screenPosition.y;
        float hypotenuse = Mathf.Sqrt(Mathf.Pow(xDelta, 2) + Mathf.Pow(yDelta, 2));

        return hypotenuse;
    }

    private bool InScreenThreshold(ITargetable targetable)
    {
        Vector3 viewPortPosition = mainCamera.WorldToViewportPoint(targetable.TargetTransform.position);

        if (viewPortPosition.x < 0.5 - lockOnThresholdX / 2 || viewPortPosition.x > 0.5 + lockOnThresholdX / 2) return false;
        if (viewPortPosition.y < 0.5 - lockOnThresholdY / 2 || viewPortPosition.y > 0.5 + lockOnThresholdY / 2) return false;
        if (viewPortPosition.z < 0) return false;

        return true;
    }
}
