﻿using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.item.stand;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class ItemPickupFx : Entity {
		private Item item;
		private bool tweened;
		private float y;
		private TweenTask task;
		
		public ItemPickupFx(Item it) {
			item = it;
			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			string text;

			if (Locale.Current == "de" || Locale.Current == "it") {
				text = item.Hidden ? "???" : (item.Scourged ? $"{item.Name} ({Locale.Get("scourged")})" : item.Name);
			} else {
				text = item.Hidden ? "???" : (item.Scourged ? $"{Locale.Get("scourged")} {item.Name}" : item.Name);
			}

			var size = Font.Medium.MeasureString(text);

			Width = size.Width;
			Height = size.Height;
			
			var component = new TextGraphicsComponent(text);
			AddComponent(component);

			component.Scale = 0;
			task = Tween.To(component, new {Scale = 1.3f}, 0.25f, Ease.BackOut);

			y = 12;
			Tween.To(0, y, x => y = x, 0.2f);
			
			UpdatePosition();

			if (item.Scourged) {
				component.Color = Palette.Default[ItemGraphicsComponent.ScourgedColorId];
			}
		}

		private void UpdatePosition() {
			var yy = 0f;
			
			if (item.Animation == null) {
				var c = item.GetComponent<ItemGraphicsComponent>();
				yy = ItemGraphicsComponent.CalculateMove(c.T) * Display.UiScale;
			}
			
			Center = Camera.Instance.CameraToUi(new Vector2(item.CenterX, item.Y - 8 + y + yy));
			GetComponent<TextGraphicsComponent>().Angle = (float) (Math.Cos(Engine.Instance.State.Time) * 0.05f);
		}
		
		public override void Render() {
			if (!Engine.Instance.State.Paused) {
				base.Render();
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			UpdatePosition();

			if (!tweened) {
				if (!item.TryGetComponent<InteractableComponent>(out var component) || component.CurrentlyInteracting == null) {
					if (item.TryGetComponent<OwnerComponent>(out var owner) && owner.Owner is ItemStand stand && stand.GetComponent<InteractableComponent>().CurrentlyInteracting != null) {
						return;
					}

					task.Ended = true;
					
					Tween.To(GetComponent<TextGraphicsComponent>(), new {Scale = 0}, 0.2f).OnEnd = () => Done = true;
					Tween.To(12, y, x => y = x, 0.5f);
					
					tweened = true;
				}
			}
		}
	}
}