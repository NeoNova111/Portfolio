using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatePuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pillars;
    private bool[] pillarIsUp;
    private Vector3[] pillarsStartPosition;
    [SerializeField] private float[] pillarTargetHeight;
    [SerializeField] private GameObject[] pressurePlates;
    [SerializeField] private PressurePlate[] pressurePlateScripts;
    [SerializeField] private Vector3Int[] allocations_Plate_PillarUp_PillarDown;
    //public Vector2 pressurePlateSpeedDownUp;
    [SerializeField] private Vector2 pillarSpeedDownUp;
    [SerializeField] private PressurePlateReset pressurePlateReset;
    [SerializeField] private GameEvent pressurePlateDown;
    [SerializeField] private GameEvent pressurePlateUp;
    [SerializeField] private GameEvent pillarMovement;
    // Start is called before the first frame update
    void Start()
    {
        pillarIsUp = new bool[pillars.Length];
        pillarsStartPosition = new Vector3[pillars.Length];
        for (int i = 0; i< pillarsStartPosition.Length; i++)
        {
            pillarsStartPosition[i] = pillars[i].transform.position;
        }
        pressurePlateScripts = new PressurePlate[pressurePlates.Length];
        for (int i = 0; i < pressurePlates.Length; i++)
        {
            pressurePlateScripts[i] = pressurePlates[i].GetComponent<PressurePlate>();
        }
        if (pillarTargetHeight.Length < pillars.Length)//in case there are less values in the "pillarTargetPos" array then in the "pillars" array. Sets the target pos of all pillars without a target pos value to the first value in the array
        {
            float[] newArray = new float[pillars.Length];
            for (int i = 0; i < pillars.Length; i++)
            {
                if(pillarTargetHeight.Length > i)
                {
                    newArray[i] = pillarTargetHeight[i];
                }
                else
                {
                    newArray[i] = pillarTargetHeight[0];
                }
            }
            pillarTargetHeight = newArray;
        }
    }

    private void Update()
    {
        for (int i = 0; i < pillarIsUp.Length; i++)
        {
            if (pillarIsUp[i])
            {
                Vector3 newPos = pillars[i].transform.position;
                newPos.y += pillarSpeedDownUp[1];
                if(newPos.y < pillarTargetHeight[i])
                {
                    pillars[i].transform.position = newPos;
                }
            }
            else
            {
                Vector3 newPos = pillars[i].transform.position;
                newPos.y -= pillarSpeedDownUp[0];
                if (newPos.y > pillarsStartPosition[i].y)
                {
                    pillars[i].transform.position = newPos;
                }
            }
        }
    }

    public void OnChildTriggerEnter(GameObject pressurePlate)
    {
        int pressureplateIndex = GetIndexOfPressurePlate(pressurePlate);
        
        for (int i = 0; i < allocations_Plate_PillarUp_PillarDown.Length; i++)
        {
            if (allocations_Plate_PillarUp_PillarDown[i].x == pressureplateIndex)
            {
                int pillarToMoveUp = allocations_Plate_PillarUp_PillarDown[i].y;
                pillarIsUp[pillarToMoveUp] = true; //move pillar up
                pillarMovement.Raise();
                int pillarToMoveDownIndex = allocations_Plate_PillarUp_PillarDown[i].z; //get index of pillar to move down
                if(pillarToMoveDownIndex>=0 && pillarToMoveDownIndex < pillars.Length)// only move a pillar down if the index exists
                {
                    pillarIsUp[pillarToMoveDownIndex] = false; //move pillar down
                    pillarMovement.Raise();

                    //find pressureplates that can move up the pillar that is moved down (and activate it)
                    foreach(Vector3Int allocation in allocations_Plate_PillarUp_PillarDown)
                    {
                        if(allocation.y == pillarToMoveDownIndex)
                        {
                            //Debug.Log("Some pressure plate should move up");
                            pressurePlateScripts[allocation.x].movePressurePlateUp();
                            pressurePlateUp.Raise();
                        }
                    }
                    //find pressureplates that moves pillar that was moved up back down (and activate it)
                    foreach (Vector3Int allocation in allocations_Plate_PillarUp_PillarDown)
                    {
                        if (allocation.z == pillarToMoveUp)
                        {
                            //Debug.Log("Some pressure plate should move up");
                            pressurePlateScripts[allocation.x].movePressurePlateUp();
                            pressurePlateDown.Raise();
                        }
                    }
                }
            }
        }
        pressurePlateReset.movePressurePlateUp();
        pressurePlateUp.Raise();
        pillarMovement.Raise();
    }

    public void OnResetTriggerEnter()
    {
        for(int i = 0; i < pillarIsUp.Length; i++)
        {
            pillarIsUp[i] = false;
        }
        for (int i = 0; i < pressurePlateScripts.Length; i++)
        {
            pressurePlateScripts[i].movePressurePlateUp();
        }
        pressurePlateUp.Raise();
        pillarMovement.Raise();
    }
    

    private int GetIndexOfPressurePlate(GameObject pressurePlate)
    {
        for(int i = 0; i<pressurePlates.Length;i++)
        {
            if (pressurePlates[i] == pressurePlate)
            {
                return i;
            }
        }
        Debug.Log("oh no error :(((");
        return 42069;
    }
}
