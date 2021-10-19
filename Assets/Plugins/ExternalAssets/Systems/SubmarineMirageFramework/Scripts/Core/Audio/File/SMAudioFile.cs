//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using System.IO;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	///====================================================================================================
	/// <summary>
	/// ■ AudioClipのラッパークラス
	///		音ファイルに関する必要な情報を定義。
	/// </summary>
	///====================================================================================================
	public class SMAudioFile<T> : SMStandardBase where T : struct, Enum {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>アセット名</summary>
		public string _fileName			{ get; private set; }
		/// <summary>階層名</summary>
		public string _path				{ get; private set; }
		/// <summary>定数名</summary>
		public T _enumName				{ get; private set; }
		/// <summary>音量</summary>
		public float _volume			{ get; private set; }
		/// <summary>音ファイル本体</summary>
		public AudioClip _audioClip		{ get; private set; }
		/// <summary>読込状態</summary>
		public SMAudioFileState _state	{ get; private set; }

		/// <summary>読込の非同期停止用の識別子</summary>
		readonly SMAsyncCanceler _loadCanceler = new SMAsyncCanceler();

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public SMAudioFile( string fileName, string path, T enumName, float volume ) {
			_fileName = fileName;
			_path = path;
			_enumName = enumName;
			_volume = Mathf.Clamp01( volume );

			_disposables.AddFirst( () => {
				Unload();
				_loadCanceler.Dispose();
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 読込
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public async UniTask Load() {
			// 読込中、読込済の場合は、未処理
			switch ( _state ) {
				case SMAudioFileState.Loading:
				case SMAudioFileState.Loaded:
					return;
			}
			_state = SMAudioFileState.Loading;

			_loadCanceler.Cancel();

			try {
				// とりあえずリソースから読む（判定方法が無い為）
				await LoadResource();
				// 失敗したらアセットバンドルから読む
				if ( _audioClip == null ) {
					await LoadAssetBundle();
				}
			} catch {
				_state = SMAudioFileState.LoadError;
				throw;
			}

			// 読込失敗を判定
			if ( _audioClip == null ) {
				_state = SMAudioFileState.LoadError;
				throw new InvalidOperationException( string.Join( "\n",
					$"音読込失敗 : {Path.Combine( _path, _fileName )}"
				) );
			}

			// 読込成功の場合、読込済とする
			_state = SMAudioFileState.Loaded;
		}
		
		/// <summary>
		/// ● 読込（リソース）
		/// </summary>
		async UniTask LoadResource() {
			var path = Path.Combine( SMMainSetting.Audio_RESOURCE_PATH, _path, _fileName );
			_audioClip = await Resources.LoadAsync( path )
				.ToUniTask( _loadCanceler )
				as AudioClip;
		}

		/// <summary>
		/// ● 読込（アセットバンドル）
		/// </summary>
		async UniTask LoadAssetBundle() {
			var isDone = false;
// TODO : アセットバンドル読込に対応させる
			isDone = true;
			await UTask.WaitWhile( _loadCanceler, () => !isDone );
		}

		/// <summary>
		/// ● 読込解除
		/// </summary>
		public void Unload() {
			// 未読込の場合は、未処理
			if ( _state == SMAudioFileState.Unload )	{ return; }

			_loadCanceler.Cancel();
			_state = SMAudioFileState.Unload;
			Resources.UnloadAsset( _audioClip );	// 実機でのみ破棄される
		}
	}
}