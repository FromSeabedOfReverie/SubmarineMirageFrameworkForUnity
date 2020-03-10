//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Singleton.New {
	using UnityEngine;
	using Extension;
	using Process.New;
	using Debug;


	// TODO : コメント追加、整頓


	public class MonoBehaviourSingletonManager : Singleton<MonoBehaviourSingletonManager> {
		public override CoreProcessManager.ProcessType _type => CoreProcessManager.ProcessType.DontWork;
		static GameObject s_parent;


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


		public T CreateComponent<T>() where T : MonoBehaviourProcess {
			CreateGameObject();

			var component = s_parent.GetComponent<T>();
			if ( component != null )	{ return component; }

			component = s_parent.AddComponent<T>();
			Log.Debug( $"作成（MonoBehaviour） : { component.GetAboutName() }", Log.Tag.Singleton );
			return component;
		}


		public override void Create() {
		}
	}
}