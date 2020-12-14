//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Extension {
	using UnityEngine;
	using Base;



	// TODO : コメント追加、整頓



	///====================================================================================================
	/// <summary>
	/// ■ モノ動作の拡張クラス
	///----------------------------------------------------------------------------------------------------
	///		MonoBehaviourを使うときは、必ずこれを継承する。
	/// </summary>
	///====================================================================================================
	public abstract class MonoBehaviourSMExtension : MonoBehaviour, ISMBase {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		public uint _id	{ get; set; }

		/// <summary>変形のキャッシュ</summary>
		Transform _transform;
		/// <summary>変形のキャッシュを取得</summary>
		public new Transform transform {
			get {
				if ( _transform == null )	{ _transform = base.transform; }
				return _transform;
			}
		}
		/// <summary>ゲーム物のキャッシュ</summary>
		GameObject _gameObject;
		/// <summary>ゲーム物のキャッシュを取得</summary>
		public new GameObject gameObject {
			get {
				if ( _gameObject == null )	{ _gameObject = base.gameObject; }
				return _gameObject;
			}
		}

		protected virtual void Awake() => SMBaseManager.s_instance.SetID( this );

		public abstract void Dispose();

		protected void OnDestroy() => Dispose();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文字列に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override string ToString() => ToString( 0 );
		public virtual string ToString( int indent ) => this.ToShowString( indent );
		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}