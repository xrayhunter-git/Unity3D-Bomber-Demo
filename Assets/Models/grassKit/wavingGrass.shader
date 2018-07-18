Shader "grassKit/wavingGrass"
{
	Properties
	{
		_Color  ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB), Alpha(A)", 2D) = "white" {}
		_Cutoff ("Shadows Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		_BendByView("Bend By View", Float) = 0.5
		[Space]
		_GrassBottom("Grass Bottom(Y)", Float) = 0.0
		_GrassTop   ("Grass Top(Y)",    Float) = 1.0
		[Space]
		_WaveSpeed        ("Waves Speed", Float) = 2.0
		_WaveFrequency    ("Waves Frequency", Float) = 20.0
		_WaveAndDistortion("Waves Distortion", Float) = 0.75
		[Space]
		_WavingTintLighten("Waving Tint Lighten", Color) = (0.5, 0.5, 0.5, 0.5)
		_WavingTintDarken ("Waving Tint Darken",  Color) = (0.5, 0.5, 0.5, 0.5)
	}
	SubShader
	{
		Pass
		{
			Name "Caster"
			Tags{ "LightMode" = "ShadowCaster" }
			Cull off

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"

struct v2f
{
	V2F_SHADOW_CASTER;
	float2  uv : TEXCOORD1;
};

uniform float4 _MainTex_ST;
v2f vert(appdata_base v)
{
	v2f o;
	TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
	o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
return o;
}

uniform sampler2D _MainTex;
uniform fixed _Cutoff;
uniform fixed4 _Color;
float4 frag(v2f i) : SV_Target
{
	fixed4 texcol = tex2D(_MainTex, i.uv);
	clip(texcol.a*_Color.a - _Cutoff);

	SHADOW_CASTER_FRAGMENT(i)
}
ENDCG
		}

		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull off
		LOD 200
		
CGPROGRAM
#pragma surface surf PassThrough alpha vertex:vert noforwardadd
#pragma target 3.0
#include "TerrainEngine.cginc"

uniform sampler2D _MainTex;
uniform fixed4 _Color;
uniform half _BendByView;

uniform half _GrassBottom;
uniform half _GrassTop;

uniform half _WaveSpeed;
uniform half _WaveFrequency;
uniform half _WaveAndDistortion;
uniform fixed4 _WavingTintLighten;
uniform fixed4 _WavingTintDarken;

struct Input
{
	float2 uv_MainTex;
	fixed4 gColor;
};


fixed4 TerrainWaveGrass_Mod (inout float4 vertex, float waveAmount, fixed4 color)
{
	float4 _waveXSize = float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y;
	float4 _waveZSize = float4(0.006, 0.02, 0.02, 0.05)  * _WaveAndDistance.y;
	float4  waveSpeed = float4(0.3, 0.5, 0.4, 1.2) * 4.0;

	float4 _waveXmove = float4(0.012, 0.02, -0.06, 0.048) * 2.0;
	float4 _waveZmove = float4(0.006, 0.02, -0.02, 0.1);

	float4 waves;
	waves = vertex.x * _waveXSize;
	waves += vertex.z * _waveZSize;

	// Add in time to model them over time
	waves += _WaveAndDistance.x * waveSpeed;

	float4 s, c;
	waves = frac(waves);
	FastSinCos(waves, s,c);

	s = s * s;
	s = s * s;

	float lighting = dot(s, normalize(float4 (1.0, 1.0, 0.4, 0.2))) * 0.7;

	s = s * waveAmount;

	float3 waveMove = float3(0.0, 0.0, 0.0);
	waveMove.x = dot (s, _waveXmove);
	waveMove.z = dot (s, _waveZmove);

	vertex.xz -= waveMove.xz * _WaveAndDistance.z;
	
	fixed3 waveColor = lerp(_WavingTintDarken.rgb, _WavingTintLighten.rgb, lighting);
	
return fixed4(2.0 * waveColor * color.rgb, color.a);
}

void vert(inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	
	float4 wPos = mul(unity_ObjectToWorld, v.vertex);

	float grassHeight = (v.vertex.y + _GrassBottom) / _GrassTop;
	grassHeight = saturate(grassHeight);

	float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
	float  fresnel = viewDir.y;

	viewDir.y = 0.0;
	viewDir = normalize(viewDir);
	v.vertex.xz = v.vertex.xz - viewDir.xz * _BendByView * fresnel * grassHeight;

	_WaveAndDistance = float4(_Time.x * _WaveSpeed, _WaveFrequency, _WaveAndDistortion, 1.0);
	float waveAmount = grassHeight * _WaveAndDistance.z;

	o.gColor = TerrainWaveGrass_Mod(v.vertex, waveAmount, v.color);
}

inline fixed4 LightingPassThrough(SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed3 diff = abs(dot(s.Normal, lightDir)) * 0.5 + 0.5;

	fixed4 c = fixed4(0.0, 0.0, 0.0, 0.0);
	c.rgb = s.Albedo * _LightColor0.rgb * diff * atten;
	c.a = s.Alpha;
return c;
}

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * IN.gColor.rgb;
	o.Alpha = c.a * IN.gColor.a;
}
ENDCG

	}
FallBack "Legacy Shaders/Transparent/Diffuse"
}