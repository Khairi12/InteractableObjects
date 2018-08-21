using UnityEngine;

public class Fade : MonoBehaviour
{
    public float fadeSpeed = 1f;

    private MeshRenderer meshRenderer;
    private Material material;
    private bool fading;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        fading = false;
    }

    public void StartFadeIn()
    {
        fading = true;
        FadeIn();
    }

    public void StartFadeOut()
    {
        fading = true;
        FadeOut();
    }

    private void FadeOut()
    {
        if (material.color.a > 0.1f)
        {
            Color col = new Color(
                material.color.r,
                material.color.g,
                material.color.b,
                material.color.a - Time.deltaTime * fadeSpeed);

            material.SetColor("_Color", col);
        }
        else
        {
            fading = false;
        }
    }

    private void FadeIn()
    {
        if (material.color.a < 0.99f)
        {
            Color col = new Color (
                material.color.r,
                material.color.g,
                material.color.b,
                material.color.a + Time.deltaTime * fadeSpeed);

            material.SetColor("_Color", col);
        }
        else
        {
            fading = false;
        }
    }

    private void Update()
    {
        if (!fading)
        {

        }
    }
}
