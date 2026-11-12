using UnityEngine;

public static class CardUIAnimations
{
	public static CinematicFunction CreateRevealAnimationDiagonalFlip(Transform upgradeCardUI, float timeOffset)
	{
		upgradeCardUI.transform.localRotation = Quaternion.Euler(0f, 90f, 25f);
		float wait = timeOffset;
		return new CinematicFunction(delegate(float dt)
		{
			if (upgradeCardUI == null)
			{
				return true;
			}
			wait -= dt;
			if (wait > 0f)
			{
				return false;
			}
			upgradeCardUI.transform.localRotation = Quaternion.Slerp(upgradeCardUI.transform.localRotation, Quaternion.identity, Mathf.Clamp01(dt * 8f));
			if (Mathf.Abs(upgradeCardUI.transform.localRotation.eulerAngles.y) < 0.01f)
			{
				upgradeCardUI.transform.localRotation = Quaternion.identity;
				return true;
			}
			return false;
		}, blocking: false);
	}

	public static CinematicFunction CreateRevealAnimation(Transform upgradeCardUI, float timeOffset)
	{
		upgradeCardUI.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
		float wait = timeOffset;
		return new CinematicFunction(delegate(float dt)
		{
			wait -= dt;
			if (wait > 0f)
			{
				return false;
			}
			upgradeCardUI.transform.localRotation = Quaternion.Slerp(upgradeCardUI.transform.localRotation, Quaternion.identity, Mathf.Clamp01(dt * 8f));
			if (Mathf.Abs(upgradeCardUI.transform.localRotation.eulerAngles.y) < 0.01f)
			{
				upgradeCardUI.transform.localRotation = Quaternion.identity;
				return true;
			}
			return false;
		}, blocking: false);
	}

	public static CinematicFunction CreateHideAnimation(Transform upgradeCardUITransform, float timeOffset)
	{
		return new CinematicFunction(delegate(float dt)
		{
			timeOffset -= dt;
			if (timeOffset > 0f)
			{
				return false;
			}
			upgradeCardUITransform.localRotation = Quaternion.Slerp(upgradeCardUITransform.localRotation, Quaternion.Euler(0f, 90f, 0f), Mathf.Clamp01(dt * 32f));
			return Mathf.Abs(upgradeCardUITransform.localRotation.eulerAngles.y - 90f) < 0.01f;
		}, blocking: false);
	}

	public static CinematicFunction CreateDissolveAnimation(Transform upgradeCardUITransform, float timeOffset)
	{
		UpgradeCardUI upgradeCardUI = upgradeCardUITransform.GetComponent<UpgradeCardUI>();
		return new CinematicFunction(delegate(float dt)
		{
			timeOffset -= dt;
			if (timeOffset > 0f)
			{
				return false;
			}
			upgradeCardUI.SetBlend(Mathf.Lerp(upgradeCardUI.GetBlend(), 0f, Mathf.Clamp01(dt * 4f)));
			return upgradeCardUI.GetBlend() < 0.01f;
		}, blocking: false);
	}

	public static CinematicFunction CreateLockedSelectedAnimation(Transform upgradeCardUI)
	{
		float shakeTime = 0.1f;
		return new CinematicFunction(delegate(float dt)
		{
			shakeTime -= dt;
			upgradeCardUI.transform.localPosition += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 2f;
			return shakeTime < 0f;
		}, blocking: false);
	}

	public static CinematicFunction CreateHighlightAnimation(Transform upgradeCardUI)
	{
		float time = 0.5f;
		float angleY = 360f;
		return new CinematicFunction(delegate(float dt)
		{
			time -= dt;
			angleY = Mathf.Lerp(angleY, 0f, Mathf.Clamp01(dt * 8f));
			upgradeCardUI.transform.localRotation = Quaternion.Euler(0f, angleY, 0f);
			if (time < 0f)
			{
				upgradeCardUI.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				return true;
			}
			return false;
		}, blocking: false);
	}

	public static CinematicFunction CreateMoveAwayAnimation(Transform upgradeCardUI, float timeOffset)
	{
		return new CinematicFunction(delegate(float dt)
		{
			timeOffset -= dt;
			if (timeOffset > 0f)
			{
				return false;
			}
			upgradeCardUI.transform.localPosition += Vector3.up * dt * 4096f;
			return upgradeCardUI.transform.localPosition.y > 1024f;
		}, blocking: false);
	}
}
