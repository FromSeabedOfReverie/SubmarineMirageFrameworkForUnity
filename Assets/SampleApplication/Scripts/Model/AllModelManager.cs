using System;
using System.Collections.Generic;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;



/// <summary>
/// ■ 全モデルの管理クラス
/// </summary>
public class AllModelManager : Singleton<AllModelManager> {
	/// <summary>全モデルの辞書</summary>
	[SMShow] readonly Dictionary<Type, IModel> _models = new Dictionary<Type, IModel>();



#region ToString
	public override void SetToString() {
		base.SetToString();

		_toStringer.SetValue( nameof( _models ), i => _toStringer.DefaultValue( _models, i, true ) );
	}
#endregion



	/// <summary>
	/// ● コンストラクタ
	/// </summary>
	public AllModelManager() {
		var setting = new ModelSetting();
		setting._registerModels.ForEach( m => {
			Register( m.GetType(), m );
		} );
		setting.Dispose();

		_models.ForEach( pair => {
			pair.Value.Initialize( this );
		} );

		_disposables.AddFirst( () => {
			_models.ForEach( pair => pair.Value?.Dispose() );
			_models.Clear();
		} );
	}



	/// <summary>
	/// ● モデルを登録
	///		モデルのTypeを鍵とする。
	/// </summary>
	public T Register<T>( T model ) where T : class, IModel
		=> Register( typeof( T ), model ) as T;

	public IModel Register( Type modelType, IModel model ) {
		CheckDisposeError( nameof( Register ) );

		var key = modelType;

		var last = _models.GetOrDefault( key );
		if ( last != null ) {
			throw new InvalidOperationException( string.Join( "\n",
				$"すでに同型のModelが登録されています。 : ",
				$"{nameof( key )} : {key}",
				$"{nameof( last )} : {last}",
				$"{nameof( model )} : {model}",
				$"{nameof( AllModelManager )}.{nameof( Register )}",
				$"{this}"
			) );
		}

		_models[key] = model;
		return model;
	}

	/// <summary>
	/// ● モデル登録を解除
	///		解除時に、Disposeも行う。
	/// </summary>
	public void Unregister<T>() where T : class, IModel {
		CheckDisposeError( nameof( Unregister ) );

		var key = typeof( T );

		var value = _models.GetOrDefault( key );
		value?.Dispose();

		_models.Remove( key );
	}

	/// <summary>
	/// ● モデルを取得
	/// </summary>
	public T Get<T>() where T : class, IModel {
		CheckDisposeError( nameof( Get ) );

		var key = typeof( T );
		var model = _models.GetOrDefault( key ) as T;

		if ( model == null ) {
			throw new NullReferenceException( string.Join( "\n",
				$"Modelが登録されていません。 : ",
				$"{nameof( key )} : {key}",
				$"{nameof( model )} : {model}",
				$"{nameof( AllModelManager )}.{nameof( Get )}",
				$"{this}"
			) );
		}

		return model;
	}
}