using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RhythmGameManager : MonoBehaviour
{
    public static RhythmGameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one GameManager instance: destroying new one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    [HideInInspector] public BeltLine[] beltLines;
    Vector2 beltYPos;

    public KeyCode[] upperInputs;
    public KeyCode[] lowerInputs;

    public GameObject notePrefab;
    public GameObject holdNotePrefab;
    public GameObject avoidNotePrefab;
    public GameObject scanLine;

    public float perfectDistance;
    public float greatDistance;

    public int perfectScore;
    public int greatScore;
    public int missScore;
    public GameObject missPrefab;

    public GameObject perfectHitPrefab;
    public GameObject greatHitPrefab;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    public int combo = 0;
    public TextMeshProUGUI comboText;
    public GameEvent comboBreak;
    public GameEvent combostart;

    public Song currentSong;
    public int tact = 8;
    public bool adjustForBpm = false;
    int songPotentialScore;
    Coroutine songRoutine;
    public float noteSpawnX = 10;

    bool holdingUpper = false;
    bool holdingLower = false;
    public int holdPoints = 10;
    public float holdPointsInterval = 0.1f;

    public GameObject upperHoldParticles;
    public GameObject lowerHoldParticles;

    public GameEvent endOfSong;
    public AudioSource musicSource;

    float timeSInceLastSpawn = 0;
    float timeSinceStart = 0;

    [SerializeField] CashierScript cashier;

    [SerializeField] GameAffectionSlider affectionSlider;

    [SerializeField] private Image songProgress;
    private float lastNoteTime;

    [SerializeField] Animator comboAnim;
    //ToDo: EventSystem

    private void Start()
    {
        Setup();

        if (!currentSong)
            StartCoroutine(SpawnRandom());
        else
        {
            //change timing of song, add to serializable object itself?
            if (currentSong.changeTiming.change)
            {
                for(int i = (int)currentSong.changeTiming.indexRange.x; i < currentSong.changeTiming.indexRange.y && i < currentSong.notesToSpawn.Count; i++)
                {
                    NoteSpawn replace = new NoteSpawn();
                    replace = currentSong.notesToSpawn[i];
                    replace.timingInSeconds += currentSong.changeTiming.changeBySec;
                    currentSong.notesToSpawn[i] = replace;
                }

                currentSong.changeTiming.change = false;
            }

            StartPlayingCurrentSong();
        }
    }

    private void Setup()
    {
        score = 0;
        beltYPos = new Vector2(1.7f, -1.7f);
        beltLines = new BeltLine[2];
        beltLines[0].notes = new Queue<Note>();
        beltLines[1].notes = new Queue<Note>();
        currentSong = GameManager.instance.currentSong;
        songPotentialScore = 0;

        if (currentSong != null)
        {
            foreach (NoteSpawn note in currentSong.notesToSpawn)
            {
                if (note.typeToSpawn == NoteType.HOLD)
                {
                    songPotentialScore += perfectScore * 2; //not adding the "holdscore" to create a bit of leaniance
                }
                else
                {
                    songPotentialScore += perfectScore;
                }
            }

            affectionSlider.Setup(currentSong.targetScorePercentage);
            lastNoteTime = currentSong.notesToSpawn[currentSong.notesToSpawn.Count - 1].timingInSeconds;
        }
        else
        {
            affectionSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        timeSInceLastSpawn += Time.deltaTime;
        timeSinceStart += Time.deltaTime;
        ManageInputs();
        songProgress.fillAmount = timeSinceStart / lastNoteTime;

        if (GameManager.instance.menu)
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }

    public void ManageInputs()
    {
        if (beltLines[0].notes.Count > 0)
        {
            if (UpperPressedDown())
            {
                holdingUpper = true;
                HitLine(0);
            }
        }

        if (beltLines[1].notes.Count > 0)
        {
            if (LowerPressedDown())
            {
                holdingLower = true;
                HitLine(1);
            }
        }


        if (UpperReleased())
        {
            if (beltLines[0].notes.Count > 0)
            {
                if (beltLines[0].notes.Peek().hit && beltLines[0].notes.Peek().type == NoteType.HOLD)
                {
                    HitLine(0);
                    upperHoldParticles.SetActive(false);
                }
            }

            holdingUpper = false;
        }

        if (LowerReleased())
        {
            if (beltLines[1].notes.Count > 0)
            {
                if (beltLines[1].notes.Peek().hit && beltLines[1].notes.Peek().type == NoteType.HOLD)
                {
                    HitLine(1);
                    lowerHoldParticles.SetActive(false);
                }
            }

            holdingLower = false;
        }
    }

    bool UpperPressedDown()
    {
        if (holdingUpper)
            return false;

        for(int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyDown(upperInputs[i]))
                return true;
        }
        return false;
    }

    bool UpperReleased()
    {

        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyUp(upperInputs[i]))
                return true;
        }
        return false;
    }

    bool LowerPressedDown()
    {
        if (holdingLower)
            return false;

        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyDown(lowerInputs[i]))
                return true;
        }
        return false;
    }

    bool LowerReleased()
    {
        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyUp(lowerInputs[i]))
                return true;
        }
        return false;
    }

    public Note SpawnNote(int beltLineIndex, GameObject noteToSpawn)
    {
        GameObject obj = Instantiate(notePrefab, new Vector3(noteSpawnX, beltYPos[beltLineIndex], 0), Quaternion.identity);
        Note toQueue = obj.GetComponent<Note>();
        beltLines[beltLineIndex].notes.Enqueue(toQueue);
        toQueue.line = beltLineIndex;
        toQueue.SetRandomSprite();
        return toQueue;
    }

    public Note SpawnNote(NoteSpawn noteInfo)
    {
        timeSInceLastSpawn = 0;
        GameObject obj;
        switch (noteInfo.typeToSpawn)
        {
            case NoteType.TAP:
                obj = Instantiate(notePrefab, new Vector3(noteSpawnX, beltYPos[noteInfo.lineToSpawnOn], 0), Quaternion.identity);
                break;
            case NoteType.HOLD:
                obj = Instantiate(holdNotePrefab, new Vector3(noteSpawnX, beltYPos[noteInfo.lineToSpawnOn], 0), Quaternion.identity);
                break;
            case NoteType.AVOID:
                obj = Instantiate(avoidNotePrefab, new Vector3(noteSpawnX, beltYPos[noteInfo.lineToSpawnOn], 0), Quaternion.identity);
                break;
            default:
                obj = Instantiate(notePrefab, new Vector3(noteSpawnX, beltYPos[noteInfo.lineToSpawnOn], 0), Quaternion.identity);
                Debug.LogWarning("something went wrong in spawn switch");
                break;
        }

        Note toQueue = obj.GetComponent<Note>();
        beltLines[noteInfo.lineToSpawnOn].notes.Enqueue(toQueue);
        toQueue.line = noteInfo.lineToSpawnOn;
        toQueue.lengthInHoldTime = noteInfo.noteLength;

        if(noteInfo.typeToSpawn != NoteType.HOLD)
        {
            if (noteInfo.overrideSprite)
                toQueue.SetSprite(noteInfo.overrideSprite);
            else
            {
                //ToDo: move this into note script
                if (GameManager.instance && GameManager.instance.currentCharacter.specialProductSprites.Length > 0 && Random.Range(0,10) >= 9)
                {
                    int randomSpriteIndex = Random.Range(0, GameManager.instance.currentCharacter.specialProductSprites.Length);
                    toQueue.SetSprite(GameManager.instance.currentCharacter.specialProductSprites[randomSpriteIndex]);
                }
                else
                    toQueue.SetRandomSprite();

            }
        }

        return toQueue;
    }

    public void DiscardNote(int beltLineIdx)
    {
        Note note = DequeueNote(beltLineIdx);

        if (beltLineIdx == 0)
            cashier.DiscardUpper(note.transform);
        else
            cashier.DiscardLower(note.transform);

        note.CatapultOut(20);
        note.enabled = false;
        Destroy(note.gameObject, 1.5f);
    }

    public void ScanNote(int beltLineIdx)
    {
        Note note = DequeueNote(beltLineIdx);

        if (beltLineIdx == 0)
            cashier.PickUpUpper(note.transform);
        else
            cashier.PickUpLower(note.transform);

        note.enabled = false;
        Destroy(note.gameObject);
    }

    public Note DequeueNote(int beltLineIdx)
    {
        if (beltLines[beltLineIdx].notes.Count <= 0)
            return null;

        Note note = beltLines[beltLineIdx].notes.Dequeue();

        if (note.type == NoteType.HOLD)
        {
            if (beltLineIdx == 0)
                upperHoldParticles.SetActive(false);
            else
                lowerHoldParticles.SetActive(false);
        }

        return note;
    }

    //red?
    public void HitLine(int beltLineIdx)
    {
        NoteType typeOfHitNote = beltLines[beltLineIdx].notes.Peek().type;

        switch (typeOfHitNote)
        {
            case NoteType.TAP:
                //HitTapNote(beltLineIdx);
                beltLines[beltLineIdx].notes.Peek().Hit();
                break;
            case NoteType.HOLD:
                //HitHoldNote(beltLineIdx);
                beltLines[beltLineIdx].notes.Peek().Hit();
                break;
            case NoteType.AVOID:
                beltLines[beltLineIdx].notes.Peek().Hit();
                //miss
                break;
            default:
                Debug.LogWarning("something went wrong in the hit switch");
                break;
        }
    }

    public void AddScore(int add)
    {
        score += add;
        if (score < 0)
        {
            score = 0;
        }
        scoreText.text = "" + score;
        comboText.text = "" + combo;
        comboAnim.Play("ComboHit");
        affectionSlider.UpdateValue(score, songPotentialScore);
        WonAffection(); //cheks progress and triggers event if reached
    }
        
    IEnumerator SpawnRandom()
    {
        while(true)
        {
        yield return new WaitForSeconds(0.2f);
        SpawnNote(Random.Range(0, 2), notePrefab);
        }
    }

    public bool NotesOnScreen()
    {
        if (beltLines[0].notes.Count > 0)
            return true;

        if (beltLines[1].notes.Count > 0)
            return true;

        return false;
    }

    public bool WonAffection()
    {
        return affectionSlider.TargetReached();
    }

    public void StartPlayingCurrentSong()
    {
        if (adjustForBpm)
            songRoutine = StartCoroutine(PlaySongAdjustedToBpm());
        else
            songRoutine = StartCoroutine(PlaySongAsRecorded());
    }

    IEnumerator PlaySongAsRecorded() //ToDo: change prefab spawn thingy & add switch statement with types and override of song
    {
        musicSource.time = 0;
        musicSource.clip = currentSong.music;
        musicSource.Play();
        float timePassed = 0;

        for(int i = 0; i < currentSong.notesToSpawn.Count; i++)
        {
            yield return new WaitForSeconds(currentSong.notesToSpawn[i].timingInSeconds - timePassed - currentSong.secondsTillLine);
            timePassed += timeSInceLastSpawn; /*currentSong.notesToSpawn[i].timingInSeconds - currentSong.secondsTillLine;*/
            Debug.Log(timeSinceStart - timePassed);

            if(i + 1 < currentSong.notesToSpawn.Count && currentSong.notesToSpawn[i].timingInSeconds == currentSong.notesToSpawn[i + 1].timingInSeconds)
            {
                SpawnNote(currentSong.notesToSpawn[i]);
                SpawnNote(currentSong.notesToSpawn[i + 1]);
                i++;
            }
            else
            {
                SpawnNote(currentSong.notesToSpawn[i]);
            }
        }

        bool ended = false;
        while (!ended)
        {
            if (!NotesOnScreen())
            {
                //yield return new WaitForSeconds(5);
                EndSong();
                ended = true;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PlaySongAdjustedToBpm() //ToDo: change prefab spawn thingy & add switch statement with types and override of song
    {
        musicSource.time = 0;
        musicSource.clip = currentSong.music;
        musicSource.Play();
        float timePassed = 0;

        for (int i = 0; i < currentSong.notesToSpawn.Count; i++)
        {
            yield return new WaitForSeconds(TimingAdjustedToBpm(currentSong.notesToSpawn[i].timingInSeconds) - timePassed - currentSong.secondsTillLine);
            timePassed += timeSInceLastSpawn; /*currentSong.notesToSpawn[i].timingInSeconds - currentSong.secondsTillLine;*/
            Debug.Log(timeSinceStart - timePassed);

            if (i + 1 < currentSong.notesToSpawn.Count && TimingAdjustedToBpm(currentSong.notesToSpawn[i].timingInSeconds) == TimingAdjustedToBpm(currentSong.notesToSpawn[i + 1].timingInSeconds))
            {
                SpawnNote(currentSong.notesToSpawn[i]);
                SpawnNote(currentSong.notesToSpawn[i + 1]);
                i++;
            }
            else if(i + 1 < currentSong.notesToSpawn.Count && currentSong.notesToSpawn[i].lineToSpawnOn != currentSong.notesToSpawn[i + 1].lineToSpawnOn && Mathf.Abs(currentSong.notesToSpawn[i].timingInSeconds - currentSong.notesToSpawn[i + 1].timingInSeconds) < 0.05)
            {
                SpawnNote(currentSong.notesToSpawn[i]);
                SpawnNote(currentSong.notesToSpawn[i + 1]);
                i++;
            }
            else
            {
                SpawnNote(currentSong.notesToSpawn[i]);
            }
        }

        bool ended = false;
        while (!ended)
        {
            if (!NotesOnScreen())
            {
                //yield return new WaitForSeconds(5);
                EndSong();
                ended = true;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void EndSong()
    {
        if(score > currentSong.highScore)
        {
            currentSong.highScore = score;
        }

        if (combo > currentSong.highestCombo)
        {
            currentSong.highestCombo = combo;
        }

        GameManager.instance.lastGameSuccess = WonAffection();
        GameManager.instance.cameFromGame = true;
        SaveManager.instance.SaveGameData();
        endOfSong.Raise();
    }

    float TimingAdjustedToBpm(float currentTiming)
    {
        float beatTimeDif = 60f / currentSong.bpm;
        float rest = currentTiming % beatTimeDif;
        float lowerTiming = currentTiming - rest;

        for(int i = 0; i < tact; i++)
        {
            if (lowerTiming + beatTimeDif / tact > currentTiming)
                break;

            lowerTiming += (beatTimeDif / tact);
        }

        float upperTiming = lowerTiming + beatTimeDif / tact;

        return (Mathf.Abs(currentTiming - lowerTiming) <= Mathf.Abs(currentTiming - upperTiming)) ? lowerTiming : upperTiming;
    }
}

public struct BeltLine
    {
        public Queue<Note> notes;
    }

