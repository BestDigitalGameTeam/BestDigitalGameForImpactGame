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
    [SerializeField] Canvas PauseCanvas;
    public AudioSource m_AudioSource;

    public ShooterHUD m_FPSHUD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SubtitleText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (AnnouncerAlgorithm.Instance)
        {
            AnnouncerAlgorithm.Instance.AnnouncerDialogue.AddListener(ShowDialogueSubtitle);
            AnnouncerAlgorithm.Instance.DialogueEnded.AddListener(HideDialogueSubtitle);
        }
        else
        {
            Debug.LogWarning("AnnouncerAlgorithm Not Set");
        }
        

        ShowInteractUI.AddListener(ShowInteractText);
        HideInteractUI.AddListener(HideInteractText);
        GameManager.Instance.PauseGame.AddListener(ShowPauseMenu);
        PauseCanvas.enabled = false;
        m_FPSHUD = GetComponentInChildren<ShooterHUD>();
        m_FPSHUD.gameObject.SetActive(false);
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

    private void ShowPauseMenu(bool _show)
    {
        if (_show)
        {
            PauseCanvas.enabled = true;
        }
        else
        {
            PauseCanvas.enabled = false;
        }
    }

    public void PlayUIAudio(AudioClip _audio)
    {
        m_AudioSource.PlayOneShot(_audio, GameManager.Instance.EffectsVolume);
    }

    public void ChangeWeapon(GameObject _newWeaponPrefab)
    {
        m_FPSHUD.gameObject.SetActive(true);
        m_FPSHUD.ChangeActiveGun(_newWeaponPrefab.GetComponent<Gun>().WeaponHUDKey);
        m_FPSHUD.ChangeAmmoCount(_newWeaponPrefab.GetComponent<Gun>().GetGunData().m_iCurrentAmmo);
    }
    public void UpdateAmmoCount(int _ammo)
    {
        m_FPSHUD.gameObject.SetActive(true);
        m_FPSHUD.ChangeAmmoCount(_ammo);
    }
}
