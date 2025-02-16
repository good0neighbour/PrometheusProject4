Shader "Custom/NoiseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Colour ("Colour", Color) = (1,1,1,1)
        _NoiseSpeed ("NoiseSpeed", Float) = 2.34
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        cull back

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            fixed4 _Colour;
            fixed _NoiseSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, fixed2(i.uv.x, i.uv.y + _Time.w * _NoiseSpeed)) * _Colour;
            }
            ENDCG
        }
    }
}
