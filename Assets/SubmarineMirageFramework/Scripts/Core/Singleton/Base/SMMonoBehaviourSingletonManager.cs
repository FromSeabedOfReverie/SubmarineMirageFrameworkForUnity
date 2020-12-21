//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestSingleton
namespace SubmarineMirage.Singleton {
	using UnityEngine;
	using Task;
	using Task.Behaviour;
	using Task.Object;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class SMMonoBehaviourSingletonManager : SMMonoBehaviourSingleton<SMMonoBehaviourSingletonManager> {
		public override SMTaskType _type => SMTaskType.DontWork;


		new public static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<SMMonoBehaviourSingletonManager>();
			if ( s_isCreated )	{ return; }

			var tag = SMTagManager.Name.Singletons.ToString();
			var go = GameObject.FindWithTag( tag );
			if ( go != null ) {
				var b = go.GetComponent<SMMonoBehaviourSingletonManager>();
				if ( b != null ) {
					s_instanceObject = b;
				}
			}
			if ( s_isCreated )	{ return; }

			go = new GameObject( tag );
			go.tag = tag;
			s_instanceObject = go.AddComponent<SMMonoBehaviourSingletonManager>();
			new SMObject( go, new ISMBehaviour[] { s_instanceObject }, null );

			SMLog.Debug( $"作成（GameObject） : {s_instanceObject.GetAboutName()}", SMLogTag.Singleton );
		}


		public T CreateBehaviour<T>() where T : SMMonoBehaviour {
			var b = _object.GetBehaviour<T>();
			if ( b != null )	{ return b; }

			b = _object.AddBehaviour<T>();
			SMLog.Debug( $"作成（{nameof( Task )}） : {b.GetAboutName()}", SMLogTag.Singleton );
			return b;
		}


#if TestSingleton
		public MonoBehaviourSingletonManager()
			=> SMLog.Debug( $"{nameof( MonoBehaviourSingletonManager )}() : {this}" );
#endif

		public override void Create() {
#if TestSingleton
			SMLog.Debug( $"{nameof( Create )} : {this}" );
#endif
		}
	}
}