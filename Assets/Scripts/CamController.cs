using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour {

    public float sensitivity;

    private float xRot;
    private float yRot;

	// Update is called once per frame
	void Update ()
    {
        xRot += Input.GetAxis("Mouse Y") * sensitivity * -1;
        yRot += Input.GetAxis("Mouse X") * sensitivity;

        xRot = Mathf.Clamp(xRot, -70, 35);
        transform.localEulerAngles = new Vector3(xRot, yRot, 0);
    }
}
