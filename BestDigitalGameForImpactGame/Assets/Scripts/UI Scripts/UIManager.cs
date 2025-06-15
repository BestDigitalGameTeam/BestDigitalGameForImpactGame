using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections; // for serialised dictionary
using UnityEngine.Events;
using UnityEngine.UI; 
// File authour: Charli
// Manages UI etc
// Including dialogue subtitles

public class UIManager : SingletonPersistent<UIManager>
{
    private TextMeshProUGUI m_SubtitleText;
    [SerializedDictionary("Key", "Text")] public SerializedDictionary<int, string> AnnouncerDialogueSubtitles;

    public UnityEvent ShowInteractUI;
    public UnityEvent HideInteractUI;
    public bool IntActive = false;
    [SerializeField] Image InteractImage; // change to text?

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SubtitleText = GetComponentInChildren<TextMeshProUGUI>(true);
        AnnouncerAlgorithm.Instance.AnnouncerDialogue.AddListener(ShowDialogueSubtitle);
        AnnouncerAlgorithm.Instance.DialogueEnded.AddListener(HideDialogueSubtitle);

        ShowInteractUI.AddListener(ShowInteractText);
        HideInteractUI.AddListener(HideInteractText);
    }

    private void ShowDialogueSubtitle(int _key)
    {
        if (AnnouncerDialogueSubtitles.ContainsKey(_key))
        {
            m_SubtitleText.SetText(AnnouncerDialogueSubtitles[_key]);
        }
        else { Debug.Log("Subtitle for this dialogue does not exist!"); }
    }

    private void HideDialogueSubtitle()
    {
        m_SubtitleText.SetText("");
    }

    private void ShowInteractText()
    {
        IntActive = true;
        InteractImage.enabled = true;
    }
    private void HideInteractText()
    {
        IntActive = false;
        InteractImage.enabled = false;
    }
}
