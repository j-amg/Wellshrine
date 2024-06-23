//from https://github.com/o3de/o3de/blob/development/Gems/Atom/Feature/Common/Assets/ShaderLib/Atom/Features/ParallaxMapping.azsli
shader_type spatial;

group_uniforms albedo;
uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D texture_albedo : source_color,filter_linear_mipmap,repeat_enable;
group_uniforms roughness;
uniform float roughness : hint_range(0.0, 1.0, 0.1) = 0.5;
uniform sampler2D texture_roughness : hint_default_white,filter_linear_mipmap,repeat_enable;
group_uniforms metallic;
uniform float metallic : hint_range(0.0, 1.0, 0.01) = 0;
uniform sampler2D texture_metallic : hint_default_white,filter_linear_mipmap,repeat_enable;
uniform float specular : hint_range(0.0, 1.0, 0.01) = 0.5;
group_uniforms normal_map;
uniform sampler2D texture_normal : hint_roughness_normal,filter_linear_mipmap,repeat_enable;
uniform float normal_scale : hint_range(-16,16) = 1;
group_uniforms ambient_occlusion;
uniform sampler2D texture_ao : hint_default_white,filter_linear_mipmap,repeat_enable;
uniform float ao_effect : hint_range(0.0, 1.0, 0.1) = 0.0;
group_uniforms height;
uniform sampler2D texture_heightmap : hint_default_black,filter_linear_mipmap,repeat_enable;
uniform float heightmap_scale : hint_range(0.0, 1.0, 0.01) = 0.5;
uniform int heightmap_layers : hint_range(8, 128, 1) = 32;
uniform float depthOffset : hint_range(0.0, 1.0, 0.1) = 0;
group_uniforms uv;
uniform vec2 uv1_scale = vec2(1.0);
uniform vec2 uv1_offset = vec2(0);

float GetNormalizedDepth(float startDepth, float stopDepth, float inverseDepthRange, vec2 uv) {
    float current_sample = 1.0 - texture(texture_heightmap, uv.xy).r;
    float normalizedDepth = 0.0;
    if(stopDepth - startDepth > 0.0001)
    {
        float minNormalizedDepth = -startDepth * inverseDepthRange;
        normalizedDepth = max(current_sample, minNormalizedDepth);
    }
    return normalizedDepth; 
}

#define PARALLAX_MULTIPLIER 0.65

void vertex() {
	UV=UV*uv1_scale+uv1_offset;
}

void fragment(){
	vec2 base_uv = UV;
	{
		vec3 dirToCameraTS = normalize(normalize(VIEW) * mat3(TANGENT, -BINORMAL, NORMAL));
		float height_scale = heightmap_scale * PARALLAX_MULTIPLIER;

	    float dirToCameraZInverse = 1.0 / dirToCameraTS.z;
	    float steps = 1.0 / float(heightmap_layers);
		float currentStep = 0.0;

	    // the amount to shift per step, shift in the inverse direction of dirToCameraTS
	    vec3 delta = -dirToCameraTS.xyz * height_scale * dirToCameraZInverse * steps;

	    float depthSearchStart = depthOffset;
	    float depthSearchEnd = depthSearchStart + height_scale;
	    float inverseDepthFactor = 1.0 / height_scale;

	    // This is the relative position at which we begin searching for intersection.
	    // It is adjusted according to the depthOffset, raising or lowering the whole surface by depthOffset units.
	    vec3 parallaxOffset = -dirToCameraTS.xyz * dirToCameraZInverse * depthOffset;

	    float currentSample = 1.0 - texture(texture_heightmap, base_uv + parallaxOffset.xy).r;
	    float prevSample;

	    // Do a basic search for the intersect step
	    while(currentSample > currentStep) {
	        currentStep += steps;
	        parallaxOffset += delta;

	        prevSample = currentSample;
	        currentSample = GetNormalizedDepth(depthSearchStart, depthSearchEnd, inverseDepthFactor, base_uv + parallaxOffset.xy);
	    }

	    if(currentStep > 0.0)
	    {
	        // Contact refinement propose by Andrea Riccardi 
	        // https://www.artstation.com/andreariccardi/blog/3VPo/a-new-approach-for-parallax-mapping-presenting-the-contact-refinement-parallax-mapping-technique

	        // Based on the rough approximation, rolling back to the previous step along the ray.
	        parallaxOffset -= delta;
	        currentStep -= steps;
	        currentSample = prevSample;

	        // Adjust precision
	        vec3 adjustedDelta = delta * steps;
	        float adjustedStep = steps * steps;

	        // Uses another loop with the same step numbers, this times only covers the distance between previous point and the rough intersection point.
	        while(currentSample > currentStep)
	        {
	            currentStep += adjustedStep;
	            parallaxOffset += adjustedDelta;
	            prevSample = currentSample;

	            currentSample = GetNormalizedDepth(depthSearchStart, depthSearchEnd, inverseDepthFactor, base_uv + parallaxOffset.xy);
	        }
	    }
	    if(parallaxOffset.z > 0.0)
	    {
	        parallaxOffset = vec3(0.0);
	    }
		
		base_uv += parallaxOffset.xy;
	}
	ALBEDO = albedo.rgb * texture(texture_albedo, base_uv).rgb;
	METALLIC = metallic * texture(texture_metallic, base_uv).r;
	SPECULAR = specular;
	ROUGHNESS = roughness * texture(texture_roughness, base_uv).r;
	AO = texture(texture_ao, base_uv).r;
	AO_LIGHT_AFFECT = ao_effect;
	NORMAL_MAP = texture(texture_normal, base_uv).rgb;
	NORMAL_MAP_DEPTH = normal_scale;
}