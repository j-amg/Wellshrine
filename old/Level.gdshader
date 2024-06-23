shader_type spatial;
render_mode unshaded;
uniform vec3 playerPos;

varying vec3 worldPos;

void vertex() {
	// Called for every vertex the material is visible on.
	worldPos = mat3(MODEL_MATRIX) * -VERTEX;
	worldPos -= NODE_POSITION_WORLD;
}

const float mortarSize = .05;

void fragment(){
	vec3 pos = worldPos;
	
	//pos /= gridSize;
	//pos += gridSize * 20.0;
	//pos.y += 1.0*float(fract(float(int(pos.x*2.0))/2.0));
	//pos.z += float(fract(float(int(pos.y*2.0))/2.0));
	//vec3 col = vec3(fract(float(int(pos.z*2.0))/2.0));
	
	if (fract(pos.x) < (0.0 + mortarSize) || fract(pos.x) > (1.0 - mortarSize) || fract(pos.z) < (0.0 + mortarSize) || fract(pos.z) > (1.0 - mortarSize))
		ALBEDO = vec3(0,0,0);
	else
		ALBEDO = vec3(1,1,1);
	
	
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
	//LIGHT_COLOR = vec3(1,1,1);
//}
