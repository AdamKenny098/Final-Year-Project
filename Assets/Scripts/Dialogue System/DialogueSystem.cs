using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour
{
    [Header("Params")]
    [Range(-.1f, .1f)][SerializeField] public float typingSpeed = .04f;
    [SerializeField] public Slider typingSpeedSlider;

    [Header("Dialogue UI")]
    [SerializeField] public TextMeshProUGUI dialogueText;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] private Story story;
    [SerializeField] public TextAsset inkJSONAsset;
    [SerializeField] public GameObject normalText;
    [SerializeField] public Animator portraitAnimator;
    [SerializeField] public Animator backgroundAnimator;
    
    [Header("Choices UI")]
    [SerializeField] public GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Menus UI")]
    [SerializeField] public GameObject mainMenu;
    public AudioSource mainMenuMusic; 
    public GameObject VN;
    [SerializeField] public GameObject settingsMenu;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip[] musicClips;
    
    [Header("Typing Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] dialogueTypingSoundClips;
    [SerializeField] private Slider beepSlider;
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;
    [Range(1,5)][SerializeField] private int frequencyLevel = 2;
    [Range(-3,3)][SerializeField] private float minPitch = .5f;
    [Range(-3,3)][SerializeField] private float maxPitch = .5f;
    [SerializeField] private bool stopAudioSource;
    [SerializeField] private bool makePredictable;

    private Coroutine displayLineCoroutine;
    private bool canContinueToNextLine = true;
    private bool isSkipping = false;
    private static DialogueSystem instance;
    [SerializeField]private UIManager uiManager;
    [SerializeField] public List<GameObject> endingPanels;


    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string BACKGROUND_TAG = "background";
    private const string MUSIC_TAG = "music";
    

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("2 or more Dialogue Managers");
        }
        instance = this;
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.volume = volume; 
    }

    public void Start()
    {
        story = new Story(inkJSONAsset.text);

        if (story != null)
        {
            story.BindExternalFunction("EndGame", () =>
            {
                EndGame();
            });

            story.BindExternalFunction("ShowEndingScreen", (int endingNumber) =>
{
    Debug.Log("ShowEndingScreen called from Ink with ending number: " + endingNumber);
    ShowEndingScreen(endingNumber);
});


        }

        ContinueStory();

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        audioSource.enabled = true;
        musicSource.enabled = true;
    } 
    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (canContinueToNextLine)
            {
                ContinueStory();
            }
            else
            {
                isSkipping = true;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            uiManager.Settings();
        }

        typingSpeed = typingSpeedSlider.maxValue - typingSpeedSlider.value; 
    }

    private void ContinueStory()
    {
        if (canContinueToNextLine && 
                story.canContinue && 
                !mainMenu.activeSelf && 
                !settingsMenu.activeSelf)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            displayLineCoroutine = StartCoroutine(DisplayLine(story.Continue()));
            DisplayChoices();
            HandleTags(story.currentTags);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    nameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case BACKGROUND_TAG:
                    backgroundAnimator.Play(tagValue);
                    break; 
                case MUSIC_TAG:
                    PlayBackgroundMusic(tagValue);
                    break;
            }
        }
    }


    private void DisplayChoices()
    {
        List<Choice> currentChoices = story.currentChoices;
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices than can be managed");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            normalText.SetActive(false);
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
            normalText.SetActive(true);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }


    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            story.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
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
            if(makePredictable)
            {
                int hashCode = currentCharacter.GetHashCode();
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];

                int minPitchInt = (int) (minPitch * 100);
                int maxPitchInt = (int) (maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;

                if(pitchRangeInt !=0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt /100f;
                    audioSource.pitch = predictablePitch;
                }
            }
            
            else
            {
                int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }
                audioSource.volume = Mathf.Clamp(beepSlider.value, 0.1f, 1f);
                audioSource.PlayOneShot(soundClip);
        }  
    }

    public void PlayBackgroundMusic(string clipName)
    {
        foreach (var clip in musicClips)
        {
            if (clip.name == clipName)
            {
                musicSource.clip = clip;
                musicSource.Play();
                return;
            }
        }
        Debug.LogWarning("Audio clip not found: " + clipName);
    }

    public void EndGame()
    {
        //mainMenuMusic.enabled = true;
        //mainMenu.SetActive(true);
        //VN.SetActive(false);
        audioSource.enabled = false;
        musicSource.enabled = false;
    }

    public void setSpeedSlider(float value)
    {
        typingSpeed = Mathf.Clamp01(value);
    }

    public void ShowEndingScreen(int endingNumber)
{
    
    int index = endingNumber - 1;

    if (index >= 0 && index < endingPanels.Count)
    {
        // First, deactivate all ending panels
        foreach (GameObject panel in endingPanels)
        {
            panel.SetActive(false);
        }

        // Activate the correct ending panel
        endingPanels[index].SetActive(true);
    }
    else
    {
        Debug.LogWarning("Invalid ending number: " + endingNumber);
    }
}
}