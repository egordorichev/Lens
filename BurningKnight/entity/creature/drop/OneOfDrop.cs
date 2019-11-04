using System.Collections.Generic;
using Lens.util.math;

namespace BurningKnight.entity.creature.drop {
	public class OneOfDrop : Drop {
		public Drop[] Drops;

		public OneOfDrop(params Drop[] drops) {
			Drops = drops;
		}
		
		public override List<string> GetItems() {
			var items = base.GetItems();

			if (Drops != null) {
				var sum = 0f;

				foreach (var drop in Drops) {
					sum += drop.Chance;
				}			
				
				var value = Random.Float(sum);
				sum = 0;

				foreach (var drop in Drops) {
					sum += drop.Chance;

					if (value <= sum) {
						items.AddRange(drop.GetItems());
						break;
					}
				}
			}

			return items;
		}
	}
}