using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.creature;
using BurningKnight.entity.item;
using Lens.entity.component;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class DropsComponent : Component {
		public List<Drop> Drops = new List<Drop>();

		public void Add(params Drop[] drops) {
			Drops.AddRange(drops);
		}
		
		public List<Item> GetDrops() {
			var drops = new List<Item>();

			foreach (var drop in Drops) {
				if (Random.Float(1f) > drop.Chance) {
					continue;
				}
				
				var ids = drop.GetItems();

				foreach (var id in ids) {
					var item = Items.Create(id);

					if (item != null) {
						drops.Add(item);
					}
				}
			}

			if (Entity is DropModifier d) {
				d.ModifyDrops(drops);
			}
			
			return drops;
		}
		
		public void SpawnDrops() {
			var drops = GetDrops();

			foreach (var item in drops) {
				item.Center = Entity.Center;
				Entity.Area.Add(item);
				item.AddDroppedComponents();
				item.RandomizeVelocity(1f);
			}
		}
	}
}