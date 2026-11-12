using UnityEngine;

public class FireDashModification : MonoBehaviour, IDashModification
{
	public ValueStackFloat fireLifeTime = new ValueStackFloat(4f);

	private ParticleSystem particleSystem;

	private ParticleSystemRenderer particleSystemRenderer;

	private void Start()
	{
		particleSystem = GetComponentInChildren<ParticleSystem>();
		particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
	}

	private void Update()
	{
		Vector3 vector = Player.Get().GetAttributes().GetElement();
		particleSystemRenderer.sharedMaterial.SetVector("_ReactionScale", new Vector4(vector.x, vector.y, vector.z, 0f) * 0.1f);
	}

	public void OnEndDashing()
	{
	}

	public void OnStartDashing()
	{
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = true;
	}

	public void OnStartRunning()
	{
	}

	public void OnEndRunning()
	{
		ParticleSystem.MainModule main = particleSystem.main;
		main.startLifetime = fireLifeTime.GetCurrentValue();
		ParticleSystem.EmissionModule emission = particleSystem.emission;
		emission.enabled = false;
	}
}
