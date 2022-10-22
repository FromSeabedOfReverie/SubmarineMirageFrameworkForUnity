//---------------------------------------------------------------------------------------------------------
// ▽ Submarine Mirage Framework for Unity
//		Copyright (c) 2020 夢想海の水底より(from Seabed of Reverie)
//		Released under the MIT License :
//			https://github.com/FromSeabedOfReverie/SubmarineMirageFrameworkForUnity/blob/master/LICENSE
//---------------------------------------------------------------------------------------------------------
namespace SubmarineMirageFramework {
	using System;
	using System.IO;
	using System.Text;
	using UnityEngine;
	///====================================================================================================
	/// <summary>
	/// ■ 音の拡張クラス
	/// </summary>
	///====================================================================================================
	public static class AudioClipSMExtension {
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 生情報に変換
		/// </summary>
		///------------------------------------------------------------------------------------------------
		public static SMAudioRawData ToRawData( this AudioClip audio )
			=> new SMAudioRawData( audio );
		///------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● WAV形式に暗号化
		///		参考URL : https://www.jianshu.com/p/aadfb911aa69
		///			該当ソースは、参考元のライセンスが適用されます。
		/// </summary>
		///------------------------------------------------------------------------------------------------

		//	Copyright (c) 2012 Calvin Rien
		//        http://the.darktable.com
		//
		//	This software is provided 'as-is', without any express or implied warranty. In
		//	no event will the authors be held liable for any damages arising from the use
		//	of this software.
		//
		//	Permission is granted to anyone to use this software for any purpose,
		//	including commercial applications, and to alter it and redistribute it freely,
		//	subject to the following restrictions:
		//
		//	1. The origin of this software must not be misrepresented; you must not claim
		//	that you wrote the original software. If you use this software in a product,
		//	an acknowledgment in the product documentation would be appreciated but is not
		//	required.
		//
		//	2. Altered source versions must be plainly marked as such, and must not be
		//	misrepresented as being the original software.
		//
		//	3. This notice may not be removed or altered from any source distribution.
		//
		//  =============================================================================
		//
		//  derived from Gregorio Zanon's script
		//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

		public static byte[] EncodeToWAV( this AudioClip audio ) {
			byte[] bytes = null;

			using ( var memoryStream = new MemoryStream() ) {
				memoryStream.Write( new byte[44], 0, 44 );	// 44文字のヘッド情報を予約

				var bytesData = audio.GetData16();
				memoryStream.Write( bytesData, 0, bytesData.Length );
				memoryStream.Seek( 0, SeekOrigin.Begin );

				var riff = Encoding.UTF8.GetBytes( "RIFF" );
				memoryStream.Write( riff, 0, 4 );

				var chunkSize = BitConverter.GetBytes( memoryStream.Length - 8 );
				memoryStream.Write( chunkSize, 0, 4 );

				var wave = Encoding.UTF8.GetBytes( "WAVE" );
				memoryStream.Write( wave, 0, 4 );

				var fmt = Encoding.UTF8.GetBytes( "fmt " );
				memoryStream.Write( fmt, 0, 4 );

				var subChunk1 = BitConverter.GetBytes( 16 );
				memoryStream.Write( subChunk1, 0, 4 );

				var two = ( ushort )2;
				var one = ( ushort )1;

				var audioFormat = BitConverter.GetBytes( one );
				memoryStream.Write( audioFormat, 0, 2 );

				var numChannels = BitConverter.GetBytes( audio.channels );
				memoryStream.Write( numChannels, 0, 2 );

				var sampleRate = BitConverter.GetBytes( audio.frequency );
				memoryStream.Write( sampleRate, 0, 4 );

				// sampleRate * bytesPerSample * numberOfChannels
				var byteRate = BitConverter.GetBytes( audio.frequency * audio.channels * 2 );
				memoryStream.Write( byteRate, 0, 4 );

				var blockAlign = ( ushort )( audio.channels * 2 );
				memoryStream.Write( BitConverter.GetBytes( blockAlign ), 0, 2 );

				var bps = ( ushort )16;
				var bitsPerSample = BitConverter.GetBytes( bps );
				memoryStream.Write( bitsPerSample, 0, 2 );

				var datastring = Encoding.UTF8.GetBytes( "data" );
				memoryStream.Write( datastring, 0, 4 );

				var subChunk2 = BitConverter.GetBytes( audio.samples * audio.channels * 2 );
				memoryStream.Write( subChunk2, 0, 4 );

				bytes = memoryStream.ToArray();
			}

			return bytes;
		}

		/// <summary>
		/// ● 情報を取得（16bit）
		/// </summary>
		static byte[] GetData16( this AudioClip audio ) {
			var data = new float[audio.samples * audio.channels];
			audio.GetData( data, 0 );

			var bytes = new byte[data.Length * 2];
			var rescaleFactor = 32767;

			for ( var i = 0; i < data.Length; i++ ) {
				var value = ( short )( data[i] * rescaleFactor );
				BitConverter.GetBytes( value ).CopyTo( bytes, i * 2 );
			}

			return bytes;
		}
	}
}