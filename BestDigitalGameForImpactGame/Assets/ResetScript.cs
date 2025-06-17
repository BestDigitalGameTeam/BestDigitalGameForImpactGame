using System.Collections;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        StartCoroutine(ResetEverything());
    }

    private IEnumerator ResetEverything()
    {
        yield return null;
        yield return null;
        GameManager.Instance.ResetGame.Invoke();
    }
}
