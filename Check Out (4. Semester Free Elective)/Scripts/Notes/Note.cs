using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public NoteType type;
    public int line = 0;
    [HideInInspector] public float speed = 10;
    protected RhythmGameManager rhythmManagerInstance;
    protected GameManager gameManagerInstance;
    public bool hit = false;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer shadowSpriteRenderer;
    public Sprite[] normalSprites;
    public bool special = false;
    public bool missed = false;
    public float lengthInHoldTime;
    Song currentSong;

    [SerializeField] protected Rigidbody2D rb;

    protected void Start()
    {
        type = NoteType.TAP;
        rhythmManagerInstance = RhythmGameManager.instance;
        gameManagerInstance = GameManager.instance;
        currentSong = gameManagerInstance.currentSong;

        if (currentSong)
        {
            float distance = Mathf.Abs(transform.position.x - rhythmManagerInstance.scanLine.transform.position.x);
            speed = distance / currentSong.secondsTillLine;
        }
    }

    private void Update()
    {   
        Move();

        if (transform.position.x <= rhythmManagerInstance.scanLine.transform.position.x - rhythmManagerInstance.greatDistance && !hit)
        {
            //managerInstance.HitLine(line);
            Miss(transform);
            rhythmManagerInstance.DequeueNote(line);
            Destroy(gameObject);
        }

        if (transform.position.x <= -20)
        {
            Destroy(gameObject);
        }
    }

    public void Move()
    {
        transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
    }

    public void CatapultOut(float force)
    {
        if (!gameObject.GetComponent<Rigidbody2D>())
        {
            Debug.LogWarning("note has nor rb to catapult");
            return;
        }

        Vector2 forceDirection = new Vector2(0.5f, 1 - 2 * line);
        rb.velocity = forceDirection * force;
        rb.angularVelocity = (1 - 2 * line) * force * 90;
    }

    public virtual void Hit()
    {
        hit = true;
        float distance = Mathf.Abs(transform.position.x - rhythmManagerInstance.scanLine.transform.position.x);
        if (distance > rhythmManagerInstance.greatDistance)
        {
            //miss
            Miss(transform);
            rhythmManagerInstance.DiscardNote(line);
        }
        else if (distance > rhythmManagerInstance.perfectDistance)
        {
            //great
            Great(transform);
            rhythmManagerInstance.ScanNote(line);
        }
        else
        {
            //perfect
            Perfect(transform);
            rhythmManagerInstance.ScanNote(line);
        }
    }

    public virtual void TintColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void Miss(Transform t)
    {
        Debug.Log("miss");
        TintColor(Color.grey);
        if(rhythmManagerInstance.combo > rhythmManagerInstance.currentSong.highestCombo)
        {
            rhythmManagerInstance.currentSong.highestCombo = rhythmManagerInstance.combo;
        }
        rhythmManagerInstance.combo = 0;
        rhythmManagerInstance.comboBreak.Raise();
        missed = true;
        rhythmManagerInstance.AddScore(rhythmManagerInstance.missScore);
        Instantiate(rhythmManagerInstance.missPrefab, t.position + new Vector3(rhythmManagerInstance.missPrefab.GetComponentInChildren<RectTransform>().sizeDelta.x, 0, 0) / 2, Quaternion.identity);
    }

    public void Miss()
    {
        Miss(transform);
    }

    public void Great(Transform t)
    {
        Debug.Log("great");
        if (rhythmManagerInstance.combo == 0)
        {
            rhythmManagerInstance.combostart.Raise();
        }
        rhythmManagerInstance.combo++;
        rhythmManagerInstance.AddScore(rhythmManagerInstance.greatScore);
        Instantiate(rhythmManagerInstance.greatHitPrefab, t.position + new Vector3(rhythmManagerInstance.greatHitPrefab.GetComponentInChildren<RectTransform>().sizeDelta.x, 0, 0) / 2, Quaternion.identity);
    }

    public void Great()
    {
        Great(transform);
    }

    public void Perfect(Transform t)
    {
        Debug.Log("perfect");
        if (rhythmManagerInstance.combo == 0)
        {
            rhythmManagerInstance.combostart.Raise();
        }
        rhythmManagerInstance.combo++;
        rhythmManagerInstance.AddScore(rhythmManagerInstance.perfectScore);
        Instantiate(rhythmManagerInstance.perfectHitPrefab, t.position + new Vector3(rhythmManagerInstance.perfectHitPrefab.GetComponentInChildren<RectTransform>().sizeDelta.x, 0, 0) / 2, Quaternion.identity);
    }

    public void Perfect()
    {
        Perfect(transform);
    }

    public virtual void SetSprite(Sprite sprite)
    {
        if (normalSprites.Length > 0)
        {
            spriteRenderer.sprite = sprite;
            shadowSpriteRenderer.sprite = sprite;
        }
    }

    public virtual void SetRandomSprite()
    {
        if (normalSprites.Length > 0)
        {
            int randomSpriteIndex = Random.Range(0, normalSprites.Length);
            SetSprite(normalSprites[randomSpriteIndex]);
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

public enum NoteType { TAP, HOLD, AVOID }
