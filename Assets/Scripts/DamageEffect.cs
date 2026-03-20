using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
       public SpriteRenderer sr;
    public Color flashColor = Color.white;
    public float flashDuration = 0.06f;
    public int flashCount = 2;

    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            originalColor = sr.color;
    }

    public void Flash()
    {
        if (sr == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            sr.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        flashCoroutine = null;
    }
}