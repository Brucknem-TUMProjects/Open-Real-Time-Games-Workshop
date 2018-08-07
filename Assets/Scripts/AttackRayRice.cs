using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to spawn an UFO to suck in Ray Rice with its light when the training lesson / record is over
public class AttackRayRice : MonoBehaviour
{
    public GameObject ufoRayRicePrefab;
    public AudioSource source;
    public AudioClip lightning;
    public CloudSpawner spawner;
    public GameObject darkCloudPrefab;
    public float spawnDistance;
    public Vector3 spawnOffset;
    public RayRice rayRice;
    public Material skyboxNormal, skyboxDark;
    public UnityEngine.PostProcessing.PostProcessingProfile profile;
    public float blendSpeed;

    private GameObject ufo;
    private UnityEngine.PostProcessing.BloomModel.Settings bloomSettings;

    // Use this for initialization
    void OnEnable()
    {
        Vector3 spawnPos = transform.position + spawnDistance * transform.forward + spawnOffset.x * transform.right + spawnOffset.y * transform.up + spawnOffset.z * transform.forward;
        ufo = Instantiate(ufoRayRicePrefab, spawnPos, Quaternion.identity, null);
        ufo.GetComponent<FlightAI>().follow = transform;
        ufo.GetComponent<UfoAI>().target = transform.GetChild(0);
        ufo.GetComponent<UfoAI>().player = GetComponent<RayRice>();
        rayRice.ufoLight = ufo.GetComponent<UfoAI>().ufoLight;

        // Change skybox
        StartCoroutine(blendToSkybox(skyboxDark));
    }

    void Start ()
    {
        bloomSettings = profile.bloom.settings;
        bloomSettings.bloom.softKnee = 0.5f;
        bloomSettings.bloom.radius = 4;
    }

    IEnumerator blendToSkybox (Material skybox)
    {
        while (RenderSettings.skybox.GetFloat("_Exposure") < 6)
        {
            RenderSettings.skybox.SetFloat("_Exposure", RenderSettings.skybox.GetFloat("_Exposure") + blendSpeed * Time.deltaTime);
            bloomSettings.bloom.intensity += 0.25f * blendSpeed * Time.deltaTime;
            profile.bloom.settings = bloomSettings;
            yield return new WaitForEndOfFrame();
        }
        source.PlayOneShot(lightning, 1.0f);
        spawner.clouds[0] = darkCloudPrefab;
        RenderSettings.skybox.SetFloat("_Exposure", 1);
        skybox.SetFloat("_Exposure", 6);
        UnityEngine.RenderSettings.skybox = skybox;
        while (RenderSettings.skybox.GetFloat("_Exposure") > 1)
        {
            RenderSettings.skybox.SetFloat("_Exposure", RenderSettings.skybox.GetFloat("_Exposure") - blendSpeed * Time.deltaTime);
            bloomSettings.bloom.intensity -= 0.25f * blendSpeed * Time.deltaTime;
            profile.bloom.settings = bloomSettings;
            yield return new WaitForEndOfFrame();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    void OnDestroy ()
    {
        skyboxNormal.SetFloat("_Exposure", 1);
        skyboxDark.SetFloat("_Exposure", 1);
        bloomSettings.bloom.intensity = 0;
        profile.bloom.settings = bloomSettings;
    }
}
