Shader "VSX/UniversalVehicleCombat/Energy Shield" {
    Properties 
	{

		// Buffer of hit positions, can add more
		_EffectPosition0 ("Effect Position 0",Vector) = (0,0,0,1)
        _EffectPosition1 ("Effect Position 1",Vector) = (0,0,0,0)
		_EffectPosition2 ("Effect Position 2",Vector) = (0,0,0,0)
		_EffectPosition3 ("Effect Position 3",Vector) = (0,0,0,0)
		_EffectPosition4 ("Effect Position 4",Vector) = (0,0,0,0)
		_EffectPosition5 ("Effect Position 5",Vector) = (0,0,0,0)
		_EffectPosition6 ("Effect Position 6",Vector) = (0,0,0,0)
		_EffectPosition7 ("Effect Position 7",Vector) = (0,0,0,0)
		_EffectPosition8 ("Effect Position 8",Vector) = (0,0,0,0)
		_EffectPosition9 ("Effect Position 9",Vector) = (0,0,0,0)

		_EffectColor0 ("Effect Color 0", Color) = (1,1,1,1)
		_EffectColor1 ("Effect Color 1", Color) = (1,1,1,1)
		_EffectColor2 ("Effect Color 2", Color) = (1,1,1,1)
		_EffectColor3 ("Effect Color 3", Color) = (1,1,1,1)
		_EffectColor4 ("Effect Color 4", Color) = (1,1,1,1)
		_EffectColor5 ("Effect Color 5", Color) = (1,1,1,1)
		_EffectColor6 ("Effect Color 6", Color) = (1,1,1,1)
		_EffectColor7 ("Effect Color 7", Color) = (1,1,1,1)
		_EffectColor8 ("Effect Color 8", Color) = (1,1,1,1)
		_EffectColor9 ("Effect Color 9", Color) = (1,1,1,1)

		_MeshScale ("Mesh Scale", float) = 1.0
        _SpreadFactor ("Spread Factor", float) = 3
		_EffectMultiplier("Effect Multiplier", float) = 2.0
        _MainTex ("Texture (RGB)", 2D) = "white" {}
    }
    SubShader {

        ZWrite Off
        Tags { "Queue" = "Transparent" }
        Blend One One
        Cull Off

        Pass { 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_fog_exp2

            #include "UnityCG.cginc"

            struct v2f 
			{
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD2;
                float effectStrength:COLOR1;
				float4 effectColor:COLOR;
            };

            uniform float _SpreadFactor;
			uniform float _MeshScale;
			uniform float _EffectMultiplier;

			uniform float4 _EffectPosition0;
            uniform float4 _EffectPosition1;
			uniform float4 _EffectPosition2;
			uniform float4 _EffectPosition3;
			uniform float4 _EffectPosition4;
			uniform float4 _EffectPosition5;
			uniform float4 _EffectPosition6;
			uniform float4 _EffectPosition7;
			uniform float4 _EffectPosition8;
			uniform float4 _EffectPosition9;

			uniform float4 _EffectColor0;
            uniform float4 _EffectColor1;
			uniform float4 _EffectColor2;
			uniform float4 _EffectColor3;
			uniform float4 _EffectColor4;
			uniform float4 _EffectColor5;
			uniform float4 _EffectColor6;
			uniform float4 _EffectColor7;
			uniform float4 _EffectColor8;
			uniform float4 _EffectColor9;
			
            sampler2D _MainTex;
			float4 _MainTex_ST;
            
			// Get the glow effect as an exponential function of distance from impact point
			float GetEffectValue(float strength, float _dist)
			{
				return (strength*(1/(1+pow(3, 5*(_dist-0.5)))));
			} 

            v2f vert (appdata_full v) 
			{

                v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
               	o.normal = v.normal;

				float4 effectColor = float4(0,0,0,0);
				float totalEffectStrength = 0.0001f;
				o.effectStrength = 0;
				o.effectColor = float4(0,0,0,0);

				// For each vertex, get the strongest (non-additive) effect value from the buffered hit positions
				float dist = (distance (v.vertex.xyz, _EffectPosition0.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength0 = GetEffectValue(_EffectPosition0.w, dist);
				totalEffectStrength += effectStrength0;
				o.effectStrength = max(effectStrength0, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition1.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength1 = GetEffectValue(_EffectPosition1.w, dist);
				totalEffectStrength += effectStrength1;
				o.effectStrength = max(effectStrength1, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition2.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength2 = GetEffectValue(_EffectPosition2.w, dist);
				totalEffectStrength += effectStrength2;
				o.effectStrength = max(effectStrength2, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition3.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength3 = GetEffectValue(_EffectPosition3.w, dist);
				totalEffectStrength += effectStrength3;
				o.effectStrength = max(effectStrength3, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition4.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength4 = GetEffectValue(_EffectPosition4.w, dist);
				totalEffectStrength += effectStrength4;
				o.effectStrength = max(effectStrength4, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition5.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength5 = GetEffectValue(_EffectPosition5.w, dist);
				totalEffectStrength += effectStrength5;
				o.effectStrength = max(effectStrength5, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition6.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength6 = GetEffectValue(_EffectPosition6.w, dist);
				totalEffectStrength += effectStrength6;
				o.effectStrength = max(effectStrength6, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition7.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength7 = GetEffectValue(_EffectPosition7.w, dist);
				totalEffectStrength += effectStrength7;
				o.effectStrength = max(effectStrength7, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition8.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength8 = GetEffectValue(_EffectPosition8.w, dist);
				totalEffectStrength += effectStrength8;
				o.effectStrength = max(effectStrength8, o.effectStrength);

				dist = (distance (v.vertex.xyz, _EffectPosition9.xyz) * _MeshScale)/(_SpreadFactor * _MeshScale);
				float effectStrength9 = GetEffectValue(_EffectPosition9.w, dist);
				totalEffectStrength += effectStrength9;
				o.effectStrength = max(effectStrength9, o.effectStrength);

				// Update the color

				o.effectColor += (effectStrength0 / totalEffectStrength) * _EffectColor0;
				o.effectColor += (effectStrength1 / totalEffectStrength) * _EffectColor1;
				o.effectColor += (effectStrength2 / totalEffectStrength) * _EffectColor2;
				o.effectColor += (effectStrength3 / totalEffectStrength) * _EffectColor3;
				o.effectColor += (effectStrength4 / totalEffectStrength) * _EffectColor4;
				o.effectColor += (effectStrength5 / totalEffectStrength) * _EffectColor5;
				o.effectColor += (effectStrength6 / totalEffectStrength) * _EffectColor6;
				o.effectColor += (effectStrength7 / totalEffectStrength) * _EffectColor7;
				o.effectColor += (effectStrength8 / totalEffectStrength) * _EffectColor8;
				o.effectColor += (effectStrength9 / totalEffectStrength) * _EffectColor9;

				
                return o;

            }

            half4 frag (v2f o) : COLOR
            {

				// Get the texture value
				half4 tex = tex2D (_MainTex, o.uv) * o.effectColor;
				
                return tex * _EffectMultiplier;
            }

            ENDCG
        }
    }
    Fallback "Transparent/VertexLit"
}
