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



	// TODO : コメント追加、整頓



	public static class SMServiceLocator {
		public static bool s_isDisposed	{ get; private set; }
		static readonly Dictionary<Type, ISMService> s_container = new Dictionary<Type, ISMService>();


		static SMServiceLocator() {
			s_isDisposed = false;
		}


		public static void Dispose() {
			if ( s_isDisposed )	{ return; }

			s_isDisposed = true;
			s_container.ForEach( pair => pair.Value.Dispose() );
			s_container.Clear();
		}


		public static T Register<T>( T instance ) where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			var type = typeof( T );
			if ( s_container.ContainsKey( type ) ) {
				throw new InvalidOperationException( $"既に登録済 : {type}" );
			}
			s_container[type] = instance;
			return instance;
		}

		public static T RegisterRawBehaviour<T>() where T : MonoBehaviourSMExtension, ISMService {
			if ( s_isDisposed )	{ return null; }

			var type = typeof( T );
			if ( s_container.ContainsKey( type ) ) {
				throw new InvalidOperationException( $"既に登録済 : {type}" );
			}
			var go = new GameObject( type.GetAboutName() );
			var b = go.AddComponent<T>();
			Resolve<SMSceneManager>().MoveForeverScene( go );
			s_container[type] = b;
			return b;
		}

		public static T RegisterBehaviour<T>() where T : SMMonoBehaviour, ISMService {
			if ( s_isDisposed )	{ return null; }

			var type = typeof( T );
			if ( s_container.ContainsKey( type ) ) {
				throw new InvalidOperationException( $"既に登録済 : {type}" );
			}
			var o = SMObjectSMUtility.Create<T>();
			if ( o._behaviour._lifeSpan != SMTaskLifeSpan.Forever ) {
				throw new InvalidOperationException(
					$"{SMTaskLifeSpan.Forever}でない : {nameof( o._behaviour._lifeSpan )}" );
			}
			var b = (T)o._behaviour;
			s_container[type] = b;
			return b;
		}


		public static void Unregister<T>( bool isDispose = true ) where T : class, ISMService {
			if ( s_isDisposed )	{ return; }

			var type = typeof( T );
			if ( isDispose )	{ s_container.GetOrDefault( type )?.Dispose(); }
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