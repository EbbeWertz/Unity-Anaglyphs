Shader "Custom/Anaglyph"
{
    Properties
    {
        _MainTex ("Left Eye", 2D) = "white" {}
        _MainTex2("Right Eye", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;   // linkerbeeld
            sampler2D _MainTex2;  // rechterbeeld

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 left = tex2D(_MainTex, i.uv).rgb;
                fixed3 right = tex2D(_MainTex2, i.uv).rgb;

                // Combineer rood van links, groen+blauw van rechts
                fixed3 combined = fixed3(left.r, right.g, right.b);

                return fixed4(combined, 1);
            }
            ENDCG
        }
    }
}
