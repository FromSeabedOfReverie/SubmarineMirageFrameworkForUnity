//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.MultiEvent {
	using System;
	using System.Collections.Generic;
	using UniRx;
	using KoganeUnityLib;


	// TODO : コメント追加、整頓


	public class MultiDisposable : BaseMultiEvent<IDisposable> {
		public override void OnRemove( IDisposable function ) => function.Dispose();


		public void InsertFirst( string findKey, string key, Action function )
			=> Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.First, key, Disposable.Create( function )
			) );

		public void InsertFirst( string findKey, Action function )
			=> Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.First, string.Empty, Disposable.Create( function )
			) );

		public void InsertFirst( string findKey, string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.First, key, f
			) ) );

		public void InsertFirst( string findKey, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.First, string.Empty, f
			) ) );

		public void InsertFirst( string findKey, string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.First, key, f
			) ) );

		public void InsertFirst( string findKey, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.First, string.Empty, f
			) ) );


		public void InsertLast( string findKey, string key, Action function )
			=> Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.Last, key, Disposable.Create( function )
			) );

		public void InsertLast( string findKey, Action function )
			=> Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.Last, string.Empty, Disposable.Create( function )
			) );

		public void InsertLast( string findKey, string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.Last, key, f
			) ) );

		public void InsertLast( string findKey, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.Last, string.Empty, f
			) ) );

		public void InsertLast( string findKey, string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.Last, key, f
			) ) );

		public void InsertLast( string findKey, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertEventModifyData<IDisposable>(
				findKey, EventAddType.Last, string.Empty, f
			) ) );


		public void AddFirst( string key, Action function )
			=> Register( new AddEventModifyData<IDisposable>(
				EventAddType.First, key, Disposable.Create( function )
			) );

		public void AddFirst( Action function )
			=> Register( new AddEventModifyData<IDisposable>(
				EventAddType.First, string.Empty, Disposable.Create( function )
			) );

		public void AddFirst( string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.First, key, f
			) ) );

		public void AddFirst( params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.First, string.Empty, f
			) ) );

		public void AddFirst( string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.First, key, f
			) ) );

		public void AddFirst( IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.First, string.Empty, f
			) ) );


		public void AddLast( string key, Action function ) {
			Register( new AddEventModifyData<IDisposable>(
				EventAddType.Last, key, Disposable.Create( function )
			) );
		}

		public void AddLast( Action function )
			=> Register( new AddEventModifyData<IDisposable>(
				EventAddType.Last, string.Empty, Disposable.Create( function )
			) );

		public void AddLast( string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.Last, key, f
			) ) );

		public void AddLast( params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.Last, string.Empty, f
			) ) );

		public void AddLast( string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.Last, key, f
			) ) );

		public void AddLast( IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddEventModifyData<IDisposable>(
				EventAddType.Last, string.Empty, f
			) ) );
	}
}