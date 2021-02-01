//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Singleton {
	using Cysharp.Threading.Tasks;
	using Task;
	using Task.Behaviour;
	using Task.Behaviour.Modifyler;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMMonoBehaviourSingleton<T> : SMMonoBehaviour, ISMSingleton
		where T : SMMonoBehaviour
	{
		protected static T s_instanceObject;
		public static bool s_isCreated => s_instanceObject != null;
		public override SMTaskType _type => SMTaskType.FirstWork;
		public override SMTaskLifeSpan _lifeSpan => SMTaskLifeSpan.Forever;


		[SMHide] public static T s_instance {
			get {
				if ( !s_isCreated )	{ CreateInstance(); }
				return s_instanceObject;
			}
		}


		protected static void CreateInstance() {
			if ( s_isCreated )	{ return; }

			s_instanceObject = FindObjectOfType<T>();
			if ( s_isCreated )	{ return; }

			s_instanceObject = SMMonoBehaviourSingletonManager.s_instance.CreateBehaviour<T>();
		}


		public static async UniTask WaitForCreation() {
// TODO : 登録順が担保できれば、不要
			var i = s_instance;
			await UTask.NextFrame( s_instance._asyncCancelerOnDisable );
		}


		public static void DisposeInstance() {
			if ( !s_isCreated )	{ return; }

			// 複数のシングルトンが_objectに含まれる為、_object.Disposeはしない
			s_instanceObject.Dispose();
// TODO : Modifylerを無視しているので、修正
			s_instanceObject._body.Unlink();
			s_instanceObject.Destroy();
			s_instanceObject = null;
		}
	}
}