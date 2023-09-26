using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
	[SerializeField]
	protected int[] m_queues = new int[1] { 3000 };

	protected void Awake()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		Material[] materials = component.materials;
		for (int i = 0; i < materials.Length && i < m_queues.Length; i++)
		{
			materials[i].renderQueue = m_queues[i];
		}
	}
}
