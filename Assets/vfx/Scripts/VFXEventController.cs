using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using static UnityEditor.SceneView;


[RequireComponent(typeof(VisualEffect))]
public class VFXEventController : MonoBehaviour
{
    [Header("VFX References")]
    public VisualEffect vfxEffect;

    [Header("Event Names")]
    public string startEventName = "StartVortex";
    public string stopEventName = "StopVortex";

    [Header("Fade Settings")]
    public string aliveParameter = "Alive";
    public float fadeDuration = 1.8f;
    public float particleCheckInterval = 0.2f;

    [Header("Input Settings")]
    public KeyCode startKey = KeyCode.E;
    public KeyCode stopKey = KeyCode.R;

    private bool isEffectActive = false;
    private Coroutine cleanupRoutine;
    public VFXCameraController cameraController;

    void Awake()
    {
        if (vfxEffect == null)
            vfxEffect = GetComponent<VisualEffect>();
    }

    void Update()
    {
        if (Input.GetKeyDown(startKey))
        {
            if (!isEffectActive)
            {
                StartVFX();
            }
        }
        else if (Input.GetKeyDown(stopKey))
        {
            if (isEffectActive)
            {
                StopVFX();
            }
        }
    }

    public void StartVFX()
    {
        ForceCleanup(); // Всегда делаем сброс перед запуском

        cameraController?.MoveToTarget();
        vfxEffect.SetFloat(aliveParameter, 1f);
        vfxEffect.SendEvent(startEventName);
        isEffectActive = true;
    }

    public void StopVFX()
    {
        cameraController?.StopMoving();
        vfxEffect.SetFloat(aliveParameter, 0f);
        vfxEffect.SendEvent(stopEventName);
        isEffectActive = false;

        if (cleanupRoutine != null)
            StopCoroutine(cleanupRoutine);

        cleanupRoutine = StartCoroutine(WaitForParticlesDeath());
    }

    IEnumerator WaitForParticlesDeath()
    {
        yield return new WaitForSeconds(fadeDuration);

        while (vfxEffect.aliveParticleCount > 0)
        {
            yield return new WaitForSeconds(particleCheckInterval);
        }

        ForceCleanup();
    }

    void ForceCleanup()
    {
        if (vfxEffect != null)
        {
            vfxEffect.Stop();
            vfxEffect.Reinit();
        }
    }

    void OnDisable()
    {
        if (cleanupRoutine != null)
            StopCoroutine(cleanupRoutine);
    }
}