//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System;
	using Base;
	using Service;
	using Extension;



	// TODO : コメント追加、整頓



	[AttributeUsage(
		AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true
	)]
	public class SMShowLineAttribute : Attribute, ISMLightBase {
		public uint _id	{ get; private set; }

		public SMShowLineAttribute() {
			var manager = SMServiceLocator.Resolve<BaseSMManager>();
			_id = manager?.GetNewID( this ) ?? 0;
		}

		public void Dispose() {}

		~SMShowLineAttribute() => Dispose();

		public override string ToString() => ToString( 0 );
		public virtual string ToString( int indent ) => this.ToShowString( indent );
		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}