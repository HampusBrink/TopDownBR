using UnityEngine;

[RequireComponent(typeof(Light))]
public class TorchFlicker : MonoBehaviour
{
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;

    private Light torchLight;
    private float randomIntensity;

    void Start()
    {
        torchLight = GetComponent<Light>();
    }

    void Update()
    {
        randomIntensity = Random.Range(minIntensity, maxIntensity);
        torchLight.intensity = Mathf.Lerp(torchLight.intensity, randomIntensity, flickerSpeed * Time.deltaTime);
    }
}

