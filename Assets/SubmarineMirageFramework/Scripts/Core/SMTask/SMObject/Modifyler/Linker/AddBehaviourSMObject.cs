//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
#define TestSMTaskModifyler
namespace SubmarineMirage.SMTask.Modifyler {
	using System;
	using System.Linq;
	using Cysharp.Threading.Tasks;
	using UTask;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public class AddBehaviourSMObject : SMObjectModifyData {
		public SMMonoBehaviour _behaviour	{ get; private set; }


		public AddBehaviourSMObject( SMObject smObject, Type type ) : base( smObject ) {
			_type = ModifyType.Linker;

			if ( !_object._isGameObject ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{_object}" );
			}
			_behaviour = (SMMonoBehaviour)_object._owner.AddComponent( type );
			if ( _behaviour == null ) {
				throw new NotSupportedException( $"{nameof( SMMonoBehaviour )}で無い為、追加不可 :\n{type}" );
			}
		}

		public override void Cancel() {
#if TestSMTaskModifyler
			var o = _behaviour?._object;
			var go = _behaviour?.gameObject;
			var bs = go?.GetComponents<SMMonoBehaviour>() ?? new SMMonoBehaviour[0];
			Log.Debug( string.Join( "\n",
				$"{nameof( Cancel )} start :",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _behaviour._object )} : {o}",
				$"{nameof( go.GetComponents )} : ",
				string.Join( "\n", bs.Select( b => $"    {b.ToLineString()}" ) ),
				$"{nameof( _owner )} : {_owner}"
			) );
#endif
			_behaviour.Dispose();
#if TestSMTaskModifyler
			bs = go?.GetComponents<SMMonoBehaviour>() ?? new SMMonoBehaviour[0];
			Log.Debug( string.Join( "\n",
				$"{nameof( Cancel )} end :",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _behaviour._object )} : {o}",
				$"{nameof( go.GetComponents )} : ",
				string.Join( "\n", bs.Select( b => $"    {b.ToLineString()}" ) ),
				$"{nameof( _owner )} : {_owner}"
			) );
#endif
		}


		public override async UniTask Run() {
			var last = _object.GetBehaviourAtLast();
#if TestSMTaskModifyler
			Log.Debug( $"{nameof( Run )} : start" );
			Log.Debug( string.Join( "\n",
				$"{nameof( last )} : {last}",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _object )} : {_object}",
				$"{nameof( _owner )} : {_owner}"
			) );
#endif
			last._next = _behaviour;
			_behaviour._previous = last;
			_behaviour._object = _object;
			_group.SetAllData();

			_behaviour.Constructor();
			_owner.Register( new InitializeBehaviourSMObject( _object, _behaviour ) );

			await UTask.DontWait();

#if TestSMTaskModifyler
			Log.Debug( string.Join( "\n",
				$"{nameof( last )} : {last}",
				$"{nameof( _behaviour )} : {_behaviour}",
				$"{nameof( _object )} : {_object}",
				$"{nameof( _owner )} : {_owner}"
			) );
			Log.Debug( $"{nameof( Run )} : end" );
#endif
		}


		public override string ToString() => base.ToString().InsertLast( ", ",
			string.Join( ", ",
				_behaviour?.ToLineString()
			)
			+ ", "
		);
	}
}