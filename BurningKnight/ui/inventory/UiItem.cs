using BurningKnight.assets;
using Lens.assets;
using Lens.graphics;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.ui.inventory {
	/*
	 * TODO: support rerolling and removing
	 */
	public class UiItem : UiEntity {
		public static UiItem Hovered;
		
		public readonly string Id;

		private TextureRegion region;
		public string Name;
		public string Description;
		public Vector2 NameSize;
		public Vector2 DescriptionSize;
		
		private string countStr;
		private int count;
		private int countW;
		private int countH;

		private float border;

		private Vector2 iconScale = Vector2.One;

		public int Count {
			get => count;

			set {
				count = value;
				countStr = $"{count}";
				
				var s = Font.Small.MeasureString(countStr);
				countW = (int) s.Width;
				countH = (int) s.Height;

				iconScale.X = 0;
				iconScale.Y = 2;

				Tween.To(1, iconScale.X, x => iconScale.X = x, 0.3f);
				Tween.To(1, iconScale.Y, x => iconScale.Y = x, 0.3f);
			}
		}

		public UiItem(string item) {
			Name = Locale.Get(item);
			Description = Locale.Get($"{item}_desc");
			region = CommonAse.Items.GetSlice(item);

			Id = item;

			NameSize = Font.Small.MeasureString(Name);
			DescriptionSize = Font.Small.MeasureString(Description);

			Width = region.Width;
			// Height = region.Height;

			Count = 1;
			ScaleMod = 2;
		}

		public override void Destroy() {
			base.Destroy();

			if (Hovered == this) {
				Hovered = null;
			}
		}

		protected override void OnHover() {
			base.OnHover();
			
			Tween.To(1, border, x => border = x, 0.3f);
			Audio.PlaySfx("moving", 0.5f);

			Hovered = this;
		}

		protected override void OnUnhover() {
			base.OnUnhover();
			
			Tween.To(0, border, x => border = x, 0.3f);

			if (Hovered == this) {
				Hovered = null;
			}
		}

		public override void Render() {
			if (border > 0.01f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(border);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(region, Center + d, 0, region.Center, iconScale * scale);
				}

				Shaders.End();
			}
			
			Graphics.Render(region, Center, 0, region.Center, iconScale * scale);
			
			if (count < 2) {
				return;
			}
			
			Graphics.Print(countStr, Font.Small, (int) (X + Width - countW), (int) (Y + Height + 6 - countH));
		}
	}
}