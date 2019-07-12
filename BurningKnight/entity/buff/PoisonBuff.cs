using BurningKnight.entity.component;

namespace BurningKnight.entity.buff {
	public class PoisonBuff : Buff {
		public static string Id = "bk:poison";
		
		public PoisonBuff() : base(Id) {
			Duration = 30;
		}

		private float tillDamage;

		public override void Update(float dt) {
			base.Update(dt);

			tillDamage -= dt;

			if (tillDamage <= 0) {
				tillDamage = 0.5f;
				Entity.GetComponent<HealthComponent>().ModifyHealth(-1, Entity, false);
			}
		}
	}
}