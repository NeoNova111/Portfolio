using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScannerEffectDemo : MonoBehaviour
{
	public Material EffectMaterial;
	public float ScanDistance;
    public float scanSpeed = 10;

	private Camera _camera;
    private float offset;

    public GameEvent Scan;
    public Image scanCooldownImage;
    AbilityUI abilityUI;

    public SubStats submarineStats;

    bool _scanning;
    bool setSubsequent;
    float maxScanCooldown;
    float maxScanDistance;

    Plane cameraPlane;

    void Start()
	{
        cameraPlane = new Plane();

        offset = EffectMaterial.GetFloat("_ScanWidth") + EffectMaterial.GetFloat("_ScanDistanceOffset");
        maxScanDistance = EffectMaterial.GetFloat("_MaxScanDistance") + EffectMaterial.GetFloat("_ScanDistanceOffset") + EffectMaterial.GetFloat("_ScanWidth");
        maxScanCooldown = maxScanDistance / scanSpeed;

        if(scanCooldownImage != null)
            scanCooldownImage.fillAmount = 0;

        abilityUI = UIManager.instance.abilityUI;
    }

    void Update()
    {
        if (submarineMovement.instance && submarineMovement.instance.GetCurrentRoom())
        {
            switch (submarineMovement.instance.GetCurrentRoom().perspective)
            {
                case CameraPerspective.TOPDOWN:
                    EffectMaterial.SetFloat("_PerspectiveType", 0);
                    break;
                case CameraPerspective.SIDE:
                    EffectMaterial.SetFloat("_PerspectiveType", 1);
                    break;
                case CameraPerspective.POV:
                    EffectMaterial.SetFloat("_PerspectiveType", 2);
                    break;
            }
        }

        cameraPlane.SetNormalAndPosition(transform.forward, transform.position);

        EffectMaterial.SetFloat("_SubViewDepth", cameraPlane.GetDistanceToPoint(submarineStats.submarinePosition));
        EffectMaterial.SetVector("_SubPos", submarineStats.submarinePosition);

        if (_scanning)
        {
            ScanDistance += scanSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.C) && !_scanning)
		{
            StartScan();
        }

        if(ScanDistance >= offset && !setSubsequent)
        {
            EffectMaterial.SetVector("_SubsequentWorldSpaceScannerPos", submarineStats.submarinePosition);
            setSubsequent = true;
        }

        EffectMaterial.SetFloat("_ScanDistance", ScanDistance);
        EffectMaterial.SetTexture("_MainTex", _camera.activeTexture);

        //maybe move into coroutine for less calls
        if (_scanning && EffectMaterial.GetFloat("_ScanDistance") >= maxScanDistance)
            _scanning = false;
    }

	void OnEnable()
	{
		_camera = GetComponent<Camera>();
		_camera.depthTextureMode = DepthTextureMode.Depth;
	}

    void StartScan()
    {
        if (abilityUI != null)
        {
            abilityUI.StartScanCooldown(maxScanCooldown);
        }
        else
        {
            Debug.LogWarning("No AbilityUI/ UICanvas prefab found to display cooldown");
        }

        Scan.Raise();
        _scanning = true;
        setSubsequent = false;
        ScanDistance = 1;
        EffectMaterial.SetVector("_WorldSpaceScannerPos", submarineStats.submarinePosition);      //moved to allow for doppler effect
    }

    public void StopScan()
    {
        _scanning = false;
        ScanDistance = 0;
        abilityUI.ResetScanCooldown();
    }
}
