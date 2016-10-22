<root dataType="Struct" type="Duality.Resources.VertexShader" id="129723834">
  <assetInfo dataType="Struct" type="Duality.Editor.AssetManagement.AssetInfo" id="427169525">
    <customData />
    <importerId dataType="String">BasicShaderAssetImporter</importerId>
    <sourceFileHint dataType="Array" type="System.String[]" id="1100841590">
      <item dataType="String">{Name}.vert</item>
    </sourceFileHint>
  </assetInfo>
  <source dataType="String">uniform float TimeOffset;
uniform float GameTime;
uniform vec2 MoveDir;

varying vec2 TexSampleOffset;

void main()
{
	float animTimer = GameTime + TimeOffset;
	
	vec2 moveOffset = 2.5 * MoveDir / (1 + length(MoveDir));
	vec2 organicOffset;
	organicOffset.x = 0.15 * (sin(animTimer * 1.2 + 9.2) + sin(animTimer * 1.5 + 12.6));
	organicOffset.y = 0.35 * sin(animTimer * 3);
	vec2 totalOffset = 
		1.5 * moveOffset + 
		organicOffset.x * (vec2(moveOffset.x, -moveOffset.y)) + 
		organicOffset.y * (moveOffset);
	
	TexSampleOffset = 0.6 * totalOffset / (1.0 + length(totalOffset));
	
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}</source>
</root>
<!-- XmlFormatterBase Document Separator -->
