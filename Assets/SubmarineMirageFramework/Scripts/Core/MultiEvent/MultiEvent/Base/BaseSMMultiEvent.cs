//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;
	using Base;
	using Extension;
	using Utility;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class BaseSMMultiEvent<T> : SMRawBase {
		public LinkedList< KeyValuePair<string, T> > _events	{ get; private set; }
			= new LinkedList< KeyValuePair<string, T> >();
		protected SMEventModifyler<T> _modifyler	{ get; private set; }


		public BaseSMMultiEvent() {
			_modifyler = new SMEventModifyler<T>( this );
			_disposables.Add( () => {
				_modifyler.Dispose();
				_events.ForEach( pair => OnRemove( pair.Value ) );
				_events.Clear();
			} );
		}


		public void InsertFirst( string findKey, string key, T function )
			=> _modifyler.Register( new InsertSMEvent<T>( findKey, SMEventAddType.First, key, function ) );

		public void InsertFirst( string findKey, T function )
			=> _modifyler.Register( new InsertSMEvent<T>( findKey, SMEventAddType.First, string.Empty, function ) );

		public void InsertLast( string findKey, string key, T function )
			=> _modifyler.Register( new InsertSMEvent<T>( findKey, SMEventAddType.Last, key, function ) );

		public void InsertLast( string findKey, T function )
			=> _modifyler.Register( new InsertSMEvent<T>( findKey, SMEventAddType.Last, string.Empty, function ) );


		public void AddFirst( string key, T function )
			=> _modifyler.Register( new AddSMEvent<T>( SMEventAddType.First, key, function ) );

		public void AddFirst( T function )
			=> _modifyler.Register( new AddSMEvent<T>( SMEventAddType.First, string.Empty, function ) );

		public void AddLast( string key, T function )
			=> _modifyler.Register( new AddSMEvent<T>( SMEventAddType.Last, key, function ) );

		public void AddLast( T function )
			=> _modifyler.Register( new AddSMEvent<T>( SMEventAddType.Last, string.Empty, function ) );


		public void Reverse() => _modifyler.Register( new ReverseSMEvent<T>() );

		// 利用者は、これを呼ばない
		// 必ず、Modifyler経由で呼ぶ
		// その為、Reverseを使う
		public void ApplyReverse() => _events = _events.Reverse();


		public void Remove( string key ) => _modifyler.Register( new RemoveSMEvent<T>( key ) );

		public abstract void OnRemove( T function );


		public void CheckDisposeError( SMEventModifyData<T> _data = null ) {
			if ( !_isDispose )	{ return; }
			if ( _data != null && _data._function != null ) {
				OnRemove( _data._function );
			}
			throw new ObjectDisposedException( $"{nameof( _disposables )}", "既に解放済" );
		}


		public override string ToString( int indent, bool isUseHeadIndent = true ) {
			var prefix = StringSMUtility.IndentSpace( indent );
			var hPrefix = isUseHeadIndent ? prefix : "";
			indent++;
			var mPrefix = StringSMUtility.IndentSpace( indent );
			indent++;
			var aPrefix = StringSMUtility.IndentSpace( indent );

			return string.Join( "\n",
				$"{hPrefix}{this.GetAboutName()}(",
				$"{mPrefix}{nameof( _id )} : {_id},",
				$"{mPrefix}{nameof( _isDispose )} : {_isDispose},",
				$"{mPrefix}{nameof( _events )} :",
				string.Join( ",\n", _events.Select( pair =>
					$"{aPrefix}{pair.Key} : {pair.Value.GetAboutName()}"
				) ),
				$"{mPrefix}{nameof( _modifyler )} : {_modifyler.ToString( indent + 1 )},",
				$"{prefix})"
			);
		}
	}
}