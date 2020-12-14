//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Base {
	using Extension;
	using Debug;


	// TODO : コメント追加、整頓


	public abstract class SMBase : ISMBase {
		[ShowLine] public uint _id	{ get; set; }


		public SMBase() {
			if ( !( this is SMBaseManager ) )	{ SMBaseManager.s_instance.SetID( this ); }
		}

		public abstract void Dispose();

		~SMBase() => Dispose();

		public override string ToString() => ToString( 0 );
		public virtual string ToString( int indent ) => this.ToShowString( indent );
		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}