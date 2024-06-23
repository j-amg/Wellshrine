shader_type sky;
uniform vec3 skyColour : source_color;

float random (vec2 uv) {
    return fract(sin(dot(uv.xy,
        vec2(12.9898,78.233))) * 43758.5453123);
}


void sky() {

		float col = fract(random(vec2(ceil(EYEDIR.x * 50.0 + TIME), ceil(EYEDIR.y * 50.0))));
		COLOR = vec3(col) * -EYEDIR.z;
	
}