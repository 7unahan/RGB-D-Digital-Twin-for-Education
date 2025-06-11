using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GazeDetection : MonoBehaviour
{
    public Transform headTransform;  // Assign the student's head (camera or head bone)
    public LineRenderer gazeLine;    // Assign a Line Renderer in the Inspector
    public float maxGazeDistance = 10f; // Max distance the student can see
    public List<Transform> classroomObjects; // List of objects to detect
    public TextMeshPro gazeText; // Floating text above the student

    private Transform currentTarget = null; // Stores the object being looked at
    private Renderer currentRenderer = null; // Stores the object's renderer
    private Color originalColor; // Stores the object's original color
    private Vector3 textOffset = new Vector3(0, 0.5f, 0); // Offset above the head

    void Start()
    {
        gazeLine.enabled = false; // Hide gaze line at start
        if (gazeText != null)
        {
            gazeText.text = ""; // Start with empty text
        }
    }

    void Update()
    {
        CheckGaze();
        UpdateTextPosition();
    }

    void CheckGaze()
    {
        if (headTransform == null || gazeLine == null)
        {
            Debug.LogWarning("Missing headTransform or gazeLine reference!");
            return;
        }

        RaycastHit hit;
        Vector3 gazeDirection = headTransform.forward;

        // Cast a ray from the student's head
        if (Physics.Raycast(headTransform.position, gazeDirection, out hit, maxGazeDistance))
        {
            Transform hitObject = hit.transform;
            if (classroomObjects.Contains(hitObject))
            {
                gazeLine.enabled = true;
                gazeLine.SetPosition(0, headTransform.position);
                gazeLine.SetPosition(1, hit.point);

                // Update floating text
                if (gazeText != null)
                {
                    gazeText.text = hitObject.name; // Show object name
                }

                // Only update color if it's a new object
                if (currentTarget != hitObject)
                {
                    ResetPreviousColor(); // Restore old object's color

                    currentTarget = hitObject;
                    currentRenderer = hitObject.GetComponent<Renderer>();

                    if (currentRenderer != null && !GazeTracker.IsBeingLookedAt(currentTarget))
                    {
                        originalColor = currentRenderer.material.color; // Store original color

                        foreach (var mat in currentRenderer.materials)
                        {
                            mat.color = gazeLine.material.color; // Change color
                        }

                        GazeTracker.AddLooker(hitObject); // Register gaze
                    }

                    Debug.Log("Looking at: " + currentTarget.name);
                }
            }
            else
            {
                gazeLine.enabled = false;
                ResetPreviousColor();
                if (gazeText != null) gazeText.text = "";
            }
        }
        else
        {
            gazeLine.enabled = false;
            ResetPreviousColor();
            if (gazeText != null) gazeText.text = "";
        }
    }

    void ResetPreviousColor()
    {
        if (currentRenderer != null)
        {
            GazeTracker.RemoveLooker(currentTarget); // Remove gaze

            // Only reset color if NO ONE is looking anymore
            if (!GazeTracker.IsBeingLookedAt(currentTarget))
            {
                currentRenderer.material.color = originalColor;
            }

            currentRenderer = null;
            currentTarget = null;
        }
    }

void UpdateTextPosition()
    {
        if (gazeText != null && headTransform != null)
        {
            // Keep text above the head
            gazeText.transform.position = headTransform.position + textOffset;
            gazeText.transform.LookAt(Camera.main.transform); // Make sure text faces the camera
            gazeText.transform.Rotate(0, 180, 0); // Fix the mirrored text issue
        }
    }


}
