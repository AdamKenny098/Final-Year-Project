using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Ink.Runtime;


public class DialogueSystem : MonoBehaviour
{
    [Range(-.1f, .1f)]public float typingSpeed = .04f;

    [Header("Dialogue UI")]
    public TextMeshProUGUI dialogueText;
    public DialogueContainer dialoguePanel;
    public GameObject dialogueUI;
    public Story story;
    public TextAsset inkJSONAsset;

    [Header("Choices UI")]
    public GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Typing Audio")]
    public AudioSource audioSource;
    public AudioClip[] dialogueTypingSoundClips;
    [Range(1, 5)]public int frequencyLevel = 2;
    [Range(-3, 3)]public float minPitch = .5f;
    [Range(-3, 3)]public float maxPitch = .5f;
    public bool stopAudioSource;
    public bool makePredictable;

    public static DialogueSystem Instance;

    private Coroutine displayLineCoroutine;
    public bool canContinueToNextLine = true;
    private bool isSkipping = false;
    
    private const string OPENSHOP_TAG = "OPENSHOP";
    private const string CLOSESHOP_TAG = "CLOSESHOP";

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ShopSystem.Instance.shopUI.activeSelf)
            {
                // Close the shop directly
                ShopSystem.Instance.ToggleTrade(false);

                // Also notify Ink logic (optional)
                HandleTags(new List<string> { "CLOSESHOP" });
            }
        }
    }

    public void SkipDialogue()
    {
        if (!canContinueToNextLine)
        {
            isSkipping = true;
            return;
        }

        ContinueStory();
    }

    public void ContinueStory()
    {

        if (story == null)
        {
            return;
        }

        if (!story.canContinue)
        {
            dialogueUI.SetActive(false);
            return;
        }

        if (canContinueToNextLine && story.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            string nextLine = story.Continue();
            HandleTags(story.currentTags);
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
    }


    private void DisplayChoices()
    {
        List<Choice> currentChoices = story.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            return;
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            GameObject buttonObj = choices[index];
            buttonObj.SetActive(true);

            // Update choice text
            choicesText[index].text = choice.text;

            // Remove old listeners, then add new one for this index
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            int capturedIndex = index; // Capture local copy to avoid closure bug
            button.onClick.AddListener(() => MakeChoice(capturedIndex));

            index++;
        }

        // Hide unused buttons
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }

        // Optionally auto-select first choice for keyboard navigation
        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        canContinueToNextLine = false;
        isSkipping = false;

        foreach (char letter in line.ToCharArray())
        {
            if (isSkipping)
            {
                dialogueText.maxVisibleCharacters = line.Length;
                isSkipping = false;
                break;
            }

            else
            {
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }

        }

        canContinueToNextLine = true;

        if(dialogueText.maxVisibleCharacters == line.Length)
        {
            DisplayChoices();
        }
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {

        if (currentDisplayedCharacterCount % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }

            AudioClip soundClip = null;
            if (makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];

                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;

                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
            }

            else
            {
                int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }
            audioSource.PlayOneShot(soundClip);
        }
    }

    public void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            switch (tag)
            {
                case OPENSHOP_TAG:
                    ShopSystem.Instance.ToggleTrade(true);
                    dialogueUI.SetActive(false);
                    break;
                case CLOSESHOP_TAG:
                    ShopSystem.Instance.ToggleTrade(false);
                    dialogueUI.SetActive(true);
                    break;
            }
        }
    }

    public void StartDialogue(NPC npc)
    {
        dialogueUI.SetActive(true);
        dialogueText = dialoguePanel.dialogueText;
        story = new Story(npc.dialogueInkJSON.text);
        ContinueStory();

    }

    public void MakeChoice(int choiceIndex)
    {
        if (story == null)
        {
            return;
        }

        HideChoices();
        story.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    private void HideChoices()
    {
        foreach (GameObject choice in choices)
        {
            choice.SetActive(false);
        }
    }
}