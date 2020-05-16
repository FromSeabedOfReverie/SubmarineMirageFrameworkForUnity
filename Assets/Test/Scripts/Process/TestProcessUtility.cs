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
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class TestProcessUtility {
		public static IProcess CreateMonoBehaviourProcess( string processData, bool isTopActive = true ) {
			IProcess top = null;

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
					a.types
						.Where( t => t != "null" )
						.ForEach( t => {
							var c = go.AddComponent( Type.GetType( $"{typeof(BaseM).Namespace}.{t}" ) );
							if ( top == null ) {
								top = (IProcess)c;
								go.SetActive( isTopActive );
							}
						} );
				} );

			return top;
		}


		public static void LogHierarchy( string text, ProcessHierarchy hierarchy ) {
			if ( hierarchy == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = hierarchy._owner != null ? hierarchy._owner.name : null;
			Log.Debug( $"{text} : " + string.Join( ", ",
				hierarchy._processes.Select( p => p.GetAboutName() )
			) + $" : {name}" );
		}

		public static void LogHierarchies( string text, IEnumerable<ProcessHierarchy> hierarchies ) {
			Log.Debug( $"{text} :\n" + string.Join( "\n",
				hierarchies.Select( h => {
					var name = h._owner != null ? h._owner.name : null;
					return string.Join( ", ",
						h._processes.Select( p => p.GetAboutName() )
					) + $" : {name}";
				} )
			) );
		}


		public static void LogProcess( string text, IProcess process ) {
			if ( process == null ) {
				Log.Debug( $"{text} : null" );
				return;
			}
			var name = process._hierarchy._owner != null ? process._hierarchy._owner.name : null;
			var id = ( process as BaseM )?._id ?? ( process as BaseB )?._id;
			Log.Debug( $"{text} : {process.GetAboutName()}, {name}, processID : {id}" );
		}

		public static void LogProcesses( string text, IEnumerable<IProcess> processes ) {
			Log.Debug( $"{text} :\n" + string.Join( "\n",
				processes.Select( p => {
					var name = p._hierarchy._owner != null ? p._hierarchy._owner.name : null;
					var id = ( p as BaseM )?._id ?? ( p as BaseB )?._id;
					return $"{p.GetAboutName()}, {name}, processID : {id}";
				} )
			) );
		}
	}



	public abstract class BaseB : BaseProcess {
		public override ProcessBody.Type _type => ProcessBody.Type.DontWork;
		public override ProcessBody.LifeSpan _lifeSpan => ProcessBody.LifeSpan.InScene;
		static int s_count = 0;
		public int _id;
		public BaseB() => _id = s_count++;
		public override void Create() {}
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
		static int s_count = 0;
		public int _id;
		public BaseM() => _id = s_count++;
		public override void Create() {}
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