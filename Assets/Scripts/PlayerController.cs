using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float walkSpeed;
    public float sprintMultiplier;
    public float rotationLerpSpeed;
    public bool invertY = false;

    public Light flashlight;
    public Camera cam;
    public Animator animator;

    private float currentSpeed;

    private void Start()
    {
        if (!isLocalPlayer) { return; }

        animator = GetComponent<Animator>();
        Camera.main.gameObject.SetActive(false);
        cam.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovement();
        HandleInput();
    }

    void HandleMovement()
    {
        Vector3 direction = ((cam.transform.forward * Input.GetAxisRaw("Vertical")) + (cam.transform.right * Input.GetAxisRaw("Horizontal"))).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
            CmdSetAnimationBool("Walk", true);
        else
            CmdSetAnimationBool("Walk", false);

        MovePlayer(direction, currentSpeed, invertY);
    }

    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            CmdToggleFlashlight();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
            CmdSetAnimationBool("Sprint", true);
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = walkSpeed;
            CmdSetAnimationBool("Sprint", false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            CmdSetAnimationBool("Aim", true);
        } else if (Input.GetMouseButtonUp(1))
            CmdSetAnimationBool("Aim", false);
    }

    /*
     SERVER AND CLIENT COMMANDS/RPCS (COMMAND CALLED BY CLIENT AND RUN ON SERVER - RPC CALLED BY SERVER AND RUN ON ALL CLIENTS)
         */

    // Move the player by a step towards the direction of the camera, interpolate between current rotation and camera rotation by lerpSpeed
    void MovePlayer(Vector3 _direction, float step, bool invert = false)
    {
        if (!invert)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + _direction, Time.deltaTime * step);
            transform.forward = Vector3.Lerp(transform.forward, _direction, Time.deltaTime * rotationLerpSpeed);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position - _direction, Time.deltaTime * step * -1);
            transform.forward = Vector3.Lerp(transform.forward, cam.transform.forward, Time.deltaTime * rotationLerpSpeed);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        }
    }

    // Animation Cmd/Rpc
    [Command]
    void CmdSetAnimationBool(string param, bool value)
    {
        RpcOnWalk(param, value);
    }

    [ClientRpc]
    void RpcOnWalk(string _param, bool _value)
    {
        animator.SetBool(_param, _value);
    }

    // Toggle flashlight for all players
    [Command]
    void CmdToggleFlashlight()
    {
        RpcSetFlashlight();
    }

    [ClientRpc]
    void RpcSetFlashlight()
    {
        flashlight.enabled = !flashlight.enabled;
    }


}
