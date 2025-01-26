using UnityEngine;

public class ChangeCameraSize : MonoBehaviour
{
    public Camera mainCamera;
    public float defaultProjectionSize = 5.0f;
    public float newProjectionSize = 90.0f;

    void Update()
    {
        ChangeSize();
    }

    public void ChangeSize()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (mainCamera.orthographicSize == defaultProjectionSize)
            {
                mainCamera.orthographicSize = newProjectionSize;
            }
            else
            {
                mainCamera.orthographicSize = defaultProjectionSize;
            }
        }
    }
}
