//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
//#define TestService
namespace SubmarineMirage {
	using System;
	using System.Collections.Generic;
	using Cysharp.Threading.Tasks;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ オブジェクト注入のクラス
	/// </summary>
	///====================================================================================================
	public static class SMServiceLocator {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		[SMShowLine] public static bool s_isDisposed => s_disposables._isDispose;
		static readonly SMDisposable s_disposables = new SMDisposable();
		public static readonly Dictionary<Type, ISMService> s_container = new Dictionary<Type, ISMService>();

		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		static SMServiceLocator() {
#if TestService
			SMLog.Debug( $"{nameof( SMServiceLocator )}()", SMLogTag.Service );
#endif

			s_disposables.AddFirst( () => {
#if TestService
				SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Dispose )}", SMLogTag.Service );
#endif
				s_container.ForEach( pair => pair.Value?.Dispose() );
				s_container.Clear();
			} );
		}

		/// <summary>
		/// ● 破棄
		/// </summary>
		public static void Dispose()
			=> s_disposables.Dispose();



		public static T Register<T>( T instance ) where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			var type = typeof( T );
			if ( s_container.ContainsKey( type ) ) {
				throw new InvalidOperationException( $"既に登録済 : {type}" );
			}
#if TestService
			SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Register )} : {type.GetAboutName()}",
				SMLogTag.Service );
#endif
			s_container[type] = instance;
			return instance;
		}

		public static void Unregister<T>( bool isCallDispose = true ) where T : class, ISMService {
			if ( s_isDisposed )	{ return; }

			var type = typeof( T );
#if TestService
			SMLog.Debug( $"{nameof( SMServiceLocator )}.{nameof( Unregister )} : {type.GetAboutName()}",
				SMLogTag.Service );
#endif
			if ( isCallDispose ) {
				var instance = s_container.GetOrDefault( type );
				instance?.Dispose();
			}
			s_container.Remove( type );
		}



		public static T Resolve<T>() where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			return s_container.GetOrDefault( typeof( T ) ) as T;
		}

		public static async UniTask<T> WaitResolve<T>( SMAsyncCanceler canceler ) where T : class, ISMService {
			if ( s_isDisposed )	{ return null; }

			var type = typeof( T );
			ISMService instance = null;
			await UTask.WaitWhile( canceler, () => {
				instance = s_container.GetOrDefault( type );
				return instance == null;
			} );
			return instance as T;
		}
	}
}