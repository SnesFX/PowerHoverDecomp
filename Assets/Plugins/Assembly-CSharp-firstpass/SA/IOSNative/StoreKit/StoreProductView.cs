using System;
using System.Collections.Generic;
using SA.Common.Pattern;

namespace SA.IOSNative.StoreKit
{
	public class StoreProductView
	{
		private int _id;

		private List<string> _ids = new List<string>();

		public int Id
		{
			get
			{
				return _id;
			}
		}

		public event Action Loaded = delegate
		{
		};

		public event Action LoadFailed = delegate
		{
		};

		public event Action Appeared = delegate
		{
		};

		public event Action Dismissed = delegate
		{
		};

		public StoreProductView()
		{
			foreach (string item in IOSNativeSettings.Instance.DefaultStoreProductsView)
			{
				addProductId(item);
			}
			Singleton<PaymentManager>.Instance.RegisterProductView(this);
		}

		public StoreProductView(params string[] ids)
		{
			foreach (string productId in ids)
			{
				addProductId(productId);
			}
			Singleton<PaymentManager>.Instance.RegisterProductView(this);
		}

		public void addProductId(string productId)
		{
			if (!_ids.Contains(productId))
			{
				_ids.Add(productId);
			}
		}

		public void Load()
		{
		}

		public void Show()
		{
		}

		public void OnProductViewAppeard()
		{
			this.Appeared();
		}

		public void OnProductViewDismissed()
		{
			this.Dismissed();
		}

		public void OnContentLoaded()
		{
			Show();
			this.Loaded();
		}

		public void OnContentLoadFailed()
		{
			this.LoadFailed();
		}

		public void SetId(int viewId)
		{
			_id = viewId;
		}
	}
}
