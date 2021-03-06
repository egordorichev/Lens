using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.walls {
	public class SegmentedWall : WallPainter {
		private bool fl;
		
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var t = Tiles.RandomSolid();
			fl = Rnd.Chance(10);

			CreateWalls(level, new Rect(room.Left + 1, room.Top + 1, room.Right - 1, room.Bottom - 1), t);
		}

		private void CreateWalls(Level level, Rect area, Tile tile) {
			var w = area.GetWidth();
			var h = area.GetHeight();
			
			if (Math.Max(w + 1, h + 1) < 5 ||
			    Math.Min(w + 1, h + 1) < 5) {

				return;
			}

			var tries = 10;

			if (w > h || (w == h && Rnd.Chance())) {
				do {
					var splitX = Rnd.Int(area.Left + 2, area.Right - 2);

					if (level.Get(splitX, area.Top - 1) == tile && level.Get(splitX, area.Bottom + 1) == tile) {
						tries = 0;
						
						Painter.DrawLine(level, new Dot(splitX, area.Top),
							new Dot(splitX, area.Bottom), tile);

						var space = Rnd.Int(area.Top, area.Bottom - 1);
						var f = fl ? Tile.SensingSpikeTmp : Tiles.RandomFloorOrSpike();
						
						Painter.Set(level, splitX, space, f);
						Painter.Set(level, splitX, space + 1, f);
						
						CreateWalls(level, new Rect(area.Left, area.Top, splitX - 1, area.Bottom), tile);
						CreateWalls(level, new Rect(splitX + 1, area.Top, area.Right, area.Bottom), tile);
					}
				} while (tries-- > 0);
			} else {
				do {
					var splitY = Rnd.Int(area.Top + 2, area.Bottom - 2);

					if (level.Get(area.Left - 1, splitY) == tile && level.Get(area.Right + 1, splitY) == tile) {
						tries = 0;
						
						Painter.DrawLine(level, new Dot(area.Left, splitY), new Dot(area.Right, splitY), tile);

						var space = Rnd.Int(area.Left, area.Right - 1);
						var f = fl ? Tile.SensingSpikeTmp : Tiles.RandomFloorOrSpike();
						
						Painter.Set(level, space, splitY, f);
						Painter.Set(level, space + 1, splitY, f);
						
						CreateWalls(level, new Rect(area.Left, area.Top, area.Right, splitY - 1), tile);
						CreateWalls(level, new Rect(area.Left, splitY + 1, area.Right, area.Bottom), tile);
					}
				} while (--tries > 0);
			}
		}
	}
}