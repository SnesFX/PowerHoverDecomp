using System;
using UnityEngine;

namespace SA.Common.Animation
{
	public class ValuesTween : MonoBehaviour
	{
		public bool DestoryGameObjectOnComplete = true;

		private float FinalFloatValue;

		private Vector3 FinalVectorValue;

		public event Action OnComplete = delegate
		{
		};

		public event Action<float> OnValueChanged = delegate
		{
		};

		public event Action<Vector3> OnVectorValueChanged = delegate
		{
		};

		public static ValuesTween Create()
		{
			return new GameObject("SA.Common.Animation.ValuesTween").AddComponent<ValuesTween>();
		}

		private void Update()
		{
			this.OnValueChanged(base.transform.position.x);
			this.OnVectorValueChanged(base.transform.position);
		}

		public void ValueTo(float from, float to, float time, EaseType easeType = EaseType.linear)
		{
			Vector3 position = base.transform.position;
			position.x = from;
			base.transform.position = position;
			FinalFloatValue = to;
			SA_iTween.MoveTo(base.gameObject, SA_iTween.Hash("x", to, "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
		}

		public void VectorTo(Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.linear)
		{
			base.transform.position = from;
			FinalVectorValue = to;
			SA_iTween.MoveTo(base.gameObject, SA_iTween.Hash("position", to, "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
		}

		public void ScaleTo(Vector3 from, Vector3 to, float time, EaseType easeType = EaseType.linear)
		{
			base.transform.localScale = from;
			FinalVectorValue = to;
			SA_iTween.ScaleTo(base.gameObject, SA_iTween.Hash("scale", to, "time", time, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
		}

		public void VectorToS(Vector3 from, Vector3 to, float speed, EaseType easeType = EaseType.linear)
		{
			base.transform.position = from;
			FinalVectorValue = to;
			SA_iTween.MoveTo(base.gameObject, SA_iTween.Hash("position", to, "speed", speed, "easeType", easeType.ToString(), "oncomplete", "onTweenComplete", "oncompletetarget", base.gameObject));
		}

		public void Stop()
		{
			SA_iTween.Stop(base.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void onTweenComplete()
		{
			this.OnValueChanged(FinalFloatValue);
			this.OnVectorValueChanged(FinalVectorValue);
			this.OnComplete();
			if (DestoryGameObjectOnComplete)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}
