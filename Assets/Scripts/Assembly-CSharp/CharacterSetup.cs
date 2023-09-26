using UnityEngine;

public class CharacterSetup : MonoBehaviour
{
	public SkinnedMeshRenderer CharacterRenderer;

	public MeshFilter Board;

	public CharacterUpgrade Character { get; set; }

	private void Start()
	{
		if (Main.Instance.TestingLevel || Main.Instance.TutorialLevel || Main.Instance.CreditsLevel)
		{
			Character = new CharacterUpgrade();
			Character.ControlMaxEasingAdd = 0;
			Character.ControlSpeedAdd = 0;
		}
		else
		{
			Character = ((!SceneLoader.Instance.Current.IsChallenge) ? TrickController.Instance.CurrentCharacter : TrickController.Instance.ChallengeCharacter);
			Board.mesh = Character.HoverBoard;
			CharacterRenderer.sharedMesh = Character.CharacterMesh;
			CharacterRenderer.material = Character.CharacterMainMaterial;
		}
	}
}
