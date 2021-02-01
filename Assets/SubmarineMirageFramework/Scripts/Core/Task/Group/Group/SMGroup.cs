//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Task {
	using SubmarineMirage.Base;
	using Task.Base;
	using Debug;



	// TODO : コメント追加、整頓



	public class SMGroup : SMStandardBase {
		[SMShowLine] public SMGroupBody _body	{ get; private set; }



		public SMGroup( SMObject top ) {
			_body = new SMGroupBody( this, top );

			_disposables.AddLast( () => {
				_body.Dispose();
			} );
		}

		public override void Dispose() => base.Dispose();
	}
}