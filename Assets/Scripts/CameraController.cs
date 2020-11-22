using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Transform obstruction;

    float zoomSpeed = 3;
    float rotationSpeed = 1;
    float mouseX, mouseY;

    private void Start()
    {
        obstruction = target;
    }

    private void LateUpdate()
    {
        CamControl();
        // ViewObstructed();
    }

    void CamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(target);
        target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void ViewObstructed()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, 4.5f))
        {
            if (hit.collider.gameObject.tag != "Player")
            {
                obstruction = hit.transform;
                obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if (Vector3.Distance(obstruction.position, transform.position) >= 3f && Vector3.Distance(transform.position, target.position) >= 1.5f)
                    transform.Translate(Vector3.forward * zoomSpeed * Time.deltaTime);
            }
            else
            {
                obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                if (Vector3.Distance(transform.position, target.position) < 4.5f)
                    transform.Translate(Vector3.back * zoomSpeed * Time.deltaTime);
            }
        }
    }
}

