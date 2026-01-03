using UnityEngine;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    // Change this to an array
    public SpriteRenderer[] spriteRenderers;
    public float fadeDuration = 0.5f;

    // fadeDirection = true → fade in, false → fade out
    public void Fade(bool fadeDirection)
    {
        float targetAlpha = fadeDirection ? 1f : 0f;

        // Loop through all sprite renderers
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.DOFade(targetAlpha, fadeDuration);
        }
    }
}
