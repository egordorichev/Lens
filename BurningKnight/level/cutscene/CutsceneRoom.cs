using BurningKnight.assets.prefabs;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.tile;

namespace BurningKnight.level.cutscene {
	public class CutsceneRoom : ExitRoom {
		private Prefab prefab;
		private string prefabId;

		public CutsceneRoom(int id) {
			prefabId = $"cutscene_{id}";
			prefab = Prefabs.Get(prefabId);
		}
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, Tile.WallA);
			Painter.Prefab(level, prefabId, Left + 1, Top + 1);
		}

		public override bool ConvertToEntity() {
			return false;
		}

		public override void PaintFloor(Level level) {
			
		}

		public override int GetMinWidth() {
			return prefab.Level.Width;
		}

		public override int GetMaxWidth() {
			return prefab.Level.Width + 1;
		}

		public override int GetMinHeight() {
			return prefab.Level.Height;
		}
		
		public override int GetMaxHeight() {
			return prefab.Level.Height + 1;
		}
	}
}