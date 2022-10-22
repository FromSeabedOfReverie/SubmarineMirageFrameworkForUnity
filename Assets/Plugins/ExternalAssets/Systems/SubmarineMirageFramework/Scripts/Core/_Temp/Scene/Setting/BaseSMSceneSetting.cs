//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using System.Collections.Generic;



	public abstract class BaseSMSceneSetting : SMStandardBase, ISMService {
		public Dictionary< Type, IEnumerable<Type> > _datas	{ get; protected set; }
			= new Dictionary< Type, IEnumerable<Type> >();



		public BaseSMSceneSetting() {
			_disposables.AddFirst( () => {
				_datas.Clear();
			} );
		}

		public abstract void Setup();
	}
}