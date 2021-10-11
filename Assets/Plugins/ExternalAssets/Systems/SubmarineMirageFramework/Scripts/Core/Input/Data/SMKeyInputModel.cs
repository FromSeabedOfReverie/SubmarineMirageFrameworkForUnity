//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System;
	using UniRx;
	///====================================================================================================
	/// <summary>
	/// ■ キー入力のモデルクラス
	/// </summary>
	///====================================================================================================
	public class SMKeyInputModel : BaseSMInputModel<SMInputKey> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>間引き開始秒数</summary>
		const float START_THIN_OUT_SECONDS = 23 / 60f;
		/// <summary>間引き繰り返し秒数</summary>
		const float REPEAT_THIN_OUT_SECONDS = 6 / 60f;


		/// <summary>有効秒数</summary>
		[SMShowLine] float _enableSeconds		{ get; set; }
		/// <summary>次回の繰り返し秒数</summary>
		[SMShowLine] float _nextThinOutSeconds	{ get; set; } = float.PositiveInfinity;

		/// <summary>有効中か？</summary>
		[SMShow] public bool _isEnabling		{ get; private set; }
		/// <summary>有効時か？</summary>
		[SMShow] public bool _isEnabled			{ get; private set; }
		/// <summary>間引き有効中か？</summary>
		[SMShow] public bool _isThinOutEnabling	{ get; private set; }
		/// <summary>無効中か？</summary>
		[SMShow] public bool _isDisabling		{ get; private set; }
		/// <summary>無効時か？</summary>
		[SMShow] public bool _isDisabled		{ get; private set; }

		/// <summary>有効中イベント</summary>
		public readonly SMSubject _enablingEvent = new SMSubject();
		/// <summary>有効時イベント</summary>
		public readonly SMSubject _enabledEvent = new SMSubject();
		/// <summary>間引き有効中イベント</summary>
		public readonly SMSubject _thinOutEnablingEvent = new SMSubject();
		/// <summary>無効中イベント</summary>
		public readonly SMSubject _disablingEvent = new SMSubject();
		/// <summary>無効時イベント</summary>
		public readonly SMSubject _disabledEvent = new SMSubject();

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMKeyInputModel( SMInputKey type, Func<bool> isCheckEnableEvent )
			: base( type )
		{
			var timeManager = SMServiceLocator.Resolve<SMTimeManager>();


			_updateEvent.AddLast().Subscribe( _ => {
				var isLastEnable = _isEnabling;
				_isEnabling = isCheckEnableEvent.Invoke();

				// 押下時間を設定
				// 押下中の場合
				if ( _isEnabling ) {
					// 押上直後の補正
					if ( _enableSeconds < 0 )	{ _enableSeconds = 0; }
					_enableSeconds += timeManager._unscaledDeltaTime;
					// オーバーフロー対策
					if ( _enableSeconds < 0 )	{ _enableSeconds = float.PositiveInfinity; }
					if ( !isLastEnable )		{ _nextThinOutSeconds = 0; }

				// 押上した場合
				} else if ( isLastEnable ) {
					_enableSeconds = -1;
					_nextThinOutSeconds = float.PositiveInfinity;

				// 押上中の場合
				} else {
					_enableSeconds = 0;
					_nextThinOutSeconds = float.PositiveInfinity;
				}

				// 押下中を判定
				_isEnabling = _enableSeconds > 0;
				// 押下直後を判定
				_isEnabled =
					_enableSeconds == timeManager._unscaledDeltaTime &&
					timeManager._unscaledDeltaTime != 0;
				// 押上中を判定
				_isDisabling = _enableSeconds == 0;
				// 押上直後を判定
				_isDisabled = _enableSeconds < 0;

				// 間引き押下中を判定
				_isThinOutEnabling = _nextThinOutSeconds < timeManager._unscaledTime;
				if ( _isThinOutEnabling ) {
					_nextThinOutSeconds =
						(
							_nextThinOutSeconds == 0	? START_THIN_OUT_SECONDS
														: REPEAT_THIN_OUT_SECONDS
						)
						+ timeManager._unscaledTime;
				}

				// イベント実行
				if ( _isEnabling )			{ _enablingEvent.Run(); }
				if ( _isEnabled )			{ _enabledEvent.Run(); }
				if ( _isThinOutEnabling )	{ _thinOutEnablingEvent.Run(); }
				if ( _isDisabling )			{ _disablingEvent.Run(); }
				if ( _isDisabled )			{ _disabledEvent.Run(); }
			} );


			_disposables.AddFirst( () => {
				_enableSeconds = 0;
				_nextThinOutSeconds = 0;

				_isEnabling = false;
				_isEnabled = false;
				_isThinOutEnabling = false;
				_isDisabling = false;
				_isDisabled = false;

				_enablingEvent.Dispose();
				_enabledEvent.Dispose();
				_thinOutEnablingEvent.Dispose();
				_disablingEvent.Dispose();
				_disabledEvent.Dispose();
			} );
		}
	}
}