//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Singleton.New {
	using UniRx.Async;
	using Extension;
	using Debug;
	using Process.New;


	// TODO : コメント追加、整頓


	public abstract class Singleton<T> : BaseProcess, ISingleton
		where T : BaseProcess, new()
	{
		static T s_instanceObject;
		public static bool s_isCreated => s_instanceObject != null;
		public override CoreProcessManager.ProcessType _type => CoreProcessManager.ProcessType.FirstWork;
		public override CoreProcessManager.ProcessLifeSpan _lifeSpan
			=> CoreProcessManager.ProcessLifeSpan.Forever;


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
			var i = s_instance;
			await UniTask.Delay( 1 );
		}
	}
}