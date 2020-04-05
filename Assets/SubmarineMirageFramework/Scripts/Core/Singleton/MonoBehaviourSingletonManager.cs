//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Singleton {
	using UnityEngine;
	using Extension;
	using Process;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ MonoBehaviourシングルトンの管理クラス
	///----------------------------------------------------------------------------------------------------
	///		MonoBehaviourSingletonを管理し、親オブジェクト処理を行う。
	/// </summary>
	///====================================================================================================
	public class MonoBehaviourSingletonManager : Singleton<MonoBehaviourSingletonManager> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>登録するか？</summary>
		public override bool _isRegister => false;
		/// <summary>親</summary>
		static GameObject s_parent;
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成（ゲーム物）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void CreateGameObject() {
			if ( s_parent != null )	{ return; }

			var tag = TagManager.s_instance.Get( TagManager.Name.Singletons );
			s_parent = GameObject.FindWithTag( tag );
			if ( s_parent != null )	{ return; }

			s_parent = new GameObject( tag );
			s_parent.tag = tag;
			Object.DontDestroyOnLoad( s_parent );

			Log.Debug( $"作成（GameObject） : { this.GetAboutName() }", Log.Tag.Singleton );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 作成（部品）
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public T CreateComponent<T>() where T : MonoBehaviourProcess {
			CreateGameObject();
			var component = s_parent.AddComponent<T>();
			Log.Debug( $"作成（MonoBehaviour） : { component.GetAboutName() }", Log.Tag.Singleton );
			return component;
		}
	}
}