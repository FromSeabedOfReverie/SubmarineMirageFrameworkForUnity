//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using Service;
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class BaseSM : IBaseSM {
		[SMShowLine] public uint _id	{ get; private set; }


		public BaseSM() {
			var idCounter = SMServiceLocator.Resolve<SMIDCounter>();
			_id = idCounter?.GetNewID( this ) ?? 0;
		}

		public abstract void Dispose();

		~BaseSM() => Dispose();

		public override string ToString() => ToString( 0 );
		public virtual string ToString( int indent ) => this.ToShowString( indent );
		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}