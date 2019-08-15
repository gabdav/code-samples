using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour {
    Camera viewCamera;
    public float moveSpeed = 5;
    PlayerController controller;
    public Renderer rend;
    public Color p;
    public Light lt;

    public float maxIntensity = 2.0f;
    public float minIntensity = 0f;
    public float pulseSpeed = 1;

    private float targetIntensity = 2.0f;
    private float currentIntensity;

    public Lose enemy;

    void Start () {
        controller = GetComponent<PlayerController>();
        viewCamera = Camera.main;
        rend = GetComponent<Renderer>();
        lt = GetComponent<Light>();
      
    }
	
	void Update () {

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);//returns the position of the mouse with a ray
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) // asigns to rayDistance ray
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Quaternion targetRotation = Quaternion.LookRotation(point - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7f * Time.deltaTime);

        }

        if (!controller.hit)
        {
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);

            if (Input.GetKey(KeyCode.Space))
            {
                currentIntensity = Mathf.MoveTowards(p[2], targetIntensity, Time.deltaTime * pulseSpeed);
                if (currentIntensity >= maxIntensity)
                {
                    currentIntensity = maxIntensity;

                }

            }
            else
            {
                currentIntensity = Mathf.MoveTowards(currentIntensity, minIntensity, Time.deltaTime * pulseSpeed * 2); ;
            }

        lt.intensity = currentIntensity*2;
        p[1] = currentIntensity / 2;
        p[2] = currentIntensity;
        rend.material.SetColor("_EmissionColor", p);
        }
        else
        {

            currentIntensity = maxIntensity;
            lt.intensity = currentIntensity * 2;
            p[0] = currentIntensity;
            p[1] = 0;
            p[2] = 0;
            rend.material.SetColor("_EmissionColor", p);
        }

    }

 }
