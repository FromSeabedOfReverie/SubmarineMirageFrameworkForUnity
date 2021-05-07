//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using Debug;



	public static class DebugSetter {
		[SMShowLine] public static bool s_isDevelop	{ get; private set; }
		[SMShowLine] public static bool s_isUnityEditor	{ get; private set; }



		public static void Initialize() {
			s_isDevelop =
#if DEVELOP
				true;
#else
				false;
#endif

			s_isUnityEditor =
#if UNITY_EDITOR
				true;
#else
				false;
#endif

			// 有効にならないと、全くログを表示しない為、真っ先に有効化
			SMLog.s_isEnable = s_isDevelop;
		}
	}
}