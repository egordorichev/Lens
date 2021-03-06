using System;
using BurningKnight.assets.achievements;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class AchievementStatue : Prop {
		private string id = "bk:rip";
		private Achievement achievement;
		private TextureRegion achievementTexture;
		private float offset;
		private bool hidden;

		public override void AddComponents() {
			base.AddComponents();

			offset = Rnd.Float(1);
			
			Width = 24;
			Height = 32;
			
			AddComponent(new DialogComponent());
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !hidden
			});
			
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new InteractableSliceComponent("props", "achievement_statue"));
			AddComponent(new RectBodyComponent(0, 13, 24, 19, BodyType.Static));

			GetComponent<DialogComponent>().Dialog.Voice = 30;
			
			AddTag(Tags.Statue);

			Area.Add(new RenderTrigger(this, RenderTop, Layers.FlyingMob));
			
			Achievements.UnlockedCallback += UpdateState;
			Achievements.LockedCallback += UpdateState;
		}

		public override void Destroy() {
			base.Destroy();
			
			Achievements.UnlockedCallback -= UpdateState;
			Achievements.LockedCallback -= UpdateState;
		}

		public override void PostInit() {
			base.PostInit();
			SetupSprite();
			UpdateState();
		}
		
		private bool Interact(Entity e) {
			foreach (var s in Area.Tagged[Tags.Statue]) {
				if (s.TryGetComponent<DialogComponent>(out var d)) {
					d.Close();
				}
			}

			string state;

			if (achievement != null && achievement.Unlocked) {
				var d = achievement.CompletionDate;

				if (d == "???") {
					d = "~~???~~";
				}
				
				state = $"[sp 2][cl orange]{Locale.Get($"ach_{id}")}[cl]\n{Locale.Get($"ach_{id}_desc")}\n[cl gray]{Locale.Get("completed_on")} {d}[cl]";
			} else {
				state = $"[sp 2][cl orange]{Locale.Get($"ach_{id}")}[cl]";

				if (achievement.Max > 0) {
					var p = GlobalSave.GetInt($"ach_{id}", 0);
					state += $"\n[cl gray]{MathUtils.Clamp(0, achievement.Max, p)}/{achievement.Max} {Locale.Get("complete")}[cl]";
				}
			}
			
			GetComponent<DialogComponent>().Start(state);
			return true; 
		}

		private void SetupSprite() {
			achievementTexture = Animations.Get("achievements").GetSlice(id);
		}

		private void UpdateState(string i = null) {
			achievement = Achievements.Get(id);

			if (achievement == null || Engine.EditingLevel) {
				return;
			}

			if (!achievement.Unlocked && achievement.Secret) {
				hidden = true;
				GetComponent<RectBodyComponent>().Body.IsSensor = true;
			} else {
				hidden = false;
				GetComponent<RectBodyComponent>().Body.IsSensor = false;
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			id = stream.ReadString();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(id);
		}

		public override void Render() {
			if (hidden) {
				return;
			}
			
			base.Render();
		}

		public void RenderShadow() {
			if (hidden) {
				return;
			}
			
			GraphicsComponent.Render(true);
		}

		public void RenderTop() {
			if (!hidden && achievement != null && achievement.Unlocked) {
				Graphics.Render(achievementTexture, Position + new Vector2(2, (float) Math.Cos(Engine.Time * 1.5f + offset) * 2.5f - 2.5f));
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputText("Id", ref id, 128)) {
				SetupSprite();
				UpdateState();
			}
		}
	}
}