using UnityEngine;

// Set the y rotation to face the camera
public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.Rotate(0, 180, 0);
    }
}
