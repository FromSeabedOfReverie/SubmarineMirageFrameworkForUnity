//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton.New {
	using System.Collections.Generic;
	using UnityEngine;
	using Extension;
	using Process.New;
	using Debug;


	// TODO : コメント追加、整頓


	public class MonoBehaviourSingletonManager : MonoBehaviourSingleton<MonoBehaviourSingletonManager> {
		public static MonoBehaviourSingletonManager CreateTopInstance() {
			MonoBehaviourSingletonManager instance = null;

			var tag = TagManager.s_instance.Get( TagManager.Name.Singletons );
			var go = GameObject.FindWithTag( tag );
			if ( go != null ) {
				var p = go.GetComponent<MonoBehaviourSingletonManager>();
				if ( p != null ) {
					instance = p;
				}
			}
			if ( instance != null )	{ return instance; }

			go = new GameObject( tag );
			go.tag = tag;
			instance = go.AddComponent<MonoBehaviourSingletonManager>();
			new ProcessHierarchy( go, new List<IProcess>() { instance }, null );

			Log.Debug( $"作成（GameObject） : { instance.GetAboutName() }", Log.Tag.Singleton );
			return instance;
		}


		public T CreateProcess<T>() where T : MonoBehaviourProcess {
			var process = _hierarchy.GetProcess<T>();
			if ( process != null )	{ return process; }

			process = _hierarchy.AddProcess<T>();
			Log.Debug( $"作成（Component） : { process.GetAboutName() }", Log.Tag.Singleton );
			return process;
		}


		public override void Create() {}
	}
}