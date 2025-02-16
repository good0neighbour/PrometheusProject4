Shader "Custom/PlanetShader"
{
    Properties
    {
        _SurfTex ("Planet Surface Texture", 2D) = "white" {}
        [Space(20)]
        [Header(Land)]
        _LandCol ("Land Colour", Color) = (1,1,1,1)
        _GrassCol ("Grass Colour", Color) = (1,1,1,1)
        _GrassPow ("Grass Power", Range(0,20)) = 2
        _GrassCont ("Grass Contrast", Range(0,20)) = 2
        _GrassLati ("Grass Latitude Affection", Range(0,1)) = 0.5
        _Grass ("Grass Amount", Range(0,1)) = 0.5
        [Space(20)]
        [Header(Water)]
        _WaterCol ("Water Colour", Color) = (1,1,1,1)
        _WaterSpeCol ("Water Specular Colour", Color) = (1,1,1,1)
        _WaterSpeSprd ("Water Specular Spread", Range(100,1)) = 30
        _Water ("Water Amount", Range(0,1)) = 0
        [Space(20)]
        [Header(Ice)]
        _IceNor ("Ice Normalmap", 2D) = "white" {}
        _Ice ("Ice Amount", Range(0,1)) = 0
        [Space(20)]
        [Header(Atmosphere)]
        [HDR] _AtmoCol ("Atmosphere Colour", Color) = (1,1,1,1)
        _AtmoPow ("Atmosphere Power", Range(1,10)) = 5
        _Atmosphere ("Atmosphere Amount", Range(0,1)) = 1
        [Space(20)]
        [Header(Cloud)]
        _CloudTex ("Cloud Texture", 2D) = "white" {}
        _OverCloudyWhite ("Over Cloudy White Balance", Range(1,0)) = 0.7
        _OverCloudyBlue ("Over Cloudy Blue Balance", Range(1,0)) = 0.5
        _Cloud ("Cloud Amount", Range(0,2)) = 1
        [Space(20)]
        [Header(Night View)]
        [HDR] _NightCol ("Night Colour", Color) = (1,1,1,1)
        _NightSmooth ("Night Smoothness", Range(0,20)) = 10
        _NightPos ("Night Edge Position", Range(-10,10)) = 0
        _Night ("Night Lighting Amount", Range(0,1)) = 1
        [Space(20)]
        [Header(Planet Turning)]
        _SurfRotation ("Surface Rotation Multiply", Range(0,-1)) = -0.1
        _CloudRotation ("Cloud Rotation Multiply", Range(0,-1)) = -0.025
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        cull back

        CGPROGRAM
        #pragma surface surf PlanetSurface noambient noshadow noforwardadd nolightmap
        #pragma target 3.0


        sampler2D _SurfTex;
        sampler2D _IceNor;
        sampler2D _CloudTex;


        struct Input
        {
            float2 uv_SurfTex;
            float2 uv_CloudTex;
        };


        fixed4 _LandCol;
        fixed4 _GrassCol;
        fixed4 _NightCol;
        fixed4 _WaterCol;
        fixed4 _WaterSpeCol;
        fixed4 _AtmoCol;
        fixed _GrassPow;
        fixed _GrassCont;
        fixed _GrassLati;
        fixed _Grass;
        fixed _Water;
        fixed _WaterSpeSprd;
        fixed _Ice;
        fixed _Atmosphere;
        fixed _AtmoPow;
        fixed _Cloud;
        fixed _OverCloudyWhite;
        fixed _OverCloudyBlue;
        fixed _NightSmooth;
        fixed _NightPos;
        fixed _Night;
        fixed _SurfRotation;
        fixed _CloudRotation;


        struct PlanetSurfaceOutput
        {
            fixed4 Albedo;
            fixed4 SurfaceData;
            fixed3 Normal;
            fixed3 Emission;
            fixed Alpha;
        };


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf (Input IN, inout PlanetSurfaceOutput o)
        {
            // Planet surface texture and counterclockwise turning
            fixed4 c = tex2D(
                _SurfTex,
                fixed2(
                    IN.uv_SurfTex.x + _Time.x * _SurfRotation,
                    IN.uv_SurfTex.y
                )
            );

            // Ice
            o.SurfaceData.y = step(
                1 - _Ice,
                (c.g + abs(IN.uv_SurfTex.y - 0.5) * 6) * 0.33
            );

            // Water
            o.SurfaceData.z = saturate(step(c.g, _Water) - o.SurfaceData.y);

            // Land
            o.SurfaceData.w = 1 - o.SurfaceData.z - o.SurfaceData.y;

            // Grass amount
            c.a = pow(saturate((_Grass - pow(c.g, _GrassPow) + 0.000001) / (_Grass + 0.000001)), _GrassCont)
                * (cos((IN.uv_SurfTex.y - 0.5) * 6 * 3.14159265358979323846) * _GrassLati * 0.5 + (1 - _GrassLati * 0.5));

            // Surface colour
            o.Albedo.rgb =
                fixed3(o.SurfaceData.y, o.SurfaceData.y, o.SurfaceData.y) // Cloud
                + _WaterCol.rgb * o.SurfaceData.z // Water
                + (_LandCol.rgb * (1 - c.a) + _GrassCol * c.a) // Land
                * o.SurfaceData.w * (_LandCol.a + c.r * (1 - _LandCol.a));

            // Night colour set
            o.Albedo.a = c.b * o.SurfaceData.w * saturate((c.g + _Night - 0.999999) / (_Night + 0.000001));

            // Cloud
            fixed4 cloud = tex2D(
                _CloudTex,
                fixed2(
                    IN.uv_CloudTex.x + _Time.x * _CloudRotation,
                    IN.uv_CloudTex.y
                )
            );
            cloud.a = saturate(_Cloud - 1);
            o.SurfaceData.x = cloud.r * (saturate(_Cloud) - cloud.a);

            // Cloud colour
            o.Albedo.rgb = fixed3(o.SurfaceData.x, o.SurfaceData.x, o.SurfaceData.x) + o.Albedo.rgb * (1 - o.SurfaceData.x);

            // Over cloudy
            fixed2 cloudPow = fixed2(pow(cloud.g, _OverCloudyWhite), pow(cloud.g, _OverCloudyBlue));
            o.Albedo.rgb = fixed3(cloudPow.x, cloudPow.x, cloudPow.y) * cloud.a + o.Albedo.rgb * (1 - cloud.a);
            o.SurfaceData.x += cloud.a;

            // Applying normalmap
            fixed4 normal;
            normal.xyz = UnpackNormal(tex2D(
                _IceNor,
                fixed2(
                    IN.uv_CloudTex.x + _Time.x * _SurfRotation,
                    IN.uv_CloudTex.y
                )
            ));
            normal.a = saturate(o.SurfaceData.y - o.SurfaceData.x);
            o.Normal = normal.xyz * normal.a + o.Normal * (1 - normal.a);

            o.Alpha = 1;
        }


        inline fixed4 LightingPlanetSurface(PlanetSurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed tAtten)
        {
            fixed4 result;

            // Lighting amount
            result.a = dot(s.Normal, lightDir);

            // Atmosphere
            fixed atmo = pow(saturate(1 - dot(s.Normal, viewDir)), _AtmoPow);

            // Planet colour
            result.rgb = _AtmoCol.rgb * _Atmosphere * atmo // Atmosphere
                + (s.Albedo.rgb // Surface
                + saturate(s.SurfaceData.z - s.SurfaceData.x) // Water specular
                * _WaterSpeCol.rgb
                * pow(
                    saturate(dot(s.Normal, normalize(lightDir + viewDir))),
                    _WaterSpeSprd
                ))
                * (1 - atmo); // Surface hidded behind the atmosphere

            // Day lighting
            result.rgb *= saturate(result.a);

            // Night lighting
            result.rgb += saturate(
                    pow(saturate(1 - result.a + _NightPos), _NightSmooth)
                    //cos(saturate(result.a * _NightSmooth + _NightPos) * 3.14159265358979323846)
                    - s.SurfaceData.x * saturate(_Cloud - 1)
                ) * s.Albedo.a * _NightCol * (1 - atmo);

            // Return
            result.a = 1;
            return result;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
