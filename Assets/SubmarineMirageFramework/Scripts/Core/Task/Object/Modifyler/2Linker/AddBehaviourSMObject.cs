//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestObjectModifyler
namespace SubmarineMirage.Task.Object.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using Behaviour;
	using Object;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public class AddBehaviourSMObject : SMObjectModifyData {
		public override SMTaskModifyType _type => SMTaskModifyType.Linker;
		[SMShowLine] public SMMonoBehaviour _behaviour	{ get; private set; }


		public AddBehaviourSMObject( SMObject target, Type type ) : base( target ) {
			if ( !_target._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{_target}" );
			}
			_behaviour = (SMMonoBehaviour)_target._owner.AddComponent( type );
			if ( _behaviour == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{type}" );
			}
		}

		protected override void Cancel() {
#if TestObjectModifyler
			var o = _behaviour?._object;
			var go = _behaviour?.gameObject;
			var bs = go?.GetComponents<SMMonoBehaviour>() ?? new SMMonoBehaviour[0];
			SMLog.Debug( string.Join( "\n",
				$"{nameof( Cancel )} start :",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _behaviour._object )} : {o}",
				$"{nameof( go.GetComponents )} : ",
				string.Join( "\n", bs.Select( b => $"    {b.ToLineString()}" ) ),
				$"{nameof( _modifyler )} : {_modifyler}"
			) );
#endif
			_behaviour.Dispose();
#if TestObjectModifyler
			bs = go?.GetComponents<SMMonoBehaviour>() ?? new SMMonoBehaviour[0];
			SMLog.Debug( string.Join( "\n",
				$"{nameof( Cancel )} end :",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _behaviour._object )} : {o}",
				$"{nameof( go.GetComponents )} : ",
				string.Join( "\n", bs.Select( b => $"    {b.ToLineString()}" ) ),
				$"{nameof( _modifyler )} : {_modifyler}"
			) );
#endif
		}


		public override async UniTask Run() {
			var last = _target.GetBehaviourAtLast();
#if TestObjectModifyler
			SMLog.Debug( $"{nameof( Run )} : start" );
			SMLog.Debug( string.Join( "\n",
				$"{nameof( last )} : {last}",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _target )} : {_target}",
				$"{nameof( _modifyler )} : {_modifyler}"
			) );
#endif
			last._next = _behaviour;
			_behaviour._previous = last;
			_behaviour._object = _target;
			_owner.SetAllData();

			_behaviour.Constructor();
			_modifyler.Register( new InitializeBehaviourSMObject( _target, _behaviour ) );

			await UTask.DontWait();

#if TestObjectModifyler
			SMLog.Debug( string.Join( "\n",
				$"{nameof( last )} : {last}",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _target )} : {_target}",
				$"{nameof( _modifyler )} : {_modifyler}"
			) );
			SMLog.Debug( $"{nameof( Run )} : end" );
#endif
		}
	}
}