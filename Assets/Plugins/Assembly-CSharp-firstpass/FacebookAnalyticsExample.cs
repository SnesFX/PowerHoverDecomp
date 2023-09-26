using UnityEngine;

public class FacebookAnalyticsExample : MonoBehaviour
{
	public void ActivateApp()
	{
		SPFacebookAnalytics.ActivateApp();
	}

	public void AchievedLevel()
	{
		SPFacebookAnalytics.AchievedLevel(1);
	}

	public void AddedPaymentInfo()
	{
		SPFacebookAnalytics.AddedPaymentInfo(true);
	}

	public void AddedToCart()
	{
		SPFacebookAnalytics.AddedToCart(54.23f, "HDFU-8452", "shoes");
	}

	public void AddedToWishlist()
	{
		SPFacebookAnalytics.AddedToWishlist(54.23f, "HDFU-8452", "shoes");
	}

	public void CompletedRegistration()
	{
		SPFacebookAnalytics.CompletedRegistration("Email");
	}

	public void InitiatedCheckout()
	{
		SPFacebookAnalytics.InitiatedCheckout(54.23f, 3, "HDFU-8452", "shoes", true);
	}

	public void Purchased()
	{
		SPFacebookAnalytics.Purchased(54.23f, 3, "shoes", "HDFU-8452");
	}

	public void Rated()
	{
		SPFacebookAnalytics.Rated(4, "HDFU-8452", "shoes", 5);
	}

	public void Searched()
	{
		SPFacebookAnalytics.Searched("shoes", "HD", true);
	}

	public void SpentCredits()
	{
		SPFacebookAnalytics.SpentCredits(120f, "shoes", "HDFU-8452");
	}

	public void UnlockedAchievement()
	{
		SPFacebookAnalytics.UnlockedAchievement("ShoeMan");
	}

	public void ViewedContent()
	{
		SPFacebookAnalytics.ViewedContent(54.23f, "shoes", "HDFU-8452");
	}
}
