using System;



/// <summary>
/// ■ モデルのインターフェース
/// </summary>
public interface IModel : IDisposable {
	void Initialize( AllModelManager manager );
}