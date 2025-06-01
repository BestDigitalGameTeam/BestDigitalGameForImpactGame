using UnityEngine;

public class FallAwayScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Add destruction Animation?
            Invoke(nameof(SelfDestruct),1.0f);
        }
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
