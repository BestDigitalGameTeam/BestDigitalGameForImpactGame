using UnityEngine;
using TMPro;
using AYellowpaper.SerializedCollections; // for serialised dictionary

// File authour: Charli
// Manages UI etc
// Including dialogue subtitles

public class UIManager : Singleton<UIManager>
{
    private TextMeshProUGUI m_SubtitleText;
    [SerializedDictionary("Key", "Text")] public SerializedDictionary<int, string> AnnouncerDialogueSubtitles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SubtitleText = GetComponentInChildren<TextMeshProUGUI>(true);
        AnnouncerAlgorithm.Instance.AnnouncerDialogue.AddListener(ShowDialogueSubtitle);
        AnnouncerAlgorithm.Instance.DialogueEnded.AddListener(HideDialogueSubtitle);
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
