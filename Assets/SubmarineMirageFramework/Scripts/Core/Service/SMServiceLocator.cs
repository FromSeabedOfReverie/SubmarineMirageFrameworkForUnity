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
	using Task.Behaviour;
	using Scene;
	using Extension;
	using Utility;
	using Debug;



	// TODO : コメント追加、整頓



	public static class SMServiceLocator {
		[SMShowLine] public static bool s_isDisposed	{ get; private set; } = true;
		public static readonly Dictionary<Type, ISMService> s_container = new Dictionary<Type, ISMService>();


		static SMServiceLocator() {
			s_isDisposed = false;
		}


		public static void Dispose() {
			if ( s_isDisposed )	{ return; }

			SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Dispose )}", SMLogTag.Service );
			s_isDisposed = true;
			s_container.ForEach( pair => pair.Value.Dispose() );
			s_container.Clear();
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
				case SMMonoBehaviour b:
					if ( b._lifeSpan != SMTaskLifeSpan.Forever ) {
						throw new InvalidOperationException(
							$"{SMTaskLifeSpan.Forever}でない : {nameof( b._lifeSpan )}" );
					}
					break;
				case MonoBehaviourSMExtension b:
					var _sceneManager = Resolve<SMSceneManager>();
					if ( _sceneManager != null )	{ _sceneManager.MoveForeverScene( b.gameObject ); }
					else							{ b.gameObject.DontDestroyOnLoad(); }
					break;
			}

			s_container[type] = instance;
			return instance;
		}

		public static T Register<T>( T instance = null ) where T : class, ISMService
			=> (T)Register( typeof( T ), instance );


		static ISMService Create( Type type ) {
			if ( type.IsInheritance( typeof( SMMonoBehaviour ) ) ) {
				var o = SMObjectSMUtility.Create( type );
				return (ISMService)o._behaviour;

			} else if ( type.IsInheritance( typeof( MonoBehaviourSMExtension ) ) ) {
				var go = new GameObject( type.GetAboutName() );
				return (ISMService)go.AddComponent( type );

			} else {
				return (ISMService)type.Create();
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
			}
			s_container.Remove( type );
		}


		public static T Resolve<T>() where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			return (T)s_container.GetOrDefault( typeof( T ) );
		}

		public static async UniTask<T> WaitResolve<T>( SMTaskCanceler canceler ) where T : class, ISMService {
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