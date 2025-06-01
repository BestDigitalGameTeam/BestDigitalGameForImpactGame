using UnityEngine;

public class FallAwayScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Add destruction Animation?
            Destroy(gameObject);
        }
    }
}
