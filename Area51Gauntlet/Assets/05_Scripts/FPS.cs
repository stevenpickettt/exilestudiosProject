using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    float deltaTime = 0.0f;
    bool showDebug = false;
    Text FpsText;

    void Start()
    {
         FpsText = gameObject.GetComponent<Text>();
    }
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if(Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftShift))
        {
            showDebug = !showDebug;
        }
    
    }

    void OnGUI()
    {
        if (showDebug)
        {
            FpsText.enabled = true;
            float fps = 1.0f / deltaTime;
            int curFps = Mathf.RoundToInt(fps);
            if (curFps >= 30)
                FpsText.color = Color.green;
            else if (curFps < 30 && curFps >= 20)
                FpsText.color = Color.yellow;
            else
                FpsText.color = Color.red;
            FpsText.text = curFps.ToString();
        }
        else
            FpsText.enabled = false;
    }
}
