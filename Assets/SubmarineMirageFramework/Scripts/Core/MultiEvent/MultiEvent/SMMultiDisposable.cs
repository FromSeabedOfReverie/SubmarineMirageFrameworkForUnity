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



	public class SMMultiDisposable : BaseSMMultiEvent<IDisposable> {
		public override void OnRemove( IDisposable function ) => function.Dispose();


		public void InsertFirst( string findKey, string key, Action function )
			=> Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.First, key, Disposable.Create( function )
			) );

		public void InsertFirst( string findKey, Action function )
			=> Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.First, string.Empty, Disposable.Create( function )
			) );

		public void InsertFirst( string findKey, string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.First, key, f
			) ) );

		public void InsertFirst( string findKey, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.First, string.Empty, f
			) ) );

		public void InsertFirst( string findKey, string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.First, key, f
			) ) );

		public void InsertFirst( string findKey, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.First, string.Empty, f
			) ) );


		public void InsertLast( string findKey, string key, Action function )
			=> Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.Last, key, Disposable.Create( function )
			) );

		public void InsertLast( string findKey, Action function )
			=> Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.Last, string.Empty, Disposable.Create( function )
			) );

		public void InsertLast( string findKey, string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.Last, key, f
			) ) );

		public void InsertLast( string findKey, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.Last, string.Empty, f
			) ) );

		public void InsertLast( string findKey, string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.Last, key, f
			) ) );

		public void InsertLast( string findKey, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new InsertSMEvent<IDisposable>(
				findKey, SMEventAddType.Last, string.Empty, f
			) ) );


		public void AddFirst( string key, Action function )
			=> Register( new AddSMEvent<IDisposable>(
				SMEventAddType.First, key, Disposable.Create( function )
			) );

		public void AddFirst( Action function )
			=> Register( new AddSMEvent<IDisposable>(
				SMEventAddType.First, string.Empty, Disposable.Create( function )
			) );

		public void AddFirst( string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.First, key, f
			) ) );

		public void AddFirst( params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.First, string.Empty, f
			) ) );

		public void AddFirst( string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.First, key, f
			) ) );

		public void AddFirst( IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.First, string.Empty, f
			) ) );


		public void AddLast( string key, Action function ) {
			Register( new AddSMEvent<IDisposable>(
				SMEventAddType.Last, key, Disposable.Create( function )
			) );
		}

		public void AddLast( Action function )
			=> Register( new AddSMEvent<IDisposable>(
				SMEventAddType.Last, string.Empty, Disposable.Create( function )
			) );

		public void AddLast( string key, params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.Last, key, f
			) ) );

		public void AddLast( params IDisposable[] functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.Last, string.Empty, f
			) ) );

		public void AddLast( string key, IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.Last, key, f
			) ) );

		public void AddLast( IEnumerable<IDisposable> functions )
			=> functions.ForEach( f => Register( new AddSMEvent<IDisposable>(
				SMEventAddType.Last, string.Empty, f
			) ) );
	}
}