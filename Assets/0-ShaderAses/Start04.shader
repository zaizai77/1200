// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "My Shader/Star04"
{
	Properties
	{
		[HDR]_EdgeColor("EdgeColor", Color) = (1.314039,0.4016495,0.4016495,1)
		_TextureSample2("Texture Sample 0", 2D) = "white" {}
		_Vector2("Vector 0", Vector) = (0,0,0,0)
		_EdgeWidth("EdgeWidth", Range( 0 , 1)) = 0.1377324

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
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


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _TextureSample2;
			uniform float4 _Vector2;
			uniform float _EdgeWidth;
			uniform float4 _EdgeColor;
			inline float4 ASE_ComputeGrabScreenPos( float4 pos )
			{
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				float4 o = pos;
				o.y = pos.w * 0.5f;
				o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
				return o;
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				
				o.ase_texcoord2.xy = v.ase_texcoord2.xy;
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
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
				float2 appendResult66 = (float2(_Vector2.z , _Vector2.w));
				float4 screenPos = i.ase_texcoord1;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float2 appendResult63 = (float2(ase_grabScreenPosNorm.r , ase_grabScreenPosNorm.g));
				float2 appendResult64 = (float2(_Vector2.x , _Vector2.y));
				float2 panner69 = ( _Time.y * appendResult66 + ( appendResult63 * appendResult64 ));
				float2 texCoord28 = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_26_0 = ( 1.0 - texCoord28.x );
				float2 texCoord2 = i.ase_texcoord2.zw * float2( 1,1 ) + float2( 0,0 );
				float blendOpSrc1 = texCoord2.x;
				float blendOpDest1 = texCoord2.y;
				float temp_output_1_0 = ( saturate( abs( blendOpSrc1 - blendOpDest1 ) ));
				float blendOpSrc7 = texCoord2.x;
				float blendOpDest7 = ( 1.0 - texCoord2.y );
				float temp_output_7_0 = ( saturate( abs( blendOpSrc7 - blendOpDest7 ) ));
				float blendOpSrc10 = temp_output_1_0;
				float blendOpDest10 = temp_output_7_0;
				float temp_output_10_0 = ( saturate( ( blendOpDest10/ max( 1.0 - blendOpSrc10, 0.00001 ) ) ));
				float blendOpSrc11 = temp_output_7_0;
				float blendOpDest11 = temp_output_1_0;
				float temp_output_11_0 = ( saturate( ( blendOpDest11/ max( 1.0 - blendOpSrc11, 0.00001 ) ) ));
				float temp_output_53_0 = ( 1.0 - ( step( temp_output_26_0 , temp_output_10_0 ) * step( temp_output_26_0 , temp_output_11_0 ) ) );
				float temp_output_57_0 = ( temp_output_26_0 + _EdgeWidth );
				
				
				finalColor = ( ( tex2D( _TextureSample2, panner69 ) * temp_output_53_0 ) + ( ( ( 1.0 - ( step( temp_output_57_0 , temp_output_11_0 ) * step( temp_output_57_0 , temp_output_10_0 ) ) ) - temp_output_53_0 ) * _EdgeColor ) );
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
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;392.5673,38.03752;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;663.5482,37.50256;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;26;-388.329,646.6688;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;50;129.2111,22.12063;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;54;156.5543,643.42;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;429.0397,638.2856;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;58;682.6804,636.03;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;7;-855.7239,273.632;Inherit;True;Difference;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;1;-862.0574,11.97673;Inherit;True;Difference;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-147.1934,669.2743;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;51;129.0358,287.1837;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;10;-502.6527,15.45533;Inherit;True;ColorDodge;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;11;-503.9526,270.9556;Inherit;True;ColorDodge;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;9;-1077.408,262.3645;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1912.926,357.8841;Float;False;True;-1;2;ASEMaterialInspector;100;5;My Shader/Star04;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;3;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;=;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;1431.918,587.5595;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;23;1070.738,707.3423;Inherit;False;Property;_EdgeColor;EdgeColor;1;1;[HDR];Create;True;0;0;0;False;0;False;1.314039,0.4016495,0.4016495,1;6.65098,2.572549,2.478431,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;61;-538.2445,-670.8342;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-124.9299,-474.1125;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;63;-303.6504,-626.4263;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-290.306,-426.5356;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;65;-519.0513,-453.5435;Inherit;False;Property;_Vector2;Vector 0;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,2,2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;66;-167.7833,-300.5945;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;67;-48.52839,-169.1345;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;68;495.5253,-326.0645;Inherit;True;Property;_TextureSample2;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;7637f87eac7abc64691cb36960454e01;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;69;189.9574,-389.6365;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;56;153.1659,884.3256;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;1610.873,313.0551;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;1296.953,50.48485;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;60;1032.562,293.4676;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-695.6461,642.6833;Inherit;False;Property;_Float1;Float 1;0;0;Create;True;0;0;0;False;0;False;0.6879752;0.706;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-654.4799,498.3147;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;-489.9828,746.4163;Inherit;False;Property;_EdgeWidth;EdgeWidth;4;0;Create;True;0;0;0;False;0;False;0.1377324;0.267;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1342.676,8.037468;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;53;0;52;0
WireConnection;26;0;28;1
WireConnection;50;0;26;0
WireConnection;50;1;10;0
WireConnection;54;0;57;0
WireConnection;54;1;11;0
WireConnection;55;0;54;0
WireConnection;55;1;56;0
WireConnection;58;0;55;0
WireConnection;7;0;2;1
WireConnection;7;1;9;0
WireConnection;1;0;2;1
WireConnection;1;1;2;2
WireConnection;57;0;26;0
WireConnection;57;1;59;0
WireConnection;51;0;26;0
WireConnection;51;1;11;0
WireConnection;10;0;1;0
WireConnection;10;1;7;0
WireConnection;11;0;7;0
WireConnection;11;1;1;0
WireConnection;9;0;2;2
WireConnection;0;0;71;0
WireConnection;21;0;60;0
WireConnection;21;1;23;0
WireConnection;62;0;63;0
WireConnection;62;1;64;0
WireConnection;63;0;61;1
WireConnection;63;1;61;2
WireConnection;64;0;65;1
WireConnection;64;1;65;2
WireConnection;66;0;65;3
WireConnection;66;1;65;4
WireConnection;68;1;69;0
WireConnection;69;0;62;0
WireConnection;69;2;66;0
WireConnection;69;1;67;0
WireConnection;56;0;57;0
WireConnection;56;1;10;0
WireConnection;71;0;70;0
WireConnection;71;1;21;0
WireConnection;70;0;68;0
WireConnection;70;1;53;0
WireConnection;60;0;58;0
WireConnection;60;1;53;0
ASEEND*/
//CHKSM=EC727CA3759266FC1160BCBBF755862DF9E4E970