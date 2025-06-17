using UnityEngine;

public class CollectablleScript : MonoBehaviour
{
    public int iScore = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Add to player score/gamemanager
            Destroy(gameObject);
        }
    }
}
