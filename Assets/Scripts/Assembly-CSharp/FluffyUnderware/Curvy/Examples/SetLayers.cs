using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class SetLayers : MonoBehaviour
	{
		public LayerSet[] Layers;

		public LayerIgnore[] IgnoreLayers;

		private void Awake()
		{
			LayerSet[] layers = Layers;
			foreach (LayerSet layerSet in layers)
			{
				GameObject[] objects = layerSet.Objects;
				foreach (GameObject gameObject in objects)
				{
					gameObject.gameObject.layer = layerSet.Layer;
				}
			}
			LayerIgnore[] ignoreLayers = IgnoreLayers;
			foreach (LayerIgnore layerIgnore in ignoreLayers)
			{
				Physics.IgnoreLayerCollision(layerIgnore.A, layerIgnore.B);
			}
		}
	}
}
