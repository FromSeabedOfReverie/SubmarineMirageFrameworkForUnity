//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton {
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
				var b = go.GetComponent<MonoBehaviourSingletonManager>();
				if ( b != null ) {
					s_instanceObject = b;
				}
			}
			if ( s_isCreated )	{ return; }

			go = new GameObject( tag );
			go.tag = tag;
			s_instanceObject = go.AddComponent<MonoBehaviourSingletonManager>();
			new SMObject( go, new ISMBehaviour[] { s_instanceObject }, null );

			Log.Debug( $"作成（GameObject） : {s_instanceObject.GetAboutName()}", Log.Tag.Singleton );
		}


		public T CreateBehaviour<T>() where T : SMMonoBehaviour {
			var b = _object.GetBehaviour<T>();
			if ( b != null )	{ return b; }

			b = _object.AddBehaviour<T>();
			Log.Debug( $"作成（{nameof( SMTask )}） : {b.GetAboutName()}", Log.Tag.Singleton );
			return b;
		}


		public override void Create() {}
	}
}