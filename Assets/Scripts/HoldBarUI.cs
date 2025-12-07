using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class HoldBarUI: MonoBehaviour
{
    public GameObject holdBarRoot;   // assign the parent background GameObject (HoldBar_Back)
    public Image fillImage;          // assign the fill Image (HoldBar_Fill) - Image.Type MUST be Filled
    public Text timeText;            // optional text showing seconds

    // LeanTween ids so we can cancel
    int valueTweenId = -1;
    int fadeTweenId = -1;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // hide at start
        if (holdBarRoot != null) holdBarRoot.SetActive(false);
        if (fillImage != null) fillImage.type = Image.Type.Filled; // enforce
    }

    /// <summary>
    /// Start showing and animating the bar from full -> empty over `duration` seconds.
    /// If a previous tween exists it will be cancelled and replaced.
    /// </summary>
    public void StartHold(float duration)
    {
        if (holdBarRoot == null || fillImage == null) return;

        // ensure the bar is active and fully visible
        holdBarRoot.SetActive(true);
        canvasGroup.alpha = 0f;
        if (fadeTweenId != -1) LeanTween.cancel(fadeTweenId);
        fadeTweenId = LeanTween.alphaCanvas(canvasGroup, 1f, 0.15f).setEaseLinear().id;

        // cancel any existing value tween
        if (valueTweenId != -1) LeanTween.cancel(valueTweenId);

        // start value tween from 1 -> 0 over duration
        fillImage.fillAmount = 1f;
        valueTweenId = LeanTween.value(gameObject, 1f, 0f, duration)
            .setEaseLinear()
            .setOnUpdate((float v) =>
            {
                // apply value
                fillImage.fillAmount = v;
                if (timeText != null)
                {
                    // optional: show seconds left
                    float secondsLeft = Mathf.Ceil(v * duration);
                    timeText.text = secondsLeft.ToString();
                }
            })
            .setOnComplete(() =>
            {
                // when complete, hide the bar (quick fade)
                valueTweenId = -1;
                FadeOutAndDisable();
            }).id;
    }

    /// <summary>
    /// Stop and hide the bar immediately (used on manual drop).
    /// </summary>
    public void StopHold()
    {
        // cancel value tween
        if (valueTweenId != -1) { LeanTween.cancel(valueTweenId); valueTweenId = -1; }
        // cancel fade tween
        if (fadeTweenId != -1) { LeanTween.cancel(fadeTweenId); fadeTweenId = -1; }
        // hide immediately with a very small fade
        FadeOutAndDisable();
    }

    void FadeOutAndDisable()
    {
        if (fadeTweenId != -1) LeanTween.cancel(fadeTweenId);
        fadeTweenId = LeanTween.alphaCanvas(canvasGroup, 0f, 0.12f)
            .setOnComplete(() =>
            {
                if (holdBarRoot != null) holdBarRoot.SetActive(false);
                fadeTweenId = -1;
            }).id;
    }
}
