using UnityEngine;
using UnityEngine.UI;

public class TaskProgressBar : MonoBehaviour
{
    [Header("Progress Bar Components")]
    public Image fillImage;
    public Image backgroundImage;
    public Image borderImage;

    [Header("Progress Settings")]
    public Color fillColor = Color.green;
    public Color fillColorFast = Color.blue;
    public Color fillColorSlow = Color.red;
    public Color backgroundColor = Color.gray;

    public void SetProgress(float progress, float workSpeed = 1f)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp01(progress);
            
            // Color based on work speed
            if (workSpeed >= 1.5f)
                fillImage.color = fillColorFast;
            else if (workSpeed <= 0.5f)
                fillImage.color = fillColorSlow;
            else
                fillImage.color = fillColor;
        }
    }

    public void SetColors(Color fill, Color background)
    {
        if (fillImage != null)
            fillImage.color = fill;
        if (backgroundImage != null)
            backgroundImage.color = background;
    }
}
