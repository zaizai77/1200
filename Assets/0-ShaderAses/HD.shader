// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "My Shader/HD"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (0.2342444,1.387448,1.04509,1)
		_VertexOffest("VertexOffest", 2D) = "white" {}
		_QD("QD", Float) = 1
		_Mask("Mask", 2D) = "white" {}
		_Vector0("Vector 0", Vector) = (0,0,0,0)
		_OffestRange("OffestRange", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _Mask;
			uniform float4 _Mask_ST;
			uniform float _QD;
			uniform sampler2D _VertexOffest;
			uniform float4 _Vector0;
			uniform float4 _VertexOffest_ST;
			uniform float _OffestRange;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _Color0;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 uv_Mask = v.ase_texcoord.xy * _Mask_ST.xy + _Mask_ST.zw;
				float mulTime18 = _Time.y * _Vector0.z;
				float2 texCoord22 = v.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult16 = (float2(_Vector0.x , _Vector0.y));
				float2 uv_VertexOffest = v.ase_texcoord.xy * _VertexOffest_ST.xy + _VertexOffest_ST.zw;
				float2 panner12 = ( ( mulTime18 + texCoord22.x ) * appendResult16 + uv_VertexOffest);
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = ( v.ase_normal * tex2Dlod( _Mask, float4( uv_Mask, 0, 0.0) ).r * ( _QD * (_OffestRange + (tex2Dlod( _VertexOffest, float4( panner12, 0, 0.0) ).r - 0.0) * (1.0 - _OffestRange) / (1.0 - 0.0)) ) );
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
				clip( ( i.ase_color.a * _Color0.a * tex2DNode1.a ) - 0.2411765);
				
				
				finalColor = ( tex2DNode1 * _Color0 * i.ase_color );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;228.1228,3.456398;Float;False;True;-1;2;ASEMaterialInspector;100;5;My Shader/HD;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-162.6248,277.3122;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;12;-1286.558,375.9187;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;15;-1930.801,397.245;Inherit;False;Property;_Vector0;Vector 0;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;-1,0,1,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;16;-1685.78,427.242;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;18;-1653.575,540.4091;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-1458.917,580.9895;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-1085.221,315.9844;Inherit;True;Property;_VertexOffest;VertexOffest;2;0;Create;True;0;0;0;False;0;False;-1;3ab335ce782803244b3b2147ede15d83;3ab335ce782803244b3b2147ede15d83;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-471.2172,792.5552;Inherit;True;Property;_Mask;Mask;4;0;Create;True;0;0;0;False;0;False;-1;28b3be673aa77d740bb1ac7146396688;28b3be673aa77d740bb1ac7146396688;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-421.4506,558.6904;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;-704.1797,380.8448;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;8;-863.9073,162.9066;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClipNode;4;-207.61,-120.2831;Inherit;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-691.3199,-55.53912;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-715.3015,-319.6985;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1287.794,-167.5754;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;-1;51360d81cd3c7354188d997167bdcc87;51360d81cd3c7354188d997167bdcc87;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;20;-1242.868,-652.1998;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-415.7416,33.28201;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0.2411765;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-1311.499,-415.3606;Inherit;False;Property;_Color0;Color 0;1;1;[HDR];Create;True;0;0;0;False;0;False;0.2342444,1.387448,1.04509,1;1.526061,1.065144,5.216475,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;22;-1762.773,655.1844;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1608.832,243.0474;Inherit;False;0;6;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-821.2212,783.2136;Inherit;False;Property;_QD;QD;3;0;Create;True;0;0;0;False;0;False;1;0.69;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-953.3502,560.9468;Inherit;False;Property;_OffestRange;OffestRange;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
WireConnection;0;0;4;0
WireConnection;0;1;11;0
WireConnection;11;0;8;0
WireConnection;11;1;10;1
WireConnection;11;2;7;0
WireConnection;12;0;14;0
WireConnection;12;2;16;0
WireConnection;12;1;23;0
WireConnection;16;0;15;1
WireConnection;16;1;15;2
WireConnection;18;0;15;3
WireConnection;23;0;18;0
WireConnection;23;1;22;1
WireConnection;6;1;12;0
WireConnection;7;0;9;0
WireConnection;7;1;24;0
WireConnection;24;0;6;1
WireConnection;24;3;25;0
WireConnection;4;0;2;0
WireConnection;4;1;21;0
WireConnection;4;2;5;0
WireConnection;21;0;20;4
WireConnection;21;1;3;4
WireConnection;21;2;1;4
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;2;2;20;0
ASEEND*/
//CHKSM=561F8BAA7771945173E4A49F782675F584C440F6