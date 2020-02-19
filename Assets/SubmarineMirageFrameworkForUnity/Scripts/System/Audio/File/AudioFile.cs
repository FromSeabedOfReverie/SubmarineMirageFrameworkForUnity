//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework.Audio {
	using System;
	using System.IO;
	using System.Threading;
	using UnityEngine;
	using UniRx.Async;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ AudioClipのラッパークラス
	///----------------------------------------------------------------------------------------------------
	///		音ファイルに関する必要な情報を定義。
	/// </summary>
	///====================================================================================================
	public class AudioFile<T> where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>リソース読込の階層</summary>
		const string RESOURCE_PATH = "Audio";
		/// <summary>読込状態</summary>
		public enum State {
			/// <summary>読込中</summary>
			Loading,
			/// <summary>読込完了</summary>
			Loaded,
			/// <summary>読込失敗</summary>
			LoadError,
			/// <summary>読込解除</summary>
			Unload
		}

		/// <summary>アセット名</summary>
		public string _fileName		{ get; private set; }
		/// <summary>階層名</summary>
		public string _path			{ get; private set; }
		/// <summary>定数名</summary>
		public T _enumName			{ get; private set; }
		/// <summary>音量</summary>
		public float _volume		{ get; private set; }
		/// <summary>音ファイル本体</summary>
		public AudioClip _audioClip	{ get; private set; }
		/// <summary>読込状態</summary>
		public State _state			{ get; private set; } = State.Unload;
		/// <summary>読込処理停止用の記号</summary>
		CancellationTokenSource _loadCancel = new CancellationTokenSource();
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public AudioFile( string fileName, string path, T enumName, float volume ) {
			_fileName = fileName;
			_path = path;
			_enumName = enumName;
			_volume = Mathf.Clamp01( volume );
		}
		///------------------------------------------------------------------------------------------------
		/// ● 読込
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public async UniTask Load( CancellationTokenSource cancel = null ) {
			// 読込中、読込済の場合は、未処理
			if ( _state == State.Loading || _state == State.Loaded )	{ return; }
			_state = State.Loading;

			_loadCancel = cancel != null ? cancel : new CancellationTokenSource();
			var cancelToken = _loadCancel.Token;


			// 処理停止命令が入っても、続行
			try {
				// とりあえずリソースから読む（判定方法が無い為）
				if ( !cancelToken.IsCancellationRequested ) {
					await LoadResource( cancelToken );
				}
				// 失敗したらアセットバンドルから読む
				if ( !cancelToken.IsCancellationRequested && _audioClip == null ) {
					await LoadAssetBundle( cancelToken );
				}
			} catch ( OperationCanceledException _ ) {}	// 処理停止例外を無視


			// 読込成功の場合、読込済とする
			if ( _audioClip != null ) {
				_state = State.Loaded;

			// 読込失敗の場合、処理停止エラー発生
			} else {
				_state = State.LoadError;
				Log.Error( $"音読込失敗 : {Path.Combine( _path, _fileName )}", Log.Tag.Audio );
				_loadCancel.Cancel();
				throw new OperationCanceledException( cancelToken );
			}
		}
		/// <summary>
		/// ● 読込（リソース）
		/// </summary>
		async UniTask LoadResource( CancellationToken cancel ) {
			var request = Resources.LoadAsync( Path.Combine( RESOURCE_PATH, _path, _fileName ) );
			await request.ConfigureAwait( null, PlayerLoopTiming.Update, cancel );
			_audioClip = (AudioClip)request.asset;
		}
		/// <summary>
		/// ● 読込（アセットバンドル）
		/// </summary>
		async UniTask LoadAssetBundle( CancellationToken cancel ) {
			var isDone = false;
			// TODO : アセットバンドル読込に対応させる
			isDone = true;
			await UniTask.WaitUntil( () => isDone, PlayerLoopTiming.Update, cancel );
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込解除
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public void Unload() {
			// 未読込の場合は、未処理
			if ( _state == State.Unload )	{ return; }

			_loadCancel.Cancel();
			_state = State.Unload;
			Resources.UnloadAsset( _audioClip );	// 実機でのみ解放される
		}
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デストラクタ
		///		何時呼ばれるか不明なので、明示的にUnloadを呼ぶべき。
		/// </summary>
		///------------------------------------------------------------------------------------------------
		~AudioFile() {
			Unload();
		}
	}
}