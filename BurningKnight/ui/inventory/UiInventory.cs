using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.inventory {
	public class UiInventory : UiEntity {
		public TextureRegion ItemSlot;
		public TextureRegion UseSlot;
		
		private TextureRegion bomb;
		private TextureRegion key;
		private TextureRegion coin;

		public static TextureRegion Heart;
		public static TextureRegion HalfHeart;
		public static TextureRegion HeartBackground;
		private TextureRegion changedHeartBackground;
		private static TextureRegion halfHeartBackground;
		private TextureRegion changedHalfHeartBackground;
		
		public Player Player;

		private int coins;
		private int keys;
		private int bombs;
		
		private Vector2 coinScale = new Vector2(1);
		private Vector2 keyScale = new Vector2(1);
		private Vector2 bombScale = new Vector2(1);

		private List<UiItem> items = new List<UiItem>();
		private UiActiveItemSlot activeSlot;
		
		public UiInventory(Player player) {
			Player = player;	
			activeSlot = new UiActiveItemSlot(this);
		}

		public override void Init() {
			base.Init();
			
			Area.Add(activeSlot);

			var anim = Animations.Get("ui");

			ItemSlot = anim.GetSlice("item_slot");
			UseSlot = new TextureRegion();
			UseSlot.Set(ItemSlot);
			
			bomb = anim.GetSlice("bomb");
			key = anim.GetSlice("key");
			coin = anim.GetSlice("coin");
			
			Heart = anim.GetSlice("heart");
			HalfHeart = anim.GetSlice("half_heart");
			HeartBackground = anim.GetSlice("heart_bg");
			changedHeartBackground = anim.GetSlice("heart_hurt_bg");
			halfHeartBackground = anim.GetSlice("half_heart_bg");
			changedHalfHeartBackground = anim.GetSlice("half_heart_hurt");
			
			if (Player != null) {
				var component = Player.GetComponent<ConsumablesComponent>();

				coins = component.Coins;
				keys = component.Keys;
				bombs = component.Bombs;

				var area = Player.Area;

				Subscribe<ConsumableAddedEvent>(area);
				Subscribe<ConsumableRemovedEvent>(area);
				Subscribe<ItemUsedEvent>(area);
				Subscribe<ItemAddedEvent>(area);

				var inventory = Player.GetComponent<InventoryComponent>();

				foreach (var item in inventory.Items) {
					AddArtifact(item);
				}
			}
		}

		private void AnimateConsumableChange(int amount, int now, ItemType type) {
			if (type == ItemType.Bomb) {
				if (Math.Abs(amount) == 1) {
					bombs = now;
				} else {
					Tween.To(this, new {bombs = now}, 0.05f * Math.Abs(amount));					
				}
				
				Tween.To(0.3f, bombScale.X, x => bombScale.X = x, 0.1f).OnEnd = () =>
					Tween.To(1f, bombScale.X, x => bombScale.X = x, 0.2f);

				Tween.To(2f, bombScale.Y, x => bombScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, bombScale.Y, x => bombScale.Y = x, 0.2f);
			} else if (type == ItemType.Key) {
				if (Math.Abs(amount) == 1) {
					keys = now;
				} else {
					Tween.To(this, new {keys = now}, 0.05f * Math.Abs(amount));					
				}
				
				Tween.To(0.3f, keyScale.X, x => keyScale.X = x, 0.1f).OnEnd = () =>
					Tween.To(1f, keyScale.X, x => keyScale.X = x, 0.2f);

				Tween.To(2f, keyScale.Y, x => keyScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, keyScale.Y, x => keyScale.Y = x, 0.2f);
			} else if (type == ItemType.Coin) {
				if (Math.Abs(amount) == 1) {
					coins = now;
				} else {
					Tween.To(this, new {coins = now}, 0.05f * Math.Abs(amount));					
				}
				
				Tween.To(2f, coinScale.Y, x => coinScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, coinScale.Y, x => coinScale.Y = x, 0.2f);
			}
		}
		
		public override bool HandleEvent(Event e) {
			switch (e) {
				case ConsumableAddedEvent add: {
					AnimateConsumableChange(add.Amount, add.TotalNow, add.Type);
					break;
				}

				case ConsumableRemovedEvent rem: {
					AnimateConsumableChange(rem.Amount, rem.TotalNow, rem.Type);
					break;
				}
				
				case ItemAddedEvent iae: {
					if (iae.Who == Player) {
						var item = iae.Item;
								
						if (item.Type == ItemType.Artifact) {
							AddArtifact(item);
						}
					}
					
					break;
				}
			}

			return base.HandleEvent(e);
		}
		
		private void AddArtifact(Item item) {
			UiItem old = null;

			foreach (var i in items) {
				if (i.Id == item.Id) {
					old = i;
					break;
				}
			}

			if (old == null) {
				var x = Display.UiWidth - 4f;

				if (items.Count > 0) {
					x = items[items.Count - 1].X - 4;
				}
								
				old = new UiItem(item.Id);
				Area.Add(old);
				items.Add(old);

				old.Right = x;
				old.Bottom = Display.UiHeight - 4f;
			} else {
				old.Count++;
			}
		}		
		public override void Render() {
			if (Player == null || Player.Done || (Run.Depth < 1 && Run.Depth != -2)) {
				Done = true;
				return;
			}

			// Seriously tmp solution about hiding the inventory ui, need to tween it all away!!!!
			if (Player.GetComponent<HealthComponent>().Dead || Engine.Instance.State.Paused) {
				return;
			}

			var show = Run.Depth > 0;

			if (show && Player != null) {
				RenderConsumables();

				if (UiItem.Hovered != null) {
					var item = UiItem.Hovered;
					
					var x = Math.Min(Display.UiWidth - 4 - Math.Max(item.DescriptionSize.X, item.NameSize.X), item.Position.X);
					
					Graphics.Print(item.Name, Font.Small,  new Vector2(x, item.Position.Y - item.DescriptionSize.Y - item.NameSize.Y));
					Graphics.Print(item.Description, Font.Small, new Vector2(x, item.Position.Y - item.DescriptionSize.Y));
				}
			}
			
			RenderHealthBar(show);
		}

		private Vector2 GetHeartPosition(bool pad, int i, bool bg = false) {
			var d = 0;
			var it = Player.GetComponent<ActiveItemComponent>().Item;

			if (pad && it != null && Math.Abs(it.UseTime) > 0.01f) {
				d = 4;
			}
			
			var a = 0;
			
			if (it == null && (coins != 0 || bombs != 0 || keys != 0)) {
				a += 24;
			}

			return new Vector2(
				(bg ? 0 : 1) + (pad ? (2 + (2 + ItemSlot.Source.Width + d) * (activeSlot.ActivePosition + 1) + a) : 6) + (int) (i % HeartsComponent.PerRow * 5.5f),
				Display.UiHeight - (bg ? 11 : 10) - (i / HeartsComponent.PerRow) * 10 - (pad ? (-activeSlot.ActivePosition * 2) : 4)
				+ (float) Math.Cos(i / 8f * Math.PI + Engine.Time * 12) * 0.5f * Math.Max(0, (float) (Math.Cos(Engine.Time * 0.25f) - 0.9f) * 10f)
			);
		}

		private float lastRed;
		
		private void RenderHealthBar(bool pad) {
			var red = Player.GetComponent<HealthComponent>();
			var totalRed = red.Health;

			if (lastRed > totalRed) {
				lastRed = totalRed;
			} else if (lastRed < totalRed) { 
				lastRed = Math.Min(totalRed, lastRed + Engine.Delta * 30);
			}

			var r = (int) lastRed;
			var maxRed = red.MaxHealth;
			var hurt = red.InvincibilityTimer > 0;

			int i = 0;
			
			for (; i < maxRed; i += 2) {
				var region = hurt ? changedHeartBackground : HeartBackground;

				if (i == maxRed - 1) {
					region = hurt ? changedHalfHeartBackground : halfHeartBackground;
				}
				
				Graphics.Render(region, GetHeartPosition(pad, i, true));
			}

			var n = r;

			for (var j = 0; j < n; j++) {
				var h = j % 2 == 0;
				Graphics.Render(h ? HalfHeart : Heart, GetHeartPosition(pad, j) + (h ? Vector2.Zero : new Vector2(-1, 0)));
			}
		}

		private void RenderConsumables() {
			var bottomY = Display.UiHeight - ItemSlot.Source.Height * (activeSlot.ActivePosition + 1) - 16;

			if (coins > 0) {
				Graphics.Render(coin, new Vector2(4 + coin.Center.X, bottomY + 1 + coin.Center.Y), 0, coin.Center, coinScale);
				Graphics.Print($"{coins}", Font.Small, new Vector2(14, bottomY - 1));
				bottomY -= 12;
			}

			if (keys > 0) {
				Graphics.Render(key, new Vector2(4 + key.Center.X, bottomY + key.Center.Y), 0, key.Center, keyScale);
				Graphics.Print($"{keys}", Font.Small, new Vector2(14, bottomY - 1));
				bottomY -= bomb.Source.Height + 2;
			}

			if (bombs > 0) {
				// Bomb sprite has bigger height
				Graphics.Render(bomb, new Vector2(4 + bomb.Center.X, bottomY + bomb.Center.Y), 0,
					bomb.Center, bombScale);

				Graphics.Print($"{bombs}", Font.Small, new Vector2(14, bottomY - 1));
			}
		}
	}
}