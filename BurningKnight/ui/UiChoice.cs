using Lens.assets;
using Lens.input;

namespace BurningKnight.ui {
	public class UiChoice : UiButton {
		private float cx;
		private int option;
		
		public int Option {
			get => option;

			set {
				option = value;

				if (Options != null) {
					Label = $"{Locale.Get(Name)}: {Locale.Get(Options[Option])}";
					CenterX = cx;
				}
			}
		}
		
		public string Name = "";
		public string[] Options;

		
		public override void Init() {
			base.Init();

			cx = RelativeCenterX;
			Label = $"{Locale.Get(Name)}: {Locale.Get(Options[Option])}";
			RelativeCenterX = cx;
		}

		protected override void OnClick() {
			var o = Option + (Input.Mouse.CheckRightButton ? -1 : 1);

			if (o < 0) {
				Option = Options.Length - 1;
			} else if (o >= Options.Length) {
				Option = 0;
			} else {
				Option = o;
			}
			
			base.OnClick();
		}
	}
}