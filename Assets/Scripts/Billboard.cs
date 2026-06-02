using UnityEngine;

// Set the y rotation to face the camera
public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.Rotate(0, 180, 0);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
