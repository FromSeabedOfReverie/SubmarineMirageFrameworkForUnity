//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.Runtime.Serialization;



	[Serializable]
	public abstract class SMException : Exception, ISMLightBase {
		public uint _id { get; private set; }



#region ToString
		public virtual string AddToString( int indent )
			=> string.Empty;
#endregion



		public SMException() : base() {
			_id = SMIDCounter.GetNewID( this );
		}

		public SMException( string message ) : base( message ) {
			_id = SMIDCounter.GetNewID( this );
		}

		public SMException( string message, Exception innerException ) : base( message, innerException ) {
			_id = SMIDCounter.GetNewID( this );
		}

		protected SMException( SerializationInfo info, StreamingContext context ) : base( info, context ) {
			_id = SMIDCounter.GetNewID( this );
		}

		public virtual void Dispose() {}

		~SMException() => Dispose();



		public override string ToString() => ToString( 0 );

		public virtual string ToString( int indent, bool isUseHeadIndent = true )
			=> this.ToShowString( indent, false, false, isUseHeadIndent );

		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}