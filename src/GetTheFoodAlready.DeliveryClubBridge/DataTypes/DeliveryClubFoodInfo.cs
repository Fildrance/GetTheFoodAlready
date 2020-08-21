using System.Collections.Generic;

namespace GetTheFoodAlready.DeliveryClubBridge.DataTypes
{
	public class DeliveryClubFoodInfo
	{
		public IList<MenuItem> Products { get; set; }
		public IList<MenuCategory> Menu { get; set; }
	}
	public class MenuCategory
	{
		public IList<string> ProductIds { get; set; }
		public string Name { get; set; }
	}

	public class InventoryId
	{
		public string Inventory { get; set; }
		public string Primary { get; set; }
	}

	public class Images
	{
		public string p1000 { get; set; }
		public string p200 { get; set; }
		public string p650 { get; set; } 
	}

	public class ItemPrice
	{
		public string Currency { get; set; }
		public int Value { get; set; }
	}

	public class ItemProperties
	{
		/// <summary> Item volume in milliliters.</summary>
		public int Volume { get; set; }
		/// <summary> Item weight in gramms. </summary>
		public string Weight { get; set; }
	}

	public class MenuItem
	{
		public string Description { get; set; }
		public InventoryId Id { get; set; }
		public Images Images { get; set; }
		public string Name { get; set; }
		public ItemPrice Price { get; set; }
		public ItemProperties ItemProperties { get; set; }
	}
}