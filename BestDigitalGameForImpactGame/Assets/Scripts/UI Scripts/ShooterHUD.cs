using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShooterHUD : Singleton<ShooterHUD>
{
    [SerializeField] private Slider m_HealthBar;
    [SerializeField] private AnimationCurve m_HealthbarEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField, Range(0, 0.25f)] private float AnimationDuration = 0.1f;
    public TMP_Text AmmoCountText;
    [SerializeField] private Image[] WeaponImages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AddListeners());
    }

    private IEnumerator AddListeners()
    {
        // wait one frame before adding listener to make sure it is loaded
        yield return null;
        PlayerController.Instance.GetComponent<PlayerHealth>()?.PlayerHealthChanged.AddListener(ChangeHealthValue);
    }

    private void ChangeHealthValue(float _newHealth)
    {
        StartCoroutine(UpdateHealthBar(_newHealth));
    }

    private IEnumerator UpdateHealthBar(float _healthValue)
    {
        float startValue = m_HealthBar.value;
        float endValue = _healthValue;

        float time = 0;

        while (time < AnimationDuration)
        {
            time += Time.deltaTime;

            float lerpValue = m_HealthbarEase.Evaluate(time / AnimationDuration);
            m_HealthBar.value = Mathf.Lerp(startValue, endValue, lerpValue);

            yield return null;
        }

    }

    public void ChangeActiveGun(int _key)
    {
        for (int i = WeaponImages.Length - 1; i >= 0; i--)
        {
            WeaponImages[i].enabled = (_key == i);
        }
    }

    public void ChangeAmmoCount(int _amt)
    {
        AmmoCountText.SetText(_amt.ToString());
    }
}
