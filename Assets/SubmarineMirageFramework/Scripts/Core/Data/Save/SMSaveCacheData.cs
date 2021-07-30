//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage.Data.Save {
	using System;
	using System.Runtime.Serialization;
	using UnityEngine;
	using Cysharp.Threading.Tasks;
	using Service;
	using Data.Raw;
	using Data.Cache;
	using Extension;
	using Utility;
	using Debug;
	///====================================================================================================
	/// <summary>
	/// ■ 保存キャッシュの情報クラス
	///		このキャッシュに保存された情報は、必ずセーブされる。
	///		主に、サーバー受信情報のキャッシュ情報を格納。
	/// </summary>
	///====================================================================================================
	public class SMSaveCacheData<T> : BaseSMSaveData where T : class {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>登録鍵になる、書類の階層</summary>
		[SMShowLine] public string _path;
		/// <summary>情報の型名</summary>
		[SMShow] public string _typeName;    // Type型では、そのまま保存できない
		/// <summary>情報の型</summary>
		[SMShow] public SMDataType _typeEnum;
		/// <summary>生情報</summary>
		[SMShow] public T _rawData;

		/// <summary>元のキャッシュ情報</summary>
		[IgnoreDataMember, SMShow] public SMTemporaryCacheData _cacheData	{ get; set; }
		///------------------------------------------------------------------------------------------------
		/// ● 作成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ（読込用）
		/// </summary>
		public SMSaveCacheData() {
		}

		/// <summary>
		/// ● コンストラクタ（書込用）
		/// </summary>
		public SMSaveCacheData( SMTemporaryCacheData cacheData, SMDataType typeEnum ) {
			_cacheData = cacheData;
			_typeEnum = typeEnum;
		}

		/// <summary>
		/// ● 解放（補助）
		/// </summary>
		protected override void DisposeSub() {
			base.DisposeSub();

			_path = null;
			_typeName = null;
			_rawData = null;
			_cacheData?.Dispose();
		}

		///------------------------------------------------------------------------------------------------
		/// ● 登録、解除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 読込
		/// </summary>
		public override async UniTask Load() {
			if ( _cacheData != null ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"キャッシュ情報が、既に読込済 : ",
					$"{nameof( SMSaveCacheData<T> )}.{nameof( Load )}",
					$"{nameof( _cacheData )}{_cacheData}",
					$"{this}"
				) );
			}

			_cacheData = new SMTemporaryCacheData();
			_cacheData._path = _path;
			_cacheData._type = Type.GetType( _typeName );

			switch ( _typeEnum ) {
				case SMDataType.Texture:
					_cacheData._data = ( _rawData as SMTextureRawData ).ToTexture2D();
					break;
				case SMDataType.Sprite:
					_cacheData._data = ( _rawData as SMTextureRawData ).ToSprite();
					break;
				case SMDataType.Audio:
					_cacheData._data = ( _rawData as SMAudioRawData ).ToAudioClip();
					break;
				case SMDataType.Text:
				case SMDataType.Raw:
					_cacheData._data = _rawData;
					break;
			}

			await base.Load();
		}

		/// <summary>
		/// ● 保存
		/// </summary>
		public override async UniTask Save() {
			if ( _cacheData == null ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"キャッシュ情報が、未読込 : ",
					$"{nameof( SMSaveCacheData<T> )}.{nameof( Save )}",
					$"{nameof( _cacheData )}{_cacheData}",
					$"{this}"
				) );
			}

			// 読込中の場合、読込完了まで待機
			if ( _cacheData._data == null ) {
				SMLog.Debug( $"保存前、読込完了まで待機 : start\n{_cacheData}" );
				var allDataManager = SMServiceLocator.Resolve<SMAllDataManager>();
				await UTask.WaitWhile( allDataManager._asyncCancelerOnDispose, () => _cacheData._data == null );
				SMLog.Debug( $"保存前、読込完了まで待機 : end\n{_cacheData}" );
			}

			_path = _cacheData._path;
			_typeName = _cacheData._type.FullName;

			switch ( _typeEnum ) {
				case SMDataType.Texture:
					_rawData = ( _cacheData._data as Texture2D ).ToRawData() as T;
					break;
				case SMDataType.Sprite:
					_rawData = ( _cacheData._data as Sprite ).ToRawData() as T;
					break;
				case SMDataType.Audio:
					_rawData = ( _cacheData._data as AudioClip ).ToRawData() as T;
					break;
				case SMDataType.Text:
				case SMDataType.Raw:
					_rawData = _cacheData._data as T;
					break;
			}

			// Dispose時に、元のキャッシュが解放されないよう、リンクを切る
			_cacheData = null;

			await base.Save();
		}
	}
}