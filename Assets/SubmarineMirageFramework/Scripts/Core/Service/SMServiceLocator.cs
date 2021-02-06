//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Service {
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	using Task;
	using Scene;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMServiceLocator {
		[SMShowLine] public static bool s_isDisposed	{ get; private set; } = true;
		public static readonly Dictionary<Type, ISMService> s_container = new Dictionary<Type, ISMService>();
		static SMSceneManager s_sceneManager	{ get; set; }

		static SMServiceLocator() {
			s_isDisposed = false;
		}

		public static void Dispose() {
			if ( s_isDisposed )	{ return; }

			SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Dispose )}", SMLogTag.Service );
			s_isDisposed = true;

			s_container.ForEach( pair => pair.Value.Dispose() );
			s_container.Clear();
			s_sceneManager = null;
		}



		public static ISMService Register( Type type, ISMService instance = null ) {
			if ( s_isDisposed )	{ return null; }
			if ( s_container.ContainsKey( type ) ) {
				throw new InvalidOperationException( $"既に登録済 : {type}" );
			}
			SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Register )} : {type.GetAboutName()}",
				SMLogTag.Service );

			if ( instance == null )	{ instance = Create( type ); }

			switch ( instance ) {
				case SMBehaviour b:
					break;
				case MonoBehaviourSMExtension b:
					if ( s_sceneManager != null )	{ s_sceneManager._body.MoveForeverScene( b.gameObject ); }
					else							{ b.gameObject.DontDestroyOnLoad(); }
					break;
			}

			if ( s_sceneManager == null && instance is SMSceneManager ) {
				s_sceneManager = (SMSceneManager)instance;
			}

			s_container[type] = instance;
			return instance;
		}

		public static T Register<T>( T instance = null ) where T : class, ISMService
			=> (T)Register( typeof( T ), instance );


		static ISMService Create( Type type ) {
			if ( type.IsInheritance( typeof( SMBehaviour ) ) ) {
				return (ISMService)SMBehaviour.Generate( type, s_sceneManager._foreverScene );

			} else if ( type.IsInheritance( typeof(MonoBehaviourSMExtension) ) ) {
				var go = new GameObject( type.GetAboutName() );
				return (ISMService)go.AddComponent( type );

			} else {
				return type.Create<ISMService>();
			}
		}



		public static void Unregister<T>( bool isDispose = true ) where T : class, ISMService {
			if ( s_isDisposed )	{ return; }

			var type = typeof( T );
			SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Unregister )} : {type.GetAboutName()}",
				SMLogTag.Service );
			if ( isDispose ) {
				var instance = s_container.GetOrDefault( type );
				instance?.Dispose();
// TODO : SMBehaviourの場合、リンク解除、GameObject破棄等を行う
			}
			s_container.Remove( type );
		}


		public static T Resolve<T>() where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			return (T)s_container.GetOrDefault( typeof( T ) );
		}

		public static async UniTask<T> WaitResolve<T>( SMAsyncCanceler canceler ) where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			var type = typeof( T );
			ISMService instance = null;
			await UTask.WaitWhile( canceler, () => {
				instance = s_container.GetOrDefault( type );
				return instance == null;
			} );
			return (T)instance;
		}
	}
}