using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordingManager : MonoBehaviour
{
    public Song songToRecord;

    public KeyCode[] upperInputs;
    public KeyCode[] lowerInputs;

    public AudioSource musicSource;
    public AudioSource tickSource;
    public Text timerText;
    public GameObject timerObj;
    public Text recordingText;
    public InputField startTimeInput;
    public InputField delayInput;
    public Text delayText;
    public Text liveDelayText;
    public GameEvent finishedCalc;

    float overwriteStartSecond;
    float timeSinceStart = 0;
    float songPlayDelay = 0;

    float reactionDelay = 0;
    float maxTickIntervall = 2;
    float minTickIntervall = 1;
    float timeSincePress = 0;
    int maxReactionTicks = 10;
    List<float> delayInputs;
    bool testing = false;
    bool testPressed = false;

    List<NoteSpawn> recordedNotes;
    bool recording = false;
    bool upperPressed = false;
    bool lowerPressed = false;
    [Range(0, 1)]public float holdTimeStartThreshold;
    float upperPressedTimer = 0;
    float lowerPressedTimer = 0;
    int prepDelay = 5;


    // Start is called before the first frame update
    void Start()
    {
        musicSource.time = 0;
        overwriteStartSecond = 0;
        recordedNotes = new List<NoteSpawn>();
        delayInputs = new List<float>();
        recordingText.text = "Recording...\n" + songToRecord.name;
    }

    // Update is called once per frame
    void Update()
    {
        delayText.text = "" + Mathf.Ceil(reactionDelay * 1000);

        if(recording)
        {
            if (upperPressed)
            {
                upperPressedTimer += Time.deltaTime;
            }

            if(lowerPressed)
            {
                lowerPressedTimer += Time.deltaTime;
            }

            timeSinceStart += Time.deltaTime;
            RecordInput();
        }

        if(testing)
        {
            timeSincePress += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && !testPressed)
            {
                NewReactionDelay();
            }
        }
    }

    void NewReactionDelay()
    {
        delayInputs.Add(timeSincePress);

        float avg = 0;
        foreach(float f in delayInputs)
        {
            avg += f;
        }
        reactionDelay = avg / delayInputs.Count;

        testPressed = true;
        liveDelayText.text = "" + Mathf.Ceil(reactionDelay * 1000) + "ms";
    }

    public void StartDelayCalc()
    {
        StopAllCoroutines();
        StartCoroutine(DelayCalcRoutine());
    }

    public void ResetDelay()
    {
        reactionDelay = 0;
        delayText.text = "" + Mathf.Ceil(reactionDelay * 1000);
    }

    public void SetDelayToField()
    {
        if (delayInput.text != "")
        {
            reactionDelay = float.Parse(delayInput.text) / 1000;
        }
    }

    IEnumerator DelayCalcRoutine()
    {
        liveDelayText.text = "" + Mathf.Ceil(reactionDelay * 1000) + "ms";
        delayInputs.Clear();
        testing = true;
        testPressed = true;
        int ticks = 0;
        yield return new WaitForSecondsRealtime(maxTickIntervall);

        while (ticks < maxReactionTicks)
        {
            timeSincePress = 0;
            ticks++;
            tickSource.Play();
            testPressed = false;

            yield return new WaitForSecondsRealtime(Random.Range(minTickIntervall, maxTickIntervall));

            if(!testPressed)
            {
                NewReactionDelay();
            }
        }

        testing = false;
        delayText.text = "" + Mathf.Ceil(reactionDelay * 1000);
        finishedCalc.Raise();
    }

    void RecordInput()
    {
        if (UpperPressedDown())
        {
            recordedNotes.Add(new NoteSpawn(NoteType.TAP, 0, timeSinceStart));
        }

        if (LowerPressedDown())
        {
            recordedNotes.Add(new NoteSpawn(NoteType.TAP, 1, timeSinceStart));
        }

        if (UpperReleased() && upperPressedTimer >= holdTimeStartThreshold)
        {
            ReplaceLastNoteWithHold(0, upperPressedTimer);
        }

        if (LowerReleased() && lowerPressedTimer >= holdTimeStartThreshold)
        {
            ReplaceLastNoteWithHold(1, lowerPressedTimer);
        }
    }

    void ReplaceLastNoteWithHold(int beltIndex, float timer)
    {
        for(int i = recordedNotes.Count - 1; i >= 0; i--)
        {
            if(recordedNotes[i].lineToSpawnOn == beltIndex)
            {
                NoteSpawn holdReplacement = new NoteSpawn(NoteType.HOLD, beltIndex, recordedNotes[i].timingInSeconds, timer);
                recordedNotes[i] = holdReplacement;
                return;
            }
        }
    }

    public void StartRecording()
    {
        timeSinceStart = overwriteStartSecond;
        PrepareSongForRecording();
        StartTimerWithDelay();
    }

    public void StartTimerWithDelay()
    {
        StartCoroutine(StartTimer(prepDelay));
    }

    IEnumerator StartTimer(int time/*, GameEvent gameEvent*/)
    {
        timerObj.SetActive(true);

        while(time >= 0)
        {
            if(songPlayDelay < 0 && !musicSource.isPlaying)
            {
                musicSource.Play();
            }

            if(time == 0)
            {
                timerText.text = "Start!";
            }
            else
            {
                timerText.text = ""+time;
            }
            tickSource.Play();

            yield return new WaitForSecondsRealtime(1);

            songPlayDelay--;
            time--;
        }

        //gameEvent.Raise();
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
        timerObj.SetActive(false);
        recording = true;
    }

    public void SaveRecording()
    {
        recording = false;
        musicSource.Stop();

        for (int i = 0; i < songToRecord.notesToSpawn.Count; i++)
        {
            if (songToRecord.notesToSpawn[i].timingInSeconds >= overwriteStartSecond)
            {
                songToRecord.notesToSpawn.Remove(songToRecord.notesToSpawn[i]);
                i--;
            }
        }

        //songToRecord.recordingDelay = reactionDelay;
        songToRecord.notesToSpawn.AddRange(recordedNotes);
    }

    public void StopRecording()
    {
        recording = false;
        musicSource.Stop();
    }

    public void PauseRecording()
    {
        recording = false;
        recordingText.text = "Paused...";
    }

    public void ContinueRecording()
    {
        recording = true;
        recordingText.text = "Recording...";      
    }

    void PrepareSongForRecording()
    {
        recordedNotes.Clear();

        musicSource.clip = songToRecord.music;
        if(overwriteStartSecond - prepDelay < 0)
        {
            songPlayDelay = prepDelay - overwriteStartSecond;
            musicSource.time = 0;
        }
        else
        {
            songPlayDelay = 0;
            musicSource.time = overwriteStartSecond - prepDelay;
        }
    }

    public void SetStartSecondsToFieldValue()
    {
        if(startTimeInput.text != "")
        {
            overwriteStartSecond = int.Parse(startTimeInput.text);
        }
    }

    #region Inputs

    bool UpperPressedDown()
    {
        //if (holdingUpper)
        //    return false;

        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyDown(upperInputs[i]))
            {
                upperPressed = true;
                upperPressedTimer = 0;
                return true;
            }
        }
        return false;
    }

    bool UpperReleased()
    {

        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyUp(upperInputs[i]))
            {
                upperPressed = false;
                return true;
            }
        }
        return false;
    }

    bool LowerPressedDown()
    {
        //if (holdingLower)
        //    return false;

        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyDown(lowerInputs[i]))
            {
                lowerPressed = true;
                lowerPressedTimer = 0;
                return true;
            }
        }
        return false;
    }

    bool LowerReleased()
    {
        for (int i = 0; i < upperInputs.Length; i++)
        {
            if (Input.GetKeyUp(lowerInputs[i]))
            {
                lowerPressed = false;
                return true;
            }
        }
        return false;
    }

    #endregion
}
