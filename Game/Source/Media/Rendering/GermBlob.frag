uniform sampler2D mainTex;
uniform sampler2D cloudNoiseTex;
uniform float TimeOffset;
uniform float GameTime;
uniform vec2 MoveDir;
uniform float ColorShift;
uniform vec4 FirstColor;
uniform vec4 SecondColor;

varying vec2 TexSampleOffset;

void main()
{
	float animTimer = GameTime + TimeOffset;
	
	vec4 mainColor = gl_Color;
	vec4 texColor = texture2D(mainTex, gl_TexCoord[0].st - TexSampleOffset * 0.5);
	vec4 texColor2 = texture2D(mainTex, gl_TexCoord[0].st + TexSampleOffset * 0.5);
	vec4 cloudNoiseTex = texture2D(cloudNoiseTex, gl_TexCoord[0].st - TexSampleOffset * 0.1);
	
	float sharpAlphaThreshold = clamp(1.0 * (abs(dFdx(gl_TexCoord[0].s)) + abs(dFdy(gl_TexCoord[0].t))), 0.0, 0.5);
	
	float density = texColor.r + texColor2.r;
	float brightness = 0.5 + 0.5 * smoothstep(0.5, 1.7, density);
	float opacity = smoothstep(0.5 - sharpAlphaThreshold, 0.5 + sharpAlphaThreshold, density);
	float sideFactor = 1.0 - smoothstep(0.5, 0.95, density);
	
	float sideHighlight = 2.5 * max(0.0, sideFactor - 0.9);
	opacity *= mix(0.4, 1.0, 1.0 - sideFactor) + sideHighlight;
	
	float colorShiftProgress = min(1.0, ColorShift * 2.0) * smoothstep(0.0, 0.2, cloudNoiseTex.r - 1.0 + ColorShift * 1.3 - sideHighlight);
	vec4 localColor = mix(FirstColor, SecondColor, colorShiftProgress);
	
	float cellularMatterDensity = (0.65 + 0.35 * cloudNoiseTex.r);
	gl_FragColor = vec4(mainColor.rgb * localColor.rgb * cellularMatterDensity * brightness + 0.25 * sideHighlight, opacity * cellularMatterDensity * localColor.a * mainColor.a);
}