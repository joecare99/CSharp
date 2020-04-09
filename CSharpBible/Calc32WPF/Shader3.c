void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    // Normalized pixel coordinates (from -1 to 1)
    vec2 uv = (fragCoord-.5*iResolution.xy)/iResolution.y;
	
	vec3 col = vec3(0);
	
	vec2 gv = fract( uv*5. );
	
	col.rg = gv;
	
	fragColor = vec4(col,1.);
}