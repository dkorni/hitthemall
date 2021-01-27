#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class StoreScreenshotUtils
{

	public struct ScreenshotResolution
	{
		public string resName;
		public int width;
		public int height;
		public bool toDelete;
		public bool active;
	}

	public enum GameViewSizeType
	{
		FixedResolution,
		AspectRatio
	}

	static Type gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
	static Type gameViewSize = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
	static Type gameViewSizes = typeof(Editor).Assembly.GetType("UnityEditor.ScriptableSingleton`1").MakeGenericType(typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes"));
	static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

	private static GameViewSize _gameViewSize;

	public class GameViewSize
	{
		public GameViewSizeType type;
		public int width;
		public int height;
		public string baseText;
	}

	public static void AddCustomSize(GameViewSizeGroupType groupType, GameViewSize gameViewSize)
	{
		_gameViewSize = gameViewSize;
		object sizeType = Enum.Parse(gameViewSizeType, gameViewSize.type.ToString());

		ConstructorInfo ctor = StoreScreenshotUtils.gameViewSize.GetConstructor(new Type[]
			{
				gameViewSizeType,
				typeof(int),
				typeof(int),
				typeof(string)
			});

		object instance_gameViewSize = ctor.Invoke(new object[]
			{
				sizeType,
				gameViewSize.width,
				gameViewSize.height,
				gameViewSize.baseText
			});

		object instance_gameViewSizeGroup = GetGroup(groupType, instance);

		if (!Contains(instance_gameViewSizeGroup))
		{
			AddCustomSize(instance_gameViewSizeGroup, instance_gameViewSize);
		}
	}

	public static void AddCustomSize(int width, int height, string baseText)
	{
		AddCustomSize(CurrentType(), GameViewSizeType.FixedResolution, width, height, baseText);
	}

	public static void AddCustomSize(GameViewSizeGroupType groupType, GameViewSizeType type, int width, int height, string baseText)
	{
		AddCustomSize(groupType, new GameViewSize { type = type, width = width, height = height, baseText = baseText });
	}

	public static void RemoveCustomSize(int width, int height, string baseText)
	{
		RemoveCustomSize(CurrentType(), GameViewSizeType.FixedResolution, width, height, baseText);
	}

	public static bool RemoveCustomSize(GameViewSizeGroupType groupType, GameViewSizeType type, int width, int height, string baseText)
	{
		_gameViewSize = new GameViewSize { type = type, width = width, height = height, baseText = baseText };
		return Remove(GetGroup(groupType, instance));
	}
	public static bool RemoveCustomSize(GameViewSizeGroupType groupType, GameViewSize gameViewSize)
	{
		_gameViewSize = gameViewSize;
		return Remove(GetGroup(groupType, instance));
	}
	public static bool Contains(GameViewSizeGroupType groupType, GameViewSizeType type, int width, int height, string baseText)
	{
		_gameViewSize = new GameViewSize { type = type, width = width, height = height, baseText = baseText };
		return Contains(GetGroup(groupType, instance));
	}
	public static bool Contains(GameViewSizeGroupType groupType, GameViewSize gameViewSize)
	{
		_gameViewSize = gameViewSize;
		return Contains(GetGroup(groupType, instance));
	}

	public static GameViewSizeGroupType CurrentType()
	{
		EditorWindow gameView = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
		PropertyInfo currentSizeGroupType = typeof(Editor).Assembly.GetType("UnityEditor.GameView").GetProperty("currentSizeGroupType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
		return (GameViewSizeGroupType)currentSizeGroupType.GetValue(gameView, null);
	}

	public static void ChangeGameViewSize(int width, int height, string baseText)
	{
		ChangeGameViewSize(CurrentType(), GameViewSizeType.FixedResolution, width, height, baseText);
	}

	public static void ChangeGameViewSize(GameViewSizeGroupType groupType, GameViewSizeType type, int width, int height, string baseText)
	{
		ChangeGameViewSize(groupType, new GameViewSize { type = type, width = width, height = height, baseText = baseText });
	}

	public static void ChangeGameViewSize(GameViewSizeGroupType groupType, GameViewSize gameViewSize)
	{
		EditorWindow gameView = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));

		if (groupType != CurrentType())
		{
			Debug.LogError(string.Format("GameViewSizeGroupType is {0}. but Current GameViewSizeGroupType is {1}.", groupType, CurrentType()));
			return;
		}
		object group = GetGroup(groupType, instance);
		int totalCount = GetTotalCount(group);
		int gameViewSizeLength = GetCustomCount(group);
		int index = -1;
		for (int i = totalCount - gameViewSizeLength; i < totalCount; i++)
		{
			object other_gameViewSize = GetGameViewSize(group, i);
			if (GameViewSize_Equals(gameViewSize, other_gameViewSize))
			{
				index = i;
				break;
			}
		}
		if (index != -1)
		{
			PropertyInfo selectedSizeIndex = typeof(Editor).Assembly.GetType("UnityEditor.GameView").GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
			selectedSizeIndex.SetValue(gameView, index, null);
		}
	}



	static bool Remove(object instance_gameViewSizeGroup)
	{
		int gameViewSizeLength = GetCustomCount(instance_gameViewSizeGroup);
		int totalCount = GetTotalCount(instance_gameViewSizeGroup);
		for (int i = totalCount - gameViewSizeLength; i < totalCount; i++)
		{
			object other_gameViewSize = GetGameViewSize(instance_gameViewSizeGroup, i);
			if (GameViewSize_Equals(_gameViewSize, other_gameViewSize))
			{
				RemoveCustomSize(instance_gameViewSizeGroup, i);
				return true;
			}
		}
		return false;
	}

	static bool Contains(object instance_gameViewSizeGroup)
	{
		int gameViewSizeLength = GetCustomCount(instance_gameViewSizeGroup);
		int totalCount = GetTotalCount(instance_gameViewSizeGroup);
		for (int i = totalCount - gameViewSizeLength; i < totalCount; i++)
		{
			if (GameViewSize_Equals(_gameViewSize, GetGameViewSize(instance_gameViewSizeGroup, i)))
			{
				return true;
			}
		}
		return false;
	}

	private static bool GameViewSize_Equals(GameViewSize a, object b)
	{
		int b_width = (int)GetGameSizeProperty(b, "width");
		int b_height = (int)GetGameSizeProperty(b, "height");
		string b_baseText = (string)GetGameSizeProperty(b, "baseText");
		GameViewSizeType b_sizeType = (GameViewSizeType)Enum.Parse(typeof(GameViewSizeType), GetGameSizeProperty(b, "sizeType").ToString());

		return a.type == b_sizeType && a.width == b_width && a.height == b_height && a.baseText == b_baseText;
	}

	static object GetGameSizeProperty(object instance, string name)
	{
		return instance.GetType().GetProperty(name).GetValue(instance, new object[0]);
	}

	static object m_instance;

	static object instance
	{
		get
		{
			if (m_instance == null)
			{
				PropertyInfo propertyInfo_gameViewSizes = gameViewSizes.GetProperty("instance");
				m_instance = propertyInfo_gameViewSizes.GetValue(null, new object[0]);
			}
			return m_instance;
		}
	}

	static object GetGroup(GameViewSizeGroupType groupType, object instance_gameViewSizes)
	{
		Type[] returnTypes = new Type[] { groupType.GetType() };
		object[] parameters = new object[] { groupType };
		return instance_gameViewSizes.GetType().GetMethod("GetGroup", bindingFlags, null, returnTypes, null).Invoke(instance_gameViewSizes, parameters);
	}

	static object GetGameViewSize(object instance_gameViewSizeGroup, int i)
	{
		Type[] returnTypes = new Type[] { typeof(int) };
		object[] parameters = new object[] { i };
		return instance_gameViewSizeGroup.GetType().GetMethod("GetGameViewSize", bindingFlags, null, returnTypes, null).Invoke(instance_gameViewSizeGroup, parameters);
	}

	static int GetCustomCount(object instance_gameViewSizeGroup)
	{
		return (int)instance_gameViewSizeGroup.GetType().GetMethod("GetCustomCount", bindingFlags, null, new Type[0], null).Invoke(instance_gameViewSizeGroup, new object[0]);
	}

	static int GetTotalCount(object instance_gameViewSizeGroup)
	{
		return (int)instance_gameViewSizeGroup.GetType().GetMethod("GetTotalCount", bindingFlags, null, new Type[0], null).Invoke(instance_gameViewSizeGroup, new object[0]);
	}

	static void AddCustomSize(object instance_gameViewSizeGroup, object instance_gameViewSize)
	{
		Type[] returnTypes = new Type[] { gameViewSize };
		object[] parameters = new object[] { instance_gameViewSize };
		instance_gameViewSizeGroup.GetType().GetMethod("AddCustomSize", bindingFlags, null, returnTypes, null).Invoke(instance_gameViewSizeGroup, parameters);
	}

	static void RemoveCustomSize(object instance_gameViewSizeGroup, int index)
	{
		Type[] returnTypes = new Type[] { typeof(int) };
		object[] parameters = new object[] { index };
		instance_gameViewSizeGroup.GetType().GetMethod("RemoveCustomSize", bindingFlags, null, returnTypes, null).Invoke(instance_gameViewSizeGroup, parameters);
	}
}
#endif