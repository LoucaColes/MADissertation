using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that allows screenshots to be taken
/// </summary>
public class Screenshot : MonoBehaviour
{
    // Designer variables
    public string m_filename;
    public int m_superSize = 1;
    public KeyCode m_screenshotKey;

    // Update is called once per frame
    private void Update()
    {
        // If the screenshot button is pressed, take a screenshot
        if (Input.GetKeyDown(m_screenshotKey))
        {
            ScreenCapture.CaptureScreenshot(m_filename + ".png", m_superSize);
        }
    }
}