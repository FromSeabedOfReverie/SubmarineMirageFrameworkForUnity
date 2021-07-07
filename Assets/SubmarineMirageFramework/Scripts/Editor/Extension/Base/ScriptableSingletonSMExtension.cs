//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.EditorExtension {
	using UnityEngine;
	using UnityEditor;
	using Base;
	using Extension;
	using Debug;



	public abstract class ScriptableSingletonSMExtension<T> : ScriptableSingleton<T>, ISMLightBase
		where T : ScriptableObject
	{
		[SMShowLine] public uint _id	{ get; private set; }



		public virtual string AddToString( int indent )
			=> string.Empty;



		protected virtual void Awake() {
			_id = SMIDCounter.GetNewID( this );
		}

		protected virtual void OnDestroy() => Dispose();

		public abstract void Dispose();



		public override string ToString() => ToString( 0 );

		public virtual string ToString( int indent, bool isUseHeadIndent = true )
			=> this.ToShowString( indent, false, false, isUseHeadIndent );

		public virtual string ToLineString( int indent = 0 ) => ObjectSMExtension.ToLineString( this, indent );
	}
}