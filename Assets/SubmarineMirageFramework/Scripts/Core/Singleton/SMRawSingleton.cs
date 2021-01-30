//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton {
	using Base;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMRawSingleton<T> : SMRawBase, ISMSingleton
		where T : SMRawBase, ISMSingleton, new()
	{
		static T s_instanceObject;
		public static bool s_isCreated => s_instanceObject != null;


		[SMHide] public static T s_instance {
			get {
				if ( !s_isCreated )	{ CreateInstance(); }
				return s_instanceObject;
			}
		}


		protected static void CreateInstance() {
			if ( s_isCreated )	{ return; }
			s_instanceObject = new T();
			SMLog.Debug( $"作成 : { s_instanceObject.GetAboutName() }", SMLogTag.Singleton );
			s_instanceObject._disposables.Add( () =>
				SMLog.Debug( $"{nameof( Dispose )} : {s_instanceObject.GetAboutName()}", SMLogTag.Singleton )
			);
		}

		public static void DisposeInstance() {
			if ( !s_isCreated )	{ return; }
			s_instanceObject.Dispose();
			s_instanceObject = null;
		}
	}
}