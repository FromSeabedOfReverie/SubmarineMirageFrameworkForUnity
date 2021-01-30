//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Debug {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using KoganeUnityLib;
	using Base;
	using Service;
	using Extension;
	using Utility;



	// TODO : コメント追加、整頓



	public class SMIDCounter : SMStandardBase, ISMService {
		readonly Dictionary<Type, uint> _idCounts = new Dictionary<Type, uint>();


		public SMIDCounter() {
			_disposables.AddLast( () => _idCounts.Clear() );
		}


		public uint GetLastID( Type type )
			=> _idCounts.GetOrDefault( type );

		public uint GetNewID( IBaseSM baseSM ) {
			var type = baseSM.GetType();
			return _idCounts[type] = GetLastID( type ) + 1;
		}


		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _idCounts ), i => "\n" +
				string.Join( ",\n", _idCounts.Select( pair =>
					StringSMUtility.IndentSpace( i + 1 ) +
					$"{pair.Key.GetAboutName()} : {pair.Value}"
				) )
			);
		}
	}
}