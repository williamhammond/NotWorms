using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            transform.Translate(new Vector3(0, 100, 0).normalized * Speed * Time.deltaTime);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            transform.Translate(new Vector3(-100, 0, 0).normalized * Speed * Time.deltaTime);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            transform.Translate(new Vector3(0, -100, 0).normalized * Speed * Time.deltaTime);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            transform.Translate(new Vector3(100, 0, 0).normalized * Speed * Time.deltaTime);
        }

        Camera.main.orthographicSize -= Input.mouseScrollDelta.y;
        Camera.main.orthographicSize = Mathf.Max(1, Camera.main.orthographicSize);
    }
}
