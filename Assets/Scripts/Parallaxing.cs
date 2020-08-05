using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour
{
    public Transform[] backgrounds; // Array of all backgrounds to be parallaxed
    private float[] parallaxScales; // proportion of camera's movement to move the backgrounds by
    public float parallaxAmount = 1; // How smooth the parallax amount will be, set above 0

    private Transform cam; // main camera's transform
    private Vector3 previousCamPos; // position of camera in previous frame

    // Awake is called before Start but after Game Objects are set up - good for setting up references
    private void Awake()
    {
        // set up camera reference
        cam = Camera.main.transform;
    }

    // Start is called before the first frame update - useful for initialization
    void Start()
    {
        previousCamPos = cam.position; // previous frame had current frame's camera pos
        // assigning corresponding parallax scales
        parallaxScales = new float[backgrounds.Length]; // establish length of parallax scales array
        for (int i = 0; i < backgrounds.Length; i++)
        {
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // parallax is opposite of the camera movement because the previous frame multiplied by the scale
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i]; // take difference in camera movement and multiply it by the parallax scale

            // set a target position for each background which is the current position + parallax
            float backgroundTargetPosX = backgrounds[i].position.x + parallax;

            // create a target position which is the background's current position with its target x position
            Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            // fade between current position and target position using lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, parallaxAmount * Time.deltaTime);
        }
        // Set the previousCamPos to the camera's position at the end of the frame
        previousCamPos = cam.position;
    }
}
