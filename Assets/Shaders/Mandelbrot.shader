Shader "Explorer/Mandelbrot"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Area ("Area", vector) = (0, 0, 3, 2)
        _Angle ("Angle", range(-3.1415, 3.1415)) = 0
        _MaxIter ("MaxIter", range(4, 1000)) = 255
        _Color ("Color", range(0, 1)) = 0.5
        _Repeat ("Repeat", float) = 5
        _Speed ("Speed", float) = 0
    }

    SubShader
    {
        // No culling or depth
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _Area;
            float _Angle, _MaxIter, _Color, _Repeat, _Speed;
            sampler2D _MainTex;

            float2 rot(float2 p, float2 pivot, float a) {
                float s = sin(a);
                float c = cos(a);

                p -= pivot;
                p = float2(p.x * c - p.y * s, p.x * s + p.y * c);

                p += pivot;

                return p;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;

                float2 c = _Area.xy + uv * _Area.zw;
                c = rot(c, _Area.xy, _Angle);

                float r = 16;
                float2 z;
                float iter;

                for(iter = 0; iter < _MaxIter; iter++) {
                    // standard Mandelbrot
                    z = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y) + c;

                    //z^3 Mandelbrot
                    //z = float2(z.x * z.x * z.x - 3 * z.x * z.y * z.y, 3 * z.x * z.x * z.y - z.y * z.y * z.y) + c;

                    //z^4 Mandelbrot
                    //z = float2(z.x * z.x * z.x * z.x - 6 * z.x * z.x * z.y * z.y + z.y * z.y * z.y * z.y, 4 * z.x * z.x * z.x * z.y - 4 * z.x * z.y *z.y *z.y) + c;
                    if(length(z) > r) {
                        break;
                    }
                }

                if (iter >= _MaxIter) return 0;

                //smoothing coloring
                float dist = length(z);
                float smoothIter;
                smoothIter = log2(log(dist) / log(r));
                iter -= smoothIter;

                float m = sqrt(iter / _MaxIter);
                float4 col;
                col = tex2D(_MainTex, float2(m * _Repeat + _Time.y * _Speed, _Color));

                return col;
            }
            ENDCG
        }
    }

}
