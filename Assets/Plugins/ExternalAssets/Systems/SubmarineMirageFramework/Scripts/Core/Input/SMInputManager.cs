//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirage {
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UniRx;
	using KoganeUnityLib;
	///====================================================================================================
	/// <summary>
	/// ■ 入力の管理クラス
	/// </summary>
	///====================================================================================================
	public class SMInputManager : SMUnityName<SMInputName> {
		///------------------------------------------------------------------------------------------------
		/// ● 要素
		///------------------------------------------------------------------------------------------------
		/// <summary>実行型</summary>
		[SMShowLine] public override SMTaskRunType _type => SMTaskRunType.Sequential;

		SMTimeManager _timeManager	{ get; set; }

		/// <summary>不動範囲の2乗</summary>
		[SMShow] public float _noMoveSqrRange	{ get; private set; }
		/// <summary>スワイプ範囲の2乗</summary>
		[SMShow] public float _swipeSqrRange	{ get; private set; }

		/// <summary>軸の一覧</summary>
		[SMShow] readonly Dictionary<SMInputAxis, SMAxisInputModel> _axisDatas
			= new Dictionary<SMInputAxis, SMAxisInputModel>();
		/// <summary>キー押下秒数の一覧</summary>
		[SMShow] readonly Dictionary<SMInputKey, SMKeyInputModel> _keyDatas
			= new Dictionary<SMInputKey, SMKeyInputModel>();
		/// <summary>スワイプ状態</summary>
		[SMShow] public SMInputSwipe _swipe	{ get; private set; } = SMInputSwipe.None;
		/// <summary>スワイプ時イベントの一覧</summary>
		[SMShow] readonly Dictionary<SMInputSwipe, SMSwipeInputModel> _swipeDatas
			= new Dictionary<SMInputSwipe, SMSwipeInputModel>();

#region ToString
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 文章変換を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public override void SetToString() {
			base.SetToString();

			_toStringer.SetValue( nameof( _axisDatas ),		i => _toStringer.DefaultValue( _axisDatas, i, true ) );
			_toStringer.SetValue( nameof( _keyDatas ),		i => _toStringer.DefaultValue( _keyDatas, i, true ) );
			_toStringer.SetValue( nameof( _swipeDatas ),	i => _toStringer.DefaultValue( _swipeDatas, i, true ) );
		}
#endregion

		///------------------------------------------------------------------------------------------------
		/// ● 生成、削除
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		public SMInputManager() {
			_disposables.AddFirst( () => {
				_axisDatas.ForEach( pair => pair.Value.Dispose() );
				_axisDatas.Clear();
				_keyDatas.ForEach( pair => pair.Value.Dispose() );
				_keyDatas.Clear();
				_swipe = SMInputSwipe.None;
				_swipeDatas.ForEach( pair => pair.Value.Dispose() );
				_swipeDatas.Clear();

				_timeManager = null;
			} );
		}

		/// <summary>
		/// ● 作成
		/// </summary>
		public override void Create() {
			_timeManager = SMServiceLocator.Resolve<SMTimeManager>();

			SetAxis();
			SetKey();
			SetMoveRange();
			SetPressNothing();
			SetSwipe();
			SetAnyOperation();
			SetDebug();

			var setting = SMServiceLocator.Resolve<BaseSMInputSetting>();
			setting.Setup( this );
			SMServiceLocator.Unregister<BaseSMInputSetting>();

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
// TODO : 本当は、配列複製とかしたくない・・・変更対応の構造を考える
				foreach ( var data in _axisDatas.Values.ToArray() ) {
					if ( _isDispose )	{ return; }
					data._updateEvent.Run();
				}
				foreach ( var data in _keyDatas.Values.ToArray() ) {
					if ( _isDispose )	{ return; }
					data._updateEvent.Run();
				}
				foreach ( var data in _swipeDatas.Values.ToArray() ) {
					if ( _isDispose )	{ return; }
					data._updateEvent.Run();
				}
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// ● 取得
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 軸を取得
		/// </summary>
		public Vector2 GetAxis( SMInputAxis axis )
			=> _axisDatas[axis]._axis;

		/// <summary>
		/// ● キーを取得
		/// </summary>
		public SMKeyInputModel GetKey( SMInputKey key )
			=> _keyDatas[key];

		/// <summary>
		/// ● スワイプを取得
		/// </summary>
		public SMSwipeInputModel GetSwipe( SMInputSwipe swipe )
			=> _swipeDatas[swipe];

		/// <summary>
		/// ● 軸を取得
		/// </summary>
		Vector2 GetAxis( SMInputName xType, SMInputName yType ) {
			var axis = new Vector2(
				Input.GetAxis( Get( xType ) ),
				Input.GetAxis( Get( yType ) )
			);
			if ( axis.magnitude > 1 )	{ axis.Normalize(); }
			return axis;
		}

		///------------------------------------------------------------------------------------------------
		/// ● 判定
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● ボタンが有効か？
		/// </summary>
		bool IsEnableButton( SMInputName type )
			=> Input.GetButton( Get( type ) );

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 軸入力を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetAxis() {
			_axisDatas[SMInputAxis.Move] = new SMAxisInputModel(
				SMInputAxis.Move,
				() => GetAxis( SMInputName.MoveAxisX, SMInputName.MoveAxisY )
			);
			_axisDatas[SMInputAxis.Rotate] = new SMAxisInputModel(
				SMInputAxis.Rotate,
				() => GetAxis( SMInputName.RotateAxisX, SMInputName.RotateAxisY )
			);
			_axisDatas[SMInputAxis.Mouse] = new SMAxisInputModel(
				SMInputAxis.Mouse,
				() => Input.mousePosition
			);
			if ( SMDebugManager.IS_DEVELOP ) {
				_axisDatas[SMInputAxis.Debug] = new SMAxisInputModel(
					SMInputAxis.Debug,
					() => GetAxis( SMInputName.DebugAxisX, SMInputName.DebugAxisY )
				);
			}
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● キー入力を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetKey() {
			_keyDatas[SMInputKey.Decide] = new SMKeyInputModel(
				SMInputKey.Decide,
				() => IsEnableButton( SMInputName.Decide )
			);
			_keyDatas[SMInputKey.Quit] = new SMKeyInputModel(
				SMInputKey.Quit,
				() => IsEnableButton( SMInputName.Quit )
			);
			_keyDatas[SMInputKey.Reset] = new SMKeyInputModel(
				SMInputKey.Reset,
				() => IsEnableButton( SMInputName.Reset )
			);
			_keyDatas[SMInputKey.Finger1] = new SMKeyInputModel(
				SMInputKey.Finger1,
				() => Input.GetMouseButton( 0 )
			);
			_keyDatas[SMInputKey.Finger2] = new SMKeyInputModel(
				SMInputKey.Finger2,
				() => Input.GetMouseButton( 1 )
			);
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 移動範囲を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetMoveRange() {
			// 画面大きさのキャッシュ
			var lastSize = Vector2.zero;

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				var size = new Vector2( Screen.width, Screen.height );
				if ( size == lastSize )	{ return; }
				lastSize = size;

				// 画面の大きさから、各種範囲を設定
				_noMoveSqrRange = ( size / 100 ).sqrMagnitude;
				_swipeSqrRange = ( size / 10 ).sqrMagnitude;
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 無の押下を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetPressNothing() {
			// 無の押下を設定
			_keyDatas[SMInputKey.Nothing] = new SMKeyInputModel(
				SMInputKey.Nothing,
				() => {
					// 決定キーが未押下の場合
					if ( !GetKey( SMInputKey.Decide )._isEnabling )	{ return false; }
					// イベントシステムが存在しない場合
					if ( EventSystem.current == null )				{ return true; }

					// イベントシステムから光線を飛ばす
					var pointer = new PointerEventData( EventSystem.current ) {
						position = GetAxis( SMInputAxis.Mouse )
					};
					var result = new List<RaycastResult>();
					EventSystem.current.RaycastAll( pointer, result );
					// UIに触れていないかで判定
					return result.IsEmpty();
				}
			);


			// 無の長押下を設定
			const float LONG_NOTHING_SECOND = 1;
			var clickPosition = Vector2.zero;
			var seconds = 0f;

			_keyDatas[SMInputKey.LongNothing] = new SMKeyInputModel(
				SMInputKey.LongNothing,
				() => {
					var key = GetKey( SMInputKey.Nothing );

					// 押下時にマウス位置を設定
					if ( key._isEnabled ) {
						clickPosition = GetAxis( SMInputAxis.Mouse );
					}

					// 無を押下中で、指不動の場合
					if (	key._isEnabling &&
							clickPosition != Vector2.zero &&
							( GetAxis( SMInputAxis.Mouse ) - clickPosition ).sqrMagnitude < _noMoveSqrRange
					) {
						seconds = Mathf.Min( seconds + _timeManager._unscaledDeltaTime, LONG_NOTHING_SECOND );
					} else {
						seconds = 0;
						clickPosition = Vector2.zero;
					}

					// 蓄積秒から、長押し判定
					return seconds >= LONG_NOTHING_SECOND;
				}
			);
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● スワイプを設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetSwipe() {
			// スワイプ開始時の位置を設定
			const float CHECK_CLICK_SECOND = 1;
			var clickPosition = Vector2.zero;
			var seconds = 0f;
			var lastSwipe = _swipe;

			// ● 更新
			_updateEvent.AddLast().Subscribe( _ => {
				lastSwipe = _swipe;
				var key = GetKey( SMInputKey.Finger1 );

				// 押下時に、押下位置を設定
				if ( key._isEnabled ) {
					clickPosition = GetAxis( SMInputAxis.Mouse );
				}
				seconds = (
					key._isEnabling ? Mathf.Min( seconds + _timeManager._unscaledDeltaTime, CHECK_CLICK_SECOND )
									: 0
				);
				if ( seconds >= CHECK_CLICK_SECOND ) {
					clickPosition = Vector2.zero;
				}

				// マウス押下中で一定以上移動した場合、移動差を返す
				if ( key._isEnabling && clickPosition != Vector2.zero ) {
					var delta = GetAxis( SMInputAxis.Mouse ) - clickPosition;
					if ( delta.sqrMagnitude > _swipeSqrRange ) {
						clickPosition = Vector2.zero;

						// 移動差から角度を求め、0～360度に補正する
						var angle = Mathf.Atan2( delta.x, delta.y ) * Mathf.Rad2Deg;
						if ( angle < 0 )	{ angle += 360; }

						// スワイプ方向を求める
						if		(	( 337.5	< angle && angle <= 360 ) ||
									( 0		<= angle && angle < 22.5 ) )	{ _swipe = SMInputSwipe.Up; }
						else if		( 22.5	< angle && angle < 67.5 )		{ _swipe = SMInputSwipe.UpperRight; }
						else if		( 67.5	< angle && angle < 112.5 )		{ _swipe = SMInputSwipe.Right; }
						else if		( 112.5	< angle && angle < 157.5 )		{ _swipe = SMInputSwipe.DownerRight; }
						else if		( 157.5	< angle && angle < 202.5 )		{ _swipe = SMInputSwipe.Down; }
						else if		( 202.5	< angle && angle < 247.5 )		{ _swipe = SMInputSwipe.DownerLeft; }
						else if		( 247.5	< angle && angle < 292.5 )		{ _swipe = SMInputSwipe.Left; }
						else if		( 292.5	< angle && angle < 337.5 )		{ _swipe = SMInputSwipe.UpperLeft; }
						return;
					}
				}

				// 無移動の場合
				_swipe = SMInputSwipe.None;
			} );


			// 各種スワイプ情報を設定
			EnumUtils.GetValues<SMInputSwipe>().ForEach( e => {
				_swipeDatas[e] = new SMSwipeInputModel(
					e,
					() => e == _swipe && ( _swipe != SMInputSwipe.None || _swipe != lastSwipe )
				);
			} );
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 何らかの操作を設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetAnyOperation() {
			// 何らかの操作をしたか判定
			_keyDatas[SMInputKey.AnyOperation] = new SMKeyInputModel(
				SMInputKey.AnyOperation,
				() => (
					// マウス以外で、軸入力があるか？
					_axisDatas
						.Select( pair => pair.Value )
						.Where( data => data._type != SMInputAxis.Mouse )
						.Any( data => data._axis != Vector2.zero ) ||
					// 何らかの操作以外で、キー入力があるか？
					_keyDatas
						.Select( pair => pair.Value )
						.Where( data => data._type != SMInputKey.AnyOperation )
						.Any( data => data._isEnabling ) ||
					// スワイプしたか？
					_swipe != SMInputSwipe.None
				)
			);
		}

		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● デバッグを設定
		/// </summary>
		///------------------------------------------------------------------------------------------------
		void SetDebug() {
			if ( !SMDebugManager.IS_DEVELOP )	{ return; }

			// デバッグキーを設定
			_keyDatas[SMInputKey.Debug] = new SMKeyInputModel(
				SMInputKey.Debug,
				() => {
					// デバッグキーが押されている場合
					if ( Input.GetButton( Get( SMInputName.Debug ) ) ) {
						return true;
					}

					// マウス左右キー押下中か、2本指でタッチ中で、上スワイプした場合
					var isMouse = (
						GetKey( SMInputKey.Finger1 )._isEnabling &&
						GetKey( SMInputKey.Finger2 )._isEnabling
					);
					var isTouch = Input.touchCount >= 2;
					return ( isMouse || isTouch ) && GetSwipe( SMInputSwipe.Up )._isEnabled;
				}
			);


			// デバッグキー押下の場合、画面ログ表示を切り替え
			var displayLog = SMServiceLocator.Resolve<SMDisplayLog>();

			GetKey( SMInputKey.Debug )._enabledEvent.AddLast().Subscribe( _ => {
				displayLog._isDraw = !displayLog._isDraw;
			} );
		}
	}
}