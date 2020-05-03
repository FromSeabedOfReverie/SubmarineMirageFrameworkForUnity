//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.TestProcess {
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx;
	using UniRx.Async;
	using KoganeUnityLib;
	using Process.New;
	using Extension;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestProcessUtility {
		public static void CreateMonoBehaviourProcess( string processData ) {
			var parents = new Dictionary<int, Transform>();
			processData
				.UnifyNewLine()
				.Split( '\n' )
				.Select( s => new {
					i = s.CountString( "\t" ),
					s = s.Replace( "\t", "" ),
				} )
				.Where( a => !a.s.IsNullOrEmpty() )
				.Select( a => new {
					a.i,
					types = a.s
						.Replace( " ", "" )
						.Split( "," )
						.Where( s => !s.IsNullOrEmpty() ),
				} )
				.ForEach( ( a, i ) => {
					var go = new GameObject( $"indent : {a.i}, id : {i}" );
					go.SetParent( parents.GetOrDefault( a.i - 1 ) );
					parents[a.i] = go.transform;
					go.SetActive( false );
					a.types
						.Where( t => t != "null" )
						.ForEach( t =>
							go.AddComponent( Type.GetType( $"{typeof(BaseM).Namespace}.{t}" ) )
						);
				} );
		}
	}



	public abstract class BaseB : BaseProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
		static int s_count;
		public int _id;
		public override void Create() => _id = s_count++;
	}
	public class B1 : BaseB {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
	}
	public class B2 : BaseB {
		public override ProcessBody.Type _type => ProcessBody.Type.Work;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
	}
	public class B3 : BaseB {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
	}
	public class B4 : BaseB {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
	}
	public class B5 : BaseB {
		public override ProcessBody.Type _type => ProcessBody.Type.Work;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
	}
	public class B6 : BaseB {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
	}



	public abstract class BaseM : MonoBehaviourProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
		static int s_count;
		public int _id;
		public override void Create() => _id = s_count++;
	}
	public class M1 : BaseM {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
	}
	public class M2 : BaseM {
		public override ProcessBody.Type _type => ProcessBody.Type.Work;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
	}
	public class M3 : BaseM {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
	}
	public class M4 : BaseM {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
	}
	public class M5 : BaseM {
		public override ProcessBody.Type _type => ProcessBody.Type.Work;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
	}
	public class M6 : BaseM {
		public override ProcessBody.Type _type => ProcessBody.Type.FirstWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.Forever;
	}
}