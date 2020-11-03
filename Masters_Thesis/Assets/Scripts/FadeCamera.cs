using UnityEngine;
 
using System.Collections;
using UnityEngine.UI;

public class FadeCamera : MonoBehaviour {
	public static FadeCamera current;

	public delegate void Callback();

	[SerializeField] bool debug;
	[SerializeField] GameObject debugTools;
	[SerializeField] Image overlay;

	float FADE_DELAY = 0f;
	float FADE_TIME = 1f;

	void Awake () {
		current = this;

		if (debug) {
			debugTools.SetActive(true);
			GetComponent<UnityEngine.UI.GraphicRaycaster>().enabled = true;
		}
	}

	void Start () {
		FadeIn();
	}

	public void FadeIn (Callback newCallback = null, float time = 1f, float delay = 0f) {
		// Stop all current coroutines
		StopAllCoroutines();

		// Start new coroutine
		StartCoroutine(FadeInDelayCoroutine(newCallback, time, delay));
	}

	public void FadeOut (Callback newCallback = null, float time = 1f, float delay = 0f) {
		StopAllCoroutines();
		StartCoroutine(FadeOutDelayCoroutine(newCallback, time, delay));
	}

	public void FadeInTest () {
		current.FadeIn();
	}
	
	public void FadeOutTest () {
		current.FadeOut();
	}

	void Log (string message) {
		if (!debug) return;
		Debug.Log(message);
	}

	IEnumerator FadeOutDelayCoroutine (Callback newCallback, float time, float delay) {
		Log("Fade Out Started");

		yield return new WaitForSeconds(delay);
		
		// Offset the timer based on current alpha
		float timer = overlay.color.a;
		Log("Fade Out Loop");
		Log("Timer: " + timer);
		
		Color c;
		while (timer <= 1f) {
			timer += Time.deltaTime / time;
			
			c = overlay.color;
			c.a = timer;
			overlay.color = c;
			
			yield return null;
		}

		// Force color to 1 in-case of rounding error
		c = overlay.color;
		c.a = 1f;
		overlay.color = c;
		
		if (newCallback != null) {
			newCallback();
		}

		Log("Fade Out Complete");
	}

	IEnumerator FadeInDelayCoroutine (Callback newCallback, float time, float delay) {
		Log("Fade In Started");

		yield return new WaitForSeconds(delay);

		// Offset the timer based on current alpha
		float timer = overlay.color.a;
		Log("Fade In Loop");
		
		Color c;
		while (timer >= 0f) {
			timer -= Time.deltaTime / time;

			c = overlay.color;
			c.a = timer;
			overlay.color = c;

			yield return null;
		}

		// Force color to 1 in-case of rounding error
		c = overlay.color;
		c.a = 0f;
		overlay.color = c;

		if (newCallback != null) {
			newCallback();
		}

		Log("Fade In Complete");
	}

	void OnDestroy () {
		current = null;
	}
}