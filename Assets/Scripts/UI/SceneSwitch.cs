using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class SceneSwitch : ScriptableObject
{
	public string SceneName;

	public void Switch()
	{
		SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
	}
}
