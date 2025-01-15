Shader "Custom/AlphaAnimationShaderUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Control ("Control", Range(0,1)) = 0.0 // 外部控制的float值，x值
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 200

        // 开启Alpha混合
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 属性
            sampler2D _MainTex;
            float _Control;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float CalculateAlpha(float x, float b)
            {
                // 计算透明度
                //float alpha =1.0 - log(2.0 * x + b) + (1 - b);
                float slope = lerp(1,10,b);
                float alpha = slope * (x - 1.0) + 1;
                
                // 将alpha限制在0到1之间
                return clamp(alpha, 0.0, 1.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 计算a值
                float brightness = (texColor.r + texColor.g + texColor.b) / 3.0;
                
                // 计算透明度
                float alpha = CalculateAlpha(_Control, brightness);
                
                // 设置alpha值
                texColor.a = alpha;

                return texColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
