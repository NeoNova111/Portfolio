using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldNote : Note
{
    public Transform startPoint;
    public Transform endPoint;

    public SpriteRenderer startSprite;
    List<SpriteRenderer> midPartSpriteRenderers;
    public SpriteRenderer endSprite;

    public Transform midSpritesHolder;

    private float intervalTimer;
    public GameObject midPart;
    public float partWidth = 1.5f;


    private void Start()
    {
        base.Start();

        midPartSpriteRenderers = new List<SpriteRenderer>();

        if (lengthInHoldTime < 0.1f)
            lengthInHoldTime = 0.1f;

        type = NoteType.HOLD;
        AdjustNote();
        SetRandomSprite();
        //SpawnShadows();
    }

    //ToDo: clean WAY up using EventSystem
    private void Update()
    {
        Move();

        if (startPoint.position.x <= rhythmManagerInstance.scanLine.transform.position.x - rhythmManagerInstance.greatDistance && !hit && !missed)
        {
            Miss(startPoint);
            missed = true;
            rhythmManagerInstance.DequeueNote(line);
        }
        else if (endPoint.position.x <= rhythmManagerInstance.scanLine.transform.position.x && hit)
        {
            Perfect(endPoint);
            hit = false;
            rhythmManagerInstance.ScanNote(line);
        }

        if (endPoint.position.x <= -20)
        {
            Destroy(gameObject);
        }

        intervalTimer += Time.deltaTime;
        if (hit && !missed && intervalTimer >= rhythmManagerInstance.holdPointsInterval)
        {
            rhythmManagerInstance.score += rhythmManagerInstance.holdPoints;
            intervalTimer = 0;

            if (line == 0)
                rhythmManagerInstance.upperHoldParticles.SetActive(true);
            else
                rhythmManagerInstance.lowerHoldParticles.SetActive(true);
        }
    }

    public override void TintColor(Color color)
    {
        startSprite.color = color;
        foreach(SpriteRenderer sr in midPartSpriteRenderers)
        {
            sr.color = color;
        }
        endSprite.color = color;
    }

    public void SetSprite(Sprite begin, Sprite mid, Sprite end)
    {
        for (int i = -1; i <= midPartSpriteRenderers.Count; i++)
        {
            if (i == -1)
            {
                startSprite.sprite = begin;
            }
            else if (i == midPartSpriteRenderers.Count)
            {
                endSprite.sprite = end;
            }
            else
            {
                midPartSpriteRenderers[i].sprite = mid;
            }
        }
    }

    public override void SetRandomSprite()
    {
        int range = normalSprites.Length / 3;
        int rnd = Random.Range(0, range);
        SetSprite(normalSprites[rnd * 3], normalSprites[rnd * 3 + 1], normalSprites[rnd * 3 + 2]);
    }

    void SpawnShadows()
    {
        CreateShadowCopyObject(startPoint.gameObject);
        CreateShadowCopyObject(endPoint.gameObject);

        if (midPartSpriteRenderers.Count > 0)
        {
            float startEndDistance = speed * lengthInHoldTime;
            GameObject midShadow = Instantiate(midPartSpriteRenderers[0].gameObject, midSpritesHolder);
            midShadow.transform.localPosition += new Vector3(startEndDistance / 2, -1f, 0);
            midShadow.transform.localScale = new Vector3(startEndDistance / 2, 0.5f, 0f);
            SpriteRenderer r = midShadow.GetComponent<SpriteRenderer>();
            r.color = new Color(0, 0, 0, 150f/255f);
            r.sortingLayerName = "Game";
            r.sortingOrder = 8;
        }
    }

    void CreateShadowCopyObject(GameObject parentObject)
    {
        GameObject shadowObject = Instantiate(parentObject, parentObject.transform);
        shadowObject.transform.localPosition = new Vector3(0, -1f, 0);
        shadowObject.transform.localScale = new Vector3(1.2f, 0.5f, 0f);
        SpriteRenderer r = shadowObject.GetComponent<SpriteRenderer>();
        r.color = new Color(0, 0, 0, 150f/255f);
        r.sortingLayerName = "Game";
        r.sortingOrder = 8;
    }

    public void AdjustNote()
    {
        float startEndDistance = speed * lengthInHoldTime;
        //Debug.Log("distance: "+startEndDistance+", speed: "+speed+", length: "+lengthInHoldTime);
        endPoint.position += new Vector3(startEndDistance, 0, 0);
        int numberOfMidParts = Mathf.CeilToInt(startEndDistance / partWidth);
        float individualDistance = startEndDistance / numberOfMidParts;
        for(int i = 0; i < numberOfMidParts; i++)
        {
            GameObject obj = Instantiate(midPart, midSpritesHolder);
            midPartSpriteRenderers.Add(obj.GetComponent<SpriteRenderer>());
            obj.transform.position += new Vector3(partWidth + individualDistance * i, 0, 0);
        }
    }

    public override void Hit()
    {
        if (!hit)
        {
            hit = true;
            float distance = Mathf.Abs(startPoint.position.x - rhythmManagerInstance.scanLine.transform.position.x);
            if (distance > rhythmManagerInstance.greatDistance)
            {
                //miss
                Miss(startPoint);
                rhythmManagerInstance.DiscardNote(line);
            }
            else if (distance > rhythmManagerInstance.perfectDistance)
            {
                //great
                Great(startPoint);
            }
            else
            {
                //perfect
                Perfect(startPoint);
            }
        }
        else
        {
            float distance = Mathf.Abs(endPoint.position.x - rhythmManagerInstance.scanLine.transform.position.x);
            if (distance > rhythmManagerInstance.greatDistance)
            {
                //miss
                Miss(endPoint);
                rhythmManagerInstance.DiscardNote(line);
            }
            else if (distance > rhythmManagerInstance.perfectDistance)
            {
                //great
                Great(endPoint);
                rhythmManagerInstance.ScanNote(line);
            }
            else
            {
                //perfect
                Perfect(endPoint);
                rhythmManagerInstance.ScanNote(line);
            }
        }
    }
}
