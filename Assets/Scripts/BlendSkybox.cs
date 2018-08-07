using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendSkybox : MonoBehaviour
{
	public Material skyboxFrom, skyboxToBlendIn;
	public GameObject clearCloudPrefab;
	public CloudSpawner spawner;
	public UnityEngine.PostProcessing.PostProcessingProfile profile;
	public float blendSpeed;

	private UnityEngine.PostProcessing.BloomModel.Settings bloomSettings;


	// Use this for initialization
	void Start ()
	{
		bloomSettings = profile.bloom.settings;
		bloomSettings.bloom.softKnee = 0.5f;
		bloomSettings.bloom.radius = 4;
		StartCoroutine(blendToSkybox(skyboxToBlendIn));
		if (!spawner)
			spawner = GameObject.Find ("Cloud Spawner").GetComponent<CloudSpawner>();
	}
	
	// Update is called once per frame
	void Update () {
		
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
		spawner.clouds[0] = clearCloudPrefab;
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

	void OnDestroy ()
	{
		skyboxFrom.SetFloat("_Exposure", 1);
		skyboxToBlendIn.SetFloat("_Exposure", 1);
		bloomSettings.bloom.intensity = 0;
		profile.bloom.settings = bloomSettings;
	}
}
