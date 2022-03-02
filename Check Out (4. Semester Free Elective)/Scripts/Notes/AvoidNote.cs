using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidNote : Note
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        type = NoteType.AVOID;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if(transform.position.x <= rhythmManagerInstance.scanLine.transform.position.x + rhythmManagerInstance.greatDistance && !hit)
        {
            hit = true;
            Miss(transform);
            rhythmManagerInstance.DequeueNote(line);
            Destroy(gameObject, 2f);
        }
    }

    public override void Hit()
    {
        hit = true;
        Perfect(transform);
        rhythmManagerInstance.DiscardNote(line);
    }
}
