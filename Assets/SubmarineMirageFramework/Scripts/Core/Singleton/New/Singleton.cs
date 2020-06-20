//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton.New {
	using UniRx.Async;
	using SMTask;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class Singleton<T> : SMBehavior, ISingleton
		where T : SMBehavior, new()
	{
		static T s_instanceObject;
		public static bool s_isCreated => s_instanceObject != null;
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;


		public static T s_instance {
			get {
				if ( !s_isCreated )	{ CreateInstance(); }
				return s_instanceObject;
			}
		}


		protected static void CreateInstance() {
			if ( s_isCreated )	{ return; }
			s_instanceObject = new T();
			Log.Debug( $"作成 : { s_instanceObject.GetAboutName() }", Log.Tag.Singleton );
		}


		public static async UniTask WaitForCreation() {
// TODO : 登録順が担保できれば、不要
			var i = s_instance;
			await UniTaskUtility.Yield( s_instance._activeAsyncCancel );
		}


		public static void DisposeInstance() {
			if ( !s_isCreated )	{ return; }
			s_instanceObject.Dispose();
			s_instanceObject = null;
		}
	}
}