Shader "Custom/CurvedUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurveStrength ("Curve Strength", Range(-5, 5)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _CurveStrength;

            v2f vert (appdata_t v)
            {
                v2f o;

                // Calculate curvature effect
                float curveAmount = _CurveStrength * (v.uv.x - 0.5) * (v.uv.x - 0.5); // Exponential curve
                v.vertex.y -= curveAmount * 2.0; // Strengthen the curve in Y-axis

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
