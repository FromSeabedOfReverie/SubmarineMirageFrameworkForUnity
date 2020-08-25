//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System.Linq;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using MultiEvent;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class ChangeActiveSMObject : SMObjectModifyData {
		static readonly string EVENT_KEY = $"{nameof( ChangeActiveSMObject )}.{nameof( ChangeActive )}";
		public override ModifyType _type => ModifyType.Runner;
		bool _isActive;
		bool _isChangeOwner;


		public ChangeActiveSMObject( SMObject smObject, bool isActive, bool isChangeOwner ) : base( smObject ) {
			_isActive = isActive;
			_isChangeOwner = isChangeOwner;
		}

		public override void Cancel() {}


		public override UniTask Run() => ChangeActive( _object, _isActive, _isChangeOwner );


		bool IsCanChangeActive( SMObject smObject, bool isChangeOwner ) {
			if ( smObject._parent?._owner != null && !smObject._parent._owner.activeInHierarchy )	{ return false; }
			if ( !isChangeOwner && smObject._owner != null && !smObject._owner.activeSelf )			{ return false; }
			return true;
		}


		async UniTask ChangeActive( SMObject smObject, bool isActive, bool isChangeOwner ) {
			using ( var events = new MultiAsyncEvent( isCancelByCanceler => isCancelByCanceler ) ) {
				var isCanChangeActive = false;
				if ( isChangeOwner && smObject._owner != null && smObject._type != SMTaskType.DontWork ) {
					events.AddLast( async _ => {
// TODO : Disable時でも、Activeにしてしまうが、Managerの方で呼ばないはず、確認する
						smObject._owner.SetActive( isActive );
						await UTask.DontWait();
#if TestSMTaskModifyler
						Log.Debug( $"{smObject._owner.GetAboutName()}.SetActive : {isActive}" );
#endif
					} );
				}
				if ( isActive ) {
					events.AddLast( async _ => {
						isCanChangeActive = IsCanChangeActive( smObject, isChangeOwner );
						await UTask.DontWait();
					} );
				}

				switch ( smObject._type ) {
					case SMTaskType.FirstWork:
						foreach ( var b in smObject.GetBehaviours() ) {
							events.AddLast( async _ => {
								if ( !isCanChangeActive )	{ return; }
								await b.ChangeActive( isActive );
							} );
						}
						events.AddLast( async _ => {
							if ( !isCanChangeActive )	{ return; }
							foreach ( var o in smObject.GetChildren() ) {
								await ChangeActive( o, isActive, false );
							}
						} );
						break;

					case SMTaskType.Work:
						events.AddLast( async _ => {
							if ( !isCanChangeActive )	{ return; }
							await smObject.GetBehaviours().Select( b => b.ChangeActive( isActive ) )
								.Concat( smObject.GetChildren().Select( o => ChangeActive( o, isActive, false ) ) );
						} );
						break;
				}

				if ( !isActive ) {
					events.AddLast( async _ => {
						isCanChangeActive = IsCanChangeActive( smObject, isChangeOwner );
						await UTask.DontWait();
					} );
					events.Reverse();
				}
				await events.Run( smObject._asyncCanceler );
			}
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_isActive,
				_isChangeOwner
			)
			+ ", "
		);
	}
}