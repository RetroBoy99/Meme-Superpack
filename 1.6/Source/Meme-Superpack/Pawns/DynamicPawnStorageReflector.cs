using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace MSSMeme.Pawns
{
	/// <summary>
	/// Reflection-based access to DynamicPawnStorage from the Flavor Pack
	/// This allows the meme pack to compile without requiring the Flavor Pack at compile time
	/// </summary>
	public class DynamicPawnStorageReflector : IExposable
	{
		private object _storageInstance;
		private Type _storageType;

		// Cached reflection info for performance
		private static Type s_dynamicPawnStorageType;
		private static PropertyInfo s_hasStoredPawnProperty;
		private static PropertyInfo s_originalPawnNameProperty;
		private static MethodInfo s_storePawnMethod;
		private static MethodInfo s_restorePawnMethod;
		private static MethodInfo s_clearStorageMethod;
		private static MethodInfo s_getStorageDescriptionMethod;
		private static MethodInfo s_exposeDataMethod;

		public bool HasStoredPawn
		{
			get
			{
				if (_storageInstance == null || s_hasStoredPawnProperty == null)
					return false;

				try
				{
					return (bool)s_hasStoredPawnProperty.GetValue(_storageInstance);
				}
				catch (Exception ex)
				{
					Log.Error($"MSSMeme: Error getting HasStoredPawn: {ex.Message}");
					return false;
				}
			}
		}

		public string OriginalPawnName
		{
			get
			{
				if (_storageInstance == null || s_originalPawnNameProperty == null)
					return null;

				try
				{
					return (string)s_originalPawnNameProperty.GetValue(_storageInstance);
				}
				catch (Exception ex)
				{
					Log.Error($"MSSMeme: Error getting OriginalPawnName: {ex.Message}");
					return null;
				}
			}
		}

		public DynamicPawnStorageReflector()
		{
			InitializeReflection();
			CreateStorageInstance();
		}

		private static void InitializeReflection()
		{
			if (s_dynamicPawnStorageType != null)
				return; // Already initialized

			try
			{
				// Try to find the DynamicPawnStorage class from the flavor pack
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var assembly in assemblies)
				{
					var type = assembly.GetType("MSSFP.Pawns.DynamicPawnStorage", false);
					if (type != null)
					{
						s_dynamicPawnStorageType = type;
						break;
					}
				}

				if (s_dynamicPawnStorageType == null)
				{
					Log.Warning("MSSMeme: DynamicPawnStorage type not found - Flavor Pack may not be loaded");
					return;
				}

				// Cache reflection info
				s_hasStoredPawnProperty = s_dynamicPawnStorageType.GetProperty("HasStoredPawn");
				s_originalPawnNameProperty = s_dynamicPawnStorageType.GetProperty("OriginalPawnName");
				s_storePawnMethod = s_dynamicPawnStorageType.GetMethod(
					"StorePawn",
					new[] { typeof(Pawn), typeof(DestroyMode) }
				);
				s_restorePawnMethod = s_dynamicPawnStorageType.GetMethod(
					"RestorePawn",
					new[] { typeof(IntVec3), typeof(Map), typeof(Faction) }
				);
				s_clearStorageMethod = s_dynamicPawnStorageType.GetMethod("ClearStorage", Type.EmptyTypes);
				s_getStorageDescriptionMethod = s_dynamicPawnStorageType.GetMethod(
					"GetStorageDescription",
					Type.EmptyTypes
				);
				s_exposeDataMethod = s_dynamicPawnStorageType.GetMethod("ExposeData", Type.EmptyTypes);

				Log.Message("MSSMeme: Successfully initialized reflection for DynamicPawnStorage");
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Error initializing DynamicPawnStorage reflection: {ex.Message}");
			}
		}

		private void CreateStorageInstance()
		{
			if (s_dynamicPawnStorageType == null)
				return;

			try
			{
				_storageInstance = Activator.CreateInstance(s_dynamicPawnStorageType);
				_storageType = s_dynamicPawnStorageType;
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Failed to create DynamicPawnStorage instance: {ex.Message}");
			}
		}

		public bool StorePawn(Pawn pawn, DestroyMode destroyMode = DestroyMode.Vanish)
		{
			if (_storageInstance == null || s_storePawnMethod == null)
			{
				Log.Warning("MSSMeme: Cannot store pawn - DynamicPawnStorage not available");
				return false;
			}

			try
			{
				return (bool)s_storePawnMethod.Invoke(_storageInstance, new object[] { pawn, destroyMode });
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Error storing pawn: {ex.Message}");
				return false;
			}
		}

		public Pawn RestorePawn(IntVec3 position, Map map, Faction factionOverride = null)
		{
			if (_storageInstance == null || s_restorePawnMethod == null)
			{
				Log.Warning("MSSMeme: Cannot restore pawn - DynamicPawnStorage not available");
				return null;
			}

			try
			{
				return (Pawn)
					s_restorePawnMethod.Invoke(
						_storageInstance,
						new object[] { position, map, factionOverride }
					);
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Error restoring pawn: {ex.Message}");
				return null;
			}
		}

		public void ClearStorage()
		{
			if (_storageInstance == null || s_clearStorageMethod == null)
				return;

			try
			{
				s_clearStorageMethod.Invoke(_storageInstance, null);
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Error clearing storage: {ex.Message}");
			}
		}

		public string GetStorageDescription()
		{
			if (_storageInstance == null || s_getStorageDescriptionMethod == null)
				return "DynamicPawnStorage not available";

			try
			{
				return (string)s_getStorageDescriptionMethod.Invoke(_storageInstance, null);
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Error getting storage description: {ex.Message}");
				return "Error getting description";
			}
		}

		public void ExposeData()
		{
			if (_storageInstance == null)
			{
				// Store a flag to indicate we need flavor pack
				bool needsFlavourPack = true;
				Scribe_Values.Look(ref needsFlavourPack, "needsFlavourPack", true);

				if (Scribe.mode == LoadSaveMode.LoadingVars && needsFlavourPack)
				{
					Log.Warning("MSSMeme: Save file contains plaque sign data but flavor pack is not loaded");
				}
				return;
			}

			try
			{
				s_exposeDataMethod?.Invoke(_storageInstance, null);
			}
			catch (Exception ex)
			{
				Log.Error($"MSSMeme: Error during ExposeData: {ex.Message}");
			}
		}
	}
}
