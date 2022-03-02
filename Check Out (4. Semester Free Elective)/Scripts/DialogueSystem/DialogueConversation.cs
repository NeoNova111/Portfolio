using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conversation")]
public class DialogueConversation : ScriptableObject
{
    public DialogueCharacter dialogueCharacter;
    public Line[] conversationLines;
    [HideInInspector] public int lineIndex = 0;
    [HideInInspector] public bool endOfLines { get { return lineIndex == conversationLines.Length - 1; } }

    //returns false when it can't iterate further, and resets the index
    public bool IterateLineIndex()
    {
        if (lineIndex + 1 >= conversationLines.Length)
        {
            return false;
        }
        lineIndex++;

        return true;
    }

    public Line currentLine()
    {
        return conversationLines[lineIndex];
    }
}

[System.Serializable]
public struct Line
{
    public CharacterSpriteType spriteType;
    public DialogueType dialogueType;
    [TextArea(2, 5)] public string text;
    [Tooltip("Avoid in combination with choices (untested behaviour)")] public GameEvent[] triggerOnComplete;
    [Tooltip("Only in use when the choice dialogue type is selected. Always keep at size 2")]public Choice[] choices;
}

[System.Serializable]
public struct Choice
{
    public string choiceText;
    public int affectionConsequence;
    public DialogueConversation dialogueConsequence;
    public GameEvent eventConsequence;
}

public enum DialogueType { ADDITIVE, OVERRIDE, CHOICE }

public enum CharacterSpriteType { DEFAULT=0, SUCCESS=1, FAIL=2 }
