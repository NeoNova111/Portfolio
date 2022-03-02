using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextPrompt
{
    public ContextPrompt(string type, int priority)
    {
        this.type = type;
        this.priority = priority;
    }

    public string type;
    public int priority;
}

public enum ContextPromtType { Talk, Unlock, Read, PickUp }

public class ContextManager : MonoBehaviour
{
    [SerializeField] private GameEvent OnContextChange;

    private List<ContextPrompt> contextPrompts = new List<ContextPrompt>();
    private UIManager userInterfaceManager;
    private PlayerStateMachine player;
    private DialogueSystem dialogueSystem;
    private ContextPrompt previousContext;
    private ContextPrompt currentContext;
    private string previousContextType;
    private string currentContextType;

    private void Start()
    {
        player = PlayerStateMachine.Instance;
        userInterfaceManager = UIManager.Instance;
        dialogueSystem = DialogueSystem.Instance;

        previousContext = null;
        currentContext = null;
        previousContextType = "";
        currentContextType = "";

    }

    private void Update()
    {
        if (!userInterfaceManager.ContextUIAnimating)
        {
            if (!player.Interacing)
            {
                IInteractable closest = player.GetClosestInteractable(player.InteractRange);
                if (closest != null)
                    currentContext = closest.ContextPrompt;
                else
                {
                    currentContextType = "Dodge";
                    currentContext = null;
                }

                if (closest == null || currentContext.type == "Talk" && !player.CanTalk)
                {
                    currentContextType = "Dodge";
                    currentContext = null;
                    userInterfaceManager.UpdateContextPrompt("Dodge", previousContextType);
                    player.InContext = false;
                }
                else
                {
                    currentContextType = currentContext.type;
                    userInterfaceManager.UpdateContextPrompt(currentContextType, previousContextType);
                    player.InContext = true;
                }
            }
            else
            {
                currentContext = player.InteractingWith.ContextPrompt;
                if (currentContext.type == "Talk")
                {
                    if(!dialogueSystem.IsSpeaking && dialogueSystem.ReachedEnd)
                    {
                        currentContextType = "Return";
                        userInterfaceManager.UpdateContextPrompt("Return", previousContextType);
                    }
                    else if (dialogueSystem.IsSpeaking)
                    {
                        currentContextType = "Skip";
                        userInterfaceManager.UpdateContextPrompt("Skip", previousContextType);
                    }
                    else
                    {
                        currentContextType = "Next";
                        userInterfaceManager.UpdateContextPrompt("Next", previousContextType);
                    }

                    player.InContext = true;
                }
                else if(currentContext.type == "Read")
                {
                    currentContextType = "Return";
                    userInterfaceManager.UpdateContextPrompt("Return", previousContextType);
                    player.InContext = true;
                }
            }

            if (ContextChanged())
            {
                OnContextChange.Raise();
            }

            previousContext = currentContext;
            previousContextType = currentContextType;
        }
    }

    private bool ContextChanged()
    {
        return previousContext != currentContext || currentContextType != previousContextType;
    }
}
