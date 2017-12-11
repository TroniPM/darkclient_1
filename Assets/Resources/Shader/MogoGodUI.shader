Shader "Mogo/GodUI"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent+100"
			"IgnoreProjector" = "True"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite On
			ZTest Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			//AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}