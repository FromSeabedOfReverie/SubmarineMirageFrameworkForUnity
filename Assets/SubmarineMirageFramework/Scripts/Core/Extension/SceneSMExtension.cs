//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine.SceneManagement;
	using Scene;
	///====================================================================================================
	/// <summary>
	/// ■ 場面の拡張クラス
	/// </summary>
	///====================================================================================================
	public static class SceneSMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● SMSceneに変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static SMScene ToSMScene( this Scene self )
			=> SMSceneManager.s_instance._fsm.GetScene( self );
	}
}