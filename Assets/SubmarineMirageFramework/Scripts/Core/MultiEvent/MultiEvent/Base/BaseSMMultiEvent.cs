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
	using Base;
	using Extension;
	using Utility;


	// TODO : コメント追加、整頓


	public abstract class BaseSMMultiEvent<T> : SMRawBase {
		public readonly List< KeyValuePair<string, T> > _events = new List< KeyValuePair<string, T> >();
		protected SMEventModifyler<T> _modifyler	{ get; private set; }


		public BaseSMMultiEvent() {
			_modifyler = new SMEventModifyler<T>( this );
			_disposables.Add( () => {
				_modifyler.Dispose();
				_events.ForEach( pair => OnRemove( pair.Value ) );
				_events.Clear();
			} );
		}


		protected void Register( SMEventModifyData<T> data ) => _modifyler.Register( data );


		public void InsertFirst( string findKey, string key, T function )
			=> Register( new InsertSMEvent<T>( findKey, SMEventAddType.First, key, function ) );

		public void InsertFirst( string findKey, T function )
			=> Register( new InsertSMEvent<T>( findKey, SMEventAddType.First, string.Empty, function ) );

		public void InsertLast( string findKey, string key, T function )
			=> Register( new InsertSMEvent<T>( findKey, SMEventAddType.Last, key, function ) );

		public void InsertLast( string findKey, T function )
			=> Register( new InsertSMEvent<T>( findKey, SMEventAddType.Last, string.Empty, function ) );


		public void AddFirst( string key, T function )
			=> Register( new AddSMEvent<T>( SMEventAddType.First, key, function ) );

		public void AddFirst( T function )
			=> Register( new AddSMEvent<T>( SMEventAddType.First, string.Empty, function ) );

		public void AddLast( string key, T function )
			=> Register( new AddSMEvent<T>( SMEventAddType.Last, key, function ) );

		public void AddLast( T function )
			=> Register( new AddSMEvent<T>( SMEventAddType.Last, string.Empty, function ) );


		public void Reverse() => Register( new ReverseSMEvent<T>() );


		public void Remove( string key ) => Register( new RemoveSMEvent<T>( key ) );

		public abstract void OnRemove( T function );


		public void CheckDisposeError( SMEventModifyData<T> _data = null ) {
			if ( !_isDispose )	{ return; }
			if ( _data != null && _data._function != null ) {
				OnRemove( _data._function );
			}
			throw new ObjectDisposedException( $"{nameof( _disposables )}", "既に解放済" );
		}


		public override string ToString( int indent ) {
			var nameI = StringSMUtility.IndentSpace( indent );
			var memberI = StringSMUtility.IndentSpace( indent + 1 );
			var arrayI = StringSMUtility.IndentSpace( indent + 2 );

			return string.Join( "\n",
				$"{nameI}{this.GetAboutName()}(",
				$"{memberI}{nameof( _id )} : {_id},",
				$"{memberI}{nameof( _isDispose )} : {_isDispose},",
				$"{memberI}{nameof( _events )} :",
				string.Join( ",\n", _events.Select( pair =>
					$"{arrayI}{pair.Key} : {pair.Value.GetAboutName()}"
				) ),
				$"{memberI}{nameof( _modifyler )} : {_modifyler.ToString( indent + 1 )},",
				$"{nameI})"
			);
		}
	}
}