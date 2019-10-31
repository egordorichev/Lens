﻿using System;
using BurningKnight.level;
using BurningKnight.save;
using BurningKnight.save.statistics;
using Lens;
using Lens.util;
using Random = Lens.util.math.Random;

namespace BurningKnight.state {
	public static class Run {
		public const int ContentEndDepth = 5;

		private static int depth = 1;// BK.Version.Dev ? 0 : 0;
		public static int NextDepth = depth;
		public static int LastDepth = depth;
		public static int SavingDepth;
		public static bool StartingNew;
		public static int KillCount;
		public static float Time;
		public static Level Level;
		public static bool StartedNew;
		public static bool HasRun;
		public static string Seed;
		public static bool IgnoreSeed;
		public static int Luck;
		public static int Curse;
		public static bool IntoMenu;
		public static RunStatistics Statistics;
		
		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}

		public static void Update() {
			if (StartingNew || depth != NextDepth) {
				LastDepth = depth;
				SavingDepth = depth;
				depth = NextDepth;
				StartedNew = StartingNew;
				StartingNew = false;
				
				SaveManager.Delete(SaveType.Player, SaveType.Game, SaveType.Level);
				
				Engine.Instance.SetState(new LoadState {
					Menu = IntoMenu
				});

				IntoMenu = false;
			}
		}

		public static void StartNew(int depth = 1) {
			StartingNew = true;
			HasRun = false;
			NextDepth = depth;

			if (IgnoreSeed) {
				IgnoreSeed = true;
			} else {
				Seed = Random.GenerateSeed();
			}

			GlobalSave.RunId++;
			Random.Seed = Seed;
			
			Log.Debug($"This run's seed is {Seed}");
		}

		public static void ResetStats() {
			KillCount = 0;
			Time = 0;
			HasRun = false;
			Luck = 0;
			Curse = 0;
			Seed = Random.GenerateSeed();
		}

		public static string FormatTime() {
			return $"{Math.Floor(Time / 360f)}h {Math.Floor(Time / 60f)}m {Math.Floor(Time % 60f)}s";
		}
	}
}