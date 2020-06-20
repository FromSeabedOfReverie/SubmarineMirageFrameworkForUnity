//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton.New {
	using UnityEngine;
	using SMTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class MonoBehaviourSingletonManager : MonoBehaviourSingleton<MonoBehaviourSingletonManager> {
		public override SMTaskType _type => SMTaskType.DontWork;


		new public static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<MonoBehaviourSingletonManager>();
			if ( s_isCreated )	{ return; }

			var tag = TagManager.Name.Singletons.ToString();
			var go = GameObject.FindWithTag( tag );
			if ( go != null ) {
				var p = go.GetComponent<MonoBehaviourSingletonManager>();
				if ( p != null ) {
					s_instanceObject = p;
				}
			}
			if ( s_isCreated )	{ return; }

			go = new GameObject( tag );
			go.tag = tag;
			s_instanceObject = go.AddComponent<MonoBehaviourSingletonManager>();
			new SMHierarchy( go, new ISMBehavior[] { s_instanceObject }, null );

			Log.Debug( $"作成（GameObject） : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton );
		}


		public T CreateProcess<T>() where T : SMMonoBehaviour {
			var process = _hierarchy.GetProcess<T>();
			if ( process != null )	{ return process; }

			process = _hierarchy.AddProcess<T>();
			Log.Debug( $"作成（Component） : {process.GetAboutName()}", Log.Tag.Singleton );
			return process;
		}


		public override void Create() {}
	}
}