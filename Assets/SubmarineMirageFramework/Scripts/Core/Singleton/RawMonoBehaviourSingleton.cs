//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton {
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public abstract class RawMonoBehaviourSingleton<T> : MonoBehaviourSMExtension, ISingleton, ISMRawBase
		where T : MonoBehaviourSMExtension, ISingleton, ISMRawBase
	{
		[Hide] public CompositeDisposable _disposables	{ get; private set; } = new CompositeDisposable();
		public bool _isDispose => _disposables.IsDisposed;
		protected static T s_instanceObject;
		public static bool s_isCreated => s_instanceObject != null;


		public static T s_instance {
			get {
				if ( !s_isCreated )	{ CreateInstance(); }
				return s_instanceObject;
			}
		}


		protected static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<T>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.GetComponent<T>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.AddComponent<T>();
			Log.Debug( $"作成（{nameof( SMTask )}） : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton );
			s_instanceObject._disposables.Add( () =>
				Log.Debug( $"{nameof( Dispose )} : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton )
			);
		}


		public static void DisposeInstance() {
			if ( !s_isCreated )	{ return; }
			s_instanceObject.Dispose();
			Destroy( s_instanceObject );
			s_instanceObject = null;
		}

		public override void Dispose() => _disposables.Dispose();
	}
}