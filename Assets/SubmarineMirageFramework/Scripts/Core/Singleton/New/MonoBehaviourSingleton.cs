//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton.New {
	using Cysharp.Threading.Tasks;
	using UTask;
	using SMTask;


	// TODO : コメント追加、整頓


	public abstract class MonoBehaviourSingleton<T> : SMMonoBehaviour, ISingleton
		where T : SMMonoBehaviour
	{
		protected static T s_instanceObject;
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

			s_instanceObject = FindObjectOfType<T>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = MonoBehaviourSingletonManager.s_instance.CreateBehaviour<T>();
		}


		public static async UniTask WaitForCreation() {
// TODO : 登録順が担保できれば、不要
			var i = s_instance;
			await UTask.NextFrame( s_instance._activeAsyncCancel );
		}


		public static void DisposeInstance() {
			if ( !s_isCreated )	{ return; }
			s_instanceObject.Dispose();
			s_instanceObject = null;
		}
	}
}