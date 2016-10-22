uniform sampler2D mainTex;
uniform sampler2D cloudNoiseTex;
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
	vec4 cloudNoiseTex = texture2D(cloudNoiseTex, gl_TexCoord[0].st);
	
	float sharpAlphaThreshold = clamp(1.0 * (abs(dFdx(gl_TexCoord[0].s)) + abs(dFdy(gl_TexCoord[0].t))), 0.0, 0.5);
	
	float density = texColor.r + texColor2.r;
	float brightness = clamp(0.5 + 0.5 * smoothstep(0.5, 1.7, density), 0, 1);
	float opacity = smoothstep(0.5 - sharpAlphaThreshold, 0.5 + sharpAlphaThreshold, density);
	float sideFactor = 1.0 - smoothstep(0.5, 0.95, density);
	
	opacity *= mix(0.4, 1.0, 1.0 - sideFactor) + 2.5 * max(0.0, sideFactor - 0.9);
	
	float cellularMatterDensity = (0.75 + 0.25 * cloudNoiseTex.r);
	gl_FragColor = vec4(mainColor.rgb * cellularMatterDensity * brightness, opacity * cellularMatterDensity);
}