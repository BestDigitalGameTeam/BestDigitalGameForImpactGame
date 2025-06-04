using UnityEngine;

public class OrbScript : MonoBehaviour
{
    private float fTime = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fTime += 1 * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, (float)(0.25 * Mathf.Sin(fTime*2) + 1.75), transform.position.z);
    }   
}
