<root dataType="Struct" type="Duality.Resources.FragmentShader" id="129723834">
  <assetInfo dataType="Struct" type="Duality.Editor.AssetManagement.AssetInfo" id="427169525">
    <customData />
    <importerId dataType="String">BasicShaderAssetImporter</importerId>
    <sourceFileHint dataType="Array" type="System.String[]" id="1100841590">
      <item dataType="String">{Name}.frag</item>
    </sourceFileHint>
  </assetInfo>
  <source dataType="String">uniform sampler2D mainTex;
uniform float TimeOffset;
uniform float GameTime;
uniform vec2 MoveDir;

varying vec2 TexSampleOffset;

void main()
{
	float animTimer = GameTime + TimeOffset;
	
	vec4 mainColor = gl_Color;
	vec4 texColor = texture2D(mainTex, gl_TexCoord[0].st - TexSampleOffset * 0.5);
	vec4 texColor2 = texture2D(mainTex, gl_TexCoord[0].st + TexSampleOffset * 0.5);
	
	float sharpAlphaThreshold = clamp(1.0 * (abs(dFdx(gl_TexCoord[0].s)) + abs(dFdy(gl_TexCoord[0].t))), 0.0, 0.5);
	
	float density = texColor.r + texColor2.r;
	float brightness = clamp(0.5 + 0.5 * smoothstep(0.5, 1.7, density), 0, 1);
	float opacity = smoothstep(0.5 - sharpAlphaThreshold, 0.5 + sharpAlphaThreshold, density);
	opacity *= 0.5 + 0.5 * smoothstep(0.5, 0.85, density);
	
	gl_FragColor = vec4(mainColor.rgb * brightness, opacity);
}</source>
</root>
<!-- XmlFormatterBase Document Separator -->
