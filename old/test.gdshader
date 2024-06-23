shader_type spatial;
//render_mode unshaded;
group_uniforms albedo;
uniform vec4 albedo : source_color = vec4(1.0);
uniform sampler2D texture_albedo : source_color,filter_nearest_mipmap,repeat_enable;
group_uniforms metallic;
uniform float metallic : hint_range(0.0, 1.0, 0.01) = 0;
uniform sampler2D texture_metallic : hint_default_white,filter_nearest_mipmap,repeat_enable;
uniform float specular : hint_range(0.0, 1.0, 0.01) = 0.5;
group_uniforms roughness;
uniform float roughness : hint_range(0,1) = 0.5;
uniform sampler2D texture_roughness : hint_roughness_r,filter_nearest_mipmap,repeat_enable;
group_uniforms ambient_occlusion;
uniform sampler2D texture_ao : hint_default_white,filter_nearest_mipmap,repeat_enable;
uniform float ao_effect : hint_range(0.0, 1.0, 0.1) = 0.0;
group_uniforms normal_map;
uniform sampler2D texture_normal : hint_roughness_normal,filter_nearest_mipmap,repeat_enable;
uniform float normal_scale : hint_range(-16,16) = 1;
group_uniforms uv;
uniform vec2 uv1_scale = vec2(1.0);

uniform float tiling = 1;
uniform float ratio = 1.0;
uniform vec2 tex_size = vec2(128.0, 128.0);

varying vec3 worldPos;

void vertex() {
	worldPos = mat3(MODEL_MATRIX) * -VERTEX;
	//worldPos -= NODE_POSITION_WORLD;
	UV=vec2(UV.x + worldPos.x, UV.y + worldPos.z);
}

void fragment() {
	
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_albedo,base_uv);
	//float ambient = 10.0;
	ALBEDO = albedo.rgb * albedo_tex.rgb;
	float metallic_tex = texture(texture_metallic,base_uv).r;
	METALLIC = metallic_tex * metallic;
	vec4 roughness_texture_channel = vec4(1.0,0.0,0.0,0.0);
	float roughness_tex = dot(texture(texture_roughness,base_uv),roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	SPECULAR = specular;
	AO = texture(texture_ao, base_uv).r;
	AO_LIGHT_AFFECT = ao_effect;
	NORMAL_MAP = texture(texture_normal,base_uv).rgb;
	NORMAL_MAP_DEPTH = normal_scale;
}

//void light(){
	////DIFFUSE_LIGHT = ALBEDO;
	//// add alpha based on attenuation, which includes if we're in shadow, cap at our max opacity
	////DIFFUSE_LIGHT = min(DIFFUSE_LIGHT + 0.25f*(1.0 - ATTENUATION), 1.0f);
	////DIFFUSE_LIGHT += LIGHT_COLOR * vec3(1,0,0);
	//vec3 L = normalize(LIGHT);
	//vec3 N = normalize(NORMAL);
	//vec3 V = normalize(VIEW);
	//vec3 light_dir = -L * tbn;
	//float NdotL = min(dot(N, L), 1.0);
	//float cNdotL = max(NdotL, 0.0);
	//float NdotV = dot(N, V);
	//float cNdotV = max(NdotV, 1e-4);
	//vec3 H = normalize(V + L);
	//float cLdotH = clamp(dot(L, H), 0.0, 1.0);
	//float cNdotH = clamp(dot(N, H), 0.0, 1.0);
	//float cLdotH5 = SchlickFresnel(cLdotH);
	//float shadow = GetParallaxShadow(base_uv, light_dir);
	//float diffuse_brdf_NL = cNdotL * (1.0 / PI); //built-in Godot lambert diffuse solution
	//float D = D_GGX(cNdotH, ROUGHNESS);
	//float G = V_GGX(cNdotL, cNdotV, ROUGHNESS);
	//vec3 f0 = F0(METALLIC, specular, ALBEDO);
	//float f90 = clamp(dot(f0, vec3(50.0 * 0.33)), METALLIC, 1.0);
	//vec3 F = f0 + (f90 - f0) * cLdotH5;
	//vec3 specular_brdf_NL = cNdotL * D * F * G; //built-in Godot specular SchlickGGX solution
	//
	//float atten;
	//
	//float d = length( light_dir );
	//atten = clamp( 10.0 / d, 0.0, 1.0);
//
	////if(use_self_shadow)
		////DIFFUSE_LIGHT += diffuse_brdf_NL * shadow * LIGHT_COLOR * (ATTENUATION);
	////else
//
	//DIFFUSE_LIGHT += diffuse_brdf_NL * LIGHT_COLOR;
	//SPECULAR_LIGHT += specular_brdf_NL * LIGHT_COLOR * SPECULAR_AMOUNT;
//
//
//
//
	////else
	////{
	//
	////}
//
//}