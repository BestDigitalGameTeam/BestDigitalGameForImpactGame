using UnityEngine;

public class CollectablleScript : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSrc;
    public int iScore = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioSrc.Play();
            //Add to player score/gamemanager
            Destroy(gameObject);
        }
    }
}
