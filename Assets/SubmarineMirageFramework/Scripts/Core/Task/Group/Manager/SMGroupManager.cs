//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using SubmarineMirage.Base;
	using Task.Base;
	using Scene;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroupManager : SMStandardBase {
		[SMShowLine] public SMGroupManagerBody _body	{ get; private set; }



		public SMGroupManager( SMScene scene ) {
			_body = new SMGroupManagerBody( this, scene );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();



		public T GetBehaviour<T>() where T : ISMBehaviour
			=> (T)GetBehaviour( typeof( T ) );

		public ISMBehaviour GetBehaviour( Type type )
			=> GetBehaviours( type ).FirstOrDefault();

		public IEnumerable<T> GetBehaviours<T>() where T : ISMBehaviour
			=> GetBehaviours( typeof( T ) )
				.Select( b => (T)b );

		public IEnumerable<ISMBehaviour> GetBehaviours( Type type ) {
			var currents = new Queue<SMObjectBody>( _body.GetAllTops() );
			while ( !currents.IsEmpty() ) {
				var o = currents.Dequeue();
				foreach ( var b in o._object.GetBehaviours( type ) ) {
					yield return b;
				}
				o.GetChildren().ForEach( oc => currents.Enqueue( oc ) );
			}
		}
	}
}