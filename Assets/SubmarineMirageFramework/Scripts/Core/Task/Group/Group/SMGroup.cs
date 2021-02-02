//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using System.Collections.Generic;
	using UnityEngine;
	using SubmarineMirage.Base;
	using Task.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroup : SMStandardBase {
		[SMShowLine] public SMGroupBody _body	{ get; private set; }



		public SMGroup( GameObject gameObject, IEnumerable<ISMBehaviour> behaviours ) {
			_body = new SMGroupBody( this, gameObject, behaviours );
			SMGroupSub();
		}

		public SMGroup( SMObjectBody objectBody ) {
			_body = new SMGroupBody( this, objectBody );
			SMGroupSub();
		}

		void SMGroupSub() {
			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();
	}
}